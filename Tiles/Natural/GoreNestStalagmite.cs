using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.Natural
{
    public class GoreNestStalagmite : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileNoAttach[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.CoordinateHeights = new int[2] { 16, 18 };
            TileObjectData.newTile.RandomStyleRange = 6;

            TileObjectData.addTile(Type);

            DustType = DustID.Blood;
            SoundType = SoundID.NPCKilled;
            SoundStyle = 1;

            AddMapEntry(new Color(95, 22, 37, 255));
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            type = DustType;
            Dust.NewDustDirect(new Vector2(i * 16f, j * 16f), 16, 16, DustID.FoodPiece, newColor: new Color(145, 10, 10)).velocity *= 2f;
            return true;
        }
    }
}
