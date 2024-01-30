using Aequus.Core.Assets;
using Aequus.Core.Graphics;
using Aequus.Core.Particles;
using System;

namespace Aequus.Content.Graphics.Particles;

/// <summary>
/// Bubble particles which only work underwater. Floats upwards.
/// </summary>
public class UnderwaterBubbleParticles : ParallelParticleSystem<UnderwaterBubbleParticles.Bubble> {
    public const Int32 FRAME_COUNT_X = 9;
    public const Int32 FRAME_COUNT_Y = 5;

    public const Int32 FRAME_Y_CULLABLE_OUTLINE = 0;
    public const Int32 FRAME_Y_HIGHLIGHTS = 1;
    public const Int32 FRAME_Y_CULL_MASK = 2;
    public const Int32 FRAME_Y_CULL_OUTLINE_MIXING = 3; // Unused
    public const Int32 FRAME_Y_BASIC = 4;

    public override Int32 ParticleCount => 100;

    public static Boolean AllowMergeDrawing => Aequus.highQualityEffects;

    public override void Draw(SpriteBatch spriteBatch) {
        if (!AllowMergeDrawing) {
            spriteBatch.BeginDusts();
            DrawAll(spriteBatch, FRAME_Y_BASIC);
            spriteBatch.End();
            return;
        }

        if (!Main.IsGraphicsDeviceAvailable || DrawHelper.BadRenderTarget(_mergeTarget) || DrawHelper.BadRenderTarget(_bubbleOutlineTarget)) {
            return;
        }

        //spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

        try {
            Effect mergeEffect = AequusShaders.BubbleMerge.Value;
            GraphicsDevice g = Main.instance.GraphicsDevice;
            g.Textures[1] = _bubbleOutlineTarget;
            mergeEffect.CurrentTechnique.Passes[0].Apply();

            spriteBatch.Draw(_mergeTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
            //spriteBatch.Draw(_bubbleOutlineTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
        }
        catch {

        }

        spriteBatch.End();

        spriteBatch.BeginDusts();
        DrawAll(spriteBatch, FRAME_Y_HIGHLIGHTS);
        spriteBatch.End();
        //spriteBatch.BeginDusts();
    }

    private void DrawAll(SpriteBatch spriteBatch, Int32 frameY, Single minOpacity = 0f) {
        Texture2D texture = AequusTextures.BubbleParticles;
        lock (this) {
            for (Int32 k = 0; k < Particles.Length; k++) {
                Bubble bubble = Particles[k];

                if (bubble == null || !bubble.Active) {
                    continue;
                }

                Rectangle frame = texture.Frame(FRAME_COUNT_X, FRAME_COUNT_Y, bubble.Frame, frameY);
                Vector2 origin = frame.Size() / 2f;

                Single rotation = 0f;
                Single scale = Helper.Oscillate(Main.GameUpdateCount / 60f + k, 0.9f, 1.1f);
                Vector2 drawLocation = bubble.Location - Main.screenPosition;
                Vector2 velocity = bubble.Velocity;
                Color color = ExtendLight.Get(bubble.Location) * Math.Max(minOpacity, bubble.Opacity);

                spriteBatch.Draw(texture, drawLocation, frame, color, rotation, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }

    protected override void UpdateParallel(Int32 start, Int32 end) {
        for (Int32 i = start; i < end; i++) {
            Bubble bubble = Particles[i];
            if (bubble == null || !bubble.Active) {
                continue;
            }
            Active = true;

            Single opacity = bubble.Opacity;
            if (Main.rand.NextBool(240)) {
                bubble.Frame--;
            }
            if (!Collision.WetCollision(bubble.Location, 0, 0) || Collision.LavaCollision(bubble.Location, 0, 0) || bubble.Frame < 0 || opacity <= 0f) {
                bubble.Active = false;
                continue;
            }

            //particle.Rotation = particle.Velocity.ToRotation() + MathHelper.Pi;
            bubble.Location += bubble.Velocity;
            bubble.Velocity.X *= 0.995f;
            if (bubble.Velocity.Y > 0f) {
                bubble.Velocity.Y *= 0.99f;
            }
            bubble.UpLift *= 0.99f;
            bubble.Velocity.Y -= bubble.UpLift;
            bubble.Opacity -= 0.001f;
        }
    }

    public override void Activate() {
        Main.OnPreDraw += DrawMergeTarget;
        DrawLayers.Instance.PostDrawDust += Draw;
    }

    public override void Deactivate() {
        Main.OnPreDraw -= DrawMergeTarget;
        DrawLayers.Instance.PostDrawDust -= Draw;
    }

    #region Merging effect
    private RenderTarget2D _mergeTarget;
    private RenderTarget2D _bubbleOutlineTarget;

    public void DrawMergeTarget(GameTime gameTime) {
        if (!AllowMergeDrawing) {
            return;
        }

        SpriteBatch sb = Main.spriteBatch;
        GraphicsDevice g = Main.instance.GraphicsDevice;
        if (DrawHelper.BadRenderTarget(_mergeTarget, Main.screenWidth, Main.screenHeight) || DrawHelper.BadRenderTarget(_bubbleOutlineTarget, Main.screenWidth, Main.screenHeight)) {
            try {
                if (Main.IsGraphicsDeviceAvailable) {
                    _mergeTarget = new RenderTarget2D(g, Main.screenWidth, Main.screenHeight, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                    _bubbleOutlineTarget = new RenderTarget2D(g, Main.screenWidth, Main.screenHeight, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                }
            }
            catch {
            }
            return;
        }

        RenderTargetBinding[] oldTargets = g.GetRenderTargets();

        try {
            g.SetRenderTarget(_mergeTarget);
            g.Clear(Color.Transparent);

            sb.BeginDusts();
            //DrawAll(sb, FRAME_Y_CULL_OUTLINE_MIXING);
            DrawAll(sb, FRAME_Y_CULL_MASK, minOpacity: 1f);
            sb.End();

            g.SetRenderTarget(_bubbleOutlineTarget);
            g.Clear(Color.Transparent);

            sb.BeginDusts();
            DrawAll(sb, FRAME_Y_CULLABLE_OUTLINE);
            sb.End();
        }
        catch {
        }
        finally {
        }

        g.SetRenderTargets(oldTargets);
    }
    #endregion

    public class Bubble : IParticle {
        private Boolean _active;
        public Boolean Active {
            get => _active;
            set {
                if (!_active) {
                    Opacity = 1f;
                }
                _active = value;
            }
        }

        public Vector2 Location;
        public Vector2 Velocity;

        public Single UpLift;
        public Single Opacity;

        public Byte Frame;
    }
}
