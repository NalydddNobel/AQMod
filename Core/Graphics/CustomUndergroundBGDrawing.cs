using Aequus.Core.Assets;
using System;
using Terraria.GameContent;

namespace Aequus.Core.Graphics;

public class CustomUndergroundBGDrawing : ModSystem {
    private static RenderTarget2D _backgroundTarget;

    private static void Main_OnPreDraw(GameTime obj) {
        if (Main.BackgroundEnabled) {
            LightMap.Needed = true;
            SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice g = Main.instance.GraphicsDevice;
            if (DrawHelper.BadRenderTarget(_backgroundTarget, Main.screenWidth, Main.screenHeight)) {
                try {
                    if (Main.IsGraphicsDeviceAvailable) {
                        _backgroundTarget = new RenderTarget2D(g, Main.screenWidth, Main.screenHeight, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                    }
                }
                catch {
                }
                return;
            }

            RenderTargetBinding[] oldTargets = g.GetRenderTargets();

            try {
                g.SetRenderTarget(_backgroundTarget);
                g.Clear(Color.Transparent);

                sb.Begin();

                PollutedOceanBGTest();

                sb.End();
            }
            catch {
            }
            finally {
            }

            g.SetRenderTargets(oldTargets);
        }
    }

    private static void On_Main_DoDraw_WallsTilesNPCs(On_Main.orig_DoDraw_WallsTilesNPCs orig, Main main) {
        if (Main.BackgroundEnabled && Main.shimmerAlpha < 1f && !DrawHelper.BadRenderTarget(_backgroundTarget)) {
            float opacity = 1f - Main.shimmerAlpha;
            int topBounds = (int)(Main.worldSurface * 16f + 16f - Main.screenPosition.Y);
            int bottomBounds = (int)(Main.UnderworldLayer * 16f - Main.screenPosition.Y);
            Rectangle cropping = new Rectangle(0, Math.Max(topBounds, 0), _backgroundTarget.Width, Math.Min(_backgroundTarget.Height, bottomBounds));
            
            if (LightMap.TargetAvailable) {
                SpriteBatchCache cache = new SpriteBatchCache(Main.spriteBatch);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                Effect lightEffect = AequusShaders.Multiply.Value;
                Main.graphics.GraphicsDevice.Textures[1] = LightMap.MapTarget;
                lightEffect.CurrentTechnique.Passes[0].Apply();

                Main.spriteBatch.Draw(_backgroundTarget, new Vector2(0f, cropping.Y), cropping, Color.White * opacity);

                Main.spriteBatch.End();
                cache.Begin(Main.spriteBatch);
            }

            int offScreenRange = Main.drawToScreen ? 0 : Main.offScreenRange;
            if (Main.screenPosition.Y + Main.screenHeight > Main.worldSurface * 16 && Main.screenPosition.Y <= Main.worldSurface * 16 + 300f) {
                main.LoadBackground(290);
                main.LoadBackground(291);
                Texture2D transitionTexture = TextureAssets.Background[290].Value;
                Texture2D fillTexture = TextureAssets.Background[291].Value;
                DrawTransitionBGStripTest((int)Main.worldSurface, transitionTexture, fillTexture, opacity);
            }
            if (Main.screenPosition.Y + Main.screenHeight >= Main.UnderworldLayer * 16f - 400f) {
                main.LoadBackground(292);
                main.LoadBackground(293);
                Texture2D transitionTexture = TextureAssets.Background[292].Value;
                Texture2D fillTexture = TextureAssets.Background[293].Value;

                DrawTransitionBGStripTest(Main.UnderworldLayer - 12, transitionTexture, fillTexture, opacity);
            }
        }
        orig(main);
    }

    private static void PollutedOceanBGTest() {
        Vector2 offScreenRangeVector2 = Vector2.Zero;
        Point offScreenRange = offScreenRangeVector2.ToPoint();
        if (Main.screenPosition.Y + Main.screenHeight + offScreenRange.Y < Main.worldSurface * 16) {
            return;
        }

        int bgTopY = (int)(Main.worldSurface * 16f - 16f - Main.screenPosition.Y + 16f);
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(-offScreenRange.X, Math.Max(-offScreenRange.Y, bgTopY + 300), Main.screenWidth + offScreenRange.X * 4, Main.screenHeight + offScreenRange.Y * 4), Color.Black);

        if (!TextureAssets.Underworld[2].IsLoaded) {
            Main.Assets.Request<Texture2D>(TextureAssets.Underworld[2].Name);
        }
        if (!TextureAssets.Underworld[9].IsLoaded) {
            Main.Assets.Request<Texture2D>(TextureAssets.Underworld[9].Name);
        }
        DrawBackgroundLayerTest(TextureAssets.Underworld[9].Value, 0.00003f, Color.Cyan);
        DrawBackgroundLayerTest(TextureAssets.Underworld[2].Value, 0.0001f, Color.Cyan);
        //DrawBackgroundLayerTest(TextureAssets.MagicPixel.Value, 0.00003f, Color.Cyan);
        //DrawBackgroundLayerTest(TextureAssets.MagicPixel.Value, 0.0001f, Color.Cyan);
    }

