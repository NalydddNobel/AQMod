using Aequus.Items.Misc.Energies;
using Aequus.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee
{
    public class PhaseDisc : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.SetWeaponValues(28, 3f, 0);
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.rare = ItemRarityID.LightPurple;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.value = Item.sellPrice(gold: 3);
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<PhaseDiscProj>();
            Item.shootSpeed = 6.5f;
            Item.autoReuse = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 7;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai1: player.altFunctionUse == 2 ? 1 : 0);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LightDisc, 5)
                .AddIngredient<Valari>()
                .AddIngredient<AtmosphericEnergy>()
                .AddTile(TileID.MythrilAnvil)
                .TryRegisterAfter(ItemID.LightDisc);
        }
    }
}