using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Common.Commands
{
    internal abstract class RegisterableCommand
    {
        protected bool NoServerCheck(CommandCaller caller)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                caller.Reply("This command can only be called on a client");
                return false;
            }
            return true;
        }

        protected bool ServerCheck(CommandCaller caller)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                caller.Reply("This command can only be called on a server console");
                return false;
            }
            return true;
        }

        protected bool NoArgumentsCheck(CommandCaller caller, string[] arguments)
        {
            if (arguments.Length == 0)
            {
                caller.Reply("Please add an argument");
                return true;
            }
            return false;
        }

        public abstract void Action(CommandCaller caller, string[] arguments);

        public abstract string Help(CommandCaller caller);
    }
}