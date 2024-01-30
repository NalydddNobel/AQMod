using Terraria.Chat;
using Terraria.Localization;

namespace Aequus.Core.Utilities;

public class TextBroadcast {
    /// <summary>
    /// Broadcasts a message. Only does something when <see cref="Main.netMode"/> is not equal to <see cref="NetmodeID.MultiplayerClient"/>.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="color"></param>
    /// <param name="args"></param>
    internal static void NewText(System.String key, Color color, params System.Object[] args) {
        if (Main.netMode == NetmodeID.SinglePlayer) {
            Main.NewText(Language.GetTextValue(key, args), color);
        }
        else if (Main.netMode == NetmodeID.Server) {
            ChatHelper.BroadcastChatMessage(NetworkText.FromKey(key, args), color);
        }
    }

    /// <summary>
    /// Broadcasts a language key.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="color"></param>
    public static void NewText(System.String text, Color color) {
        NewTextLiteral(text, color);
    }

    /// <summary>
    /// Broadcasts a literal key.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="color"></param>
    public static void NewTextLiteral(System.String text, Color color) {
        if (Main.netMode != NetmodeID.Server) {
            Main.NewText(Language.GetTextValue(text), color);
            return;
        }

        ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), color);
    }
}
