using Aequus.Tiles;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc
{
    public class SkeletonKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.GoldenKey);
            Item.rare++;
        }
    }

    public class SkeletonKeyChestUnlocker : GlobalTile
    {
        public override void RightClick(int i, int j, int type)
        {
            if (type == TileID.Containers && 
                ChestTypes.GetChestStyle(Main.tile[i, j].TileFrameX) == ChestTypes.LockedGold &&
                Main.LocalPlayer.HasItem(ModContent.ItemType<SkeletonKey>()))
            {
                i -= Main.tile[i, j].TileFrameX % 36 / 18;
                j -= Main.tile[i, j].TileFrameY % 36 / 18;
                Chest.Unlock(i, j);
            }
        }
    }
}