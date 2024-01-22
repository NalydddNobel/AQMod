using Aequus.Core;
using Aequus.Core.Graphics;
using System;
using System.Collections.Generic;
using Terraria.GameContent;

namespace Aequus.Content.Biomes.PollutedOcean.Background;

public class PollutedOceanUndergroundBG : CustomDrawnUGBackground {
    public override void FillTextureArray(int[] textureSlots) {
        textureSlots[0] = 290; // Beach Top Layer
        textureSlots[1] = 291; // Beach Fill
        textureSlots[2] = 293; // Corrupted Beach Fill
        textureSlots[3] = 293; // Corrupted Beach Fill
    }

    public override void RenderBackground(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice) {
        if (Main.screenPosition.Y + Main.screenHeight < Main.worldSurface * 16) {
            return;
        }

        if (!TextureAssets.Underworld[2].IsLoaded) {
            Main.Assets.Request<Texture2D>(TextureAssets.Underworld[2].Name);
        }
        if (!TextureAssets.Underworld[9].IsLoaded) {
            Main.Assets.Request<Texture2D>(TextureAssets.Underworld[9].Name);
        }

        //DrawBackgroundSimple(TextureAssets.MagicPixel.Value, 0.00003f, Color.Cyan);
        //DrawBackgroundSimple(TextureAssets.MagicPixel.Value, 0.0001f, Color.Cyan);
        DrawBackgroundSimple(TextureAssets.Underworld[9].Value, 0.00003f, Color.White);
        UpdateAndDrawRain(Main.spriteBatch, Main.gameTimeCache);
        DrawBackgroundSimple(TextureAssets.Underworld[2].Value, 0.0001f, Color.White);
    }

    public override void RenderTopTransitionStrip(SpriteBatch spriteBatch, int worldY, float opacity) {
        Main.instance.LoadBackground(290);
        Main.instance.LoadBackground(291);
        Texture2D transitionTexture = TextureAssets.Background[290].Value;
        Texture2D fillTexture = TextureAssets.Background[291].Value;
        DrawStrip(worldY, transitionTexture, opacity, SpriteEffects.FlipVertically);
    }

    public override void RenderBottomTransitionStrip(SpriteBatch spriteBatch, int worldY, float opacity) {
        Main.instance.LoadBackground(292);
        Main.instance.LoadBackground(293);
        DrawStrip(worldY - 6 * UndergroundBackgroundSystem.TransitionStrips - 2, TextureAssets.Background[292].Value, opacity, SpriteEffects.None);
        DrawStrip(worldY - 1, TextureAssets.Background[293].Value, opacity, SpriteEffects.None);
        DrawStrip(worldY, TextureAssets.Background[292].Value, opacity, SpriteEffects.FlipVertically);
    }

    #region Background Rain 
    private class RainDropletParticle : IPoolable {
        public Vector2 ScreenPosition;

        public bool Resting { get; set; }
    }

    private List<RainDropletParticle> _droplets = new();
    private double _nextRainDroplet;

    private void UpdateAndDrawRain(SpriteBatch spriteBatch, GameTime gameTime) {
        int timeScale = Helper.GetTimeScale();
        double timeElapsed = gameTime.ElapsedGameTime.TotalMilliseconds * timeScale;
        _nextRainDroplet += timeElapsed;

        Texture2D rainTexture = AequusTextures.BackgroundRainDroplets;
        Rectangle frame = rainTexture.Frame(horizontalFrames: 3, frameX: 2);
        Vector2 velocity = new Vector2(Main.windSpeedCurrent * 0.01f, 1f) * (float)timeElapsed * 0.9f;
        float rotation = velocity.ToRotation() - MathHelper.PiOver2;
        Vector2 origin = new Vector2(frame.Width / 2f, frame.Height - 4f);
        Color drawColor = Color.Cyan * (0.7f - Math.Clamp(_droplets.Count / 30f, 0f, 0.5f));
        Vector2 scale = new Vector2(1.5f, 1.5f + Math.Clamp(_droplets.Count / 40f, 0f, 1f));

        if (timeScale != 0) {
            for (int i = 0; i < _droplets.Count; i++) {
                RainDropletParticle d = _droplets[i];
                d.ScreenPosition += velocity;
                if (d.ScreenPosition.Y > Main.screenHeight + 200f) {
                    d.Rest();
                    _droplets.RemoveAt(i);
                    i--;
                }
                else {
                    spriteBatch.Draw(rainTexture, d.ScreenPosition, frame, drawColor, rotation, origin, scale, SpriteEffects.None, 0f);
                }
            }

            float spawnThreshold = Math.Clamp(1f - 0.99f * Main.maxRaining, 0.01f, 1f) * 100f;
            _nextRainDroplet = Math.Min(_nextRainDroplet, spawnThreshold * 20f);
            for (; _nextRainDroplet > spawnThreshold; _nextRainDroplet -= Main.rand.NextFloat(spawnThreshold, spawnThreshold * 2f)) {
                RainDropletParticle next = InstancePool<RainDropletParticle>.Get();
                next.ScreenPosition.Y = -100f;
                next.ScreenPosition.X = Main.rand.NextFloat(-200f, Main.screenWidth + 200f);
                _droplets.Add(next);
            }
        }
        else {
            for (int i = 0; i < _droplets.Count; i++) {
                spriteBatch.Draw(rainTexture, _droplets[i].ScreenPosition, frame, drawColor, rotation, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }
    #endregion
}