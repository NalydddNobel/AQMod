using Aequus.Common;
using Aequus.Common.ItemDrops;
using Aequus.Content.Invasions;
using Aequus.Effects;
using Aequus.Items.Misc;
using Aequus.Items.Misc.Dyes;
using Aequus.Items.Misc.Energies;
using Aequus.Items.Placeable;
using Aequus.Particles.Dusts;
using Aequus.Projectiles.Monster;
using Aequus.Sounds;
using Microsoft.Xna.Framework;
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
    public class SpaceSquid : ModNPC
    {
        private bool _setupFrame;
        public int frameIndex;
        public const int FramesX = 8;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Position = new Vector2(24f, 0f),
            });
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
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.WindyDay,
                new FlavorTextBestiaryInfoElement("Mods.Aequus.Bestiary.SpaceSquid")
            });
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(NPC.GetBestiaryCreditId(), true);
        }

        public override void SetDefaults()
        {
            NPC.width = 80;
            NPC.height = 120;
            NPC.lifeMax = 4000;
            NPC.damage = 45;
            NPC.defense = 15;
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.value = Item.buyPrice(gold: 2);
            NPC.coldDamage = true;
            //Banner = NPC.type;
            //BannerItem = ModContent.ItemType<SpaceSquidBanner>();
            NPC.noTileCollide = true;

            //NPC.GetGlobalNPC<AQNPC>().temperature = -40;
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
            if (Main.netMode == NetmodeID.Server)
                return;
            var center = NPC.Center;
            if (NPC.life < 0)
            {
                for (int i = 0; i < 60; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<SpaceSquidBlood>(), 0f, 0f, 0, default(Color), Main.rand.NextFloat(0.8f, 1.3f));
                    Main.dust[d].velocity = (Main.dust[d].position - center) / 12f;
                }
                //for (int i = 0; i < 10; i++)
                //{
                //    var spawnPos = NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(20));
                //    AQMod.Particles.PostDrawPlayers.AddParticle(new SpaceSquidSnowflake(spawnPos, new Vector2(Main.rand.NextFloat(-4f, 4f), -Main.rand.NextFloat(1f, 4f)), default(Color), Main.rand.NextFloat(0.6f, 1.2f)));
                //}
                //for (int i = 0; i < 3; i++)
                //{
                //    AQGore.NewGore(new Vector2(NPC.position.X + NPC.width / 2f + 10 * (i - 1), NPC.position.Y + NPC.height - 30f), new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(1f, 4f)),
                //        "GaleStreams/spacesquid_2");
                //}

                //AQGore.NewGore(new Vector2(NPC.position.X + (NPC.direction == 1 ? NPC.width : 0), NPC.position.Y + 40f), new Vector2(Main.rand.NextFloat(-4f, 4f), -Main.rand.NextFloat(1f, 4f)),
                //    "GaleStreams/spacesquid_0");
                //AQGore.NewGore(new Vector2(NPC.position.X + NPC.width / 2f, NPC.position.Y + 20f), new Vector2(Main.rand.NextFloat(-4f, 4f), -Main.rand.NextFloat(1f, 4f)),
                //    "GaleStreams/spacesquid_1");
            }
            else
            {
                for (int i = 0; i < damage / 100; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<SpaceSquidBlood>());
                    Main.dust[d].velocity = (Main.dust[d].position - center) / 12f;
                }

                //if (Main.rand.NextBool(20))
                //{
                //    var spawnPos = NPC.position + new Vector2(Main.rand.Next(-10, 10) + NPC.width, Main.rand.Next(20));
                //    AQMod.Particles.PostDrawPlayers.AddParticle(new SpaceSquidSnowflake(spawnPos, new Vector2(Main.rand.NextFloat(-4f, 4f), -Main.rand.NextFloat(1f, 4f))));
                //}
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

        public const int Phase_Goodbye = -1;
        public const int Phase_SpaceGun = 1;
        public const int Phase_ChangeDirection = 2;
        public const int Phase_SnowflakeSpiral = 3;

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
                    NPC.ai[0] = Phase_Goodbye;
                    return;
                }
                if (Main.player[NPC.target].position.X + Main.player[NPC.target].width / 2f < NPC.position.X + NPC.width / 2f)
                {
                    NPC.direction = -1;
                }
                else
                {
                    NPC.direction = 1;
                }
                NPC.spriteDirection = NPC.direction;
                AdvancePhase();
                NPC.velocity = -Vector2.Normalize(Main.player[NPC.target].Center - center) * 20f;
            }
            if (!NPC.HasValidTarget)
            {
                NPC.ai[0] = Phase_Goodbye;
                return;
            }
            switch ((int)NPC.ai[0])
            {
                case Phase_SpaceGun:
                    {
                        bool runOtherAis = true;
                        bool noDeathray = true;
                        if (Main.expertMode && NPC.life * 2 < NPC.lifeMax)
                        {
                            noDeathray = false;
                            if ((int)NPC.ai[1] == 202)
                            {
                                NPC.ai[2] = 0f;
                                if (Main.netMode != NetmodeID.Server && (Main.player[Main.myPlayer].Center - center).Length() < 2000f)
                                {
                                    SoundHelper.Play(SoundType.Sound, "awesomedeathray");
                                }
                            }
                            bool doEffects = AequusHelpers.ShouldDoEffects(center);
                            if (NPC.ai[1] >= 242f && (int)NPC.ai[2] < 1 && doEffects)
                            {
                                ModContent.GetInstance<ModEffects>().SetFlash(NPC.Center, 0.8f, 12f);
                            }
                            if ((int)NPC.ai[1] >= 245)
                            {
                                runOtherAis = false;
                                if ((int)NPC.ai[2] < 1)
                                {
                                    if (doEffects)
                                    {
                                        ModContent.GetInstance<ModEffects>().SetShake(8f, 12f);
                                    }
                                    NPC.ai[2]++;
                                    NPC.velocity.X = -NPC.direction * 12.5f;
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        int p = Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), GetEyePos(), new Vector2(0f, 0f), ModContent.ProjectileType<SpaceSquidDeathray>(), 70, 1f, Main.myPlayer);
                                        Main.projectile[p].ai[0] = NPC.whoAmI + 1;
                                        Main.projectile[p].direction = NPC.direction;
                                    }
                                }
                                if (NPC.velocity.Length() > 2f)
                                {
                                    NPC.velocity *= 0.92f;
                                    if (NPC.velocity.Length() < 2f)
                                    {
                                        NPC.velocity = Vector2.Normalize(NPC.velocity) * 2f;
                                    }
                                }
                            }
                            else if ((int)NPC.ai[1] > 200)
                            {
                                var eyePos = GetEyePos();
                                Dust.NewDustPerfect(eyePos, ModContent.DustType<MonoDust>(), new Vector2(0f, 0f), 0, new Color(10, 255, 20, 0), 0.9f);
                                int spawnChance = 3 - (int)(NPC.ai[1] - 210) / 8;
                                if (spawnChance <= 1 || Main.rand.NextBool(spawnChance))
                                {
                                    var spawnPos = eyePos + new Vector2(Main.rand.NextFloat(-60f, 60f), Main.rand.NextFloat(-60f, 60f));
                                    Dust.NewDustPerfect(spawnPos, ModContent.DustType<MonoDust>(), (eyePos - spawnPos) / 8f + NPC.velocity, 0, new Color(10, 200, 20, 0), Main.rand.NextFloat(0.9f, 1.35f));
                                }
                                if (spawnChance <= 1)
                                {
                                    var spawnPos = eyePos + new Vector2(Main.rand.NextFloat(-120f, 120f), Main.rand.NextFloat(-120f, 120f));
                                    Dust.NewDustPerfect(spawnPos, ModContent.DustType<MonoDust>(), (eyePos - spawnPos) / 12f + NPC.velocity, 0, new Color(10, 200, 20, 0), Main.rand.NextFloat(0.5f, 0.75f));
                                }
                            }
                            if ((int)NPC.ai[1] >= 330)
                            {
                                AdvancePhase(Phase_SpaceGun);
                            }
                        }
                        if (runOtherAis)
                        {
                            if (NPC.ai[1] >= 120f)
                            {
                                if ((int)NPC.ai[1] >= 200)
                                {
                                    if (noDeathray && (int)NPC.ai[1] >= 240)
                                    {
                                        if (Main.player[NPC.target].position.X + Main.player[NPC.target].width / 2f < NPC.position.X + NPC.width / 2f)
                                        {
                                            NPC.direction = -1;
                                        }
                                        else
                                        {
                                            NPC.direction = 1;
                                        }
                                        AdvancePhase(Phase_SpaceGun);
                                        NPC.velocity *= 0.95f;
                                    }
                                }
                                else
                                {
                                    int timer = (int)(NPC.ai[1] - 120) % 10;
                                    if (timer == 0)
                                    {
                                        frameIndex = 8;
                                        if (Main.netMode != NetmodeID.Server)
                                        {
                                            SoundHelper.Play(SoundType.Sound, "spacegun", NPC.Center);
                                        }
                                        var spawnPosition = new Vector2(NPC.position.X + (NPC.direction == 1 ? NPC.width + 20f : -20), NPC.position.Y + NPC.height / 2f);
                                        var velocity = new Vector2(20f * NPC.direction, 0f);
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), spawnPosition, velocity, ModContent.ProjectileType<SpaceSquidLaser>(), 30, 1f, Main.myPlayer);
                                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), spawnPosition, velocity.RotatedBy(MathHelper.PiOver4), ModContent.ProjectileType<SpaceSquidLaser>(), 40, 1f, Main.myPlayer);
                                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), spawnPosition, velocity.RotatedBy(-MathHelper.PiOver4), ModContent.ProjectileType<SpaceSquidLaser>(), 40, 1f, Main.myPlayer);
                                        }
                                    }
                                }
                                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, (Main.player[NPC.target].position.X - NPC.direction * 300f - center.X) / 16f, 0.001f);
                                NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, (Main.player[NPC.target].position.Y + 6f - center.Y) / 8f, 0.01f);
                            }
                            else
                            {
                                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, (Main.player[NPC.target].position.X - NPC.direction * 300f - center.X) / 16f, 0.05f);
                                NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, (Main.player[NPC.target].position.Y + 6f - center.Y) / 8f, 0.1f);
                            }
                        }
                        NPC.ai[1]++;
                        NPC.rotation = NPC.velocity.X * 0.01f;
                    }
                    break;

                case Phase_ChangeDirection:
                    {
                        NPC.velocity *= 0.8f;
                        NPC.ai[1]++;
                        if (NPC.ai[1] > 20f)
                        {
                            NPC.spriteDirection = NPC.direction;
                            AdvancePhase((int)NPC.ai[2]);
                        }
                    }
                    break;

                case Phase_SnowflakeSpiral:
                    {
                        var gotoPosition = new Vector2(205f * -NPC.direction, 0f).RotatedBy(NPC.ai[1] * 0.01f);
                        gotoPosition = Main.player[NPC.target].Center + new Vector2(gotoPosition.X * 2f, gotoPosition.Y);
                        if (Main.player[NPC.target].position.X + Main.player[NPC.target].width / 2f < NPC.position.X + NPC.width / 2f)
                        {
                            NPC.direction = -1;
                        }
                        else
                        {
                            NPC.direction = 1;
                        }
                        if (NPC.spriteDirection != NPC.direction)
                        {
                            if (frameIndex >= 24)
                            {
                                frameIndex = 19;
                            }
                        }
                        if ((int)NPC.ai[1] == 0)
                        {
                            NPC.ai[1] = Main.rand.NextFloat(MathHelper.Pi * 100f);
                            NPC.netUpdate = true;
                        }
                        NPC.ai[2]++;
                        if (NPC.ai[2] > 120f)
                        {
                            NPC.ai[1] += 0.2f;
                            if ((int)NPC.ai[3] == 0)
                            {
                                NPC.ai[3] = Main.rand.NextFloat(MathHelper.Pi * 6f);
                                NPC.netUpdate = true;
                            }
                            NPC.ai[3]++;
                            NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(gotoPosition - NPC.Center) * 10f, 0.006f);
                            int timeBetweenShots = 3 + NPC.life / (NPC.lifeMax / 3);
                            int timer = (int)(NPC.ai[2] - 60) % timeBetweenShots;
                            if (timer == 0)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    var velocity = new Vector2(10f, 0f).RotatedBy(NPC.ai[3] * 0.12f);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center + velocity * 4f, velocity, ModContent.ProjectileType<SpaceSquidSnowflake>(), 20, 1f, Main.myPlayer);
                                }
                                if (Main.netMode != NetmodeID.Server)
                                {
                                    SoundHelper.Play(SoundType.Sound, "combo", NPC.Center);
                                }
                            }
                            if (NPC.ai[2] > 180f + (6 - timeBetweenShots) * 40f)
                            {
                                NPC.ai[2] = 0f;
                                NPC.ai[3] = 0f;
                                if (Main.player[NPC.target].position.X + Main.player[NPC.target].width / 2f < NPC.position.X + NPC.width / 2f)
                                {
                                    NPC.direction = -1;
                                }
                                else
                                {
                                    NPC.direction = 1;
                                }
                                AdvancePhase(Phase_SnowflakeSpiral);
                            }
                        }
                        else
                        {
                            NPC.ai[1]++;
                            NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(gotoPosition - NPC.Center) * 20f, 0.01f);
                        }
                        NPC.rotation = NPC.rotation.AngleLerp(0f, 0.1f);
                    }
                    break;
            }
        }

        private void AdvancePhase(int curPhase = -1)
        {
            NPC.TargetClosest(faceTarget: false);
            NPC.netUpdate = true;
            if (!NPC.HasValidTarget)
            {
                NPC.ai[0] = Phase_Goodbye;
                return;
            }
            if (curPhase != Phase_ChangeDirection && NPC.direction != NPC.spriteDirection)
            {
                NPC.ai[1] = 0f;
                NPC.ai[2] = NPC.ai[0];
                NPC.ai[0] = Phase_ChangeDirection;
                frameIndex = 19;
                return;
            }
            int[] selectablePhases = new int[] { Phase_SpaceGun, Phase_SnowflakeSpiral };
            for (int i = 0; i < 50; i++)
            {
                NPC.ai[0] = selectablePhases[Main.rand.Next(selectablePhases.Length)];
                if ((int)NPC.ai[0] != curPhase)
                {
                    break;
                }
            }
            NPC.ai[1] = 0f;
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

                case Phase_Goodbye:
                    frameIndex = 20;
                    break;

                case Phase_SpaceGun:
                    {
                        if ((int)NPC.ai[1] >= 200)
                        {
                            if (frameIndex < 13)
                            {
                                frameIndex = 13;
                            }
                            NPC.frameCounter += 1.0d;
                            if (NPC.frameCounter > 4.0d)
                            {
                                NPC.frameCounter = 0.0d;
                                frameIndex++;
                                if (frameIndex > 18)
                                {
                                    frameIndex = 18;
                                }
                            }
                        }
                        else if (frameIndex > 13)
                        {
                            NPC.frameCounter += 1.0d;
                            if (frameIndex > 18)
                            {
                                frameIndex = 18;
                            }
                            if (NPC.frameCounter > 4.0d)
                            {
                                NPC.frameCounter = 0.0d;
                                frameIndex--;
                                if (frameIndex == 13)
                                {
                                    frameIndex = 0;
                                }
                            }
                        }
                        else if (frameIndex > 7)
                        {
                            NPC.frameCounter += 1.0d;
                            if (NPC.frameCounter > 3.0d)
                            {
                                NPC.frameCounter = 0.0d;
                                frameIndex++;
                                if (frameIndex > 12)
                                {
                                    frameIndex = 12;
                                }
                            }
                        }
                        else
                        {
                            NPC.frameCounter += 1.0d;
                            if (NPC.frameCounter > 4.0d)
                            {
                                NPC.frameCounter = 0.0d;
                                frameIndex++;
                                if (frameIndex > 7)
                                {
                                    frameIndex = 0;
                                }
                            }
                        }
                    }
                    break;

                case Phase_ChangeDirection:
                    {
                        if (frameIndex < 19)
                        {
                            frameIndex = 19;
                        }
                        NPC.frameCounter += 1.0d;
                        if (NPC.frameCounter > 4.0d)
                        {
                            NPC.frameCounter = 0.0d;
                            frameIndex++;
                            if (frameIndex > 23)
                            {
                                frameIndex = 23;
                            }
                        }
                    }
                    break;

                case Phase_SnowflakeSpiral:
                    {
                        if (NPC.direction != NPC.spriteDirection)
                        {
                            if (frameIndex < 19)
                            {
                                frameIndex = 19;
                            }
                            NPC.frameCounter += 1.0d;
                            if (NPC.frameCounter > 4.0d)
                            {
                                NPC.frameCounter = 0.0d;
                                frameIndex++;
                                if (frameIndex > 23)
                                {
                                    frameIndex = 24;
                                    NPC.spriteDirection = NPC.direction;
                                }
                            }
                        }
                        else
                        {
                            if (frameIndex < 24)
                            {
                                frameIndex = 24;
                            }
                            NPC.frameCounter += 1.0d;
                            if (NPC.frameCounter > 4.0d)
                            {
                                NPC.frameCounter = 0.0d;
                                frameIndex++;
                                if (frameIndex > 28)
                                {
                                    frameIndex = 25;
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
            npcLoot.Add(new GuaranteedDropWhenBeatenFlawlessly(ModContent.ItemType<SpaceSquidTrophy>(), 10));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<SpaceSquidRelic>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AtmosphericEnergy>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GelidTentacle>(), 1, 10, 24));
            npcLoot.Add(ItemDropRule.Common(ItemID.SoulofFlight, 1, 2, 6));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrostbiteDye>(), 7));
        }

        public override void OnKill()
        {
            WorldFlags.MarkAsDefeated(ref WorldFlags.downedSpaceSquid);
        }

        //public override void NPCLoot()
        //{
        //    if (NPC.target != -1)
        //        GaleStreams.ProgressEvent(Main.player[NPC.target], 40);

        //    if (Main.rand.NextBool(7))
        //    {
        //        Item.NewItem(NPC.getRect(), ModContent.ItemType<SpaceSquidMask>());
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

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var texture = TextureAssets.Npc[Type].Value;
            var drawPosition = NPC.Center;
            var origin = new Vector2(NPC.frame.Width / 2f, NPC.frame.Height / 2f);
            Vector2 scale = new Vector2(NPC.scale, NPC.scale);
            float aura = 3f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 5f);
            float speedX = NPC.velocity.X.Abs();
            var effects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
            {
                effects = SpriteEffects.FlipHorizontally;
            }
            if (speedX > 8f)
            {
                scale.X += (speedX - 8f) / 120f;
                drawPosition.X -= (scale.X - 1f) * 16f;
            }
            if (aura > 0f)
            {
                var auraTexture = Aequus.Tex(this.GetPath() + "_Aura");
                for (int i = 0; i < 8; i++)
                {
                    Main.spriteBatch.Draw(auraTexture, drawPosition - screenPos + new Vector2(aura, 0f).RotatedBy(MathHelper.PiOver4 * i), NPC.frame, new Color(120, 120, 120, 20), NPC.rotation, origin, scale, effects, 0f);
                }
            }

            Main.spriteBatch.Draw(texture, drawPosition - screenPos, NPC.frame, drawColor, NPC.rotation, origin, scale, effects, 0f);
            Main.spriteBatch.Draw(Aequus.Tex(this.GetPath() + "_Glow"), drawPosition - screenPos, NPC.frame, Color.White, NPC.rotation, origin, scale, effects, 0f);
            return false;
        }

        internal Vector2 GetEyePos()
        {
            if (NPC.direction == -1)
            {
                return NPC.position + new Vector2(4f, NPC.height / 2f - 2f);
            }
            return NPC.position + new Vector2(NPC.width - 4f, NPC.height / 2f - 2f);
        }
    }
}