    private static void DrawBackgroundLayerTest(Texture2D bgTexture, float horizontalScrollSpeed, Color color) {
        float upScale = 1.5f;
        Vector2 bgScale = new Vector2(1f, Math.Max(Main.screenHeight / (float)bgTexture.Height, 1f)) * upScale;
        float bgX = (1f - Main.screenPosition.X * horizontalScrollSpeed % 1f) * (bgTexture.Width * bgScale.X);
        float bgY = Main.screenPosition.Y / 16f / Main.maxTilesY;

        int bgXLoops = (int)(Main.screenWidth / (bgTexture.Width * bgScale.X)) + 1;
        float bgDrawY = (Main.screenHeight - bgTexture.Height * bgScale.Y) * bgY;
        for (int i = -1; i < bgXLoops; i++) {
            Main.spriteBatch.Draw(bgTexture, new Vector2(bgX + bgTexture.Width * bgScale.X * i, bgDrawY), null, color, 0f, Vector2.Zero, bgScale, SpriteEffects.None, 0f);
        }
    }

    private static void DrawTransitionBGStripTest(int Y, Texture2D transitionTexture, Texture2D fillTexture, float opacity) {
        //Vector2 offScreenRange = TileHelper.DrawOffset;
        Vector2 offScreenRange = Vector2.Zero;
        int offScreenRangeX = (int)offScreenRange.X;
        Y = (int)(Y * 16f - 16f - Main.screenPosition.Y + 16f);
        int transitionTextureWidth = transitionTexture.Width - 32;
        int bgLoops = (Main.screenWidth + offScreenRangeX * 2) / transitionTextureWidth + 2;
        int bgStartX = (int)(-Math.IEEERemainder(transitionTextureWidth + (double)Main.screenPosition.X * Main.caveParallax, transitionTextureWidth) - transitionTextureWidth / 2) - offScreenRangeX;
        int fillStrips = fillTexture.Height / 16;
        for (int l = 0; l < bgLoops; l++) {
            for (int m = 0; m < transitionTextureWidth / 16; m++) {
                int frameX = (int)(float)Math.Round(0f - (float)Math.IEEERemainder(bgStartX + Main.screenPosition.X, 16.0));
                if (frameX == -8) {
                    frameX = 8;
                }
                float x = bgStartX + transitionTextureWidth * l + m * 16 + 8;
                Color color = Lighting.GetColor((int)((x + Main.screenPosition.X) / 16f), (int)((Main.screenPosition.Y + Y) / 16f));
                Vector2 drawCoordinates = new Vector2(x - 8f + frameX, Y) + offScreenRange;
                Rectangle frame = new Rectangle(16 * m + frameX + 16, 0, 16, 16);
                Main.spriteBatch.Draw(transitionTexture, drawCoordinates, frame, color);
                drawCoordinates.Y += 16;
                for (int k = 0; k < 2; k++) {
                    for (int p = 0; p < fillStrips; p++) {
                        color = LightHelper.GetLightColor(drawCoordinates - offScreenRange + Main.screenPosition);
                        Main.spriteBatch.Draw(fillTexture, drawCoordinates, frame with { Y = p * 16 }, color);
                        drawCoordinates.Y += 16f;
                    }
                }
                drawCoordinates.Y -= fillTexture.Height + 16f;
                Main.spriteBatch.Draw(transitionTexture, drawCoordinates + new Vector2(0f, fillTexture.Height + 16f), frame, color, 0f, Vector2.Zero, 1f, SpriteEffects.FlipVertically, 0f);
            }
        }
    }

    public override bool IsLoadingEnabled(Mod mod) {
        return Aequus.DEBUG_MODE;
    }

    public override void Load() {
        On_Main.DoDraw_WallsTilesNPCs += On_Main_DoDraw_WallsTilesNPCs;
        Main.OnPreDraw += Main_OnPreDraw;
    }

    public override void Unload() {
        Main.OnPreDraw -= Main_OnPreDraw;
        _backgroundTarget = null;
    }
}

public class CustomDrawnUGBackground : ModUndergroundBackgroundStyle {
    private int _noTextureSlot;

    public override void Load() {
        BackgroundTextureLoader.AddBackgroundTexture(Mod, AequusTextures.None.Path);
        _noTextureSlot = BackgroundTextureLoader.GetBackgroundSlot(Mod, AequusTextures.None.ModPath);
    }

    public override void FillTextureArray(int[] textureSlots) {
        for (int i = 0; i < textureSlots.Length; i++) {
            textureSlots[i] = _noTextureSlot;
        }
    }
}