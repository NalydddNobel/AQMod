namespace Aequus.Core.Utilities;

public class ChatCommandInserts {
    public static System.String ColorCommandStart(Color color, System.Boolean alphaPulse = false) {
        if (alphaPulse) {
            color = Colors.AlphaDarken(color);
        }
        return $"[c/{color.Hex3()}:";
    }

    public static System.String ColorCommand(System.String text, Color color, System.Boolean alphaPulse = false) {
        return $"{ColorCommandStart(color, alphaPulse)}{text}]";
    }

    public static System.String ItemCommand(System.Int32 itemID) {
        return "[i:" + itemID + "]";
    }
    public static System.String ItemCommand<T>() where T : ModItem {
        return ItemCommand(ModContent.ItemType<T>());
    }
}
