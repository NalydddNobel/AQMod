using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public class FaultyCoin : ModItem, Hooks.IUpdateBank
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 10);
        }

        public override void UpdateInventory(Player player)
        {
            player.Aequus().scamChance += 0.1f;
        }

        public void UpdateBank(Player player, AequusPlayer aequus, int slot, int bank)
        {
            aequus.scamChance += 0.1f;
        }
    }
}