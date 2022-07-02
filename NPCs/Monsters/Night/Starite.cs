using Aequus.Biomes;
using Aequus.Buffs.Debuffs;
using Aequus.Graphics;
using Aequus.Items.Consumables.Foods;
using Aequus.Items.Misc;
using Aequus.Items.Misc.Dyes;
using Aequus.Items.Placeable.Banners;
using Aequus.Particles;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters.Night
{
    public class Starite : ModNPC
    {
        public static int[] BuffImmunities => new int[]
        {
            BuffID.Confused,
            BuffID.OnFire,
            BuffID.OnFire3,
            BuffID.ShadowFlame,
            BuffID.Frostburn,
            BuffID.Frostburn2,
            BuffID.Ichor,
            BuffID.Poisoned,
            BuffID.Venom,
            BuffID.Bleeding,
            BuffID.Weak,
            BuffID.Stinky,
            BuffID.Lovestruck,
            BuffID.Wet,
            BuffID.Slimed,
            ModContent.BuffType<CrimsonHellfire>(),
            ModContent.BuffType<CorruptionHellfire>(),
        };

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[Type] = 7;
            NPCID.Sets.TrailCacheLength[Type] = 12;
            NPCID.Sets.DebuffImmunitySets.Add(Type, new Terraria.DataStructures.NPCDebuffImmunityData()
            {
                SpecificallyImmuneTo = BuffImmunities,
            });
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Position = new Vector2(0f, 17f),
                PortraitPositionYOverride = 36,
            });
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            this.CreateLoot(npcLoot)
                .Add<CelesitalEightBall>(chance: 50, stack: 1)
                .Add<NeutronYogurt>(chance: 5, stack: 1);
        }

        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 20;
            NPC.lifeMax = 45;
            NPC.damage = 45;
            NPC.defense = 3;
            NPC.HitSound = SoundID.NPCHit39;
            NPC.DeathSound = SoundID.NPCDeath55;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.knockBackResist = 1.1f;
            NPC.value = Item.buyPrice(silver: 2);
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<StariteBanner>();

            this.SetBiome<GlimmerInvasion>();
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            float x = NPC.velocity.X.Abs() * hitDirection;
            if (NPC.life <= 0)
            {
                if (NPC.life == -33333)
                {
                    for (int i = 0; i < 60; i++)
                    {
                        var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Unit() * Main.rand.Next(2, 32), ModContent.DustType<MonoDust>(), newColor: Color.Lerp(Color.Yellow.UseB(128), Color.White, Main.rand.NextFloat(0.2f, 1f)).UseA(0));
                        d.velocity *= 0.2f;
                        d.velocity += d.position - NPC.Center;
                    }
                    for (int i = 0; i < 50; i++)
                    {
                        var b = new BloomParticle(NPC.Center + Main.rand.NextVector2Unit() * Main.rand.Next(2, 12), Vector2.Zero, Color.White.UseA(0), new Color(25, 25, 40, 0), Main.rand.NextFloat(0.8f, 1.45f), 0.33f);
                        b.Velocity += (b.Position - NPC.Center) / 2f;
                        AequusEffects.AbovePlayers.Add(b);
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        var b = new BloomParticle(NPC.Center + Main.rand.NextVector2Unit() * Main.rand.Next(10, 42), Vector2.Zero, Color.White.UseA(0), new Color(25, 25, 40, 0), Main.rand.NextFloat(0.8f, 1.45f), 0.33f);
                        b.Velocity += (b.Position - NPC.Center) / 3f;
                        AequusEffects.AbovePlayers.Add(b);
                    }
                    for (int i = 0; i < 25; i++)
                    {
                        var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Unit() * Main.rand.Next(2, 32), DustID.Enchanted_Gold + Main.rand.Next(2), newColor: Color.White.UseA(0));
                        d.velocity *= 0.1f;
                        d.velocity += (d.position - NPC.Center) / 2f;
                    }
                }
                for (int i = 0; i < 30; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 57 + Main.rand.Next(2));
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                }
                for (int i = 0; i < 4; i++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.NextFloat(-5f, 5f) + x, Main.rand.NextFloat(-5f, 5f)), 16 + Main.rand.Next(2));
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 55);
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(5f, 12f);
                }
                if (Main.rand.NextBool())
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 57 + Main.rand.Next(2));
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                }
                if (Main.rand.NextBool())
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.NextFloat(-4f, 4f) + x * 0.75f, Main.rand.NextFloat(-4f, 4f)), 16 + Main.rand.Next(2));
            }
        }

        public override void AI()
        {
            if (Main.dayTime)
            {
                NPC.life = -1;
                NPC.HitEffect();
                NPC.active = false;
                return;
            }
            Vector2 center = NPC.Center;
            const float collisionMult = 0.8f;
            if (NPC.collideX)
            {
                if (NPC.oldVelocity.X.Abs() > 2f)
                    NPC.velocity.X = -NPC.oldVelocity.X * collisionMult;
                NPC.ai[2] *= -collisionMult;
            }
            if (NPC.collideY)
            {
                if (NPC.oldVelocity.Y.Abs() > 2f)
                    NPC.velocity.Y = -NPC.oldVelocity.Y * collisionMult;
                NPC.ai[3] *= -collisionMult;
            }
            if ((int)NPC.ai[0] == -1)
            {
                NPC.velocity *= 0.9f;
                if (NPC.ai[3] > 0f)
                    NPC.ai[3] = 0f;
                NPC.ai[3] -= 0.66f;
                if (Main.rand.NextBool())
                {
                    var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Unit() * NPC.ai[3] / 2f, ModContent.DustType<MonoDust>(), newColor: Color.Lerp(Color.Yellow.UseB(128), Color.White, Math.Min(Main.rand.NextFloat(0.5f, 1f) - NPC.ai[3] / 60f, 1f)).UseA(0));
                    d.velocity *= 0.2f;
                    d.velocity += (NPC.Center - d.position) / 16f;
                }
                if (NPC.ai[3] < -60f)
                {
                    NPC.life = -33333;
                    NPC.HitEffect();
                    NPC.checkDead();
                }
                return;
            }

            if (Main.rand.NextBool(20))
            {
                var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.MagicMirror);
                d.velocity = NPC.velocity * 0.01f;
            }
            if (Main.rand.NextBool(40))
            {
                var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Enchanted_Pink);
                d.velocity.X = Main.rand.NextFloat(-4f, 4f);
                d.velocity.Y = Main.rand.NextFloat(-4f, 4f);
            }
            if (Main.rand.NextBool(40))
            {
                var g = Gore.NewGoreDirect(NPC.GetSource_FromThis(), NPC.position + new Vector2(Main.rand.Next(NPC.width - 4), Main.rand.Next(NPC.height - 4)), new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f)), 16 + Main.rand.Next(2));
                g.scale *= 0.6f;
            }
            Lighting.AddLight(NPC.Center, new Vector3(0.4f, 0.4f, 0.2f));
            NPC.rotation += NPC.velocity.Length() * 0.0157f;

            if (NPC.ai[0] == 0f)
            {
                NPC.TargetClosest(faceTarget: false);
                if (NPC.HasValidTarget && !Main.player[NPC.target].dead && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                {
                    NPC.ai[0] = 1f;
                    NPC.velocity = new Vector2(Main.rand.NextFloat(4f, 6f), 0f).RotatedBy((Main.player[NPC.target].Center - center).ToRotation());
                }
                else
                {
                    NPC.velocity *= 0.985f;
                    return;
                }
            }
            var player = Main.player[NPC.target];
            var plrCenter = player.Center;
            if (NPC.ai[0] == 1f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (NPC.ai[1] == 0f)
                    {
                        NPC.ai[1] = Main.rand.Next(60, 100);
                        NPC.netUpdate = true;
                    }
                    if (NPC.ai[1] == 1f)
                    {
                        NPC.ai[0] = 2f;
                        NPC.netUpdate = true;
                    }
                }
                float turnSpeed = MathHelper.Clamp((NPC.ai[1] + NPC.localAI[0]) / 100000f, 0f, 1f); // don't even ask...
                NPC.ai[1]--;
                if (NPC.localAI[0] > 0)
                    NPC.localAI[0]--;
                if (turnSpeed != 0f)
                {
                    float length = NPC.velocity.Length();
                    Vector2 difference = plrCenter - center;
                    NPC.velocity = Vector2.Normalize(Vector2.Lerp(NPC.velocity, difference, turnSpeed)) * length;
                }
            }
            if (NPC.ai[0] == 2f)
            {
                if (NPC.ai[2] == 0f && NPC.ai[3] == 0f)
                {
                    NPC.TargetClosest(faceTarget: false);
                    if (NPC.HasValidTarget && !Main.player[NPC.target].dead && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                    {
                        float min;
                        float max;
                        float add;
                        if (Main.expertMode)
                        {
                            min = 4f;
                            max = 6.5f;
                            add = 2f;
                        }
                        else
                        {
                            min = 2f;
                            max = 3f;
                            add = 1.5f;
                        }
                        var gotoVelo = new Vector2(Main.rand.NextFloat(min, max) + add * (1f - NPC.life / NPC.lifeMax), 0f).RotatedBy((Main.player[NPC.target].Center - center).ToRotation());
                        NPC.ai[2] = gotoVelo.X;
                        NPC.ai[3] = gotoVelo.Y;
                    }
                    else
                    {
                        NPC.ai[0] = 0f;
                        return;
                    }
                }
                else
                {
                    var gotoVelo = new Vector2(NPC.ai[2], NPC.ai[3]);
                    float length = gotoVelo.Length();
                    NPC.velocity = Vector2.Normalize(Vector2.Lerp(NPC.velocity, gotoVelo, 0.05f)) * length;
                    bool xCloseEnough = (NPC.velocity.X - gotoVelo.X).Abs() < 0.1f;
                    bool yCloseEnough = (NPC.velocity.Y - gotoVelo.Y).Abs() < 0.1f;
                    if (Main.netMode != NetmodeID.MultiplayerClient && xCloseEnough && yCloseEnough)
                    {
                        NPC.velocity.X = gotoVelo.X;
                        NPC.velocity.Y = gotoVelo.Y;
                        NPC.ai[0] = 1f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                        NPC.netUpdate = true;
                    }
                }
            }
        }

        private void OnHit(int plr, int damage)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.ai[1] = MathHelper.Clamp(NPC.ai[1] + Main.rand.Next(30, 50), 0f, 160f);
                NPC.localAI[0] = MathHelper.Clamp(damage * 8, 0f, 800f);
                if (plr > 0 && plr < 255)
                    NPC.target = plr;
                NPC.netUpdate = true;
            }
        }

        public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
        {
            OnHit(player.whoAmI, damage);
        }

        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
            OnHit(projectile.owner, damage);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.expertMode)
            {
                target.AddBuff(ModContent.BuffType<BlueFire>(), 60);
                target.AddBuff(BuffID.Blackout, 300);
            }
            else
            {
                if (Main.rand.NextBool(4))
                    target.AddBuff(BuffID.OnFire, 60);
                if (Main.rand.NextBool())
                    target.AddBuff(BuffID.Darkness, 300);
            }
        }

        public override bool CheckDead()
        {
            if (NPC.ai[0] == -1f)
                return true;
            NPC.ai[0] = -1f;
            NPC.ai[1] = 0f;
            NPC.ai[2] = 0f;
            NPC.ai[3] = 0f;
            NPC.velocity *= 0.5f;
            NPC.dontTakeDamage = true;
            NPC.life = NPC.lifeMax;
            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!Main.dayTime && spawnInfo.Player.position.Y < Main.worldSurface * 16f)
                return 0.005f;
            return 0f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            var offset = new Vector2(NPC.width / 2f, NPC.height / 2f);
            Vector2 origin = NPC.frame.Size() / 2f;
            Vector2 drawPos = NPC.Center - screenPos;
            float mult = 1f / NPCID.Sets.TrailCacheLength[NPC.type];
            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Main.spriteBatch.Draw(texture, NPC.oldPos[i] + offset - Main.screenPosition, NPC.frame, new Color(80, 80, 80, 0) * (mult * (NPCID.Sets.TrailCacheLength[NPC.type] - i)), NPC.oldRot[i], origin, NPC.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, drawPos, NPC.frame, new Color(255, 255, 255, 255), NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPos, NPC.frame, new Color(20, 20, 20, 0), NPC.rotation, origin, NPC.scale + 0.1f, SpriteEffects.None, 0f);

            if ((int)NPC.ai[0] == -1)
            {
                float scale = (float)Math.Pow(Math.Min(NPC.scale * (-NPC.ai[3] / 60f), 1f), 3f) * 1.25f;
                var shineColor = new Color(120, 120, 180, 0) * scale * NPC.Opacity;

                var lightRay = ModContent.Request<Texture2D>(Aequus.AssetsPath + "LightRay").Value;
                var lightRayOrigin = lightRay.Size() / 2f;

                int i = 0;
                foreach (float f in AequusHelpers.Circular(8, Main.GlobalTimeWrappedHourly * 0.8f + (int)(NPC.position.X * 2f + NPC.position.Y * 2f)))
                {
                    var rayScale = new Vector2(AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 0.8f + (int)(NPC.position.X + NPC.position.Y) + i * (int)(NPC.position.Y), 0.3f, 1f));
                    rayScale.X *= 0.5f;
                    rayScale.X *= (float)Math.Pow(scale, Math.Min(rayScale.Y, 1f));
                    Main.spriteBatch.Draw(lightRay, drawPos, null, shineColor * scale * NPC.Opacity, f, lightRayOrigin, scale * rayScale, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(lightRay, drawPos, null, shineColor * 0.5f * scale * NPC.Opacity, f, lightRayOrigin, scale * rayScale * 2f, SpriteEffects.None, 0f);
                    i++;
                }

                var spotlightTexture = ModContent.Request<Texture2D>(Aequus.AssetsPath + "Bloom_20x20").Value;
                var spotlightOrigin = spotlightTexture.Size() / 2f;
                Main.spriteBatch.Draw(spotlightTexture, drawPos, null, shineColor * scale * NPC.Opacity, 0f, spotlightOrigin, scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlightTexture, drawPos, null, shineColor * 0.5f * scale * NPC.Opacity, 0f, spotlightOrigin, scale * 2f, SpriteEffects.None, 0f);

                Main.instance.LoadProjectile(ProjectileID.RainbowCrystalExplosion);
                var shine = TextureAssets.Projectile[ProjectileID.RainbowCrystalExplosion].Value;
                var shineOrigin = shine.Size() / 2f;
                Main.EntitySpriteDraw(shine, drawPos, null, shineColor, 0f, shineOrigin, new Vector2(NPC.scale * 0.5f, NPC.scale) * scale, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(shine, drawPos, null, shineColor, MathHelper.PiOver2, shineOrigin, new Vector2(NPC.scale * 0.5f, NPC.scale * 2f) * scale, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}