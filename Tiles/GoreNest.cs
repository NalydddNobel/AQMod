using AQMod.Common.SceneLayers;
using AQMod.Content.WorldEvents.DemonicEvent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.Tiles
{
    public class GoreNest : ModTile
    {
        internal static bool GrowGoreNest(int x, int y, bool checkX, bool checkY)
        {
            if (checkX)
            {
                int thirdX = Main.maxTilesX / 3;
                if (x <= thirdX || x >= Main.maxTilesX - thirdX)
                    return false;
            }
            if (checkY)
            {
                if (y < Main.maxTilesY - 300)
                    return false;
            }
            if (Main.tile[x, y].active())
            {
                if (!Main.tile[x, y - 1].active())
                {
                    y--;
                }
                else
                {
                    return false;
                }
            }
            else if (!Main.tile[x, y + 1].active())
            {
                return false;
            }
            y -= 2;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int x2 = x + i;
                    int y2 = y + j;
                    if (Main.tileCut[Main.tile[x2, y2].type])
                        Main.tile[x2, y2].active(active: false);
                    if (Framing.GetTileSafely(x2, y2).active() || Main.tile[x2, y2].liquid > 0)
                        return false;
                }
            }
            y += 3;
            for (int i = 0; i < 3; i++)
            {
                int x2 = x + i;
                if (!Framing.GetTileSafely(x2, y).active() || !Main.tileSolid[Main.tile[x2, y].type] || Main.tileCut[Main.tile[x2, y].type])
                    return false;
            }
            for (int i = 0; i < 3; i++)
            {
                int x2 = x + i;
                Main.tile[x2, y].slope(slope: 0);
                Main.tile[x2, y].halfBrick(false);
            }
            y--;
            int tileType = ModContent.TileType<GoreNest>();
            WorldGen.PlaceTile(x, y, tileType);
            if (Main.tile[x, y].type == tileType)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void SetDefaults()
        {
            Main.tileHammer[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 18 };
            TileObjectData.addTile(Type);
            dustType = DustID.Blood;
            disableSmartCursor = true;
            adjTiles = new int[] { TileID.DemonAltar };
            AddMapEntry(new Color(175, 15, 15), Lang.GetItemName(ModContent.ItemType<Items.Placeable.GoreNestItem>()));
        }


        private static (DemonSiegeUpgrade? upgrade, Item item) getUpgradeableItem(Player player)
        {
            for (int i = 0; i < Main.maxInventory; i++)
            {
                var upgrade = DemonSiege.GetUpgrade(player.inventory[i]);
                if (upgrade != null)
                {
                    return (upgrade, player.inventory[i]);
                }
            }
            return (null, null);
        }

        public override bool HasSmartInteract()
        {
            if (DemonSiege.IsActive)
            {
                return false;
            }
            return getUpgradeableItem(Main.LocalPlayer).item != null;
        }

        public override void MouseOver(int i, int j)
        {
            if (DemonSiege.IsActive)
            {
                return;
            }
            var player = Main.player[Main.myPlayer];
            var upgradeableItem = getUpgradeableItem(player);
            if (upgradeableItem.item != null && upgradeableItem.item.type > ItemID.None)
            {
                player.noThrow = 2;
                player.showItemIcon = true;
                player.showItemIcon2 = upgradeableItem.item.type;
            }
        }

        public override bool AutoSelect(int i, int j, Item item)
        {
            return DemonSiege.GetUpgrade(item) != null;
        }

        public override bool NewRightClick(int i, int j)
        {
            if (DemonSiege.IsActive)
            {
                return false;
            }
            var player = Main.player[Main.myPlayer];
            var upgradeableItem = getUpgradeableItem(player);
            if (upgradeableItem.item != null && upgradeableItem.item.type > ItemID.None)
            {
                DemonSiege.Activate(i, j, player.whoAmI, upgradeableItem.item);
                Main.PlaySound(SoundID.DD2_EtherianPortalOpen, new Vector2(i * 16f, j * 16f));
            }
            return false;
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            return Main.hardMode && (!DemonSiege.IsActive || !DemonSiege.altarRectangle().Contains(i, j));
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 48, 48, ModContent.ItemType<Items.Placeable.GoreNestItem>());
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref Color drawColor, ref int nextSpecialDrawIndex)
        {
            if (Main.tile[i, j].frameX == 0 && Main.tile[i, j].frameY == 0)
                GoreNestLayer.AddCorrds(i, j);
        }
    }
}