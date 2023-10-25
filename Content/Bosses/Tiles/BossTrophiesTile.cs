using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Content.Bosses.Tiles {
    [LegacyName("Trophies")]
    public class BossTrophiesTile : ModTile {
        public const int OmegaStarite = 0;
        public const int Crabson = 1;
        public const int RedSprite = 2;
        public const int SpaceSquid = 3;
        public const int DustDevil = 4;
        public const int UltraStarite = 5;

        public override void SetStaticDefaults() {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 36;
            TileObjectData.addTile(Type);
            DustType = DustID.WoodFurniture;
            TileID.Sets.DisableSmartCursor[Type] = true;
            AddMapEntry(new Color(120, 85, 60), Language.GetText("MapObject.Trophy"));
        }
    }
}