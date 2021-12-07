using AQMod.Assets.LegacyItemOverlays;
using AQMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Ranged
{
    public class HamaYumi : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new GlowmaskOverlay(AQUtils.GetPath(this) + "_Glow"), item.type);
        }

        public override void SetDefaults()
        {
            item.damage = 33;
            item.ranged = true;
            item.useTime = 42;
            item.useAnimation = 42;
            item.width = 30;
            item.height = 30;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = AQItem.Rarities.GoreNestRare;
            item.shoot = ProjectileID.WoodenArrowFriendly;
            item.shootSpeed = 4f;
            item.useAmmo = AmmoID.Arrow;
            item.UseSound = SoundID.Item5;
            item.value = AQItem.Prices.DemonSiegeWeaponValue;
            item.noMelee = true;
            item.autoReuse = true;
            item.knockBack = 6f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return AQItem.Similarities.DemonSiegeItem_GetAlpha(lightColor);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            type = HamaYumiArrow.ArrowProjectileIDToHamaYumiProjectileID(type);
            return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }
    }
}