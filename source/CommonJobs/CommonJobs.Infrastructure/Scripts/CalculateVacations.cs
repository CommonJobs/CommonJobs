﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Raven.Infrastructure;
using CommonJobs.Domain;
using CommonJobs.JavaScript;

namespace CommonJobs.Infrastructure.Scripts
{
    public class CalculateVacations : ScriptCommand<CalculatedVacations>
    {
        public DateTime? HiringDate { get; set; }
        public IEnumerable<Vacation> Vacations { get; set; }
        public Employee Employee
        {
            set
            {
                HiringDate = value.HiringDate;
                Vacations = value.Vacations;
            }
        }

        protected override object[] GetParameters()
        {
            return new object[] { HiringDate, Vacations };
        }

        public CalculateVacations()
            : base("CJLogic", "CalculateVacations", "underscore.js", "moment.js", "twix.js")
        {
        }
    }
}
