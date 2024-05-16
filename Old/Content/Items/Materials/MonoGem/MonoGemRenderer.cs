using Aequus.Core;
using Aequus.Core.Graphics;
using Aequus.Core.Graphics.Tiles;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace Aequus.Old.Content.Items.Materials.MonoGem;

public class MonoGemRenderer : RequestHandler<Point> {
    private class MonoGemScreenShaderData : ScreenShaderData {
        public MonoGemScreenShaderData(Ref<Effect> shader, string passName) : base(shader, passName) {
        }

        public override void Apply() {
            Main.instance.GraphicsDevice.Textures[1] = Instance._target;
            base.Apply();
        }
    }

    public static MonoGemRenderer Instance { get; private set; }

    public const string ScreenShaderKey = "Aequus:MonoGem";

    private RenderTarget2D _target;

    protected override void OnActivate() {
        SpecialTileRenderer.ClearTileEffects += ClearQueue;
        DrawHelper.AddPreDrawHook(HandleRequestsOnPreDraw);
        DrawLayers.Instance.PostDrawDust += DrawOntoScreen;
    }

    protected override void OnDeactivate() {
        DeactivateFilter();
        SpecialTileRenderer.ClearTileEffects -= ClearQueue;
        DrawHelper.RemovePreDrawHook(HandleRequestsOnPreDraw);
        DrawLayers.Instance.PostDrawDust -= DrawOntoScreen;
    }

    private void HandleRequestsOnPreDraw(GameTime gameTime) {
        HandleRequests();
    }

    protected override bool HandleRequests(IEnumerable<Point> todo) {
        if (Main.gameMenu) {
            return false;
        }

        SpriteBatch spriteBatch = Main.spriteBatch;
        GraphicsDevice device = Main.instance.GraphicsDevice;

        if (!DrawHelper.CheckTargetCycle(ref _target, Main.screenWidth, Main.screenHeight, device, RenderTargetUsage.PreserveContents)) {
            return false;
        }

        RenderTargetBinding[] oldTargets = device.GetRenderTargets();

        spriteBatch.BeginDusts();
        try {
            device.SetRenderTarget(_target);
            device.Clear(Color.Transparent);

            DrawFog(spriteBatch, todo);
        }
        catch (Exception ex) {
            tModLoaderExtended.ExtendedMod.Log.Error(ex);
        }
        finally {
            spriteBatch.End();
        }

        device.SetRenderTargets(oldTargets);

        return true;
    }

    private static void DrawFog(SpriteBatch sb, IEnumerable<Point> todo) {
        Texture2D fogTexture = AequusTextures.Fog;
        int fogFrameCount = AequusTextures.FogFrameCount;
        Rectangle fogTextureFrame = fogTexture.Frame(verticalFrames: fogFrameCount, frameY: 0);
        Vector2 fogTextureOrigin = fogTextureFrame.Size() / 2f;
        Texture2D bloomStrong = AequusTextures.BloomStrong;
        Vector2 bloomStrongOrigin = bloomStrong.Size() / 2f;
        Color white = Color.White;
        Color fogColor = Color.White with { A = 0 };
        foreach (Point d in todo) {
            ulong seed = Helper.TileSeed(d);
            float pulse = Helper.Oscillate(Main.GlobalTimeWrappedHourly * 0.5f + Utils.RandomFloat(ref seed) * 20f, 0.7f, 1f);
            Vector2 drawCoordinates = new Vector2(d.X * 16f, d.Y * 16f) + new Vector2(8f) - Main.screenPosition;

            sb.Draw(bloomStrong, drawCoordinates, null, white * pulse, 0f, bloomStrongOrigin, 2f, SpriteEffects.None, 0f);

            for (int k = 0; k < 2; k++) {
                float rotationSpeed = Utils.RandomFloat(ref seed) * 0.25f;
                float rotation = Main.GlobalTimeWrappedHourly * rotationSpeed;
                float intensity = MathF.Sin((k * MathHelper.Pi / 3f + Main.GameUpdateCount / 20f) * Utils.RandomFloat(ref seed) % MathHelper.Pi);
                Rectangle frame = fogTextureFrame.Frame(0, Utils.RandomInt(ref seed, fogFrameCount));
                sb.Draw(fogTexture, drawCoordinates, frame, fogColor * intensity, rotation, fogTextureOrigin, 1f + 1.25f * pulse, SpriteEffects.None, 0f);
            }
        }
    }

    public override void Load() {
        Instance = this;

        if (!Main.dedServ) {
            SpecialTileRenderer.PreDrawNonSolidTiles += ClearQueue;
            Filters.Scene[ScreenShaderKey] = new Filter(new MonoGemScreenShaderData(
                new Ref<Effect>(
                    ModContent.Request<Effect>($"{this.NamespaceFilePath()}/MonoGemScreenShader",
                    AssetRequestMode.ImmediateLoad).Value),
                "GrayscaleMaskPass"), EffectPriority.Low);
        }
    }

    public override void Unload() {
        DrawHelper.DiscardTarget(ref _target);
        Instance = null;
    }

    public static void DrawOntoScreen(SpriteBatch sb) {
        //Main.NewText(Main.GameUpdateCount);
        Filter filter = Filters.Scene[ScreenShaderKey];
        if (Instance.Prepared) {
            if (!filter.IsActive()) {
                Filters.Scene.Activate(ScreenShaderKey, Main.LocalPlayer.Center);
                filter.GetShader().UseOpacity(1f);
            }

            if (!filter.IsActive() || !tModLoaderExtended.ExtendedMod.ScreenShadersActive) {
                Instance.DrawTargetOntoScreen(sb);
            }
        }
        else {
            if (filter.IsActive()) {
                DeactivateFilter();
            }
        }
    }

    private static void DeactivateFilter() {
        Filters.Scene.Deactivate(ScreenShaderKey, Main.LocalPlayer.Center);
        Filters.Scene[ScreenShaderKey].GetShader().UseOpacity(0f);
    }

    private void DrawTargetOntoScreen(SpriteBatch spriteBatch) {
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Matrix.Identity);

        var drawData = new DrawData(_target, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(0, 0, 0, 128));

        drawData.Draw(spriteBatch);

        spriteBatch.End();
    }
}