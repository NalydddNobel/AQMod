using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons
{
    [Obsolete("Soul gem weapon experiment ended, souls gems were cut.")]
    public abstract class LegacySoulGemWeaponBase : ModItem
    {
        public const int MaxTier = 4;
        public const int MinTier = 1;

        public int OriginalTier;
        public int tier;
        protected Vector2 ammoDrawOffset = new Vector2(8f, -24f);

        public void ClearSoulFields()
        {
            tier = OriginalTier;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Insert(tooltips.GetIndex("Tooltip#"),
                new TooltipLine(Aequus.Instance, "TooltipSoulGem", AequusText.GetTextWith("ItemTooltip.Common.SoulGemCost", new { SoulTier = GetTierName(tier), })));
            if (tier != OriginalTier)
            {
                int value = tier - OriginalTier;
                tooltips.Insert(tooltips.GetIndex("PrefixAccMeleeSpeed"),
                    new TooltipLine(Aequus.Instance, "PrefixSoulGemTier", AequusText.GetText("Prefixes.SoulGemTier", $"{(value > 0f ? " + " : "")}{value}"))
                    { IsModifier = true, IsModifierBad = value < 0, });
            }
            //TooltipsGlobalItem.PercentageModifier(soulLimit, OriginalSoulLimit, "PrefixSoulLimit", tooltips, higherIsGood: true);
        }

        public static string GetTierName(int tier)
        {
            return tier >= MinTier && tier <= MaxTier ? AequusText.GetText($"SoulGemTier.{tier}") : AequusText.Unknown;
        }
    }
}