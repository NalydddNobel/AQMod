using Aequus.Items.Misc.Festive;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Vanity
{
    [AutoloadEquip(EquipType.Back, EquipType.Front)]
    public class PumpkingCloak : ModItem
    {
        public static sbyte FrontID { get; private set; }
        public static sbyte BackID { get; private set; }

        public override void SetStaticDefaults()
        {
            FrontID = Item.frontSlot;
            BackID = Item.backSlot;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(20, 20);
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(silver: 20);
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            NewRecipe(ItemID.MysteriousCape);
            NewRecipe(ItemID.RedCape);
            NewRecipe(ItemID.WinterCape);
            NewRecipe(ItemID.CrimsonCloak);
            NewRecipe(ItemID.HunterCloak);
        }

        public void NewRecipe(int cape)
        {
            CreateRecipe()
                .AddIngredient(cape)
                .AddIngredient(ItemID.Pumpkin, 150)
                .AddIngredient<HalloweenEnergy>(10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
