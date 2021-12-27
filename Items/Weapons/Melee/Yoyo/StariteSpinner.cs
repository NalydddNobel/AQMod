using AQMod.Items.Materials.Energies;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee.Yoyo
{
    public class StariteSpinner : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 12;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
            AQItem.CreativeMode.SingleItem(this);
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 20;
            Item.knockBack = 2.11f;
            Item.value = AQItem.Prices.GlimmerWeaponValue;
            Item.UseSound = SoundID.Item1;
            Item.rare = AQItem.Rarities.StariteWeaponRare;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shootSpeed = 10f;
            //Item.shoot = ModContent.ProjectileType<Projectiles.Melee.StariteSpinner>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.WoodYoyo)
                .AddIngredient(ItemID.FallenStar, 8)
                .AddIngredient(ModContent.ItemType<CosmicEnergy>(), 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}