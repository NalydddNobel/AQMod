using Aequus.Old.Content.Items.Potions.Prefixes.BoundedPotions;
using Aequus.Old.Content.Items.Potions.Prefixes.EmpoweredPotions;
using System.Collections.Generic;
using Terraria.DataStructures;

namespace Aequus.Old.Content.Items.Potions.Prefixes;

public class PotionPrefixGlobalBuff : GlobalBuff {
    public override void Update(int type, Player player, ref int buffIndex) {
        if (!player.TryGetModPlayer(out PotionsPlayer potionPlayer)) {
            return;
        }

        // We check the buffType array to make sure the buff wasn't deleted already.
        // If it was, we really don't want to risk deleting another buff, so just don't run the empowered update code
        if (type == potionPlayer.empoweredPotionId && player.buffType.IndexInRange(buffIndex) && player.buffType[buffIndex] == type) {
            if (EmpoweredBuffs.Override.TryGetValue(type, out var buffOverride) && buffOverride.CustomAction != null) {
                buffOverride.CustomAction(player);
            }
            else if (type >= BuffID.Count) {
                // Run the buff code again.
                BuffLoader.GetBuff(type).Update(player, ref buffIndex);
            }
            // Vanilla buffs should all be in the "Override" list
        }
    }

    public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare) {
        if (!Main.LocalPlayer.TryGetModPlayer(out PotionsPlayer potionPlayer)) {
            return;
        }

        string prefix = "";
        foreach (var s in GetPrefixes(type, Main.LocalPlayer, potionPlayer)) {
            if (!string.IsNullOrEmpty(prefix)) {
                prefix += ", ";
            }

            prefix += s;
        }

        if (!string.IsNullOrEmpty(prefix)) {
            buffName = prefix + " " + buffName;
        }

        if (potionPlayer.empoweredPotionId == type && EmpoweredBuffs.Override.TryGetValue(type, out var buffOverride) && buffOverride.Tooltip != null) {
            tip += "\n" + buffOverride.Tooltip.Value;
        }
    }

    private IEnumerable<string> GetPrefixes(int type, Player player, PotionsPlayer potionPlayer) {
        if (potionPlayer.BoundedPotionIds.Contains(type)) {
            yield return ModContent.GetInstance<BoundedPrefix>().DisplayName.Value;
        }
        if (potionPlayer.empoweredPotionId == type) {
            yield return ModContent.GetInstance<EmpoweredPrefix>().DisplayName.Value;
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, int type, int buffIndex, ref BuffDrawParams drawParams) {
        if (Main.LocalPlayer.TryGetModPlayer(out PotionsPlayer potionPlayer)) {
            bool drawBounded = potionPlayer.BoundedPotionIds.Contains(type);
            bool drawEmpowered = potionPlayer.empoweredPotionId == type;

            if (!drawBounded && !drawEmpowered) {
                return true;
            }

            DrawHelper.SpriteBatchCache.InheritFrom(spriteBatch);
            spriteBatch.BeginUI(immediate: true);

            DrawData draw = new DrawData(drawParams.Texture, drawParams.Position, drawParams.Texture.Bounds, drawParams.DrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

            if (drawBounded) {
                DrawHelper.ColorOnlyShader.Apply(null, draw);

                for (int i = 0; i < 4; i++) {
                    float f = i * MathHelper.PiOver2;
                    Color color = Color.Lerp(Color.BlueViolet, Color.Violet, Helper.Oscillate(Main.GlobalTimeWrappedHourly * 10f + f, 0f, 1f)) with { A = 0 };
                    float alphaMultiplier = drawParams.DrawColor.A / 255f;
                    (draw with {
                        color = color.MultiplyRGBA(drawParams.DrawColor) * alphaMultiplier,
                        position = draw.position + f.ToRotationVector2() * 2f
                    }).Draw(spriteBatch);
                }
            }

            if (drawEmpowered) {
                DrawHelper.ColorOnlyShader.Apply(null, draw);

                for (int i = 0; i < 4; i++) {
                    float f = i * MathHelper.PiOver2;
                    Color color = Color.Lerp(Color.Green, Color.Turquoise, Helper.Oscillate(Main.GlobalTimeWrappedHourly * 15f + f, 0f, 1f)) with { A = 0 };
                    float alphaMultiplier = drawParams.DrawColor.A / 255f;
                    (draw with {
                        color = color.MultiplyRGBA(drawParams.DrawColor) * alphaMultiplier,
                        position = draw.position + f.ToRotationVector2() * 2f
                    }).Draw(spriteBatch);
                }
            }

            spriteBatch.End();
            DrawHelper.SpriteBatchCache.Begin(spriteBatch, SpriteSortMode.Deferred, transform: Main.UIScaleMatrix);
        }
        return true;
    }
}
