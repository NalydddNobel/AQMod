using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequu2.Old.Content.Tiles;

[LegacyName("Trophies", "BossTrophiesTile")]
[Obsolete("Replaced with instanced Mod Tiles for each trophy.")]
public class LegacyBossTrophiesTile : ModTile {
    public const int OmegaStarite = 0;
    public const int Crabson = 1;
    public const int RedSprite = 2;
    public const int SpaceSquid = 3;
    public const int DustDevil = 4;
    public const int UltraStarite = 5;

    public static Dictionary<int, ModTile> LegacyConverter { get; private set; } = new();

    public override string Texture => Aequu2Textures.Tile(TileID.Painting3X3);

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

    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) {
        if (drawData.tileFrameX % 54 == 0 && drawData.tileFrameY % 54 == 0) {
            int legacyConvertId = drawData.tileFrameX / 54;

            ushort type;
            bool active;
            if (LegacyConverter.TryGetValue(legacyConvertId, out var modTrophyInstance)) {
                type = modTrophyInstance.Type;
                active = true;
            }
            // Weird exception, just delete this trophy entirely
            else {
                type = 0;
                active = false;
            }

            for (int x = i; x < i + 3; x++) {
                for (int y = j; y < j + 3; y++) {
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