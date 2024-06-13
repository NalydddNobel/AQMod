using Aequus.Core.Assets;
using Aequus.Core.Graphics;
using FNAUtils.Drawing;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.GameContent.Shaders;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.UI.Chat;

namespace Aequus.Core.Utilities;

public sealed class DrawHelper : ModSystem {
    public delegate void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth);

    public static Vector2 ScreenCenter => Main.screenPosition + ScreenSize / 2f;
    public static Vector2 ScreenSize => new Vector2(Main.screenWidth, Main.screenHeight);

    public static Matrix View => Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) * Matrix.CreateTranslation(Main.graphics.GraphicsDevice.Viewport.Width / 2f, Main.graphics.GraphicsDevice.Viewport.Height / -2f, 0) * Matrix.CreateRotationZ(MathHelper.Pi);
    public static Matrix Projection => Matrix.CreateOrthographic(Main.graphics.GraphicsDevice.Viewport.Width, Main.graphics.GraphicsDevice.Viewport.Height, 0, 1000);

    private static BasicEffect _basicEffect;
    public static VertexStrip VertexStrip { get; private set; }

    public static readonly RasterizerState RasterizerState_BestiaryUI = new() {
        CullMode = CullMode.None,
        ScissorTestEnable = true
    };

    public static SpriteBatchCache SpriteBatchCache { get; private set; }

    public static Matrix WorldViewPointMatrix {
        get {
            var graphics = Main.graphics.GraphicsDevice;
            var screenZoom = Main.GameViewMatrix.Zoom;
            int width = graphics.Viewport.Width;
            int height = graphics.Viewport.Height;

            var zoom = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) *
                Matrix.CreateTranslation(width / 2f, height / -2f, 0) *
                Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(screenZoom.X, screenZoom.Y, 1f);
            var projection = Matrix.CreateOrthographic(width, height, 0, 1000);
            return zoom * projection;
        }
    }

    public static readonly GraphicsDeviceHelper graphics = new();

    public static int ColorOnlyShaderId => ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
    public static ArmorShaderData ColorOnlyShader => GameShaders.Armor.GetSecondaryShader(ColorOnlyShaderId, Main.LocalPlayer);

    /// <summary><inheritdoc cref="Quality(float, float)"/></summary>
    public static int Quality(int value, int min) {
        return (int)Quality((float)value, (float)min);
    }

    /// <summary>Multiplies <paramref name="value"/> by <see cref="Main.gfxQuality"/>, ensuring it is above <paramref name="min"/>.</summary>
    /// <returns></returns>
    public static float Quality(float value, float min) {
        return Math.Clamp(value * Main.gfxQuality, min, value);
    }

    public static void DrawMagicLensFlare(SpriteBatch spriteBatch, Vector2 drawPosition, Color color, float scale = 1f) {
        Vector2 screenCenter = new Vector2(Main.screenWidth, Main.screenHeight) / 2f;
        float distance = Vector2.Distance(drawPosition, screenCenter);

        float intensity = 1f - distance / (900f * scale);
        if (intensity <= 0f || float.IsNaN(intensity)) {
            return;
        }

        color *= intensity;
        color.A = 0;

        int textureCount = AequusTextures.LensFlares.Length;
        float lerpAmount = 2f / textureCount * (1f - intensity);

        for (int i = 0; i < textureCount; i++) {
            Texture2D texture = AequusTextures.LensFlares[i].Value;
            Vector2 position = Vector2.Lerp(drawPosition, screenCenter, lerpAmount * i);
            spriteBatch.Draw(texture, position, null, color, 0f, texture.Size() / 2f, scale, SpriteEffects.None, 0f);
        }
    }

    public static void DrawLine(Draw draw, Vector2 start, float rotation, float length, float width, Color color) {
        draw(TextureAssets.MagicPixel.Value, start, new Rectangle(0, 0, 1, 1), color, rotation, new Vector2(1f, 0.5f), new Vector2(length, width), SpriteEffects.None, 0f);
    }
    public static void DrawLine(Vector2 start, float rotation, float length, float width, Color color) {
        DrawLine(Main.spriteBatch.Draw, start, rotation, length, width, color);
    }
    public static void DrawLine(Draw draw, Vector2 start, Vector2 end, float width, Color color) {
        DrawLine(draw, start, (start - end).ToRotation(), (end - start).Length(), width, color);
    }
    public static void DrawLine(Vector2 start, Vector2 end, float width, Color color) {
        DrawLine(Main.spriteBatch.Draw, start, end, width, color);
    }

    public static void DrawBasicVertexLine(Texture2D texture, Vector2[] lineSegments, float[] lineRotations, VertexStrip.StripColorFunction getColor, VertexStrip.StripHalfWidthFunction getWidth, Vector2 offset = default, bool includeBacksides = true) {
        ApplyBasicEffect(texture);

        VertexStrip.PrepareStrip(lineSegments, lineRotations, getColor, getWidth, offset, includeBacksides: includeBacksides);
        VertexStrip.DrawTrail();
    }
    public static void DrawBasicVertexLineWithProceduralPadding(Texture2D texture, Vector2[] lineSegments, float[] lineRotations, VertexStrip.StripColorFunction getColor, VertexStrip.StripHalfWidthFunction getWidth, Vector2 offset = default, bool includeBacksides = true, bool tryStoppingOddBug = true) {
        ApplyBasicEffect(texture);

        VertexStrip.PrepareStripWithProceduralPadding(lineSegments, lineRotations, getColor, getWidth, offset, includeBacksides, tryStoppingOddBug);
        VertexStrip.DrawTrail();
    }

    public static void ApplyCurrentTechnique() {
        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
    }

    public static void GetWorldViewProjection(out Matrix view, out Matrix projection) {
        projection = Projection;
        view = View * Matrix.CreateScale(Main.GameViewMatrix.Zoom.X, Main.GameViewMatrix.Zoom.Y, 1f);
    }

    public static void ApplyBasicEffect(Texture2D texture = default, bool vertexColorsEnabled = true) {
        GetWorldViewProjection(out var view, out var projection);
        ApplyBasicEffect(view, projection, texture, vertexColorsEnabled);
    }
    public static void ApplyBasicEffect(Matrix view, Matrix projection, Texture2D texture = default, bool vertexColorsEnabled = true) {
        _basicEffect.VertexColorEnabled = vertexColorsEnabled;
        _basicEffect.Projection = projection;
        _basicEffect.View = view;

        if (_basicEffect.TextureEnabled = texture != null) {
            _basicEffect.Texture = texture;
        }

        foreach (var pass in _basicEffect.CurrentTechnique.Passes) {
            pass.Apply();
        }
    }
    public static void ApplyUVEffect(Texture2D texture, Vector2 uvMultiplier, Vector2 uvAdd) {
        GetWorldViewProjection(out var view, out var projection);

        Main.instance.GraphicsDevice.Textures[0] = texture;
        var effect = AequusShaders.UVVertexShader.Value;
        effect.CurrentTechnique = effect.Techniques["UVWrap"];
        effect.Parameters["XViewProjection"].SetValue(view * projection);
        effect.Parameters["UVMultiplier"].SetValue(uvMultiplier);
        effect.Parameters["UVAdd"].SetValue(uvAdd);
        foreach (var pass in effect.CurrentTechnique.Passes) {
            pass.Apply();
        }
    }

    public static Color GetYoyoStringColor(int stringColorId) {
        if (stringColorId == 27) {
            return Main.DiscoColor;
        }
        return WorldGen.paintColor(stringColorId);
    }

    public static Vector2 ApplyZoom(Vector2 screenCoordinate, float zoomFactor) {
        Vector2 screenCenter = new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);
        Vector2 difference = screenCoordinate - screenCenter;
        float zoom = zoomFactor;
        return screenCenter + difference * zoom;
    }
    public static float ApplyZoomY(float screenCoordinateY, float zoomFactor) {
        float screenCenterY = Main.screenHeight / 2f;
        float differenceY = screenCoordinateY - screenCenterY;
        float zoom = zoomFactor;
        return screenCenterY + differenceY * zoom;
    }
    public static float ApplyZoomX(float screenCoordinateX, float zoomFactor) {
        float screenCenterX = Main.screenWidth / 2f;
        float differenceX = screenCoordinateX - screenCenterX;
        float zoom = zoomFactor;
        return screenCenterX + differenceX * zoom;
    }

    #region Text
    public static void DrawCenteredText(SpriteBatch sb, DynamicSpriteFont font, string text, Vector2 position, Color? color = null, float rotation = 0f, Vector2? scale = null) {
        Vector2 realScale = scale ?? new Vector2(1f, 1f);
        Color realColor = color ?? Color.White;
        Vector2 origin = ChatManager.GetStringSize(font, text, new Vector2(1f, 1f));
        ChatManager.DrawColorCodedString(sb, font, text, position, realColor, rotation, origin, realScale);
    }
    #endregion

    #region Dust
    public static int LiquidTypeToDustId(int liquidType) {
        return liquidType switch {
            LiquidID.Shimmer => DustID.ShimmerSplash,
            LiquidID.Honey => DustID.Honey,
            LiquidID.Lava => DustID.Lava,
            _ => Dust.dustWater(),
        };
    }
    #endregion

    #region Water
    public static WaterShaderData WaterShader => (WaterShaderData)Filters.Scene["WaterDistortion"].GetShader();

    public static void AddWaterRipple(Vector2 where, float r, float g, float b, Vector2 size, RippleShape shape = RippleShape.Square, float rotation = 0f) {
        AddWaterRipple(where, new(r, g, b), size, shape, rotation);
    }

    public static void AddWaterRipple(Vector2 where, Color waveData, Vector2 size, RippleShape shape = RippleShape.Square, float rotation = 0f) {
        var w = WaterShader;

        w.QueueRipple(where, waveData, size, shape, rotation);
    }
    #endregion

    #region Textures
    private static readonly List<RenderTarget2D> _loadedRenderTargets = new();

    public static RenderTarget2D NewRenderTarget(GraphicsDevice gd, int width, int height, RenderTargetUsage usage = RenderTargetUsage.DiscardContents) {
        return NewRenderTarget(new RenderTarget2D(gd, width, height, mipMap: false, SurfaceFormat.Color, DepthFormat.None, preferredMultiSampleCount: 0, usage));
    }

    public static RenderTarget2D NewRenderTarget(RenderTarget2D target) {
        if (!_loadedRenderTargets.Contains(target)) {
            _loadedRenderTargets.Add(target);
        }

        return target;
    }

    public static bool CheckTargetCycle(ref RenderTarget2D target, int desiredWidth, int desiredHeight, GraphicsDevice device = null, RenderTargetUsage usage = RenderTargetUsage.DiscardContents) {
        if (!BadRenderTarget(target, desiredWidth, desiredHeight)) {
            return true;
        }

        device ??= Main.instance.GraphicsDevice;
        try {
            DiscardTarget(ref target);

            if (Main.IsGraphicsDeviceAvailable) {
                target = NewRenderTarget(device, desiredWidth, desiredHeight, RenderTargetUsage.PreserveContents);
            }
        }
        catch (Exception ex) {
            Aequus.Log.Error(ex);
        }
        finally {
        }
        return false;
    }

    public static void DiscardTarget(ref RenderTarget2D target) {
        if (target == null) {
            return;
        }

        _loadedRenderTargets.Remove(target);
        if (!target.IsDisposed) {
            Main.QueueMainThreadAction(target.Dispose);
        }

        target = null;
    }

    public static bool BadRenderTarget(RenderTarget2D renderTarget2D) {
        return renderTarget2D == null || renderTarget2D.IsDisposed || renderTarget2D.IsContentLost;
    }
    public static bool BadRenderTarget(RenderTarget2D renderTarget2D, int desiredWidth, int desiredHeight) {
        return BadRenderTarget(renderTarget2D) || renderTarget2D.Width != desiredWidth || renderTarget2D.Height != desiredHeight;
    }
    #endregion

    #region Hooks
    private static readonly List<Action<GameTime>> _loadedPreDrawHooks = new();

    public static void AddPreDrawHook(Action<GameTime> action) {
        _loadedPreDrawHooks.Add(action);
        Main.OnPreDraw += action;
    }
    public static void RemovePreDrawHook(Action<GameTime> action) {
        _loadedPreDrawHooks.Remove(action);
        Main.OnPreDraw -= action;
    }
    #endregion

    #region Initialization
    public override void Load() {
        if (Main.dedServ) {
            return;
        }

        SpriteBatchCache = new();
        VertexStrip = new();
        Main.QueueMainThreadAction(LoadOnMainThread);
    }


    public override void Unload() {
        SpriteBatchCache = null;
        VertexStrip = null;

        // Unload Render Targets.
        for (int i = 0; i < _loadedRenderTargets.Count; i++) {
            RenderTarget2D target = _loadedRenderTargets[i];
            DiscardTarget(ref target);
        }
        _loadedRenderTargets.Clear();

        // Unload pre draw hooks.
        for (int i = 0; i < _loadedPreDrawHooks.Count; i++) {
            RemovePreDrawHook(_loadedPreDrawHooks[i]);
        }
        _loadedRenderTargets.Clear();

        Main.QueueMainThreadAction(UnloadOnMainThread);
    }

    private static void LoadOnMainThread() {
        _basicEffect = new(Main.graphics.GraphicsDevice);
    }
    private static void UnloadOnMainThread() {
        _basicEffect?.Dispose();
        _basicEffect = null;
    }
    #endregion
}