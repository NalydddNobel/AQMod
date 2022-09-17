using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    [AutoloadEquip(EquipType.Waist)]
    public class HighSteaks : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(16, 16);
            Item.rare = ItemRarityID.Green;
            Item.value = ItemDefaults.BloodMimicItemValue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aequus = player.Aequus();
            aequus.accBloodDiceDamage = Math.Max(aequus.accBloodDiceDamage, 0.25f) + 0.25f;
            if (aequus.accBloodDiceMoney > 0)
            {
                aequus.accBloodDiceMoney = Math.Max(aequus.accBloodDiceMoney / 2, 1);
            }
            else
            {
                aequus.accBloodDiceMoney = Item.buyPrice(silver: 25);
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (var t in tooltips)
            {
                if (t.Mod == "Terraria" && t.Name.StartsWith("Tooltip"))
                {
                    t.Text = AequusHelpers.FormatWith(t.Text, new
                    {
                        Color = Colors.AlphaDarken(AequusTooltips.ItemDrawbackTooltip).Hex3(),
                        CoinColor = Colors.AlphaDarken(Colors.CoinSilver).Hex3(),
                    });
                }
            }
        }
    }
}