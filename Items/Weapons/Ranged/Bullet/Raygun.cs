using AQMod.Assets.Textures;
using AQMod.Common;
using AQMod.Common.ItemOverlays;
using AQMod.Projectiles.Ranged.RayGunBullets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Ranged.Bullet
{
    public class Raygun : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new LegacyLegacyGlowmask(GlowID.Raygun), item.type);
        }

        public override void SetDefaults()
        {
            item.damage = 44;
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
            item.value = AQItem.OmegaStariteWeaponValue;
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