using Aequus.Core.Assets;
using Aequus.Core.Debugging;
using Aequus.Core.Graphics;
using System;
using System.Collections.Generic;

namespace Aequus.Content.Critters.SeaFirefly;

[Autoload(Side = ModSide.Client)]
public class SeaFireflyRenderer : RequestHandler<SeaFireflyShaderRequest> {
    public static SeaFireflyRenderer Instance { get; private set; }

    public static Effect Effect => AequusShaders.SeaFirefly.Value;

    public bool Ready { get; private set; }

    private RenderTarget2D _target;
    private EffectParameter _glowMagnitude;
    private EffectParameter _outlineGlowMagnitude;
    private EffectParameter _imageSize;
    private EffectPass _tilePass;
    private EffectPass _waterPass;
    private EffectPass _refractPass;

    protected override void OnActivate() {
        DrawLayers.Instance.PostUpdateScreenPosition += HandleRequestsOnPreDraw;
        DrawLayers.Instance.PostDrawLiquids += DrawOntoScreen;
    }

    protected override void OnDeactivate() {
        Ready = false;
        DrawLayers.Instance.PostUpdateScreenPosition -= HandleRequestsOnPreDraw;
        DrawLayers.Instance.PostDrawLiquids -= DrawOntoScreen;
        DiagnosticsMenu.ClearStopwatch(DiagnosticsMenu.TimerType.SeaFireflies);
    }

    private void HandleRequestsOnPreDraw(SpriteBatch sb) {
        DiagnosticsMenu.StartStopwatch();
        HandleRequests();
        ClearQueue();
        DiagnosticsMenu.ClearOrEndStopwatch(DiagnosticsMenu.TimerType.SeaFireflies, !Active);
    }

    protected override bool HandleRequests(IEnumerable<SeaFireflyShaderRequest> todo) {
        if (Main.instance.tileTarget == null || Main.instance.tileTarget.IsDisposed || Main.instance.tileTarget.IsContentLost) {
            return false;
        }

        SpriteBatch spriteBatch = Main.spriteBatch;
        GraphicsDevice device = Main.instance.GraphicsDevice;

        if (!DrawHelper.CheckTargetCycle(ref _target, Main.instance.tileTarget.Width, Main.instance.tileTarget.Height, device, RenderTargetUsage.PreserveContents)) {
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
            Log.Error(ex);
        }
        finally {
            spriteBatch.End();
        }

        device.SetRenderTargets(oldTargets);

        return true;
    }

    private void DrawRequests(SpriteBatch spriteBatch, IEnumerable<SeaFireflyShaderRequest> todo) {
        Ready = false;
        if (Main.drawToScreen || !Lighting.NotRetro || !HighQualityEffects) {
            return;
        }

        spriteBatch.GraphicsDevice.Textures[1] = AequusTextures.EffectWaterRefraction;
        Texture2D texture = AequusTextures.Bloom;
        Rectangle frame = texture.Bounds;
        Vector2 origin = frame.Size() / 2f;
        Texture2D maskTexture = AequusTextures.EffectWaterRefraction;
        _glowMagnitude.SetValue(Helper.Oscillate(Main.GlobalTimeWrappedHourly * 2.5f, 2f, 4f));

        foreach (SeaFireflyShaderRequest request in todo) {
            _imageSize.SetValue(request.Where * 0.01f + new Vector2(0f, Main.GlobalTimeWrappedHourly * -0.03f + request.Where.X * 0.001f));
            _refractPass.Apply();

            Vector2 drawCoordinates = request.Where - Main.screenPosition + TileHelper.DrawOffset;
            Color color = request.Color;
            spriteBatch.Draw(texture, drawCoordinates, frame, color, 0f, origin, request.Scale, SpriteEffects.None, 0f);
        }

        Ready = true;
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
            _refractPass = effect.CurrentTechnique.Passes["Refraction"];
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
        if (_target == null || !Ready) {
            return;
        }
        DrawHelper.SpriteBatchCache.InheritFrom(spriteBatch);

        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.ZoomMatrix);
        //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Matrix.Identity);

        Vector2 targetSize = _target.Size();
        Vector2 screenSize = Main.ScreenSize.ToVector2();
        Vector2 offsetSize = (targetSize - screenSize) / 2f;
        Vector2 drawCoordinates = Main.sceneTilePos - Main.screenPosition;
        spriteBatch.GraphicsDevice.Textures[1] = _target;
        _glowMagnitude.SetValue(1f);
        _outlineGlowMagnitude.SetValue(3f);
        _imageSize.SetValue(new Vector2(_target.Width, _target.Height));
        Effect.Parameters["glowOffsetUV"].SetValue((drawCoordinates + offsetSize) / _target.Size());
        _tilePass.Apply();

        //Vector2 drawCoordinates = Main.sceneTilePos - Main.screenPosition + new Vector2(0f, -300f) + (Main.GlobalTimeWrappedHourly).ToRotationVector2() * 4f;
        spriteBatch.Draw(Main.instance.tileTarget, drawCoordinates, Color.White);

        drawCoordinates = Main.sceneBackgroundPos - Main.screenPosition;
        _glowMagnitude.SetValue(0.5f);
        _outlineGlowMagnitude.SetValue(3f);
        Effect.Parameters["glowOffsetUV"].SetValue((drawCoordinates + offsetSize) / _target.Size());
        _waterPass.Apply();

        spriteBatch.Draw(Main.instance.backWaterTarget, drawCoordinates, Color.White);

        spriteBatch.End();
        DrawHelper.SpriteBatchCache.Begin(spriteBatch, SpriteSortMode.Deferred, null, Main.GameViewMatrix.ZoomMatrix);
    }
}

public record struct SeaFireflyShaderRequest(Vector2 Where, float Scale, Color Color);
