using Aequus.Core.Assets;
using Aequus.Core.Graphics;
using System;
using System.Collections.Generic;

namespace Aequus.Content.Critters.SeaFirefly;

[Autoload(Side = ModSide.Client)]
internal class SeaFireflyRenderer : RequestHandler<SeaFireflyShaderRequest> {
    public static SeaFireflyRenderer Instance { get; private set; }

    public Effect Effect => AequusShaders.SeaFirefly.Value;

    private RenderTarget2D _target;
    private EffectParameter _glowMagnitude;
    private EffectParameter _outlineGlowMagnitude;
    private EffectParameter _imageSize;
    private EffectPass _tilePass;
    private EffectPass _waterPass;

    protected override void OnActivate() {
        DrawHelper.AddPreDrawHook(HandleRequestsOnPreDraw);
        DrawLayers.Instance.PostDrawLiquids += DrawOntoScreen;
    }

    protected override void OnDeactivate() {
        DrawHelper.RemovePreDrawHook(HandleRequestsOnPreDraw);
        DrawLayers.Instance.PostDrawLiquids -= DrawOntoScreen;
    }

    private void HandleRequestsOnPreDraw(GameTime gameTime) {
        HandleRequests();
    }

    protected override bool HandleRequests(IEnumerable<SeaFireflyShaderRequest> todo) {
        if (Main.gameMenu || Main.instance.tileTarget == null || Main.instance.tileTarget.IsDisposed || Main.instance.tileTarget.IsContentLost) {
            ClearQueue();
            return false;
        }

        SpriteBatch spriteBatch = Main.spriteBatch;
        GraphicsDevice device = Main.instance.GraphicsDevice;

        if (!DrawHelper.CheckTargetCycle(ref _target, Main.instance.tileTarget.Width, Main.instance.tileTarget.Height, device, RenderTargetUsage.PreserveContents)) {
            ClearQueue();
            return false;
        }

        RenderTargetBinding[] oldTargets = device.GetRenderTargets();

        device.SetRenderTarget(_target);
        device.Clear(Color.Transparent);
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        try {

            DrawRequests(spriteBatch, todo);
        }
        catch (Exception ex) {
            ExtendedMod.Log.Error(ex);
        }
        finally {
            spriteBatch.End();
        }

        ClearQueue();

        device.SetRenderTargets(oldTargets);

        return true;
    }

    private static void DrawRequests(SpriteBatch spriteBatch, IEnumerable<SeaFireflyShaderRequest> todo) {
#if DEBUG
        Instance.Load();
#endif
        spriteBatch.GraphicsDevice.Textures[1] = AequusTextures.EffectWaterRefraction;
        Texture2D texture = AequusTextures.Bloom;
        Rectangle frame = texture.Bounds;
        Vector2 origin = frame.Size() / 2f;
        Texture2D maskTexture = AequusTextures.EffectWaterRefraction;
        Instance._glowMagnitude.SetValue(Helper.Oscillate(Main.GlobalTimeWrappedHourly * 2.5f, 4f, 6f));

        foreach (SeaFireflyShaderRequest request in todo) {
            Instance._imageSize.SetValue(request.Where * 0.01f + new Vector2(0f, Main.GlobalTimeWrappedHourly * -0.03f + request.Where.X * 0.1f));
            Instance.Effect.CurrentTechnique.Passes["Refraction"].Apply();

            Vector2 drawCoordinates = request.Where - Main.screenPosition + TileHelper.DrawOffset;
            Color color = request.Color;
            spriteBatch.Draw(texture, drawCoordinates, frame, color, 0f, origin, request.Scale, SpriteEffects.None, 0f);
        }
    }

    public override void Load() {
        Instance = this;
        if (!Main.dedServ) {
            Effect effect = Effect;
            _glowMagnitude = effect.Parameters["glowMagnitude"];
            _outlineGlowMagnitude = effect.Parameters["outlineGlowMagnitude"];
            _imageSize = effect.Parameters["imageSize"];
            _tilePass = effect.CurrentTechnique.Passes["Tile"];
            _waterPass = effect.CurrentTechnique.Passes["Water"];
        }
    }

    public override void Unload() {
        DrawHelper.DiscardTarget(ref _target);
        Instance = null;
    }

    public static void DrawOntoScreen(SpriteBatch sb) {
        //Main.NewText(Main.GameUpdateCount);
        Instance.DrawTargetOntoScreen(sb);
    }

    private void DrawTargetOntoScreen(SpriteBatch spriteBatch) {
        if (_target == null) {
            return;
        }
        DrawHelper.SpriteBatchCache.InheritFrom(spriteBatch);

        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.ZoomMatrix);
        //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Matrix.Identity);

        spriteBatch.GraphicsDevice.Textures[1] = _target;
        _glowMagnitude.SetValue(0.15f);
        _outlineGlowMagnitude.SetValue(Helper.Oscillate(Main.GlobalTimeWrappedHourly, 0.2f, 0.46f));
        _imageSize.SetValue(new Vector2(_target.Width, _target.Height));
        _tilePass.Apply();

        spriteBatch.Draw(Main.instance.tileTarget, Main.sceneTilePos - Main.screenPosition, Color.White);

        _glowMagnitude.SetValue(0.75f);
        _outlineGlowMagnitude.SetValue(Helper.Oscillate(Main.GlobalTimeWrappedHourly * 5f, 2f, 3f));
        _imageSize.SetValue(new Vector2(_target.Width, _target.Height));
        _waterPass.Apply();

        spriteBatch.Draw(Main.instance.backWaterTarget, Main.sceneWaterPos - Main.screenPosition, Color.White);

        spriteBatch.End();
        DrawHelper.SpriteBatchCache.Begin(spriteBatch, SpriteSortMode.Deferred, null, Main.GameViewMatrix.ZoomMatrix);
    }
}

public record struct SeaFireflyShaderRequest(Vector2 Where, float Scale, Color Color);
