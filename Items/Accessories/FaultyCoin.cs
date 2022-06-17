using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace Aequus.Items.Accessories
{
    public class FaultyCoin : ModItem, IUpdateBank
    {
        public float Chance => 0.1f;

        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 20);
        }

        public override void UpdateInventory(Player player)
        {
            player.Aequus().scamChance += Chance;
        }

        public void UpdateBank(Player player, AequusPlayer aequus, int slot, int bank)
        {
            aequus.scamChance += Chance;
        }
    }
}