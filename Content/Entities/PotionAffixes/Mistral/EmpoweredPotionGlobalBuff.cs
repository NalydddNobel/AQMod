using Aequus.Common.Utilities;
using Aequus.Content.ItemPrefixes.Potions;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Aequus.Content.Entities.PotionAffixes.Mistral;

public class EmpoweredPotionGlobalBuff : GlobalBuff {
    public Dictionary<int, Action<Player>> CustomAction = [];
    public Dictionary<int, LocalizedText> CustomTooltip = [];

    public override void Update(int type, Player player, ref int buffIndex) {
        if (!player.TryGetModPlayer(out EmpoweredPotionPlayer potionPlayer)) {
            return;
        }

        // We check the buffType array to make sure the buff wasn't deleted already.
        // If it was, we really don't want to risk deleting another buff, so just don't run the empowered update code
        if (type == potionPlayer.empowered && player.buffType.IndexInRange(buffIndex) && player.buffType[buffIndex] == type) {
            if (CustomAction.TryGetValue(type, out var buffOverride)) {
                buffOverride(player);
            }
            else if (type >= BuffID.Count) {
                // Run the buff code again.
                BuffLoader.GetBuff(type).Update(player, ref buffIndex);
            }
            // Vanilla buffs should all be in the "Override" list
        }
    }

    public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare) {
        if (!Main.LocalPlayer.TryGetModPlayer(out EmpoweredPotionPlayer potionPlayer)) {
            return;
        }

        if (potionPlayer.empowered == type) {
            buffName = $"{Instance<EmpoweredPrefix>().DisplayName} {buffName}";
            if (CustomTooltip.TryGetValue(type, out LocalizedText tooltip)) {
                tip += "\n" + tooltip.Value;
            }
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, int type, int buffIndex, ref BuffDrawParams drawParams) {
        if (!Main.LocalPlayer.TryGetModPlayer(out EmpoweredPotionPlayer potionPlayer) || potionPlayer.empowered != type) {
            return true;
        }

        DrawHelper.SpriteBatchCache.InheritFrom(spriteBatch);
        spriteBatch.Begin_UI(immediate: true);

        DrawData draw = new DrawData(drawParams.Texture, drawParams.Position, drawParams.Texture.Bounds, drawParams.DrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

        DrawHelper.ColorOnlyShader.Apply(null, draw);

        for (int i = 0; i < 4; i++) {
            float f = i * MathHelper.PiOver2;
            Color color = Color.Lerp(Color.Green, Color.Turquoise, Helper.Wave(Main.GlobalTimeWrappedHourly * 15f + f, 0f, 1f)) with { A = 0 };
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