using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.Banners;

internal class MonsterBanners : ModTile {
    public static readonly Dictionary<System.Byte, ModTile> StyleToNewBannerTileConversion = new();

    public override System.String Texture => this.NamespaceFilePath() + "/OldMonsterBanners";

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
        TileObjectData.newTile.Height = 3;
        TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.LavaDeath = true;
        TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom | AnchorType.Platform, TileObjectData.newTile.Width, 0);
        TileObjectData.newTile.StyleWrapLimit = 111;
        TileObjectData.addTile(Type);
        DustType = -1;
        TileID.Sets.DisableSmartCursor[Type] = true;
    }

    public override System.Boolean PreDraw(System.Int32 i, System.Int32 j, SpriteBatch spriteBatch) {
        if (Main.tile[i, j].TileFrameX % 18 == 0 && Main.tile[i, j].TileFrameY % 54 == 0) {
            System.Int32 style = Main.tile[i, j].TileFrameX / 18;
            if (StyleToNewBannerTileConversion.TryGetValue((System.Byte)style, out var newTile)) {
                for (System.Int32 y = j; y < j + 3; y++) {
                    Main.tile[i, j].TileFrameX = 0;
                    Main.tile[i, j].TileType = newTile.Type;
                }
            }
            NetMessage.SendTileSquare(-1, i, j, 1, 3);
        }
        return false;
    }
}
