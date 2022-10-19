using Aequus.Projectiles.Melee.Swords;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee
{
    [GlowMask]
    public class UltimateSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToDopeSword<UltimateSwordProj>(25);
            Item.SetWeaponValues(60, 6.5f);
            Item.width = 30;
            Item.height = 30;
            Item.scale = 2f;
            Item.rare = ItemDefaults.RarityOmegaStarite + 1;
            Item.value = ItemDefaults.OmegaStariteValue * 2;
            Item.autoReuse = true;
        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }
}