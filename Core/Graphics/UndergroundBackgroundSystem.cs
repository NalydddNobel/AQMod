using Aequus.Core.Assets;
using System;
using System.Collections.Generic;
using Terraria.GameInput;

namespace Aequus.Core.Graphics;

public class UndergroundBackgroundSystem : ModSystem {
    internal static Dictionary<int, CustomDrawnUGBackground> _customDrawBGLookup = new();

    public static int TransitionStrips = 2;

    public static int EmptyBackgroundSlot { get; private set; }

    #region BG Drawing
    private static RenderTarget2D _backgroundTarget;
    private static RenderTarget2D _oldBackgroundTarget;
    private static SpriteBatchCache _sbCache = new SpriteBatchCache();
    private static bool _preparedBackground;
    private static bool _preparedOldBackground;

    private static void Main_OnPreDraw(GameTime obj) {
        if (Main.BackgroundEnabled) {
            LightMap.Needed = true;
            SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice g = Main.instance.GraphicsDevice;

            if (_customDrawBGLookup.TryGetValue(Main.undergroundBackground, out CustomDrawnUGBackground ugBg)) {
                _preparedBackground = DrawBackgroundTarget(ugBg, ref _backgroundTarget, sb, g);
            }
            else {
                _preparedBackground = false;
            }

            if (_customDrawBGLookup.TryGetValue(Main.oldUndergroundBackground, out CustomDrawnUGBackground oldUgBg)) {
                _preparedOldBackground = DrawBackgroundTarget(oldUgBg, ref _oldBackgroundTarget, sb, g);
            }
            else {
                _preparedOldBackground = false;
            }
        }
    }

    private static bool DrawBackgroundTarget(CustomDrawnUGBackground undergroundBackground, ref RenderTarget2D target, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice) {
        if (DrawHelper.BadRenderTarget(target, Main.screenWidth, Main.screenHeight)) {
            try {
                if (Main.IsGraphicsDeviceAvailable) {
                    target = new RenderTarget2D(graphicsDevice, Main.screenWidth, Main.screenHeight, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                }
            }
            catch {
            }
            return false;
        }

        DrawHelper.graphics.EnqueueRenderTargetBindings(graphicsDevice);

        bool rendered = true;

        try {
            graphicsDevice.SetRenderTarget(target);
            graphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin();

            undergroundBackground.RenderBackground(spriteBatch, graphicsDevice);

            spriteBatch.End();
        }
        catch {
            rendered = false;
        }
        finally {
        }

        DrawHelper.graphics.DequeueRenderTargetBindings(graphicsDevice);

        return rendered;
    }

    private static void On_Main_DoDraw_WallsTilesNPCs(On_Main.orig_DoDraw_WallsTilesNPCs orig, Main main) {
        if (Main.BackgroundEnabled && Main.shimmerAlpha < 1f && !DrawHelper.BadRenderTarget(_backgroundTarget) && (_preparedBackground || _preparedOldBackground)) {
            float totalOpacity = 1f - Main.shimmerAlpha;
            float transitionIn = (1f - Main.ugBackTransition) * totalOpacity;
            float transitionOut = Main.ugBackTransition * totalOpacity;

            int surfaceLayer = (int)Main.worldSurface;
            int underworldLayer = Main.UnderworldLayer - 115;

            // Draw Background targets
            if (LightMap.TargetAvailable) {
                float zoom = Main.GameViewMatrix.Zoom.X;
                int topBounds = (int)DrawHelper.ApplyZoomY(surfaceLayer * 16f + 16f + 96f * TransitionStrips - Main.screenPosition.Y, zoom);
                int bottomBounds = (int)DrawHelper.ApplyZoomY(underworldLayer * 16f - Main.screenPosition.Y, zoom);

                Rectangle cropping = new Rectangle(0, Math.Max(topBounds, 0), _backgroundTarget.Width, Math.Min(_backgroundTarget.Height, bottomBounds));

                _sbCache.InheritFrom(Main.spriteBatch);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                // Apply light shader to Background
                Effect lightEffect = AequusShaders.Multiply.Value;
                Main.graphics.GraphicsDevice.Textures[1] = LightMap.MapTarget;
                lightEffect.CurrentTechnique.Passes[0].Apply();

                if (_preparedBackground && transitionIn != 0f) {
                    Main.spriteBatch.Draw(_backgroundTarget, new Vector2(0f, cropping.Y), cropping, Color.White * transitionIn);
                }
                // Draw Old Background (Transition Bg)
                if (_preparedOldBackground && transitionOut != 0f && _preparedOldBackground && !DrawHelper.BadRenderTarget(_oldBackgroundTarget)) {
                    Main.spriteBatch.Draw(_oldBackgroundTarget, new Vector2(0f, cropping.Y), cropping, Color.White * transitionOut);
                }

                Main.spriteBatch.End();
                _sbCache.Begin(Main.spriteBatch, SpriteSortMode.Deferred, transform: Main.Transform);
            }

            // Draw surface transition
            if (Main.screenPosition.Y + Main.screenHeight > surfaceLayer * 16f && Main.screenPosition.Y <= surfaceLayer * 16f + 300f) {
                if (transitionIn != 0f && _customDrawBGLookup.TryGetValue(Main.undergroundBackground, out CustomDrawnUGBackground ugBg)) {
                    ugBg.RenderTopTransitionStrip(Main.spriteBatch, surfaceLayer, totalOpacity * transitionIn);
                }
                if (transitionOut != 0f && _customDrawBGLookup.TryGetValue(Main.oldUndergroundBackground, out CustomDrawnUGBackground oldUgBg)) {
                    oldUgBg.RenderTopTransitionStrip(Main.spriteBatch, surfaceLayer, transitionOut);
                }
            }

            // Draw underworld transition
            if (Main.screenPosition.Y + Main.screenHeight >= underworldLayer - 160f) {
                if (transitionIn != 0f && _customDrawBGLookup.TryGetValue(Main.undergroundBackground, out CustomDrawnUGBackground ugBg)) {
                    ugBg.RenderBottomTransitionStrip(Main.spriteBatch, underworldLayer, transitionIn);
                }
                if (transitionOut != 0f && _customDrawBGLookup.TryGetValue(Main.oldUndergroundBackground, out CustomDrawnUGBackground oldUgBg)) {
                    oldUgBg.RenderBottomTransitionStrip(Main.spriteBatch, underworldLayer, transitionOut);
                }
            }
        }
        orig(main);
    }
    #endregion

    public override void Load() {
        BackgroundTextureLoader.AddBackgroundTexture(Mod, AequusTextures.None.Path);
        EmptyBackgroundSlot = BackgroundTextureLoader.GetBackgroundSlot(Mod, AequusTextures.None.ModPath);

        On_Main.DoDraw_WallsTilesNPCs += On_Main_DoDraw_WallsTilesNPCs;
        Main.OnPreDraw += Main_OnPreDraw;
    }

    public override void Unload() {
        Main.OnPreDraw -= Main_OnPreDraw;
        _backgroundTarget = null;
        _oldBackgroundTarget = null;
        _customDrawBGLookup.Clear();
    }
}

public abstract class CustomDrawnUGBackground : ModUndergroundBackgroundStyle {
    public sealed override void SetStaticDefaults() {
        UndergroundBackgroundSystem._customDrawBGLookup.Add(Slot, this);
    }

