using Terraria.DataStructures;

namespace Aequus.Old.Content.Potions.Prefixes;

public class PotionPrefixGlobalBuff : GlobalBuff {
    public override bool PreDraw(SpriteBatch spriteBatch, int type, int buffIndex, ref BuffDrawParams drawParams) {
        if (Main.LocalPlayer.TryGetModPlayer(out PotionsPlayer boundedPotions) && boundedPotions.BoundedPotionIds.Contains(type)) {
            spriteBatch.End();
            DrawHelper.SpriteBatchCache.Inherit(spriteBatch);
            spriteBatch.BeginUI(immediate: true);

            DrawData draw = new DrawData(drawParams.Texture, drawParams.Position, drawParams.Texture.Bounds, drawParams.DrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            DrawHelper.ColorOnlyShader.Apply(null, draw);

            for (int i = 0; i < 4; i++) {
                float f = i * MathHelper.PiOver2;
                Color color = Color.Lerp(Color.BlueViolet, Color.Violet, Helper.Oscillate(Main.GlobalTimeWrappedHourly * 10f + f, 0f, 1f)) with { A = 0 };
                float alphaMultiplier = drawParams.DrawColor.A / 255f;
                (draw with {
                    color = Utils.MultiplyRGBA(color, drawParams.DrawColor) * alphaMultiplier * 0.8f,
                    position = draw.position + f.ToRotationVector2() * 2f
                }).Draw(spriteBatch);
            }

            spriteBatch.End();
            DrawHelper.SpriteBatchCache.Begin(spriteBatch);
        }
        return true;
    }
}
