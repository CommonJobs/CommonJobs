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
    [DeploymentItem("CJLogic\\CJLogic.js", "CJLogic")]
    [DeploymentItem("CJLogic\\CalculateVacations.js", "CJLogic")]
    public class VacationsCalculatorTest
    {
        [TestMethod()]
        public void VacationScriptTest()
        {
            string r;
            var context = new ScriptContext();
            context.ImportDependencies("json2.js", "underscore.js", "moment.js", "twix.js");
            context.Run(@"
properties = function(obj) { 
    var result = '';
    for (var i in obj) {
        result += (' ' +  i);
    }
    return result;
};");
            r = context.Eval<string>("properties(CJLogic)");
        }


        [TestMethod()]
        public void MoreThanOneVacationInYear()
        {
            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                Configuration = new VacationsReportConfiguration()
                {
                    CurrentYear = 2012,
                    DetailedYearsQuantity = 5
                },
                Data = new VacationsReportData()
                {
                    HiringDate = DateTime.Parse("2012-04-07"),
                    Vacations = new[] { 
                        new Vacation() { 
                            Period = 2012, 
                            From = DateTime.Parse("2012-08-31T00:00:00"), 
                            To = DateTime.Parse("2012-09-01T00:00:00") 
                        },
                        new Vacation() { 
                            Period = 2012, 
                            From = DateTime.Parse("2012-08-01T00:00:00"), 
                            To = DateTime.Parse("2012-08-02T00:00:00") 
                        },
                        new Vacation() { 
                            Period = 2013, 
                            From = DateTime.Parse("2012-10-1T00:00:00"), 
                            To = DateTime.Parse("2012-10-03T00:00:00") 
                        },
                        new Vacation() { 
                            Period = 2013, 
                            From = DateTime.Parse("2012-12-1T00:00:00"), 
                            To = DateTime.Parse("2012-12-03T00:00:00") 
                        },
                        new Vacation() { 
                            Period = 2015, 
                            From = DateTime.Parse("2012-11-1T00:00:00"), 
                            To = DateTime.Parse("2012-11-03T00:00:00") 
                        },
                    }
                }
            };
            var calculated = calculator.Execute();
            var expected = 4;
            int actual = calculated.ByYear[2012].Taken;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void InAdvanceTaken()
        {
            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                Configuration = new VacationsReportConfiguration()
                {
                    CurrentYear = 2012,
                    DetailedYearsQuantity = 5
                },
                Data = new VacationsReportData()
                {
                    HiringDate = DateTime.Parse("2012-04-07"),
                    Vacations = new[] { 
                        new Vacation() { 
                            Period = 2012, 
                            From = DateTime.Parse("2012-08-31T00:00:00"), 
                            To = DateTime.Parse("2012-09-01T00:00:00") 
                        },
                        new Vacation() { 
                            Period = 2012, 
                            From = DateTime.Parse("2012-08-01T00:00:00"), 
                            To = DateTime.Parse("2012-08-02T00:00:00") 
                        },
                        new Vacation() { 
                            Period = 2013, 
                            From = DateTime.Parse("2012-10-1T00:00:00"), 
                            To = DateTime.Parse("2012-10-03T00:00:00") 
                        },
                        new Vacation() { 
                            Period = 2013, 
                            From = DateTime.Parse("2012-12-1T00:00:00"), 
                            To = DateTime.Parse("2012-12-03T00:00:00") 
                        },
                        new Vacation() { 
                            Period = 2015, 
                            From = DateTime.Parse("2012-11-1T00:00:00"), 
                            To = DateTime.Parse("2012-11-03T00:00:00") 
                        },
                    }
                }
            };
            var calculated = calculator.Execute();
            var expected = 9;
            int actual = calculated.InAdvance.Taken;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Pending()
        {
            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                Configuration = new VacationsReportConfiguration()
                {
                    CurrentYear = 2012,
                    DetailedYearsQuantity = 5
                },
                Data = new VacationsReportData()
                {
                    HiringDate = DateTime.Parse("2011-04-07"),
                    Vacations = new[] { 
                        new Vacation() { 
                            Period = 2011, 
                            From = DateTime.Parse("2012-08-31T00:00:00"), 
                            To = DateTime.Parse("2012-09-01T00:00:00") 
                        },
                        new Vacation() { 
                            Period = 2012, 
                            From = DateTime.Parse("2012-08-01T00:00:00"), 
                            To = DateTime.Parse("2012-08-02T00:00:00") 
                        },
                        new Vacation() { 
                            Period = 2013, 
                            From = DateTime.Parse("2012-10-1T00:00:00"), 
                            To = DateTime.Parse("2012-10-03T00:00:00") 
                        },
                        new Vacation() { 
                            Period = 2013, 
                            From = DateTime.Parse("2012-12-1T00:00:00"), 
                            To = DateTime.Parse("2012-12-03T00:00:00") 
                        },
                        new Vacation() { 
                            Period = 2015, 
                            From = DateTime.Parse("2012-11-1T00:00:00"), 
                            To = DateTime.Parse("2012-11-03T00:00:00") 
                        },
                    }
                }
            };
            var calculated = calculator.Execute();
            var expected = 15;
            int actual = calculated.TotalPending;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void OlderVacationsPending()
        {
            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                Configuration = new VacationsReportConfiguration()
                {
                    CurrentYear = 2012,
                    DetailedYearsQuantity = 0
                },
                Data = new VacationsReportData()
                {
                    HiringDate = DateTime.Parse("2011-04-07"),
                    Vacations = new[] { 
                        new Vacation() { 
                            Period = 2011, 
                            From = DateTime.Parse("2012-08-31T00:00:00"), 
                            To = DateTime.Parse("2012-09-01T00:00:00") 
                        },
                        new Vacation() { 
                            Period = 2012, 
                            From = DateTime.Parse("2012-08-01T00:00:00"), 
                            To = DateTime.Parse("2012-08-02T00:00:00") 
                        },
                        new Vacation() { 
                            Period = 2013, 
                            From = DateTime.Parse("2012-10-1T00:00:00"), 
                            To = DateTime.Parse("2012-10-03T00:00:00") 
                        },
                        new Vacation() { 
                            Period = 2013, 
                            From = DateTime.Parse("2012-12-1T00:00:00"), 
                            To = DateTime.Parse("2012-12-03T00:00:00") 
                        },
                        new Vacation() { 
                            Period = 2015, 
                            From = DateTime.Parse("2012-11-1T00:00:00"), 
                            To = DateTime.Parse("2012-11-03T00:00:00") 
                        },
                    }
                }
            };
            var calculated = calculator.Execute();
            var expected = 15;
            int actual = calculated.TotalPending;
            Assert.AreEqual(expected, actual);
        }


        [TestMethod()]
        public void TotalDaysTest_StartAndEndDateIncluded()
        {
            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                Configuration = new VacationsReportConfiguration()
                {
                    CurrentYear = 2012,
                    DetailedYearsQuantity = 5
                },
                Data = new VacationsReportData()
                {
                    HiringDate = DateTime.Parse("2010-06-10T23:00:00"),
                    Vacations = new[] { new Vacation() { 
                        Period = 2010, 
                        From = DateTime.Parse("2012-01-01T00:00:00"), 
                        To = DateTime.Parse("2012-01-10T00:00:00") } }
                }
            };
            var calculated = calculator.Execute();
            var expected = 10;
            int actual = calculated.TotalTaken;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void TotalDaysTest_DoNotCareAboutHour()
        {
            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                Configuration = new VacationsReportConfiguration()
                {
                    CurrentYear = 2012,
                    DetailedYearsQuantity = 5
                },
                Data = new VacationsReportData()
                {
                    HiringDate = DateTime.Parse("2010-06-10T23:00:00"),
                    Vacations = new[] { new Vacation() { 
                        Period = 2010, 
                        From = DateTime.Parse("2012-01-01T23:00:00"), 
                        To = DateTime.Parse("2012-01-10T00:00:00") } }
                }
            };
            var calculated = calculator.Execute();
            var expected = 10;
            int actual = calculated.TotalTaken;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void TotalDaysTest_SameDayOneDay()
        {
            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                Configuration = new VacationsReportConfiguration()
                {
                    CurrentYear = 2012,
                    DetailedYearsQuantity = 5
                },
                Data = new VacationsReportData()
                {
                    HiringDate = DateTime.Parse("2010-06-10T23:00:00"),
                    Vacations = new[] { new Vacation() { 
                        Period = 2010, 
                        From = DateTime.Parse("2012-01-01T20:00:00"), 
                        To = DateTime.Parse("2012-01-1T20:00:00") } }
                }
            };
            var calculated = calculator.Execute();
            var expected = 1;
            int actual = calculated.TotalTaken;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void IgnoreVacationsBeforHiring()
        {
            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                Configuration = new VacationsReportConfiguration()
                {
                    CurrentYear = 2012,
                    DetailedYearsQuantity = 5
                },
                Data = new VacationsReportData()
                {
                    HiringDate = DateTime.Parse("2010-06-10T23:00:00"),
                    Vacations = new[] { new Vacation() { 
                        Period = 2009, 
                        From = DateTime.Parse("2012-01-01T20:00:00"), 
                        To = DateTime.Parse("2012-01-1T20:00:00") } }
                }
            };
            var calculated = calculator.Execute();
            var expected = 0;
            int actual = calculated.TotalTaken;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void HiredOnDecember()
        {
            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                Configuration = new VacationsReportConfiguration()
                {
                    CurrentYear = 2011,
                    DetailedYearsQuantity = 5
                },
                Data = new VacationsReportData()
                {
                    HiringDate = DateTime.Parse("2011-12-15"),
                    Vacations = new Vacation[0]
                }
            };

            var calculated = calculator.Execute();
            var expected = 1;
            int actual = calculated.TotalPending;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void HiredOnNovember()
        {
            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                Configuration = new VacationsReportConfiguration()
                {
                    CurrentYear = 2011,
                    DetailedYearsQuantity = 5
                },
                Data = new VacationsReportData()
                {
                    HiringDate = DateTime.Parse("2011-11-15"),
                    Vacations = new Vacation[0]
                }
            };

            var calculated = calculator.Execute();
            var expected = 2;
            int actual = calculated.TotalPending;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void HiredOnOctober()
        {
            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                Configuration = new VacationsReportConfiguration()
                {
                    CurrentYear = 2011,
                    DetailedYearsQuantity = 5
                },
                Data = new VacationsReportData()
                {
                    HiringDate = DateTime.Parse("2011-10-15"),
                    Vacations = new Vacation[0]
                }
            };

            var calculated = calculator.Execute();
            var expected = 3;
            int actual = calculated.TotalPending;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void HiredOnSeptember()
        {
            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                Configuration = new VacationsReportConfiguration()
                {
                    CurrentYear = 2011,
                    DetailedYearsQuantity = 5
                },
                Data = new VacationsReportData()
                {
                    HiringDate = DateTime.Parse("2011-9-15"),
                    Vacations = new Vacation[0]
                }
            };

            var calculated = calculator.Execute();
            var expected = 4;
            int actual = calculated.TotalPending;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void HiredOnAugust()
        {
            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                Configuration = new VacationsReportConfiguration()
                {
                    CurrentYear = 2011,
                    DetailedYearsQuantity = 5
                },
                Data = new VacationsReportData()
                {
                    HiringDate = DateTime.Parse("2011-8-15"),
                    Vacations = new Vacation[0]
                }
            };

            var calculated = calculator.Execute();
            var expected = 5;
            int actual = calculated.TotalPending;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void HiredOnJuly()
        {
            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                Configuration = new VacationsReportConfiguration()
                {
                    CurrentYear = 2011,
                    DetailedYearsQuantity = 5
                },
                Data = new VacationsReportData()
                {
                    HiringDate = DateTime.Parse("2011-7-15"),
                    Vacations = new Vacation[0]
                }
            };

            var calculated = calculator.Execute();
            var expected = 6;
            int actual = calculated.TotalPending;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void HiredOnJune()
        {
            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                Configuration = new VacationsReportConfiguration()
                {
                    CurrentYear = 2011,
                    DetailedYearsQuantity = 5
                },
                Data = new VacationsReportData()
                {
                    HiringDate = DateTime.Parse("2011-6-15"),
                    Vacations = new Vacation[0]
                }
            };

            var calculated = calculator.Execute();
            var expected = 14;
            int actual = calculated.TotalPending;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void HiredYearAgo()
        {
            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                Configuration = new VacationsReportConfiguration()
                {
                    CurrentYear = 2011,
                    DetailedYearsQuantity = 5
                },
                Data = new VacationsReportData()
                {
                    HiringDate = DateTime.Parse("2010-3-15"),
                    Vacations = new Vacation[0]
                }
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
                Configuration = new VacationsReportConfiguration()
                {
                    CurrentYear = 2011,
                    DetailedYearsQuantity = 5
                },
                Data = new VacationsReportData()
                {
                    HiringDate = DateTime.Parse("2009-3-15"),
                    Vacations = new Vacation[0]
                }
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
                Configuration = new VacationsReportConfiguration()
                {
                    CurrentYear = 2011,
                    DetailedYearsQuantity = 5
                },
                Data = new VacationsReportData()
                {
                    HiringDate = DateTime.Parse("2008-3-15"),
                    Vacations = new Vacation[0]
                }
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
                Configuration = new VacationsReportConfiguration()
                {
                    CurrentYear = 2011,
                    DetailedYearsQuantity = 5
                },
                Data = new VacationsReportData()
                {
                    HiringDate = DateTime.Parse("2007-3-15"),
                    Vacations = new Vacation[0]
                }
            };

            var calculated = calculator.Execute();
            var expected = 70;
            int actual = calculated.TotalPending;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void HiredFiveYearsAgo()
        {
            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                Configuration = new VacationsReportConfiguration()
                {
                    CurrentYear = 2011,
                    DetailedYearsQuantity = 5
                },
                Data = new VacationsReportData()
                {
                    HiringDate = DateTime.Parse("2006-3-15"),
                    Vacations = new Vacation[0]
                }
            };

            var calculated = calculator.Execute();
            var expected = 91;
            int actual = calculated.TotalPending;
            Assert.AreEqual(expected, actual);
        }

        [ExpectedException(typeof(ScriptCommandException))]
        [TestMethod()]
        public void HiringDateUnset()
        {
            var target = new Employee();

            var calculator = new CalculateVacations()
            {
                Context = new ScriptContext(),
                Configuration = new VacationsReportConfiguration()
                {
                    CurrentYear = 2011,
                    DetailedYearsQuantity = 5
                },
                Data = VacationsReportData.FromEmployee(target)
            };

            var result = calculator.Execute();
        }
    }
}
