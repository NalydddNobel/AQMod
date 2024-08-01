using Aequus.NPCs;
using Terraria.GameContent;
using Terraria.GameContent.Shaders;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace Aequus.Common.Utilities;

public sealed class DrawHelper : ModSystem {
    public delegate void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth);

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

    public static int ColorOnlyShaderId => ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
    public static ArmorShaderData ColorOnlyShader => GameShaders.Armor.GetSecondaryShader(ColorOnlyShaderId, Main.LocalPlayer);

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

    public static void DrawBasicVertexLineWithProceduralPadding(Texture2D texture, Vector2[] lineSegments, float[] lineRotations, VertexStrip.StripColorFunction getColor, VertexStrip.StripHalfWidthFunction getWidth, Vector2 offset = default, bool includeBacksides = true, bool tryStoppingOddBug = true) {
        ApplyBasicEffect(texture);

        VertexStrip.PrepareStripWithProceduralPadding(lineSegments, lineRotations, getColor, getWidth, offset, includeBacksides, tryStoppingOddBug);
        VertexStrip.DrawTrail();
    }

    public static void ApplyCurrentTechnique() {
        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
    }

    public static void GetWorldViewProjection(out Matrix view, out Matrix projection) {
        int width = Main.graphics.GraphicsDevice.Viewport.Width;
        int height = Main.graphics.GraphicsDevice.Viewport.Height;
        projection = Matrix.CreateOrthographic(width, height, 0, 1000);
        view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) *
            Matrix.CreateTranslation(width / 2f, height / -2f, 0) * Matrix.CreateRotationZ(MathHelper.Pi) *
            Matrix.CreateScale(Main.GameViewMatrix.Zoom.X, Main.GameViewMatrix.Zoom.Y, 1f);
    }

    public static void ApplyBasicEffect(Texture2D texture = default, bool vertexTCommonColorEnabled = true) {
        GetWorldViewProjection(out var view, out var projection);

        _basicEffect.VertexColorEnabled = vertexTCommonColorEnabled;
        _basicEffect.Projection = projection;
        _basicEffect.View = view;

        if (_basicEffect.TextureEnabled = texture != null) {
            _basicEffect.Texture = texture;
        }

        foreach (var pass in _basicEffect.CurrentTechnique.Passes) {
            pass.Apply();
        }
    }

    public static Color GetYoyoStringColor(int stringColorId) {
        if (stringColorId == 27) {
            return Main.DiscoColor;
        }
        return WorldGen.paintColor(stringColorId);
    }

    public static bool BadRenderTarget(RenderTarget2D renderTarget2D) {
        return renderTarget2D == null || renderTarget2D.IsDisposed || renderTarget2D.IsContentLost;
    }
    public static bool BadRenderTarget(RenderTarget2D renderTarget2D, int wantedWidth, int wantedHeight) {
        return BadRenderTarget(renderTarget2D) || renderTarget2D.Width != wantedWidth || renderTarget2D.Height != wantedHeight;
    }

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

    #region Initialization
    public override void Load() {
        if (Main.dedServ) {
            return;
        }

        SpriteBatchCache = new();
        VertexStrip = new();
        On_Main.DrawNPC += On_Main_DrawNPC;
        On_Main.DrawNPCs += On_Main_DrawNPCs;
        On_Main.DrawItems += On_Main_DrawItems;
        Main.QueueMainThreadAction(LoadShaders);
    }

    public override void Unload() {
        SpriteBatchCache = null;
        VertexStrip = null;
        Main.QueueMainThreadAction(UnloadShaders);
    }

    private static void LoadShaders() {
        _basicEffect = new(Main.graphics.GraphicsDevice);
    }
    private static void UnloadShaders() {
        _basicEffect = null;
    }
    #endregion

    #region Hooks

    private static void On_Main_DrawItems(On_Main.orig_DrawItems orig, Main main) {
        orig(main);
    }

    private static void On_Main_DrawNPCs(On_Main.orig_DrawNPCs orig, Main main, bool behindTiles) {
        orig(main, behindTiles);
    }

    private static void On_Main_DrawNPC(On_Main.orig_DrawNPC orig, Main main, int iNPCIndex, bool behindTiles) {
        if (!Main.npc[iNPCIndex].TryGetGlobalNPC<AequusNPC>(out var aequusNPC)) {
            orig(main, iNPCIndex, behindTiles);
            return;
        }

        orig(main, iNPCIndex, behindTiles);
    }
    #endregion
}