using AQMod.Common.Config;
using AQMod.Localization;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Vanities.Dyes
{
    public class EnchantedDye : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 99;
            item.value = Item.sellPrice(gold: 1, silver: 50);
            item.rare = ItemRarityID.Pink;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!AQConfigClient.Instance.ScrollShader)
            {
                tooltips.Add(new TooltipLine(mod, "ScrollShader", AQText.ModText("Common.NoScrollShader").Value) { overrideColor = new Color(128, 128, 128, 255) });
            }
        }
    }
}