using Aequus.Items.Materials.Festive;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class Headless : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;
            ArmorIDs.Head.Sets.PreventBeardDraw[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToHeadgear(16, 16, Item.headSlot);
            Item.rare = ItemRarityID.Yellow;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                 .AddIngredient(ItemID.FamiliarWig)
                 .AddIngredient<HorrificEnergy>(10)
                 .AddTile(TileID.Loom)
                 .Register();
        }
    }
}
