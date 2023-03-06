using Aequus.Projectiles.Melee.Swords;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.Heavy
{
    public class SuperStarSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
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
            Item.value = ItemDefaults.ValueGlimmer;
        }

        public override bool? UseItem(Player player)
        {
            Item.FixSwing(player);
            return null;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }
}