using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public class HighSteaks : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(16, 16);
            Item.rare = ItemRarityID.Blue;
            Item.value = ItemDefaults.BloodMimicItemValue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().accBloodDiceDamage = 0.5f;
            player.Aequus().accBloodDiceMoney = Item.buyPrice(silver: 25);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (var t in tooltips)
            {
                if (t.Mod == "Terraria" && t.Name.StartsWith("Tooltip"))
                {
                    t.Text = AequusHelpers.FormatWith(t.Text, new { 
                        Color = Colors.AlphaDarken(AequusTooltips.ItemDrawbackTooltip).Hex3(),
                        CoinColor = Colors.AlphaDarken(Colors.CoinSilver).Hex3(),
                    });
                }
            }
        }
    }
}