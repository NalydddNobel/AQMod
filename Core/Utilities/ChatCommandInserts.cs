namespace Aequus.Core.Utilities;

public class ChatCommandInserts {
    public static string ColorCommandStart(Color color, bool alphaPulse = false) {
        if (alphaPulse) {
            color = Colors.AlphaDarken(color);
        }
        return $"[c/{color.Hex3()}:";
    }

    public static string ColorCommand(string text, Color color, bool alphaPulse = false) {
        return $"{ColorCommandStart(color, alphaPulse)}{text}]";
    }

    public static string ItemCommand(int itemID) {
        return "[i:" + itemID + "]";
    }
    public static string ItemCommand<T>() where T : ModItem {
        return ItemCommand(ModContent.ItemType<T>());
    }
}
