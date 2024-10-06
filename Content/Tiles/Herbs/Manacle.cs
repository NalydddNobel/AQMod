using Aequus.Common;
using Aequus.Common.ContentTemplates.Generic;
using Aequus.Common.Drawing.TileAnimations;
using Aequus.Common.Entities.Tiles;
using Aequus.Common.Structures.ID;
using Aequus.Content.Systems.PotionAffixes.Bounded;
using System;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.Herbs;

[LegacyName("ManacleTile")]
public class Manacle : UnifiedHerb, IDrawWindyGrass {
    public readonly ModItem Seeds;

    readonly Color GlowColor = new Color(60, 60, 60, 0);

    public Manacle() {
        Seeds = new InstancedTileItem(this, Settings: new() { Research = 25 });
    }

    public override void Load() {
        Mod.AddContent(Seeds);
        Mod.AddContent(new InstancedHangingPot(new LazyId(() => Instance<BoundedPrefix>().Item.Type), "ManaclePot", AequusTextures.ManaclePot.FullPath));
        Mod.AddContent(new InstancedPlanterBox("ManaclePlanterBox", AequusTextures.ManaclePlanterBox.FullPath, new() {
            SellCondition = AequusConditions.DownedDemonSiege,
        }));
        ModTypeLookup<ModItem>.RegisterLegacyNames(Seeds, "ManacleSeeds");
    }

    public override void SetStaticDefaultsInner(TileObjectData obj) {
        Main.tileLighted[Type] = true;
        TileID.Sets.SwaysInWindBasic[Type] = true;

        obj.AnchorValidTiles = [
            TileID.Grass,
            TileID.HallowedGrass,
            ModContent.TileType<Meadows.MeadowGrass>(),
            TileID.Ash,
            TileID.AshGrass,
            TileID.Obsidian,
            TileID.ObsidianBrick,
            TileID.AncientObsidianBrick,
            TileID.Hellstone,
            TileID.HellstoneBrick,
            TileID.AncientHellstoneBrick,
        ];
        obj.CoordinateWidth = 20;
        obj.CoordinateHeights = [24];
        obj.DrawYOffset = -6;

        Settings.PlantDrop = Instance<BoundedPrefix>().Item.Type;
        Settings.SeedDrop = Seeds.Type;
        Settings.BloomParticleColor = new Color(255, 50, 150);

        ItemID.Sets.IsLavaImmuneRegardlessOfRarity[Seeds.Type] = true;

        AddMapEntry(new Color(75, 2, 17), CreateMapEntryName());
        DustType = DustID.Blood;
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        r = 0.66f;
        g = 0.15f;
        b = 0.1f;
    }

    [Gen.AequusTile_RandomUpdate]
    internal static void OnRandomUpdate(int i, int j, int type, int wall) {
        if (j < Main.rockLayer || (Main.remixWorld && Helper.WithinRemixSafeUnderworldRange(i))) {
            return;
        }

        switch (type) {
            case TileID.Obsidian:
            case TileID.ObsidianBrick:
            case TileID.Hellstone:
            case TileID.HellstoneBrick:
                Tile tile = Framing.GetTileSafely(i, j);
                Tile above = Framing.GetTileSafely(i, j - 1);
                int manacleType = ModContent.TileType<Manacle>();

                if (tile.Slope != SlopeType.Solid || tile.IsHalfBlock || tile.IsActuated || above.HasTile || TileHelper.ScanTilesSquare(i, j, 25, TileHelper.HasTileAction(manacleType))) {
                    return;
                }

                WorldGen.PlaceTile(i, j - 1, manacleType, mute: true);
                above.CopyPaintAndCoating(tile);

                if (Main.netMode != NetmodeID.SinglePlayer) {
                    NetMessage.SendTileSquare(-1, i, j - 1, 3, 3);
                }
                break;
        }
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        Tile tile = Main.tile[i, j];

        if (Main.instance.TilesRenderer.ShouldSwayInWind(i, j, tile) || GetState(i, j) != HerbState.Bloom || !TileDrawing.IsVisible(tile)) {
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
        Rectangle frame = new Rectangle(tile.TileFrameX + FullFrameWidth, tile.TileFrameY, FrameWidth, FullFrameHeight);
        Vector2 drawCoordinates = groundPosition + offset;
        Vector2 origin = new Vector2(FrameWidth / 2f, frame.Height - 2f);
        spriteBatch.Draw(texture, drawCoordinates, frame, Lighting.GetColor(i, j), 0f, origin, 1f, effects, 0f);

        DrawGlowmask(spriteBatch, texture, drawCoordinates, frame, 0f, origin, effects);

        return false;
    }

    bool IDrawWindyGrass.Draw(TileDrawCall drawInfo) {
        if (GetState(drawInfo.X, drawInfo.Y) != HerbState.Bloom) {
            return true;
        }

        drawInfo.DrawSelf();

        DrawGlowmask(drawInfo.SpriteBatch, drawInfo.Texture, drawInfo.Position, drawInfo.Frame, drawInfo.Rotation, drawInfo.Origin, drawInfo.SpriteEffects);

        return false;
    }

    void DrawGlowmask(SpriteBatch spriteBatch, Texture2D texture, Vector2 drawCoordinates, Rectangle frame, float rotation, Vector2 origin, SpriteEffects effects) {
        frame = frame with { Y = frame.Y + FullFrameHeight };
        for (int k = 0; k < 4; k++) {
            spriteBatch.Draw(texture, drawCoordinates + (k * MathHelper.PiOver2 + Main.GlobalTimeWrappedHourly * 2.5f).ToRotationVector2() * MathF.Sin(Main.GlobalTimeWrappedHourly * 0.61f) * 2f, frame, GlowColor, rotation, origin, 1f, effects, 0f);
        }
    }

    protected override bool BloomConditionsMet(int i, int j) {
        return Main.dayTime && Main.time < 17100;
    }
}
