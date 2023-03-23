using Aequus;
using Aequus.Common.Effects;
using Aequus.Common.Preferences;
using Aequus.Common.Primitives;
using Aequus.Common.Utilities;
using Aequus.Content.Boss.RedSpriteMiniboss.Projectiles;
using Aequus.Content.Boss.RedSpriteMiniboss.Rewards;
using Aequus.Content.Boss.SpaceSquidMiniboss;
using Aequus.Content.Events;
using Aequus.Items.Materials;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Vanity.Pets.Light;
using Aequus.NPCs;
using Aequus.Particles;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Boss.RedSpriteMiniboss
{
    [AutoloadBossHead()]
    public class RedSprite : ModNPC
    {
        public const int PHASE_DEAD = -2;
        public const int PHASE_DIRECTWIND = 1;
        public const int PHASE_SUMMON_NIMBUS = 2;
        public const int TRANSITION_DIRECT_WIND = 3;
        public const int PHASE_THUNDERCLAP = 4;
        public const int PHASE_THUNDERCLAP_Transition = 5;

        public const int FRAMES_X = 4;

        public Asset<Texture2D> GlowmaskTexture => ModContent.Request<Texture2D>(Texture + "_Glow", AssetRequestMode.ImmediateLoad);

        public static float LightningDrawOpacity;
        public static float LightningDrawProgress;


        private TrailRenderer prim;
        private TrailRenderer bloomPrim;

        public int frameIndex;
        public Color lightningBloomColor = new Color(128, 10, 5, 0);

        private bool _setupFrame;

        private float _brightnessTimer;
        private float _brightness;
        private bool _deathEffect;
        private bool _importantDeath;
        private Vector2[][] _redSpriteLightningCoords;

        private SoundEffectInstance windSoundInstance;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Position = new Vector2(1f, 0f),
            });
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData()
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.OnFire,
                    BuffID.OnFire3,
                    BuffID.Bleeding,
                }
            });

            AequusNPC.CannotBeElite.Add(Type);
            AequusNPC.HeatDamage.Add(Type);
            SnowgraveCorpse.NPCBlacklist.Add(Type);
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
            NPC.noTileCollide = true;
            NPC.npcSlots = 5f;
            NPC.Aequus().noGravityDrops = true;

            _brightness = 0.2f;

            this.SetBiome<GaleStreamsBiomeManager>();
        }

        public override void ScaleExpertStats(int numPlayers, float balance)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8);
            NPC.damage = (int)(NPC.damage * 0.8);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateGaleStreamsEntry(database, bestiaryEntry, miniBoss: true);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            this.CreateLoot(npcLoot)
                .AddBossLoot<RedSpriteTrophy, RedSpriteRelic>()
                .AddMasterPet<LightningRod>()
                .Add<AtmosphericEnergy>(chance: 1, stack: 1)
                .Add<RedSpriteMask>(chance: 7, stack: 1)
                .Add<Fluorescence>(1, (10, 24))
                .Add(ItemID.SoulofFlight, 1, (2, 6));
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            var center = NPC.Center;
            if (NPC.life < 0)
            {
                if (Main.netMode != NetmodeID.Server && windSoundInstance != null && windSoundInstance.State == SoundState.Playing)
                {
                    windSoundInstance.Stop();
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
            byte minimum = (byte)(int)(255 * _brightness);
            if (drawColor.R < minimum)
            {
                drawColor.R = minimum;
            }
            if (drawColor.G < minimum)
            {
                drawColor.G = minimum;
            }
            if (drawColor.B < minimum)
            {
                drawColor.B = minimum;
            }
            return drawColor;
        }

        public override void AI()
        {
            bool leave = (int)NPC.ai[0] == -1;
            if (!leave && !GaleStreamsBiomeManager.IsThisSpace(Main.player[NPC.target].position.Y))
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
                AI_RandomizePhase();
                NPC.velocity = -Vector2.Normalize(Main.player[NPC.target].Center - center) * 20f;
            }
            if (!NPC.HasValidTarget)
            {
                NPC.ai[0] = -1;
                return;
            }
            switch ((int)NPC.ai[0])
            {
                case PHASE_DIRECTWIND:
                    {
                        AI_Phase_DirectWind(center);
                    }
                    break;

                case TRANSITION_DIRECT_WIND:
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
                            AI_LoadWindSound();
                            if (windSoundInstance.State == SoundState.Playing)
                            {
                                if (NPC.ai[1] > 30f)
                                {
                                    windSoundInstance.Stop();
                                }
                                else
                                {
                                    float volume = 1f - NPC.ai[1] / 30f;
                                    windSoundInstance.Volume = Main.ambientVolume * volume;
                                }
                            }
                        }
                        if (NPC.ai[1] > (Main.expertMode ? 30f : 60f))
                        {
                            NPC.ai[1] = 0f;
                            AI_RandomizePhase(PHASE_DIRECTWIND);
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
                                    SoundEngine.PlaySound(SoundID.Item66, gotoPosition);
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
                                        int damage = NPC.FixedDamage();
                                        int type = ModContent.ProjectileType<RedSpriteCloud>();
                                        int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), projPosition, velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * 18f, type, damage, 1f, Main.myPlayer);
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
                                    AI_RandomizePhase(PHASE_SUMMON_NIMBUS);
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
                                AI_RandomizePhase(PHASE_THUNDERCLAP);
                                NPC.ai[2] = 0f;
                            }
                            else
                            {
                                //if (timer < 3
                                //    && Main.netMode != NetmodeID.Server &&
                                //    (Main.myPlayer == NPC.target || Main.player[Main.myPlayer].Distance(center) < 1000f))
                                //{
                                //    ScreenFlash.Flash.Set(NPC.Center, 0.75f);
                                //    ScreenShake.SetShake(8f);
                                //}
                                if (timer == 0)
                                {
                                    if (Main.netMode != NetmodeID.Server)
                                    {
                                        if (Aequus.CloseToEffect(NPC.Center))
                                        {
                                            SkyDarkness.DarknessTransition(0f, 0.05f);
                                        }
                                        ScreenShake.SetShake(30f, where: NPC.Center);
                                        if (Main.netMode != NetmodeID.Server)
                                        {
                                            SoundEngine.PlaySound(Aequus.GetSounds("RedSprite/thunderClap", 2), NPC.Center);
                                        }
                                        int dustAmount = 50;
                                        if (!ClientConfig.Instance.HighQuality)
                                        {
                                            dustAmount = (int)(dustAmount * 0.5f);
                                        }
                                        float rot = MathHelper.TwoPi / dustAmount;
                                        for (int i = 0; i < dustAmount; i++)
                                        {
                                            var normal = new Vector2(1f, 0f).RotatedBy(rot * i);
                                            int d = Dust.NewDust(center + normal * NPC.width, 2, 2, ModContent.DustType<RedSpriteDust>());
                                            Main.dust[d].velocity = normal * 12f;
                                        }
                                    }
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        int damage = NPC.FixedDamage() * 3;
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), center + new Vector2(0f, 130f), Vector2.Zero, ModContent.ProjectileType<RedSpriteThunderClap>(), damage, 1f, Main.myPlayer);
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
                            AI_RandomizePhase(PHASE_THUNDERCLAP);
                        }
                    }
                    break;

                case PHASE_DEAD:
                    {
                        NPC.rotation = MathHelper.WrapAngle(NPC.rotation);
                        NPC.rotation *= 0.8f;
                        NPC.velocity *= 0.8f;
                        NPC.ai[1]++;
                        if ((int)NPC.ai[1] >= 57 && !_deathEffect)
                        {
                            if (Main.netMode != NetmodeID.Server)
                            {
                                if (Aequus.CloseToEffect(NPC.Center))
                                {
                                    SkyDarkness.DarknessTransition(0f, 0.02f);
                                }
                                if (NPC.DeathSound != null)
                                {
                                    SoundEngine.PlaySound(NPC.DeathSound.Value.WithPitch(-0.5f), NPC.Center);
                                }
                                NPC.HitEffect(0, NPC.lifeMax);
                                if (NPC.Distance(Main.LocalPlayer.Center) < 2000f)
                                {
                                    bool reduceFX = AequusWorld.downedRedSprite || NPC.CountNPCS(Type) > 1;
                                    ScreenFlash.Flash.Set(NPC.Center, reduceFX ? 2f : 7.5f, 0.6f);
                                    ScreenShake.SetShake(reduceFX ? 18f : 20f);
                                }
                            }
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                NPC.NPCLoot();
                                //NPC.NewNPCDirect(NPC.GetSource_Death(), NPC.Center, ModContent.NPCType<RedSpriteFriendly>());
                            }
                            _deathEffect = true;
                        }
                        if ((int)NPC.ai[1] > 140f)
                        {
                            NPC npc = new NPC();
                            npc.SetDefaults(NPCID.AngryNimbus);
                            npc.Center = NPC.Center;
                            if (NPC.DeathSound != null)
                            {
                                SoundEngine.PlaySound(NPC.DeathSound.Value, NPC.Center);
                            }
                            npc.HitEffect(0, npc.lifeMax);
                            NPC.active = false;
                            NPC.life = -1;
                        }
                    }
                    break;
            }
        }
        public void AI_Phase_DirectWind(Vector2 center)
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
                    NPC.ai[0] = TRANSITION_DIRECT_WIND;
                    NPC.ai[2] = -1f;
                    NPC.ai[3] = 0f;
                }
                if (NPC.ai[2] < 0f)
                {
                    NPC.ai[2] = 0f;
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    AI_Phase_DirectWind_SpawnDust_PlaySound(center);
                }
                AI_Phase_DirectWind_SpawnProjs(center);
            }

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

            NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, (Main.player[NPC.target].position.Y - 100 - center.Y) / 4f, 0.1f);
            NPC.rotation = NPC.velocity.X * 0.01f;
        }
        public void AI_Phase_DirectWind_SpawnDust_PlaySound(Vector2 center)
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
                int d = Dust.NewDust(new Vector2(x - 1f, y - 1f), 2, 2, DustID.Sandstorm);
                Main.dust[d].velocity.X = -NPC.direction * 20f;
                Main.dust[d].velocity.Y = Main.rand.NextFloat(1f, 3f);
                Main.dust[d].color = new Color(255, 200, 10, 255);
                Main.dust[d].alpha = 100;
                Main.dust[d].scale = Main.rand.NextFloat(0.5f, 4f);

                if (Main.netMode != NetmodeID.Server && Main.ambientVolume > 0f)
                {
                    AI_LoadWindSound();
                    if (windSoundInstance.State != SoundState.Playing)
                    {
                        windSoundInstance.Volume = Main.ambientVolume;
                        windSoundInstance.Play();
                    }
                }
            }
        }
        public void AI_Phase_DirectWind_SpawnProjs(Vector2 center)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                float xOffset = 1500f * NPC.direction;
                var velocity = new Vector2(4.5f * -NPC.direction, 0f);
                int damage = NPC.FixedDamage();
                if ((int)NPC.ai[3] % 60 == 0)
                {
                    NPC.TargetClosest(faceTarget: false);
                    if (NPC.HasValidTarget)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(Main.player[NPC.target].position.X + xOffset, Main.player[NPC.target].position.Y + Main.player[NPC.target].height / 2f), velocity, ModContent.ProjectileType<RedSpriteWindFire>(), damage, 1f, Main.myPlayer);
                    }
                }
                if (Main.rand.NextBool(Main.expertMode ? 60 : 120))
                {
                    var position = center;
                    position.X += xOffset;
                    position.Y += Main.rand.NextFloat(-350f, 350f);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<RedSpriteWindFire>(), damage, 1f, Main.myPlayer);
                }
            }
        }
        public void AI_RandomizePhase(int curPhase = -1)
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
        public void AI_LoadWindSound()
        {
            if (windSoundInstance == null)
            {
                var soundEffect = SoundID.BlizzardStrongLoop.GetRandomSound();
                windSoundInstance = soundEffect.CreateInstance();
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            _brightnessTimer += 0.1f;

            float brightnessMax = 0.5f;
            if (!_setupFrame)
            {
                _setupFrame = true;
                NPC.frame.Width = NPC.frame.Width / FRAMES_X;
            }
            if (NPC.IsABestiaryIconDummy)
            {
                _brightness = Helper.Wave(_brightnessTimer, 0.2f, brightnessMax);
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
                            if (frameIndex >= Main.npcFrameCount[NPC.type] * FRAMES_X)
                            {
                                frameIndex = 0;
                            }
                        }
                    }
                    break;

                case -1:
                    frameIndex = 1;
                    break;

                case PHASE_DIRECTWIND:
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

                case TRANSITION_DIRECT_WIND:
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
                        brightnessMax += 0.5f;
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

                case PHASE_DEAD:
                    {
                        if (NPC.ai[1] > 30f)
                        {
                            frameIndex = 22 + (int)Math.Min((NPC.ai[1] - 30f) / 3f, 5f);
                        }
                        else
                        {
                            frameIndex = 22;
                        }
                    }
                    break;
            }

            _brightness = Helper.Wave(_brightnessTimer, 0.2f, brightnessMax);

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

        public override bool CheckDead()
        {
            if ((int)NPC.ai[0] == PHASE_DEAD)
            {
                return true;
            }

            _importantDeath = !AequusWorld.downedRedSprite && NPC.CountNPCS(Type) <= 1;
            NPC.ai[0] = PHASE_DEAD;
            NPC.ai[1] = 0f;
            NPC.ai[2] = 0f;
            NPC.ai[3] = 0f;
            NPC.dontTakeDamage = true;
            NPC.life = 1;
            NPC.netUpdate = true;
            return false;
        }

        public override void OnKill()
        {
            Helper.DropHearts(new EntitySource_Loot(NPC), NPC.Hitbox, 4, 4);
            AequusWorld.MarkAsDefeated(ref AequusWorld.downedRedSprite, Type);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var texture = TextureAssets.Npc[Type].Value;
            var drawPosition = NPC.Center;
            var origin = new Vector2(NPC.frame.Width / 2f, NPC.frame.Height / 2f - 14f);
            Vector2 scale = new Vector2(NPC.scale, NPC.scale);
            float auraIntensity = 0f;
            if ((int)NPC.ai[0] == PHASE_DEAD)
            {
                if (_importantDeath)
                {
                    ModContent.GetInstance<CameraFocus>().SetTarget("Red Sprite", NPC.Center, CameraPriority.MinibossDefeat, 6f, 60);
                }
                if (NPC.ai[1] > 60f)
                {
                    DrawLightning(screenPos);

                    Main.instance.LoadNPC(NPCID.AngryNimbus);
                    var nimbusTexture = TextureAssets.Npc[NPCID.AngryNimbus].Value;
                    var nimbusFrame = nimbusTexture.Frame(verticalFrames: Main.npcFrameCount[NPCID.AngryNimbus],
                        frameY: Helper.TimedBasedOn((int)Main.GameUpdateCount, 6, Main.npcFrameCount[NPCID.AngryNimbus]));
                    var nimbusOrigin = nimbusFrame.Size() / 2f;

                    Main.spriteBatch.Draw(nimbusTexture, drawPosition - screenPos, nimbusFrame, Color.Lerp(Color.White, NPC.GetNPCColorTintedByBuffs(drawColor), MathHelper.Clamp((NPC.ai[1] - 60f) / 20f, 0f, 1f)), NPC.rotation, nimbusOrigin, scale, SpriteEffects.None, 0f);
                    return false;
                }
            }
            else if ((int)NPC.ai[0] == PHASE_SUMMON_NIMBUS)
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
            DrawWithAura(spriteBatch, texture, drawPosition - screenPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, origin, NPC.scale, auraIntensity, bestiary: NPC.IsABestiaryIconDummy);
            Main.spriteBatch.Draw(GlowmaskTexture.Value, drawPosition - screenPos, NPC.frame, Color.White, NPC.rotation, origin, scale, SpriteEffects.None, 0f);
            return false;
        }
        public void DrawLightning(Vector2 screenPos)
        {
            LightningDrawOpacity = Math.Min(1f - (NPC.ai[1] - 65f) / 10f, 1f);

            if (LightningDrawOpacity <= 0f)
            {
                return;
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin_World(shader: true);

            if (_redSpriteLightningCoords == null)
            {
                GenerateLightning();
            }

            var screenPosition = NPC.Center - screenPos;
            Vector2[][] lightningStrips = new Vector2[_redSpriteLightningCoords.GetLength(0)][];
            for (int i = 0; i < _redSpriteLightningCoords.GetLength(0); i++)
            {
                lightningStrips[i] = new Vector2[_redSpriteLightningCoords[i].Length];
                PrepareLightningStrip(ref lightningStrips[i], i, Main.GlobalTimeWrappedHourly * (10f * (i + 1)), screenPosition + new Vector2(-10f, 0f));
            }

            CheckPrims();

            LightningDrawProgress = 1f - Math.Min(1f - (NPC.ai[1] - 60f) / 20f, 1f);

            for (int i = 0; i < _redSpriteLightningCoords.GetLength(0); i++)
            {
                prim.Draw(lightningStrips[i]);
                bloomPrim.Draw(lightningStrips[i]);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin_World(shader: false); ;
        }
        public void GenerateLightning()
        {
            _redSpriteLightningCoords = new Vector2[7][];
            for (int i = 0; i < _redSpriteLightningCoords.GetLength(0); i++)
            {
                _redSpriteLightningCoords[i] = new Vector2[Aequus.HQ ? 20 : 5];
                var end = (MathHelper.PiOver4 * 0.1f * (i - 3) - MathHelper.PiOver2).ToRotationVector2() * 1080f;
                GenerateLightningStrip(ref _redSpriteLightningCoords[i], Main.GlobalTimeWrappedHourly * (i + 1), end);
            }
        }
        public Vector2[] GenerateLightningStrip(ref Vector2[] coordinates, float timer, Vector2 difference)
        {
            var offsetVector = Vector2.Normalize(difference.RotatedBy(MathHelper.PiOver2));
            var rand = LegacyEffects.EffectRand;
            int old = rand.SetRand((int)timer / 2 * 2);
            float multiplier = 0.01f;
            float diff = 0f;
            for (int i = 0; i < coordinates.Length; i++)
            {
                if (multiplier < 1f)
                {
                    multiplier += 0.2f;
                    if (multiplier > 1f)
                    {
                        multiplier = 1f;
                    }
                }
                diff += rand.Rand(-20f, 20f);
                diff = MathHelper.Clamp(diff, -50f, 50f);
                coordinates[i] = difference / (coordinates.Length - 1) * i + offsetVector * diff * multiplier;
            }
            rand.SetRand(old);
            return coordinates;
        }
        public Vector2[] PrepareLightningStrip(ref Vector2[] coordinates, int k, float timer, Vector2 screenPosition)
        {
            var rand = LegacyEffects.EffectRand;
            int old = rand.SetRand((int)timer / 2 * 2);
            for (int i = 0; i < coordinates.Length; i++)
            {
                var offset = Vector2.Lerp(new Vector2(rand.Rand() / 15f, rand.Rand() / 25f), new Vector2(rand.Rand() / 15f, rand.Rand() / 25f), timer / 2f % 1f);
                coordinates[i] += _redSpriteLightningCoords[k][i] + screenPosition + offset;
            }
            rand.SetRand(old);
            CameraFocus.GetY_Check(coordinates);
            return coordinates;
        }
        public void CheckPrims()
        {
            if (prim == null)
            {
                prim = new TrailRenderer(TrailTextures.Trail[1].Value, TrailRenderer.DefaultPass,
                    (p) => new Vector2(8f) * GetRealProgress(p), (p) => new Color(255, 100, 40, 40) * LightningDrawOpacity * GetRealProgress(p) * GetRealProgress(p), obeyReversedGravity: false, worldTrail: false);
            }
            if (bloomPrim == null)
            {
                bloomPrim = new TrailRenderer(TrailTextures.Trail[1].Value, TrailRenderer.DefaultPass,
                    (p) => new Vector2(44f) * GetRealProgress(p), (p) => lightningBloomColor * LightningDrawOpacity * GetRealProgress(p) * GetRealProgress(p), obeyReversedGravity: false, worldTrail: false);
            }
        }
        public float GetRealProgress(float p)
        {
            float p2 = (p - LightningDrawProgress).Abs();
            if (p2 < 0.1f)
            {
                return 1f - p2 * 5f;
            }
            return 0.33f;
        }

        public static void DrawWithAura(SpriteBatch spriteBatch, Texture2D texture, Vector2 drawPosition, Rectangle? frame, Color drawColor, float rotation, Vector2 origin, float scale, float auraIntensity = 0f, bool bestiary = false)
        {
            int aura = (int)((Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 2f, 8f) + auraIntensity) * 4f);
            if (aura > 0f)
            {
                var color = new Color(255, 150, 0, 20) * 0.3f;
                var circular = Helper.CircularVector(3, Main.GlobalTimeWrappedHourly * 2f);

                var batchData = new SpriteBatchCache(spriteBatch);
                spriteBatch.End();
                if (bestiary)
                {
                    RasterizerState rasterizer = new RasterizerState
                    {
                        CullMode = CullMode.None,
                        ScissorTestEnable = true
                    };
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);
                }
                else
                {
                    spriteBatch.Begin_World(shader: true);
                }

                var drawData = new DrawData(texture, drawPosition, frame, new Color(255, 255, 255, 5), rotation, origin, scale, SpriteEffects.None, 0);
                LegacyEffects.VerticalGradient.ShaderData.UseSecondaryColor(Color.Orange);
                LegacyEffects.VerticalGradient.ShaderData.UseColor(Color.Red);
                LegacyEffects.VerticalGradient.ShaderData.Apply(drawData);

                foreach (var v in circular)
                {
                    Main.spriteBatch.Draw(texture, drawPosition + v * (aura / 4f), frame, color, rotation, origin, scale, SpriteEffects.None, 0f);
                }

                spriteBatch.End();
                batchData.Begin(spriteBatch);
            }
            Main.spriteBatch.Draw(texture, drawPosition, frame, drawColor, rotation, origin, scale, SpriteEffects.None, 0f);
        }
    }

    public class RedSpriteFriendly : ModNPC
    {
        public override string Texture => $"{Aequus.VanillaTexture}NPC_{NPCID.AngryNimbus}";

        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Hide = true, };
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 24;
            NPC.friendly = true;
            NPC.lifeMax = 2500;
            NPC.defense = 10;
            NPC.damage = 15;
            NPC.dontTakeDamage = true;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
            NPC.behindTiles = true;
        }

        public override bool CanChat() => true;

        public override void AI()
        {
            Main.npcFrameCount[Type] = 4;
        }

        public override string GetChat()
        {
            return $"*It appears this nimbus is in an angry shock.*";
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = "Calm";
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                Main.npcChatText = "*You attempt to calm the nimbus down, but fail.*";
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return true;
        }
    }

    public class RedSpriteFriendly2 : ModNPC
    {
        public override string Texture => $"{Aequus.VanillaTexture}NPC_{NPCID.AngryNimbus}";

        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Hide = true, };
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 24;
            NPC.friendly = true;
            NPC.lifeMax = 2500;
            NPC.defense = 10;
            NPC.damage = 15;
            NPC.dontTakeDamage = true;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
            NPC.behindTiles = true;
        }

        public override bool CanChat() => true;

        public override void AI()
        {
            Main.npcFrameCount[Type] = 4;
        }

        public override string GetChat()
        {
            return $"This lightning rod is the closest thing I have to a friend anymore, it gave me my only real friend, and you took them away";
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = "Calm";
            button2 = "Rematch";
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (!firstButton)
            {
                NPC.KillEffects();
                NPC.active = false;
                NPC.NewNPCDirect(Terraria.Entity.GetSource_NaturalSpawn(), NPC.Center, ModContent.NPCType<SpaceSquid>());
            }
            else if (Main.npcChatCornerItem > 0)
            {
                var l = FindPetItems(Main.LocalPlayer);
                var petChosen = l.Find((i) => i.type == Main.npcChatCornerItem);
                Main.npcChatCornerItem = 0;
                int buffIndex = Main.LocalPlayer.FindBuffIndex(petChosen.buffType);
                if (buffIndex != -1)
                {
                    Main.LocalPlayer.DelBuff(buffIndex);
                }
                if (petChosen != null)
                {
                    Main.npcChatText = "Oh thank you! I never thought I could feel anything except anger! But today, I learned I can feel happiness too...\nIf you are needing my service, find me near one of those floating islands, otherwise, thank you so-so much!";
                    petChosen.stack--;
                    if (petChosen.stack <= 0)
                    {
                        petChosen.TurnToAir();
                    }
                    SoundEngine.PlaySound(SoundID.Grab);
                }
            }
            else
            {
                Main.npcChatText = "Fine... I will admit that all I want is a friend...";
                var l = FindPetItems(Main.LocalPlayer);
                if (l.Count > 0)
                {
                    var petChosen = Main.rand.Next(l);
                    if (petChosen.IsLightPet())
                    {
                        Main.npcChatText += $" {Lang.GetBuffName(petChosen.buffType)}... they bring a light into my soul I haven't felt before, I wish I could have them be my friend...";
                    }
                    else
                    {
                        Main.npcChatText += $" {Lang.GetBuffName(petChosen.buffType)}... I wish I could have them be my friend...";
                    }
                    Main.npcChatCornerItem = petChosen.type;
                }
            }
        }

        public static List<Item> FindPetItems(Player player)
        {
            var l = new List<Item>();
            for (int i = 0; i < Main.InventorySlotsTotal; i++)
            {
                if (player.inventory[i].IsPet())
                {
                    l.Add(player.inventory[i]);
                }
            }
            if (player.miscEquips[Player.miscSlotPet].IsPet())
            {
                l.Add(player.miscEquips[Player.miscSlotPet]);
            }
            if (player.miscEquips[Player.miscSlotLight].IsPet())
            {
                l.Add(player.miscEquips[Player.miscSlotLight]);
            }
            return l;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return true;
        }
    }
}