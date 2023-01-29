using Aequus;
using Aequus.Biomes;
using Aequus.Buffs.Debuffs;
using Aequus.Graphics;
using Aequus.Items.Accessories;
using Aequus.Items.Consumables.BuffPotions;
using Aequus.Items.Misc;
using Aequus.Items.Placeable.Banners;
using Aequus.Items.Weapons.Magic;
using Aequus.Items.Weapons.Melee;
using Aequus.Items.Weapons.Summon.Minion;
using Aequus.Particles;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters.Night.Glimmer
{
    public class Starite : ModNPC
    {
        public bool fallenStar;
        public int fallenStarPulseDir;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[Type] = 7;
            NPCID.Sets.TrailCacheLength[Type] = 12;
            NPCID.Sets.DebuffImmunitySets.Add(Type, new Terraria.DataStructures.NPCDebuffImmunityData()
            {
                SpecificallyImmuneTo = DefaultBuffImmunities(),
            });
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Position = new Vector2(0f, 17f),
                PortraitPositionYOverride = 36,
            });
            SnowgraveCorpse.NPCBlacklist.Add(Type);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            this.CreateLoot(npcLoot)
                .AddOptions(chance: 50, DefaultItemDrops())
                .Add<CelesitalEightBall>(chance: 50, stack: 1)
                .Add<NeutronYogurt>(chance: 5, stack: 1);
        }

        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 20;
            NPC.lifeMax = 35;
            NPC.damage = 45;
            NPC.defense = 3;
            NPC.HitSound = SoundID.NPCHit39;
            NPC.DeathSound = SoundID.NPCDeath55;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 1.1f;
            NPC.value = Item.buyPrice(silver: 2);
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<StariteBanner>();

            this.SetBiome<GlimmerBiome>();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.damage = (int)(NPC.damage * 0.75f);
        }

        public override bool? CanBeHitByItem(Player player, Item item)
        {
            return fallenStar ? null : false;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            return fallenStar ? null : false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            float x = NPC.velocity.X.Abs() * hitDirection;
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 25; i++)
                {
                    var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Unit() * Main.rand.Next(2, 32), ModContent.DustType<MonoDust>(), newColor: Color.Lerp(Color.Yellow.UseB(128), Color.White, Main.rand.NextFloat(0.2f, 1f)).UseA(0));
                    d.velocity *= 0.2f;
                    d.velocity += (d.position - NPC.Center) / 6f;
                }
                for (int i = 0; i < 30; i++)
                {
                    var b = new BloomParticle(NPC.Center + Main.rand.NextVector2Unit() * Main.rand.Next(2, 12), Vector2.Zero, Color.White.UseA(0), new Color(25, 25, 40, 0), Main.rand.NextFloat(0.8f, 1.45f), 0.33f);
                    b.Velocity += (b.Position - NPC.Center) / 2f;
                    EffectsSystem.ParticlesAbovePlayers.Add(b);
                }
                for (int i = 0; i < 25; i++)
                {
                    var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Unit() * Main.rand.Next(2, 32), DustID.Enchanted_Gold + Main.rand.Next(2), newColor: Color.White.UseA(0));
                    d.velocity *= 0.1f;
                    d.velocity += (d.position - NPC.Center) / 6f;
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Pixie);
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
            if (!fallenStar)
            {
                FallenStarAI();
                return;
            }
            NPC.noTileCollide = false;
            if (Main.dayTime)
            {
                NPC.life = -1;
                NPC.HitEffect();
                NPC.active = false;
                return;
            }
            if (NPC.justHit)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.ai[1] = MathHelper.Clamp(NPC.ai[1] + Main.rand.Next(30, 50), 0f, 160f);
                NPC.localAI[0] = 180f;
                NPC.TargetClosest(faceTarget: true);
                NPC.netUpdate = true;
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
        public void FallenStarAI()
        {
            if (!NPC.noGravity)
            {
                NPC.velocity.X *= 0.97f;
                NPC.velocity.Y -= 0.2f;
                if (NPC.velocity.X.Abs() < 0.1f)
                {
                    NPC.velocity.X = 0f;
                    NPC.noGravity = true;
                    NPC.netUpdate = true;
                    fallenStar = true;
                }
                NPC.rotation = NPC.velocity.X * 0.2f;
                return;
            }

            if (NPC.velocity == Vector2.Zero)
            {
                NPC.oldVelocity = NPC.velocity;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.TargetClosest(faceTarget: false);
                    float y;
                    if (NPC.HasValidTarget)
                    {
                        y = Main.player[NPC.target].position.Y;
                    }
                    else
                    {
                        y = NPC.position.Y;
                    }
                    NPC.position.Y = y - 1000f;
                    NPC.velocity = (Main.rand.NextFloat(MathHelper.PiOver4) + MathHelper.PiOver4 * 1.5f).ToRotationVector2() * 15f;
                    NPC.netUpdate = true;
                }
            }

            if (NPC.noTileCollide)
            {
                bool noTileCollide = NPC.noTileCollide;
                NPC.noTileCollide = Collision.SolidCollision(NPC.position, NPC.width, NPC.height);
                if (noTileCollide != NPC.noTileCollide)
                {
                    NPC.netUpdate = true;
                }
            }

            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 20 + Main.rand.Next(40);
                SoundEngine.PlaySound(SoundID.Item9, NPC.position);
            }

            NPC.alpha += (int)(25f * fallenStarPulseDir);
            if (NPC.alpha > 200)
            {
                NPC.alpha = 200;
                fallenStarPulseDir = -1;
            }
            if (NPC.alpha < 0)
            {
                NPC.alpha = 0;
                fallenStarPulseDir = 1;
            }

            NPC.direction = Math.Sign(NPC.velocity.X);
            NPC.rotation += (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) * 0.01f * NPC.direction;
            ScreenCulling.SetPadding();
            if (ScreenCulling.OnScreen(NPC.getRect()) && Main.rand.NextBool(6))
            {
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity * 0.2f, Utils.SelectRandom(Main.rand, 16, 17, 17, 17));
            }
            if (Main.rand.NextBool(20))
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Enchanted_Pink, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 150, default(Color), 1.2f);
            }
            Lighting.AddLight(NPC.Center, new Vector3(0.2f, 0.35f, 0.6f) * 0.9f);

            if (NPC.collideX || NPC.collideY)
            {
                FallenStarAI_OnTileCollide();
            }
        }
        public void FallenStarAI_OnTileCollide()
        {
            SoundEngine.PlaySound(SoundID.Item10, NPC.position);

            for (int i = 0; i < 7; i++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Enchanted_Pink, NPC.velocity.X * 0.1f, NPC.velocity.Y * 0.1f, 150, default(Color), 0.8f);
            }
            for (float f = 0f; f < 1f; f += 0.125f)
            {
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<MonoSparkleDust>(), Vector2.UnitY.RotatedBy(f * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (4f + Main.rand.NextFloat() * 4f), 150, new Color(40, 160, 255, 0)).noGravity = true;
            }
            for (float f = 0f; f < 1f; f += 0.25f)
            {
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<MonoSparkleDust>(), Vector2.UnitY.RotatedBy(f * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (2f + Main.rand.NextFloat() * 3f), 150, new Color(255, 150, 50, 0)).noGravity = true;
            }
            ScreenCulling.SetPadding();
            if (ScreenCulling.OnScreen(NPC.getRect()) && Main.rand.NextBool(6))
            {
                for (int i = 0; i < 7; i++)
                {
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, Main.rand.NextVector2CircularEdge(0.5f, 0.5f) * NPC.velocity.Length(), Utils.SelectRandom(Main.rand, 16, 17, 17, 17, 17, 17, 17, 17));
                }
            }
            fallenStarPulseDir = 0;
            NPC.rotation = 0f;
            NPC.netUpdate = true;
            NPC.velocity = -(Main.rand.NextFloat(MathHelper.PiOver2) + MathHelper.PiOver4).ToRotationVector2() * 3f;
            NPC.noGravity = false;
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
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!fallenStar && NPC.noGravity && !NPC.IsABestiaryIconDummy)
            {
                DrawFallenStarForm(spriteBatch, screenPos, drawColor);
                return false;
            }

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

                var lightRay = ModContent.Request<Texture2D>(Aequus.AssetsPath + "LightRay", AssetRequestMode.ImmediateLoad).Value;
                var lightRayOrigin = lightRay.Size() / 2f;

                int i = 0;
                foreach (float f in AequusHelpers.Circular(8, Main.GlobalTimeWrappedHourly * 0.8f + (int)(NPC.position.X * 2f + NPC.position.Y * 2f)))
                {
                    var rayScale = new Vector2(AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 0.8f + (int)(NPC.position.X + NPC.position.Y) + i * (int)NPC.position.Y, 0.3f, 1f));
                    rayScale.X *= 0.5f;
                    rayScale.X *= (float)Math.Pow(scale, Math.Min(rayScale.Y, 1f));
                    Main.spriteBatch.Draw(lightRay, drawPos, null, shineColor * scale * NPC.Opacity, f, lightRayOrigin, scale * rayScale, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(lightRay, drawPos, null, shineColor * 0.5f * scale * NPC.Opacity, f, lightRayOrigin, scale * rayScale * 2f, SpriteEffects.None, 0f);
                    i++;
                }

                var spotlightTexture = ModContent.Request<Texture2D>(Aequus.AssetsPath + "Bloom_20x20", AssetRequestMode.ImmediateLoad).Value;
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
        public void DrawFallenStarForm(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var projectileTexture = TextureAssets.Npc[Type].Value;
            var frame = new Rectangle(0, 0, projectileTexture.Width, projectileTexture.Height);
            var origin = frame.Size() / 2f;
            var alpha = Color.White;
            var trailThing = TextureAssets.Extra[ExtrasID.FallingStar].Value;
            var trailFrame = trailThing.Frame();
            var trailOrigin = new Vector2((float)trailFrame.Width / 2f, 10f);
            var gfxOff = new Vector2(0f, NPC.gfxOffY);
            var spinningpoint = new Vector2(0f, -10f);
            float visualEffectsTimer = Main.GlobalTimeWrappedHourly;
            var vector36 = NPC.Center + NPC.velocity;
            var trailColor = new Color(30, 80, 160, 0);
            var trailColorWhite = new Color(200, 255, 255, 255);
            trailColorWhite.A = 0;
            float num189 = 0f;
            var color45 = trailColor;
            color45.A = 0;
            var color46 = trailColor;
            color46.A = 0;
            var color47 = trailColor;
            color47.A = 0;
            Main.spriteBatch.Draw(trailThing, vector36 - Main.screenPosition + gfxOff + spinningpoint.RotatedBy((float)Math.PI * 2f * visualEffectsTimer), trailFrame, color45, NPC.velocity.ToRotation() + (float)Math.PI / 2f, trailOrigin, 1.5f + num189, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(trailThing, vector36 - Main.screenPosition + gfxOff + spinningpoint.RotatedBy((float)Math.PI * 2f * visualEffectsTimer + (float)Math.PI * 2f / 3f), trailFrame, color46, NPC.velocity.ToRotation() + (float)Math.PI / 2f, trailOrigin, 1.1f + num189, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(trailThing, vector36 - Main.screenPosition + gfxOff + spinningpoint.RotatedBy((float)Math.PI * 2f * visualEffectsTimer + 4.18879032f), trailFrame, color47, NPC.velocity.ToRotation() + (float)Math.PI / 2f, trailOrigin, 1.3f + num189, SpriteEffects.None, 0);
            var vector37 = NPC.Center - NPC.velocity * 0.5f;
            for (float num190 = 0f; num190 < 1f; num190 += 0.5f)
            {
                float num191 = visualEffectsTimer % 0.5f / 0.5f;
                num191 = (num191 + num190) % 1f;
                float num192 = num191 * 2f;
                if (num192 > 1f)
                {
                    num192 = 2f - num192;
                }
                Main.spriteBatch.Draw(trailThing, vector37 - Main.screenPosition + gfxOff, trailFrame, trailColorWhite * num192, NPC.velocity.ToRotation() + (float)Math.PI / 2f, trailOrigin, 0.3f + num191 * 0.5f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Draw(projectileTexture, NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY), frame, alpha, NPC.rotation, origin, NPC.scale + 0.1f, NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(fallenStar);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            fallenStar = reader.ReadBoolean();
        }

        public static int[] DefaultBuffImmunities()
        {
            return new int[]
                {
                    BuffID.Confused,
                    BuffID.OnFire,
                    BuffID.OnFire3,
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
                    ModContent.BuffType<BlueFire>(),
                    ModContent.BuffType<CrimsonHellfire>(),
                    ModContent.BuffType<CorruptionHellfire>(),
                };
        }

        public static int[] DefaultItemDrops()
        {
            return new int[]
                {
                    ModContent.ItemType<SuperStarSword>(),
                    ModContent.ItemType<Nightfall>(),
                    ModContent.ItemType<StariteStaff>(),
                    ModContent.ItemType<HyperCrystal>(),
                };
        }
    }
}