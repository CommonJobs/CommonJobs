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

        [TestMethod()]
        public void IgnoreVacationsBeforHiring()
        {
            var target = new Employee();
            target.HiringDate = DateTime.Parse("2010-06-10T23:00:00");
            target.Vacations = new List<Vacation>() {
                new Vacation() { Period = 2009, From = DateTime.Parse("2012-01-01T20:00:00"), To = DateTime.Parse("2012-01-1T20:00:00") }
            };
            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                HiringDate = target.HiringDate,
                Vacations = target.Vacations
            };
            var calculated = calculator.Execute();
            var expected = 0;
            int actual = calculated.TotalTaken;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void HiredOnDecember()
        {
            var target = new Employee();

            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                HiringDate = DateTime.Parse("2011-12-15"),
                Vacations = new List<Vacation>(),
                Now = DateTime.Parse("2011-12-30")
            };

            var calculated = calculator.Execute();
            var expected = 1;
            int actual = calculated.TotalPending;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void HiredOnNovember()
        {
            var target = new Employee();

            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                HiringDate = DateTime.Parse("2011-11-15"),
                Vacations = new List<Vacation>(),
                Now = DateTime.Parse("2011-12-30")
            };

            var calculated = calculator.Execute();
            var expected = 2;
            int actual = calculated.TotalPending;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void HiredOnOctober()
        {
            var target = new Employee();

            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                HiringDate = DateTime.Parse("2011-10-15"),
                Vacations = new List<Vacation>(),
                Now = DateTime.Parse("2011-12-30")
            };

            var calculated = calculator.Execute();
            var expected = 3;
            int actual = calculated.TotalPending;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void HiredOnSeptember()
        {
            var target = new Employee();

            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                HiringDate = DateTime.Parse("2011-9-15"),
                Vacations = new List<Vacation>(),
                Now = DateTime.Parse("2011-12-30")
            };

            var calculated = calculator.Execute();
            var expected = 4;
            int actual = calculated.TotalPending;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void HiredOnAugust()
        {
            var target = new Employee();

            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                HiringDate = DateTime.Parse("2011-8-15"),
                Vacations = new List<Vacation>(),
                Now = DateTime.Parse("2011-12-30")
            };

            var calculated = calculator.Execute();
            var expected = 5;
            int actual = calculated.TotalPending;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void HiredOnJuly()
        {
            var target = new Employee();

            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                HiringDate = DateTime.Parse("2011-7-15"),
                Vacations = new List<Vacation>(),
                Now = DateTime.Parse("2011-12-30")
            };

            var calculated = calculator.Execute();
            var expected = 6;
            int actual = calculated.TotalPending;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void HiredOnJune()
        {
            var target = new Employee();

            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                HiringDate = DateTime.Parse("2011-6-15"),
                Vacations = new List<Vacation>(),
                Now = DateTime.Parse("2011-12-30")
            };

            var calculated = calculator.Execute();
            var expected = 14;
            int actual = calculated.TotalPending;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void HiredYearAgo()
        {
            var target = new Employee();

            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                HiringDate = DateTime.Parse("2010-3-15"),
                Vacations = new List<Vacation>(),
                Now = DateTime.Parse("2011-6-15")
            };

            var calculated = calculator.Execute();
            var expected = 28;
            int actual = calculated.TotalPending;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void HiredTwoYearsAgo()
        {
            var target = new Employee();

            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                HiringDate = DateTime.Parse("2009-3-15"),
                Vacations = new List<Vacation>(),
                Now = DateTime.Parse("2011-6-15")
            };

            var calculated = calculator.Execute();
            var expected = 42;
            int actual = calculated.TotalPending;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void HiredThreeYearsAgo()
        {
            var target = new Employee();

            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                HiringDate = DateTime.Parse("2008-3-15"),
                Vacations = new List<Vacation>(),
                Now = DateTime.Parse("2011-6-15")
            };

            var calculated = calculator.Execute();
            var expected = 56;
            int actual = calculated.TotalPending;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void HiredFourYearsAgo()
        {
            var target = new Employee();

            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                HiringDate = DateTime.Parse("2007-3-15"),
                Vacations = new List<Vacation>(),
                Now = DateTime.Parse("2011-6-15")
            };

            var calculated = calculator.Execute();
            var expected = 70;
            int actual = calculated.TotalPending;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void HiredFiveYearsAgo()
        {
            var target = new Employee();

            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                HiringDate = DateTime.Parse("2006-3-15"),
                Vacations = new List<Vacation>(),
                Now = DateTime.Parse("2011-6-15")
            };

            var calculated = calculator.Execute();
            var expected = 91;
            int actual = calculated.TotalPending;
            Assert.AreEqual(expected, actual);
        }

    }
}
