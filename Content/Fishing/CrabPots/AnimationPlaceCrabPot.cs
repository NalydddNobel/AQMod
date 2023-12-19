using Aequus.Core.Graphics.Animations;
using Microsoft.Xna.Framework;
using System;
using Terraria.Audio;
using Terraria.GameContent.Shaders;

namespace Aequus.Content.Fishing.CrabPots;

public class AnimationPlaceCrabPot : ITileAnimation {
    public float AnimationTime;
    public int DrawOffsetY;

    public bool Update(int x, int y) {
        if (AnimationTime == 0f) {
            int topY = y;
            for (; topY > 0 && Main.tile[x, topY].LiquidAmount == byte.MaxValue; topY--) {
            }

            DrawHelper.AddWaterRipple(new Vector2(x + 1f, y + 1f) * 16f, 0.33f, 0.6f, 0f, new Vector2(48f, 48f), RippleShape.Circle, 0f);

            float waterLineY = TileHelper.GetWaterY(Main.tile[x, y].LiquidAmount);

            var dustPosition = new Vector2(x * 16f, topY * 16f + waterLineY);
            int dustW = 32;
            int dustH = 16;
            int liquidType = Main.tile[x, topY].LiquidType;
            int dustType = DrawHelper.LiquidTypeToDustId(liquidType);
            switch (liquidType) {
                case LiquidID.Water:
                    for (int l = 0; l < 30; l++) {
                        var d = Dust.NewDustDirect(dustPosition, dustW, dustH, dustType);
                        d.velocity.Y -= 4f;
                        d.velocity.X *= 2.5f;
                        d.scale *= 0.8f;
                        d.alpha = 100;
                        d.noGravity = true;
                    }
                    break;

                case LiquidID.Lava:
                    for (int i = 0; i < 10; i++) {
                        var d = Dust.NewDustDirect(dustPosition, dustW, dustH, dustType);
                        d.velocity.Y -= 1.5f;
                        d.velocity.X *= 2.5f;
                        d.scale = 1.3f;
                        d.alpha = 100;
                        d.noGravity = true;
                    }
                    break;
            }
            switch (liquidType) {
                case LiquidID.Water:
                    SoundEngine.PlaySound(SoundID.Splash with { Pitch = 0.25f, MaxInstances = 5, }, new Vector2(x, y).ToWorldCoordinates());
                    break;

                default:
                    SoundEngine.PlaySound(SoundID.SplashWeak with { Pitch = 0.25f, MaxInstances = 5, }, new Vector2(x, y).ToWorldCoordinates());
                    break;
            }
        }

        AnimationTime += 0.05f;
        float tileY = -MathF.Sin(AnimationTime * MathHelper.PiOver2 * 3f + MathHelper.PiOver2);
        if (tileY > 0f) {
            if (tileY < 1f) {
                tileY = 1f - MathF.Pow(1f - tileY, 2f);
            }
            tileY *= 0.5f;
        }
        float reboundTime = 0.4f;
        if (AnimationTime > 1f) {
            tileY *= 1f - (AnimationTime - 1f) / reboundTime;
        }
        DrawOffsetY = (int)(tileY * 8f);
        return AnimationTime < 1f + reboundTime;
    }
}