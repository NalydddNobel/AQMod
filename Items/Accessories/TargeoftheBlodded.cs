using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class TargeoftheBlodded : ModItem // I know this is mispelt...
    {
        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.rare = ItemRarityID.Green;
            item.value = Item.sellPrice(gold: 1);
            item.defense = 2;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.armorPenetration += 2;
            player.spikedBoots += 1;
        }

        //public override void AddRecipes()
        //{
        //    var r = new ModRecipe(mod);
        //    r.AddRecipeGroup("Wood", 10);
        //    r.AddIngredient(ItemID.SharkToothNecklace);
        //    r.AddIngredient(ItemID.Shackle);
        //    r.AddIngredient(ItemID.ClimbingClaws);
        //    r.AddTile(TileID.TinkerersWorkbench);
        //    r.SetResult(this);
        //    r.AddRecipe();
        //}
    }
}