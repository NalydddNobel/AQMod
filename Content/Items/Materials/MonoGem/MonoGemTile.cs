using Aequus;
using Aequus.Common.Drawing;
using Aequus.Tiles.Base;
using System;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Aequus.Content.Items.Materials.MonoGem;

public class MonoGemTile : BaseGemTile, ITileDrawSystem {
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
        globalIntensity = Helper.Wave(Main.GlobalTimeWrappedHourly * 2.5f + Utils.RandomFloat(ref seed) * 20f, 0.7f, 1f);
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        GetRandomValues(i, j, out ulong seed, out float globalIntensity);
        var drawPos = this.GetDrawPosition(i, j, GetObjectData(i, j)) + Helper.TileDrawOffset;

        if (!Main.tile[i, j].IsTileInvisible) {
            Main.spriteBatch.Draw(
                AequusTextures.Bloom0,
                drawPos + new Vector2(8f),
                null,
                Color.Black * globalIntensity * 0.75f,
                0f,
                AequusTextures.Bloom0.Size() / 2f,
                0.45f * globalIntensity + 0.33f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(
                TextureAssets.Tile[Type].Value,
                drawPos,
                new Rectangle(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY, 16, 16),
                Color.White);
        }

        return false;
    }

    private void DrawFog(SpriteBatch sb) {
        foreach (Point p in this.GetDrawPoints()) {
            GetRandomValues(p.X, p.Y, out ulong seed, out float globalIntensity);

            var fogTexture = AequusTextures.FogParticleHQ;
            var drawPos = this.GetDrawPosition(p.X, p.Y, GetObjectData(p.X, p.Y)) + Main.screenLastPosition + new Vector2(8f);

            MonoGemRenderer.Instance.DrawData.Add(
                new DrawData(
                    AequusTextures.Bloom3,
                    drawPos,
                    null,
                    Color.White * 0.1f * globalIntensity,
                    0f,
                    AequusTextures.Bloom3.Size() / 2f,
                    1f * globalIntensity, SpriteEffects.FlipHorizontally, 0
                ));

            for (int k = 0; k < 3; k++) {
                float intensity = MathF.Sin((k * MathHelper.Pi / 3f + Main.GameUpdateCount / 60f) % MathHelper.Pi);
                var frame = fogTexture.Frame(verticalFrames: 8, frameY: Utils.RandomInt(ref seed, 8));
                MonoGemRenderer.Instance.DrawData.Add(
                    new DrawData(
                        fogTexture,
                        drawPos,
                        frame,
                        Color.White * intensity * 0.75f * globalIntensity,
                        Main.GlobalTimeWrappedHourly * 0.1f,
                        frame.Size() / 2f,
                        3f * globalIntensity, SpriteEffects.FlipHorizontally, 0));
            }
        }
    }

    int ITileDrawSystem.Type => Type;

    void IDrawSystem.Activate() {
        DrawLayers.Instance.PostUpdateScreenPosition += DrawFog;
    }

    void IDrawSystem.Deactivate() {
        DrawLayers.Instance.PostUpdateScreenPosition -= DrawFog;
    }
}
