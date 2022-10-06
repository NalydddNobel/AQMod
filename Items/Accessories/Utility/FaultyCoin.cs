using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Utility
{
    public class FaultyCoin : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 10);
        }

        public override void UpdateEquip(Player player)
        {
            player.Aequus().scamChance += 0.1f;
        }
    }
}