using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequus.Content.Bosses.Trophies;

[LegacyName("BossRelics", "BossRelicsTile")]
[Obsolete("Replaced with instanced Mod Tiles for each relic.")]
public class LegacyBossRelicsTile : ModTile {
    public override String Texture => AequusTextures.Tile(TileID.MasterTrophyBase);

    private const Int32 FrameWidth = 18 * 3;
    private const Int32 FrameHeight = 18 * 4;

    public const Int32 OmegaStarite = 0;
    public const Int32 Crabson = 1;
    public const Int32 RedSprite = 2;
    public const Int32 SpaceSquid = 3;
    public const Int32 DustDevil = 4;
    public const Int32 UltraStarite = 5;
    public const Int32 FrameCount = 6;

    public static Dictionary<Int32, ModTile> LegacyConverter { get; private set; } = new();

    public override void SetStaticDefaults() {
        Main.tileShine[Type] = 400;
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.InteractibleByNPCs[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
        TileObjectData.newTile.StyleHorizontal = false;
        TileObjectData.newTile.StyleWrapLimitVisualOverride = 2;
        TileObjectData.newTile.StyleMultiplier = 2;
        TileObjectData.newTile.StyleWrapLimit = 2;
        TileObjectData.newTile.styleLineSkipVisualOverride = 0;

        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
        TileObjectData.addAlternate(1);
        TileObjectData.addTile(Type);

        AdjTiles = new Int32[] { TileID.MasterTrophyBase, };

        AddMapEntry(new Color(233, 207, 94, 255), Language.GetText("MapObject.Relic"));
    }

    public override void Unload() {
        LegacyConverter.Clear();
    }

    public override Boolean CreateDust(Int32 i, Int32 j, ref Int32 type) {
        return false;
    }

    public override void DrawEffects(Int32 i, Int32 j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) {
        if (drawData.tileFrameX % FrameWidth == 0 && drawData.tileFrameY % FrameHeight == 0) {
            Int32 legacyConvertId = drawData.tileFrameX / FrameWidth;

            UInt16 type;
            Boolean active;
            if (LegacyConverter.TryGetValue(legacyConvertId, out var modRelicInstance)) {
                type = modRelicInstance.Type;
                active = true;
            }
            // Weird exception, just delete this relic entirely
            else {
                type = 0;
                active = false;
            }

            for (Int32 x = i; x < i + 3; x++) {
                for (Int32 y = j; y < j + 4; y++) {
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