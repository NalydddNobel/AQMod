using AQMod.Common;
using AQMod.Common.CrossMod.BossChecklist;
using AQMod.Content.Dusts;
using AQMod.Effects.ScreenEffects;
using AQMod.Items.Placeable.Banners;
using AQMod.Localization;
using AQMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters.GaleStreams
{
    [AutoloadBossHead()]
    public class RedSprite : ModNPC, ISetupContentType
    {
        private bool _setupFrame;
        public int frameIndex;
        public const int FramesX = 3;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 8;
        }

        public override void SetDefaults()
        {
            npc.width = 74;
            npc.height = 50;
            npc.lifeMax = 2750;
            npc.damage = 45;
            npc.defense = 15;
            npc.knockBackResist = 0f;
            npc.HitSound = SoundID.NPCHit30;
            npc.DeathSound = SoundID.NPCDeath33;
            npc.aiStyle = -1;
            npc.noGravity = true;
            npc.value = Item.buyPrice(silver: 30);
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.CursedInferno] = true;
            npc.buffImmune[BuffID.Ichor] = true;
            npc.buffImmune[BuffID.ShadowFlame] = true;
            npc.buffImmune[BuffID.Bleeding] = true;
            banner = npc.type;
            bannerItem = ModContent.ItemType<RedSpriteBanner>();
            npc.noTileCollide = true;

            var aQNPC = npc.GetGlobalNPC<AQNPC>();
            aQNPC.temperature = 40;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            var center = npc.Center;
            if (npc.life < 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    if (Main.trackableSoundInstances[SoundID.BlizzardStrongLoop.Style].State == SoundState.Playing)
                    {
                        Main.trackableSoundInstances[SoundID.BlizzardStrongLoop.Style].Stop();
                    }
                }
                for (int i = 0; i < 50; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<RedSpriteDust>());
                    Main.dust[d].velocity = (Main.dust[d].position - center) / 8f;
                }
            }
            else
            {
                for (int i = 0; i < damage / 100; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<RedSpriteDust>());
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
            if ((int)npc.ai[0] == -1)
            {
                if (npc.timeLeft < 20)
                {
                    npc.timeLeft = 20;
                }
                npc.velocity.Y -= 0.25f;
                return;
            }
            var center = npc.Center;
            if ((int)npc.ai[0] == 0)
            {
                npc.TargetClosest(faceTarget: false);
                if (!npc.HasValidTarget)
                {
                    npc.ai[0] = -1;
                    return;
                }
                RandomizePhase();
                npc.velocity = -Vector2.Normalize(Main.player[npc.target].Center - center) * 20f;
            }
            if (!npc.HasValidTarget)
            {
                npc.ai[0] = -1;
                return;
            }
            switch ((int)npc.ai[0])
            {
                case PHASE_DIRECT_WIND:
                {
                    if (npc.direction == -1)
                    {
                        if (npc.position.X > Main.player[npc.target].position.X - 100)
                        {
                            if (npc.velocity.X > -20f)
                            {
                                npc.velocity.X -= 0.8f;
                                if (npc.velocity.X > 0f)
                                {
                                    npc.velocity.X *= 0.96f;
                                }
                            }
                        }
                        else
                        {
                            if (Main.player[npc.target].position.X - npc.position.X > 500)
                            {
                                if (npc.velocity.X < 20f)
                                {
                                    npc.velocity.X += 0.4f;
                                    if (npc.velocity.X < 0f)
                                    {
                                        npc.velocity.X *= 0.96f;
                                    }
                                }
                            }
                            else
                            {
                                npc.ai[1] = 1f;
                                if (npc.velocity.X < -4f)
                                    npc.velocity.X *= 0.94f;
                            }
                        }
                    }
                    else
                    {
                        if (npc.position.X < Main.player[npc.target].position.X + 100)
                        {
                            if (npc.velocity.X < 20f)
                            {
                                npc.velocity.X += 0.8f;
                                if (npc.velocity.X < 0f)
                                {
                                    npc.velocity.X *= 0.96f;
                                }
                            }
                        }
                        else
                        {
                            if (npc.position.X - Main.player[npc.target].position.X > 500)
                            {
                                if (npc.velocity.X > -20f)
                                {
                                    npc.velocity.X -= 0.4f;
                                    if (npc.velocity.X > 0f)
                                    {
                                        npc.velocity.X *= 0.96f;
                                    }
                                }
                            }
                            else
                            {
                                npc.ai[1] = 1f;
                                if (npc.velocity.X > 4f)
                                    npc.velocity.X *= 0.94f;
                            }
                        }
                    }
                    if ((int)npc.ai[1] == 1)
                    {
                        npc.ai[3]++;
                        if (npc.ai[3] > 600f)
                        {
                            npc.ai[0] = PHASE_DIRECT_WIND_Transition;
                            npc.ai[2] = -1f;
                            npc.ai[3] = 0f;
                        }
                        if (npc.ai[2] < 0f)
                        {
                            npc.ai[2] = 0f;
                        }

                        if (Main.netMode != NetmodeID.Server)
                        {
                            if (Main.player[Main.myPlayer].Distance(center) < 1000f)
                            {
                                float x = Main.player[Main.myPlayer].position.X + Main.player[Main.myPlayer].width / 2f + (Main.screenWidth / 2f + 20) * npc.direction;
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
                                Main.dust[d].velocity.X = -npc.direction * 20f;
                                Main.dust[d].velocity.Y = Main.rand.NextFloat(1f, 3f);
                                Main.dust[d].color = new Color(255, 200, 10, 255);
                                Main.dust[d].alpha = 100;
                                Main.dust[d].scale = Main.rand.NextFloat(0.5f, 4f);
                            }
                        }

                        for (int i = 0; i < Main.maxPlayers; i++)
                        {
                            if (Main.player[i].active && !Main.player[i].dead && Main.player[i].Distance(center) < 2000f)
                            {
                                Main.player[i].AddBuff(ModContent.BuffType<Buffs.Debuffs.RedSpriteWind>(), 4);
                                var aQPlayer = Main.player[i].GetModPlayer<AQPlayer>();
                                aQPlayer.redSpriteWind = (sbyte)-npc.direction;
                                if (npc.direction == -1)
                                {
                                    if (Main.player[npc.target].velocity.X < 1f)
                                    {
                                        if (aQPlayer.temperatureRegen < 10 || aQPlayer.temperature == 0)
                                        {
                                            if (Main.expertMode)
                                            {
                                                aQPlayer.InflictTemperature(10);
                                                aQPlayer.temperatureRegen = 20;
                                            }
                                            else
                                            {
                                                aQPlayer.InflictTemperature(4);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (Main.player[npc.target].velocity.X > -1f)
                                    {
                                        if (aQPlayer.temperatureRegen < 10 || aQPlayer.temperature == 0)
                                        {
                                            if (Main.expertMode)
                                            {
                                                aQPlayer.InflictTemperature(10);
                                                aQPlayer.temperatureRegen = 20;
                                            }
                                            else
                                            {
                                                aQPlayer.InflictTemperature(4);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (npc.direction == -1)
                        {
                            if (Main.player[npc.target].velocity.X > 2f)
                            {
                                npc.ai[2]++;
                                if (npc.ai[2] > 180f)
                                {
                                    npc.TargetClosest(faceTarget: false);
                                    npc.ai[2] = -1f;
                                    npc.ai[1] = 0f;
                                    npc.spriteDirection = npc.direction;
                                    npc.direction = 1;
                                }
                            }
                        }
                        else
                        {
                            if (Main.player[npc.target].velocity.X < -2f)
                            {
                                npc.ai[2]++;
                                if (npc.ai[2] > 180f)
                                {
                                    npc.TargetClosest(faceTarget: false);
                                    npc.ai[2] = -1f;
                                    npc.ai[1] = 0f;
                                    npc.spriteDirection = npc.direction;
                                    npc.direction = -1;
                                }
                            }
                        }

                        if (Main.netMode != NetmodeID.Server && Main.ambientVolume > 0f)
                        {
                            if (Main.trackableSoundInstances[SoundID.BlizzardStrongLoop.Style].State != SoundState.Playing)
                            {
                                Main.trackableSoundInstances[SoundID.BlizzardStrongLoop.Style].Volume = Main.ambientVolume;
                                Main.trackableSoundInstances[SoundID.BlizzardStrongLoop.Style].Play();
                            }
                        }
                    }
                    npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, (Main.player[npc.target].position.Y - 100 - center.Y) / 4f, 0.1f);
                    npc.rotation = npc.velocity.X * 0.01f;
                }
                break;

                case PHASE_DIRECT_WIND_Transition:
                {
                    if (npc.direction == -1)
                    {
                        if (npc.position.X > Main.player[npc.target].position.X - 100)
                        {
                            if (npc.velocity.X > -20f)
                            {
                                npc.velocity.X -= 0.8f;
                                if (npc.velocity.X > 0f)
                                {
                                    npc.velocity.X *= 0.96f;
                                }
                            }
                        }
                        else
                        {
                            if (Main.player[npc.target].position.X - npc.position.X > 500)
                            {
                                if (npc.velocity.X < 20f)
                                {
                                    npc.velocity.X += 0.4f;
                                    if (npc.velocity.X < 0f)
                                    {
                                        npc.velocity.X *= 0.96f;
                                    }
                                }
                            }
                            else
                            {
                                if (npc.velocity.X < -4f)
                                    npc.velocity.X *= 0.94f;
                            }
                        }
                    }
                    else
                    {
                        if (npc.position.X < Main.player[npc.target].position.X + 100)
                        {
                            if (npc.velocity.X < 20f)
                            {
                                npc.velocity.X += 0.8f;
                                if (npc.velocity.X < 0f)
                                {
                                    npc.velocity.X *= 0.96f;
                                }
                            }
                        }
                        else
                        {
                            if (npc.position.X - Main.player[npc.target].position.X > 500)
                            {
                                if (npc.velocity.X > -20f)
                                {
                                    npc.velocity.X -= 0.4f;
                                    if (npc.velocity.X > 0f)
                                    {
                                        npc.velocity.X *= 0.96f;
                                    }
                                }
                            }
                            else
                            {
                                if (npc.velocity.X > 4f)
                                    npc.velocity.X *= 0.94f;
                            }
                        }
                    }
                    npc.ai[1]++;
                    if (Main.netMode != NetmodeID.Server && Main.ambientVolume > 0f)
                    {
                        if (Main.trackableSoundInstances[SoundID.BlizzardStrongLoop.Style].State == SoundState.Playing)
                        {
                            if (npc.ai[1] > 30f)
                            {
                                Main.trackableSoundInstances[SoundID.BlizzardStrongLoop.Style].Stop();
                            }
                            else
                            {
                                float volume = 1f - npc.ai[1] / 30f;
                                Main.trackableSoundInstances[SoundID.BlizzardStrongLoop.Style].Volume = Main.ambientVolume * volume;
                            }
                        }
                    }
                    if (npc.ai[1] > (Main.expertMode ? 30f : 60f))
                    {
                        npc.ai[1] = 0f;
                        RandomizePhase(PHASE_DIRECT_WIND);
                    }
                }
                break;

                case PHASE_SUMMON_NIMBUS:
                {
                    npc.direction = 0;
                    npc.rotation = Utils.AngleLerp(npc.rotation, 0f, 0.1f);
                    Vector2 gotoPosition = new Vector2(Main.player[npc.target].position.X + Main.player[npc.target].width / 2f, Main.player[npc.target].position.Y - 300f);
                    gotoPosition += new Vector2(40f, 0f).RotatedBy(npc.ai[2]);
                    npc.ai[2] += 0.02f;
                    if ((center - gotoPosition).Length() < 100f)
                    {
                        npc.ai[1]++;
                        if (npc.velocity.Length() > 5f)
                            npc.velocity *= 0.96f;
                        if (npc.ai[1] > 90f)
                        {
                            int cloudsAmount = 8;
                            int delayBetweenCloudSpawns = 20;
                            int cloudLifespan = 240;
                            if (Main.expertMode)
                            {
                                delayBetweenCloudSpawns /= 2;
                                cloudLifespan *= 2;
                            }
                            int timer = (int)(npc.ai[1] - 90f) % 10;
                            npc.localAI[2] = timer;
                            if (timer == 0 && (Main.expertMode || npc.ai[1] <= 160f))
                            {
                                Main.PlaySound(SoundID.Item66.WithVolume(1.3f), gotoPosition);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int timer2 = (int)(npc.ai[1] - 90f) % 20;
                                    int direction = 1;
                                    if (timer2 >= 10)
                                    {
                                        direction = -1;
                                    }
                                    var projPosition = new Vector2(center.X + 900f * direction, center.Y + Main.rand.NextFloat(-120f, 60f));
                                    var velocity = Vector2.Normalize(Main.player[npc.target].Center + new Vector2(0f, -160f) - projPosition);
                                    int damage = 50;
                                    if (Main.expertMode)
                                    {
                                        damage = 30;
                                    }
                                    int type = ModContent.ProjectileType<Projectiles.Monster.RedSpriteCloud>();
                                    int p = Projectile.NewProjectile(projPosition, velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * 18f, type, damage, 1f, Main.myPlayer);
                                    Main.projectile[p].rotation = Main.projectile[p].velocity.ToRotation();
                                    Main.projectile[p].friendly = false;
                                    Main.projectile[p].hostile = true;
                                    Main.projectile[p].timeLeft = (int)(Vector2.Distance(projPosition, Main.player[npc.target].Center) / Main.projectile[p].velocity.Length());
                                }
                            }
                            if (npc.ai[1] > delayBetweenCloudSpawns * cloudsAmount + 90f)
                            {
                                npc.ai[2] = -1f;
                                npc.localAI[2] = 0f;
                                RandomizePhase(PHASE_SUMMON_NIMBUS);
                                npc.ai[2] = 0f;
                            }
                        }
                    }
                    else
                    {
                        npc.velocity = Vector2.Lerp(npc.velocity, Vector2.Normalize(gotoPosition - center) * 20f, 0.1f);
                    }
                }
                break;

                case PHASE_THUNDERCLAP:
                {
                    npc.direction = 0;
                    npc.rotation = Utils.AngleLerp(npc.rotation, 0f, 0.1f);
                    Vector2 gotoPosition = new Vector2(Main.player[npc.target].position.X + Main.player[npc.target].width / 2f, Main.player[npc.target].position.Y - 300f);
                    var v = new Vector2(90f, 0f).RotatedBy(npc.ai[2]);
                    v.X *= 2.5f;
                    gotoPosition += v;
                    npc.ai[2] += 0.05f;
                    int timer = (int)(npc.ai[1] - 90f) % 30;
                    if (npc.ai[1] < 90f)
                    {
                        timer = 0;
                    }
                    else
                    {
                        if (npc.ai[1] > 30 * 4 + 90f || (!Main.expertMode && npc.ai[1] >= 30 * 2 + 90f))
                        {
                            npc.ai[2] = -1f;
                            npc.localAI[2] = 0f;
                            RandomizePhase(PHASE_THUNDERCLAP);
                            npc.ai[2] = 0f;
                        }
                        else
                        {
                            if (timer == 0)
                            {
                                if (Main.netMode != NetmodeID.Server && AQMod.Screenshakes && (Main.myPlayer == npc.target || Main.player[Main.myPlayer].Distance(center) < 1000f))
                                {
                                    if (Main.netMode != NetmodeID.Server)
                                    {
                                        AQSoundPlayer.PlaySound(SoundType.Item, "Sounds/Item/ThunderClap_" + Main.rand.Next(2), npc.Center, 0.6f);
                                    }
                                    ScreenShakeManager.AddEffect(new BasicScreenShake(8, AQMod.MultIntensity(12)));
                                    if (AQConfigClient.c_EffectQuality > 0.2f)
                                    {
                                        int dustAmount = 50;
                                        if (AQConfigClient.c_EffectQuality < 1f)
                                        {
                                            dustAmount = (int)(dustAmount * AQConfigClient.c_EffectQuality);
                                        }
                                        float rot = MathHelper.TwoPi / dustAmount;
                                        for (int i = 0; i < dustAmount; i++)
                                        {
                                            var normal = new Vector2(1f, 0f).RotatedBy(rot * i);
                                            int d = Dust.NewDust(center + normal * npc.width, 2, 2, ModContent.DustType<RedSpriteDust>());
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
                                    Projectile.NewProjectile(center + new Vector2(0f, 130f), Vector2.Zero, ModContent.ProjectileType<Projectiles.Monster.RedSpriteThunderClap>(), damage, 1f, Main.myPlayer);
                                }
                            }
                        }
                    }
                    npc.ai[1]++;
                    if (timer > 35)
                    {
                        npc.velocity *= 0.5f;
                    }
                    else
                    {
                        if (timer < 16)
                        {
                            npc.velocity = Vector2.Lerp(npc.velocity, Vector2.Normalize(gotoPosition - center) * 20f, 0.02f);
                        }
                    }
                }
                break;

                case PHASE_THUNDERCLAP_Transition:
                {
                    Vector2 gotoPosition = new Vector2(Main.player[npc.target].position.X + Main.player[npc.target].width / 2f, Main.player[npc.target].position.Y - 300f);
                    var v = new Vector2(90f, 0f).RotatedBy(npc.ai[2]);
                    v.X *= 2.5f;
                    gotoPosition += v;
                    npc.velocity = Vector2.Lerp(npc.velocity, Vector2.Normalize(gotoPosition - center) * 20f, 0.02f);
                    npc.ai[1]++;
                    if (npc.ai[1] > (Main.expertMode ? 30f : 60f))
                    {
                        npc.ai[1] = 0f;
                        RandomizePhase(PHASE_THUNDERCLAP);
                    }
                }
                break;
            }
        }

        private void RandomizePhase(int curPhase = -1)
        {
            npc.TargetClosest(faceTarget: false);
            if (!npc.HasValidTarget)
            {
                npc.ai[0] = -1;
                return;
            }
            for (int i = 0; i < 50; i++)
            {
                npc.ai[0] = Main.rand.Next(2) + 1;
                if (npc.life * 2 < npc.lifeMax && Main.rand.NextBool(4))
                {
                    npc.ai[0] = PHASE_THUNDERCLAP;
                }
                if ((int)npc.ai[0] != curPhase)
                {
                    break;
                }
            }
            npc.ai[1] = 0f;
            npc.netUpdate = true;
            frameIndex = 0;
            if ((int)npc.ai[0] == 1)
            {
                npc.direction = Main.rand.NextBool() ? -1 : 1;
            }
            npc.spriteDirection = npc.direction;
        }

        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            if (!_setupFrame)
            {
                _setupFrame = true;
                npc.frame.Width = npc.frame.Width / FramesX;
            }

            switch ((int)npc.ai[0])
            {
                default:
                {
                    npc.frameCounter += 1.0d;
                    if (npc.frameCounter > 2.0d)
                    {
                        npc.frameCounter = 0.0d;
                        frameIndex++;
                        if (frameIndex >= Main.npcFrameCount[npc.type] * FramesX)
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
                    int direction = npc.spriteDirection;
                    if (direction == -1)
                    {
                        if (direction != npc.direction)
                        {
                            if (frameIndex != 0)
                            {
                                npc.frameCounter += 1.0d;
                                if (npc.frameCounter > 4.0)
                                {
                                    npc.frameCounter = 0.0;
                                    frameIndex--;
                                    if (frameIndex < 6)
                                    {
                                        frameIndex = 0;
                                        npc.localAI[0] = 0f;
                                        npc.localAI[1] = 0f;
                                        npc.localAI[2] = 0f;
                                    }
                                }
                            }
                            else
                            {
                                npc.spriteDirection = npc.direction;
                            }
                        }
                        else
                        {
                            if (npc.position.X < Main.player[npc.target].position.X)
                            {
                                npc.frameCounter += 1.0d;
                            }
                            if (frameIndex == 0)
                            {
                                if (npc.frameCounter > 16.0)
                                {
                                    frameIndex = 6;
                                }
                            }
                            else
                            {
                                if (npc.frameCounter > 6.0)
                                {
                                    npc.frameCounter = 0.0;
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
                        if (direction != npc.direction)
                        {
                            if (frameIndex != 0)
                            {
                                npc.frameCounter += 1.0d;
                                if (npc.frameCounter > 4.0)
                                {
                                    npc.frameCounter = 0.0;
                                    frameIndex--;
                                    if (frameIndex < 9)
                                    {
                                        frameIndex = 0;
                                        npc.localAI[0] = 0f;
                                        npc.localAI[1] = 0f;
                                        npc.localAI[2] = 0f;
                                    }
                                }
                            }
                            else
                            {
                                npc.spriteDirection = npc.direction;
                            }
                        }
                        else
                        {
                            if (npc.position.X > Main.player[npc.target].position.X)
                            {
                                npc.frameCounter += 1.0d;
                            }
                            if (frameIndex == 0)
                            {
                                if (npc.frameCounter > 16.0)
                                {
                                    frameIndex = 9;
                                }
                            }
                            else
                            {
                                if (npc.frameCounter > 6.0)
                                {
                                    npc.frameCounter = 0.0;
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
                    if (npc.direction == -1)
                    {
                        npc.frameCounter += 1.0d;
                        if (npc.frameCounter > 4.0)
                        {
                            npc.frameCounter = 0.0;
                            frameIndex--;
                            if (frameIndex < 6)
                            {
                                frameIndex = 0;
                                npc.localAI[0] = 0f;
                                npc.localAI[1] = 0f;
                                npc.localAI[2] = 0f;
                            }
                        }
                    }
                    else
                    {
                        npc.frameCounter += 1.0d;
                        if (npc.frameCounter > 4.0)
                        {
                            npc.frameCounter = 0.0;
                            frameIndex--;
                            if (frameIndex < 9)
                            {
                                frameIndex = 0;
                                npc.localAI[0] = 0f;
                                npc.localAI[1] = 0f;
                                npc.localAI[2] = 0f;
                            }
                        }
                    }
                }
                break;

                case PHASE_SUMMON_NIMBUS:
                {
                    if ((int)npc.ai[2] == -1)
                    {
                        npc.frameCounter += 1.0d;
                        if (npc.frameCounter > 4.0)
                        {
                            npc.frameCounter = 0.0;
                            frameIndex--;
                            if (frameIndex < 0)
                            {
                                frameIndex = 0;
                                npc.localAI[0] = 0f;
                                npc.localAI[1] = 0f;
                            }
                        }
                    }
                    else
                    {
                        if (npc.ai[1] > 20f)
                        {
                            npc.frameCounter += 1.0d;
                            if (npc.frameCounter > 4.0)
                            {
                                npc.frameCounter = 0.0;
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
                    if (npc.ai[1] > 68f)
                    {
                        if (npc.ai[1] >= 88f)
                        {
                            int timer = (int)npc.ai[1] % 30;
                            if (timer == 0)
                            {
                                npc.frameCounter = 0.0;
                                frameIndex = 17;
                            }
                            else if (timer > 27)
                            {
                                npc.frameCounter = 0.0;
                                frameIndex = 16;
                            }
                            else if (frameIndex != 15)
                            {
                                npc.frameCounter += 1.0d;
                                if (npc.frameCounter > 7.0)
                                {
                                    npc.frameCounter = 0.0;
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
                            npc.frameCounter += 1.0d;
                            if (npc.frameCounter > 5.0)
                            {
                                npc.frameCounter = 0.0;
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

            npc.frame.Y = frameIndex * frameHeight;

            if (npc.frame.Y >= frameHeight * Main.npcFrameCount[npc.type])
            {
                npc.frame.X = npc.frame.Width * (npc.frame.Y / (frameHeight * Main.npcFrameCount[npc.type]));
                npc.frame.Y = npc.frame.Y % (frameHeight * Main.npcFrameCount[npc.type]);
            }
            else
            {
                npc.frame.X = 0;
            }
        }

        public override void NPCLoot()
        {
            WorldDefeats.DownedGaleStreams = true;
            WorldDefeats.DownedRedSprite = true;
            Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Materials.Energies.AtmosphericEnergy>(), Main.rand.Next(2) + 2);
            Item.NewItem(npc.getRect(), ItemID.SoulofFlight, Main.rand.Next(5) + 2);
            Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Materials.Fluorescence>(), Main.rand.Next(10) + 10 + (Main.expertMode ? Main.rand.Next(5) : 0));

            if (Main.rand.NextBool(2))
            {
                Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Vanities.Dyes.RedSpriteDye>());
            }
            if (Main.rand.NextBool(4))
            {
                Item.NewItem(npc.getRect(), ItemID.NimbusRod);
            }
            if (Main.rand.NextBool(8))
            {
                Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Foods.GaleStreams.PeeledCarrot>());
            }
            if (Main.rand.NextBool(10))
            {
                Item.NewItem(npc.getRect(), ModContent.ItemType<Items.BossItems.RedSpriteTrophy>());
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            var texture = Main.npcTexture[npc.type];
            var drawPosition = npc.Center;
            var origin = new Vector2(npc.frame.Width / 2f, npc.frame.Height / 2f - 14f);
            Vector2 scale = new Vector2(npc.scale, npc.scale);
            float electric = 3f + (float)Math.Sin(Main.GlobalTime * 5f);
            if ((int)npc.ai[0] == PHASE_SUMMON_NIMBUS)
            {
                electric += npc.localAI[2] / 2f;
            }
            else
            {
                float speedX = npc.velocity.X.Abs();
                if (speedX > 8f)
                {
                    scale.X += (speedX - 8f) / 120f;
                    drawPosition.X -= (scale.X - 1f) * 16f;
                }
            }
            if (electric > 0f)
            {
                for (int i = 0; i < 8; i++)
                {
                    Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition + new Vector2(electric, 0f).RotatedBy(MathHelper.PiOver4 * i), npc.frame, new Color(150, 255, 0, 20), npc.rotation, origin, scale, SpriteEffects.None, 0f);
                }
            }
            Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, npc.frame, drawColor, npc.rotation, origin, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(this.GetTextureobj("_Glow"), drawPosition - Main.screenPosition, npc.frame, Color.White, npc.rotation, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        void ISetupContentType.SetupContent()
        {
            try
            {
                var bossChecklist = ModLoader.GetMod("BossChecklist");
                if (bossChecklist == null)
                    return;
                new MinibossEntry(
                    () => WorldDefeats.DownedRedSprite,
                    6.67f,
                    ModContent.NPCType<RedSprite>(),
                    AQText.chooselocalizationtext(
                        en_US: "Red Sprite",
                        zh_Hans: "红色精灵"),
                    0,
                    new List<int>()
                    {
                        ItemID.NimbusRod,
                        ModContent.ItemType<Items.Materials.Energies.AtmosphericEnergy>(),
                        ModContent.ItemType<Items.Materials.Fluorescence>(),
                        ModContent.ItemType<Items.Foods.GaleStreams.PeeledCarrot>(),
                    },
                    new List<int>()
                    {
                        ModContent.ItemType<Items.BossItems.RedSpriteTrophy>(),
                        ModContent.ItemType<Items.Vanities.Dyes.RedSpriteDye>(),
                    },
                    AQText.chooselocalizationtext(
                        en_US: "Occasionally appears during the Gale Streams!",
                        zh_Hans: null),
                    "AQMod/Assets/BossChecklist/RedSprite").AddEntry(bossChecklist);
            }
            catch (Exception e)
            {
                mod.Logger.Error("An error occured when setting up boss checklist entries.");
                mod.Logger.Error(e.Message);
                mod.Logger.Error(e.StackTrace);
            }
        }
    }
}