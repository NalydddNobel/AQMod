using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequus.Content.Bosses.Trophies;

[LegacyName("Trophies", "BossTrophiesTile")]
public class LegacyBossTrophiesTile : ModTile {
    public const System.Int32 OmegaStarite = 0;
    public const System.Int32 Crabson = 1;
    public const System.Int32 RedSprite = 2;
    public const System.Int32 SpaceSquid = 3;
    public const System.Int32 DustDevil = 4;
    public const System.Int32 UltraStarite = 5;

    public static Dictionary<System.Int32, ModTile> LegacyConverter { get; private set; } = new();

    public override System.String Texture => AequusTextures.Tile(TileID.Painting3X3);

    public override void SetStaticDefaults() {
        Main.tileSpelunker[Type] = true;
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

    public override void Unload() {
        LegacyConverter.Clear();
    }

    public override void DrawEffects(System.Int32 i, System.Int32 j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) {
        if (drawData.tileFrameX % 54 == 0 && drawData.tileFrameY % 54 == 0) {
            System.Int32 legacyConvertId = drawData.tileFrameX / 54;

            System.UInt16 type;
            System.Boolean active;
            if (LegacyConverter.TryGetValue(legacyConvertId, out var modTrophyInstance)) {
                type = modTrophyInstance.Type;
                active = true;
            }
            // Weird exception, just delete this trophy entirely
            else {
                type = 0;
                active = false;
            }

            for (System.Int32 x = i; x < i + 3; x++) {
                for (System.Int32 y = j; y < j + 3; y++) {
                    var tile = Framing.GetTileSafely(x, y);
                    if (tile.HasTile && tile.TileType == Type) {
                        tile.TileType = type;
                        tile.HasTile = active;
                    }
                }
            }
        }
    }
}