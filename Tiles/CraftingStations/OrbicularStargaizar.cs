using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.Tiles.CraftingStations
{
    public class OrbicularStargaizar : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.addTile(Type);
            dustType = 15;
            disableSmartCursor = true;
            adjTiles = new int[] { TileID.CrystalBall, };
            AddMapEntry(new Color(190, 200, 255, 255), Lang.GetItemName(ModContent.ItemType<Items.Placeable.CraftingStations.OrbicularStargaizar>()));
        }

        public override bool HasSmartInteract()
        {
            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int x = i - tile.frameX % 36 / 18;
            int y = j - tile.frameY / 18;
            var plr = Main.LocalPlayer;
            plr.noThrow = 2;
            plr.showItemIcon = true;
            plr.showItemIcon2 = ModContent.ItemType<Items.Placeable.CraftingStations.OrbicularStargaizar>();
        }

        public override bool NewRightClick(int i, int j)
        {
            Main.PlaySound(SoundID.Item4);
            Main.LocalPlayer.AddBuff(ModContent.BuffType<Buffs.Harmony>(), 54000);
            return true;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 32, 32, ModContent.ItemType<Items.Placeable.CraftingStations.OrbicularStargaizar>());
        }
    }
}