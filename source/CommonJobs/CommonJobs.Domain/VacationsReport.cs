using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain
{
    public class VacationsReportConfiguration
    {
        public int CurrentYear { get; set; }
        public int DetailedYearsQuantity { get; set; }
    }

    public class VacationsReportData
    {
        public DateTime? HiringDate { get; set; }
        public Vacation[] Vacations { get; set; }

        public static VacationsReportData FromEmployee(Employee employee) 
        {
            return new VacationsReportData()
            {
                HiringDate = employee.HiringDate,
                Vacations = employee.Vacations.ToArray()
            };
        }
    }

    public class VacationsReport
    {
        public int TotalEarned { get; set; }
        public int TotalTaken { get; set; }
        public int TotalPending { get; set; }
        public Dictionary<int, VacationsPeriodReport> ByYear { get; set; }
        public VacationsPeriodReport Older { get; set; }
        public VacationsPeriodReport InAdvance { get; set; }
    }

    public class VacationsPeriodReport
    {
        public int Earned { get; set; }
        public int Taken { get; set; }
        public Vacation[] Detail { get; set; }
    }
}
