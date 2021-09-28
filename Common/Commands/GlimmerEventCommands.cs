using AQMod.Content.WorldEvents;
using AQMod.Localization;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Common.Commands
{
    internal class GlimmerEventCommands : RegisterableCommand
    {
        public static bool ForceGlimmerOff { get; private set; }

        public override void Action(CommandCaller caller, string[] arguments)
        {
            if (!ServerCheck(caller) || NoArgumentsCheck(caller, arguments))
                return;
            switch (arguments[0])
            {
                case "off":
                {
                    ForceGlimmerOff = true;
                }
                break;

                case "on":
                {
                    ForceGlimmerOff = false;
                }
                break;

                case "enabled":
                {
                    caller.Reply((!ForceGlimmerOff).ToString());
                }
                break;

                case "start":
                {
                    GlimmerEvent.Activate();
                    caller.Reply(Language.GetTextValue(AQText.Key + "Common.GlimmerEventWarning"), GlimmerEvent.TextColor);
                }
                break;

                case "end":
                {
                    GlimmerEvent.Deactivate();
                    caller.Reply(Language.GetTextValue(AQText.Key + "Common.GlimmerEventEnding"), GlimmerEvent.TextColor);
                }
                break;

                case "active":
                {
                    caller.Reply(GlimmerEvent.ActuallyActive.ToString());
                }
                break;
            }
        }

        public override string Help(CommandCaller caller)
        {
            return "{/aqc glimmer off} will disable the Glimmer Event\n" +
                "{/aqc glimmer on} will enable the Glimmer Event\n" +
                "{/aqc glimmer enabled} will write if the Glimmer Event is enabled\n" +
                "{/aqc glimmer start} will start the Glimmer Event\n" +
                "{/aqc glimmer end} will end the Glimmer Event\n" +
                "{/aqc glimmer active} will write if the Glimmer Event is active";
        }
    }
}