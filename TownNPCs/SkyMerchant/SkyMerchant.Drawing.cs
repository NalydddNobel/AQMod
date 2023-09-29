using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace Aequus.TownNPCs.SkyMerchant;

public partial class SkyMerchant {
    private void DrawAnchored(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Vector2 offset, Rectangle? frame, Color color, float rotation, Vector2 origin, float scale, SpriteEffects spriteEffects, float layerDepth) {
        spriteBatch.Draw(texture, position + offset.RotatedBy(NPC.rotation) * NPC.scale, frame, color, rotation, origin, scale, spriteEffects, 0f);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        if (NPC.ai[0] == 25f) {
            return true;
        }
        var drawCoordinates = NPC.Center - screenPos + new Vector2(0f, DrawOffsetY);
        bool shimmered = NPC.IsShimmerVariant;
        var texture = TextureAssets.Npc[Type].Value;
        float opacity = NPC.Opacity;
        if (state == MovementState.Ballooning) {
            DrawAnchored(spriteBatch, shimmered ? AequusTextures.Balloon_Shimmer : AequusTextures.Balloon_SkyMerchant, drawCoordinates, new Vector2(0f, -96f), null, drawColor * opacity * balloonOpacity, NPC.rotation, AequusTextures.Balloon_SkyMerchant.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);
            DrawAnchored(spriteBatch, texture, drawCoordinates, new Vector2(0f, -14f), NPC.frame, NPC.GetNPCColorTintedByBuffs(drawColor) * opacity * (1f - NPC.shimmerTransparency), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            spriteBatch.Draw(shimmered ? AequusTextures.Basket_Shimmer : AequusTextures.Basket_SkyMerchant, drawCoordinates, null, drawColor * opacity * balloonOpacity, NPC.rotation, AequusTextures.Basket_SkyMerchant.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);
            return false;
        }
        return true;
    }
}