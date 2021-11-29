using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    public class Dysesthesia : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = 6f;
            ProjectileID.Sets.YoyosMaximumRange[projectile.type] = 420f;
            ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 14f;
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = 99;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.melee = true;
        }

        public override void AI()
        {
            if (projectile.ai[0] != -1)
            {
                projectile.localAI[1]++;
                if (projectile.localAI[1] > 14f * Main.player[projectile.owner].meleeSpeed)
                {
                    int target = AQNPC.FindTarget(projectile.Center, 300f);
                    Vector2 normal;
                    if (target != -1)
                    {
                        normal = Vector2.Normalize(projectile.Center - Main.npc[target].Center);
                    }
                    else
                    {
                        normal = new Vector2(0f, 1f).RotatedBy((projectile.localAI[0] - projectile.localAI[1]) * 0.04f);
                    }
                    Main.PlaySound(SoundID.Item8, projectile.Center);
                    int type = ModContent.ProjectileType<MiniDemonScythe>();
                    Projectile.NewProjectile(projectile.Center, normal, type, projectile.damage, projectile.knockBack, projectile.owner, projectile.identity);
                    Projectile.NewProjectile(projectile.Center, -normal, type, projectile.damage, projectile.knockBack, projectile.owner, projectile.identity);
                    projectile.localAI[1] = 0f;
                    projectile.netUpdate = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.NextBool(4) && target.Distance(Main.player[projectile.owner].Center) < (ProjectileID.Sets.YoyosMaximumRange[projectile.type] / 2f))
                Buffs.Debuffs.CorruptionHellfire.Inflict(target, 360);
        }
    }
}