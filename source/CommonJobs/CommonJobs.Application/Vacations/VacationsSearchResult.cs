using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonJobs.Domain;

namespace CommonJobs.Application.Vacations
{
    public class VacationsSearchResult
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Vacation> Vacations { get; set; }
        public DateTime? HiringDate { get; set; }
    }
}