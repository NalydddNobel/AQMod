using Aequus.Common.ContentTemplates.Generic;
using Aequus.Common.Drawing.TileAnimations;
using Aequus.Common.Entities.Tiles;
using Aequus.Content.Tiles.Meadows;
using Aequus.Items.Potions.Pollen;
using System;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.Herbs;

[LegacyName("MistralTile")]
public class Mistral : UnifiedHerb, IDrawWindyGrass {
    public readonly ModItem Seeds;

    public Mistral() {
        Seeds = new InstancedTileItem(this);
    }

    public override void Load() {
        Mod.AddContent(Seeds);
        ModTypeLookup<ModItem>.RegisterLegacyNames(Seeds, "MistralSeeds");
    }

    public override void SetStaticDefaultsInner(TileObjectData obj) {
        TileID.Sets.SwaysInWindBasic[Type] = true;

        obj.AnchorValidTiles = [
            TileID.Cloud,
            TileID.RainCloud,
            TileID.SnowCloud,
            TileID.Grass,
            TileID.HallowedGrass,
            ModContent.TileType<MeadowGrass>(),
        ];
        obj.CoordinateWidth = 26;
        obj.CoordinateHeights = [30];
        obj.DrawYOffset = -10;

        Settings.PlantDrop = ModContent.ItemType<MistralPollen>();
        Settings.SeedDrop = Seeds.Type;
        Settings.BloomParticleColor = new Color(45, 150, 35);

        AddMapEntry(new Color(75, 2, 17), CreateMapEntryName());
    }

    [Gen.AequusTile_RandomUpdate]
    internal static void OnRandomUpdate(int i, int j, int type, int wall) {
        if (j > Main.worldSurface) {
            return;
        }

        switch (type) {
            case TileID.Cloud:
            case TileID.RainCloud:
            case TileID.SnowCloud:
                Tile tile = Framing.GetTileSafely(i, j);
                Tile above = Framing.GetTileSafely(i, j - 1);
                int mistralType = ModContent.TileType<Mistral>();

                if (tile.Slope != SlopeType.Solid || tile.IsHalfBlock || tile.IsActuated || above.HasTile || TileHelper.ScanTilesSquare(i, j, 25, TileHelper.HasTileAction(mistralType))) {
                    return;
                }

                WorldGen.PlaceTile(i, j - 1, mistralType, mute: true);
                above.CopyPaintAndCoating(tile);

                if (Main.netMode != NetmodeID.SinglePlayer) {
                    NetMessage.SendTileSquare(-1, i, j - 1, 3, 3);
                }
                break;
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
        DrawPinwheel(i, j, spriteBatch, texture, rayPosition);

        return false;
    }

    void DrawPinwheel(int i, int j, SpriteBatch spriteBatch, Texture2D texture, Vector2 position) {
        int rotation = Main.tileFrame[Type];
        Rectangle frame = new Rectangle(54, 34, 28, 28);
        spriteBatch.DrawAlign(texture, position, frame, Lighting.GetColor(i, j), rotation / 16f * MathHelper.TwoPi, 1f, SpriteEffects.None);
    }

    public override void AnimateTile(ref int frame, ref int frameCounter) {
        frameCounter += (int)Math.Abs(Main.windSpeedCurrent * 6f);
        if (frameCounter > 3) {
            frameCounter = 0;
            frame++;
            if (frame >= 16 || frame <= 0) {
                frame = 0;
            }
        }
    }

    protected override bool BloomConditionsMet(int i, int j) {
        return Main.WindyEnoughForKiteDrops;
    }

    bool IDrawWindyGrass.Draw(TileDrawCall drawInfo) {
        if (GetState(drawInfo.X, drawInfo.Y) != HerbState.Bloom) {
            return true;
        }

        Vector2 rayPosition = drawInfo.Position - new Vector2(0f, drawInfo.Origin.Y - 8f).RotatedBy(drawInfo.Rotation);
        drawInfo.DrawSelf();
        DrawPinwheel(drawInfo.X, drawInfo.Y, drawInfo.SpriteBatch, drawInfo.Texture, rayPosition);

        return false;
    }
}
