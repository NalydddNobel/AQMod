using AQMod.Effects.Dyes;
using AQMod.Items.Weapons.Magic;
using System.Collections.Generic;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace AQMod.Items.Dyes
{
    public sealed class NalydDye : DyeItem
    {
        public override string Pass => "UnchainedPass";
        public override ArmorShaderData CreateShaderData()
        {
            return new ArmorShaderDataDynamicColor(Effect, Pass, () => Narrizuul.TextColor());
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips[i].mod == "Terraria" && tooltips[i].Name == "ItemName")
                {
                    tooltips[i].overrideColor = Narrizuul.TextColor();
                    return;
                }
            }
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.mod == "Terraria" && line.Name == "ItemName")
            {
                TooltipText.DrawNarrizuulText(line);
                return false;
            }
            return true;
        }
    }
}