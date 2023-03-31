using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.Commands {
    public class ServerSetDifficulty : ModCommand {

        public override CommandType Type => CommandType.Console;

        public override string Command => "difficulty";
        
        public override string Usage => "Sets the world difficulty, unless the world is a Journey world. Use a number between 1 and 3";

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

            Main.GameMode = value switch {
                1 => GameModeID.Normal,
                2 => GameModeID.Expert,
                3 => GameModeID.Master,
                _ => throw new NotImplementedException(),
            };

            if (Main.netMode != NetmodeID.SinglePlayer) {
                NetMessage.SendData(MessageID.WorldData);
            }

            string difficultyName = Main.GameMode switch {
                GameModeID.Normal => Language.GetTextValue("UI.Normal"),
                GameModeID.Expert => Language.GetTextValue("UI.Expert"),
                GameModeID.Master => Language.GetTextValue("UI.Master"),
                _ => throw new NotImplementedException(),
            };
            Color difficultyColor = Main.GameMode switch {
                GameModeID.Normal => Color.White,
                GameModeID.Expert => Color.Orange,
                GameModeID.Master => Color.Red,
                _ => throw new NotImplementedException(),
            };
            TextHelper.BroadcastLiteral("Difficulty has been set to " + difficultyName, difficultyColor);
        }
    }
}
