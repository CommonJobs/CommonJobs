using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Infrastructure.RavenDb;
using CommonJobs.Domain;
using CommonJobs.JavaScript;

namespace CommonJobs.Application.Vacations
{
    public class CalculateVacations : ScriptCommand<VacationsReport>
    {
        public VacationsReportConfiguration Configuration { get; set; }
        public VacationsReportData Data { get; set; }

        protected override object[] GetParameters()
        {
            return new object[] { Data, Configuration};
        }

        protected override string[] GetDependencies()
        {
            return new[] { 
                "scripts\\json2.js", 
                "scripts\\underscore.js", 
                "scripts\\moment.js", 
                "scripts\\twix.js", 
                "CJLogic\\CJLogic.js", 
                "CJLogic\\CalculateVacations.js" 
            };
        }

        protected override string GetFunctionName()
        {
            return "CJLogic.CalculateVacations";
        }
    }
}
