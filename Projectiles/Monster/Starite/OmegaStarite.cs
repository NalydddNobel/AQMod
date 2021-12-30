using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Monster.Starite
{
    public class OmegaStarite : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.tileCollide = false;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.hide = true;
        }

        public override void AI()
        {
            var omegaStarite = Main.npc[(int)projectile.ai[0]];
            if (!omegaStarite.active || omegaStarite.ai[0] == -1f)
                return;
            projectile.timeLeft = 2;
            projectile.Center = omegaStarite.Center;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            var omegaStarite = (NPCs.Boss.Starite.OmegaStarite)Main.npc[(int)projectile.ai[0]].modNPC;
            for (int i = 0; i < omegaStarite.orbs.Count; i++)
            {
                var collisionCenter = new Vector2(omegaStarite.orbs[i].position.X, omegaStarite.orbs[i].position.Y);
                Rectangle rect = Utils.CenteredRectangle(collisionCenter, new Vector2(50f, 50f));
                if (rect.Intersects(targetHitbox))
                    return true;
            }
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            Main.npc[(int)projectile.ai[0]].modNPC.OnHitPlayer(target, damage, crit); // janky magic :trollface:
        }
    }
}