using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.CarpenterNPC.Shop
{
    public class LavaproofMitten : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 5);
            Item.canBePlacedInVanityRegardlessOfConditions = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.Aequus().accLavaPlace = true;
        }
    }
}