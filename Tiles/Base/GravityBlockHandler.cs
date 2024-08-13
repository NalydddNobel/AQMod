using Aequus.Common.Assets;
using Aequus.Common.Drawing;
using Aequus.Common.Rendering;
using Aequus.Common.Tiles;
using System;
using Terraria.Utilities;

namespace Aequus.Tiles.Base;
internal interface IGravityBlock {
    public sbyte GravityType { get; set; }
    public sbyte GetReach(int i, int j) {
        return GetReachDefault(i, j, GravityBlockHandler.MaximumReach, GravityType);
    }

    protected static sbyte GetReachDefault(int i, int j, sbyte maxReach, sbyte gravityType) {
        var gravity = (sbyte)Math.Sign(-gravityType);
        for (sbyte l = 1; l < maxReach; l++) {
            if (Main.tile[i, j + l * gravity].IsSolid()) {
                return l;
            }
        }
        return maxReach;
    }
}

public abstract class GravityBlockBase : ModTile, ITileDrawSystem, IGravityBlock {
    public sbyte GravityType { get; set; }
    public RequestCache<Texture2D>[] Auras { get; set; }
    public RequestCache<Texture2D> DustTexture { get; set; }

    int ITileDrawSystem.Type => Type;

    public override void SetStaticDefaults() {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        HitSound = SoundID.Tink;
    }

    public override bool Slope(int i, int j) {
        return false;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = fail ? 1 : 3;
    }

    public virtual sbyte GetReach(int i, int j) {
        return IGravityBlock.GetReachDefault(i, j, GravityBlockHandler.MaximumReach, GravityType);
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        if (Main.tile[i, j].IsActuated || Main.tile[i, j].IsInvisible())
            return true;

        var drawCoords = new Vector2(i * 16f + 8f, j * 16f + 8f) - Main.screenPosition + Helper.TileDrawOffset;
        int tileHeight = GetReach(i, j) - 1;
        if (tileHeight == 0)
            return true;

        int gravity = Math.Sign(GravityType);
        float rotation = gravity == 1 ? 0f : MathHelper.Pi;
        for (int k = 0; k < Auras.Length; k++) {
            var texture = PaintsRenderer.TryGetPaintedTexture(i, j, Auras[k].FullPath);
            Vector2 scale = new(1f, (tileHeight * 16f + 32f) / texture.Height);
            spriteBatch.Draw(
                texture,
                drawCoords,
                null,
                Color.White.UseA(0) * 0.5f,
                rotation,
                new(texture.Width / 2f, texture.Height),
                scale,
                SpriteEffects.None, 0f
            );
        }
        return true;
    }

    void DrawParticles(int i, int j, int tileHeight, SpriteBatch spriteBatch) {
        ulong seed = Helper.TileSeed(i, j);
        FastRandom rand = new(Helper.TileSeed(i, j));
        var texture = PaintsRenderer.TryGetPaintedTexture(i, j, DustTexture.FullPath);
        var origin = new Vector2(4f, 4f);
        var drawCoords = new Vector2(i * 16f, j * 16f + 8f) - Main.screenPosition;
        int dustAmt = (int)(rand.Next(tileHeight) / 1.5f + 2f);
        int gravity = Math.Sign(GravityType);
        for (int k = 0; k < dustAmt * 3; k++) {
            float p = rand.Next(50) + Main.GlobalTimeWrappedHourly * rand.NextFloat(2f, 5.2f);
            p %= 50f;
            p /= 50f;
            p = (float)Math.Pow(p, 3f);
            p *= 50f;
            p -= 2f;
            var frame = new Rectangle(0, 10 * rand.Next(3), 8, 8);
            var dustDrawOffset = new Vector2(Helper.Wave(Main.GlobalTimeWrappedHourly * rand.NextFloat(0.45f, 1f), 0f, 16f), tileHeight * 16f - p * 16f);
            float opacity = rand.NextFloat(0.1f, 1f);
            float scale = rand.NextFloat(0.25f, 0.7f);
            if (p > 0f && p < tileHeight) {
                if (p < 6f) {
                    float progress = p / 6f;
                    opacity *= progress;
                    scale *= progress;
                }
                spriteBatch.Draw(texture, drawCoords + dustDrawOffset with { Y = dustDrawOffset.Y * -gravity }, frame,
                    Color.White.UseA(0) * opacity, 0f, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }

    void DrawAllParticles(SpriteBatch sb) {
        foreach (Point p in this.GetDrawPoints()) {
            DrawParticles(p.X, p.Y, GetReach(p.X, p.Y), Main.spriteBatch);
        }
    }

    void IDrawSystem.Activate() {
        DrawLayers.Instance.WorldBehindTiles += DrawAllParticles;
    }

    void IDrawSystem.Deactivate() {
        DrawLayers.Instance.WorldBehindTiles -= DrawAllParticles;
    }
}