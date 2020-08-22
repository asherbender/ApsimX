﻿namespace UnitTests.Factorial
{
    using Models.Core;
    using Models.Factorial;
    using Models.Core.Run;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using UnitTests.Weather;
    using APSIM.Shared.Utilities;
    using Models.Core.ApsimFile;

    /// <summary>This is a test class for the Experiment class</summary>
    [TestFixture]
    public class ExperimentTests
    {
        /// <summary>Ensure a property set overrides work.</summary>
        [Test]
        public void EnsurePropertySetsWork()
        {
            var experiment = new Experiment()
            {
                Name = "Exp1",
                Children = new List<IModel>()
                {
                    new Simulation()
                    {
                        Name = "BaseSimulation",
                        Children = new List<IModel>()
                        {
                            new MockWeather()
                            {
                                Name = "Weather",
                                MaxT = 1,
                                StartDate = DateTime.MinValue
                            },
                        }
                    },
                    new Factors()
                    {
                        Children = new List<IModel>()
                        {
                            new Factor()
                            {
                                Name = "MaxT",
                                Specification = "[Weather].MaxT = 10, 20"
                            },
                        }
                    }
                }
            };
            experiment.ParentAllDescendants();

            var sims = experiment.GenerateSimulationDescriptions();
            Assert.AreEqual(sims.Count, 2);

            Assert.AreEqual(sims[0].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            Assert.AreEqual(sims[0].Descriptors.Find(d => d.Name == "MaxT").Value, "10");
            var weather = sims[0].ToSimulation().Children[0] as MockWeather;
            Assert.AreEqual(weather.MaxT, 10);

            Assert.AreEqual(sims[1].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            Assert.AreEqual(sims[1].Descriptors.Find(d => d.Name == "MaxT").Value, "20");
            weather = sims[1].ToSimulation().Children[0] as MockWeather;
            Assert.AreEqual(weather.MaxT, 20);
        }

        /// <summary>Ensure a property range override works.</summary>
        [Test]
        public void EnsurePropertyRangeWork()
        {
            var experiment = new Experiment()
            {
                Name = "Exp1",
                Children = new List<IModel>()
                {
                    new Simulation()
                    {
                        Name = "BaseSimulation",
                        Children = new List<IModel>()
                        {
                            new MockWeather()
                            {
                                Name = "Weather",
                                MaxT = 1,
                                StartDate = DateTime.MinValue
                            },
                        }
                    },
                    new Factors()
                    {
                        Children = new List<IModel>()
                        {
                            new Factor()
                            {
                                Name = "MaxT",
                                Specification = "[Weather].MaxT = 10 to 20 step 5"
                            }
                        }
                    }
                }
            };
            experiment.ParentAllDescendants();

            var sims = experiment.GenerateSimulationDescriptions();
            Assert.AreEqual(sims[0].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            Assert.AreEqual(sims[0].Descriptors.Find(d => d.Name == "MaxT").Value, "10");
            var weather = sims[0].ToSimulation().Children[0] as MockWeather;
            Assert.AreEqual(weather.MaxT, 10);

            Assert.AreEqual(sims[1].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            Assert.AreEqual(sims[1].Descriptors.Find(d => d.Name == "MaxT").Value, "15");
            weather = sims[1].ToSimulation().Children[0] as MockWeather;
            Assert.AreEqual(weather.MaxT, 15);

            Assert.AreEqual(sims[2].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            Assert.AreEqual(sims[2].Descriptors.Find(d => d.Name == "MaxT").Value, "20");
            weather = sims[2].ToSimulation().Children[0] as MockWeather;
            Assert.AreEqual(weather.MaxT, 20);

            Assert.AreEqual(sims.Count, 3);
        }

        /// <summary>Ensure model overrides work.</summary>
        [Test]
        public void EnsureModelOverrideWorks()
        {
            var experiment = new Experiment()
            {
                Name = "Exp1",
                Children = new List<IModel>()
                {
                    new Simulation()
                    {
                        Name = "BaseSimulation",
                        Children = new List<IModel>()
                        {
                            new MockWeather()
                            {
                                Name = "Weather",
                                MaxT = 1,
                                MinT = 0
                            },
                        }
                    },
                    new Factors()
                    {
                        Children = new List<IModel>()
                        {
                            new Factor()
                            {
                                Name = "Factor",
                                Specification = "[Weather]",
                                Children = new List<IModel>()
                                {
                                    new MockWeather()
                                    {
                                        Name = "Weather1",
                                        MaxT = 10,
                                        MinT = 10.2
                                    },
                                    new MockWeather()
                                    {
                                        Name = "Weather2",
                                        MaxT = 20,
                                        MinT = 10.4
                                    }
                                }
                            },
                        }
                    }
                }
            };
            experiment.ParentAllDescendants();

            var sims = experiment.GenerateSimulationDescriptions();
            Assert.AreEqual(sims[0].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            Assert.AreEqual(sims[0].Descriptors.Find(d => d.Name == "Factor").Value, "Weather1");
            var weather = sims[0].ToSimulation().Children[0] as MockWeather;
            Assert.AreEqual(weather.MaxT, 10);
            Assert.AreEqual(weather.MinT, 10.2);

            Assert.AreEqual(sims[1].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            Assert.AreEqual(sims[1].Descriptors.Find(d => d.Name == "Factor").Value, "Weather2");
            weather = sims[1].ToSimulation().Children[0] as MockWeather;
            Assert.AreEqual(weather.MaxT, 20);
            Assert.AreEqual(weather.MinT, 10.4);

            Assert.AreEqual(sims.Count, 2);
        }

        /// <summary>Ensure composite factors directly under a factors model works.</summary>
        [Test]
        public void EnsureCompositeUnderFactorsModelWorks()
        {
            var experiment = new Experiment()
            {
                Name = "Exp1",
                Children = new List<IModel>()
                {
                    new Simulation()
                    {
                        Name = "BaseSimulation",
                        Children = new List<IModel>()
                        {
                            new MockWeather()
                            {
                                Name = "Weather",
                                MaxT = 1,
                                MinT = 0
                            },
                            new MockClock()
                            {
                                Name = "Clock",
                                NumberOfTicks = 1,
                                Today = DateTime.MinValue 
                            },
                            new MockSummary()
                        }
                    },
                    new Factors()
                    {
                        Children = new List<IModel>()
                        {
                            new CompositeFactor()
                            {
                                Name = "Factor1",
                                Specifications = new List<string>() { "[Weather].MaxT = 10",
                                                                        "[Weather].MinT = 20",
                                                                        "[Clock].NumberOfTicks = 10"},
                            },
                            new CompositeFactor()
                            {
                                Name = "Factor2",
                                Specifications = new List<string>() { "[Weather].MaxT = 100",
                                                                        "[Weather].MinT = 200",
                                                                        "[Clock].NumberOfTicks = 100"},
                            }
                        }
                    }
                }
            };
            experiment.ParentAllDescendants();

            var sims = experiment.GenerateSimulationDescriptions();
            Assert.AreEqual(sims[0].Name, "Exp1Factor1");
            Assert.AreEqual(sims[0].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            var sim = sims[0].ToSimulation();
            var weather = sim.Children[0] as MockWeather;
            var clock = sim.Children[1] as MockClock;
            Assert.AreEqual(weather.MaxT, 10);
            Assert.AreEqual(weather.MinT, 20);
            Assert.AreEqual(clock.NumberOfTicks, 10);

            Assert.AreEqual(sims[1].Name, "Exp1Factor2");
            Assert.AreEqual(sims[1].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            sim = sims[1].ToSimulation();
            weather = sim.Children[0] as MockWeather;
            clock = sim.Children[1] as MockClock;
            Assert.AreEqual(weather.MaxT, 100);
            Assert.AreEqual(weather.MinT, 200);
            Assert.AreEqual(clock.NumberOfTicks, 100);

            Assert.AreEqual(sims.Count, 2);
        }

        /// <summary>Ensure composite that has a model override works.</summary>
        [Test]
        public void EnsureCompositeUnderFactorModelWorks()
        {
            var experiment = new Experiment()
            {
                Name = "Exp1",
                Children = new List<IModel>()
                {
                    new Simulation()
                    {
                        Name = "BaseSimulation",
                        Children = new List<IModel>()
                        {
                            new MockWeather()
                            {
                                Name = "Weather",
                                MaxT = 1,
                                MinT = 0
                            },
                            new MockClock()
                            {
                                Name = "Clock",
                                NumberOfTicks = 1,
                                Today = DateTime.MinValue
                            },
                            new MockSummary()
                        }
                    },
                    new Factors()
                    {
                        Children = new List<IModel>()
                        {
                            new Factor()
                            {
                                Name = "Site",
                                Children = new List<IModel>()
                                {
                                    new CompositeFactor()
                                    {
                                        Name = "Goondiwindi",
                                        Specifications = new List<string>() { "[Weather].MaxT = 10",
                                                                              "[Weather].MinT = 20",
                                                                              "[Clock]"},
                                        Children = new List<IModel>()
                                        {
                                            new MockClock()
                                            {
                                                Name = "Clock",
                                                NumberOfTicks = 10,
                                                Today = DateTime.MinValue
                                            }
                                        }
                                    },
                                    new CompositeFactor()
                                    {
                                        Name = "Toowoomba",
                                        Specifications = new List<string>() { "[Weather].MaxT = 100",
                                                                              "[Weather].MinT = 200",
                                                                              "[Clock]"},
                                        Children = new List<IModel>()
                                        {
                                            new MockClock()
                                            {
                                                Name = "Clock",
                                                NumberOfTicks = 100,
                                                Today = DateTime.MinValue
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            experiment.ParentAllDescendants();

            var sims = experiment.GenerateSimulationDescriptions();
            Assert.AreEqual(sims[0].Name, "Exp1SiteGoondiwindi");
            Assert.AreEqual(sims[0].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            Assert.AreEqual(sims[0].Descriptors.Find(d => d.Name == "Site").Value, "Goondiwindi");
            var sim = sims[0].ToSimulation();
            var weather = sim.Children[0] as MockWeather;
            var clock = sim.Children[1] as MockClock;
            Assert.AreEqual(weather.MaxT, 10);
            Assert.AreEqual(weather.MinT, 20);
            Assert.AreEqual(clock.NumberOfTicks, 10);

            Assert.AreEqual(sims[1].Name, "Exp1SiteToowoomba");
            Assert.AreEqual(sims[1].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            Assert.AreEqual(sims[1].Descriptors.Find(d => d.Name == "Site").Value, "Toowoomba");
            sim = sims[1].ToSimulation();
            weather = sim.Children[0] as MockWeather;
            clock = sim.Children[1] as MockClock;
            Assert.AreEqual(weather.MaxT, 100);
            Assert.AreEqual(weather.MinT, 200);
            Assert.AreEqual(clock.NumberOfTicks, 100);

            Assert.AreEqual(sims.Count, 2);
        }

        /// <summary>Ensure composite that has 2 model overrides works.</summary>
        [Test]
        public void EnsureCompositeWithTwoChildModelsOfSameTypeWorks()
        {
            var experiment = new Experiment()
            {
                Name = "Exp1",
                Children = new List<IModel>()
                {
                    new Simulation()
                    {
                        Name = "BaseSimulation",
                        Children = new List<IModel>()
                        {
                            new Models.Operations()
                            {
                                Name = "Sowing",
                                Operation = new List<Models.Operation>()
                                {
                                    new Models.Operation()
                                    {
                                        Action = "Sowing"
                                    }
                                }
                            },
                            new Models.Operations()
                            {
                                Name = "Cutting",
                                Operation = new List<Models.Operation>()
                                {
                                    new Models.Operation()
                                    {
                                        Action =   "Cutting"
                                    }
                                }

                            },
                        }
                    },
                    new Factors()
                    {
                        Children = new List<IModel>()
                        {
                            new Factor()
                            {
                                Name = "Site",
                                Children = new List<IModel>()
                                {
                                    new CompositeFactor()
                                    {
                                        Name = "1",
                                        Specifications = new List<string>() { "[Sowing]",
                                                                              "[Cutting]"},
                                        Children = new List<IModel>()
                                        {
                                            new Models.Operations()
                                            {
                                                Name = "Sowing",
                                                Operation = new List<Models.Operation>()
                                                {
                                                    new Models.Operation()
                                                    {
                                                        Action = "Sowing1"
                                                    }
                                                }
                                            },
                                            new Models.Operations()
                                            {
                                                Name = "Cutting",
                                                Operation = new List<Models.Operation>()
                                                {
                                                    new Models.Operation()
                                                    {
                                                        Action =   "Cutting1"
                                                    }
                                                }

                                            },
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            experiment.ParentAllDescendants();

            var sims = experiment.GenerateSimulationDescriptions();
            Assert.AreEqual(sims[0].Name, "Exp1Site1");
            var sim = sims[0].ToSimulation();
            var sowing = sim.Children[0] as Models.Operations;
            var cutting = sim.Children[1] as Models.Operations;
            Assert.AreEqual(sowing.Operation[0].Action, "Sowing1");
            Assert.AreEqual(cutting.Operation[0].Action, "Cutting1");
        }

        /// <summary>Ensure disabled simulations aren't run.</summary>
        [Test]
        public void EnsureDisabledSimulationsArentRun()
        {
            var experiment = new Experiment()
            {
                Name = "Exp1",
                Children = new List<IModel>()
                {
                    new Simulation()
                    {
                        Name = "BaseSimulation",
                        Children = new List<IModel>()
                        {
                            new MockWeather()
                            {
                                Name = "Weather",
                                MaxT = 1,
                                StartDate = DateTime.MinValue
                            },
                        }
                    },
                    new Factors()
                    {
                        Children = new List<IModel>()
                        {
                            new Factor()
                            {
                                Name = "MaxT",
                                Specification = "[Weather].MaxT = 10, 20"
                            },
                            new Factor()
                            {
                                Name = "StartDate",
                                Specification = "[Weather].StartDate = 2003-11-01, 2003-12-01"
                            }
                        }
                    }
                }
            };
            experiment.ParentAllDescendants();

            experiment.DisabledSimNames = new List<string>() { "Exp1MaxT10", "Exp1StartDate2003-11-01" };

            var sims = experiment.GenerateSimulationDescriptions();
            Assert.AreEqual(sims[0].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            Assert.AreEqual(sims[0].Descriptors.Find(d => d.Name == "MaxT").Value, "20");
            var weather = sims[0].ToSimulation().Children[0] as MockWeather;
            Assert.AreEqual(weather.MaxT, 20);
            Assert.AreEqual(weather.StartDate, DateTime.MinValue);

            Assert.AreEqual(sims[1].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            Assert.AreEqual(sims[1].Descriptors.Find(d => d.Name == "StartDate").Value, "2003-12-01");
            weather = sims[1].ToSimulation().Children[0] as MockWeather;
            Assert.AreEqual(weather.MaxT, 1);
            Assert.AreEqual(weather.StartDate, new DateTime(2003, 12, 1));

            Assert.AreEqual(sims.Count, 2);
        }

        /// <summary>Ensure a permutation correctly multiplies child models.</summary>
        [Test]
        public void EnsurePermutationWorks()
        {
            var experiment = new Experiment()
            {
                Name = "Exp1",
                Children = new List<IModel>()
                {
                    new Simulation()
                    {
                        Name = "BaseSimulation",
                        Children = new List<IModel>()
                        {
                            new MockWeather()
                            {
                                Name = "Weather"
                            },
                            new MockModel()
                            {
                                Name = "Mod"
                            },
                        }
                    },
                    new Factors()
                    {
                        Children = new List<IModel>()
                        {
                            new Permutation()
                            {
                                Children = new List<IModel>()
                                {
                                    new Factor()
                                    {
                                        Name = "Weather",
                                        Specification = "[Weather].FileName=1,2",
                                    },
                                    new Factor()
                                    {
                                        Name = "Mod",
                                        Specification = "[Mod].A=3,4",
                                    }
                                }
                            }
                        }
                    }
                }
            };
            experiment.ParentAllDescendants();

            var sims = experiment.GenerateSimulationDescriptions();
            Assert.AreEqual(sims.Count, 4);

            Assert.AreEqual(sims[0].Name, "Exp1Weather1Mod3");
            Assert.AreEqual(sims[0].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            Assert.AreEqual(sims[0].Descriptors.Find(d => d.Name == "Weather").Value, "1");
            Assert.AreEqual(sims[0].Descriptors.Find(d => d.Name == "Mod").Value, "3");
            var sim = sims[0].ToSimulation();
            var weather = sim.Children[0] as MockWeather;
            var mod = sim.Children[1] as MockModel;
            Assert.AreEqual(weather.FileName, "1");
            Assert.AreEqual(mod.A, 3);

            Assert.AreEqual(sims[1].Name, "Exp1Weather2Mod3");
            Assert.AreEqual(sims[1].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            Assert.AreEqual(sims[1].Descriptors.Find(d => d.Name == "Weather").Value, "2");
            Assert.AreEqual(sims[1].Descriptors.Find(d => d.Name == "Mod").Value, "3");
            sim = sims[1].ToSimulation();
            weather = sim.Children[0] as MockWeather;
            mod = sim.Children[1] as MockModel;
            Assert.AreEqual(weather.FileName, "2");
            Assert.AreEqual(mod.A, 3);

            Assert.AreEqual(sims[2].Name, "Exp1Weather1Mod4");
            Assert.AreEqual(sims[2].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            Assert.AreEqual(sims[2].Descriptors.Find(d => d.Name == "Weather").Value, "1");
            Assert.AreEqual(sims[2].Descriptors.Find(d => d.Name == "Mod").Value, "4");
            sim = sims[2].ToSimulation();
            weather = sim.Children[0] as MockWeather;
            mod = sim.Children[1] as MockModel;
            Assert.AreEqual(weather.FileName, "1");
            Assert.AreEqual(mod.A, 4);

            Assert.AreEqual(sims[3].Name, "Exp1Weather2Mod4");
            Assert.AreEqual(sims[3].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            Assert.AreEqual(sims[3].Descriptors.Find(d => d.Name == "Weather").Value, "2");
            Assert.AreEqual(sims[3].Descriptors.Find(d => d.Name == "Mod").Value, "4");
            sim = sims[3].ToSimulation();
            weather = sim.Children[0] as MockWeather;
            mod = sim.Children[1] as MockModel;
            Assert.AreEqual(weather.FileName, "2");
            Assert.AreEqual(mod.A, 4);

        }

        /// <summary>Ensure a two permutations don't permutate each other.</summary>
        [Test]
        public void EnsureTwoPermutationsDontPermutateEachOther()
        {
            var experiment = new Experiment()
            {
                Name = "Exp1",
                Children = new List<IModel>()
                {
                    new Simulation()
                    {
                        Name = "BaseSimulation",
                        Children = new List<IModel>()
                        {
                            new MockModel()
                            {
                                Name = "Irrigation"
                            },
                            new MockModel()
                            {
                                Name = "Fertiliser"
                            },
                        }
                    },
                    new Factors()
                    {
                        Children = new List<IModel>()
                        {
                            new Permutation()
                            {
                                Children = new List<IModel>()
                                {
                                    new Factor()
                                    {
                                        Name = "Irr",
                                        Specification = "[Irrigation].Amount=0,50",
                                    },
                                    new Factor()
                                    {
                                        Name = "Fert",
                                        Specification = "[Fertiliser].Amount=0,100",
                                    }
                                }
                            },
                            new Permutation()
                            {
                                Children = new List<IModel>()
                                {
                                    new Factor()
                                    {
                                        Name = "Irr",
                                        Specification = "[Irrigation].Amount=100,150",
                                    },
                                    new Factor()
                                    {
                                        Name = "Fert",
                                        Specification = "[Fertiliser].Amount=0,20",
                                    }
                                }
                            }
                        }
                    }
                }
            };
            experiment.ParentAllDescendants();

            var sims = experiment.GenerateSimulationDescriptions();
            Assert.AreEqual(sims.Count, 8);

            Assert.AreEqual(sims[0].Name, "Exp1Irr0Fert0");
            Assert.AreEqual(sims[0].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            Assert.AreEqual(sims[0].Descriptors.Find(d => d.Name == "Irr").Value, "0");
            Assert.AreEqual(sims[0].Descriptors.Find(d => d.Name == "Fert").Value, "0");
            var sim = sims[0].ToSimulation();
            var irr = sim.Children[0] as MockModel;
            var fert = sim.Children[1] as MockModel;
            Assert.AreEqual(irr.Amount, 0);
            Assert.AreEqual(fert.Amount, 0);

            Assert.AreEqual(sims[1].Name, "Exp1Irr50Fert0");
            Assert.AreEqual(sims[1].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            Assert.AreEqual(sims[1].Descriptors.Find(d => d.Name == "Irr").Value, "50");
            Assert.AreEqual(sims[1].Descriptors.Find(d => d.Name == "Fert").Value, "0");
            sim = sims[1].ToSimulation();
            irr = sim.Children[0] as MockModel;
            fert = sim.Children[1] as MockModel;
            Assert.AreEqual(irr.Amount, 50);
            Assert.AreEqual(fert.Amount, 0);

            Assert.AreEqual(sims[2].Name, "Exp1Irr0Fert100");
            Assert.AreEqual(sims[2].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            Assert.AreEqual(sims[2].Descriptors.Find(d => d.Name == "Irr").Value, "0");
            Assert.AreEqual(sims[2].Descriptors.Find(d => d.Name == "Fert").Value, "100");
            sim = sims[2].ToSimulation();
            irr = sim.Children[0] as MockModel;
            fert = sim.Children[1] as MockModel;
            Assert.AreEqual(irr.Amount, 0);
            Assert.AreEqual(fert.Amount, 100);

            Assert.AreEqual(sims[3].Name, "Exp1Irr50Fert100");
            Assert.AreEqual(sims[3].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            Assert.AreEqual(sims[3].Descriptors.Find(d => d.Name == "Irr").Value, "50");
            Assert.AreEqual(sims[3].Descriptors.Find(d => d.Name == "Fert").Value, "100");
            sim = sims[3].ToSimulation();
            irr = sim.Children[0] as MockModel;
            fert = sim.Children[1] as MockModel;
            Assert.AreEqual(irr.Amount, 50);
            Assert.AreEqual(fert.Amount, 100);

            Assert.AreEqual(sims[4].Name, "Exp1Irr100Fert0");
            Assert.AreEqual(sims[4].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            Assert.AreEqual(sims[4].Descriptors.Find(d => d.Name == "Irr").Value, "100");
            Assert.AreEqual(sims[4].Descriptors.Find(d => d.Name == "Fert").Value, "0");
            sim = sims[4].ToSimulation();
            irr = sim.Children[0] as MockModel;
            fert = sim.Children[1] as MockModel;
            Assert.AreEqual(irr.Amount, 100);
            Assert.AreEqual(fert.Amount, 0);

            Assert.AreEqual(sims[5].Name, "Exp1Irr150Fert0");
            Assert.AreEqual(sims[5].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            Assert.AreEqual(sims[5].Descriptors.Find(d => d.Name == "Irr").Value, "150");
            Assert.AreEqual(sims[5].Descriptors.Find(d => d.Name == "Fert").Value, "0");
            sim = sims[5].ToSimulation();
            irr = sim.Children[0] as MockModel;
            fert = sim.Children[1] as MockModel;
            Assert.AreEqual(irr.Amount, 150);
            Assert.AreEqual(fert.Amount, 0);

            Assert.AreEqual(sims[6].Name, "Exp1Irr100Fert20");
            Assert.AreEqual(sims[6].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            Assert.AreEqual(sims[6].Descriptors.Find(d => d.Name == "Irr").Value, "100");
            Assert.AreEqual(sims[6].Descriptors.Find(d => d.Name == "Fert").Value, "20");
            sim = sims[6].ToSimulation();
            irr = sim.Children[0] as MockModel;
            fert = sim.Children[1] as MockModel;
            Assert.AreEqual(irr.Amount, 100);
            Assert.AreEqual(fert.Amount, 20);

            Assert.AreEqual(sims[7].Name, "Exp1Irr150Fert20");
            Assert.AreEqual(sims[7].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            Assert.AreEqual(sims[7].Descriptors.Find(d => d.Name == "Irr").Value, "150");
            Assert.AreEqual(sims[7].Descriptors.Find(d => d.Name == "Fert").Value, "20");
            sim = sims[7].ToSimulation();
            irr = sim.Children[0] as MockModel;
            fert = sim.Children[1] as MockModel;
            Assert.AreEqual(irr.Amount, 150);
            Assert.AreEqual(fert.Amount, 20);

        }

        /// <summary>
        /// Ensure that property/model overrides apply to all paddocks.
        /// </summary>
        [Test]
        public void TestOverridingInMultiplePaddocks()
        {
            string json = ReflectionUtilities.GetResourceAsString("UnitTests.Factorial.MultiPaddockFactorOverride.apsimx");
            Simulations sims = FileFormat.ReadFromString<Simulations>(json, out List<Exception> errors);
            if (errors != null && errors.Count > 0)
                throw errors[0];

            Runner runner = new Runner(sims);
            errors = runner.Run();
            if (errors != null && errors.Count > 0)
                throw errors[0];
        }

        /// <summary>Ensure a permutation correctly multiplies child factor with composite factor models.</summary>
        [Test]
        public void EnsurePermutationWithCompositeFactorsWorks()
        {
            var experiment = new Experiment()
            {
                Name = "Exp1",
                Children = new List<IModel>()
                {
                    new Simulation()
                    {
                        Name = "BaseSimulation",
                        Children = new List<IModel>()
                        {
                            new MockWeather()
                            {
                                Name = "Weather"
                            },
                            new MockModel()
                            {
                                Name = "Mod"
                            },
                        }
                    },
                    new Factors()
                    {
                        Children = new List<IModel>()
                        {
                            new Permutation()
                            {
                                Children = new List<IModel>()
                                {
                                    new Factor()
                                    {
                                        Name = "Weather",
                                        Specification = "[Weather].FileName=1,2",
                                    },
                                    new CompositeFactor()
                                    {
                                        Name = "Mod1",
                                        Specifications = new List<string>() { "[Mod].A=3" },
                                    },
                                    new CompositeFactor()
                                    {
                                        Name = "Mod2",
                                        Specifications = new List<string>() { "[Mod].A=4" },
                                    }
                                }
                            }
                        }
                    }
                }
            };
            experiment.ParentAllDescendants();

            var sims = experiment.GenerateSimulationDescriptions();
            Assert.AreEqual(sims.Count, 4);

            Assert.AreEqual(sims[0].Name, "Exp1Weather1Mod1");
            Assert.AreEqual(sims[0].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            Assert.AreEqual(sims[0].Descriptors.Find(d => d.Name == "Weather").Value, "1");
            Assert.AreEqual(sims[0].Descriptors.Find(d => d.Name == "Permutation").Value, "Mod1");
            var sim = sims[0].ToSimulation();
            var weather = sim.Children[0] as MockWeather;
            var mod = sim.Children[1] as MockModel;
            Assert.AreEqual(weather.FileName, "1");
            Assert.AreEqual(mod.A, 3);

            Assert.AreEqual(sims[1].Name, "Exp1Weather2Mod1");
            Assert.AreEqual(sims[1].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            Assert.AreEqual(sims[1].Descriptors.Find(d => d.Name == "Weather").Value, "2");
            Assert.AreEqual(sims[1].Descriptors.Find(d => d.Name == "Permutation").Value, "Mod1");
            sim = sims[1].ToSimulation();
            weather = sim.Children[0] as MockWeather;
            mod = sim.Children[1] as MockModel;
            Assert.AreEqual(weather.FileName, "2");
            Assert.AreEqual(mod.A, 3);

            Assert.AreEqual(sims[2].Name, "Exp1Weather1Mod2");
            Assert.AreEqual(sims[2].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            Assert.AreEqual(sims[2].Descriptors.Find(d => d.Name == "Weather").Value, "1");
            Assert.AreEqual(sims[2].Descriptors.Find(d => d.Name == "Permutation").Value, "Mod2");
            sim = sims[2].ToSimulation();
            weather = sim.Children[0] as MockWeather;
            mod = sim.Children[1] as MockModel;
            Assert.AreEqual(weather.FileName, "1");
            Assert.AreEqual(mod.A, 4);

            Assert.AreEqual(sims[3].Name, "Exp1Weather2Mod2");
            Assert.AreEqual(sims[3].Descriptors.Find(d => d.Name == "Experiment").Value, "Exp1");
            Assert.AreEqual(sims[3].Descriptors.Find(d => d.Name == "Weather").Value, "2");
            Assert.AreEqual(sims[3].Descriptors.Find(d => d.Name == "Permutation").Value, "Mod2");
            sim = sims[3].ToSimulation();
            weather = sim.Children[0] as MockWeather;
            mod = sim.Children[1] as MockModel;
            Assert.AreEqual(weather.FileName, "2");
            Assert.AreEqual(mod.A, 4);

        }

    }
}
