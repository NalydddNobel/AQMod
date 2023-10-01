using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;

namespace Aequus.Content.TownNPCs.SkyMerchant;

public partial class SkyMerchant {
    public readonly struct TextureDrawSet {
        public readonly Asset<Texture2D> Balloon;
        public readonly Asset<Texture2D> Basket;
        public readonly Asset<Texture2D> Aiming;
        public readonly Asset<Texture2D> CrossbowArm;

        private readonly bool _valid = false;

        public bool Valid => _valid;

        public TextureDrawSet(Asset<Texture2D> balloon, Asset<Texture2D> basket, Asset<Texture2D> aiming, Asset<Texture2D> crossbowArm) {
            Balloon = balloon;
            Basket = basket;
            Aiming = aiming;
            CrossbowArm = crossbowArm;
            _valid = true;
        }
    }

    public static TextureDrawSet DefaultDrawSet;
    public static TextureDrawSet ShimmerDrawSet;

    public TextureDrawSet drawSet;

    private void LoadDrawSets() {
        DefaultDrawSet = new(AequusTextures.Balloon_SkyMerchant, AequusTextures.Basket_SkyMerchant, AequusTextures.SkyMerchant_Aiming_SkyMerchant, AequusTextures.SkyMerchant_CrossbowArm_SkyMerchant);
        ShimmerDrawSet = new(AequusTextures.Balloon_Shimmer, AequusTextures.Basket_Shimmer, AequusTextures.SkyMerchant_Aiming_Shimmer, AequusTextures.SkyMerchant_CrossbowArm_Shimmer);
    }

    private void UnloadDrawSets() {
        DefaultDrawSet = default;
        ShimmerDrawSet = default;
    }

    private void DrawAnchored(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Vector2 offset, Rectangle? frame, Color color, float rotation, Vector2 origin, float scale, SpriteEffects spriteEffects, float layerDepth) {
        spriteBatch.Draw(texture, position + offset.RotatedBy(NPC.rotation) * NPC.scale, frame, color, rotation, origin, scale, spriteEffects, 0f);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        if (NPC.ai[0] == 25f || !drawSet.Valid) {
            return true;
        }
        var drawCoordinates = NPC.Center - screenPos + new Vector2(0f, DrawOffsetY);
        var texture = TextureAssets.Npc[Type].Value;
        float opacity = NPC.Opacity;
        if (state == MovementState.Ballooning) {
            DrawAnchored(spriteBatch, drawSet.Balloon.Value, drawCoordinates, new Vector2(0f, -96f), null, drawColor * opacity * balloonOpacity, NPC.rotation, AequusTextures.Balloon_SkyMerchant.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);
            if (target != -1) {
                var colour = NPC.GetNPCColorTintedByBuffs(drawColor) * opacity * (1f - NPC.shimmerTransparency);
                var spriteDirection = SpriteEffects.None;
                Vector2 drawOffset = new(0f, -14f);
                float armRotation = (Main.npc[target].Center - NPC.Center).ToRotation();
                DrawAnchored(spriteBatch, drawSet.Aiming.Value, drawCoordinates, drawOffset, null, colour, NPC.rotation, drawSet.Aiming.Size() / 2f, NPC.scale, spriteDirection, 0f);
                DrawAnchored(spriteBatch, drawSet.CrossbowArm.Value, drawCoordinates, drawOffset, null, colour, armRotation, drawSet.CrossbowArm.Size() / 2f, NPC.scale, spriteDirection, 0f);
            }
            else {
                DrawAnchored(spriteBatch, texture, drawCoordinates, new Vector2(0f, -14f), NPC.frame, NPC.GetNPCColorTintedByBuffs(drawColor) * opacity * (1f - NPC.shimmerTransparency), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(drawSet.Basket.Value, drawCoordinates, null, drawColor * opacity * balloonOpacity, NPC.rotation, AequusTextures.Basket_SkyMerchant.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);
            
            return false;
        }
        return true;
    }
}