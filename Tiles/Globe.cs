using AQMod.Items.Placeable;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.Tiles
{
    public class Globe : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.AnchorInvalidTiles = new[] { (int)TileID.MagicalIceBlock, };
            TileObjectData.addTile(Type);
            dustType = DustID.Stone;
            disableSmartCursor = true;
            AddMapEntry(new Color(180, 180, 180), Lang.GetItemName(ModContent.ItemType<GlobeItem>()));
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 32, 32, ModContent.ItemType<GlobeItem>());
        }

        public override bool HasSmartInteract()
        {
            return true;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer)
            {
                AQMod.NearGlobe = 16;
            }
        }

        public override void MouseOver(int i, int j)
        {
            var player = Main.LocalPlayer;
            player.noThrow = 2;
            player.showItemIcon = true;
            player.showItemIcon2 = ModContent.ItemType<GlobeItem>();
        }

        public override bool NewRightClick(int i, int j)
        {
            var center = Main.LocalPlayer.MountedCenter;
            Main.NewText(AQMod.GetText("YourCoordinates", (int)(center.X / 16f), (int)(center.Y / 16f)));
            return true;
        }
    }
}