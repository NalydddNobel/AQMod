using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.Shoes
{
    public class MoonShoes : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Green;
            item.value = Item.sellPrice(gold: 3);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!player.controlDown)
            {
                if (player.controlUp)
                {
                    player.gravity *= 0.25f;
                }
                else
                {
                    player.gravity *= 0.5f;
                }
            }
            player.noFallDmg = true;
        }
    }
}