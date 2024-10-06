using Aequus.Common;
using Aequus.Common.ContentTemplates.Generic;
using Aequus.Common.Drawing.TileAnimations;
using Aequus.Common.Entities.Tiles;
using Aequus.Common.Structures.ID;
using Aequus.Content.Systems.PotionAffixes.Splash;
using System.Collections.Generic;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.Herbs;

[LegacyName("MorayTile")]
public class Moray : UnifiedHerb, IDrawWindyGrass {
    public readonly ModItem Seeds;

    public readonly HashSet<int> Growable = [];

    public Moray() {
        Seeds = new InstancedTileItem(this, Settings: new() { Research = 25 });
    }

    public override void Load() {
        Mod.AddContent(Seeds);
        Mod.AddContent(new InstancedHangingPot(new LazyId(() => Instance<SplashPrefix>().Item.Type), "MorayPot", AequusTextures.MorayPot.FullPath));
        Mod.AddContent(new InstancedPlanterBox("MorayPlanterBox", AequusTextures.MorayPlanterBox.FullPath, new() {
            SellCondition = AequusConditions.DownedCrabson,
        }));
        ModTypeLookup<ModItem>.RegisterLegacyNames(Seeds, "MoraySeeds");
    }

    public override void SetStaticDefaultsInner(TileObjectData obj) {
        Main.tileLighted[Type] = true;
        TileID.Sets.SwaysInWindBasic[Type] = true;

        obj.AnchorValidTiles = [
            TileID.Grass,
            TileID.HallowedGrass,
            ModContent.TileType<Meadows.MeadowGrass>(),
            TileID.Sand,
            TileID.HardenedSand,
            TileID.Sandstone,
            TileID.Pearlsand,
            TileID.HallowHardenedSand,
            TileID.HallowSandstone,
#if !CRAB_CREVICE_DISABLE
            ModContent.TileType<global::Aequus.Tiles.CrabCrevice.SedimentaryRockTile>(),
#endif
#if POLLUTED_OCEAN
            ModContent.TileType<PollutedOcean.PolymerSands.PolymerSand>(),
            ModContent.TileType<PollutedOcean.PolymerSands.PolymerSandstone>(),
#endif
        ];

        Growable.AddRange(obj.AnchorValidTiles[3..]);

        obj.CoordinateWidth = 20;
        obj.CoordinateHeights = [18];

        Settings.PlantDrop = Instance<SplashPrefix>().Item.Type;
        Settings.SeedDrop = Seeds.Type;
        Settings.BloomParticleColor = new Color(30, 50, 150);

        AddMapEntry(new Color(186, 122, 255), CreateMapEntryName());
        DustType = DustID.BrownMoss;
    }

    [Gen.AequusTile_RandomUpdate]
    internal static void OnRandomUpdate(int i, int j, int type, int wall) {
        Tile tile = Framing.GetTileSafely(i, j);
        if (tile.LiquidAmount < 255 || tile.LiquidType != LiquidID.Water || !WorldGen.oceanDepths(i, (int)Main.worldSurface) || !Instance<Moray>().Growable.Contains(type)) {
            return;
        }

        Tile above = Framing.GetTileSafely(i, j - 1);
        int plantType = ModContent.TileType<Moray>();

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
        Rectangle frame = new Rectangle(tile.TileFrameX + FrameWidth - 1, tile.TileFrameY, FrameWidth, 30);
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

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        if (GetState(i, j) != HerbState.Bloom) {
            return;
        }

        r = 0.2f;
        g = 0.05f;
        b = 0.66f;
    }

    protected override bool BloomConditionsMet(int i, int j) {
        return Main.raining && !Main.dayTime;
    }
}
