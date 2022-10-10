using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public class CrownOfBlood : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(14, 20);
            Item.rare = ItemDefaults.RarityDemonSiege;
            Item.value = Item.buyPrice(gold: 7, silver: 50);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().accBloodCrownSlot = 3;
        }
    }
}