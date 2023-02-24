using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.ExporterNPC.Shop
{
    public class GrandReward : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 15);
            Item.canBePlacedInVanityRegardlessOfConditions = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().dropRerolls += 1f;
            player.Aequus().accGrandReward = true;
        }
    }
}