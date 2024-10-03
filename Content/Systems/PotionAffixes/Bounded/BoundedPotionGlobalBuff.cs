using Aequus.Common.Utilities;
using Terraria.DataStructures;

namespace Aequus.Content.Systems.PotionAffixes.Bounded;

public class BoundedPotionGlobalBuff : GlobalBuff {
    public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare) {
        if (!Main.LocalPlayer.TryGetModPlayer(out BoundedPotionPlayer potionPlayer)) {
            return;
        }

        if (potionPlayer.bounded.Contains(type)) {
            buffName = $"{Instance<BoundedPrefix>().DisplayName} {buffName}";
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, int type, int buffIndex, ref BuffDrawParams drawParams) {
        if (!Main.LocalPlayer.TryGetModPlayer(out BoundedPotionPlayer potionPlayer) || !potionPlayer.bounded.Contains(type)) {
            return true;
        }

        DrawHelper.SpriteBatchCache.InheritFrom(spriteBatch);
        spriteBatch.Begin_UI(immediate: true);

        DrawData draw = new DrawData(drawParams.Texture, drawParams.Position, drawParams.Texture.Bounds, drawParams.DrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

        DrawHelper.ColorOnlyShader.Apply(null, draw);

        for (int i = 0; i < 4; i++) {
            float f = i * MathHelper.PiOver2;
            Color color = Color.Lerp(Color.BlueViolet, Color.Violet, Helper.Wave(Main.GlobalTimeWrappedHourly * 10f + f, 0f, 1f)) with { A = 0 };
            float alphaMultiplier = drawParams.DrawColor.A / 255f;
            (draw with {
                color = color.MultiplyRGBA(drawParams.DrawColor) * alphaMultiplier,
                position = draw.position + f.ToRotationVector2() * 2f
            }).Draw(spriteBatch);
        }

        spriteBatch.End();
        DrawHelper.SpriteBatchCache.Begin(spriteBatch, SpriteSortMode.Deferred, transform: Main.UIScaleMatrix);
        return true;
    }
}