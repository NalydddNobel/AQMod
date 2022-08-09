using Aequus.Content;
using Aequus.Items.Placeable.Furniture.Jeweled;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.Furniture.Jeweled
{
    public class JeweledChaliceTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileShine[Type] = 5000;
            Main.tileShine2[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.addTile(Type);
            AddMapEntry(Color.Gold * 1.25f, AequusText.GetTranslation("ItemName.JeweledChalice"));
            HitSound = SoundID.Dig;

            ExporterQuests.TilePlacements.Add(Type, new ExporterQuests.SolidTopPlacement());
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 0;
        }

        public override bool Drop(int i, int j)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ModContent.ItemType<JeweledChalice>());
            return true;
        }
    }
}
