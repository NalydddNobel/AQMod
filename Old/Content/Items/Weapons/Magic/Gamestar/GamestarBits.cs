using Aequu2.Core.Assets;
using Aequu2.Core.Graphics;
using Aequu2.Core.Particles;
using ReLogic.Content;
using System;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace Aequu2.Old.Content.Items.Weapons.Magic.Gamestar;

public sealed class GamestarBits : ConcurrentParticles<GamestarBits.Particle> {
    public class GamestarScreenShaderData(Asset<Effect> shader, string passName) : ScreenShaderData(shader, passName) {
        public override void Apply() {
            GamestarBits bits = ModContent.GetInstance<GamestarBits>();
            if (bits._ready) {
                Main.instance.GraphicsDevice.Textures[1] = bits._target;
            }
            base.Apply();
        }
    }

    private const string ScreenShaderKey = "Aequu2:Gamestar";

    private RenderTarget2D _target;
    private bool _ready;

    public override void OnLoad() {
        Filters.Scene[ScreenShaderKey] = new Filter(new GamestarScreenShaderData(Aequu2Shaders.Gamestar.Preload(), "ModdersToolkitShaderPass"), EffectPriority.Low);
    }

    public override void Activate() {
        DrawLayers.Instance.PostUpdateScreenPosition += Draw;
        DrawLayers.Instance.PostDrawLiquids += DrawToScreen;
    }

    public override void Deactivate() {
        DrawLayers.Instance.PostUpdateScreenPosition -= Draw;
        DrawLayers.Instance.PostDrawLiquids -= DrawToScreen;
        DeactivateFilter();
    }

    public override void Update(Particle t) {
        do {
            if (!Main.rand.NextBool(3)) {
                break;
            }

            t.Scale--;
        }
        while (t.Active = t.Scale > 0);
        t.Position += t.Velocity;
        t.Velocity *= 0.96f;
    }

    public override void Draw(SpriteBatch spriteBatch) {
        _ready = false;
        if (!DrawHelper.CheckTargetCycle(ref _target, Main.screenWidth, Main.screenHeight)) {
            return;
        }

        GraphicsDevice gd = spriteBatch.GraphicsDevice;
        Texture2D texture = TextureAssets.MagicPixel.Value;
        Rectangle frame = new Rectangle(0, 0, 1, 1);
        Vector2 origin = new Vector2(0.5f, 0.5f);

        DrawHelper.graphics.EnqueueRenderTargetBindings(gd);
        gd.SetRenderTarget(_target);
        gd.Clear(Color.Transparent);
        spriteBatch.BeginWorld();

        _ready = true;
        try {
            foreach (var bit in _bag) {
                Vector2 drawCoordinates = Utils.Floor((bit.Position - Main.screenPosition) / 4f) * 4f;
                spriteBatch.Draw(texture, drawCoordinates, frame, Color.White, 0f, origin, bit.Scale * 2, SpriteEffects.None, 0f);
            }

        }
        catch (Exception ex) {
            _ready = false;
            Log.Error(ex);
        }
        finally {
            spriteBatch.End();
            DrawHelper.graphics.DequeueRenderTargetBindings(gd);
        }
    }

    private void DrawToScreen(SpriteBatch spriteBatch) {
        Filter filter = Filters.Scene[ScreenShaderKey];
        if (_ready) {
            if (!filter.IsActive()) {
                Filters.Scene.Activate(ScreenShaderKey, Main.LocalPlayer.Center);
                filter.GetShader().UseOpacity(1f);
            }

            if (!filter.IsActive() || !ScreenShadersActive) {
                //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Matrix.Identity);

                spriteBatch.Draw(_target, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(0, 64, 0, 128));

                //spriteBatch.End();
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

    public class Particle : IParticle {
        public bool Active { get; set; }

        public Vector2 Position;
        public Vector2 Velocity;
        public int Scale;

        public void Setup(Vector2 position, int scale, Vector2 velocity = default) {
            Position = position;
            Scale = scale;
            Velocity = velocity;
        }
    }
}
