using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Domain;
using CommonJobs.Raven.Mvc;

namespace CommonJobs.Mvc.PublicUI.Controllers
{
    public class PostulationsController : CommonJobsController
    {
        public ActionResult Create()
        {
            return View();
        } 

        private TemporalFileReference GenerateFileReference(HttpPostedFileBase file)
        {
            var temporalFolderPath = System.Configuration.ConfigurationManager.AppSettings["CommonJobs/TemporalUploadsPath"];
            var internalFileName = Guid.NewGuid().ToString();
            var temporalFullName = Path.Combine(temporalFolderPath, internalFileName);
            file.SaveAs(temporalFullName);
            return new TemporalFileReference() 
            {
                OriginalFileName = file.FileName, //TODO: verify in different browsers if it is only the last part of the name
                InternalFileName = internalFileName
            };
        }


        [HttpPost]
        public ActionResult Create(Postulation postulation, HttpPostedFileBase curriculumFile)
        {
            try
            {
                //Validate
                postulation.Curriculum = GenerateFileReference(curriculumFile);
                // TODO: Add insert logic here
                //Convert to Applicant, asyncrlonusly?
                return RedirectToAction("index", "Home");
            }
            catch
            {
                return View();
            }
        }
    }
}
