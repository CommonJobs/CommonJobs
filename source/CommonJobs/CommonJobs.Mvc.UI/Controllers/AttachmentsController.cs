using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Raven.Mvc;
using CommonJobs.Infrastructure.AttachmentStorage;
using CommonJobs.Domain;
using System.Diagnostics;
using Raven.Client.Linq;
using CommonJobs.Mvc.UI.Models;
using CommonJobs.Infrastructure.Indexes;
using CommonJobs.Infrastructure.AttachmentIndexing;
using RavenData = Raven.Abstractions.Data;
using Raven.Json.Linq;

namespace CommonJobs.Mvc.UI.Controllers
{
    public class AttachmentsController : CommonJobsController
    {
        //TODO: permitir no usar los nombres de las acciones
        [HttpGet]
        public ActionResult Get(string id, bool returnName = true)
        {
            var attachment = RavenSession.Load<Attachment>(id);
            if (attachment == null)
                return HttpNotFound();

            var stream = Query(new ReadAttachment(attachment));
            if (stream == null)
                return HttpNotFound();

            if (returnName)
                return File(stream, attachment.ContentType, attachment.FileName);
            else
                return File(stream, attachment.ContentType);                
        }

        [HttpPost]
        [Authorize]
        public ActionResult Post(string id)
        {
            var entity = RavenSession.Load<object>(id);
            if (entity == null)
                return HttpNotFound("Specified entity does not exists");
            
            var attachmentReader = new RequestAttachmentReader(Request);
            var attachment = ExecuteCommand(new SaveAttachment(
                entity,
                attachmentReader.FileName,
                attachmentReader.Stream));

            return Json(new { success = true, attachment = attachment });
        }

        [Authorize]
        public ActionResult CleanAttachmentIndexInformation()
        {
            //TODO: revisar si funciona ilimitados documentos
            RavenSession.Advanced.DatabaseCommands.UpdateByIndex(
                "Attachments/ByContentExtractorConfigurationHash",
                new RavenData.IndexQuery() { Query = "ContentExtractorConfigurationHash:*" },
                new[] {
                    new RavenData.PatchRequest()
                    {
                        Type = RavenData.PatchCommandType.Set,
                        Name = "ContentExtractorConfigurationHash",
                        Value = RavenJValue.Null
                    },
                    new RavenData.PatchRequest()
                    {
                        Type = RavenData.PatchCommandType.Set,
                        Name = "PlainContent",
                        Value = RavenJValue.Null
                    }
                },
                allowStale: false);

            return Json(new { ok = true });
        }

        [Authorize]
        public ActionResult IndexAttachments(int quantity = 10)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Attachment[] attachments = Query(new GetNotIndexedAttachments(quantity));
            //TODO: indexar cada uno
            foreach (var attachment in attachments)
                ExecuteCommand(new IndexAttachment(attachment));

            stopwatch.Stop();
            return Json(new
            {
                elapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                indexItems = attachments.Length
            });
        }

        [Authorize]
        public ActionResult AttachmentsQuickSearch(AttachmentSearchModel searchModel)
        {
            //TODO: agregar soporte para filtrar por tipo de archivos o con comodines en el nombre y para filtrar por id de usuario
            RavenQueryStatistics statistics;
            var query = RavenSession.Query<Attachments_QuickSearch.Result, Attachments_QuickSearch>()
                .Statistics(out statistics)
                .Skip(searchModel.Skip)
                .Take(searchModel.Take);

            if (!string.IsNullOrWhiteSpace(searchModel.Term))
                query = query.Where(x => x.PlainContent.StartsWith(searchModel.Term) || x.FileName.StartsWith(searchModel.Term));

            if (searchModel.Orphans == AttachmentSearchModel.OrphansMode.NoOrphans)
                query = query.Where(x => !x.IsOrphan);
            else if (searchModel.Orphans == AttachmentSearchModel.OrphansMode.OnlyOrphans)
                query = query.Where(x => x.IsOrphan);

            var results = query
                //To do not bring PlainContent (big field)
                .Select(x => new 
                { 
                    x.AttachmentId, 
                    x.ContentType, 
                    x.FileName,
                    x.RelatedEntityId,
                    x.IsOrphan
                })
                .ToArray();

            return Json(new { Statistics = statistics, Results = results });
        }
    }
}
