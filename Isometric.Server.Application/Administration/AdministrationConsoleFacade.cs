using System.Text;
using BashDotNet;
using BashDotNet.Generic;
using Isometric.Core;

namespace Isometric.Server.Application.Administration
{
    public static class AdministrationConsoleFacade
    {
        private class AdministrationCommandArgs
        {
            public StringBuilder StringBuilder;

            public Account<Player> Account;
        }
        
        private static Library<AdministrationCommandArgs> _library;

        
        
        static AdministrationConsoleFacade()
        {
            _library = new Library<AdministrationCommandArgs>(
                1,
                new Command<AdministrationCommandArgs>(
                    "set_speed",
                    new[] {"value"},
                    new Option[0],
                    (args, opts, data) =>
                    {
                        float speed;
                        if (!float.TryParse(args["value"], out speed))
                        {
                            data.StringBuilder.Append("Wrong integer format of argument 'value'\n");
                            return;
                        }

                        Program.Session.GameSpeedK = speed;
                    }));
        }
        
        
        
        public static bool TryExecute(string command, Account<Player> account, out string output)
        {
            var builder = new StringBuilder("");

            var result = _library.TryExecute(
                command,
                new AdministrationCommandArgs {Account = account, StringBuilder = builder});
            
            output = builder.ToString();
            return result;
        }
    }
}