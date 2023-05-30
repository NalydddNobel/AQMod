using Aequus.Buffs.Necro;
using Aequus.Content;
using Aequus.Particles;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon.Necro {
    public class OsirisBolt : ZombieBolt
    {
        public override string Texture => Helper.GetPath<ZombieBolt>();

        public override float Tier => 3f;

        public override void SetStaticDefaults()
        {
            this.SetTrail(10);
            PushableEntities.AddProj(Type);
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.scale = 0.8f;
            Projectile.alpha = 10;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
            Projectile.alpha = 250;
            Projectile.extraUpdates = 1;
            Projectile.scale = 1.33f;
            Projectile.DamageType = Aequus.NecromancyClass;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 222, 100, 255 - Projectile.alpha);
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                var center = Projectile.Center;
                foreach (var v in Helper.CircularVector(3, Main.GlobalTimeWrappedHourly * 5f))
                {
                    if (Main.rand.NextBool(3))
                        ParticleSystem.New<BloomParticle>(ParticleLayer.BehindProjs).Setup(center + v * 8f, Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * -0.125f, new Color(255, 160, 100, 100), Color.OrangeRed.UseA(0) * 0.1f, 1.1f, 0.35f, Main.rand.NextFloat(MathHelper.TwoPi));
                }
            }

            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 13;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }

            int target = Projectile.FindTargetWithLineOfSight(500f);
            if (target != -1)
            {
                float speed = Projectile.velocity.Length();
                Projectile.velocity = Vector2.Normalize(Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[target].Center) * speed, 0.1f)) * speed;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            NecromancyDebuff.ReduceDamageForDebuffApplication<OsirisDebuff>(Tier, target, ref modifiers);
        }

        public void SpawnLocusts(Entity target)
        {
            var source = Projectile.GetSource_OnHit(target, "Aequus:Osiris");
            int distance = (int)(target.Size.Length() / 2f);
            for (int i = 0; i < 3; i++)
            {
                var normal = Main.rand.NextVector2Unit();
                var p = Projectile.NewProjectile(source, target.Center + normal * distance, normal * 3f, LocustType(Main.player[Projectile.owner]), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, 0f, target.whoAmI);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;

            NecromancyDebuff.ApplyDebuff<OsirisDebuff>(target, 600, Projectile.owner);
            SpawnLocusts(target);
        }
        public int LocustType(Player player)
        {
            if (player.strongBees && Main.rand.NextBool(3))
            {
                return ModContent.ProjectileType<LocustLarge>();
            }
            return ModContent.ProjectileType<LocustSmall>();
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            SpawnLocusts(target);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var frame = Projectile.Frame();
            var origin = frame.Size() / 2f;

            int trailLength = ProjectileID.Sets.TrailCacheLength[Type];
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            for (int i = 0; i < trailLength; i++)
            {
                float progress = Helper.CalcProgress(trailLength, i);
                Main.spriteBatch.Draw(texture, Projectile.oldPos[i] + offset - Main.screenPosition, frame, Projectile.GetAlpha(lightColor) * Projectile.Opacity * progress, Projectile.rotation, origin, Projectile.scale * 0.75f * progress, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, new Color(255, 128, 10, 100) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale * 0.5f, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            var center = Projectile.Center;
            for (int i = 0; i < 7; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.YellowStarDust, newColor: new Color(255, 255, 255, 0));
                d.velocity *= 0.2f;
                d.velocity += (d.position - center) / 8f;
                d.scale += Main.rand.NextFloat(-0.5f, 0f);
                d.fadeIn = d.scale + Main.rand.NextFloat(0.2f, 0.5f);
            }
            for (int i = 0; i < 20; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), newColor: new Color(255, 222, 222, 150));
                d.velocity *= 0.2f;
                d.velocity += (d.position - center) / 8f;
            }
        }
    }
}