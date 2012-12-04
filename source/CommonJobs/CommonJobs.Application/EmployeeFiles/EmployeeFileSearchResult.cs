using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.EmployeeFiles
{
    public class EmployeeFileSearchResult
    {
        public string Id { get; set; }
        public string Platform { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FileId { get; set; }
        public string Cuil { get; set; }
        public DateTime? HiringDate { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }
        public string BankAccount { get; set; }
        public string UniqueBankCode { get; set; }
        public string HealthInsurance { get; set; }
        public string Agreement { get; set; }
        public string CurrentProject { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
