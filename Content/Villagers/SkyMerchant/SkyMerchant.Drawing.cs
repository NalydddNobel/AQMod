using ReLogic.Content;
using System;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Map;
using Terraria.UI;

namespace Aequus.Content.Villagers.SkyMerchant;

public partial class SkyMerchant {
    public readonly struct TextureDrawSet(Asset<Texture2D> balloon, Asset<Texture2D> basket, Asset<Texture2D> aiming, Asset<Texture2D> crossbowArm, Asset<Texture2D> head) {
        public readonly Asset<Texture2D> Balloon = balloon;
        public readonly Asset<Texture2D> Basket = basket;
        public readonly Asset<Texture2D> Aiming = aiming;
        public readonly Asset<Texture2D> CrossbowArm = crossbowArm;
        public readonly Asset<Texture2D> Head = head;

        private readonly bool _valid = true;

        public bool Valid => _valid;
    }

    public static TextureDrawSet DefaultDrawSet;
    public static TextureDrawSet ShimmerDrawSet;

    public static readonly Vector2 BalloonOffset = new Vector2(0f, -96f);

    public TextureDrawSet drawSet;

    private void LoadDrawSets() {
        DefaultDrawSet = new(AequusTextures.Balloon, AequusTextures.Basket, AequusTextures.SkyMerchant_Aiming, AequusTextures.SkyMerchant_CrossbowArm, AequusTextures.SkyMerchant_CustomHead);
        ShimmerDrawSet = new(AequusTextures.Balloon_Shimmer, AequusTextures.Basket_Shimmer, AequusTextures.SkyMerchant_Shimmer_Aiming, AequusTextures.SkyMerchant_Shimmer_CrossbowArm, AequusTextures.SkyMerchant_Shimmer_CustomHead);
    }

    private void DrawAnchored(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Vector2 offset, Rectangle? frame, Color color, float rotation, Vector2 origin, float scale, SpriteEffects spriteEffects, float layerDepth) {
        spriteBatch.Draw(texture, position + offset.RotatedBy(NPC.rotation) * NPC.scale, frame, color, rotation, origin, scale, spriteEffects, 0f);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        if (NPC.IsABestiaryIconDummy) {
            drawSet = !NPC.IsShimmerVariant ? DefaultDrawSet : ShimmerDrawSet;
            state = MovementState.Ballooning;
            target = -1;
            NPC.scale = 1f;
            NPC.frame.Y = NPC.frame.Height;
        }
        if (NPC.ai[0] == 25f || !drawSet.Valid) {
            return true;
        }
        var drawCoordinates = NPC.Center - screenPos + new Vector2(0f, DrawOffsetY + NPC.gfxOffY + 8f);
        var texture = TextureAssets.Npc[Type].Value;
        float opacity = NPC.Opacity;
        if (state == MovementState.Ballooning) {
            DrawAnchored(spriteBatch, drawSet.Balloon.Value, drawCoordinates, BalloonOffset, null, drawColor * opacity * balloonOpacity, NPC.rotation, drawSet.Balloon.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);
            var npcColor = NPC.GetNPCColorTintedByBuffs(drawColor) * opacity * (1f - NPC.shimmerTransparency);
            var drawOffset = new Vector2(0f, -18f);
            int direction = NPC.spriteDirection;
            float armRotation = 0f;
            if (target == -1) {
                DrawAnchored(spriteBatch, texture, drawCoordinates, drawOffset, NPC.frame, npcColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            }
            else {
                armRotation = (NPC.Center - Main.npc[target].Center).ToRotation();
                direction = Math.Abs(armRotation) > MathHelper.PiOver2 ? 1 : -1;
                DrawAnchored(spriteBatch, drawSet.Aiming.Value, drawCoordinates, drawOffset + new Vector2(0f, -4f), null, npcColor, NPC.rotation, drawSet.Aiming.Size() / 2f, NPC.scale, direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(drawSet.Basket.Value, drawCoordinates, null, drawColor * opacity * balloonOpacity, NPC.rotation, drawSet.Basket.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);
            if (target != -1) {
                DrawAnchored(spriteBatch, drawSet.CrossbowArm.Value, drawCoordinates, drawOffset + new Vector2(-6f * direction, 2f), null, npcColor, armRotation, new Vector2(48f, 10f), NPC.scale, direction == 1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0f);
            }
            return false;
        }
        return true;
    }

    public void DrawMapHead(ref MapOverlayDrawContext context, ref string text) {
        float opacity = 1f - NPC.Distance(Main.LocalPlayer.Center) / 900f;
        if (opacity < 0f) {
            return;
        }

        if (context.Draw(drawSet.Head.Value, (NPC.Center.Floor() / 16f), Color.White * opacity, new SpriteFrame(1, 2, 0, (byte)(NPC.spriteDirection == -1 ? 0 : 1)), 1f, 1f, Alignment.Center).IsMouseOver && opacity > 0.33f) {
            text = NPC.FullName;
        }
    }
}
