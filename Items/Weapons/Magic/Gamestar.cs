using Aequus.Items.Materials.Energies;
using Aequus.Projectiles.Magic;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic
{
    public class Gamestar : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.width = 20;
            Item.height = 20;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemDefaults.RarityOmegaStarite;
            Item.shoot = ModContent.ProjectileType<GamestarProj>();
            Item.shootSpeed = 25f;
            Item.mana = 14;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item75.WithPitch(1f);
            Item.value = ItemDefaults.ValueOmegaStarite;
            Item.knockBack = 1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Nightfall>()
                .AddIngredient<CosmicEnergy>()
                .AddTile(TileID.Anvils)
                .TryRegisterAfter(ItemID.SpaceGun);
        }
    }
}