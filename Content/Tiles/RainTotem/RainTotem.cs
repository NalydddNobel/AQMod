using Aequus.Core.ContentGeneration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.RainTotem;

public class RainTotem : RainTotemTileTemplate {
    #region Update
    private static int _rainTotemTick;
    private static int _rainTotemCount;

    public static void UpdateRainState() {
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

        if ((_rainTotemTick += (int)Main.desiredWorldEventsUpdateRate) < Main.CurrentFrameFlags.ActivePlayersCount) {
            return;
        }

        _rainTotemTick = 0;
        _rainTotemCount = CountRainTotems(RainTotemMax);
    }

    private static int CountRainTotems(int max) {
        int count = 0;
        for (int i = 0; i < Main.maxPlayers; i++) {
            if (Main.player[i].active && Main.player[i].TryGetModPlayer(out RainTotemPlayer rainPlayer)) {
                count += rainPlayer.rainTotems;

                if (count >= max) {
                    return max;
                }
            }
        }

        return count;
    }
    #endregion

    #region Initialization
    public static ModItem DropItem { get; private set; }

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

        Aequus.OnAddRecipes += AddRecipes;

        void AddRecipes() {
            DropItem.CreateRecipe()
                .AddIngredient(ItemID.BambooBlock, 30)
                .AddIngredient(ItemID.RainCloud, 10)
            #if DEBUG
                            .AddIngredient(ItemID.FallenStar)
            #else
                .AddIngredient(Old.Content.Items.Materials.Energies.EnergyMaterial.Aquatic.Type)
            #endif
                .AddTile(TileID.Sawmill)
                .Register()
                .SortBeforeFirstRecipesOf(ItemID.GoblinBattleStandard);
        }
    }

    public override void Unload() {
        DropItem = null;
    }
    #endregion

    public override void CustomPreDraw(SpriteBatch spriteBatch, Vector2 drawCoordinates, Rectangle frame, Color lightColor) {
        //if (Main.raining) {
        //    return;
        //}

        float brightest = Math.Max(Math.Max(Math.Max(lightColor.R, lightColor.G), lightColor.B) / 255f, 0.2f);
        Color drawColor = Color.Cyan with { A = 0 } * Helper.Oscillate(Main.GlobalTimeWrappedHourly, 0.8f, 1f) * brightest;
        spriteBatch.Draw(AequusTextures.RainTotem_Glow, drawCoordinates, frame, drawColor);
    }

    public override void TransformTileOnWireHit(int i, int j, Tile tile) {
        tile.TileType = (ushort)ModContent.TileType<RainTotemInactive>();
    }
}

public class RainTotemInactive : RainTotemTileTemplate {
    public override string Texture => AequusTextures.RainTotem.Path;

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        AdjTiles = new int[] { ModContent.TileType<RainTotem>() };
    }

    public override void TransformTileOnWireHit(int i, int j, Tile tile) {
        tile.TileType = (ushort)ModContent.TileType<RainTotem>();
    }
}

[Browsable(browsable: false)]
public abstract class RainTotemTileTemplate : ModTile {
    #region Framing
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
    #endregion

    #region Drawing
    public sealed override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
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

        if (Main.InSmartCursorHighlightArea(i, j, out var actuallySelected)) {
            Color selectionColor = Colors.GetSelectionGlowColor(actuallySelected, (lightColor.R + lightColor.G + lightColor.B) / 3);
            spriteBatch.Draw(TextureAssets.HighlightMask[Type].Value, drawCoordinates, frame, selectionColor);
        }

        CustomPreDraw(spriteBatch, drawCoordinates, frame, lightColor);
        return false;
    }

    public virtual void CustomPreDraw(SpriteBatch spriteBatch, Vector2 drawCoordinates, Rectangle frame, Color lightColor) { }
    #endregion

    #region Wiring and Right Click
    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) {
        // Allow selection of this totem if it's at the bottom of your smart selection range.
        if (j > settings.HY - 1) {
            return true;
        }

        // Otherwise, check if it's not on top of another totem.
        int left = i - Main.tile[i, j].TileFrameX % 36 / 18;
        int top = j - Main.tile[i, j].TileFrameY % 36 / 18;
        int ground = top + 2;
        Tile groundTile = Framing.GetTileSafely(left, ground);
        return groundTile.TileType != Type;
    }

    public sealed override void MouseOver(int i, int j) {
        Player player = Main.LocalPlayer;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = RainTotem.DropItem.Type;
    }

    public sealed override bool RightClick(int i, int j) {
        HitWire(i, j);
        return true;
    }

    public sealed override void HitWire(int i, int j) {
        Tile tile = Main.tile[i, j];
        int left = i - tile.TileFrameX % 36 / 18;
        int top = j - tile.TileFrameY % 36 / 18;

        ScaleUpAndToggleTotems(left, top);

        SoundEngine.PlaySound(SoundID.Mech, new Vector2(i, j).ToWorldCoordinates());
    }

    public abstract void TransformTileOnWireHit(int i, int j, Tile tile);

    private void ScaleUpAndToggleTotems(int x, int y) {
        int totemsCount = 1;
        while (y > 0 && Main.tile[x, y].TileType == Type && totemsCount < RainTotem.RainTotemMax) {
            ToggleTotem(x, y);
            totemsCount++;
            y -= 2;
        }
        if (Main.netMode != NetmodeID.SinglePlayer) {
            NetMessage.SendTileSquare(-1, x, y, 2, 2 * totemsCount);
        }
    }

    private void ToggleTotem(int x, int y) {
        for (int i = x; i < x + 2; i++) {
            for (int j = y; j < y + 2; j++) {
                Wiring.SkipWire(i, j);
                TransformTileOnWireHit(i, j, Main.tile[i, j]);
            }
        }
    }
    #endregion

    #region Initialization
    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.AlternateTile, 2, 0);
        // Stupid
        TileObjectData.newTile.AnchorAlternateTiles = new int[] {
            TileID.Mud, TileID.JungleGrass, TileID.CorruptJungleGrass, TileID.CrimsonJungleGrass,
            TileID.LihzahrdBrick,
            TileID.MushroomGrass, TileID.MushroomBlock,
            TileID.RichMahogany, TileID.LivingMahogany, TileID.LivingMahoganyLeaves,
            TileID.BambooBlock, TileID.LargeBambooBlock,
            ModContent.TileType<RainTotem>(), ModContent.TileType<RainTotemInactive>(),
        };
        TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(PlacementPreviewHook_CheckIfCanPlace, 1, 0, true);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.StyleWrapLimit = 3;
        TileObjectData.newTile.RandomStyleRange = 3;
        TileObjectData.addTile(Type);

        DustType = DustID.JunglePlants;
        AddMapEntry(new Color(89, 98, 40), CreateMapEntryName());
    }

    private int PlacementPreviewHook_CheckIfCanPlace(int x, int y, int type, int style = 0, int direction = 1, int alternate = 0) {
        int l = y + 1;
        for (int i = 0; i < 2; i++) {
            int k = x + i;
            Tile tile = Main.tile[k, l];

            if (tile.Slope != 0 || tile.IsHalfBlock) {
                return 1;
            }
        }
        return 0;
    }
    #endregion

    public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

    public override IEnumerable<Item> GetItemDrops(int i, int j) {
        yield return new Item(RainTotem.DropItem.Type);
    }
}