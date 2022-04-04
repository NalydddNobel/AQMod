using Aequus.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus
{
    partial class AequusHelpers
    {
        public static void DefaultToDopeSword<T>(this Item item, int swingTime) where T : DopeSwordBase
        {
            item.channel = true;
            item.useTime = swingTime;
            item.useAnimation = swingTime;
            item.DamageType = DamageClass.Melee;
            item.useStyle = ItemUseStyleID.Shoot;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.shootSpeed = 1f;
            item.shoot = ModContent.ProjectileType<T>();
        }
    }
}