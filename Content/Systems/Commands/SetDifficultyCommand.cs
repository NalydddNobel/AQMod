using System;
using Terraria.Localization;

namespace Aequus.Content.Commands;

// TODO -- Localize commands?
internal class SetDifficultyCommand : ModCommand {
    public override CommandType Type => CommandType.Console;

    public override string Command => "difficulty";

    public override string Usage => $"{base.Usage} (value)";

    public override string Description => "Sets the world difficulty. Accepts values between 1 and 3";

    public override void Action(CommandCaller caller, string input, string[] args) {
        if (Main.GameMode == GameModeID.Creative) {
            caller.Reply("Error: World is in Journey mode.");
            return;
        }

        if (args.Length == 0) {
            caller.Reply("Error: No value provided.");
            return;
        }

        if (!int.TryParse(args[0], out int value)) {
            caller.Reply("Error: Value is not a valid number.");
            return;
        }

        if (value < 1 || value > 3) {
            caller.Reply("Error: Value is not between 1 and 3.");
            return;
        }

        (string text, Color color, int value) diffValue = Main.GameMode switch {
            GameModeID.Normal => (Language.GetTextValue("UI.Normal"), Color.White, GameModeID.Normal),
            GameModeID.Expert => (Language.GetTextValue("UI.Expert"), Color.Orange, GameModeID.Expert),
            GameModeID.Master => (Language.GetTextValue("UI.Master"), Color.Red, GameModeID.Master),
            _ => throw new NotImplementedException(),
        };

        Main.GameMode = diffValue.value;

        if (Main.netMode != NetmodeID.SinglePlayer) {
            NetMessage.SendData(MessageID.WorldData);
        }

        TextHelper.BroadcastLiteral($"Difficulty has been set to {diffValue.text}", diffValue.color);
    }
}