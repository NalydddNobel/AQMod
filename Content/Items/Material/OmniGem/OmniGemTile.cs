using Aequus;
using Aequus.Common.Graphics.Rendering.Tiles;
using Aequus.Common.Particles;
using Aequus.Common.Tiles.Base;
using Aequus.Content.Items.Misc.Dyes.Hueshift;
using Aequus.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Aequus.Content.Items.Material.OmniGem;

public class OmniGemTile : BaseGemTile, IBatchedTile {
    public const int MaskFrameWidth = MaskFullWidth / 3;
    public const int MaskFullWidth = 150;

    public bool SolidLayerTile => false;

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
                    (Color.Red.HueSet(Main.rand.NextFloat(1f)) * 0.6f) with { A = 33 },
                    Main.rand.NextFloat(0.1f, 0.55f),
                    0.225f,
                    Main.rand.NextFloat(MathHelper.TwoPi)
                );
        }
        return false;
    }

    public override bool KillSound(int i, int j, bool fail) {
        if (fail) {
            return true;
        }

        SoundEngine.PlaySound(AequusSounds.OmniGemShatter.Sound with { Volume = 2f, Pitch = 0.55f, PitchVariance = 0.1f, MaxInstances = 10, }, new Vector2(i * 16f + 8f, j * 16f + 8f));
        return false;
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        r = 0.3f;
        g = 0.1f;
        b = 0.5f;
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        BatchedTileRenderer.Add(i, j, Type);
        return false;
    }

    public void BatchedPreDraw(List<BatchedTileDrawInfo> tiles, int count) {
        Main.spriteBatch.End();
        Main.spriteBatch.Begin_World(shader: true);

        var effect = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<HueshiftDye>());

        var maskTexture = AequusTextures.OmniGemTile_Mask.Value;
        var glowOffset = new Vector2(-1f, -1f);
        for (var i = 0; i < count; i++) {
            var info = tiles[i];
            if (info.Tile.IsTileInvisible) {
                continue;
            }

            var frame = new Rectangle(info.Tile.TileFrameX / 18 * MaskFullWidth, info.Tile.TileFrameY / 18 * MaskFrameWidth, MaskFrameWidth, 50);
            var drawPosition = new Vector2(tiles[i].Position.X * 16f, tiles[i].Position.Y * 16f) - Main.screenPosition;
            var origin = frame.Size() / 2f;
            float globalTime = Main.GlobalTimeWrappedHourly;
            Main.GlobalTimeWrappedHourly = OmniGem.GetGlobalTime(tiles[i].Position.X, tiles[i].Position.Y);

            effect.Apply(null, null);

            Main.spriteBatch.Draw(
                TextureAssets.Tile[Type].Value,
                drawPosition,
                new Rectangle(info.Tile.TileFrameX, info.Tile.TileFrameY, 16, 16),
                Color.White,
                0f,
                Vector2.Zero,
                1f, SpriteEffects.None, 0f
            );

            Main.GlobalTimeWrappedHourly = globalTime;
        }

        Main.spriteBatch.End();
        Main.spriteBatch.Begin_World(shader: false);
    }

    public void BatchedPostDraw(List<BatchedTileDrawInfo> tiles, int count) {
        Main.spriteBatch.Begin_World(shader: true);

        var effect = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<HueshiftDye>());

        var texture = AequusTextures.OmniGemTile_Mask.Value;
        var glowOffset = new Vector2(7f, 7f);
        for (var i = 0; i < count; i++) {
            var info = tiles[i];
            if (info.Tile.IsTileInvisible) {
                continue;
            }

            ulong seed = Helper.TileSeed(tiles[i].Position);
            var frame = new Rectangle(info.Tile.TileFrameX / 18 * MaskFullWidth, info.Tile.TileFrameY / 18 * MaskFrameWidth, MaskFrameWidth, 50);
            var drawPosition = new Vector2(tiles[i].Position.X * 16f, tiles[i].Position.Y * 16f) - Main.screenPosition;
            var origin = frame.Size() / 2f;
            float globalTime = Main.GlobalTimeWrappedHourly;
            Main.GlobalTimeWrappedHourly = OmniGem.GetGlobalTime(tiles[i].Position.X, tiles[i].Position.Y);

            effect.Apply(null, null);

            Main.spriteBatch.Draw(
                texture,
                drawPosition + glowOffset,
                frame.Frame(2, 0),
                Color.White with { A = 0 } * Helper.Oscillate(Main.GlobalTimeWrappedHourly * (0.5f + Utils.RandomFloat(ref seed) * 1f) * 2.5f, 0.6f, 1f),
                0f,
                origin,
                1f,
                SpriteEffects.None,
                0f
            );

            Main.GlobalTimeWrappedHourly = globalTime;
        }

        Main.spriteBatch.End();
        Main.spriteBatch.Begin_World(shader: true);

        effect = GameShaders.Armor.GetShaderFromItemId(ItemID.StardustDye);
        texture = AequusTextures.BloomStrong;
        var color = Color.BlueViolet with { A = 0 } * 0.3f;
        glowOffset = new Vector2(8f, 8f);
        var textureOrigin = texture.Size() / 2f;
        for (var i = 0; i < count; i++) {
            var info = tiles[i];
            var drawPosition = new Vector2(tiles[i].Position.X * 16f, tiles[i].Position.Y * 16f) - Main.screenPosition;
            float globalTime = Main.GlobalTimeWrappedHourly;
            Main.GlobalTimeWrappedHourly = OmniGem.GetGlobalTime(tiles[i].Position.X, tiles[i].Position.Y) * 0.2f;

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

            drawData.Draw(Main.spriteBatch);

            Main.GlobalTimeWrappedHourly = globalTime;

            //ChatManager.DrawColorCodedString(Main.spriteBatch, FontAssets.MouseText.Value, $"{Main.tile[info.Position].TileFrameX} : {Main.tile[info.Position].TileFrameY}", drawPosition, Color.Orange, 0f, Vector2.Zero, Vector2.One);
        }
        Main.spriteBatch.End();
    }

    private static bool CheckShimmer(int i, int j, int rangeX, int rangeY, int iterations) {
        for (int k = 0; k < iterations; k++) {
            int randX = i + WorldGen.genRand.Next(-rangeX, rangeX);
            int randY = j + WorldGen.genRand.Next(-rangeY, rangeY);
            if (WorldGen.InWorld(randX, randY) && TileHelper.HasShimmer(randX, randY)) {
                return true;
            }
        }
        return false;
    }

    public static bool TryGrow(int i, int j) {
        const int RANGE_X = 60;
        const int RANGE_Y = 60;
        const int ITERATIONS = 30;
        if (!WorldGen.InWorld(i, j, Math.Max(RANGE_X, RANGE_Y) + 10)) {
            return false;
        }

        int omniGemTileID = ModContent.TileType<OmniGemTile>();
        int generateX = i + WorldGen.genRand.Next(-1, 2);
        int generateY = j + WorldGen.genRand.Next(2);
        var tile = Main.tile[generateX, generateY];
        if ((tile.HasTile && !tile.CuttableType()) || tile.SolidType() || TileID.Sets.IsVine[tile.TileType]
            || TileHelper.ScanTiles(new(i - 2, j - 1, 5, 3), TileHelper.HasTileAction(omniGemTileID), TileHelper.HasShimmer, TileHelper.IsTree)
            || !CheckShimmer(i, j + RANGE_Y / 2, RANGE_X, RANGE_Y, ITERATIONS)) {
            return false;
        }

        if (tile.CuttableType()) {
            tile.HasTile = false;
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
}
