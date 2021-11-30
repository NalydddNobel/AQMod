using AQMod.Assets.ItemOverlays;
using AQMod.Projectiles.Ranged.RayGunBullets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Ranged
{
    public class Raygun : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new GlowmaskOverlay(this.GetPath("_Glow")), item.type);
        }

        public override void SetDefaults()
        {
            item.damage = 32;
            item.ranged = true;
            item.useTime = 16;
            item.useAnimation = 16;
            item.width = 32;
            item.height = 24;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = ItemRarityID.LightRed;
            item.shoot = ModContent.ProjectileType<RayBullet>();
            item.shootSpeed = 8f;
            item.autoReuse = true;
            item.UseSound = new LegacySoundStyle(SoundID.Trackable, 228);
            item.value = AQItem.Prices.OmegaStariteWeaponValue;
            item.knockBack = 0.5f;
            item.useAmmo = AmmoID.Bullet;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            type = RayBullet.BulletProjectileIDToRayProjectileID(type);
            return true;
        }
    }
}