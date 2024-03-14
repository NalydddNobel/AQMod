using Aequus.Common.Buffs;
using Aequus.Core.ContentGeneration;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Aequus.Content.Weather.Rain;

public class RainTotem : ModTile {
    public static ModItem DropItem { get; private set; }
    public static ModBuff Buff { get; private set; }

    public static int RainTotemCount { get; internal set; }
    /// <summary>Amount of Rain Totems on screen required to reach <see cref="BonusRainChanceMax"/></summary>
    public static int RainTotemMax { get; set; } = 9;

    /// <summary>The extra rain chance per tick granted by a single totem. Extra totems interpolate between this and <see cref="BonusRainChanceMax"/> using the ratio between <see cref="RainTotemCount"/>/<see cref="RainTotemMax"/>.</summary>
    public static int BonusRainChanceMin { get; set; } = 86400; // Rain chance from slaying a lady bug.
    /// <summary>The extra rain chance per tick granted by having maximum totems on screen.</summary>
    public static int BonusRainChanceMax { get; set; } = 3600; // This chance should practically guarantee rain in 1 minute.

    public override void Load() {
        DropItem = new InstancedTileItem(this, rarity: ItemRarityID.Blue, value: Item.silver, researchSacrificeCount: 3);
        Mod.AddContent(DropItem);

        Aequus.OnAddRecipes += () => {
            DropItem.CreateRecipe()
                .AddIngredient(ItemID.BambooBlock, 30)
                .AddIngredient(ItemID.RainCloud, 10)
#if DEBUG
                .AddIngredient(ItemID.FallenStar)
#else
                .AddIngredient(Old.Content.Materials.Energies.EnergyMaterial.Aquatic.Type)
#endif
                .AddTile(TileID.Sawmill)
                .Register()
                .SortBeforeFirstRecipesOf(ItemID.GoblinBattleStandard);
        };
    }

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.AlternateTile, 2, 0);
        // Stupid
        TileObjectData.newTile.AnchorAlternateTiles = new int[] {
            TileID.Mud, TileID.JungleGrass, TileID.CorruptJungleGrass, TileID.CrimsonJungleGrass,
            TileID.BambooBlock, TileID.LargeBambooBlock,
            Type,
        };
        TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(PlacementPreviewHook_CheckIfCanPlace, 1, 0, true);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.StyleWrapLimit = 3;
        TileObjectData.newTile.RandomStyleRange = 3;
        TileObjectData.addTile(Type);

        DustType = DustID.JunglePlants;
        AdjTiles = new int[] { TileID.Extractinator };
        AddMapEntry(new Color(89, 98, 40), CreateMapEntryName());
    }

    public int PlacementPreviewHook_CheckIfCanPlace(int x, int y, int type, int style = 0, int direction = 1, int alternate = 0) {
        int l = y + 1;
        for (int i = 0; i < 2; i++) {
            int k = x + i;
            Tile tile = Main.tile[k, l];
            //Dust.NewDust(new Vector2(k, l).ToWorldCoordinates(), 2, 2, DustID.Torch);
            if (tile.TileType == Type) {
                if (tile.TileFrameX % 36 != (i * 18)) {
                    return 1;
                }

                continue;
            }

            if (!tile.IsSolid()) {
                return 1;
            }
        }
        return 0;
    }

    public override void Unload() {
        DropItem = null;
    }

    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
        Tile originalTile = Main.tile[i, j];
        int tileFrameY = originalTile.TileFrameY;
        int tileFrameYOffset = originalTile.TileFrameY % 36;

        int left = i - originalTile.TileFrameX % 36 / 18;
        int top = j - tileFrameYOffset / 18;

        int wantedFrameY = GetFrameY(left, top) * 36;
        for (int x = left; x < left + 2; x++) {
            for (int y = top; y < top + 2; y++) {
                Main.tile[x, y].TileFrameY = (short)(wantedFrameY + Main.tile[x, y].TileFrameY % 36);
            }
        }

        return base.TileFrame(i, j, ref resetFrame, ref noBreak);
    }

    private int GetFrameY(int left, int top) {
        for (int m = 0; m < 2; m++) {
            int x = left + m;
            int y = top - 1;
            Tile checkTile = Framing.GetTileSafely(x, y);
            if (!checkTile.HasUnactuatedTile && !checkTile.IsFullySolid() && checkTile.TileType != Type) {
                return 0; // Top frame
            }
        }

        //for (int m = 0; m < 2; m++) {
        //    int x = left + m;
        //    int y = top - 1;
        //    Tile checkTile = Framing.GetTileSafely(x, y);
        //    if (checkTile.HasTile && checkTile.TileType != Type) {
        //        return 4; // Bottom Frame
        //    }
        //}

        return WorldGen.genRand.Next(1, 4); // Middle frames
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        Tile tile = Main.tile[i, j];
        if (!TileDrawing.IsVisible(tile)) {
            return false;
        }
        //if (tile.TileFrameY % 36 != 0 && tile.TileFrameY % 36 != 18) {
        //    Main.NewText(tile.TileFrameY);
        //}

        Vector2 drawCoordinates = new Vector2(i * 16f, j * 16f + 2f) - Main.screenPosition + TileHelper.DrawOffset;
        Rectangle frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
        Color lightColor = TileHelper.TileColor(i, j, tile);
        spriteBatch.Draw(TextureAssets.Tile[Type].Value, drawCoordinates, frame, lightColor);

        //if (!Main.raining) {
        //    float brightest = Math.Max(Math.Max(lightColor.R, lightColor.G), lightColor.B) / 255f;
        //    Color drawColor = Color.Cyan with { A = 0 } * Helper.Oscillate(Main.GlobalTimeWrappedHourly, 0.8f, 1f) * brightest;
        //    spriteBatch.Draw(AequusTextures.RainTotem_Glow, drawCoordinates, frame, drawColor);
        //}
        return false;
    }

    public override IEnumerable<Item> GetItemDrops(int i, int j) {
        yield return new Item(DropItem.Type);
    }

    private static int _rainTotemTick;
    private static int _rainTotemCount;

    public static void UpdateRainState() {
        if (_rainTotemTick++ < Main.CurrentFrameFlags.ActivePlayersCount) {
            return;
        }

        _rainTotemTick = 0;
        _rainTotemCount = CountRainTotems(RainTotemMax);

        if (_rainTotemCount > 0) {
            // Subtract 1 since _rainTotemCount must be greater than 0, but we'd like to start at 0 progress.
            // Also subtract 1 from the count, since the totem max will never reach the real wanted count.
            // Restrict the value between 0 and 1 for good measure.
            float totemCountRatio = Math.Clamp((_rainTotemCount - 1f) / (RainTotemMax - 1f), 0f, 1f);

            // Interpolate between the min and max values using the calculated ratio
            double chance = MathHelper.Lerp(BonusRainChanceMin, BonusRainChanceMax, totemCountRatio) / Main.desiredWorldEventsUpdateRate;

            // Start rain randomly using the chance value
            if (Main.rand.NextDouble() < 1.0 / chance) {
                Main.StartRain();
            }
        }
    }

    private static int CountRainTotems(int max) {
        int count = 0;
        for (int i = 0; i < Main.maxPlayers; i++) {
            if (Main.player[i].active && Main.player[i].TryGetModPlayer(out RainPlayer rainPlayer)) {
                count += rainPlayer.rainTotems;

                if (count >= max) {
                    return max;
                }
            }
        }

        return count;
    }
}
