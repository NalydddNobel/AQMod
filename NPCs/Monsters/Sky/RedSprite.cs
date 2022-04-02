using Aequus.Assets.Effects;
using Aequus.Common;
using Aequus.Common.Configuration;
using Aequus.Common.ItemDrops;
using Aequus.Content.Invasions;
using Aequus.Items.Misc;
using Aequus.Items.Misc.Dyes;
using Aequus.Items.Misc.Energies;
using Aequus.Items.Placeable;
using Aequus.Particles.Dusts;
using Aequus.Projectiles.Monster;
using Aequus.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters.Sky
{
    [AutoloadBossHead()]
    public class RedSprite : ModNPC
    {
        private bool _setupFrame;
        public int frameIndex;
        private SoundEffectInstance WindSound;

        public const int FramesX = 3;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData()
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.OnFire,
                    BuffID.CursedInferno,
                    BuffID.Ichor,
                    BuffID.ShadowFlame,
                    BuffID.Bleeding,
                    BuffID.Frostburn,
                    BuffID.Frostburn2,
                }
            });
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
                new FlavorTextBestiaryInfoElement("Mods.Aequus.Bestiary.RedSprite")
            });
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(NPC.GetBestiaryCreditId(), true);
        }

        public override void SetDefaults()
        {
            NPC.width = 74;
            NPC.height = 50;
            NPC.lifeMax = 3250;
            NPC.damage = 45;
            NPC.defense = 15;
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit30;
            NPC.DeathSound = SoundID.NPCDeath33;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.value = Item.buyPrice(gold: 2);
            //banner = NPC.type;
            //bannerItem = ModContent.ItemType<RedSpriteBanner>();
            NPC.noTileCollide = true;

            //NPC.GetGlobalNPC<AQNPC>().temperature = 40;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8);
            //if (AQMod.calamityMod.IsActive)
            //{
            //    NPC.lifeMax = (int)(NPC.lifeMax * 2.5f);
            //    NPC.damage *= 2;
            //    NPC.defense *= 2;
            //}
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            var center = NPC.Center;
            if (NPC.life < 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    var soundEffect = SoundEngine.GetTrackableSoundByStyleId(SoundID.BlizzardStrongLoop.Style);
                    WindSound = soundEffect.CreateInstance();
                    if (WindSound.State == SoundState.Playing)
                    {
                        WindSound.Stop();
                    }
                }
                for (int i = 0; i < 50; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<RedSpriteDust>());
                    Main.dust[d].velocity = (Main.dust[d].position - center) / 8f;
                }
            }
            else
            {
                for (int i = 0; i < damage / 100; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<RedSpriteDust>());
                    Main.dust[d].velocity = (Main.dust[d].position - center) / 8f;
                }
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            if (drawColor.R < 60)
            {
                drawColor.R = 60;
            }
            if (drawColor.G < 60)
            {
                drawColor.G = 60;
            }
            if (drawColor.B < 60)
            {
                drawColor.B = 60;
            }
            return drawColor;
        }

        public const int PHASE_DIRECT_WIND = 1;
        public const int PHASE_SUMMON_NIMBUS = 2;
        public const int PHASE_DIRECT_WIND_Transition = 3;
        public const int PHASE_THUNDERCLAP = 4;
        public const int PHASE_THUNDERCLAP_Transition = 5;

        public override void AI()
        {
            bool leave = (int)NPC.ai[0] == -1;
            if (!leave && !GaleStreams.IsThisSpace(Main.player[NPC.target].position.Y))
            {
                leave = true;
            }
            else if ((int)NPC.ai[0] == 0)
            {
                NPC.TargetClosest(faceTarget: false);
                if (!NPC.HasValidTarget)
                {
                    NPC.ai[0] = -1;
                    leave = true;
                }
            }
            if (leave)
            {

                if (NPC.timeLeft < 20)
                {
                    NPC.timeLeft = 20;
                }
                NPC.velocity.Y -= 0.25f;
                return;
            }
            var center = NPC.Center;
            if ((int)NPC.ai[0] == 0)
            {
                if (!NPC.HasValidTarget)
                {
                    NPC.ai[0] = -1;
                    return;
                }
                RandomizePhase();
                NPC.velocity = -Vector2.Normalize(Main.player[NPC.target].Center - center) * 20f;
            }
            if (!NPC.HasValidTarget)
            {
                NPC.ai[0] = -1;
                return;
            }
            switch ((int)NPC.ai[0])
            {
                case PHASE_DIRECT_WIND:
                    {
                        if (NPC.direction == -1)
                        {
                            if (NPC.position.X > Main.player[NPC.target].position.X - 100)
                            {
                                if (NPC.velocity.X > -20f)
                                {
                                    NPC.velocity.X -= 0.8f;
                                    if (NPC.velocity.X > 0f)
                                    {
                                        NPC.velocity.X *= 0.96f;
                                    }
                                }
                            }
                            else
                            {
                                if (Main.player[NPC.target].position.X - NPC.position.X > 500)
                                {
                                    if (NPC.velocity.X < 20f)
                                    {
                                        NPC.velocity.X += 0.4f;
                                        if (NPC.velocity.X < 0f)
                                        {
                                            NPC.velocity.X *= 0.96f;
                                        }
                                    }
                                }
                                else
                                {
                                    NPC.ai[1] = 1f;
                                    if (NPC.velocity.X < -4f)
                                        NPC.velocity.X *= 0.94f;
                                }
                            }
                        }
                        else
                        {
                            if (NPC.position.X < Main.player[NPC.target].position.X + 100)
                            {
                                if (NPC.velocity.X < 20f)
                                {
                                    NPC.velocity.X += 0.8f;
                                    if (NPC.velocity.X < 0f)
                                    {
                                        NPC.velocity.X *= 0.96f;
                                    }
                                }
                            }
                            else
                            {
                                if (NPC.position.X - Main.player[NPC.target].position.X > 500)
                                {
                                    if (NPC.velocity.X > -20f)
                                    {
                                        NPC.velocity.X -= 0.4f;
                                        if (NPC.velocity.X > 0f)
                                        {
                                            NPC.velocity.X *= 0.96f;
                                        }
                                    }
                                }
                                else
                                {
                                    NPC.ai[1] = 1f;
                                    if (NPC.velocity.X > 4f)
                                        NPC.velocity.X *= 0.94f;
                                }
                            }
                        }
                        if ((int)NPC.ai[1] == 1)
                        {
                            NPC.ai[3]++;
                            if (NPC.ai[3] > 600f)
                            {
                                NPC.ai[0] = PHASE_DIRECT_WIND_Transition;
                                NPC.ai[2] = -1f;
                                NPC.ai[3] = 0f;
                            }
                            if (NPC.ai[2] < 0f)
                            {
                                NPC.ai[2] = 0f;
                            }

                            if (Main.netMode != NetmodeID.Server)
                            {
                                if (Main.player[Main.myPlayer].Distance(center) < 1000f)
                                {
                                    float x = Main.player[Main.myPlayer].position.X + Main.player[Main.myPlayer].width / 2f + (Main.screenWidth / 2f + 20) * NPC.direction;
                                    float y = Main.player[Main.myPlayer].position.Y + Main.player[Main.myPlayer].height / 2f - Main.screenHeight / 2f;
                                    for (int j = 0; j < 50; j++)
                                    {
                                        float y2 = y + Main.rand.NextFloat(Main.screenHeight);
                                        if (!Collision.SolidCollision(new Vector2(x - 1f, y2 - 1f), 2, 2))
                                        {
                                            y = y2;
                                            break;
                                        }
                                    }
                                    int d = Dust.NewDust(new Vector2(x - 1f, y - 1f), 2, 2, 268);
                                    Main.dust[d].velocity.X = -NPC.direction * 20f;
                                    Main.dust[d].velocity.Y = Main.rand.NextFloat(1f, 3f);
                                    Main.dust[d].color = new Color(255, 200, 10, 255);
                                    Main.dust[d].alpha = 100;
                                    Main.dust[d].scale = Main.rand.NextFloat(0.5f, 4f);
                                }
                            }

                            //for (int i = 0; i < Main.maxPlayers; i++)
                            //{
                            //    if (Main.player[i].active && !Main.player[i].dead && Main.player[i].Distance(center) < 2000f)
                            //    {
                            //        Main.player[i].AddBuff(ModContent.BuffType<Buffs.Debuffs.RedSpriteWind>(), 4);
                            //        var aQPlayer = Main.player[i].GetModPlayer<AQPlayer>();
                            //        aQPlayer.redSpriteWind = (sbyte)-NPC.direction;
                            //        if (NPC.direction == -1)
                            //        {
                            //            if (Main.player[NPC.target].velocity.X < 1f)
                            //            {
                            //                if (aQPlayer.temperatureRegen < 10 || aQPlayer.temperature == 0)
                            //                {
                            //                    if (Main.expertMode)
                            //                    {
                            //                        aQPlayer.InflictTemperature(10);
                            //                        aQPlayer.temperatureRegen = 20;
                            //                    }
                            //                    else
                            //                    {
                            //                        aQPlayer.InflictTemperature(4);
                            //                    }
                            //                }
                            //            }
                            //        }
                            //        else
                            //        {
                            //            if (Main.player[NPC.target].velocity.X > -1f)
                            //            {
                            //                if (aQPlayer.temperatureRegen < 10 || aQPlayer.temperature == 0)
                            //                {
                            //                    if (Main.expertMode)
                            //                    {
                            //                        aQPlayer.InflictTemperature(10);
                            //                        aQPlayer.temperatureRegen = 20;
                            //                    }
                            //                    else
                            //                    {
                            //                        aQPlayer.InflictTemperature(4);
                            //                    }
                            //                }
                            //            }
                            //        }
                            //    }
                            //}

                            if (NPC.direction == -1)
                            {
                                if (Main.player[NPC.target].velocity.X > 2f)
                                {
                                    NPC.ai[2]++;
                                    if (NPC.ai[2] > 180f)
                                    {
                                        NPC.TargetClosest(faceTarget: false);
                                        NPC.ai[2] = -1f;
                                        NPC.ai[1] = 0f;
                                        NPC.spriteDirection = NPC.direction;
                                        NPC.direction = 1;
                                    }
                                }
                            }
                            else
                            {
                                if (Main.player[NPC.target].velocity.X < -2f)
                                {
                                    NPC.ai[2]++;
                                    if (NPC.ai[2] > 180f)
                                    {
                                        NPC.TargetClosest(faceTarget: false);
                                        NPC.ai[2] = -1f;
                                        NPC.ai[1] = 0f;
                                        NPC.spriteDirection = NPC.direction;
                                        NPC.direction = -1;
                                    }
                                }
                            }

                            if (Main.netMode != NetmodeID.Server && Main.ambientVolume > 0f)
                            {
                                LoadWindSound();
                                if (WindSound.State != SoundState.Playing)
                                {
                                    WindSound.Volume = Main.ambientVolume;
                                    WindSound.Play();
                                }
                            }
                        }
                        NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, (Main.player[NPC.target].position.Y - 100 - center.Y) / 4f, 0.1f);
                        NPC.rotation = NPC.velocity.X * 0.01f;
                    }
                    break;

                case PHASE_DIRECT_WIND_Transition:
                    {
                        if (NPC.direction == -1)
                        {
                            if (NPC.position.X > Main.player[NPC.target].position.X - 100)
                            {
                                if (NPC.velocity.X > -20f)
                                {
                                    NPC.velocity.X -= 0.8f;
                                    if (NPC.velocity.X > 0f)
                                    {
                                        NPC.velocity.X *= 0.96f;
                                    }
                                }
                            }
                            else
                            {
                                if (Main.player[NPC.target].position.X - NPC.position.X > 500)
                                {
                                    if (NPC.velocity.X < 20f)
                                    {
                                        NPC.velocity.X += 0.4f;
                                        if (NPC.velocity.X < 0f)
                                        {
                                            NPC.velocity.X *= 0.96f;
                                        }
                                    }
                                }
                                else
                                {
                                    if (NPC.velocity.X < -4f)
                                        NPC.velocity.X *= 0.94f;
                                }
                            }
                        }
                        else
                        {
                            if (NPC.position.X < Main.player[NPC.target].position.X + 100)
                            {
                                if (NPC.velocity.X < 20f)
                                {
                                    NPC.velocity.X += 0.8f;
                                    if (NPC.velocity.X < 0f)
                                    {
                                        NPC.velocity.X *= 0.96f;
                                    }
                                }
                            }
                            else
                            {
                                if (NPC.position.X - Main.player[NPC.target].position.X > 500)
                                {
                                    if (NPC.velocity.X > -20f)
                                    {
                                        NPC.velocity.X -= 0.4f;
                                        if (NPC.velocity.X > 0f)
                                        {
                                            NPC.velocity.X *= 0.96f;
                                        }
                                    }
                                }
                                else
                                {
                                    if (NPC.velocity.X > 4f)
                                        NPC.velocity.X *= 0.94f;
                                }
                            }
                        }
                        NPC.ai[1]++;
                        if (Main.netMode != NetmodeID.Server && Main.ambientVolume > 0f)
                        {
                            LoadWindSound();
                            if (WindSound.State == SoundState.Playing)
                            {
                                if (NPC.ai[1] > 30f)
                                {
                                    WindSound.Stop();
                                }
                                else
                                {
                                    float volume = 1f - NPC.ai[1] / 30f;
                                    WindSound.Volume = Main.ambientVolume * volume;
                                }
                            }
                        }
                        if (NPC.ai[1] > (Main.expertMode ? 30f : 60f))
                        {
                            NPC.ai[1] = 0f;
                            RandomizePhase(PHASE_DIRECT_WIND);
                        }
                    }
                    break;

                case PHASE_SUMMON_NIMBUS:
                    {
                        NPC.direction = 0;
                        NPC.rotation = NPC.rotation.AngleLerp(0f, 0.1f);
                        Vector2 gotoPosition = new Vector2(Main.player[NPC.target].position.X + Main.player[NPC.target].width / 2f, Main.player[NPC.target].position.Y - 300f);
                        gotoPosition += new Vector2(40f, 0f).RotatedBy(NPC.ai[2]);
                        NPC.ai[2] += 0.02f;
                        if ((center - gotoPosition).Length() < 100f)
                        {
                            NPC.ai[1]++;
                            if (NPC.velocity.Length() > 5f)
                                NPC.velocity *= 0.96f;
                            if (NPC.ai[1] > 90f)
                            {
                                int cloudsAmount = 8;
                                int delayBetweenCloudSpawns = 20;
                                int cloudLifespan = 240;
                                if (Main.expertMode)
                                {
                                    delayBetweenCloudSpawns /= 2;
                                    cloudLifespan *= 2;
                                }
                                int timer = (int)(NPC.ai[1] - 90f) % 10;
                                NPC.localAI[2] = timer;
                                if (timer == 0 && (Main.expertMode || NPC.ai[1] <= 160f))
                                {
                                    SoundID.Item66?.Play(gotoPosition, 1.3f);
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        int timer2 = (int)(NPC.ai[1] - 90f) % 20;
                                        int direction = 1;
                                        if (timer2 >= 10)
                                        {
                                            direction = -1;
                                        }
                                        var projPosition = new Vector2(center.X + 900f * direction, center.Y + Main.rand.NextFloat(-120f, 60f));
                                        var velocity = Vector2.Normalize(Main.player[NPC.target].Center + new Vector2(0f, -160f) - projPosition);
                                        int damage = 50;
                                        if (Main.expertMode)
                                        {
                                            damage = 30;
                                        }
                                        int type = ModContent.ProjectileType<RedSpriteCloud>();
                                        int p = Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), projPosition, velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * 18f, type, damage, 1f, Main.myPlayer);
                                        Main.projectile[p].rotation = Main.projectile[p].velocity.ToRotation();
                                        Main.projectile[p].friendly = false;
                                        Main.projectile[p].hostile = true;
                                        Main.projectile[p].timeLeft = (int)(Vector2.Distance(projPosition, Main.player[NPC.target].Center) / Main.projectile[p].velocity.Length());
                                    }
                                }
                                if (NPC.ai[1] > delayBetweenCloudSpawns * cloudsAmount + 90f)
                                {
                                    NPC.ai[2] = -1f;
                                    NPC.localAI[2] = 0f;
                                    RandomizePhase(PHASE_SUMMON_NIMBUS);
                                    NPC.ai[2] = 0f;
                                }
                            }
                        }
                        else
                        {
                            NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(gotoPosition - center) * 20f, 0.1f);
                        }
                    }
                    break;

                case PHASE_THUNDERCLAP:
                    {
                        NPC.direction = 0;
                        NPC.rotation = NPC.rotation.AngleLerp(0f, 0.1f);
                        Vector2 gotoPosition = new Vector2(Main.player[NPC.target].position.X + Main.player[NPC.target].width / 2f, Main.player[NPC.target].position.Y - 300f);
                        var v = new Vector2(90f, 0f).RotatedBy(NPC.ai[2]);
                        v.X *= 2.5f;
                        gotoPosition += v;
                        NPC.ai[2] += 0.05f;
                        int timer = (int)(NPC.ai[1] - 90f) % 30;
                        if (NPC.ai[1] < 90f)
                        {
                            timer = 0;
                        }
                        else
                        {
                            if (NPC.ai[1] > 30 * 4 + 90f || !Main.expertMode && NPC.ai[1] >= 30 * 2 + 90f)
                            {
                                NPC.ai[2] = -1f;
                                NPC.localAI[2] = 0f;
                                RandomizePhase(PHASE_THUNDERCLAP);
                                NPC.ai[2] = 0f;
                            }
                            else
                            {
                                if (timer < 3
                                    && Main.netMode != NetmodeID.Server && ClientConfiguration.Instance.screenshakes &&
                                    (Main.myPlayer == NPC.target || Main.player[Main.myPlayer].Distance(center) < 1000f))
                                {
                                    GameEffects.Instance.SetFlash(NPC.Center, 0.75f, 10f);
                                }
                                if (timer == 0)
                                {
                                    if (Main.netMode != NetmodeID.Server && (Main.myPlayer == NPC.target || Main.player[Main.myPlayer].Distance(center) < 1000f))
                                    {
                                        if (ClientConfiguration.Instance.screenshakes)
                                        {
                                            GameEffects.Instance.SetShake(12f, 10f);
                                        }
                                        if (Main.netMode != NetmodeID.Server)
                                        {
                                            SoundHelper.Play(SoundType.Sound, "thunderclap" + Main.rand.Next(2), NPC.Center, 0.6f);
                                        }
                                        if (ClientConfiguration.Instance.effectQuality > 0.2f)
                                        {
                                            int dustAmount = 50;
                                            if (ClientConfiguration.Instance.effectQuality < 1f)
                                            {
                                                dustAmount = (int)(dustAmount * ClientConfiguration.Instance.effectQuality);
                                            }
                                            float rot = MathHelper.TwoPi / dustAmount;
                                            for (int i = 0; i < dustAmount; i++)
                                            {
                                                var normal = new Vector2(1f, 0f).RotatedBy(rot * i);
                                                int d = Dust.NewDust(center + normal * NPC.width, 2, 2, ModContent.DustType<RedSpriteDust>());
                                                Main.dust[d].velocity = normal * 12f;
                                            }
                                        }
                                    }
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        int damage = 100;
                                        if (Main.expertMode)
                                        {
                                            damage = 75;
                                        }
                                        Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), center + new Vector2(0f, 130f), Vector2.Zero, ModContent.ProjectileType<RedSpriteThunderClap>(), damage, 1f, Main.myPlayer);
                                    }
                                }
                            }
                        }
                        NPC.ai[1]++;
                        if (timer > 35)
                        {
                            NPC.velocity *= 0.5f;
                        }
                        else
                        {
                            if (timer < 16)
                            {
                                NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(gotoPosition - center) * 20f, 0.02f);
                            }
                        }
                    }
                    break;

                case PHASE_THUNDERCLAP_Transition:
                    {
                        Vector2 gotoPosition = new Vector2(Main.player[NPC.target].position.X + Main.player[NPC.target].width / 2f, Main.player[NPC.target].position.Y - 300f);
                        var v = new Vector2(90f, 0f).RotatedBy(NPC.ai[2]);
                        v.X *= 2.5f;
                        gotoPosition += v;
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(gotoPosition - center) * 20f, 0.02f);
                        NPC.ai[1]++;
                        if (NPC.ai[1] > (Main.expertMode ? 30f : 60f))
                        {
                            NPC.ai[1] = 0f;
                            RandomizePhase(PHASE_THUNDERCLAP);
                        }
                    }
                    break;
            }
        }

        private void RandomizePhase(int curPhase = -1)
        {
            NPC.TargetClosest(faceTarget: false);
            if (!NPC.HasValidTarget)
            {
                NPC.ai[0] = -1;
                return;
            }
            for (int i = 0; i < 50; i++)
            {
                NPC.ai[0] = Main.rand.Next(2) + 1;
                if (NPC.life * 2 < NPC.lifeMax && Main.rand.NextBool(4))
                {
                    NPC.ai[0] = PHASE_THUNDERCLAP;
                }
                if ((int)NPC.ai[0] != curPhase)
                {
                    break;
                }
            }
            NPC.ai[1] = 0f;
            NPC.netUpdate = true;
            frameIndex = 0;
            if ((int)NPC.ai[0] == 1)
            {
                NPC.direction = Main.rand.NextBool() ? -1 : 1;
            }
            NPC.spriteDirection = NPC.direction;
        }

        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            if (!_setupFrame)
            {
                _setupFrame = true;
                NPC.frame.Width = NPC.frame.Width / FramesX;
            }
            if (NPC.IsABestiaryIconDummy)
            {
                return;
            }

            switch ((int)NPC.ai[0])
            {
                default:
                    {
                        NPC.frameCounter += 1.0d;
                        if (NPC.frameCounter > 2.0d)
                        {
                            NPC.frameCounter = 0.0d;
                            frameIndex++;
                            if (frameIndex >= Main.npcFrameCount[NPC.type] * FramesX)
                            {
                                frameIndex = 0;
                            }
                        }
                    }
                    break;

                case -1:
                    frameIndex = 1;
                    break;

                case PHASE_DIRECT_WIND:
                    {
                        int direction = NPC.spriteDirection;
                        if (direction == -1)
                        {
                            if (direction != NPC.direction)
                            {
                                if (frameIndex != 0)
                                {
                                    NPC.frameCounter += 1.0d;
                                    if (NPC.frameCounter > 4.0)
                                    {
                                        NPC.frameCounter = 0.0;
                                        frameIndex--;
                                        if (frameIndex < 6)
                                        {
                                            frameIndex = 0;
                                            NPC.localAI[0] = 0f;
                                            NPC.localAI[1] = 0f;
                                            NPC.localAI[2] = 0f;
                                        }
                                    }
                                }
                                else
                                {
                                    NPC.spriteDirection = NPC.direction;
                                }
                            }
                            else
                            {
                                if (NPC.position.X < Main.player[NPC.target].position.X)
                                {
                                    NPC.frameCounter += 1.0d;
                                }
                                if (frameIndex == 0)
                                {
                                    if (NPC.frameCounter > 16.0)
                                    {
                                        frameIndex = 6;
                                    }
                                }
                                else
                                {
                                    if (NPC.frameCounter > 6.0)
                                    {
                                        NPC.frameCounter = 0.0;
                                        frameIndex++;
                                        if (frameIndex > 8)
                                        {
                                            frameIndex = 8;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (direction != NPC.direction)
                            {
                                if (frameIndex != 0)
                                {
                                    NPC.frameCounter += 1.0d;
                                    if (NPC.frameCounter > 4.0)
                                    {
                                        NPC.frameCounter = 0.0;
                                        frameIndex--;
                                        if (frameIndex < 9)
                                        {
                                            frameIndex = 0;
                                            NPC.localAI[0] = 0f;
                                            NPC.localAI[1] = 0f;
                                            NPC.localAI[2] = 0f;
                                        }
                                    }
                                }
                                else
                                {
                                    NPC.spriteDirection = NPC.direction;
                                }
                            }
                            else
                            {
                                if (NPC.position.X > Main.player[NPC.target].position.X)
                                {
                                    NPC.frameCounter += 1.0d;
                                }
                                if (frameIndex == 0)
                                {
                                    if (NPC.frameCounter > 16.0)
                                    {
                                        frameIndex = 9;
                                    }
                                }
                                else
                                {
                                    if (NPC.frameCounter > 6.0)
                                    {
                                        NPC.frameCounter = 0.0;
                                        frameIndex++;
                                        if (frameIndex > 11)
                                        {
                                            frameIndex = 11;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;

                case PHASE_DIRECT_WIND_Transition:
                    {
                        if (NPC.direction == -1)
                        {
                            NPC.frameCounter += 1.0d;
                            if (NPC.frameCounter > 4.0)
                            {
                                NPC.frameCounter = 0.0;
                                frameIndex--;
                                if (frameIndex < 6)
                                {
                                    frameIndex = 0;
                                    NPC.localAI[0] = 0f;
                                    NPC.localAI[1] = 0f;
                                    NPC.localAI[2] = 0f;
                                }
                            }
                        }
                        else
                        {
                            NPC.frameCounter += 1.0d;
                            if (NPC.frameCounter > 4.0)
                            {
                                NPC.frameCounter = 0.0;
                                frameIndex--;
                                if (frameIndex < 9)
                                {
                                    frameIndex = 0;
                                    NPC.localAI[0] = 0f;
                                    NPC.localAI[1] = 0f;
                                    NPC.localAI[2] = 0f;
                                }
                            }
                        }
                    }
                    break;

                case PHASE_SUMMON_NIMBUS:
                    {
                        if ((int)NPC.ai[2] == -1)
                        {
                            NPC.frameCounter += 1.0d;
                            if (NPC.frameCounter > 4.0)
                            {
                                NPC.frameCounter = 0.0;
                                frameIndex--;
                                if (frameIndex < 0)
                                {
                                    frameIndex = 0;
                                    NPC.localAI[0] = 0f;
                                    NPC.localAI[1] = 0f;
                                }
                            }
                        }
                        else
                        {
                            if (NPC.ai[1] > 20f)
                            {
                                NPC.frameCounter += 1.0d;
                                if (NPC.frameCounter > 4.0)
                                {
                                    NPC.frameCounter = 0.0;
                                    frameIndex++;
                                    if (frameIndex > 4)
                                    {
                                        frameIndex = 4;
                                    }
                                }
                            }
                        }
                    }
                    break;

                case PHASE_THUNDERCLAP:
                    {
                        if (NPC.ai[1] > 68f)
                        {
                            if (NPC.ai[1] >= 88f)
                            {
                                int timer = (int)NPC.ai[1] % 30;
                                if (timer == 0)
                                {
                                    NPC.frameCounter = 0.0;
                                    frameIndex = 17;
                                }
                                else if (timer > 27)
                                {
                                    NPC.frameCounter = 0.0;
                                    frameIndex = 16;
                                }
                                else if (frameIndex != 15)
                                {
                                    NPC.frameCounter += 1.0d;
                                    if (NPC.frameCounter > 7.0)
                                    {
                                        NPC.frameCounter = 0.0;
                                        frameIndex++;
                                        if (frameIndex > 19)
                                        {
                                            frameIndex = 15;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                NPC.frameCounter += 1.0d;
                                if (NPC.frameCounter > 5.0)
                                {
                                    NPC.frameCounter = 0.0;
                                    if (frameIndex == 0)
                                    {
                                        frameIndex = 12;
                                    }
                                    else
                                    {
                                        frameIndex++;
                                    }
                                }
                            }
                        }
                    }
                    break;
            }

            NPC.frame.Y = frameIndex * frameHeight;

            if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
            {
                NPC.frame.X = NPC.frame.Width * (NPC.frame.Y / (frameHeight * Main.npcFrameCount[NPC.type]));
                NPC.frame.Y = NPC.frame.Y % (frameHeight * Main.npcFrameCount[NPC.type]);
            }
            else
            {
                NPC.frame.X = 0;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(new TrophyDrop(ModContent.ItemType<RedSpriteTrophy>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AtmosphericEnergy>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Fluorescence>(), 1, 10, 24));
            npcLoot.Add(ItemDropRule.Common(ItemID.SoulofFlight, 1, 2, 6));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ScorchingDye>(), 7));
        }

        public override void OnKill()
        {
            AequusDefeats.MarkAsDefeated(ref AequusDefeats.downedRedSprite);
        }

        //public override void NPCLoot()
        //{
        //    if (NPC.target != -1)
        //        GaleStreams.ProgressEvent(Main.player[NPC.target], 40);
        //    if (Main.rand.NextBool(7))
        //    {
        //        Item.NewItem(NPC.getRect(), ModContent.ItemType<RedSpriteMask>());
        //    }
        //    if (Main.rand.NextBool(4))
        //    {
        //        if (Main.rand.NextBool())
        //        {
        //            Item.NewItem(NPC.getRect(), ModContent.ItemType<Items.Tools.Fishing.Nimrod>());
        //        }
        //        else
        //        {
        //            Item.NewItem(NPC.getRect(), ItemID.NimbusRod);
        //        }
        //    }
        //    if (Main.rand.NextBool(8))
        //    {
        //        Item.NewItem(NPC.getRect(), ModContent.ItemType<PeeledCarrot>());
        //    }
        //    if (Main.rand.NextBool(5))
        //    {
        //        Item.NewItem(NPC.getRect(), ModContent.ItemType<Items.Tools.Map.RetroGoggles>());
        //    }
        //}

        public static void DrawThingWithAura(SpriteBatch spriteBatch, Texture2D texture, Vector2 drawPosition, Rectangle? frame, Color drawColor, float rotation, Vector2 origin, float scale, float auraIntensity = 0f)
        {
            float electric = 3f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 5f) + auraIntensity;
            if (electric > 0f)
            {
                var color = new Color(255, 150, 0, 20) * 0.3f;
                for (; electric > 0f; electric -= 2f)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        Main.spriteBatch.Draw(texture, drawPosition + (MathHelper.PiOver4 * k).ToRotationVector2() * electric, frame, color, rotation, origin, scale, SpriteEffects.None, 0f);
                        Main.spriteBatch.Draw(texture, drawPosition - (MathHelper.PiOver4 * k).ToRotationVector2() * electric, frame, color, rotation, origin, scale, SpriteEffects.None, 0f);
                    }
                }
            }
            Main.spriteBatch.Draw(texture, drawPosition, frame, drawColor, rotation, origin, scale, SpriteEffects.None, 0f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var texture = TextureAssets.Npc[Type].Value;
            var drawPosition = NPC.Center;
            var origin = new Vector2(NPC.frame.Width / 2f, NPC.frame.Height / 2f - 14f);
            Vector2 scale = new Vector2(NPC.scale, NPC.scale);
            float auraIntensity = 0f;
            if ((int)NPC.ai[0] == PHASE_SUMMON_NIMBUS)
            {
                auraIntensity += NPC.localAI[2] / 2f;
            }
            else
            {
                float speedX = NPC.velocity.X.Abs();
                if (speedX > 8f)
                {
                    scale.X += (speedX - 8f) / 120f;
                    drawPosition.X -= (scale.X - 1f) * 16f;
                }
            }
            DrawThingWithAura(spriteBatch, texture, drawPosition - screenPos, NPC.frame, drawColor, 0f, origin, NPC.scale, auraIntensity);
            Main.spriteBatch.Draw(Aequus.Tex(this.GetPath() + "_Glow"), drawPosition - screenPos, NPC.frame, Color.White, NPC.rotation, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        private void LoadWindSound()
        {
            if (WindSound == null)
            {
                var soundEffect = SoundEngine.GetTrackableSoundByStyleId(SoundID.BlizzardStrongLoop.Style);
                WindSound = soundEffect.CreateInstance();
            }
        }
    }
}