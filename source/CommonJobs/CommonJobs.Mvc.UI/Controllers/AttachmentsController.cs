using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Infrastructure.Mvc;
using CommonJobs.Application.AttachmentStorage;
using CommonJobs.Domain;
using System.Diagnostics;
using Raven.Client.Linq;
using CommonJobs.Application.Indexes;
using CommonJobs.Application.AttachmentIndexing;
using RavenData = Raven.Abstractions.Data;
using Raven.Json.Linq;
using System.Drawing;
using System.IO;
using CommonJobs.Application.AttachmentSearching;
using System.Text.RegularExpressions;
using CommonJobs.Mvc.UI.Infrastructure;
using CommonJobs.Utilities;

namespace CommonJobs.Mvc.UI.Controllers
{
    [Documentation("manual-de-usuario/archivos")]
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

        [CommonJobsAuthorize(Roles = "Users,ApplicantManagers,EmployeeManagers")]
        public ActionResult CropImageAttachment(string id, int x, int y, int width, int height)
        {
            // get image
            var attachment = RavenSession.Load<Attachment>(id);
            if (attachment == null)
                return HttpNotFound();

            var stream = Query(new ReadAttachment(attachment));
            if (stream == null)
                return HttpNotFound();

            // crop it
            var image = new Bitmap(stream);

            var destArea = new Rectangle(0, 0, 100, 100);
            var srcArea = new Rectangle(x, y, width, height);

            var destImage = new Bitmap(destArea.Width, destArea.Height);
            var gfx = Graphics.FromImage(destImage);
            gfx.DrawImage(image, destArea, srcArea, GraphicsUnit.Pixel);
            
            // save it
            var relatedEntity = RavenSession.Load<object>(attachment.RelatedEntityId);
            if (relatedEntity == null)
                return new HttpStatusCodeResult(500, "Related entity not found.");

            var ms = new MemoryStream();
            destImage.Save(ms, image.RawFormat);

            var newImage = ExecuteCommand(new SavePhotoAttachments(relatedEntity, attachment.FileName, ms));
            return Json(newImage, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CommonJobsAuthorize(Roles = "Users,ApplicantManagers,EmployeeManagers")]
        public ActionResult Post(string id)
        {
            var entity = RavenSession.Load<object>(id);
            if (entity == null)
                return HttpNotFound("Specified entity does not exists");

            using (var attachmentReader = new RequestAttachmentReader(Request))
            {
                var attachments = attachmentReader
                    .Select(x => ExecuteCommand(new SaveAttachment(entity, x.Key, x.Value)))
                    .ToArray();

                return Json(new { success = true, attachment = attachments.FirstOrDefault(), attachments = attachments });
            }
        }

        [CommonJobsAuthorize(Roles = "Migrators")]
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

        [CommonJobsAuthorize(Roles = "Migrators")]
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

        [CommonJobsAuthorize(Roles = "Users")]
        public ActionResult AttachmentsQuickSearch(AttachmentSearchParameters searchParameters)
        {
            var query = new SearchAttachments(searchParameters);
            var results = Query(query);
            FlattenTextExtract(results);
            return Json(new
            {
                Items = results,
                Skipped = searchParameters.Skip,
                TotalResults = query.Stats.TotalResults
            });
        }

        public void FlattenTextExtract(IEnumerable<AttachmentSearchResult> results)
        {
            var replaceDots = new Regex("\\.\\.+");
            var replaceLines = new Regex("\r\n|\n|\r");
            foreach (var result in results)
            {
                result.PartialText = result.PartialText.Replace("<", "&lt;");
                result.PartialText = result.PartialText.Replace(">", "&gt;");
                //TODO: hablabamos de que tal vez era una buena idea remover todos los saltos de línea, pero aún queda muy feo, habría que revisar los estilos
                //result.PartialText = replaceLines.Replace(result.PartialText, " ");
                result.PartialText = replaceDots.Replace(result.PartialText, ".");
            }
        }

        [CommonJobsAuthorize(Roles = "Users")]
        public ActionResult Index(AttachmentSearchParameters searchParameters)
        {
            return View(searchParameters ?? new AttachmentSearchParameters());
        }
    }
}
