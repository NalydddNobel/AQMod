using Aequus.Buffs.Misc.Empowered;
using Aequus.Common.ContentTemplates;
using Aequus.Common.Effects;
using Aequus.Common.Utilities;
using Aequus.Content.ItemPrefixes.Potions;
using System;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Aequus.Content.Items.Potions.Modifiers.Empowered;

internal class InstancedLegacyEmpoweredBuff(UnifiedBuffPotion Parent) : InstancedPotionBuff(Parent, "Empowered"), ILegacyEmpoweredBuff {
    public override string Name => ALanguage.GetText("Buffs.ModifierFormats.Empowered").FormatWith(Parent.Name);
    public override LocalizedText Description => Parent.EmpoweredBuffDescription;

    int ILegacyEmpoweredBuff.OriginalBuffType => Parent.Buff.Type;

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        EmpoweredPrefix.ItemToEmpoweredBuff[Parent.Type] = Type;
    }

    public override void Update(Player player, ref int buffIndex) {
        // Simply update twice.
        base.Update(player, ref buffIndex);
        base.Update(player, ref buffIndex);

        for (int i = buffIndex + 1; i < Player.MaxBuffs; i++) {
            if (player.buffType[i] == Parent.Buff.Type && player.buffTime[i] > 1) {
                player.buffType[buffIndex] = player.buffType[i];
                player.buffTime[buffIndex] = Math.Max(player.buffTime[buffIndex], player.buffTime[i]);
                player.DelBuff(i);
                break;
            }
        }
        for (int i = buffIndex - 1; i >= 0; i--) {
            if (player.buffType[i] == Parent.Buff.Type && player.buffTime[i] > 1) {
                player.buffType[i] = player.buffType[buffIndex];
                player.buffTime[i] = Math.Max(player.buffTime[buffIndex], player.buffTime[i]);
                player.DelBuff(buffIndex);
                buffIndex--;
                break;
            }
            if (player.buffType[i] >= BuffID.Count && BuffLoader.GetBuff(player.buffType[i]) is EmpoweredBuffBase empoweredBuff) {
                player.buffType[i] = empoweredBuff.OriginalBuffType;
                break;
            }
        }
    }

    public override void PostDraw(SpriteBatch spriteBatch, int buffIndex, BuffDrawParams drawParams) {
        spriteBatch.EndCached();
        spriteBatch.Begin(SpriteSortMode.Immediate, null, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);

        float time = Main.GlobalTimeWrappedHourly * 2.5f;
        float scale = 1f + Math.Max(MathF.Sin(time * MathHelper.PiOver2) * 0.2f, 0f);
        Vector2 origin = drawParams.Texture.Size() / 2f;
        Vector2 drawPosition = drawParams.Position - origin * (scale - 1f);
        var dd = new DrawData(drawParams.Texture, drawPosition, drawParams.Texture.Bounds, drawParams.DrawColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
        LegacyEffects.VerticalGradient.ShaderData.UseColor(Color.Lerp(Color.Transparent, Color.Green, Helper.Wave(time + MathHelper.Pi, 0f, 0.9f)) * Main.cursorAlpha);
        LegacyEffects.VerticalGradient.ShaderData.UseSecondaryColor(Color.Lerp(Color.Transparent, Color.Teal, Helper.Wave(time, 0f, 0.9f)) * Main.cursorAlpha);
        LegacyEffects.VerticalGradient.ShaderData.Apply(dd);

        dd.color.A = 0;
        dd.Draw(spriteBatch);

        spriteBatch.End();
        spriteBatch.BeginCached(SpriteSortMode.Deferred, Main.UIScaleMatrix);
    }
}
