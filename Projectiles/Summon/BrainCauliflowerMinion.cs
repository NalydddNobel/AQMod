using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon
{
    public class BrainCauliflowerMinion : MinionBase
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 10;
            Main.projPet[Type] = true;

            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 36;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
        }
    }
}