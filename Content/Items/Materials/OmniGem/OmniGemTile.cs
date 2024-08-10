using Aequus;
using Aequus.Common.Drawing;
using Aequus.Common.Particles;
using Aequus.Common.Tiles.Global;
using Aequus.Items.Misc.Dyes;
using Aequus.Tiles.Base;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace Aequus.Content.Items.Materials.OmniGem;

public class OmniGemTile : BaseGemTile, ITileDrawSystem {
    public const int MaskFrameWidth = MaskFullWidth / 3;
    public const int MaskFullWidth = 150;

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        Main.tileLighted[Type] = true;
        Main.tileObsidianKill[Type] = true;
        Main.tileNoFail[Type] = true;

        AddMapEntry(new Color(222, 222, 222), Lang.GetItemName(ModContent.ItemType<OmniGem>()));
        DustType = DustID.RainbowRod;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = fail ? 1 : 7;
    }

    public override bool CreateDust(int i, int j, ref int type) {
        if (Main.netMode != NetmodeID.Server) {
            ParticleSystem.New<OmniGemParticle>(ParticleLayer.AboveDust)
                .Setup(
                    new Vector2(i * 16f + Main.rand.Next(16), j * 16f + Main.rand.Next(16)),
                    Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.2f, 1f) * 3.3f,
                    Color.White with { A = 0 },
                    (Color.Red.HueSet(Main.rand.NextFloat(1f)) * 0.4f).UseA(33),
                    Main.rand.NextFloat(0.3f, 0.8f),
                    0.225f,
                    0f
                );
        }
        return false;
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        r = 0.3f;
        g = 0.1f;
        b = 0.5f;
    }

    private static bool CheckShimmer(int i, int j, int rangeX, int rangeY, int iterations) {
        for (int k = 0; k < iterations; k++) {
            int randX = i + WorldGen.genRand.Next(-rangeX, rangeX);
            int randY = j + WorldGen.genRand.Next(-rangeY, rangeY);
            if (WorldGen.InWorld(i, j) && TileHelper.HasShimmer(randX, randY)) {
                return true;
            }
        }
        return false;
    }

    public static bool TryGrow(in GlobalRandomTileUpdateParams info) {
        const int RANGE_X = 60;
        const int RANGE_Y = 60;
        const int ITERATIONS = 30;
        if (!WorldGen.InWorld(info.X, info.Y, Math.Max(RANGE_X, RANGE_Y) + 10)) {
            return false;
        }

        int omniGemTileID = ModContent.TileType<OmniGemTile>();
        int generateX = info.X + WorldGen.genRand.Next(-1, 2);
        int generateY = info.Y + WorldGen.genRand.Next(2);
        var tile = Main.tile[generateX, generateY];
        if (tile.HasTile) {
            if (tile.SolidType() || TileID.Sets.IsVine[tile.TileType]) {
                return false;
            }
            if (tile.CuttableType()) {
                tile.HasTile = false;
            }
        }

        if (!CheckShimmer(info.X, info.Y + RANGE_Y / 2, RANGE_X, RANGE_Y, ITERATIONS) || TileHelper.ScanTiles(new(info.X - 2, info.Y - 1, 5, 3), TileHelper.HasTileAction(omniGemTileID), TileHelper.HasShimmer, TileHelper.IsTree)) {
            return false;
        }

        WorldGen.PlaceTile(generateX, generateY, omniGemTileID, mute: true);
        if (tile.HasTile && tile.TileType == omniGemTileID) {
            if (Main.netMode != NetmodeID.SinglePlayer) {
                NetMessage.SendTileSquare(-1, generateX, generateY);
            }
            return true;
        }
        return false;
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        return false;
    }

    public void DrawOmniGemGlowBehindTiles(SpriteBatch sb) {
        sb.End();
        sb.Begin_World(shader: true);

        var effect = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<HueshiftDye>());

        var maskTexture = AequusTextures.OmniGemTile_Mask.Value;
        var glowOffset = new Vector2(-1f, -1f);
        foreach (Point p in this.GetDrawPoints()) {
            Tile tile = Main.tile[p];
            if (tile.IsTileInvisible) {
                continue;
            }

            var frame = new Rectangle(tile.TileFrameX / 18 * MaskFullWidth, tile.TileFrameY / 18 * MaskFrameWidth, MaskFrameWidth, 50);
            var drawPosition = this.GetDrawPosition(p.X, p.Y, GetObjectData(p.X, p.Y));
            var origin = frame.Size() / 2f;
            float globalTime = Main.GlobalTimeWrappedHourly;
            Main.GlobalTimeWrappedHourly = OmniGem.GetGlobalTime(p.X, p.Y);

            effect.Apply(null, null);

            sb.Draw(
                TextureAssets.Tile[Type].Value,
                drawPosition,
                new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16),
                Color.White,
                0f,
                Vector2.Zero,
                1f, SpriteEffects.None, 0f
            );

            Main.GlobalTimeWrappedHourly = globalTime;
        }

        sb.End();
        sb.Begin_World(shader: false);
    }

    public void DrawOmniGemGlowAboveWaters(SpriteBatch sb) {
        sb.EndCached();
        sb.Begin_World(shader: true);

        var effect = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<HueshiftDye>());

        var texture = AequusTextures.OmniGemTile_Mask.Value;
        var glowOffset = new Vector2(7f, 7f);
        IEnumerable<Point> drawPoints = this.GetDrawPoints();
        foreach (Point p in drawPoints) {
            Tile tile = Main.tile[p];
            if (tile.IsTileInvisible) {
                continue;
            }

            ulong seed = Helper.TileSeed(p.X, p.Y);
            var frame = new Rectangle(tile.TileFrameX / 18 * MaskFullWidth, tile.TileFrameY / 18 * MaskFrameWidth, MaskFrameWidth, 50);
            var drawPosition = this.GetDrawPosition(p.X, p.Y, GetObjectData(p.X, p.Y));
            var origin = frame.Size() / 2f;
            float globalTime = Main.GlobalTimeWrappedHourly;
            Main.GlobalTimeWrappedHourly = OmniGem.GetGlobalTime(p.X, p.Y);

            effect.Apply(null, null);

            sb.Draw(
                texture,
                drawPosition + glowOffset,
                frame.Frame(2, 0),
                Color.White with { A = 0 } * Helper.Wave(Main.GlobalTimeWrappedHourly * (0.5f + Utils.RandomFloat(ref seed) * 1f) * 2.5f, 0.6f, 1f),
                0f,
                origin,
                1f,
                SpriteEffects.None,
                0f
            );

            Main.GlobalTimeWrappedHourly = globalTime;
        }

        sb.End();
        sb.Begin_World(shader: true);

        effect = GameShaders.Armor.GetShaderFromItemId(ItemID.StardustDye);
        texture = AequusTextures.Bloom0;
        var color = Color.BlueViolet with { A = 0 } * 0.3f;
        glowOffset = new Vector2(8f, 8f);
        var textureOrigin = texture.Size() / 2f;
        foreach (Point p in drawPoints) {
            var drawPosition = this.GetDrawPosition(p.X, p.Y, GetObjectData(p.X, p.Y));
            float globalTime = Main.GlobalTimeWrappedHourly;
            Main.GlobalTimeWrappedHourly = OmniGem.GetGlobalTime(p.X, p.Y) * 0.2f;

            DrawData drawData = new(
                texture,
                drawPosition + glowOffset,
                null,
                color,
                Main.GlobalTimeWrappedHourly * 0.1f,
                textureOrigin,
                1f,
                SpriteEffects.None,
                0f);
            effect.Apply(null, drawData);

            drawData.Draw(sb);

            Main.GlobalTimeWrappedHourly = globalTime;
        }

        sb.End();
        sb.BeginCached(SpriteSortMode.Deferred, Main.Transform);
    }

    int ITileDrawSystem.Type => Type;

    void IDrawSystem.Activate() {
        DrawLayers.Instance.WorldBehindTiles += DrawOmniGemGlowBehindTiles;
        DrawLayers.Instance.PostDrawLiquids += DrawOmniGemGlowAboveWaters;
    }

    void IDrawSystem.Deactivate() {
        DrawLayers.Instance.WorldBehindTiles -= DrawOmniGemGlowBehindTiles;
        DrawLayers.Instance.PostDrawLiquids -= DrawOmniGemGlowAboveWaters;
    }
}
