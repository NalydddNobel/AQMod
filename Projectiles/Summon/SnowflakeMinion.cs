using Aequus.Buffs.Minion;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Aequus.Projectiles.Summon
{
    public sealed class SnowflakeMinion : MinionBase
    {
        private float _pulseTimer;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.manualDirectionChange = true;
            Projectile.minionSlots = 0.5f;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 55;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(155, 155, 155, 100);
        }

        public override Vector2 IdlePosition(Player player, int leader, int minionPos, int count)
        {
            return base.IdlePosition(player, leader, minionPos, count) + new Vector2(0f, -100f);
        }

        public override bool MinionContactDamage()
        {
            return true;
        }

        public override void AI()
        {
            if (!AequusHelpers.UpdateProjActive<SnowflakeBuff>(Projectile))
            {
                return;
            }

            var player = Main.player[Projectile.owner];
            Projectile.idStaticNPCHitCooldown = 45;
            for (int i = 0; i < player.ownedProjectileCounts[Type]; i++)
            {
                Projectile.idStaticNPCHitCooldown -= Math.Clamp(5 - i / 2, 1, 8);
            }
            _pulseTimer = Main.GlobalTimeWrappedHourly;
            Projectile.GetMinionLeadership(out int leader, out int minionPos, out int count);
            if (leader == Projectile.whoAmI)
            {
                Projectile.ai[1] = 0f;
                Projectile.rotation += 0.01125f;

                int target = Projectile.GetMinionTarget(out float distance, 1250f, 400f);
                if (target != -1)
                {
                    var targetCenter = Main.npc[target].Center;
                    float amount = 0.03f;
                    if (distance < 320f)
                    {
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(targetCenter - Projectile.Center) * ((320f - distance) / 8f + 7.5f), distance / 320f * 0.1f);
                        if (Projectile.velocity.Length() < 5.5f)
                        {
                            Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 5.5f;
                        }
                    }
                    else
                    {
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(targetCenter - Projectile.Center) * 7.5f, amount);
                    }
                }
                else
                {
                    var idlePos = IdlePosition(player, leader, minionPos, count);
                    distance = (Projectile.Center - idlePos).Length();
                    if (distance > 2000f)
                    {
                        Projectile.Center = idlePos;
                        Projectile.velocity *= 0.1f;
                    }
                    else if (distance > 20f)
                    {
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, (idlePos - Projectile.Center) / 32f, 0.01f);
                    }
                }
                Projectile.rotation += Projectile.velocity.Length() * 0.0157f;
            }
            else
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.ai[0] = 0f;
                int index = (int)Main.projectile[leader].ai[1];
                int outwards = (4 + index) / 8 + 1;
                Projectile.rotation = Main.projectile[leader].rotation;
                float rot = Projectile.rotation;
                if (index >= 4)
                {
                    float rot2 = 0.15f * ((index - 4) / 8f + 1);
                    rot2 = (index - 4) / 4 % 2 == 1 ? -rot2 : rot2;
                    rot += rot2;
                    Projectile.rotation += rot2;
                }
                Projectile.Center = Main.projectile[leader].Center + new Vector2((Projectile.width - 6f) * outwards * Projectile.scale, 0f).RotatedBy(index * MathHelper.PiOver2 + rot);
                Main.projectile[leader].ai[1]++;
                _pulseTimer -= (Main.projectile[leader].Center - Projectile.Center).Length() * 0.15f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 120);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var frame = texture.Frame();
            var origin = frame.Size() / 2f;
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f) - Main.screenPosition;
            var color = Projectile.GetAlpha(lightColor);
            int trailLength = ProjectileID.Sets.TrailCacheLength[Projectile.type];
            for (int i = 0; i < trailLength; i++)
            {
                float progress = 1f / trailLength * i;
                Main.spriteBatch.Draw(texture, Projectile.oldPos[i] + offset, frame, new Color(0, 0, 120, 0) * (1f - progress), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, Projectile.position + offset, frame, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            float wave = AequusHelpers.Wave(_pulseTimer * 4f, 0f, 4f);
            color *= 0.1f + wave * 0.1f;
            color.A = 0;
            for (int i = 0; i < 4; i++)
            {
                Main.spriteBatch.Draw(texture, Projectile.position + offset + new Vector2(wave, 0f).RotatedBy(MathHelper.PiOver2 * i + Projectile.rotation), frame, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}