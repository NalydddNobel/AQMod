using Terraria;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories
{
    public class EquivalenceMachine : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.accessory = true;
            item.rare = AQItem.Rarities.OmegaStariteRare;
            item.value = Item.buyPrice(gold: 80);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AQPlayer>().antiGravityItems = true;
        }
    }
}