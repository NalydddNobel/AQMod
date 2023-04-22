using Aequus.Items.Accessories.CrownOfBlood;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Utility
{
    public class LavaproofMitten : ModItem
    {
        public override void SetStaticDefaults() {
            CrownOfBloodItem.NoBoost.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 5);
            Item.hasVanityEffects = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.Aequus().accLavaPlace = true;
        }
    }
}