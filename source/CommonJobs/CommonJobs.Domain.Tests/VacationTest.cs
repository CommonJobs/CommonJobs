using CommonJobs.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CommonJobs.Domain.Tests
{
    
    
    /// <summary>
    ///This is a test class for VacationTest and is intended
    ///to contain all VacationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class VacationTest
    {
        [TestMethod()]
        public void TotalDaysTest_StartAndEndDateIncluded()
        {
            Vacation target = new Vacation() { From = DateTime.Parse("2012-01-01T00:00:00"), To = DateTime.Parse("2012-01-10T00:00:00") };
            var expected = 10;
            int actual = target.TotalDays;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void TotalDaysTest_DoNotCareAboutHour()
        {
            Vacation target = new Vacation() { From = DateTime.Parse("2012-01-01T23:00:00"), To = DateTime.Parse("2012-01-10T00:00:00") };
            var expected = 10;
            int actual = target.TotalDays;
            Assert.AreEqual(expected, actual);
        }
    }
}
