using AQMod.Common.Graphics;
using AQMod.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Ranged
{
    public class SpaceShot : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.aiStyle = -1;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 255);

        public override void AI()
        {
            if (projectile.ai[0] == 0f)
            {
                projectile.ai[0] = ProjectileID.Bullet;
            }
            projectile.rotation += projectile.velocity.Length() * 0.08f;
            projectile.ai[1]++;
            if (projectile.ai[1] >= 14f)
            {
                projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int dustAmount = damage / 10;
            if (dustAmount < 1)
            {
                dustAmount = 1;
            }
            if (crit)
            {
                dustAmount *= 2;
            }
            if (target.life > 0 && !target.buffImmune[ModContent.BuffType<Buffs.Debuffs.BlueFire>()] && Main.rand.NextBool(2))
            {
                dustAmount *= 2;
                target.AddBuff(ModContent.BuffType<Buffs.Debuffs.BlueFire>(), 120);
                if (Main.netMode != NetmodeID.Server)
                {
                    AQSound.Play(SoundType.NPCHit, "inflict_bluefire", target.Center, 0.8f);
                }
            }
            for (int i = 0; i < dustAmount; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<Dusts.MonoSparkleDust>(),
                    Vector2.UnitX.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi)) * (4f + Main.rand.NextFloat() * 4f), 150, new Color(150, 170, 200, 100)).noGravity = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);

            for (int i = 0; i < 7; i++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 58, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 150, default(Color), 0.8f);
            }
            for (float f = 0f; f < 1f; f += 0.125f)
            {
                Dust.NewDustPerfect(projectile.Center, ModContent.DustType<Dusts.MonoSparkleDust>(), Vector2.UnitY.RotatedBy(f * (MathHelper.Pi * 2f) + Main.rand.NextFloat() * 0.3f) * (3f + Main.rand.NextFloat() * 3f), 150, Color.CornflowerBlue * 0.85f).noGravity = true;
            }
            for (float f = 0f; f < 1f; f += 0.25f)
            {
                Dust.NewDustPerfect(projectile.Center, ModContent.DustType<Dusts.MonoSparkleDust>(), Vector2.UnitY.RotatedBy(f * (MathHelper.Pi * 2f) + Main.rand.NextFloat() * 0.2f) * (1f + Main.rand.NextFloat() * 2f), 150, Color.Gold * 0.9f).noGravity = true;
            }
            if (AQGraphics.Rendering.Culling.InScreenWorld(projectile.getRect()))
            {
                for (int i = 0; i < 3; i++)
                {
                    Gore.NewGore(projectile.position, Main.rand.NextVector2CircularEdge(0.5f, 0.5f) * projectile.velocity.Length() * 0.3f, Utils.SelectRandom<int>(Main.rand, 16, 17, 17, 17, 17, 17, 17, 17));
                }
            }
            if (Main.myPlayer == projectile.owner)
            {
                var center = projectile.Center;
                var speed = projectile.velocity.Length() * 0.9f;
                int type = (int)projectile.ai[0];
                float velocityRotation = projectile.velocity.ToRotation();
                for (float rotAdd = 0; rotAdd < 6.28f; rotAdd += 1.26f)
                {
                    float rot = velocityRotation + rotAdd;
                    if (rotAdd != 0)
                    {
                        rot += Main.rand.NextFloat(-0.1f, 0.1f);
                    }
                    int p = Projectile.NewProjectile(center, new Vector2(speed, 0f).RotatedBy(rot), type, projectile.damage, projectile.knockBack, projectile.owner);
                    // gives all weapons an extra penetration,
                    // this is mostly for removing non-penetrating bullets dealing ~x3 more damage on contact shots,
                    // doesn't effect players who use that "penetration fixer" mod
                    if (Main.projectile[p].penetrate >= 0)
                        Main.projectile[p].penetrate++; 
                }
            }
        }
    }
}
