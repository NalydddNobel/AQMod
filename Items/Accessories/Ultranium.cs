using AQMod.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories
{
    public class Ultranium : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 24;
            item.accessory = true;
            item.rare = ItemRarityID.Pink;
            item.value = Item.buyPrice(gold: 10);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            float r = Main.DiscoR / 255f;
            return new Color(r * 0.1f + 0.9f, r * 0.1f + Main.DiscoG / 255f * 0.5f + 0.4f, r * 0.1f + Main.DiscoB / 255f * 0.5f + 0.4f, 0f);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.meleeCrit += 10;
            player.GetModPlayer<AQPlayer>().bossChanneling = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (DateTime.Now.Month == 7 && DateTime.Now.Day == 21)
            {
                tooltips.Add(new TooltipLine(mod, "Ultranium", "You will be missed, Ultranium."));
            }
        }
    }
}