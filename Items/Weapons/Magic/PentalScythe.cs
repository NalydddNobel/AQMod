using Aequus.Items.Misc;
using Aequus.Items.Misc.Energies;
using Aequus.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic
{
    [GlowMask]
    public class PentalScythe : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.width = 32;
            Item.height = 32;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<PentalScytheProj>();
            Item.shootSpeed = 25f;
            Item.mana = 7;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item109;
            Item.value = ItemDefaults.DemonSiegeValue;
            Item.knockBack = 2f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DemonScythe)
                .AddIngredient<DemonicEnergy>(3)
                .AddIngredient<Hexoplasm>(8)
                .AddTile(TileID.Bookcases)
                .RegisterAfter(ItemID.GoldenShower);
        }
    }
}