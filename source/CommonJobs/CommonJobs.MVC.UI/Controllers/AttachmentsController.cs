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

            var stream = Query(new ReadAttachment() 
            { 
                Attachment = attachment, 
                UploadPath = CommonJobs.Mvc.UI.Properties.Settings.Default.UploadPath 
            });
            if (stream == null)
                return HttpNotFound();

            if (returnName)
                return File(stream, attachment.ContentType, attachment.FileName);
            else
                return File(stream, attachment.ContentType);                
        }

        [HttpPost]
        public ActionResult Post(string id)
        {
            var entity = RavenSession.Load<object>(id);
            if (entity == null)
                return HttpNotFound("Specified entity does not exists");
            
            var attachmentReader = new RequestAttachmentReader(Request);
            var attachment = ExecuteCommand(new SaveAttachment()
            {
                UploadPath = CommonJobs.Mvc.UI.Properties.Settings.Default.UploadPath,
                RelatedEntity = entity,
                FileName = attachmentReader.FileName,
                Stream = attachmentReader.Stream
            });

            return Json(new { success = true, attachment = attachment });
        }

        [Obsolete("This action should be executed automatically by the system")]
        public ActionResult IndexAttachments(int quantity = 10)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Attachment[] attachments = Query(new GetNotIndexedAttachments() { Quantity = quantity });
            //TODO: indexar cada uno
            foreach (var attachment in attachments)
                ExecuteCommand(new IndexAttachment() { Attachment = attachment });

            stopwatch.Stop();
            return Json(new
            {
                elapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                indexItems = attachments.Length
            });
        }

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
