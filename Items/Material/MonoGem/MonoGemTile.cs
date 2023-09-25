using Aequus.Common.Graphics.Rendering.Tiles;
using Aequus.Common.Tiles.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Material.MonoGem;

public class MonoGemTile : BaseGemTile, ISpecialTileRenderer {
    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        Main.tileLighted[Type] = true;

        AddMapEntry(new Color(66, 55, 55), Lang.GetItemName(ModContent.ItemType<MonoGem>()));
        DustType = DustID.Ambient_DarkBrown;
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        r = 0.2f;
        g = 0.2f;
        b = 0.2f;
    }

    public void GetRandomValues(int i, int j, out ulong seed, out float globalIntensity) {
        seed = Helper.TileSeed(i, j);
        globalIntensity = Helper.Oscillate(Main.GlobalTimeWrappedHourly * 2.5f + Utils.RandomFloat(ref seed) * 20f, 0.7f, 1f);
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        GetRandomValues(i, j, out ulong seed, out float globalIntensity);
        var drawPos = new Vector2(i * 16f, j * 16f) + DrawHelper.TileDrawOffset - Main.screenPosition;

        if (!Main.tile[i, j].IsTileInvisible) {
            Main.spriteBatch.Draw(
                AequusTextures.Bloom,
                drawPos + new Vector2(8f),
                null,
                Color.Black * globalIntensity * 0.75f,
                0f,
                AequusTextures.Bloom.Size() / 2f,
                0.45f * globalIntensity + 0.33f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(
                TextureAssets.Tile[Type].Value,
                drawPos,
                new Rectangle(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY, 16, 16),
                Color.White);
        }

        SpecialTileRenderer.Add(i, j, TileRenderLayerID.PostDrawMasterRelics);
        return false;
    }

    void ISpecialTileRenderer.Render(int i, int j, byte layer) {
        GetRandomValues(i, j, out ulong seed, out float globalIntensity);

        var fogTexture = AequusTextures.BloomStrong;
        var drawPos = new Vector2(i * 16f, j * 16f) + new Vector2(8f);

        MonoGemRenderer.Instance.DrawData.Add(
            new DrawData(
                AequusTextures.BloomStrong,
                drawPos,
                null,
                Color.White * 0.1f * globalIntensity,
                0f,
                AequusTextures.BloomStrong.Size() / 2f,
                2f * globalIntensity, SpriteEffects.FlipHorizontally, 0
            ));

        for (int k = 0; k < 2; k++) {
            float intensity = MathF.Sin((k * MathHelper.Pi / 3f + Main.GameUpdateCount / 60f) % MathHelper.Pi);
            //var frame = fogTexture.Frame(verticalFrames: 8, frameY: Utils.RandomInt(ref seed, 8));
            var frame = fogTexture.Frame();
            MonoGemRenderer.Instance.DrawData.Add(
                new DrawData(
                    fogTexture,
                    drawPos,
                    frame,
                    Color.White * intensity * globalIntensity,
                    Main.GlobalTimeWrappedHourly * 0.1f,
                    frame.Size() / 2f,
                    2f * globalIntensity, SpriteEffects.FlipHorizontally, 0));
        }
    }
}