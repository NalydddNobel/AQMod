using System.Collections.Generic;
using Terraria.ModLoader;

namespace AQMod.Common.Commands
{
    public class AQCommand : ModCommand
    {
        private static Dictionary<string, RegisterableCommand> Commands { get; set; }

        public override string Command => "aqc";

        public override CommandType Type => CommandType.Chat | CommandType.Console;

        public override string Usage => "/aqc help for more info";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            var callType = args[0].ToLower();
            if (callType == "help")
            {
                caller.Reply("List of commands:");
                var keys = new List<string>(Commands.Keys);
                for (int i = 0; i < keys.Count; i++)
                {
                    caller.Reply(i + ": " + keys[i]);
                }
            }
            else
            {
                if (Commands.TryGetValue(callType, out var command))
                {
                    if (args.Length > 1)
                    {
                        if (args[1] != "help")
                        {
                            string[] arguments = new string[args.Length - 1];
                            for (int i = 1; i < args.Length; i++)
                            {
                                arguments[i - 1] = args[i];
                            }
                            command.Action(caller, arguments);
                        }
                        else
                        {
                            caller.Reply(command.Help(caller));
                        }
                    }
                    else
                    {
                        command.Action(caller, new string[0]);
                    }
                }
            }
        }

        internal static void LoadCommands()
        {
            RegisterCommand(new GlimmerEventCommands(), "glimmer");
        }

        internal static void UnloadCommands()
        {
            Commands = null;
        }

        internal static void RegisterCommand(RegisterableCommand command, string name)
        {
            if (AQMod.Loading)
            {
                if (Commands == null)
                    Commands = new Dictionary<string, RegisterableCommand>();
                if (!Commands.ContainsKey(name))
                    Commands.Add(name.ToLower(), command);
            }
        }
    }
}