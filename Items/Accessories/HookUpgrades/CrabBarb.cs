using AQMod.Buffs;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.HookUpgrades
{
    public class CrabBarb : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.damage = 15;
            item.accessory = true;
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(silver: 40);
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            aQPlayer.hookDamage += item.damage;
            aQPlayer.hookDebuffs.Add(new BuffData(BuffID.Poisoned, 120));
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips[i].mod == "Terraria")
                {
                    if (tooltips[i].Name == "CritChance")
                    {
                        tooltips.RemoveAt(i);
                        i--;
                    }
                    else if (tooltips[i].Name == "Knockback")
                    {
                        tooltips.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
    }
}