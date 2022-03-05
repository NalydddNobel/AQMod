using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Summon
{
    public abstract class AQMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public virtual Vector2 IdlePosition(Player player, AQPlayer aQPlayer)
        {
            return player.MountedCenter;
        }

        public override bool? CanCutTiles() => false;

        protected void TurnTo(Vector2 to, float amount)
        {
            TurnTo(to, amount, projectile.velocity.Length());
        }
        protected void TurnTo(Vector2 to, float amount, float speed)
        {
            projectile.velocity = Vector2.Normalize(Vector2.Lerp(projectile.velocity, to - projectile.Center, amount)) * speed;
        }
        protected void SnapTo(Vector2 to, float amount)
        {
            var lerpPosition = Vector2.Lerp(projectile.Center, to, amount);
            var difference = lerpPosition - projectile.Center;
            projectile.velocity = difference;
            if (difference.X < 0f)
            {
                projectile.direction = -1;
            }
            else
            {
                projectile.direction = 1;
            }
        }
        protected int FindTarget(float distance, bool useForceTarget = true, float tileLineCheckDistanceMultiplier = 2f)
        {
            float _ = distance;
            return FindTarget(ref _, useForceTarget, tileLineCheckDistanceMultiplier);
        }
        protected int FindTarget(ref float distance, bool useForceTarget = true, float tileLineCheckDistanceMultiplier = 2f)
        {
            int target = -1;
            var player = Main.player[projectile.owner];
            var center = projectile.Center;

            if (useForceTarget && player.HasMinionAttackTargetNPC)
            {
                int t = player.MinionAttackTargetNPC;
                float d = (Main.npc[t].Center - center).Length();
                if (d < distance)
                {
                    target = t;
                    distance = d;
                }
            }

            if (target == -1)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(attacker: projectile, ignoreDontTakeDamage: false))
                    {
                        var difference = npc.Center - center;
                        float c = (float)Math.Sqrt(difference.X * difference.X + difference.Y * difference.Y);
                        if (!Collision.CanHitLine(npc.position, npc.width, npc.height, Main.player[projectile.owner].position, Main.player[projectile.owner].width, Main.player[projectile.owner].height))
                            c *= tileLineCheckDistanceMultiplier;
                        if (c < distance)
                        {
                            target = i;
                            distance = c;
                        }
                    }
                }
            }

            return target;
        }

        protected bool ActiveCheck(ref bool flag)
        {
            return ActiveCheck(ref flag, Main.player[projectile.owner]);
        }
        protected bool ActiveCheck(ref bool flag, Player player)
        {
            if (player.dead)
                flag = false;
            if (flag)
                projectile.timeLeft = 2;
            return flag;
        }
    }
}