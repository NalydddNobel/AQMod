using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Localization
{
    partial class AequusText
    {
        public static Color BossSummonColor => new Color(175, 75, 255, 255);
        internal static Color EventMessage => new Color(50, 255, 130, 255);

        public static void BroadcastAwakened(NPC npc)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(Language.GetTextValue("Announcement.HasAwoken", npc.TypeName), BossSummonColor);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", npc.GetTypeNetName()), BossSummonColor);
            }
        }

        public static void BroadcastAwakened(string key)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(Language.GetTextValue("Announcement.HasAwoken", Language.GetTextValue(key)), BossSummonColor);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", key), BossSummonColor);
            }
        }

        public static string Item<T>() where T : ModItem
        {
            return Item(ModContent.ItemType<T>());
        }

        public static string Item(int item)
        {
            return "[i:" + item + "]";
        }
    }
}