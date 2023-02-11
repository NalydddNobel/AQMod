using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Bait
{
    public class XenonBait : ModItem
    {
        //public override void SetStaticDefaults()
        //{
        //    SacrificeTotal = 5;
        //}

        public override void SetDefaults()
        {
            Item.width = 6;
            Item.height = 6;
            Item.bait = 25;
            Item.maxStack = 9999;
            Item.consumable = false;
            Item.rare = ItemRarityID.Green;
        }

        public override bool CanPickup(Player player)
        {
            return !player.ItemSpace(Item).ItemIsGoingToVoidVault;
        }

        public override bool? CanConsumeBait(Player player)
        {
            return false;
        }
    }
}