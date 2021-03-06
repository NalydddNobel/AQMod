using Aequus.Projectiles.Summon.Necro;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.Necro
{
    public class Revenant : BaseScepter
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
            Item.shootSpeed = 10f;
            Item.rare = ItemRarityID.Green;
            Item.value = ItemDefaults.DungeonValue;
            Item.mana = 15;
            Item.UseSound = SoundID.Item8;
        }
    }
}