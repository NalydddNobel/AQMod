using Aequus.Buffs.Minion;
using Aequus.Buffs.Necro;
using Aequus.Content.Necromancy;
using Aequus.Graphics;
using Aequus.Particles;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon
{
    public class RedSpriteMinion : MinionBase
    {
        public const int STATE_HAUNTING = 2;
        public const int STATE_ATTACKING = 1;
        public const int STATE_IDLE = 0;

        public float scaleTimer;

        public int State { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }

        public override void SetStaticDefaults()
        {
            Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 30;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 2;
            Projectile.localNPCHitCooldown = 120;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 40);
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool MinionContactDamage()
        {
            return State == STATE_HAUNTING;
        }

        public override void AI()
        {
            if (!AequusHelpers.UpdateProjActive<RedSpriteMinionBuff>(Projectile))
            {
                return;
            }

            int target = Projectile.ai[1] > 0f ? Projectile.GetMinionTarget(out float distance, 1250f, 400f) : -1;

            if (State == STATE_HAUNTING)
            {
                if (target != -1)
                {
                    Projectile.position = Main.npc[target].Center;
                    Projectile.velocity *= 0.1f;
                    return;
                }
                State = STATE_IDLE;
            }

            Projectile.rotation = Projectile.velocity.X * 0.1f;

            Projectile.GetMinionLeadership(out int leader, out int minionPos, out int count);
            Projectile.scale = AequusHelpers.Wave(scaleTimer * 0.01f, 0.5f, 0.7f);
            scaleTimer += Main.rand.NextFloat(0.2f, 1f);
            if (Projectile.whoAmI == leader)
            {
                Projectile.scale += 0.3f;
            }

            if (Projectile.alpha < 50 && Main.rand.NextBool((24 - (int)(12 * Projectile.scale)) * 5))
            {
                var d2 = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height - 8, ModContent.DustType<MonoSparkleDust>(), Alpha: 200, newColor: new Color(200, 70, 120, 40), Scale: Main.rand.NextFloat(0.8f, 1.2f));
                d2.velocity.X *= 0.1f;
                d2.velocity.Y = Main.rand.NextFloat(-4f, -1f);
                d2.fadeIn = d2.scale + 0.75f;
            }

            if (target != -1)
            {
                if (State != STATE_ATTACKING)
                {
                    Projectile.ai[1] = Main.rand.NextFloat(0f, 30f + count * 10f);
                    State = STATE_ATTACKING;
                    Projectile.netUpdate = true;
                }
                Projectile.ai[1]++;
                if (Projectile.ai[1] > 120f + 10f * count)
                {
                    Projectile.velocity.Y *= 0.94f;
                    Projectile.velocity.X *= 0.9f;
                    if (Projectile.alpha == 0)
                    {
                        Projectile.velocity.Y = -4f;
                    }
                    Projectile.alpha += 3;
                    if (Projectile.alpha > 255)
                    {
                        Projectile.Center = Main.npc[target].Center;
                        Projectile.alpha = 255;
                        State = STATE_HAUNTING;
                    }
                    return;
                }
            }
            else
            {
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 4;
                    if (Projectile.alpha < 0)
                    {
                        Projectile.alpha = 0;
                    }
                }
                Projectile.ai[1] += Main.rand.NextFloat(0.8f, 1f);
                Projectile.ai[0] = STATE_IDLE;
            }
            var idlePosition = IdlePosition(Main.player[Projectile.owner], leader, minionPos, count);
            float d = Projectile.Distance(idlePosition);
            if (d > 2000f)
            {
                Projectile.Center = Main.player[Projectile.owner].Center;
                Projectile.velocity *= 0.1f;
            }
            else if (d > 30f)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (idlePosition - Projectile.Center) / 30f, 0.015f);
            }
            Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.02f, 0.02f));
            Projectile.CollideWithOthers();
        }
        public override Vector2 IdlePosition(Player player, int leader, int minionPos, int count)
        {
            var p = base.IdlePosition(player, leader, minionPos, count) + new Vector2(0f, -100f);
            if (Projectile.whoAmI != leader)
            {
                int movePos = (minionPos + 1) / 2;
                int dir = minionPos % 2 == 0 ? 1 : -1;
                p.X += 30f * movePos * dir;
            }
            return p;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            var aequus = Main.player[Projectile.owner].Aequus();
            if (aequus.ghostSlots < aequus.ghostSlotsMax && target.lifeMax < 1800 && target.defense < 50 &&
                NecromancyDatabase.TryGet(target, out var info) && info.EnoughPower(3.1f))
            {
                target.AddBuffToHeadOrSelf(ModContent.BuffType<ConversionRedSprite>(), 120);
            }
            SoundEngine.PlaySound(SoundID.Item14, target.Center);
            Projectile.ai[1] = Main.rand.NextFloat(-60f, 0f);
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.position + new Vector2(Main.rand.NextFloat(target.width), Main.rand.NextFloat(target.height)), new Vector2(target.Center.X / 2f < Main.player[Projectile.owner].Center.X ? -0.01f : 0.01f, 0f),
                    ModContent.ProjectileType<RedSpriteMinionExplosion>(), damage, Projectile.knockBack, Main.myPlayer, target.whoAmI);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var t, out var off, out var frame, out var origin, out int _);

            var c = Projectile.GetAlpha(lightColor) * Projectile.Opacity * Projectile.scale;

            off -= Main.screenPosition;
            var tentaclePos = Projectile.position;
            float f = 0f;
            const int tentacleLength = 30;
            for (int i = 0; i < tentacleLength; i++)
            {
                float p = AequusHelpers.CalcProgress(tentacleLength, i);
                float scale = Projectile.scale * (1f + (1f - p)) * 0.33f;

                f += Math.Clamp(Projectile.rotation * 0.4f, -0.05f, 0.05f) + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 5f + i * 0.05f) * 0.02f;
                tentaclePos += Vector2.UnitY.RotatedBy(f);
                Main.spriteBatch.Draw(TextureCache.Bloom[0].Value, tentaclePos + off + new Vector2(0f, 32f * (1f - p) * Projectile.scale), null, Color.Black * Projectile.Opacity * p * p * Projectile.scale, 0f, TextureCache.Bloom[0].Value.Size() / 2f, scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(TextureCache.Bloom[0].Value, Projectile.position + off, null, Color.Black * Projectile.Opacity * Projectile.scale, 0f, TextureCache.Bloom[0].Value.Size() / 2f, Projectile.scale * 0.4f, SpriteEffects.None, 0f);

            tentaclePos = Projectile.position;
            f = 0f;
            for (int i = 0; i < tentacleLength; i++)
            {
                float p = AequusHelpers.CalcProgress(tentacleLength, i);
                float scale = Projectile.scale * (1f + (1f - p));
                f += Math.Clamp(Projectile.rotation * 0.4f, -0.05f, 0.05f) + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 5f + i * 0.05f) * 0.02f;
                tentaclePos += Vector2.UnitY.RotatedBy(f);
                Main.EntitySpriteDraw(t, tentaclePos + off + new Vector2(0f, 32f * (1f - p)), frame, c * p * 0.2f, Projectile.rotation, origin, scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(t, Projectile.position + off, frame, c, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }

    public class RedSpriteMinionExplosion : ModProjectile
    {
        public override string Texture => "Aequus/Assets/Explosion1";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.DefaultToExplosion(90, DamageClass.Summon, 40);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Red.UseA(50) * 0.8f;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return (target.whoAmI + 1) == (int)Projectile.ai[0] ? false : null;
        }

        public override void AI()
        {
            if (Projectile.frame == 0 && Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < 10; i++)
                {
                    var v = Main.rand.NextVector2Unit();
                    EffectsSystem.ParticlesBehindPlayers.Add(new BloomParticle(Projectile.Center + v * Main.rand.NextFloat(16f), v * Main.rand.NextFloat(3f, 12f),
                        Color.Red.UseA(0) * Main.rand.NextFloat(0.3f, 0.7f), Color.MediumVioletRed.UseA(0) * Main.rand.NextFloat(0.05f, 0.15f), Main.rand.NextFloat(0.8f, 1.6f), 0.3f, Main.rand.NextFloat(MathHelper.TwoPi)));
                }
                for (int i = 0; i < 15; i++)
                {
                    var v = Main.rand.NextVector2Unit();
                    EffectsSystem.ParticlesBehindPlayers.Add(new BloomParticle(Projectile.Center + v * Main.rand.NextFloat(16f), v * Main.rand.NextFloat(1f, 5f),
                        Color.Red.UseA(0) * Main.rand.NextFloat(0.1f, 0.4f), Color.MediumVioletRed.UseA(0) * Main.rand.NextFloat(0.01f, 0.05f), Main.rand.NextFloat(1.4f, 2.5f), 0.3f, Main.rand.NextFloat(MathHelper.TwoPi)));
                }
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Type])
                {
                    Projectile.hide = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int _);
            Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}