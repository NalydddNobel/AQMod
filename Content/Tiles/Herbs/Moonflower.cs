using Aequus.Common;
using Aequus.Common.ContentTemplates.Generic;
using Aequus.Common.Drawing.TileAnimations;
using Aequus.Common.Entities.Tiles;
using Aequus.Common.Structures.ID;
using Aequus.Content.Systems.PotionAffixes.Stuffed;
using System;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.Herbs;

[LegacyName("MoonflowerTile")]
public class Moonflower : UnifiedHerb, IDrawWindyGrass {
    public readonly ModItem Seeds;

    public Moonflower() {
        Seeds = new InstancedTileItem(this, Settings: new() { Research = 25 });
    }

    public override void Load() {
        Mod.AddContent(Seeds);
        Mod.AddContent(new InstancedHangingPot(new LazyId(() => Instance<StuffedPrefix>().Item.Type), "MoonflowerPot", AequusTextures.MoonflowerPot.FullPath));
        Mod.AddContent(new InstancedPlanterBox("MoonflowerPlanterBox", AequusTextures.MoonflowerPlanterBox.FullPath, new() {
            SellCondition = AequusConditions.DownedOmegaStarite,
            LightColor = new Vector3(0.32f, 0.16f, 0.12f),
            GlowColor = new Color(100, 100, 100, 0)
        }));
        ModTypeLookup<ModItem>.RegisterLegacyNames(Seeds, "MoonflowerSeeds");
    }

    public override void SetStaticDefaultsInner(TileObjectData obj) {
        Main.tileLighted[Type] = true;
        TileID.Sets.SwaysInWindBasic[Type] = true;

        obj.AnchorValidTiles = [
            TileID.Meteorite,
            TileID.Grass,
            TileID.HallowedGrass,
            ModContent.TileType<Meadows.MeadowGrass>(),
        ];
        obj.CoordinateWidth = 26;
        obj.CoordinateHeights = [30];
        obj.DrawYOffset = -10;

        Settings.PlantDrop = Instance<StuffedPrefix>().Item.Type;
        Settings.SeedDrop = Seeds.Type;
        Settings.BloomParticleColor = new Color(150, 150, 30);

        AddMapEntry(new Color(186, 122, 255), CreateMapEntryName());
        DustType = DustID.Grubby;
    }

    [Gen.AequusTile_RandomUpdate]
    internal static void OnRandomUpdate(int i, int j, int type, int wall) {
        if (j > Main.worldSurface || type != TileID.Meteorite) {
            return;
        }

        Tile tile = Framing.GetTileSafely(i, j);
        Tile above = Framing.GetTileSafely(i, j - 1);
        int plantType = ModContent.TileType<Moonflower>();

        if (tile.Slope != SlopeType.Solid || tile.IsHalfBlock || tile.IsActuated || above.HasTile || TileHelper.ScanTilesSquare(i, j, 25, TileHelper.HasTileAction(plantType))) {
            return;
        }

        WorldGen.PlaceTile(i, j - 1, plantType, mute: true);
        above.CopyPaintAndCoating(tile);

        if (Main.netMode != NetmodeID.SinglePlayer) {
            NetMessage.SendTileSquare(-1, i, j - 1, 3, 3);
        }

    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        Tile tile = Main.tile[i, j];

        if (Main.instance.TilesRenderer.ShouldSwayInWind(i, j, tile) || GetState(i, j) != HerbState.Bloom) {
            return true;
        }

        Texture2D texture = Main.instance.TilesRenderer.GetTileDrawTexture(tile, i, j);
        if (texture == null) {
            return true;
        }

        Vector2 offset = (Helper.TileDrawOffset - Main.screenPosition).Floor();
        Vector2 groundPosition = new Vector2(i * 16f + 8f, j * 16f + 16f).Floor();

        SpriteEffects effects = SpriteEffects.None;
        SetSpriteEffects(i, j, ref effects);
        Rectangle frame = new Rectangle(tile.TileFrameX + FrameWidth - 1, tile.TileFrameY, FrameWidth, 30);
        Vector2 drawCoordinates = groundPosition + offset;
        Vector2 origin = new Vector2(FrameWidth / 2f, frame.Height - 2f);
        if (TileDrawing.IsVisible(tile)) {
            spriteBatch.Draw(texture, drawCoordinates, frame, Lighting.GetColor(i, j), 0f, origin, 1f, effects, 0f);
        }

        Vector2 rayPosition = groundPosition + offset + new Vector2(0f, -20f);
        DrawLightFlare(i, j, spriteBatch, rayPosition);

        return false;
    }

    bool IDrawWindyGrass.Draw(TileDrawCall drawInfo) {
        if (GetState(drawInfo.X, drawInfo.Y) != HerbState.Bloom) {
            return true;
        }

        Vector2 rayPosition = drawInfo.Position - new Vector2(0f, drawInfo.Origin.Y - 8f).RotatedBy(drawInfo.Rotation);
        drawInfo.DrawSelf();
        DrawLightFlare(drawInfo.X, drawInfo.Y, drawInfo.SpriteBatch, rayPosition);

        return false;
    }

    void DrawLightFlare(int i, int j, SpriteBatch spriteBatch, Vector2 position) {
        Texture2D bloom = AequusTextures.BloomStrong;
        Texture2D ray = AequusTextures.MoonflowerEffect;
        float wave = Helper.Wave(Main.GlobalTimeWrappedHourly * 2f, 1f, 1.25f);
        float hueWave = MathF.Sin(Main.GlobalTimeWrappedHourly * 7.1f + i);
        Color rayColor = new Color(120, 100, 25).HueAdd(hueWave * 0.04f - 0.02f) with { A = 0 } * wave;
        Vector2 rayScale = new Vector2(1f, 0.5f) * wave;
        spriteBatch.Draw(bloom, position, null, rayColor * 0.1f, 0f, bloom.Size() / 2f, 0.2f, SpriteEffects.None, 0f);
        spriteBatch.Draw(bloom, position, null, rayColor * 0.2f, 0f, bloom.Size() / 2f, 0.7f, SpriteEffects.None, 0f);
        spriteBatch.Draw(ray, position, null, rayColor, 0f, ray.Size() / 2f, rayScale, SpriteEffects.None, 0f);
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        if (Main.tile[i, j].TileFrameX <= FullFrameWidth) {
            return;
        }

        r = 0.45f;
        g = 0.05f;
        b = 1f;
    }

    protected override bool BloomConditionsMet(int i, int j) {
        return !Main.dayTime && Main.time > Main.nightLength / 2 - 3600 && Main.time < Main.nightLength / 2 + 3600;
    }
}
