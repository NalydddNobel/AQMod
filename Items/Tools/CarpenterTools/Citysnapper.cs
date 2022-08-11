using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.CarpenterTools
{
    public class Citysnapper : ModItem
    {

        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadExtraRange[Type] = 400;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 5);
            Item.useAmmo = CitysnapperClipAmmo.AmmoID;
        }

        public override bool? UseItem(Player player)
        {
            return true;
        }
    }
}