using Aequus.Projectiles.Summon.Necro;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.Scepters
{
    [GlowMask]
    public class Revenant : ScepterBase
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Item.DefaultToNecromancy(25);
            Item.SetWeaponValues(40, 1f, 0);
            Item.shoot = ModContent.ProjectileType<RevenantBolt>();
            Item.shootSpeed = 11.5f;
            Item.rare = ItemRarityID.Green;
            Item.value = ItemDefaults.ValueDungeon;
            Item.mana = 15;
            Item.UseSound = SoundID.Item8;
        }
    }
}