using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Items.Reforges
{
    public abstract class CooldownPrefix : AQPrefix
    {
        protected virtual float CooldownMultiplier => 1f;
        protected virtual float ComboMultiplier => 1f;

        public override bool Autoload(ref string name)
        {
            return base.Autoload(ref name) && ModContent.GetInstance<AQConfigServer>().cooldownReforges;
        }

        public override bool CanRoll(Item item) => !item.accessory && (item.modItem is ICooldown || item.modItem is ICombo);
        public override float RollChance(Item item) => 12.5f;

        public override PrefixCategory Category => PrefixCategory.AnyWeapon;

        public override void Apply(Item item)
        {
            var aQItem = item.GetGlobalItem<AQItem>();
            aQItem.cooldownMultiplier = CooldownMultiplier;
            aQItem.comboMultiplier = ComboMultiplier;
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= (CooldownMultiplier + ComboMultiplier) / 2f;
        }

        public override void ModifyTooltips(Item item, AQItem aQItem, AQPlayer aQPlayer, List<TooltipLine> tooltips)
        {
            if (CooldownMultiplier != 1f && item.modItem is ICooldown)
            {
                tooltips.Add(new TooltipLine(mod, "PrefixCooldown", (CooldownMultiplier < 1f ? "+" : "-") + Language.GetTextValue("Mods.AQMod.PrefixTooltip.Cooldown", (int)((CooldownMultiplier - 1f) * 100f).Abs())) { isModifier = true, isModifierBad = CooldownMultiplier < 1f, });
            }
            if (ComboMultiplier != 1f && item.modItem is ICombo)
            {
                tooltips.Add(new TooltipLine(mod, "PrefixCombo", (ComboMultiplier > 1f ? "+" : "") + Language.GetTextValue("Mods.AQMod.PrefixTooltip.Combo", (int)((ComboMultiplier - 1f) * 100f))) { isModifier = true, isModifierBad = ComboMultiplier < 1f, });
            }
        }
    }
}