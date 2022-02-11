using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.HookUpgrades
{
    public class DemoniteBarb : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.damage = 25;
            item.accessory = true;
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(gold: 1);
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
            r.AddIngredient(ModContent.ItemType<MetalBarb>());
            r.AddIngredient(ItemID.DemoniteBar, 8);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}