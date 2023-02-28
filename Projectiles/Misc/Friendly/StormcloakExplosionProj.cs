using Aequus.Buffs.Debuffs;
using Aequus.Common;
using Aequus.Particles;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc.Friendly
{
    public class StormcloakExplosionProj : ModProjectile
    {
        public override string Texture => "Aequus/Assets/Explosion1";

        public bool Ice => (int)Projectile.ai[0] == 1;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.DefaultToExplosion(90, DamageClass.Ranged, 20);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Ice ? Color.Cyan.UseA(80) * 1.5f : CrimsonHellfire.BloomColor.UseA(20) * 5;
        }

        public override void AI()
        {
            if (Projectile.frame == 0 && Projectile.frameCounter == 0 && Main.netMode != NetmodeID.Server)
            {
                Projectile.frame = -1;
                Projectile.frameCounter = Main.rand.Next(-10, 3);
            }
            if (Projectile.frame == -1)
            {
                Projectile.hide = true;
                if (Projectile.frameCounter == 0)
                {
                    Projectile.hide = false;
                    SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                    for (int i = 0; i < 5; i++)
                    {
                        var v = Main.rand.NextVector2Unit();
                        ParticleSystem.New<BloomParticle>(ParticleLayer.BehindPlayers).Setup(Projectile.Center + v * Main.rand.NextFloat(16f), v * Main.rand.NextFloat(3f, 12f),
                            Ice ? Color.Cyan.UseA(80) : CrimsonHellfire.FireColor, Ice ? Color.Blue * 0.1f : CrimsonHellfire.BloomColor * 0.1f, 1.25f, 0.3f);
                    }
                    for (int i = 0; i < 15; i++)
                    {
                        var v = Main.rand.NextVector2Unit();
                        Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<MonoDust>(), v * Main.rand.NextFloat(1f, 12f), 0,
                           Ice ? Color.Cyan.UseA(80) : new Color(255, 85, 25), Main.rand.NextFloat(0.4f, 1.5f));
                    }
                    Projectile.frame = 0;
                    Projectile.frameCounter = 1;
                }
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 2)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Type])
                {
                    Projectile.hide = true;
                }
            }
        }

        public void OnHit(Entity target)
        {
            new EntityCommons(target).AddBuff(Ice ? BuffID.Frostburn2 : BuffID.OnFire3, 300);
            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), Projectile.Center, Projectile.DirectionTo(target.Center) * 0.3f, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, ai0: Projectile.ai[0]);
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            OnHit(target);
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            OnHit(target);
        }
        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            OnHit(target);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int _);
            Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}