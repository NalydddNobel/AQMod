using AQMod.Common;
using AQMod.Common.ItemOverlays;
using AQMod.Common.Utilities;
using AQMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Ranged
{
    public class Deltoid : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new GlowmaskOverlayData(CommonUtils.GetPath(this) + "_Glow"), item.type);
        }

        public override void SetDefaults()
        {
            item.damage = 16;
            item.ranged = true;
            item.useTime = 28;
            item.useAnimation = 28;
            item.width = 30;
            item.height = 30;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = ItemRarityID.Orange;
            item.shoot = ProjectileID.WoodenArrowFriendly;
            item.shootSpeed = 2.8f;
            item.useAmmo = AmmoID.Arrow;
            item.UseSound = SoundID.Item5;
            item.value = AQItem.DemonSiegeWeaponValue;
            item.noMelee = true;
            item.autoReuse = true;
            item.knockBack = 3f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return AQItem.GetAlphaDemonSiegeWeapon(lightColor);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), DeltoidArrow.ArrowProjectileIDToHamaYumiProjectileID(type), damage, knockBack, player.whoAmI, 0f, type);
            return false;
        }
    }
}