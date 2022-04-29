using Aequus.Items.Accessories;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Projectiles
{
    public class ProjectileEffects : GlobalProjectile
    {
        public override void PostAI(Projectile projectile)
        {
            if (projectile.owner >= 0 && projectile.owner != 255)
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
    }
}