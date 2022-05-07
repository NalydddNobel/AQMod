using Aequus.Items.Accessories;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles
{
    public class AequusProjectile : GlobalProjectile
    {
        public override void PostAI(Projectile projectile)
        {
            if (projectile.friendly && projectile.owner >= 0 && projectile.owner != 255)
            {
                var aequus = Main.player[projectile.owner].Aequus();
                if (aequus.glowCore > 0)
                {
                    AequusPlayer.teamContext = Main.player[projectile.owner].team;
                    GlowCore.AddLight(projectile, aequus.glowCore);
                    AequusPlayer.teamContext = 0;
                }
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.sentry || ProjectileID.Sets.SentryShot[projectile.type])
            {
                if (Main.player[projectile.owner].Aequus().frostburnSentry && Main.rand.NextBool(6))
                {
                    target.AddBuff(BuffID.Frostburn2, 240);
                }
            }
        }
    }
}