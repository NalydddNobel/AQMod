using Aequus.Buffs.Misc;
using Aequus.Projectiles.Melee.Swords;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.Heavy
{
    [AutoloadGlowMask]
    public class UltimateSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToDopeSword<UltimateSwordProj>(20);
            Item.SetWeaponValues(50, 2.5f);
            Item.width = 30;
            Item.height = 30;
            Item.scale = 1.1f;
            Item.rare = ItemDefaults.RarityOmegaStarite;
            Item.value = ItemDefaults.ValueOmegaStarite;
            Item.autoReuse = true;
        }

        public override bool? UseItem(Player player)
        {
            Item.FixSwing(player);
            return null;
        }

        public override bool MeleePrefix()
        {
            return true;
        }

        public override void HoldItem(Player player)
        {
            player.AddBuff(ModContent.BuffType<UltimateSwordBuff>(), 1, quiet: true);
        }
    }
}