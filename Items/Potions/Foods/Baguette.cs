using AQMod.Buffs;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Potions.Foods
{
    public class Baguette : ModItem, IDedicatedItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 17;
            item.useTime = 17;
            item.UseSound = SoundID.Item2;
            item.maxStack = 999;
            item.consumable = true;
            item.rare = AQItem.RarityDedicatedItem;
            item.value = Item.buyPrice(silver: 80);
            item.buffType = ModContent.BuffType<BaguetteBuff>();
            item.buffTime = 216000;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips[i].mod == "Terraria" && tooltips[i].Name == "ItemName")
                {
                    tooltips[i].overrideColor = Main.LocalPlayer.FX().BaguetteGradient.GetColor(Main.GlobalTime);
                    return;
                }
            }
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.mod == "Terraria" && line.Name == "ItemName")
            {
                AQItem.DrawString_Developer(line);
                return false;
            }
            return true;
        }

        public override bool CanBurnInLava()
        {
            return true;
        }

        Color IDedicatedItem.DedicatedColoring => new Color(187, 142, 42, 255);
    }
}