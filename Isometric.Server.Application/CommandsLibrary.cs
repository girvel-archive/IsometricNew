using System;
using System.Linq;
using BashDotNet;
using Isometric.Core;
using Isometric.Server.Common;

namespace Isometric.Server.Application
{
    public static class CommandsLibrary
    {
        public static readonly Library Current = 
            new Library(
                1,
                new Command(
                    "help",
                    new string[0], 
                    new Option[0], 
                    (args, opts) =>
                    {
                        foreach (var command in Current.Commands)
                        {
                            Console.WriteLine(
                                command.Name 
                                + command.PositionalArguments.Aggregate("", (sum, arg) => sum + $" <{arg}>")
                                + command.Options.Aggregate("", (sum, op) => sum + $" [-{op.ShortName}, --{op.LongName} ({op.Name})]"));
                        }
                    }), 
                new Command(
                    "acc",
                    new[] { "login", "password" },
                    new[]
                    {
                        new Option("admin", "admin", 'a'),
                    },
                    (args, opts) =>
                    {
                        Program.Server.Accounts.Add(
                            new Account<Player>(
                                args["login"],
                                args["password"],
                                opts["admin"] == "true"
                                    ? new[] { "user", "admin" }
                                    : new[] { "user" },
                                Player.CreateForWorld(Program.Session.World)));
                    }),
                new Command(
                    "ip",
                    new string[0],
                    new Option[0],
                    (args, opts) =>
                    {
                        Console.WriteLine(Program.Server.EndPoint.ToLogString());
                    }),
                new Command(
                    "log",
                    new string[0],
                    new Option[0],
                    (args, opts) =>
                    {
                        Console.WriteLine("Log mode");
                        Program.LogMode = true;
                    }),
                new Command(
                    "stats",
                    new string[0],
                    new Option[0],
                    (args, opts) =>
                    {
                        Console.WriteLine($"Playing total time: {Program.PlayingTimeCalculator.PlayingTime}");
                    }));
    }
}