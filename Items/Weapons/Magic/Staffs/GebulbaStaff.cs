using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic.Staffs
{
    public class GebulbaStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 16;
            item.magic = true;
            item.useTime = 42;
            item.useAnimation = 42;
            item.width = 40;
            item.height = 40;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = ItemRarityID.Blue;
            item.shoot = ModContent.ProjectileType<Projectiles.GelBulb>();
            item.shootSpeed = 10f;
            item.mana = 5;
            item.autoReuse = true;
            item.UseSound = SoundID.Item85;
            item.value = Item.sellPrice(silver: 20);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Gel, 20);
            recipe.AddRecipeGroup("IronBar", 4);
            recipe.AddTile(TileID.Solidifier);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}