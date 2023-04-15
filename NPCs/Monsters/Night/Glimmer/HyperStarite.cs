using Aequus.Buffs.Debuffs;
using Aequus.Common.Effects;
using Aequus.Common.Primitives;
using Aequus.Content.Critters;
using Aequus.Content.Events.GlimmerEvent;
using Aequus.Items.Materials;
using Aequus.Items.Potions;
using Aequus.Particles;
using Aequus.Particles.Dusts;
using Aequus.Projectiles.Monster;
using Aequus.Tiles.Banners.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters.Night.Glimmer {
    public class HyperStarite : ModNPC
    {
        public const int STATE_ARMS_IN = 4;
        public const int STATE_ARMS_OUT = 3;
        public const int STATE_CHASE = 2;
        public const int STATE_FLYUP = 1;
        public const int STATE_IDLE = 0;
        public const int STATE_GOODBYE = -1;
        public const int STATE_DEAD = -2;

        public static readonly Color SpotlightColor = new Color(100, 100, 10, 0);

        public int State { get => (int)NPC.ai[0]; set => NPC.ai[0] = value; }
        public float ArmsLength { get => NPC.ai[3]; set => NPC.ai[3] = value; }

        public float[] oldArmsLength;
        public TrailRenderer armTrail;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 3;
            NPCID.Sets.TrailingMode[Type] = 7;
            NPCID.Sets.TrailCacheLength[Type] = 15;
            ItemID.Sets.KillsToBanner[BannerItem] = 25;
            NPCID.Sets.DebuffImmunitySets.Add(Type, new Terraria.DataStructures.NPCDebuffImmunityData()
            {
                SpecificallyImmuneTo = Starite.DefaultBuffImmunities(),
            });
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Scale = 0.6f,
            });
            SnowgraveCorpse.NPCBlacklist.Add(Type);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            this.CreateLoot(npcLoot)
                .Add<StariteMaterial>(chance: 1, stack: (2, 4))
                .Add(ItemID.Megaphone, chance: 50, stack: 1)
                .Add<NeutronYogurt>(chance: 1, stack: (1, 2));
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry);
        }

        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 50;
            NPC.lifeMax = 200;
            NPC.damage = 80;
            NPC.defense = 8;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath55;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(silver: 30);
            NPC.npcSlots = 3f;

            this.SetBiome<GlimmerBiomeManager>();

            Banner = NPC.type;
            BannerItem = ModContent.ItemType<HyperStariteBanner>();

            oldArmsLength = new float[NPCID.Sets.TrailCacheLength[Type]];
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: balance -> balance (bossAdjustment is different, see the docs for details) */
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.9f * numPlayers);
            NPC.damage = (int)(NPC.damage * 0.66f);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            float x = NPC.velocity.X.Abs() * hit.HitDirection;
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server && State == STATE_DEAD)
                {
                    ScreenShake.SetShake(15, multiplier: 0.9f, where: NPC.Center);
                    ScreenFlash.Flash.Set(NPC.Center, 0.1f);
                }
                for (int i = 0; i < 50; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.MagicMirror);
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                }
                for (int i = 0; i < 70; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 57 + Main.rand.Next(2));
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                }
                for (int i = 0; i < 16; i++)
                {
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, new Vector2(Main.rand.NextFloat(-5f, 5f) + x, Main.rand.NextFloat(-5f, 5f)), 16 + Main.rand.Next(2));
                }
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.MagicMirror);
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(5f, 12f);
                }
                int d1 = Dust.NewDust(NPC.position, NPC.width, NPC.height, 57 + Main.rand.Next(2));
                Main.dust[d1].velocity.X += x;
                Main.dust[d1].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.NextFloat(-4f, 4f) + x * 0.75f, Main.rand.NextFloat(-4f, 4f)), 16 + Main.rand.Next(2));
            }
        }

        private bool PlayerCheck()
        {
            NPC.TargetClosest(faceTarget: false);
            if (!NPC.HasValidTarget || Main.player[NPC.target].dead)
            {
                NPC.ai[0] = -1f;
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void AI()
        {
            var aequus = NPC.Aequus();
            if (State == STATE_DEAD)
            {
                if (NPC.localAI[0] == 0)
                {
                    NPC.localAI[0] = Main.rand.Next(100);
                }
                NPC.velocity *= 0.97f;
                NPC.rotation += 0.1f * (1f + NPC.ai[2] / 60f);
                if (NPC.ai[2] > 0f)
                    NPC.ai[2] = 0f;
                NPC.ai[2] -= 1f - NPC.ai[2] / 60f;
                for (int i = 0; i < Main.rand.Next(2, 6); i++)
                {
                    var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Unit() * NPC.ai[2] * Main.rand.NextFloat(0.2f, 1f) * 3f, ModContent.DustType<MonoDust>(), newColor: Color.Lerp(new Color(255, 20, 100), new Color(255, 150, 250), Math.Min(Main.rand.NextFloat(1f) - NPC.ai[2] / 60f, 1f)).UseA(0));
                    d.velocity *= 0.2f;
                    d.velocity += (NPC.Center - d.position) / 8f;
                    d.scale = Main.rand.NextFloat(0.3f, 2f);
                    d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                }
                if (NPC.ai[2] < -60f)
                {
                    for (int i = 0; i < 60; i++)
                    {
                        var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Unit() * 50f * Main.rand.NextFloat(0.01f, 1f), ModContent.DustType<MonoDust>(), newColor: Color.Lerp(new Color(255, 20, 100), new Color(255, 150, 250), Main.rand.NextFloat(1f)).UseA(0));
                        d.velocity *= 0.2f;
                        d.velocity += (d.position - NPC.Center) / 2f;
                        d.scale = Main.rand.NextFloat(0.3f, 2.5f);
                        d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    }
                    NPC.life = -33333;
                    NPC.HitEffect();
                    NPC.checkDead();
                }
                return;
            }

            if (Main.dayTime && State != STATE_DEAD && aequus.lastHit > 60)
            {
                aequus.noOnKill = true;
            }
            else
            {
                aequus.noOnKill = false;
            }

            if (Main.rand.NextBool(8))
            {
                var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Enchanted_Pink);
                d.velocity = (d.position - NPC.Center) / 8f;
            }
            if (Main.rand.NextBool(10))
            {
                var g = Gore.NewGoreDirect(NPC.GetSource_FromThis(), NPC.position + new Vector2(Main.rand.Next(NPC.width - 4), Main.rand.Next(NPC.height - 4)), new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f)), 16);
                g.velocity = (g.position - NPC.Center) / 8f;
                g.scale *= 0.6f;
            }
            Lighting.AddLight(NPC.Center, new Vector3(1.2f, 1.2f, 0.5f));
            Vector2 center = NPC.Center;
            if (NPC.ai[0] == -1f)
            {
                NPC.noTileCollide = true;
                NPC.velocity.X *= 0.95f;
                if (NPC.velocity.Y > 0f)
                    NPC.velocity.Y *= 0.96f;
                NPC.velocity.Y -= 0.075f;

                NPC.timeLeft = Math.Min(NPC.timeLeft, 100);
                NPC.rotation += NPC.velocity.Length() * 0.0157f;
                return;
            }

            Player player = Main.player[NPC.target];
            Vector2 plrCenter = player.Center;
            float armsWantedLength = 320f;
            oldArmsLength[0] = NPC.ai[3];
            Helper.UpdateCacheList(oldArmsLength);
            switch (State)
            {
                case STATE_IDLE:
                    {
                        NPC.TargetClosest(faceTarget: false);
                        if (NPC.HasValidTarget)
                        {
                            if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height) || NPC.life < NPC.lifeMax)
                            {
                                NPC.ai[0] = STATE_FLYUP;
                                NPC.ai[1] = 0f;
                                for (int i = 0; i < 5; i++)
                                {
                                    int damage = Main.expertMode ? 30 : 55;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), center, new Vector2(0f, 0f), ModContent.ProjectileType<HyperStariteProj>(), damage, 1f, Main.myPlayer, NPC.whoAmI + 1, i);
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                NPC.ai[1]++;
                                if (NPC.ai[1] >= 1200f)
                                {
                                    NPC.timeLeft = 0;
                                    NPC.ai[0] = -1f;
                                }
                                NPC.velocity *= 0.96f;
                                return;
                            }
                        }
                        else
                        {
                            if (Main.player[NPC.target].dead)
                            {
                                NPC.ai[0] = -1f;
                                NPC.ai[1] = -0f;
                                NPC.netUpdate = true;
                            }
                            NPC.ai[1]++;
                            if (NPC.ai[1] >= 1200f)
                            {
                                NPC.timeLeft = 0;
                                NPC.ai[0] = -1f;
                                NPC.netUpdate = true;
                            }
                            NPC.velocity *= 0.96f;
                            return;
                        }
                    }
                    break;

                case STATE_FLYUP:
                    {
                        NPC.ai[1]++;
                        NPC.velocity.Y -= 0.45f;
                        if (NPC.ai[1] > 20f && PlayerCheck())
                        {
                            State = STATE_CHASE;
                            NPC.ai[1] = 0f;
                        }
                    }
                    break;

                case STATE_CHASE:
                    {
                        if (!PlayerCheck())
                        {
                            return;
                        }
                        NPC.ai[1]++;
                        if (NPC.ai[1] < 50f)
                        {
                            NPC.velocity *= 0.96f;
                        }
                        else
                        {
                            float wantedDistance = 500f;
                            var difference = Main.player[NPC.target].Center - NPC.Center;
                            if (difference.Length() > wantedDistance)
                            {
                                NPC.velocity = Vector2.Lerp(NPC.velocity, difference / 100f, 0.035f);
                            }
                            else
                            {
                                NPC.velocity *= 0.96f;
                                NPC.ai[1] += 4;
                            }
                        }
                        if (NPC.ai[1] > 600f)
                        {
                            State = STATE_ARMS_OUT;
                            NPC.ai[1] = 0f;
                        }
                        NPC.rotation += 0.01f + NPC.velocity.Length() * 0.01f;
                    }
                    break;

                case STATE_ARMS_OUT:
                    {
                        if (NPC.velocity.Length() > 1f)
                            NPC.velocity *= 0.9f;
                        NPC.ai[1]++;
                        float progress = MathHelper.Clamp(NPC.ai[1] / 120f, 0f, 1f);
                        ArmsLength = (float)Math.Sin(progress * MathHelper.Pi * (2 - MathHelper.Pi / (MathHelper.TwoPi + 2f)) - MathHelper.Pi) * armsWantedLength;
                        if (ArmsLength < 0f)
                            ArmsLength /= 25f * (1f - progress * progress);
                        NPC.rotation += Math.Max(0.06f * progress, 0.01f) + NPC.velocity.Length() * 0.01f;

                        if (NPC.ai[1] > 300f)
                        {
                            State = STATE_ARMS_IN;
                            NPC.ai[1] = 0f;
                        }
                    }
                    break;

                case STATE_ARMS_IN:
                    {
                        NPC.ai[1]++;
                        if (ArmsLength <= 0.1f)
                        {
                            ArmsLength = 0f;
                        }
                        float progress = NPC.ai[1] / 90f;
                        ArmsLength *= 0.995f - progress * 0.1f;
                        ArmsLength -= 2f;
                        NPC.rotation += Math.Max(0.06f * (1f - progress), 0.01f) + NPC.velocity.Length() * 0.01f;
                        if (NPC.ai[1] > 90f)
                        {
                            State = STATE_CHASE;
                            NPC.ai[1] = 0f;
                        }
                    }
                    break;
            }
            if (NPC.velocity.Length() < 1.5f && center.Y + 160f > plrCenter.Y && Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                NPC.velocity.Y -= 0.6f;
        }

        public override void UpdateLifeRegen(ref int damage)
        {
            if (Main.dayTime && State != STATE_DEAD && !Helper.ShadedSpot(NPC.Center))
            {
                NPC.lifeRegen = -50;
                damage = 8;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (Main.rand.NextBool(Main.expertMode ? 1 : 2))
                target.AddBuff(ModContent.BuffType<BlueFire>(), 240);
            if (Main.rand.NextBool(Main.expertMode ? 1 : 4))
                target.AddBuff(BuffID.Blackout, 600);
            if (Main.rand.NextBool(Main.expertMode ? 4 : 12))
                target.AddBuff(BuffID.Silenced, 120);
        }

        public override bool CheckDead()
        {
            if (State == STATE_DEAD)
                return true;
            State = STATE_DEAD;
            NPC.ai[1] = 0f;
            NPC.ai[2] = 0f;
            NPC.velocity *= 0.5f;
            NPC.dontTakeDamage = true;
            NPC.life = NPC.lifeMax;
            return false;
        }

        public override void OnKill()
        {
            AequusWorld.MarkAsDefeated(ref AequusWorld.downedHyperStarite, Type);
            NPC.NewNPCDirect(NPC.GetSource_Death(), NPC.Center, ModContent.NPCType<DwarfStarite>());
        }

        public override int SpawnNPC(int tileX, int tileY)
        {
            return NPC.NewNPC(null, tileX * 16 + 8, tileY * 16 - 80, NPC.type);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (armTrail == null)
                armTrail = TrailRenderer.NewRenderer(1, 50f, Color.Blue.UseA(0));
            var texture = TextureAssets.Npc[Type].Value;
            var origin = NPC.frame.Size() / 2f;
            var offset = new Vector2(NPC.width / 2f, NPC.height / 2f);
            float mult = 1f / NPCID.Sets.TrailCacheLength[NPC.type];
            var armFrame = NPC.frame;
            var coreFrame = new Rectangle(NPC.frame.X, NPC.frame.Y + NPC.frame.Height * 2, NPC.frame.Width, NPC.frame.Height);
            var bloom = AequusTextures.Bloom0;
            var bloomFrame = new Rectangle(0, 0, bloom.Width, bloom.Height);
            var bloomOrigin = bloomFrame.Size() / 2f;

            var armLength = (NPC.height + 56f) * NPC.scale;
            if (NPC.IsABestiaryIconDummy)
            {
                armLength -= 24f * NPC.scale;
            }

            bool dying = State == STATE_DEAD;
            Main.spriteBatch.Draw(bloom, new Vector2((int)(NPC.position.X + offset.X - screenPos.X), (int)(NPC.position.Y + offset.Y - screenPos.Y)), bloomFrame, SpotlightColor, 0f, bloomOrigin, NPC.scale * 2, SpriteEffects.None, 0f);
            if (!dying && !NPC.IsABestiaryIconDummy)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin_World(shader: true);

                int trailLength = NPCID.Sets.TrailCacheLength[Type];
                int armTrailLength = (int)(trailLength * MathHelper.Clamp((float)Math.Pow(ArmsLength / 240f, 1.2f), 0f, 1f));
                var armPositions = new List<Vector2>[5];

                for (int j = 0; j < 5; j++)
                    armPositions[j] = new List<Vector2>();

                for (int i = 0; i < trailLength; i++)
                {
                    var pos = NPC.oldPos[i] + offset - screenPos;
                    float progress = Helper.CalcProgress(trailLength, i);
                    Color color = new Color(45, 35, 60, 0) * (mult * (NPCID.Sets.TrailCacheLength[NPC.type] - i));
                    Main.spriteBatch.Draw(texture, pos.Floor(), coreFrame, color, 0f, origin, NPC.scale * progress * progress, SpriteEffects.None, 0f);
                    color = new Color(30, 25, 140, 4) * (mult * (NPCID.Sets.TrailCacheLength[NPC.type] - i)) * 0.6f;
                    if (i > armTrailLength || (i > 1 && (NPC.oldRot[i] - NPC.oldRot[i - 1]).Abs() < 0.002f))
                        continue;
                    for (int j = 0; j < 5; j++)
                    {
                        float rotation = NPC.oldRot[i] + MathHelper.TwoPi / 5f * j;
                        var armPos = NPC.position + offset + (rotation - MathHelper.PiOver2).ToRotationVector2() * (armLength + oldArmsLength[i]) - screenPos;
                        armPositions[j].Add(armPos + screenPos);
                        //Main.spriteBatch.Draw(texture, armPos.Floor(), armFrame, color, rotation, origin, NPC.scale, SpriteEffects.None, 0f);
                    }
                }

                for (int j = 0; j < 5; j++)
                    armTrail.Draw(armPositions[j].ToArray());

                Main.spriteBatch.End();
                Main.spriteBatch.Begin_World(shader: false); ;
            }
            var armSegmentFrame = new Rectangle(NPC.frame.X, NPC.frame.Y + NPC.frame.Height, NPC.frame.Width, NPC.frame.Height);

            float segmentLength = (NPC.height - 10f) * NPC.scale;
            if (NPC.IsABestiaryIconDummy)
            {
                segmentLength -= 10f * NPC.scale;
            }
            for (int i = 0; i < 5; i++)
            {
                float rotation = NPC.rotation + MathHelper.TwoPi / 5f * i;
                if (dying)
                    rotation += Main.rand.NextFloat(-0.2f, 0.2f) * Main.rand.NextFloat(NPC.ai[2] / 60f);
                var n = (rotation - MathHelper.PiOver2).ToRotationVector2();
                var armPos = NPC.position + offset + n * (armLength + NPC.ai[3]) - screenPos;
                if (dying)
                {
                    armPos += new Vector2(Main.rand.NextFloat(NPC.ai[2] / 8f), Main.rand.NextFloat(NPC.ai[2] / 8f));
                    armPos += n * NPC.ai[2] / 2f;
                }
                Main.spriteBatch.Draw(texture, armPos.Floor(), armFrame, Color.White, rotation, origin, NPC.scale, SpriteEffects.None, 0f);

                rotation += MathHelper.TwoPi / 10f;
                armPos = NPC.position + offset + (rotation - MathHelper.PiOver2).ToRotationVector2() * segmentLength - screenPos;
                if (dying)
                    armPos += new Vector2(Main.rand.NextFloat(NPC.ai[2] / 4f), Main.rand.NextFloat(NPC.ai[2] / 4f));
                Main.spriteBatch.Draw(texture, armPos.Floor(), armSegmentFrame, Color.White, rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            }
            if (dying)
                offset += new Vector2(Main.rand.NextFloat(NPC.ai[2] / 4f), Main.rand.NextFloat(NPC.ai[2] / 4f));

            Main.spriteBatch.Draw(texture, new Vector2((int)(NPC.position.X + offset.X - screenPos.X), (int)(NPC.position.Y + offset.Y - screenPos.Y)), coreFrame, new Color(255, 255, 255, 255), 0f, origin, NPC.scale, SpriteEffects.None, 0f);
            if (dying)
            {
                DrawDeathExplosion(NPC.position + offset - screenPos);
            }
            return false;
        }

        public void DrawDeathExplosion(Vector2 drawPos)
        {
            float scale = (float)Math.Min(NPC.scale * (-NPC.ai[2] / 60f), 1f) * 3f;
            var shineColor = new Color(200, 40, 150, 0) * scale * NPC.Opacity;

            var lightRay = ModContent.Request<Texture2D>(Aequus.AssetsPath + "LightRay", AssetRequestMode.ImmediateLoad).Value;
            var lightRayOrigin = lightRay.Size() / 2f;

            var r = LegacyEffects.EffectRand;
            int seed = r.SetRand((int)NPC.localAI[0]);
            int i = 0;
            foreach (float f in Helper.Circular((int)(6 + r.Rand(4)), Main.GlobalTimeWrappedHourly * 1.8f + NPC.localAI[0]))
            {
                var rayScale = new Vector2(Helper.Wave(r.Rand(MathHelper.TwoPi) + Main.GlobalTimeWrappedHourly * r.Rand(1f, 5f) * 0.5f, 0.3f, 1f) * r.Rand(0.5f, 2.25f));
                rayScale.X *= 0.05f;
                rayScale.X *= (float)Math.Pow(scale, Math.Min(rayScale.Y, 1f));
                Main.spriteBatch.Draw(lightRay, drawPos, null, shineColor * scale * NPC.Opacity, f, lightRayOrigin, scale * rayScale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(lightRay, drawPos, null, shineColor * 0.5f * scale * NPC.Opacity, f, lightRayOrigin, scale * rayScale * 2f, SpriteEffects.None, 0f);
                i++;
            }
            r.SetRand(seed);
            var bloom = AequusTextures.Bloom2;
            var bloomOrigin = bloom.Size() / 2f;
            scale *= 0.7f;
            Main.spriteBatch.Draw(bloom, drawPos, null, shineColor * scale * NPC.Opacity, 0f, bloomOrigin, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(bloom, drawPos, null, shineColor * 0.5f * scale * NPC.Opacity, 0f, bloomOrigin, scale * 1.4f, SpriteEffects.None, 0f);

            Main.instance.LoadProjectile(ProjectileID.RainbowCrystalExplosion);
            var shine = TextureAssets.Projectile[ProjectileID.RainbowCrystalExplosion].Value;
            var shineOrigin = shine.Size() / 2f;
            Main.EntitySpriteDraw(shine, drawPos, null, shineColor, 0f, shineOrigin, new Vector2(NPC.scale * 0.5f, NPC.scale) * scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(shine, drawPos, null, shineColor, MathHelper.PiOver2, shineOrigin, new Vector2(NPC.scale * 0.5f, NPC.scale * 2f) * scale, SpriteEffects.None, 0);
        }
    }
}