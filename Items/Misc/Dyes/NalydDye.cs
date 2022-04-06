using Aequus.Common.Players;
using Aequus.Effects.Armor;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Dyes
{
    public sealed class NalydDye : DyeItemBase
    {
        public override string Pass => "UnchainedPass";
        public override ArmorShaderData CreateShaderData()
        {
            return new ArmorShaderDataDynamicColor(Effect, Pass, (e, d) =>
            {
                if (e is Player p)
                {
                    return p.GetModPlayer<DrawEffectsPlayer>().NalydGradientPersonal.GetColor(Main.GlobalTimeWrappedHourly);
                }
                return DrawEffectsPlayer.NalydGradient.GetColor(Main.GlobalTimeWrappedHourly);
            });
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips[i].Mod == "Terraria" && tooltips[i].Name == "ItemName")
                {
                    tooltips[i].OverrideColor = Main.LocalPlayer.GetModPlayer<DrawEffectsPlayer>().NalydGradientPersonal.GetColor(Main.GlobalTimeWrappedHourly);
                    return;
                }
            }
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                ItemTooltipsHelper.DrawDevTooltip(line);
                return false;
            }
            return true;
        }
    }
}