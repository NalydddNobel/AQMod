using Aequus.Common.Hooks;
using Aequus.Common.Tiles;
using Aequus.Content.Vanity.Dyes;
using Aequus.Core.Graphics.Tiles;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.Localization;

namespace Aequus.Old.Content.Materials.OmniGem;

public class OmniGemTile : BaseGemTile, IBatchedTile {
    public const int GROW_RANGE_HORIZONTAL = 30;
    public const int GROW_RANGE_UP = 60;
    public const int GROW_RANGE_DOWN = 0;

    public const int MASK_FRAME_COUNT = 3;
    public const int MASK_FULL_WIDTH = 150;
    public const int MASK_FRAME_WIDTH = MASK_FULL_WIDTH / MASK_FRAME_COUNT;

    public const int MAP_ENTRY_COUNT = 16;

    public bool SolidLayerTile => false;

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        Main.tileLighted[Type] = true;
        Main.tileObsidianKill[Type] = true;
        Main.tileNoFail[Type] = true;

        TerrariaHooks.OnRandomTileUpdate += GrowOmniGems;

        LocalizedText mapEntry = LanguageDatabase.GetItemName(ModContent.ItemType<OmniGem>());
        for (int i = 0; i < MAP_ENTRY_COUNT; i++) {
            AddMapEntry(Main.hslToRgb(new Vector3(i / (float)MAP_ENTRY_COUNT, 1f, 0.66f)), mapEntry);
        }
        DustType = DustID.RainbowRod;
    }

    public override ushort GetMapOption(int i, int j) {
        var seed = Helper.TileSeed(i, j);
        return (ushort)Utils.RandomInt(ref seed, 0, MAP_ENTRY_COUNT);
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = fail ? 1 : 7;
    }

    public override bool CreateDust(int i, int j, ref int type) {
        if (Main.netMode != NetmodeID.Server) {
            var particle = ModContent.GetInstance<OmniGemParticle>().New();
            particle.Location = new Vector2(i * 16f + Main.rand.Next(16), j * 16f + Main.rand.Next(16));
            particle.Velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.2f, 1f) * 3.3f;
            particle.Color = Color.White with { A = 0 };
            particle.BloomColor = (Color.Red.HueSet(Main.rand.NextFloat(1f)) * 0.6f) with { A = 33 };
            particle.Scale = Main.rand.NextFloat(0.1f, 0.55f);
            particle.BloomScale = 0.225f;
        }
        return false;
    }

    public override bool KillSound(int i, int j, bool fail) {
        if (fail) {
            return true;
        }

        SoundEngine.PlaySound(AequusSounds.OmniGemBreak with { Volume = 1.66f, Pitch = 0.55f, PitchVariance = 0.1f, MaxInstances = 10, }, new Vector2(i * 16f + 8f, j * 16f + 8f));
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
        Main.spriteBatch.BeginWorld(shader: true);

        var effect = GameShaders.Armor.GetShaderFromItemId(DyeLoader.HueshiftDye.Type);

        var maskTexture = AequusTextures.OmniGemTile_Mask.Value;
        var glowOffset = new Vector2(-1f, -1f);
        for (var i = 0; i < count; i++) {
            var info = tiles[i];
            if (info.Tile.IsTileInvisible) {
                continue;
            }

            var frame = new Rectangle(info.Tile.TileFrameX / 18 * MASK_FULL_WIDTH, info.Tile.TileFrameY / 18 * MASK_FRAME_WIDTH, MASK_FRAME_WIDTH, 50);
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
        Main.spriteBatch.BeginWorld(shader: false);
    }

    public void BatchedPostDraw(List<BatchedTileDrawInfo> tiles, int count) {
        Main.spriteBatch.BeginWorld(shader: true);

        var effect = GameShaders.Armor.GetShaderFromItemId(DyeLoader.HueshiftDye.Type);

        var texture = AequusTextures.OmniGemTile_Mask.Value;
        var glowOffset = new Vector2(7f, 7f);
        for (var i = 0; i < count; i++) {
            var info = tiles[i];
            if (info.Tile.IsTileInvisible) {
                continue;
            }

            ulong seed = Helper.TileSeed(tiles[i].Position);
            var frame = new Rectangle(info.Tile.TileFrameX / 18 * MASK_FULL_WIDTH, info.Tile.TileFrameY / 18 * MASK_FRAME_WIDTH, MASK_FRAME_WIDTH, 50);
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
        Main.spriteBatch.BeginWorld(shader: true);

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

    public static void GrowOmniGems(int i, int j) {
        Tile tile = Main.tile[i, j];
        // Abort if the block being updated does not have Shimmer.
        if (tile.LiquidAmount == 0 || tile.LiquidType != LiquidID.Shimmer) {
            return;
        }

        int gemX = i + WorldGen.genRand.Next(-GROW_RANGE_HORIZONTAL, GROW_RANGE_HORIZONTAL);
        int gemY = j - WorldGen.genRand.Next(GROW_RANGE_DOWN, GROW_RANGE_UP);

        // Abort if the random spot is outside of the world, or has a Tile already.
        if (!WorldGen.InWorld(gemX, gemY, 5) || Main.tile[gemX, gemY].HasTile || !TileHelper.GetGemFramingAnchor(gemX, gemY).IsSolidTileAnchor()) {
            return;
        }

        int omniGemTileID = ModContent.TileType<OmniGemTile>();

        // Abort if the random spot has an omni gem, shimmer, or a tree/gem tree within a 5x3 rectangle
        if (TileHelper.ScanTiles(new Rectangle(gemX - 2, gemY - 1, 5, 3), TileHelper.HasTileAction(omniGemTileID), TileHelper.HasShimmer, TileHelper.IsTree)) {
            return;
        }

        // Place the Gem
        WorldGen.PlaceTile(gemX, gemY, omniGemTileID, mute: true);

        // Sync the gem if it was successfully placed.
        if (tile.HasTile && tile.TileType == omniGemTileID) {
            if (Main.netMode != NetmodeID.SinglePlayer) {
                NetMessage.SendTileSquare(-1, gemX, gemY);
            }
        }
    }
}
