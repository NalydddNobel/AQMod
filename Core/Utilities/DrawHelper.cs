using Aequus.Common.NPCs;
using Aequus.Common.Particles;
using Aequus.Content.Items.Material.MonoGem;
using Aequus.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus;

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

    public static void DrawBasicVertexLine(Texture2D texture, Vector2[] lineSegments, float[] lineRotations, VertexStrip.StripColorFunction getColor, VertexStrip.StripHalfWidthFunction getWidth, Vector2 offset = default, bool includeBacksides = true, bool tryStoppingOddBug = true) {
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

    public static void ApplyBasicEffect(Texture2D texture = default, bool vertexColorsEnabled = true) {
        GetWorldViewProjection(out var view, out var projection);

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

    public static Color GetYoyoStringColor(int stringColorId) {
        if (stringColorId == 27) {
            return Main.DiscoColor;
        }
        return WorldGen.paintColor(stringColorId);
    }

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
        On_Main.DrawDust += On_Main_DrawDust;
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
    private static void On_Main_DrawDust(On_Main.orig_DrawDust orig, Main main) {
        orig(main);
        var particleRenderer = ParticleSystem.GetLayer(ParticleLayer.AboveDust);
        if (particleRenderer.Particles.Count > 0) {
            Main.spriteBatch.BeginWorld();
            particleRenderer.Draw(Main.spriteBatch);
            Main.spriteBatch.End();
        }
        MonoGemRenderer.HandleScreenRender();
    }

    private static void On_Main_DrawItems(On_Main.orig_DrawItems orig, Main main) {
        orig(main);
        ParticleSystem.GetLayer(ParticleLayer.AboveItems).Draw(Main.spriteBatch);
    }

    private static void On_Main_DrawNPCs(On_Main.orig_DrawNPCs orig, Main main, bool behindTiles) {
        if (!behindTiles) {
            orig(main, behindTiles);
            ParticleSystem.GetLayer(ParticleLayer.AboveNPCs).Draw(Main.spriteBatch);
        }
        else {
            ParticleSystem.GetLayer(ParticleLayer.BehindAllNPCsBehindTiles).Draw(Main.spriteBatch);
            orig(main, behindTiles);
        }
    }

    private static void On_Main_DrawNPC(On_Main.orig_DrawNPC orig, Main main, int iNPCIndex, bool behindTiles) {
        if (!Main.npc[iNPCIndex].TryGetGlobalNPC<AequusNPC>(out var aequusNPC)) {
            orig(main, iNPCIndex, behindTiles);
            return;
        }

        Vector2 drawOffset = Vector2.Zero;
        aequusNPC.DrawBehindNPC(iNPCIndex, behindTiles, ref drawOffset);
        Main.npc[iNPCIndex].position += drawOffset;
        orig(main, iNPCIndex, behindTiles);
        Main.npc[iNPCIndex].position -= drawOffset;
        aequusNPC.DrawAboveNPC(iNPCIndex, behindTiles);
    }
    #endregion
}