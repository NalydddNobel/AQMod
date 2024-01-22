using Aequus.Core.Graphics;
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

        DrawBackgroundSimple(TextureAssets.Underworld[9].Value, 0.00003f, Color.Cyan);
        DrawBackgroundSimple(TextureAssets.Underworld[2].Value, 0.0001f, Color.Cyan);
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
}