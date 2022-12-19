using Aequus.Projectiles.Melee.Swords;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee
{
    public class SuperStarSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToDopeSword<SuperStarSwordProj>(30);
            Item.SetWeaponValues(24, 4.5f);
            Item.width = 20;
            Item.height = 20;
            Item.scale = 1.25f;
            Item.rare = ItemDefaults.RarityGlimmer;
            Item.autoReuse = true;
            Item.value = ItemDefaults.GlimmerValue;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool MeleePrefix()
        {
            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            damage = (int)(damage * 0.8f);
        }
    }
}