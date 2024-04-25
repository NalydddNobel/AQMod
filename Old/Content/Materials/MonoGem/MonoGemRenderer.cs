using Aequus.Core.Graphics;
using Aequus.Core.Graphics.Tiles;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace Aequus.Old.Content.Materials.MonoGem;

public class MonoGemRenderer : ILoad {
    private class MonoGemScreenShaderData : ScreenShaderData {
        public MonoGemScreenShaderData(Ref<Effect> shader, string passName) : base(shader, passName) {
        }

        public override void Apply() {
            Main.instance.GraphicsDevice.Textures[1] = Instance._target;
            base.Apply();
        }
    }

    public static MonoGemRenderer Instance { get; private set; }
    private readonly List<Point> FogDrawQueue = new();

    public const string ScreenShaderKey = "Aequus:MonoGem";

    private RenderTarget2D _target;
    public bool Prepared { get; private set; }
    private bool _active;

    public void Load(Mod mod) {
        Instance = this;

        if (!Main.dedServ) {
            SpecialTileRenderer.PreDrawNonSolidTiles += Instance.FogDrawQueue.Clear;
            Filters.Scene[ScreenShaderKey] = new Filter(new MonoGemScreenShaderData(
                new Ref<Effect>(
                    ModContent.Request<Effect>($"{this.NamespaceFilePath()}/MonoGemScreenShader",
                    AssetRequestMode.ImmediateLoad).Value),
                "GrayscaleMaskPass"), EffectPriority.Low);
        }
    }

    public void Unload() {
        Main.OnPreDraw -= DrawOntoTarget;
        DrawHelper.DiscardTarget(ref _target);
        Instance = null;
    }

    public void AddFog(int i, int j) {
        FogDrawQueue.Add(new Point(i, j));
        if (!_active) {
            Main.OnPreDraw += DrawOntoTarget;
            DrawLayers.Instance.PostDrawDust += DrawOntoScreen;
            _active = true;
        }
    }

    protected void DrawOntoTarget(GameTime gameTime) {
        Prepared = false;

        if (Main.gameMenu) {
            return;
        }

        if (FogDrawQueue.Count <= 0) {
            if (_active) {
                Main.OnPreDraw -= DrawOntoTarget;
                DrawLayers.Instance.PostDrawDust -= DrawOntoScreen;
            }
            _active = false;
        }

        SpriteBatch sb = Main.spriteBatch;
        GraphicsDevice g = Main.instance.GraphicsDevice;
        if (DrawHelper.BadRenderTarget(_target, Main.screenWidth, Main.screenHeight)) {
            try {
                DrawHelper.DiscardTarget(ref _target);

                if (Main.IsGraphicsDeviceAvailable) {
                    _target = new RenderTarget2D(g, Main.screenWidth, Main.screenHeight, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                }
            }
            catch {
            }
            return;
        }

        RenderTargetBinding[] oldTargets = g.GetRenderTargets();

        sb.BeginDusts();
        try {
            g.SetRenderTarget(_target);
            g.Clear(Color.Transparent);

            DrawFog(sb);
        }
        catch (Exception ex) {
            Aequus.Log.Error(ex);
        }
        finally {
            sb.End();
        }

        g.SetRenderTargets(oldTargets);

        Prepared = true;
    }

    private void DrawFog(SpriteBatch sb) {
        const int FOG_TEXTURE_FRAME_COUNT = 8;

        Texture2D fogTexture = AequusTextures.Fog;
        Rectangle fogTextureFrame = fogTexture.Frame(verticalFrames: FOG_TEXTURE_FRAME_COUNT, frameY: 0);
        Vector2 fogTextureOrigin = fogTextureFrame.Size() / 2f;
        Texture2D bloomStrong = AequusTextures.BloomStrong;
        Vector2 bloomStrongOrigin = bloomStrong.Size() / 2f;
        Color white = Color.White;
        Color fogColor = Color.White with { A = 0 };
        foreach (Point d in FogDrawQueue) {
            ulong seed = Helper.TileSeed(d);
            float pulse = Helper.Oscillate(Main.GlobalTimeWrappedHourly * 0.5f + Utils.RandomFloat(ref seed) * 20f, 0.7f, 1f);
            Vector2 drawCoordinates = new Vector2(d.X * 16f, d.Y * 16f) + new Vector2(8f) - Main.screenPosition;

            sb.Draw(bloomStrong, drawCoordinates, null, white * pulse, 0f, bloomStrongOrigin, 2f, SpriteEffects.None, 0f);

            for (int k = 0; k < 2; k++) {
                float rotationSpeed = Utils.RandomFloat(ref seed) * 0.25f;
                float rotation = Main.GlobalTimeWrappedHourly * rotationSpeed;
                float intensity = MathF.Sin((k * MathHelper.Pi / 3f + Main.GameUpdateCount / 20f) * Utils.RandomFloat(ref seed) % MathHelper.Pi);
                Rectangle frame = fogTextureFrame.Frame(0, Utils.RandomInt(ref seed, FOG_TEXTURE_FRAME_COUNT));
                sb.Draw(fogTexture, drawCoordinates, frame, fogColor * intensity, rotation, fogTextureOrigin, 1f + 1.25f * pulse, SpriteEffects.None, 0f);
            }
        }
    }

    public static void DrawOntoScreen(SpriteBatch sb) {
        //Main.NewText(Main.GameUpdateCount);
        Filter filter = Filters.Scene[ScreenShaderKey];
        if (Instance.Prepared) {
            if (!filter.IsActive()) {
                Filters.Scene.Activate(ScreenShaderKey, Main.LocalPlayer.Center);
                filter.GetShader().UseOpacity(1f);
            }

            if (!Lighting.NotRetro || !filter.IsActive() || !Filters.Scene.CanCapture()) {
                Instance.DrawTargetOntoScreen(sb);
            }
        }
        else {
            if (filter.IsActive()) {
                Filters.Scene.Deactivate(ScreenShaderKey, Main.LocalPlayer.Center);
                filter.GetShader().UseOpacity(0f);
            }
        }
    }

    public void DrawTargetOntoScreen(SpriteBatch spriteBatch) {
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Matrix.Identity);

        var drawData = new DrawData(_target, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(0, 0, 0, 128));

        drawData.Draw(spriteBatch);

        spriteBatch.End();
        Prepared = false;
    }
}