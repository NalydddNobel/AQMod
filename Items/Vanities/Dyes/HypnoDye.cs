using AQMod.Common.Config;
using AQMod.Localization;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Vanities.Dyes
{
    public class HypnoDye : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 99;
            item.value = Item.sellPrice(gold: 1, silver: 50);
            item.rare = ItemRarityID.Green;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!AQConfigClient.Instance.HypnoShader)
            {
                tooltips.Add(new TooltipLine(mod, "HypnoShader", AQText.ModText("Common.NoHypnoShader").Value) { overrideColor = new Color(128, 128, 128, 255) });
            }
        }
    }
}