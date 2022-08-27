using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public class CrownOfDarkness : ModItem
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
            player.Aequus().darknessDamage += 0.1f;
        }
    }
}