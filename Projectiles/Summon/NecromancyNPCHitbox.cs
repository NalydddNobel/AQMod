using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon
{
    public class NecromancyNPCHitbox : ModProjectile
    {
        public override string Texture => Aequus.TextureNone;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;

            Projectile.localNPCHitCooldown = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.timeLeft = 30;
        }

        public override void AI()
        {
            int npc = (int)Projectile.ai[0];
            if (!Main.npc[npc].active)
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = Main.npc[npc].Center;
        }
    }
}