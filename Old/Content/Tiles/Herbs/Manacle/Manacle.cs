using Aequu2.Core.Graphics.Tiles;
using Aequu2.Old.Content.Items.Potions.Prefixes.BoundedPotions;
using System;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Aequu2.Old.Content.Tiles.Herbs.Manacle;

public class Manacle : ModHerb, IDrawWindyGrass {
    private readonly Color GlowColor = new Color(60, 60, 60, 0);

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        Tile tile = Main.tile[i, j];

        if (Main.instance.TilesRenderer.ShouldSwayInWind(i, j, tile) || GetGrowthStage(i, j) != STAGE_BLOOMING || !TileDrawing.IsVisible(tile)) {
            return true;
        }

        Texture2D texture = Main.instance.TilePaintSystem.TryGetTileAndRequestIfNotReady(Type, 0, tile.TileColor);
        if (texture == null) {
            return true;
        }

        Vector2 offset = (TileHelper.DrawOffset - Main.screenPosition).Floor();
        Vector2 groundPosition = new Vector2(i * 16f + 8f, j * 16f + 16f).Floor();

        SpriteEffects effects = SpriteEffects.None;
        SetSpriteEffects(i, j, ref effects);
        Rectangle frame = new Rectangle(tile.TileFrameX + FrameWidth - 1, tile.TileFrameY, FrameWidth, 30);
        Vector2 drawCoordinates = groundPosition + offset;
        Vector2 origin = new Vector2(FrameWidth / 2f, frame.Height - 2f);
        spriteBatch.Draw(texture, drawCoordinates, frame, Lighting.GetColor(i, j), 0f, origin, 1f, effects, 0f);

        DrawGlowmask(spriteBatch, drawCoordinates, frame, 0f, origin, effects);

        return false;
    }

    public bool DrawWindyGrass(TileDrawCall drawInfo) {
        if (GetGrowthStage(drawInfo.X, drawInfo.Y) != STAGE_BLOOMING) {
            return true;
        }

        drawInfo.DrawSelf();

        DrawGlowmask(drawInfo.SpriteBatch, drawInfo.Position, drawInfo.Frame, drawInfo.Rotation, drawInfo.Origin, drawInfo.SpriteEffects);

        return false;
    }

    private void DrawGlowmask(SpriteBatch spriteBatch, Vector2 drawCoordinates, Rectangle? frame, float rotation, Vector2 origin, SpriteEffects effects) {
        for (int k = 0; k < 4; k++) {
            spriteBatch.Draw(Aequu2Textures.Manacle_Glow,
                drawCoordinates + (k * MathHelper.PiOver2 + Main.GlobalTimeWrappedHourly * 2.5f).ToRotationVector2() * MathF.Sin(Main.GlobalTimeWrappedHourly * 0.61f) * 2f, frame, GlowColor, rotation, origin, 1f, effects, 0f);
        }
    }

    protected override bool IsBlooming(int i, int j) {
        return Main.dayTime && Main.time < 17100;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = 1;
    }

    public override bool CreateDust(int i, int j, ref int type) {
        int petals = 3;
        int growthStage = GetGrowthStage(i, j);

        if (growthStage == STAGE_MATURE) {
            petals = 6;
        }
        else if (growthStage == STAGE_BLOOMING) {
            petals = 12;

            Vector2 center = new Vector2(i * 16f + 8f, j * 16f + 4f);
            for (int k = 0; k < 12; k++) {
                var d = Dust.NewDustDirect(new Vector2(i * 16f, j * 16f), 16, 16, DustID.Torch);
                var n = (MathHelper.TwoPi / 12f * k + Main.rand.NextFloat(-0.15f, 0.15f)).ToRotationVector2();
                d.position = center + n * 4f;
                d.velocity = n * 7.5f;
                d.noGravity = true;
            }
        }

        for (int k = 0; k < petals; k++) {
            Dust.NewDust(new Vector2(i * 16f, j * 16f), 16, 16, DustID.Blood);
        }

        return false;
    }

    public override bool KillSound(int i, int j, bool fail) {
        if (GetGrowthStage(i, j) == STAGE_BLOOMING) {
            SoundEngine.PlaySound(Aequu2Sounds.MoonflowerBreak with { PitchVariance = 0.1f }, new Vector2(i * 16f, j * 16f));
        }

        return base.KillSound(i, j, fail);
    }

    protected override void GetDropParams(int growthStage, Player closestPlayer, Item playerHeldItem, out int plant) {
        plant = ModContent.GetInstance<BoundedPrefix>().Item.Type;
    }

    protected override void SafeSetStaticDefaults() {
        // Set the seed as lava immune
        ItemSets.IsLavaImmuneRegardlessOfRarity[SeedItem.Type] = true;

        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.LavaPlacement = LiquidPlacement.Allowed;
        TileObjectData.newTile.CoordinateWidth = 26;
        TileObjectData.newTile.CoordinateHeights = new int[] { 30 };
        TileObjectData.newTile.DrawYOffset = -12;
        TileObjectData.newTile.AnchorValidTiles = new int[] {
            TileID.Ash,
            TileID.AshGrass,
            TileID.Hellstone,
        };
        TileObjectData.newTile.AnchorAlternateTiles = new int[] {
            TileID.Obsidian,
            TileID.ObsidianBrick,
            TileID.HellstoneBrick,
        };

        AddMapEntry(new Color(205, 80, 25), CreateMapEntryName());
    }

    public override bool CanNaturallyGrow(int X, int Y, Tile tile, bool[] anchoredTiles) {
        if (Main.remixWorld) {
            Point range = Aequu2System.RemixWorldSafeUnderworldRange;

            // Manacle plants cannot generate in the middle of remix seed worlds.
            if (X > range.X && X < range.Y && Y > Main.rockLayer) {
                return false;
            }
        }

        return TileHelper.ScanTilesSquare(X, Y, 20, TileHelper.HasTileAction(ModContent.TileType<Manacle>()));
    }
}
