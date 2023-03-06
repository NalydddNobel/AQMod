using Aequus.Common.Recipes;
using Aequus.Items.Accessories.Utility;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Offense
{
    [AutoloadEquip(EquipType.HandsOn)]
    public class FoolsGoldRing : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 10);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().accFoolsGoldRing++;
        }

        public override void AddRecipes()
        {
            AequusRecipes.CreateShimmerTransmutation(Type, ModContent.ItemType<ForgedCard>());
        }
    }
}