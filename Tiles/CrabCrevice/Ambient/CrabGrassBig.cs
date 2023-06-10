using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.CrabCrevice.Ambient {
    [LegacyName("Ambient3x2", "CrabGrass3x2")]
    public class CrabGrassBig : ModTile {
        public override void SetStaticDefaults() {
            Main.tileNoAttach[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.CoordinateHeights = new int[2] { 16, 18 };
            TileObjectData.newTile.RandomStyleRange = 3;

            TileObjectData.addTile(Type);

            DustType = DustID.JungleGrass;
            HitSound = SoundID.Grass;

            AddMapEntry(Color.Teal);
        }
    }
}