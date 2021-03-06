using Aequus.Items.Recipes;
using Aequus.Projectiles.Ranged;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Ranged
{
    [GlowMask]
    public class Baozhu : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 55;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.width = 16;
            Item.height = 16;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemDefaults.RarityGaleStreams;
            Item.shoot = ModContent.ProjectileType<BaozhuProj>();
            Item.shootSpeed = 8.5f;
            Item.UseSound = SoundID.Item1;
            Item.value = ItemDefaults.GaleStreamsValue;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            AequusRecipes.RedSpriteRecipe(this, ItemID.MolotovCocktail, 50);
        }
    }
}