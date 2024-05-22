using Aequus.Core.Graphics;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;

namespace Aequus.Content.Critters.SeaFirefly;

internal class SeaFireflyRenderer : RequestHandler<SeaFireflyShaderRequest> {
    public static SeaFireflyRenderer Instance { get; private set; }

    private RenderTarget2D _target;

    protected override void OnActivate() {
        DrawHelper.AddPreDrawHook(HandleRequestsOnPreDraw);
        DrawLayers.Instance.PostDrawDust += DrawOntoScreen;
    }

    protected override void OnDeactivate() {
        DrawHelper.RemovePreDrawHook(HandleRequestsOnPreDraw);
        DrawLayers.Instance.PostDrawDust -= DrawOntoScreen;
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

        spriteBatch.BeginTiles();
        try {
            device.SetRenderTarget(_target);
            device.Clear(Color.Transparent);

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
        Texture2D texture = AequusTextures.BloomStrong;
        Rectangle frame = texture.Bounds;
        Vector2 origin = frame.Size() / 2f;
        foreach (SeaFireflyShaderRequest request in todo) {
            Vector2 drawCoordinates = request.Where - Main.screenPosition;
            Color color = request.Color;
            spriteBatch.Draw(texture, drawCoordinates + TileHelper.DrawOffset, frame, color, 0f, origin, request.Scale, SpriteEffects.None, 0f);
        }
    }

    public override void Load() {
        Instance = this;
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
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Matrix.Identity);

        Effect seaFireflyEffect = ModContent.Request<Effect>("Aequus/Assets/Shaders/SeaFireflies").Value;
        Main.instance.GraphicsDevice.Textures[1] = _target;
        seaFireflyEffect.Parameters["glowMagnitude"].SetValue(0.2f);
        seaFireflyEffect.Parameters["outlineGlowMagnitude"].SetValue(2f);
        seaFireflyEffect.Parameters["imageSize"].SetValue(new Vector2(_target.Width, _target.Height));
        seaFireflyEffect.CurrentTechnique.Passes[0].Apply();

        var drawData = new DrawData(Main.instance.tileTarget, Main.sceneTilePos - Main.screenPosition, Color.White);

        drawData.Draw(spriteBatch);

        spriteBatch.End();
    }

}

public record struct SeaFireflyShaderRequest(Vector2 Where, float Scale, Color Color);
