using AQMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.Tiles.Furniture
{
    public sealed class AQBars : ModTile
    {
        public const int CrustaciumBar = 0;

        public override void SetDefaults()
        {
            Main.tileShine[Type] = 1100;
            Main.tileSolid[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(100, 4, 20), Language.GetText("MapObject.MetalBar"));
        }

        public override bool Drop(int i, int j)
        {
            Item.NewItem(i * 16, j * 16, 16, 16, ModContent.ItemType<CrustaciumBar>());
            return false;
        }
    }
}