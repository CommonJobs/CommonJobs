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
using CommonJobs.Infrastructure.Indexes;
using CommonJobs.Infrastructure.AttachmentIndexing;
using RavenData = Raven.Abstractions.Data;
using Raven.Json.Linq;
using System.Drawing;
using System.IO;
using CommonJobs.Infrastructure.AttachmentSearching;

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
        public ActionResult AttachmentsQuickSearch(AttachmentSearchParameters searchParameters)
        {
            var query = new SearchAttachments(searchParameters);
            var results = Query(query);
            return Json(new
            {
                Items = results,
                Skiped = searchParameters.Skip,
                TotalResults = query.Stats.TotalResults
            });
        }

        [Authorize]
        public ActionResult Index(AttachmentSearchParameters searchParameters)
        {
            return View(searchParameters ?? new AttachmentSearchParameters());
        }
    }
}
