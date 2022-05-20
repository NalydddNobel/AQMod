using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Monster.OmegaStarite
{
    public class OmegaStariteProj : ModProjectile
    {
        public float Radius => 84.85f / 2f;

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
        }

        public override bool? CanDamage()
        {
            return Projectile.ai[1] > 0;
        }

        public override void AI()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                var omegaStarite = Main.npc[(int)Projectile.ai[0]];
                if (!omegaStarite.active || omegaStarite.ai[0] == -1f || !(omegaStarite.ModNPC is NPCs.Boss.OmegaStarite))
                    return;
                Projectile.ai[1] = 1f;
                Projectile.timeLeft = 32;
                Projectile.Center = omegaStarite.Center;
            }
            else
            {
                Projectile.ai[1] = 1f;
                var omegaStarite = Main.npc[(int)Projectile.ai[0]];
                if (!omegaStarite.active || omegaStarite.ai[0] == -1f)
                    return;
                Projectile.timeLeft = 2;
                Projectile.Center = omegaStarite.Center;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            var omegaStarite = (NPCs.Boss.OmegaStarite)Main.npc[(int)Projectile.ai[0]].ModNPC;
            for (int i = 0; i < omegaStarite.rings.Count; i++)
            {
                for (int j = 0; j < omegaStarite.rings[i].amountOfSegments; j++)
                {
                    if (omegaStarite.rings[i].CachedHitboxes[j].Intersects(targetHitbox))
                    {
                        return AequusHelpers.IsRectangleCollidingWithCircle(omegaStarite.rings[i].CachedHitboxes[j].Center.ToVector2(), Radius, targetHitbox);
                    }
                }
            }
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (target.townNPC || target.life < 5)
                damage = (int)(damage * 0.1f);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            Main.npc[(int)Projectile.ai[0]].ModNPC.OnHitPlayer(target, damage, crit); // janky magic :trollface:
        }
    }
}