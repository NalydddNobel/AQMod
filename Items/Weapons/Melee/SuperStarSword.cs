using Aequus.Projectiles.Melee.Swords;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

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
            Item.SetWeaponValues(20, 4.5f);
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

        public override int ChoosePrefix(UnifiedRandom rand)
        {
            return AequusHelpers.RollSwordPrefix(Item, rand);
        }
    }
}