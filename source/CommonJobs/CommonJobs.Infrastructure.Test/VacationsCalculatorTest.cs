using CommonJobs.Infrastructure.Vacations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CommonJobs.Domain;
using CommonJobs.JavaScript;
using System.Collections.Generic;

namespace CommonJobs.Infrastructure.Test
{
    [TestClass()]
    [DeploymentItem("Scripts\\json2.js", "Scripts")]
    [DeploymentItem("Scripts\\underscore.js", "Scripts")]
    [DeploymentItem("Scripts\\moment.js", "Scripts")]
    [DeploymentItem("Scripts\\twix.js", "Scripts")]
    [DeploymentItem("CJLogic\\CJLogic.CalculateVacations.js", "CJLogic")]
    public class VacationsCalculatorTest
    {
        [TestMethod()]
        public void TotalDaysTest_StartAndEndDateIncluded()
        {
            var target = new Employee();
            target.HiringDate = DateTime.Parse("2010-06-10T23:00:00");
            target.Vacations = new List<Vacation>() {
                new Vacation() { Period = 2010, From = DateTime.Parse("2012-01-01T00:00:00"), To = DateTime.Parse("2012-01-10T00:00:00") }
            };
            var calculator = new CalculateVacations() {
                Context = new ScriptContext(),
                Employee = target
            };
            var calculated = calculator.Execute();
            var expected = 10;
            int actual = calculated.TotalTaken;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void TotalDaysTest_DoNotCareAboutHour()
        {
            var target = new Employee();
            target.HiringDate = DateTime.Parse("2010-06-10T23:00:00");
            target.Vacations = new List<Vacation>() {
                new Vacation() { Period = 2010, From = DateTime.Parse("2012-01-01T23:00:00"), To = DateTime.Parse("2012-01-10T00:00:00") }
            };
            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                Employee = target
            };
            var calculated = calculator.Execute();
            var expected = 10;
            int actual = calculated.TotalTaken;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void TotalDaysTest_SameDayOneDay()
        {
            var target = new Employee();
            target.HiringDate = DateTime.Parse("2010-06-10T23:00:00");
            target.Vacations = new List<Vacation>() {
                new Vacation() { Period = 2010, From = DateTime.Parse("2012-01-01T20:00:00"), To = DateTime.Parse("2012-01-1T20:00:00") }
            };
            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                HiringDate = target.HiringDate,
                Vacations = target.Vacations
            };
            var calculated = calculator.Execute();
            var expected = 1;
            int actual = calculated.TotalTaken;
            Assert.AreEqual(expected, actual);
        }
    }
}
