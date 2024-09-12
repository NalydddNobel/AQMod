using Aequus.Content.Tiles.Meadows;

namespace Aequus.Content.Items.Tools.Hammers;

public class MeadowHammer : ModItem {
    public override void SetDefaults() {
        // Duplicate of Ash Wood Hammer
        Item.autoReuse = true;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 30;
        Item.useTime = 20;
        Item.hammer = 45;
        Item.width = 24;
        Item.height = 28;
        Item.damage = 9;
        Item.knockBack = 5.5f;
        Item.scale = 1f;
        Item.UseSound = SoundID.Item1;
        Item.value = Item.sellPrice(copper: 10);
        Item.DamageType = DamageClass.Melee;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ModContent.GetInstance<MeadowWood>().Item.Type, 8)
            .AddTile(TileID.WorkBenches)
            .Register();
    }
}
