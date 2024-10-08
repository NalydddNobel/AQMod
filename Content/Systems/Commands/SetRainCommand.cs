using Aequus.Common.Utilities;

namespace Aequus.Content.Systems.Commands;
internal class SetRainCommand : ModCommand {
    public override CommandType Type => CommandType.Console;

    public override string Command => "rain";

    public override string Usage => $"{base.Usage} (*value)";

    public override string Description => "Sets rain. Accepts values between 0 and 1 for intensity.";

    public override void Action(CommandCaller caller, string input, string[] args) {
        float value;
        if (args.Length == 0) {
            // Lazy rain start/stop for calls without parameters.
            if (Main.raining) {
                value = 0f;
                Main.StopRain();
            }
            else {
                value = -1f;
            }
        }
        else {
            if (!float.TryParse(args[0], out value)) {
                caller.Reply("Error: Value is not a valid number.");
                return;
            }

            if (value < 0f || value > 1f) {
                caller.Reply("Error: Value is not between 0-1.");
                return;
            }
        }


        if (value == 0f) {
            // Stop rain if value is exactly zero.
            Main.StopRain();
        }
        else {
            Main.StartRain();

            // Set value if specified
            if (value > 0f) {
                Main.cloudAlpha = value;
                Main.maxRaining = value;
            }
            // Otherwise spit out the generated value.
            else {
                value = Main.maxRaining;
            }
        }

        Main.SyncRain();

        TextHelper.BroadcastLiteral($"Rain intensity has been set to {ALanguage.Decimals(value)}", Color.Lerp(Color.White, Color.LightSlateGray, value));
    }
}
