using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Raven.Infrastructure;
using CommonJobs.Domain;
using CommonJobs.JavaScript;

namespace CommonJobs.Infrastructure.Vacations
{
    public class CalculateVacations : ScriptCommand<VacationsReport>
    {
        public VacationsReportConfiguration Configuration { get; set; }
        public VacationsReportData Data { get; set; }

        protected override object[] GetParameters()
        {
            return new object[] { Data, Configuration};
        }

        public CalculateVacations()
            : base("CJLogic", "CalculateVacations", 
                "underscore.js", "moment.js", "twix.js")
        {
        }
    }
}
