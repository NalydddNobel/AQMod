using Aequus.Buffs;
using Aequus.Buffs.Debuffs;
using Aequus.Common.Audio;
using Aequus.Content;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Magic
{
    public class TriacanthornProj : ModProjectile
    {
        private float _glowy;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 18;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Type] = 3;
            AequusProjectile.InflictsHeatDamage.Add(Type);
        }

        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 56;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 100;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return CorruptionHellfire.FireColor * 2f * Projectile.Opacity;
        }

        public override void AI()
        {
            Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 0.01f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.timeLeft < 90)
            {
                _glowy = MathHelper.Lerp(_glowy, 0f, 0.1f);
            }
            if (Projectile.timeLeft < 20)
            {
                if (Projectile.alpha < 255)
                {
                    Projectile.alpha += 40;
                    if (Projectile.alpha > 255)
                    {
                        Projectile.alpha = 255;
                    }
                }
            }
            else
            {
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 40;
                    _glowy = 1f - Projectile.alpha / 255f;
                    if (Projectile.alpha <= 0)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), newColor: CorruptionHellfire.FireColor, Scale: Main.rand.NextFloat(0.3f, 1f));
                            d.color *= d.scale * 2f;
                            d.velocity *= d.scale;
                        }
                        for (int i = 0; i < 2; i++)
                        {
                            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoSparkleDust>(), newColor: CorruptionHellfire.FireColor, Scale: Main.rand.NextFloat(0.4f, 1f));
                            d.color *= d.scale * 2f;
                            d.velocity *= d.scale;
                            d.fadeIn = d.scale + Main.rand.NextFloat(0.5f, 0.8f);
                        }
                        Projectile.alpha = 0;
                    }
                }
            }
            if (Projectile.ai[0] > -20f)
            {
                Projectile.localAI[0] = Projectile.ai[0];
                if (Projectile.alpha < 200)
                {
                    var v = Vector2.Normalize(Projectile.velocity);
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + v * 10f, Projectile.velocity,
                            Type, Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0] - 0.8f, (int)(Projectile.ai[1] + 1f) % 2);
                        int index = -(int)(Projectile.ai[0] / 0.8f);
                        if (index % 8 == 0)
                        {
                            int i = (index % 16 == 0 ? -1 : 1) * Projectile.direction;
                            var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, v.RotatedBy(MathHelper.PiOver4 / 2f * i) * 5f,
                                ModContent.ProjectileType<TriacanthornBolt>(), Projectile.damage / 2, Projectile.knockBack / 2f, Projectile.owner);
                        }
                    }
                    Projectile.ai[0] = -21f;
                }
            }
            else if ((int)Projectile.ai[0] == -20)
            {
                Projectile.localAI[0] = Projectile.ai[0];
                Projectile.ai[1] = 2f;
                if (Projectile.alpha == 0)
                {
                    Projectile.ai[0] = -22f;
                    SoundEngine.PlaySound(SoundID.Item74, Projectile.Center);
                }
            }
            Projectile.frame = (int)Projectile.ai[1];
            Lighting.AddLight(Projectile.Center, CorruptionHellfire.BloomColor.ToVector3() * Projectile.scale);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CorruptionHellfire.AddBuff(target, 120);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            AequusBuff.ApplyBuff<CorruptionHellfire>(target, 120, out bool canPlaySound);
            if (canPlaySound)
            {
                ModContent.GetInstance<BlueFireDebuffSound>().Play(target.Center, pitchOverride: -0.2f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int _);
            float opacity = Projectile.Opacity;
            if (Projectile.localAI[0] + 1f > -16f)
            {
                opacity *= (-Projectile.localAI[0] + 1f) / 17f;
            }
            var glowOffset = Vector2.Normalize(Projectile.velocity).RotatedBy(MathHelper.PiOver2) * 2f;
            Main.spriteBatch.Draw(texture, Projectile.position + offset + glowOffset - Main.screenPosition, frame, CorruptionHellfire.BloomColor * opacity * 4f, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, Projectile.position + offset - glowOffset - Main.screenPosition, frame, CorruptionHellfire.BloomColor * opacity * 4f, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, CorruptionHellfire.FireColor * 2f * opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            if (_glowy > 0f)
            {
                Main.spriteBatch.Draw(texture, Projectile.position + offset + glowOffset * _glowy * 4f - Main.screenPosition, frame, CorruptionHellfire.BloomColor * 2f * _glowy * opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, Projectile.position + offset - glowOffset * _glowy * 4f - Main.screenPosition, frame, CorruptionHellfire.BloomColor * 2f * _glowy * opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, Projectile.position + offset + glowOffset * _glowy * 8f - Main.screenPosition, frame, CorruptionHellfire.BloomColor * _glowy * opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, Projectile.position + offset - glowOffset * _glowy * 8f - Main.screenPosition, frame, CorruptionHellfire.BloomColor * _glowy * opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }

    public class TriacanthornBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            this.SetTrail(15);
            PushableEntities.AddProj(Type);
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 80;
        }

        public override void AI()
        {
            int target = Projectile.FindTargetWithLineOfSight(400f);
            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptionThorns, Projectile.velocity.Y * 0.3f, Projectile.velocity.Y * 0.3f);
            d.velocity *= 0.2f;
            d.noGravity = true;
            d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), Projectile.velocity.Y * 0.3f, Projectile.velocity.Y * 0.3f, newColor: Color.BlueViolet.UseA(0) * 0.6f);
            d.velocity *= 0.5f;
            d.noGravity = true;
            if (target != -1)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Main.npc[target].Center - Projectile.Center) * 5f, 0.04f);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }
}