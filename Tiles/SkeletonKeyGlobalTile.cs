using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Tiles
{
    public class SkeletonKeyGlobalTile : GlobalTile
    {
        public override void Load()
        {
            On.Terraria.UI.ItemSlot.RightClick_FindSpecialActions += LockboxDetour;
        }
        private static bool LockboxDetour(On.Terraria.UI.ItemSlot.orig_RightClick_FindSpecialActions orig, Item[] inv, int context, int slot, Player player)
        {
            if (context == ItemSlot.Context.InventoryItem && Main.mouseRight && Main.mouseRightRelease)
            {
                if (inv[slot].type == ItemID.LockBox && player.Aequus().hasSkeletonKey)
                {
                    InnerOpenLockbox(inv[slot], player, player.OpenLockBox);
                    return true;
                }
            }
            return orig(inv, context, slot, player);
        }
        public static void InnerOpenLockbox(Item item, Player player, Action<int> opening)
        {
            if (ItemLoader.ConsumeItem(item, player))
            {
                item.stack--;
            }
            if (item.stack < 0)
            {
                item.SetDefaults();
            }
            SoundEngine.PlaySound(SoundID.Unlock);
            Main.stackSplit = 30;
            Main.mouseRightRelease = false;
            opening(item.type);
            Recipe.FindRecipes();
        }

        public override void RightClick(int i, int j, int type)
        {
            if (type == TileID.Containers)
            {
                if (ChestType.GetStyle(Main.tile[i, j].TileFrameX) == ChestType.LockedGold && Main.LocalPlayer.Aequus().hasSkeletonKey)
                {
                    i -= Main.tile[i, j].TileFrameX % 36 / 18;
                    j -= Main.tile[i, j].TileFrameY % 36 / 18;
                    Chest.Unlock(i, j);
                }
            }
        }
    }
}