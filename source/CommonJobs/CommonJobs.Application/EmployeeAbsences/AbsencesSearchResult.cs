﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonJobs.Domain;

namespace CommonJobs.Application.EmployeeAbsences
{
    public class AbsencesSearchResult
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? HiringDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public List<Absence> Absences { get; set; }
    }
}