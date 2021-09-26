using Terraria.ModLoader;

namespace AQMod.NPCs.Ocean.Crabson
{
    public class CrabBubble : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.hostile = true;
            projectile.aiStyle = 95;
            projectile.timeLeft = 120;
        }

        public override void AI()
        {
            projectile.velocity.X *= 0.98f;
        }
    }
}