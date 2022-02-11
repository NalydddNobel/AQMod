using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.HookUpgrades
{
    public class MetalBarb : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.damage = 10;
            item.value = Item.sellPrice(silver: 20);
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AQPlayer>().hookDamage += item.damage;
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

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Chain, 3);
            r.AddIngredient(ItemID.Hook);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}