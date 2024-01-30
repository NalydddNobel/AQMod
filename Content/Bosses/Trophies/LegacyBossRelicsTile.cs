using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    public override string Texture => AequusTextures.Tile(TileID.MasterTrophyBase);

    private const int FrameWidth = 18 * 3;
    private const int FrameHeight = 18 * 4;

    public const int OmegaStarite = 0;
    public const int Crabson = 1;
    public const int RedSprite = 2;
    public const int SpaceSquid = 3;
    public const int DustDevil = 4;
    public const int UltraStarite = 5;
    public const int FrameCount = 6;

    public static Dictionary<int, ModTile> LegacyConverter { get; private set; } = new();

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

        AdjTiles = new int[] { TileID.MasterTrophyBase, };

        AddMapEntry(new Color(233, 207, 94, 255), Language.GetText("MapObject.Relic"));
    }

    public override void Unload() {
        LegacyConverter.Clear();
    }

    public override bool CreateDust(int i, int j, ref int type) {
        return false;
    }

    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) {
        if (drawData.tileFrameX % FrameWidth == 0 && drawData.tileFrameY % FrameHeight == 0) {
            int legacyConvertId = drawData.tileFrameX / FrameWidth;

            ushort type;
            bool active;
            if (LegacyConverter.TryGetValue(legacyConvertId, out var modRelicInstance)) {
                type = modRelicInstance.Type;
                active = true;
            }
            // Weird exception, just delete this relic entirely
            else {
                type = 0;
                active = false;
            }

            for (int x = i; x < i + 3; x++) {
                for (int y = j; y < j + 4; y++) {
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