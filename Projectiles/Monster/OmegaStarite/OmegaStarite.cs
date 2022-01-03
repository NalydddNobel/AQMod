using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Monster.OmegaStarite
{
    public class OmegaStarite : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
            Projectile.timeLeft = 16;
        }

        public override void AI()
        {
            var omegaStarite = Main.npc[(int)Projectile.ai[0]];
            if (!omegaStarite.active || omegaStarite.ai[0] == -1f)
                return;
            Projectile.timeLeft = 2;
            Projectile.Center = omegaStarite.Center;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            var omegaStarite = (NPCs.Boss.OmegaStarite)Main.npc[(int)Projectile.ai[0]].ModNPC;
            for (int i = 0; i < omegaStarite.orbs.Count; i++)
            {
                var collisionCenter = new Vector2(omegaStarite.orbs[i].position.X, omegaStarite.orbs[i].position.Y);
                Rectangle rect = Utils.CenteredRectangle(collisionCenter, new Vector2(50f, 50f));
                if (rect.Intersects(targetHitbox))
                    return true;
            }
            return false;
        }
    }
}