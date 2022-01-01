using AQMod.Items.DrawOverlays;
using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Ranged
{
    public class SpaceShot : ModItem, IItemOverlaysWorldDraw, IItemOverlaysPlayerDraw
    {
        private static readonly GlowmaskOverlay _overlay = new GlowmaskOverlay(AQUtils.GetPath<SpaceShot>("_Glow"));
        IOverlayDrawWorld IItemOverlaysWorldDraw.WorldDraw => _overlay;
        IOverlayDrawPlayerUse IItemOverlaysPlayerDraw.PlayerDraw => _overlay;

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useTime = 30;
            item.useAnimation = 30;
            item.damage = 13;
            item.rare = AQItem.Rarities.StariteWeaponRare;
            item.ranged = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.shoot = ProjectileID.Bullet;
            item.shootSpeed = 15f;
            item.value = Item.sellPrice(silver: 80);
            item.useAmmo = AmmoID.Bullet;
            item.knockBack = 4f;
            item.UseSound = SoundID.Item11;
            item.noMelee = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<Projectiles.Ranged.SpaceShot>(), damage, knockBack, player.whoAmI, type);
            return false;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.FlintlockPistol);
            r.AddIngredient(ItemID.FallenStar, 8);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>(), 3);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}