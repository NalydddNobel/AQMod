using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.Tiles
{
    public class GoreNest : ModTile
    {
        public static List<int> GoreNestUpgradeableItems { get; private set; }

        public override void SetDefaults()
        {
            GoreNestUpgradeableItems = new List<int>
            {
                ItemID.BloodButcherer
            };
            Main.tileHammer[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 18 };
            TileObjectData.addTile(Type);
            dustType = DustID.Blood;
            disableSmartCursor = true;

            ModTranslation name = CreateMapEntryName();
            name.SetDefault("{$Mods.AQMod.ItemName.GoreNestItem}");
            AddMapEntry(new Color(175, 15, 15), name);
        }

        public static int getInteractableItemType(Player player)
        {
            for (int i = 0; i < GoreNestUpgradeableItems.Count; i++)
            {
                if (player.HasItem(GoreNestUpgradeableItems[i]))
                {
                    return GoreNestUpgradeableItems[i];
                }
            }
            return -1;
        }

        public override bool HasSmartInteract()
        {
            return getInteractableItemType(Main.player[Main.myPlayer]) != -1;
        }

        public override void MouseOver(int i, int j)
        {
            var player = Main.player[Main.myPlayer];
            int item = getInteractableItemType(player);
            if (item != -1)
            {
                player.noThrow = 2;
                player.showItemIcon = true;
                player.showItemIcon2 = item;
            }
        }
        public override bool NewRightClick(int i, int j)
        {
            var player = Main.player[Main.myPlayer];
            int item = getInteractableItemType(player);
            if (item != -1)
            {
                switch (item)
                {
                    case ItemID.BloodButcherer:
                    {
                        player.ConsumeItem(ItemID.BloodButcherer);
                        player.QuickSpawnItem(ModContent.ItemType<Items.Weapons.Melee.CrimsonHellSword>());
                    }
                    break;
                }
            }
            return false;
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            return Main.hardMode;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 48, 48, ModContent.ItemType<Items.Placeable.GoreNestItem>());
        }
    }
}