    /// <summary>
    /// Renders the background to the background render target.
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="graphicsDevice"></param>
    public abstract void RenderBackground(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice);
    public abstract void RenderTopTransitionStrip(SpriteBatch spriteBatch, int worldY, float opacity);
    public abstract void RenderBottomTransitionStrip(SpriteBatch spriteBatch, int worldY, float opacity);

    protected static void DrawBackgroundSimple(Texture2D texture, float horizontalScrollSpeed, Color colorTint, float upScale = 1.5f) {
        Vector2 bgScale = new Vector2(1f, Math.Max(Main.screenHeight / (float)texture.Height, 1f)) * upScale;
        float bgX = (1f - Main.screenPosition.X * horizontalScrollSpeed % 1f) * (texture.Width * bgScale.X);
        float bgY = Main.screenPosition.Y / 16f / Main.maxTilesY;

        int bgXLoops = (int)(Main.screenWidth / (texture.Width * bgScale.X)) + 1;
        float bgDrawY = (Main.screenHeight - texture.Height * bgScale.Y) * bgY;
        for (int i = -1; i < bgXLoops; i++) {
            Main.spriteBatch.Draw(texture, new Vector2(bgX + texture.Width * bgScale.X * i, bgDrawY), null, colorTint, 0f, Vector2.Zero, bgScale, SpriteEffects.None, 0f);
        }
    }

    protected static void DrawStrip(int Y, Texture2D transitionTexture, float opacity, SpriteEffects spriteEffects) {
        Y = (int)(Y * 16f - Main.screenPosition.Y);
        int transitionTextureWidth = transitionTexture.Width - 32;
        int bgLoops = Main.screenWidth / transitionTextureWidth + 2;
        int bgStartX = (int)(-Math.IEEERemainder(transitionTextureWidth + (double)Main.screenPosition.X * Main.caveParallax, transitionTextureWidth) - transitionTextureWidth / 2);
        for (int l = 0; l < bgLoops; l++) {
            for (int m = 0; m < transitionTextureWidth / 16; m++) {
                int frameX = (int)(float)Math.Round(0f - (float)Math.IEEERemainder(bgStartX + Main.screenPosition.X, 16.0));
                if (frameX == -8) {
                    frameX = 8;
                }
                float x = bgStartX + transitionTextureWidth * l + m * 16 + 8;
                Vector2 drawCoordinates = new Vector2(x - 8f + frameX, Y + 16f + 96f * UndergroundBackgroundSystem.TransitionStrips);
                Color color = ExtendLight.Get(drawCoordinates + Main.screenPosition) * opacity;
                Rectangle frame = new Rectangle(16 * m + frameX + 16, 0, 16, 16);
                Main.spriteBatch.Draw(transitionTexture, drawCoordinates, frame, color, 0f, Vector2.Zero, 1f, spriteEffects, 0f);
            }
        }
    }
}