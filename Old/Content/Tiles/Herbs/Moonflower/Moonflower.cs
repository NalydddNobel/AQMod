using Aequu2.Core.Graphics.Tiles;
using Aequu2.Old.Content.Items.Potions.Prefixes.StuffedPotions;
using System;
using Terraria.Audio;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Aequu2.Old.Content.Tiles.Herbs.Moonflower;

[LegacyName("MoonflowerTile")]
public class Moonflower : ModHerb, IDrawWindyGrass {
    public static double BloomTimeMin { get; set; } = Main.nightLength / 2 - 3600;
    public static double BloomTimeMax { get; set; } = Main.nightLength / 2 + 3600;

    public static Vector3 GlowColor { get; set; } = new Vector3(0.45f, 0.05f, 1f);

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        Tile tile = Main.tile[i, j];

        if (Main.instance.TilesRenderer.ShouldSwayInWind(i, j, tile) || GetGrowthStage(i, j) != STAGE_BLOOMING) {
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
        if (TileDrawing.IsVisible(tile)) {
            spriteBatch.Draw(texture, groundPosition + offset, frame, Lighting.GetColor(i, j), 0f, new Vector2(FrameWidth / 2f, frame.Height - 2f), 1f, effects, 0f);
        }

        Vector2 rayPosition = groundPosition + offset + new Vector2(0f, -20f);
        DrawLightFlare(i, j, spriteBatch, rayPosition);

        return false;
    }

    public bool DrawWindyGrass(TileDrawCall drawInfo) {
        if (GetGrowthStage(drawInfo.X, drawInfo.Y) != STAGE_BLOOMING) {
            return true;
        }

        Vector2 rayPosition = drawInfo.Position - new Vector2(0f, drawInfo.Origin.Y - 8f).RotatedBy(drawInfo.Rotation);
        drawInfo.DrawSelf();
        DrawLightFlare(drawInfo.X, drawInfo.Y, drawInfo.SpriteBatch, rayPosition);

        return false;
    }

    private static void DrawLightFlare(int i, int j, SpriteBatch spriteBatch, Vector2 position) {
        Texture2D bloom = Aequu2Textures.BloomStrong;
        Texture2D ray = Aequu2Textures.MoonflowerEffect;
        float wave = Helper.Oscillate(Main.GlobalTimeWrappedHourly * 2f, 1f, 1.25f);
        float hueWave = MathF.Sin(Main.GlobalTimeWrappedHourly * 7.1f + i);
        Color rayColor = new Color(120, 100, 25).HueAdd(hueWave * 0.04f - 0.02f) with { A = 0 } * wave;
        Vector2 rayScale = new Vector2(1f, 0.5f) * wave;
        spriteBatch.Draw(bloom, position, null, rayColor * 0.1f, 0f, bloom.Size() / 2f, 0.2f, SpriteEffects.None, 0f);
        spriteBatch.Draw(bloom, position, null, rayColor * 0.2f, 0f, bloom.Size() / 2f, 0.7f, SpriteEffects.None, 0f);
        spriteBatch.Draw(ray, position, null, rayColor, 0f, ray.Size() / 2f, rayScale, SpriteEffects.None, 0f);
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        Vector3 clr = GlowColor * Math.Min(GetGrowthStage(i, j) * 0.3f, 0.6f);

        r = clr.X;
        g = clr.Y;
        b = clr.Z;
    }

    protected override bool IsBlooming(int i, int j) {
        return !Main.dayTime && Main.time > BloomTimeMin && Main.time < BloomTimeMax;
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
                var d = Dust.NewDustDirect(new Vector2(i * 16f, j * 16f), 16, 16, DustID.ShadowbeamStaff);
                var n = (MathHelper.TwoPi / 12f * k + Main.rand.NextFloat(-0.15f, 0.15f)).ToRotationVector2();
                d.position = center + n * 4f;
                d.velocity = n * 7.5f;
            }
        }

        for (int k = 0; k < petals; k++) {
            Dust.NewDust(new Vector2(i * 16f, j * 16f), 16, 16, DustID.Grubby);
        }

        return false;
    }

    public override bool KillSound(int i, int j, bool fail) {
        if (GetGrowthStage(i, j) == STAGE_BLOOMING) {
            SoundEngine.PlaySound(Aequu2Sounds.MoonflowerBreak with { PitchVariance = 0.1f }, new Vector2(i * 16f, j * 16f));
            //return false;
        }

        return base.KillSound(i, j, fail);
    }

    protected override void GetDropParams(int growthStage, Player closestPlayer, Item playerHeldItem, out int plant) {
        plant = ModContent.GetInstance<StuffedPrefix>().Item.Type;
    }

    protected override void SafeSetStaticDefaults() {
        Main.tileLighted[Type] = true;
        TileObjectData.newTile.CoordinateWidth = 26;
        TileObjectData.newTile.CoordinateHeights = new int[] { 30 };
        TileObjectData.newTile.DrawYOffset = -12;
        TileObjectData.newTile.AnchorValidTiles = new int[] {
            TileID.Meteorite,
        };

        AddMapEntry(new Color(186, 122, 255), CreateMapEntryName());
    }
}
