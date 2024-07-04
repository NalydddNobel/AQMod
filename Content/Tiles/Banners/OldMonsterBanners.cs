using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace AequusRemake.Content.Tiles.Banners;

[Browsable(false)]
[Obsolete]
internal class MonsterBanners : ModTile {
    public static readonly Dictionary<byte, ModTile> StyleToNewBannerTileConversion = new();

    public override string Texture => this.NamespaceFilePath() + "/OldMonsterBanners";

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

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        if (Main.tile[i, j].TileFrameX % 18 == 0 && Main.tile[i, j].TileFrameY % 54 == 0) {
            int style = Main.tile[i, j].TileFrameX / 18;
            if (StyleToNewBannerTileConversion.TryGetValue((byte)style, out var newTile)) {
                for (int y = j; y < j + 3; y++) {
                    Main.tile[i, j].TileFrameX = 0;
                    Main.tile[i, j].TileType = newTile.Type;
                }
            }
            NetMessage.SendTileSquare(-1, i, j, 1, 3);
        }
        return false;
    }
}
