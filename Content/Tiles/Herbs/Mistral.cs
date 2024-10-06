using Aequus.Common;
using Aequus.Common.ContentTemplates.Generic;
using Aequus.Common.Drawing.TileAnimations;
using Aequus.Common.Entities.Tiles;
using Aequus.Common.Structures.ID;
using Aequus.Content.Systems.PotionAffixes.Empowered;
using System;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.Herbs;

[LegacyName("MistralTile")]
public class Mistral : UnifiedHerb, IDrawWindyGrass {
    public readonly ModItem Seeds;

    public Mistral() {
        Seeds = new InstancedTileItem(this, Settings: new() { Research = 25 });
    }

    public override void Load() {
        Mod.AddContent(Seeds);
        Mod.AddContent(new InstancedHangingPot(new LazyId(() => Instance<EmpoweredPrefix>().Item.Type), "MistralPot", AequusTextures.MistralPot.FullPath));
        Mod.AddContent(new InstancedPlanterBox("MistralPlanterBox", AequusTextures.MistralPlanterBox.FullPath, new() {
            SellCondition = AequusConditions.DownedDustDevil,
        }));
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
            ModContent.TileType<Meadows.MeadowGrass>(),
        ];
        obj.CoordinateWidth = 16;
        obj.CoordinateHeights = [28];
        obj.DrawYOffset = -10;

        Settings.PlantDrop = Instance<EmpoweredPrefix>().Item.Type;
        Settings.SeedDrop = Seeds.Type;
        Settings.BloomParticleColor = new Color(45, 150, 35);

        AddMapEntry(new Color(186, 122, 255), CreateMapEntryName());

        DustType = DustID.Grass;
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

        if (!TileDrawing.IsVisible(tile) || Main.instance.TilesRenderer.ShouldSwayInWind(i, j, tile) || GetState(i, j) != HerbState.Bloom) {
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
        spriteBatch.Draw(texture, drawCoordinates, frame with { Y = FullFrameHeight }, Color.White, 0f, origin, 1f, effects, 0f);
        return false;
    }

    bool IDrawWindyGrass.Draw(TileDrawCall drawInfo) {
        if (GetState(drawInfo.X, drawInfo.Y) != HerbState.Bloom) {
            return true;
        }

        Vector2 rayPosition = drawInfo.Position - new Vector2(0f, drawInfo.Origin.Y - 8f).RotatedBy(drawInfo.Rotation);
        drawInfo.DrawSelf();
        (drawInfo with { Frame = drawInfo.Frame with { Y = FullFrameHeight }, Color = Color.White }).DrawSelf();

        return false;
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
}
