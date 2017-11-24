using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Isometric.Core;
using Isometric.Core.Buildings;
using Isometric.Core.Common;
using Isometric.Core.Managers.Tasks;
using Isometric.Core.Vectors;
using Isometric.Dtos;
using Isometric.Game;
using Isometric.Server.Application.Administration;
using Isometric.Server.Application.Common;
using Isometric.Server.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Isometric.Server.Application
{
    internal class Program
    {
        public static Server<Player> Server;

        public static Session Session;

        public static bool LogMode = true;

        public static PlayingTimeCalculator<Player> PlayingTimeCalculator; 

        private static JsonSerializer savingSerializer;



        private static void Main(string[] consoleArgs)
        {
            #region Loading Session

//#if !DEBUG
//            try
//            {
//                using (var stream = File.OpenWrite(Path.Combine("saves", Directory.GetFiles("saves").Max())))
//                using (var reader = new StreamReader(stream))
//                {
//                    var jobject = JObject.Parse(reader.ReadToEnd());

//                    Server.Accounts = jobject["accounts"].ToObject<List<Account<Player>>>();
//                    Session = jobject["session"].ToObject<Session>();
//                }
//            }
//            catch
//            {
//                Session = new SessionGenerator().Generate();
//            }
//#else
            Session = new SessionGenerator().Generate();
//#endif

            #endregion

            #region News

            Session.ArmiesManager.OnTaskFinished += (a, previous, next) =>
            {
                Server.NewsManager.AddNews(
                    a.Owner,
                    "army task is finished",
                    new Dictionary<string, dynamic>
                    {
                        ["position"] = a.Position,
                    });

                if (next is DestroyingTask)
                {
                    var currentBuilding = Session.World.GetBuilding(a.Position);
                    Server.NewsManager.AddNews(
                        a.Owner,
                        "building loot starts",
                        new Dictionary<string, dynamic>
                        {
                            ["position"] = a.Position,
                            ["loot time"] = a.AttackTime.Multiple(
                                (float) currentBuilding.LifePoints 
                                / a.GetDamageFor(currentBuilding) 
                                / Session.GameSpeedK),
                        });
                }
            };

            Session.World.OnPlayerCreate += p =>
            {
                p.OnArmyCreated += a =>
                {
                    a.OnPositionChanged += (oldPosition, oldIndex) =>
                    {
                        Server.NewsManager.AddNews(
                            p, 
                            "army position is changed", 
                            new Dictionary<string, dynamic>
                            {
                                ["old position"] = oldPosition,
                                ["old index"] = oldIndex,
                                ["new position"] = a.Position,
                            });
                    };

                    a.OnDestroyedObject += o =>
                    {
                        var b = o as Building;

                        if (b != null)
                        {
                            Server.NewsManager.AddNews(
                                p,
                                "building is destroyed",
                                new Dictionary<string, dynamic>
                                {
                                    ["position"] = b.Position,
                                });
                        }
                    };

                    Server.NewsManager.AddNews(
                        p,
                        "army is appeared",
                        new Dictionary<string, dynamic>
                        {
                            ["position"] = a.Position,
                        });
                };

                p.OnBuildingBegin += b =>
                {
                    b.OnHungerChanged += () =>
                    {
                        Server.NewsManager.AddNews(
                            p,
                            "hunger is changed",
                            new Dictionary<string, dynamic>
                            {
                                ["position"] = b.Position,
                                ["hunger"] = b.ArePeopleHungry,
                            });
                    };
                };
            };

            #endregion


            var clocksThread = Session.StartClocksThread(new TimeSpan(0, 0, 1));

            Directory.CreateDirectory("logs");
            using (var stream = File.OpenWrite(Path.Combine("logs", DateTime.Now.ToString("hh mm dd.MM.yyyy"))))
            using (var fileWriter = new StreamWriter(stream))
            using (Server) 
            {
                Server.Log.Writers.Add(fileWriter);
                CommandsLibrary.Current.TryExecute("acc '' '' -a");

                new Thread(Server.Start) {Name = "Server"}.Start();
                new Thread(StartSaving) {Name = "Saving"}.Start();
                new Thread((PlayingTimeCalculator = new PlayingTimeCalculator<Player>(Server)).Start)
                {
                    Name = "Playing time calculating",
                }.Start();

                while (true)
                {
                    if (LogMode)
                    {
                        var key = Console.ReadKey(true);

                        if (key.Key == ConsoleKey.W)
                        {
                            LogMode = false;
                            Server.Log.Writers.Remove(Console.Out);
                            Console.WriteLine("Command mode");
                        }
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.Write("isometric@");
                        if (!CommandsLibrary.Current.TryExecute(Console.ReadLine()))
                        {
                            Console.WriteLine("Wrong command");
                        }
                    }
                }
            }
        }

        private static void StartSaving()
        {
#if !DEBUG
            //const int savingPeriodMilliseconds = 1000 * 60 * 60 * 6; // 6 hours
            //while (true)
            //{
            //    Directory.CreateDirectory("saves");
            //    using (var stream = File.OpenWrite(Path.Combine("saves", DateTime.Now.ToString("hh mm dd.MM.yyyy"))))
            //    using (var writer = new StreamWriter(stream))
            //    {
            //        writer.Write(
            //            JToken.FromObject(
            //                new Dictionary<string, dynamic>
            //                {
            //                    ["accounts"] = Server.Accounts,
            //                    ["session"] = Session,
            //                },
            //                savingSerializer).ToString());
            //    }
            //    Thread.Sleep(savingPeriodMilliseconds);
            //}
#endif
        }



        private static string GetPlayerName(Player p) =>
            Server.Accounts.FirstOrDefault(acc => acc.ExternalData == p)?.Login ?? "no owner";

        private static Building GetPrototypeByName(string name)
            => Session.AllBuildingPrototypes.FirstOrDefault(p => p.Name == name);



        static Program()
        {
            savingSerializer = JsonSerializer.Create(
                new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.All
                });

            Server = new Server<Player>(
                new IPEndPoint(
                    Dns.GetHostAddresses(Dns.GetHostName())
                        .First(a => a.GetAddressBytes().Length == 4 && a.GetAddressBytes()[0] == 192),
                    7999),
                new RequestManager<Player>(
                    new Dictionary<string, ResponsePair<Player>>
                    {
                        #region Requests

                        #region Net
                        ["get version"] =
                            new ResponsePair<Player>(
                                (args, c) => new Dictionary<string, dynamic>
                                {
                                    ["version"] = typeof(Program).Assembly.GetName().Version
                                },
                                ""),

                        ["login"] = new ResponsePair<Player>(
                            (args, c) =>
                            {
                                var account =
                                    c.Server.Accounts.FirstOrDefault(
                                        a => a.Login == args["login"] && a.Password == args["password"]);

                                if (c.Server.Connections.All(co => co.Account != account))
                                {
                                    c.Account = account;
                                }

                                return new Dictionary<string, dynamic> { ["success"] = account != null };
                            },
                            ""),
                        #endregion
                        
                        #region Administration and debug
                        
                        ["execute command"] = 
                            new ResponsePair<Player>(
                                (args, c) => new Dictionary<string, dynamic>
                                {
                                    ["success"] = AdministrationConsoleFacade.TryExecute(
                                        (string) args["command"], 
                                        c.Account, 
                                        out string output),
                                    ["output"] = output,
                                },
                                "admin"),

                        #endregion

                        #region Area & resources
                        ["get world width"] = new ResponsePair<Player>(
                            (args, c) => new Dictionary<string, dynamic>
                            {
                                ["world width"] = Session.World.Landscape.GetLength(0) * Session.World.AreaWidth,
                            },
                            "user"),
                        
                        ["get main area position"] = new ResponsePair<Player>(
                            (args, c) => new Dictionary<string, dynamic>
                            {
                                ["position"] = c.Account.ExternalData.Area.Position,
                            },
                            "user"),

                        ["get area"] =
                            new ResponsePair<Player>(
                                (args, c) =>
                                {
                                    var area = Session.World.GetArea((Vector) args["position"]);
                                    var isOpened = Session.VisionManager.IsAreaOpened(c.Account.ExternalData, area);
    
                                    return new Dictionary<string, dynamic>
                                    {
                                        ["buildings"] =
                                        area.Buildings.TwoDimSelect(
                                            b => new BuildingAreaDto
                                            {
                                                Name = isOpened
                                                    ? b.Name
                                                    : "",
                                                OwnerName = GetPlayerName(b.Owner),
                                                BuildingTime = b.Finished
                                                    ? TimeSpan.Zero 
                                                    : b.BuildingTime.Multiple(
                                                        b.Prototype.Builders == 0
                                                            ? 1
                                                            : (float)b.Builders / b.Prototype.Builders),
                                                IsThereArmy = b.Armies.Any(),
                                                ArePeopleHungry = b.ArePeopleHungry || b.Armies.Any(a => a.IsHungry),
                                            }),
                                    };
                                },
                                "user"),

                        ["get vision"] =
                            new ResponsePair<Player>(
                                (args, c) =>
                                {
                                    var vision = Session.VisionManager.GetVision(Session.World, c.Account.ExternalData);
                                    return new Dictionary<string, dynamic>
                                    {
                                        ["vision"] = new VisionDto
                                        {
                                            Buildings = vision.Buildings.TwoDimSelect(
                                                b => b == null
                                                    ? null
                                                    : new BuildingAreaDto
                                                    {
                                                        Name = b.Name,
                                                        OwnerName = GetPlayerName(b.Owner),
                                                        BuildingTime = b.Finished
                                                            ? TimeSpan.Zero
                                                            : b.BuildingTime.Multiple(
                                                                b.Prototype.Builders == 0
                                                                    ? 1
                                                                    : (float) b.Builders / b.Prototype.Builders),
                                                        IsThereArmy = b.Armies.Any(),
                                                        ArePeopleHungry =
                                                            b.ArePeopleHungry || b.Armies.Any(a => a.IsHungry),
                                                    }),
                                            Position = vision.Position,
                                        }
                                    };
                                },
                                "user"),

                        ["get resources"] = new ResponsePair<Player>(
                            (args, c) => new Dictionary<string, dynamic>
                            {
                                ["resources"] = new ResourcesDto
                                {
                                    ResourcesArray = c.Account.ExternalData.Resources.ResourcesArray,
                                    FreePeople = c.Account.ExternalData.TotalPeople,
                                    MaxPeople = c.Account.ExternalData.MaxPeople,
                                },
                            },
                            "user"),
                        #endregion

                        #region Buildings
                        ["upgrade"] = new ResponsePair<Player>(
                            (args, c) =>
                            {
                                var position = (Vector) args["position"];
                                var b = Session.World.GetBuilding(position);
                                if (b.Owner == Session.World.Constants.NeutralPlayer)
                                {
                                    b.Owner = c.Account.ExternalData;
                                }

                                var upgraded = false;
                                var upgrade = Session.AllBuildingPrototypes.First(p => p.Name == args["to"]);

                                if (b.Owner == c.Account.ExternalData)
                                {
                                    upgraded = Session.World.TryUpgrade(position, upgrade);
                                }

                                return new Dictionary<string, dynamic>
                                {
                                    ["success"] = upgraded,
                                    ["upgrade time"] = upgrade.BuildingTime,
                                };
                            },
                            "user"),

                        ["get building information"] = new ResponsePair<Player>(
                            (args, c) =>
                            {
                                var building = Session.World.GetBuilding((Vector) args["position"]);
                                var workerBuilding = building as WorkerBuilding;
                                var incomeBuilding = building as IncomeBuilding;
                                var armyBuilding = building as ArmyBuilding;

                                return new Dictionary<string, dynamic>
                                {
                                    ["data"] = new BuildingFullDto
                                    {
                                        OwnerName = GetPlayerName(building.Owner),
                                        Name = building.Name,
                                        FreePeople = building.FreePeople,
                                        Builders = building.Builders,
                                        MaxBuilders = building.Prototype.Builders,
                                        IsFinished = building.Finished,
                                        IsWorkerBuilding = workerBuilding != null,
                                        IsIncomeBuilding = incomeBuilding != null,
                                        Workers = workerBuilding?.Workers ?? 0,
                                        MaxWorkers = workerBuilding?.Prototype.Workers ?? 0,
                                        Income = 
                                            (incomeBuilding?.Incomes[incomeBuilding.LastIncomeIndex] ?? Resources.Zero)
                                            .ResourcesArray,
                                        LastIncome = (incomeBuilding?.LastIncome ?? Resources.Zero).ResourcesArray,
                                        Armies = building.Armies
                                            .Select(a => $"{a.LifePoints} ({GetPlayerName(a.Owner)})")
                                            .ToArray(),
                                        IsArmyBuilding = armyBuilding != null,
                                        ArmyCreationTime = armyBuilding?.ArmyCreationTime ?? TimeSpan.Zero,
                                        ArmyCreationTimeMax = armyBuilding?.Prototype.ArmyCreationTime ?? TimeSpan.Zero,
                                        ArmyQueueSize =
                                            armyBuilding == null
                                                ? 0
                                                : (int)
                                                    Math.Floor((float)armyBuilding.TotalPeople /
                                                               armyBuilding.ArmyPrototype.RequiredPeople),
                                        CreatingArmy = armyBuilding?.ArmyPrototype.Name ?? "",
                                        PeopleForArmy = armyBuilding?.ArmyPrototype.RequiredPeople ?? 0,
                                        ArmyPrice = (armyBuilding?.ArmyPrototype.Price ?? Resources.Zero).ResourcesArray,
                                    },
                                };
                            },
                            "user"),

                        ["get upgrades"] = new ResponsePair<Player>(
                            (args, c) =>
                            {
                                Func<Building, UpgradePossibility> getUpgradePossibility = to =>
                                {
                                    if (!c.Account.ExternalData.Resources.Enough(to.Price))
                                    {
                                        return UpgradePossibility.NotEnoughResources;
                                    }

                                    if (!to.RequiredResearches.All(c.Account.ExternalData.ResearchedTechnologies.Contains))
                                    {
                                        return UpgradePossibility.RequiresResearches;
                                    }

                                    return UpgradePossibility.Possible;
                                };

                                var b = Session.World.GetBuilding((Vector) args["position"]);
                                
                                return new Dictionary<string, dynamic>
                                {
                                    ["upgrades"] =
                                        new[]
                                        {
                                            c.Account.ExternalData, 
                                            Session.World.Constants.NeutralPlayer,
                                        }.Contains(b.Owner) && b.Finished
                                            ? b.Upgrades
                                                .Select(u => new UpgradeDto
                                                {
                                                    Name = u.Name,
                                                    Price = u.Price.ResourcesArray,
                                                    Time = u.BuildingTime,
                                                    Possibility = getUpgradePossibility(u),
                                                    RequiredResearches = u.RequiredResearches
                                                        .Where(r => !c.Account.ExternalData.ResearchedTechnologies
                                                            .Contains(r))
                                                        .Select(r => r.Name)
                                                        .ToArray(),
                                                })
                                                .ToArray()
                                            : new UpgradeDto[0],
                                };
                            },
                            "user"),
                        #endregion

                        #region People management
                        ["add workers"] = new ResponsePair<Player>(
                            (args, c) =>
                            {
                                var building = Session.World.GetBuilding((Vector)args["position"]) as WorkerBuilding;

                                return new Dictionary<string, dynamic>
                                {
                                    ["success"] =
                                        building != null &&
                                        Session.PeopleManager.TryAddWorkers(building, (int)args["delta"]),
                                };
                            },
                            "user"),

                        ["get max workers"] = new ResponsePair<Player>(
                            (args, c) => new Dictionary<string, dynamic>
                            {
                                ["max workers"] =
                                    (Session.World.GetBuilding((Vector)args["position"]).Prototype as IncomeBuilding)
                                        ?.Workers ?? 0,
                            },
                            "user"),

                        ["add builders"] = new ResponsePair<Player>(
                            (args, c) => new Dictionary<string, dynamic>
                            {
                                ["success"] = Session.PeopleManager
                                    .TryAddBuilders(
                                        Session.World.GetBuilding((Vector)args["position"]),
                                        (int)args["delta"]),
                            },
                            "user"),

                        ["get max builders"] = new ResponsePair<Player>(
                            (args, c) => new Dictionary<string, dynamic>
                            {
                                ["max workers"] =
                                    (Session.World.GetBuilding((Vector)args["position"]).Prototype as IncomeBuilding)
                                        ?.Workers ?? 0,
                            },
                            "user"),

                        ["get all income buildings"] = new ResponsePair<Player>(
                            (args, c) => new Dictionary<string, dynamic>
                            {
                                ["buildings"] = c.Account.ExternalData.OwnBuildings
                                    .OfType<IncomeBuilding>()
                                    .GroupBy(b => b.Prototype)
                                    .Select(group => new IncomeBuildingDto
                                    {
                                        Name = group.Key.Name,
                                        Workers = group.Sum(b => b.Workers),
                                        MaxWorkers = group.Sum(b => b.Prototype.Workers),
                                        LastIncome = group
                                            .Aggregate(new Resources(), (sum, b) => sum + (Resources)(b.LastIncome ?? Resources.Zero))
                                            .ResourcesArray,
                                        MaxIncome = group
                                            .Aggregate(new Resources(), (sum, b) => sum + (Resources)(b.Incomes[b.LastIncomeIndex] ?? Resources.Zero))
                                            .ResourcesArray,
                                        Count = group.Count(),
                                    })
                                    .ToArray(),
                            },
                            "user"),

                        ["add workers for player"] = new ResponsePair<Player>(
                            (args, c) => new Dictionary<string, dynamic>
                            {
                                ["added"] = Session.PeopleManager.AddWorkersForPlayer(
                                    (WorkerBuilding)GetPrototypeByName((string)args["name"]),
                                    (int)args["delta"],
                                    c.Account.ExternalData)
                            },
                            "user"),
                        #endregion

                        #region Researches
                        ["get nearest researches"] = new ResponsePair<Player>(
                            (args, c) => new Dictionary<string, dynamic>
                            {
                                ["current"] = c.Account.ExternalData.CurrentResearch?.Name ?? "",
                                ["researches"] = (c.Account.ExternalData.ResearchedTechnologies.Any()
                                    ? Session.RootResearch.Where(
                                        r => r.Possible(
                                            c.Account.ExternalData.ResearchedTechnologies,
                                            Session.RootResearch))
                                    : new[] { Session.RootResearch })
                                    .Select(r => new ResearchDto
                                    {
                                        Name = r.Name,
                                        NewBuildings = Session.AllBuildingPrototypes
                                            .Where(p =>
                                                p.RequiredResearches.Contains(r)
                                                && p.RequiredResearches.All(rr =>
                                                    rr == r
                                                    || c.Account.ExternalData.ResearchedTechnologies.Contains(rr)))
                                            .Select(p => p.Name)
                                            .ToArray()
                                    })
                                    .ToArray(),
                            },
                            "user"),

                        ["research"] = new ResponsePair<Player>(
                            (args, c) => new Dictionary<string, dynamic>
                            {
                                ["success"] = c.Account.ExternalData.BeginResearch(
                                    Session.RootResearch.First(r => r.Name == args["name"]),
                                    Session.RootResearch),
                            },
                            "user"),

                        ["get research points"] = new ResponsePair<Player>(
                            (args, c) => new Dictionary<string, dynamic>
                            {
                                ["current points"] = c.Account.ExternalData.CurrentResearchPoints,
                                ["required points"] = c.Account.ExternalData.CurrentResearch?.ResearchPointsRequired ?? 0.0f,
                                ["points per minute"] = c.Account.ExternalData.Area.Buildings
                                    .OfType<ScienceBuilding>()
                                    .Where(b => b.Owner == c.Account.ExternalData)
                                    .Sum(b => b.ResearchIncome
                                        * b.Efficiency
                                        * ((float)TimeSpan.FromMinutes(1).Ticks
                                            / b.IncomePeriod.Ticks)),
                            },
                            "user"),
                        #endregion

                        #region Armies

                        ["get armies info"] = new ResponsePair<Player>(
                            (args, c) => new Dictionary<string, dynamic>
                            {
                                ["armies"] = Session.World.GetBuilding((Vector)args["position"]).Armies
                                    .Select(a => new ArmyDto
                                    {
                                        Name = a.Name,
                                        Owner = GetPlayerName(a.Owner),
                                        Task = Session.ArmiesManager.GetTask(a)?.GetType().Name.ToString() ?? "",
                                        IsControllable = c.Account.ExternalData == a.Owner,
                                        IsBusy = Session.ArmiesManager.IsBusy(a),
                                        LifePoints = a.LifePoints,
                                        Damage = a.Damage,
                                        BonusDamage = a.BonusDamage,
                                        ArmorType = (int)a.Armor,
                                        BonusArmorType = (int)a.BonusDamageArmorType,
                                    })
                                    .ToArray(),
                            },
                            "user"),

                        ["clear army tasks queue"] = new ResponsePair<Player>(
                            (args, c) =>
                            {
                                var army = Session.World
                                    .GetBuilding((Vector) args["position"])
                                    .Armies[(int) args["index"]];

                                if (army.Owner == c.Account.ExternalData)
                                {
                                    Session.ArmiesManager.ClearQueue(army);
                                }
                                
                                return new Dictionary<string, dynamic>();
                            },
                            "user"),
                        
                        ["move army"] = new ResponsePair<Player>(
                            (args, c) =>
                            {
                                var army 
                                    = Session.World
                                        .GetBuilding((Vector)args["position"])
                                        .Armies[(int) args["army index"]];

                                if (army.Owner == c.Account.ExternalData)
                                {
                                    Session.ArmiesManager.AddMovingTask(army, (Vector) args["to"]);
                                }

                                return new Dictionary<string, dynamic>();
                            },
                            "user"),

                        ["train army"] = new ResponsePair<Player>(
                            (args, c) =>
                            {
                                var b = Session.World.GetBuilding((Vector)args["position"]) as ArmyBuilding;

                                return new Dictionary<string, dynamic>
                                {
                                    ["success"] = 
                                        b != null 
                                        && b.Owner == c.Account.ExternalData
                                        && Session.PeopleManager.TryMovePeople(b, b.ArmyPrototype.RequiredPeople),
                                    ["training time"] = b?.ArmyCreationTime ?? TimeSpan.Zero,
                                };
                            },
                            "user"),

                        ["loot area"] = new ResponsePair<Player>(
                            (args, c) =>
                            {
                                var b = Session.World.GetBuilding((Vector)args["position"]);
                                var army = b.Armies[(int)args["army index"]];

                                if (army.Owner == c.Account.ExternalData)
                                {
                                    Session.ArmiesManager.AddAreaLootTask(army, (Vector) args["to"], (int) args["range"]);
                                }

                                return new Dictionary<string, dynamic>();
                            },
                            "user"),

                        #endregion

                        #region News
                        ["get news"] = new ResponsePair<Player>(
                            (args, c) => new Dictionary<string, dynamic>
                            {
                                ["news"] = Server.NewsManager
                                    .GetNews(c.Account.ExternalData)
                                    .Select(n => new NewsDto
                                    {
                                        Type = n.Type,
                                        Info = n.Info,
                                    })
                                    .ToArray(),
                            },
                            "user"),
                        #endregion

                        #endregion
                    },
                    (args, c) => new Dictionary<string, dynamic> { ["error type"] = "permission" },
                    ex => (args, c) =>
                        new Dictionary<string, dynamic>
                        {
                            ["error type"] = "request",
                            ["exception"] = ex,
                        }),
                new Log(Console.Out));
        }
    }
}
