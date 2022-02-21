using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Armor.PassiveSummon
{
    [AutoloadEquip(EquipType.Head)]
    public class FlowerCrown : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.summon = true;
            item.damage = 10;
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(silver: 10);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips[i].mod == "Terraria")
                {
                    if (tooltips[i].Name == "Knockback")
                    {
                        tooltips.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public override void UpdateEquip(Player player)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            aQPlayer.helmetFlowerCrown = true;
            aQPlayer.passiveSummonDelay = 120;
        }

        public override void DrawHair(ref bool drawHair, ref bool drawAltHair)
        {
            drawAltHair = true;
            drawHair = true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Daybloom, 3);
            r.AddIngredient(ItemID.Mushroom);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}