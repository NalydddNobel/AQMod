﻿using Aequu2.Core;
using Aequu2.Content.Dusts;
using Aequu2.Content.Events.GaleStreams;
using Aequu2.Core.Assets;
using Aequu2.Core.CodeGeneration;
using Aequu2.Core.ContentGeneration;
using Aequu2.Core.Entities.Bestiary;
using Aequu2.DataSets;
using Aequu2.Old.Content.Bosses.SpaceSquid.Projectiles;
using Aequu2.Old.Core;
using ReLogic.Content;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;

namespace Aequu2.Old.Content.Bosses.SpaceSquid;

[Gen.Aequu2System_WorldField<bool>("downedSpaceSquid")]
[BestiaryBiome<GaleStreamsZone>()]
[AutoloadBossHead()]
public class SpaceSquid : UnifiedBoss {
    public const int FramesX = 8;

    public const int PHASE_DEAD = -2;
    public const int PHASE_GOODBYE = -1;
    public const int PHASE_SPACEGUN = 1;
    public const int PHASE_TRANSITION_CHANGEDIRECTION = 2;
    public const int PHASE_SNOWFLAKESPIRAL = 3;

    public Asset<Texture2D> GlowmaskTexture => Aequu2Textures.SpaceSquid_Glow;
    public Asset<Texture2D> DefeatedTexture => Aequu2Textures.SpaceSquidDefeated;
    public Asset<Texture2D> DefeatedGlowTexture => Aequu2Textures.SpaceSquidDefeated_Glow;

    public static SoundStyle SpaceGunSound => Aequu2Sounds.SpaceSquidGun;
    public static SoundStyle SnowflakeShootSound => Aequu2Sounds.SpaceSquidSnowflakeShoot;
    public static SoundStyle AwesomeDeathraySound => Aequu2Sounds.SpaceSquidDeathray;

    public int frameIndex;

    private bool _setupFrame;

    private bool _importantDeath;
    private bool _magicMirrorSound;

    private float _brightnessTimer;
    private float _brightness;

    public SpaceSquid() : base(new BossParams() {
        ItemRarity = Commons.Rare.EventGaleStreams,
    }) { }

    public override void Load() {
        LoadTrophy(Aequu2Textures.SpaceSquidTrophy.Path);
        LoadRelic(new RelicRenderer(Aequu2Textures.SpaceSquidRelic.Path));
        LoadMask(Aequu2Textures.SpaceSquidMask.Path);
    }

    public override void SetStaticDefaults() {
        Main.npcFrameCount[NPC.type] = 6;
        NPCSets.NPCBestiaryDrawOffset.Add(Type, new() {
            Position = new Vector2(24f, 0f),
        });

        NPCSets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        NPCSets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
        NPCSets.SpecificDebuffImmunity[Type][BuffID.Frostburn2] = true;
        NPCSets.SpecificDebuffImmunity[Type][BuffID.Chilled] = true;
        NPCSets.SpecificDebuffImmunity[Type][BuffID.Frozen] = true;
        //NPCSets.SpecificDebuffImmunity[Type][ModContent.BuffType<SnowgraveDebuff>()] = true;

        NPCDataSet.PrefixBlacklist.Add(Type);
        NPCDataSet.NoSnowgraveEffects.Add(Type);
    }

    public override void SetDefaults() {
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
        NPC.noTileCollide = true;

        _brightness = 0.2f;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment) {
        NPC.lifeMax = (int)(NPC.lifeMax * (0.6f + 0.3f * numPlayers));
        NPC.damage = (int)(NPC.damage * 0.8);
    }

    public override Color? GetAlpha(Color drawColor) {
        byte minimum = (byte)(int)(255 * _brightness);
        if (drawColor.R < minimum) {
            drawColor.R = minimum;
        }
        if (drawColor.G < minimum) {
            drawColor.G = minimum;
        }
        if (drawColor.B < minimum) {
            drawColor.B = minimum;
        }
        return drawColor;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot) {
        return false;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry).QuickUnlock();
    }

    public override void HitEffect(NPC.HitInfo hit) {
        if (Main.netMode == NetmodeID.Server)
            return;
        var center = NPC.Center;
        if (NPC.life < 0) {
            for (int i = 0; i < 60; i++) {
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
        else {
            for (int i = 0; i < hit.Damage / 100; i++) {
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

    public override void AI() {
        bool leave = (int)NPC.ai[0] == -1;
        if (!leave && !Helper.ZoneSkyHeight(Main.player[NPC.target].position.Y)) {
            leave = true;
        }
        else if ((int)NPC.ai[0] == 0) {
            NPC.TargetClosest(faceTarget: false);
            if (!NPC.HasValidTarget) {
                NPC.ai[0] = -1;
                leave = true;
            }
        }
        if (leave) {
            if (NPC.timeLeft < 20) {
                NPC.timeLeft = 20;
            }
            NPC.velocity.Y -= 0.25f;
            return;
        }
        var center = NPC.Center;
        if ((int)NPC.ai[0] == 0) {
            if (!NPC.HasValidTarget && !NPC.TryRetargeting()) {
                NPC.ai[0] = PHASE_GOODBYE;
                return;
            }
            if (Main.player[NPC.target].position.X + Main.player[NPC.target].width / 2f < NPC.position.X + NPC.width / 2f) {
                NPC.direction = -1;
            }
            else {
                NPC.direction = 1;
            }
            NPC.spriteDirection = NPC.direction;
            AI_AdvancePhase();
            NPC.velocity = -Vector2.Normalize(Main.player[NPC.target].Center - center) * 20f;
        }
        if (!NPC.HasValidTarget && !NPC.TryRetargeting()) {
            NPC.ai[0] = PHASE_GOODBYE;
            return;
        }
        switch ((int)NPC.ai[0]) {
            case PHASE_SPACEGUN: {
                    bool runOtherAis = true;
                    bool noDeathray = true;
                    if ((int)NPC.ai[3] == 0) {
                        if (NPC.life * 2 < NPC.lifeMax) {
                            NPC.ai[3] = 2f;
                        }
                        else {
                            NPC.ai[3] = 1f;
                        }
                    }
                    if (Main.expertMode && (int)NPC.ai[3] == 2) {
                        noDeathray = false;
                        if ((int)NPC.ai[1] == 202) {
                            NPC.ai[2] = 0f;
                            if (Main.netMode != NetmodeID.Server && (Main.player[Main.myPlayer].Center - center).Length() < 2000f) {
                                SoundEngine.PlaySound(AwesomeDeathraySound);
                            }
                        }
                        bool doEffects = NPC.Distance(DrawHelper.ScreenCenter) < 2000f;
                        if (NPC.ai[1] >= 242f && (int)NPC.ai[2] < 1 && doEffects) {
                            ScreenFlash.Flash.Set(NPC.Center, 0.75f);
                            SkyDarkness.DarknessTransition(0f, 0.05f);
                        }
                        if ((int)NPC.ai[1] >= 245) {
                            runOtherAis = false;
                            if ((int)NPC.ai[2] < 1) {
                                if (doEffects) {
                                    ScreenShake.SetShake(12f);
                                }
                                NPC.ai[2]++;
                                NPC.velocity.X = -NPC.direction * 12.5f;
                                if (Main.netMode != NetmodeID.MultiplayerClient) {
                                    int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), GetEyePos(), new Vector2(0f, 0f), ModContent.ProjectileType<SpaceSquidDeathray>(), 70, 1f, Main.myPlayer);
                                    Main.projectile[p].ai[0] = NPC.whoAmI + 1;
                                    Main.projectile[p].direction = NPC.direction;
                                }
                            }
                            if (NPC.velocity.Length() > 2f) {
                                NPC.velocity *= 0.92f;
                                if (NPC.velocity.Length() < 2f) {
                                    NPC.velocity = Vector2.Normalize(NPC.velocity) * 2f;
                                }
                            }
                        }
                        else if ((int)NPC.ai[1] > 200) {
                            var eyePos = GetEyePos();
                            Dust.NewDustPerfect(eyePos, ModContent.DustType<MonoDust>(), new Vector2(0f, 0f), 0, new Color(10, 255, 80, 0), 0.9f);
                            int spawnChance = 3 - (int)(NPC.ai[1] - 210) / 8;
                            if (spawnChance <= 1 || Main.rand.NextBool(spawnChance)) {
                                var spawnPos = eyePos + new Vector2(Main.rand.NextFloat(-60f, 60f), Main.rand.NextFloat(-60f, 60f));
                                Dust.NewDustPerfect(spawnPos, ModContent.DustType<MonoDust>(), (eyePos - spawnPos) / 8f + NPC.velocity, 0, new Color(10, 200, 80, 0), Main.rand.NextFloat(0.9f, 1.35f));
                            }
                            if (spawnChance <= 1) {
                                var spawnPos = eyePos + new Vector2(Main.rand.NextFloat(-120f, 120f), Main.rand.NextFloat(-120f, 120f));
                                Dust.NewDustPerfect(spawnPos, ModContent.DustType<MonoDust>(), (eyePos - spawnPos) / 12f + NPC.velocity, 0, new Color(10, 200, 80, 0), Main.rand.NextFloat(0.5f, 0.75f));
                            }
                        }
                        if ((int)NPC.ai[1] >= 330) {
                            NPC.ai[2] = 0f;
                            NPC.ai[3] = 0f;
                            AI_AdvancePhase(PHASE_SPACEGUN);
                        }
                    }
                    if (runOtherAis) {
                        if (NPC.ai[1] >= 120f) {
                            if ((int)NPC.ai[1] >= 200) {
                                if (noDeathray && (int)NPC.ai[1] >= 240) {
                                    if (Main.player[NPC.target].position.X + Main.player[NPC.target].width / 2f < NPC.position.X + NPC.width / 2f) {
                                        NPC.direction = -1;
                                    }
                                    else {
                                        NPC.direction = 1;
                                    }
                                    NPC.ai[2] = 0f;
                                    NPC.ai[3] = 0f;
                                    AI_AdvancePhase(PHASE_SPACEGUN);
                                    NPC.velocity *= 0.95f;
                                }
                            }
                            else {
                                int timeBetweenShots = 10;
                                if (Main.getGoodWorld) {
                                    timeBetweenShots /= 3;
                                }
                                int timer = (int)(NPC.ai[1] - 120) % timeBetweenShots;
                                if (timer == 0) {
                                    frameIndex = 8;
                                    if (Main.netMode != NetmodeID.Server) {
                                        SoundEngine.PlaySound(SpaceGunSound, NPC.Center);
                                    }
                                    var spawnPosition = new Vector2(NPC.position.X + (NPC.direction == 1 ? NPC.width + 20f : -20), NPC.position.Y + NPC.height / 2f);
                                    var velocity = new Vector2(20f * NPC.direction, 0f);
                                    if (Main.getGoodWorld) {
                                        velocity = velocity.RotatedBy(Math.Sin((NPC.ai[1] - 120) * (MathHelper.TwoPi / 80f)) * MathHelper.PiOver4);
                                        velocity *= 0.5f;
                                    }
                                    if (Main.netMode != NetmodeID.MultiplayerClient) {
                                        var source = NPC.GetSource_FromAI();
                                        Projectile.NewProjectile(source, spawnPosition, velocity, ModContent.ProjectileType<SpaceSquidLaser>(), 30, 1f, Main.myPlayer);
                                        Projectile.NewProjectile(source, spawnPosition, velocity.RotatedBy(MathHelper.PiOver4), ModContent.ProjectileType<SpaceSquidLaser>(), 40, 1f, Main.myPlayer);
                                        Projectile.NewProjectile(source, spawnPosition, velocity.RotatedBy(-MathHelper.PiOver4), ModContent.ProjectileType<SpaceSquidLaser>(), 40, 1f, Main.myPlayer);
                                    }
                                }
                            }
                            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, (Main.player[NPC.target].position.X - NPC.direction * 300f - center.X) / 16f, 0.001f);
                            NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, (Main.player[NPC.target].position.Y + 6f - center.Y) / 8f, 0.01f);
                        }
                        else {
                            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, (Main.player[NPC.target].position.X - NPC.direction * 300f - center.X) / 16f, 0.05f);
                            NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, (Main.player[NPC.target].position.Y + 6f - center.Y) / 8f, 0.1f);
                        }
                    }
                    NPC.ai[1]++;
                    NPC.rotation = NPC.velocity.X * 0.01f;
                }
                break;

            case PHASE_TRANSITION_CHANGEDIRECTION: {
                    NPC.velocity *= 0.8f;
                    NPC.ai[1]++;
                    if (NPC.ai[1] > 20f) {
                        NPC.spriteDirection = NPC.direction;
                        AI_AdvancePhase((int)NPC.ai[2]);
                    }
                }
                break;

            case PHASE_SNOWFLAKESPIRAL: {
                    var gotoPosition = new Vector2(205f * -NPC.direction, 0f).RotatedBy(NPC.ai[1] * 0.01f);
                    gotoPosition = Main.player[NPC.target].Center + new Vector2(gotoPosition.X * 2f, gotoPosition.Y);
                    if (Main.player[NPC.target].position.X + Main.player[NPC.target].width / 2f < NPC.position.X + NPC.width / 2f) {
                        NPC.direction = -1;
                    }
                    else {
                        NPC.direction = 1;
                    }
                    if (NPC.spriteDirection != NPC.direction) {
                        if (frameIndex >= 24) {
                            frameIndex = 19;
                        }
                    }
                    if ((int)NPC.ai[1] == 0) {
                        NPC.ai[1] = Main.rand.NextFloat(MathHelper.Pi * 100f);
                        NPC.netUpdate = true;
                    }
                    NPC.ai[2]++;
                    if (NPC.ai[2] > 120f) {
                        NPC.ai[1] += 0.2f;
                        if ((int)NPC.ai[3] == 0) {
                            NPC.ai[3] = Main.rand.NextFloat(MathHelper.Pi * 6f);
                            NPC.netUpdate = true;
                        }
                        NPC.ai[3]++;
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(gotoPosition - NPC.Center) * 10f, 0.006f);
                        int timeBetweenShots = 3 + NPC.life / (NPC.lifeMax / 3);
                        if (Main.getGoodWorld) {
                            timeBetweenShots /= 2;
                        }
                        int timer = (int)(NPC.ai[2] - 60) % timeBetweenShots;
                        if (timer == 0) {
                            if (Main.netMode != NetmodeID.MultiplayerClient) {
                                var velocity = new Vector2(10f, 0f).RotatedBy(NPC.ai[3] * 0.12f);
                                if (Main.getGoodWorld) {
                                    velocity = velocity.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f));
                                    velocity *= Main.rand.NextFloat(1.9f, 2.1f);
                                }
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + velocity * 4f, velocity, ModContent.ProjectileType<SpaceSquidSnowflake>(), 20, 1f, Main.myPlayer);
                            }
                            if (Main.netMode != NetmodeID.Server) {
                                SoundEngine.PlaySound(SnowflakeShootSound, NPC.Center);
                            }
                        }
                        if (NPC.ai[2] > 180f + (6 - timeBetweenShots) * 40f) {
                            NPC.ai[2] = 0f;
                            NPC.ai[3] = 0f;
                            if (Main.player[NPC.target].position.X + Main.player[NPC.target].width / 2f < NPC.position.X + NPC.width / 2f) {
                                NPC.direction = -1;
                            }
                            else {
                                NPC.direction = 1;
                            }
                            AI_AdvancePhase(PHASE_SNOWFLAKESPIRAL);
                        }
                    }
                    else {
                        NPC.ai[1]++;
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(gotoPosition - NPC.Center) * 20f, 0.01f);
                    }
                    NPC.rotation = NPC.rotation.AngleLerp(0f, 0.1f);
                }
                break;

            case PHASE_DEAD: {
                    NPC.rotation = MathHelper.WrapAngle(NPC.rotation);
                    NPC.rotation *= 0.8f;
                    NPC.velocity *= 0.8f;
                    if ((int)NPC.ai[1] == 0) {
                        frameIndex = 0;
                    }
                    NPC.ai[1]++;
                    if (NPC.ai[1] > 30f) {
                        if (!_magicMirrorSound && Main.netMode != NetmodeID.Server) {
                            SoundEngine.PlaySound(SoundID.Item6, NPC.Center);
                            _magicMirrorSound = true;
                        }

                        if (Main.rand.NextBool()) {
                            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.MagicMirror, 0f, 0f, 150, default(Color), 1.1f);
                        }

                        if (NPC.ai[1] > 30f + 45f) {
                            if (Main.netMode != NetmodeID.MultiplayerClient) {
                                NPC.NPCLoot();
                            }
                            for (int d = 0; d < 70; d++) {
                                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.MagicMirror, 0f, 0f, 150, default(Color), 1.5f);
                            }
                            NPC.active = false;
                            NPC.life = -1;
                        }
                    }
                }
                break;
        }
    }
    private void AI_AdvancePhase(int curPhase = -1) {
        NPC.TargetClosest(faceTarget: false);
        NPC.netUpdate = true;
        if (!NPC.HasValidTarget) {
            NPC.ai[0] = PHASE_GOODBYE;
            return;
        }
        if (curPhase != PHASE_TRANSITION_CHANGEDIRECTION && NPC.direction != NPC.spriteDirection) {
            NPC.ai[1] = 0f;
            NPC.ai[2] = NPC.ai[0];
            NPC.ai[0] = PHASE_TRANSITION_CHANGEDIRECTION;
            frameIndex = 19;
            return;
        }
        int[] selectablePhases = new int[] { PHASE_SPACEGUN, PHASE_SNOWFLAKESPIRAL };
        for (int i = 0; i < 50; i++) {
            NPC.ai[0] = selectablePhases[Main.rand.Next(selectablePhases.Length)];
            if ((int)NPC.ai[0] != curPhase) {
                break;
            }
        }
        NPC.ai[1] = 0f;
    }

    public override void FindFrame(int frameHeight) {
        if (Main.netMode == NetmodeID.Server)
            return;
        _brightnessTimer += 0.1f;

        float brightnessMax = 0.5f;
        if (!_setupFrame) {
            _setupFrame = true;
            NPC.frame.Width = NPC.frame.Width / FramesX;
        }
        if (NPC.IsABestiaryIconDummy) {
            _brightness = Helper.Oscillate(_brightnessTimer, 0.2f, brightnessMax);
            return;
        }

        switch ((int)NPC.ai[0]) {
            default: {
                    NPC.frameCounter += 1.0d;
                    if (NPC.frameCounter > 2.0d) {
                        NPC.frameCounter = 0.0d;
                        frameIndex++;
                        if (frameIndex >= Main.npcFrameCount[NPC.type] * FramesX) {
                            frameIndex = 0;
                        }
                    }
                }
                break;

            case PHASE_GOODBYE:
                frameIndex = 20;
                break;

            case PHASE_SPACEGUN: {
                    if ((int)NPC.ai[1] >= 200) {
                        if (frameIndex < 13) {
                            frameIndex = 13;
                        }
                        NPC.frameCounter += 1.0d;
                        if (NPC.frameCounter > 4.0d) {
                            NPC.frameCounter = 0.0d;
                            frameIndex++;
                            if (frameIndex > 18) {
                                frameIndex = 18;
                            }
                        }
                    }
                    else if (frameIndex > 13) {
                        NPC.frameCounter += 1.0d;
                        if (frameIndex > 18) {
                            frameIndex = 18;
                        }
                        if (NPC.frameCounter > 4.0d) {
                            NPC.frameCounter = 0.0d;
                            frameIndex--;
                            if (frameIndex == 13) {
                                frameIndex = 0;
                            }
                        }
                    }
                    else if (frameIndex > 7) {
                        NPC.frameCounter += 1.0d;
                        if (NPC.frameCounter > 3.0d) {
                            NPC.frameCounter = 0.0d;
                            frameIndex++;
                            if (frameIndex > 12) {
                                frameIndex = 12;
                            }
                        }
                    }
                    else {
                        NPC.frameCounter += 1.0d;
                        if (NPC.frameCounter > 4.0d) {
                            NPC.frameCounter = 0.0d;
                            frameIndex++;
                            if (frameIndex > 7) {
                                frameIndex = 0;
                            }
                        }
                    }
                }
                break;

            case PHASE_TRANSITION_CHANGEDIRECTION: {
                    if (frameIndex < 19) {
                        frameIndex = 19;
                    }
                    NPC.frameCounter += 1.0d;
                    if (NPC.frameCounter > 4.0d) {
                        NPC.frameCounter = 0.0d;
                        frameIndex++;
                        if (frameIndex > 23) {
                            frameIndex = 23;
                        }
                    }
                }
                break;

            case PHASE_SNOWFLAKESPIRAL: {
                    if (NPC.direction != NPC.spriteDirection) {
                        if (frameIndex < 19) {
                            frameIndex = 19;
                        }
                        NPC.frameCounter += 1.0d;
                        if (NPC.frameCounter > 4.0d) {
                            NPC.frameCounter = 0.0d;
                            frameIndex++;
                            if (frameIndex > 23) {
                                frameIndex = 24;
                                NPC.spriteDirection = NPC.direction;
                            }
                        }
                    }
                    else {
                        if (frameIndex < 24) {
                            frameIndex = 24;
                        }
                        NPC.frameCounter += 1.0d;
                        if (NPC.frameCounter > 4.0d) {
                            NPC.frameCounter = 0.0d;
                            frameIndex++;
                            if (frameIndex > 28) {
                                frameIndex = 25;
                            }
                        }
                    }
                }
                break;

            case PHASE_DEAD: {
                    if (frameIndex < 15) {
                        NPC.frameCounter += 1.0d;
                        if (NPC.frameCounter >= 2.0d) {
                            frameIndex++;
                        }
                    }
                    _brightness = Helper.Oscillate(_brightnessTimer, 0.2f, brightnessMax);
                }
                return;
        }

        _brightness = Helper.Oscillate(_brightnessTimer, 0.2f, brightnessMax);
        NPC.frame.Y = frameIndex * frameHeight;

        if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type]) {
            NPC.frame.X = NPC.frame.Width * (NPC.frame.Y / (frameHeight * Main.npcFrameCount[NPC.type]));
            NPC.frame.Y = NPC.frame.Y % (frameHeight * Main.npcFrameCount[NPC.type]);
        }
        else {
            NPC.frame.X = 0;
        }
    }

    public override bool CheckDead() {
        if ((int)NPC.ai[0] == PHASE_DEAD) {
            return true;
        }

        _importantDeath = !Aequu2System.downedSpaceSquid && NPC.CountNPCS(Type) <= 1;
        NPC.ai[0] = PHASE_DEAD;
        NPC.ai[1] = 0f;
        NPC.ai[2] = 0f;
        NPC.ai[3] = 0f;
        NPC.dontTakeDamage = true;
        NPC.life = 1;
        NPC.netUpdate = true;
        frameIndex = 0;
        return false;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        base.ModifyNPCLoot(npcLoot);
        //npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<Pets.SpaceSquidPet>(), 4));
        //npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Materials.FrozenTear>(), minimumDropped: 14, maximumDropped: 24));
    }

    public override void OnKill() {
        ExtendItem.DropHearts(new EntitySource_Loot(NPC), NPC.Hitbox, 4, 4);
        NPC.SetEventFlagCleared(ref Aequu2System.downedSpaceSquid, -1);
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

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        var drawPosition = NPC.Center;
        var effects = SpriteEffects.None;
        if (NPC.spriteDirection == 1) {
            effects = SpriteEffects.FlipHorizontally;
        }
        if ((int)NPC.ai[0] == PHASE_DEAD) {
            DrawDeathSequence(spriteBatch, drawPosition, screenPos, NPC.GetNPCColorTintedByBuffs(drawColor), effects);
            return false;
        }
        var texture = TextureAssets.Npc[Type].Value;
        var origin = new Vector2(NPC.frame.Width / 2f, NPC.frame.Height / 2f);
        Vector2 scale = new Vector2(NPC.scale, NPC.scale);
        float speedX = NPC.velocity.X.Abs();
        if (speedX > 8f) {
            scale.X += (speedX - 8f) / 120f;
            drawPosition.X -= (scale.X - 1f) * 16f;
        }

        DrawBGAura(spriteBatch, texture, drawPosition, screenPos, NPC.frame, NPC.rotation, origin, scale, effects, NPC.IsABestiaryIconDummy);

        spriteBatch.Draw(texture, drawPosition - screenPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, origin, scale, effects, 0f);
        spriteBatch.Draw(GlowmaskTexture.Value, drawPosition - screenPos, NPC.frame, Color.White, NPC.rotation, origin, scale, effects, 0f);

        if (!NPC.IsABestiaryIconDummy && Main.expertMode)
            RenderDeathrayTelegraph(spriteBatch, screenPos);
        return false;
    }
    private void DrawDeathSequence(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, Color drawColor, SpriteEffects effects) {
        int verticalFrames = 6;
        var texture = DefeatedTexture;
        if (!texture.IsLoaded) {
            return;
        }
        var frame = texture.Value.Frame(horizontalFrames: 3, verticalFrames: verticalFrames);
        frame.Y = frameIndex * frame.Height;

        if (frame.Y >= frame.Height * verticalFrames) {
            frame.X = frame.Width * (frame.Y / (frame.Height * verticalFrames));
            frame.Y %= frame.Height * verticalFrames;
        }
        else {
            frame.X = 0;
        }

        frame.Width -= 2;
        frame.Height -= 2;

        var origin = frame.Size() / 2f;

        DrawBGAura(spriteBatch, texture.Value, drawPosition, screenPos, frame, NPC.rotation, origin, new Vector2(NPC.scale), effects, NPC.IsABestiaryIconDummy);

        spriteBatch.Draw(texture.Value, NPC.Center - screenPos, frame, NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, origin, NPC.scale, effects, 0f);
        spriteBatch.Draw(DefeatedGlowTexture.Value, NPC.Center - screenPos, frame, Color.White, NPC.rotation, origin, NPC.scale, effects, 0f);
    }
    private void RenderDeathrayTelegraph(SpriteBatch spriteBatch, Vector2 screenPos) {
        if ((int)NPC.ai[0] == PHASE_SPACEGUN && NPC.ai[1] > 200f && NPC.ai[1] <= 245f && (int)NPC.ai[3] == 2) {
            float opacity = 1f;
            if (NPC.ai[1] < 10f) {
                opacity = (NPC.ai[1] - 200f) / 10f;
            }
            float progress = (NPC.ai[1] - 200f) / 45f;
            var eyePosition = GetEyePos(NPC.position);
            eyePosition.X -= NPC.direction * 6f;
            eyePosition.Y -= 2f;
            var scale = new Vector2(10000f, 4f);
            if (NPC.direction == -1) {
                eyePosition.X -= scale.X;
            }
            var pixel = TextureAssets.MagicPixel.Value;
            var frame = new Rectangle(0, 0, 1, 1);
            var laserColor = new Color(10, 200, 80, 0);
            for (int i = 0; i < 8; i++) {
                float progress2 = 1f - 1f / 10f * (i + 2);
                spriteBatch.Draw(pixel, eyePosition + new Vector2(0f, i * scale.Y * (1f - progress)) - screenPos, frame, laserColor * progress2 * opacity, 0f, new Vector2(0f, 0.5f), scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(pixel, eyePosition - new Vector2(0f, i * scale.Y * (1f - progress)) - screenPos, frame, laserColor * progress2 * opacity, 0f, new Vector2(0f, 0.5f), scale, SpriteEffects.None, 0f);
            }

        }
    }
    public static void DrawBGAura(SpriteBatch spriteBatch, Texture2D texture, Vector2 drawPosition, Vector2 screenPos, Rectangle frame, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, bool bestiary = false) {
        int aura = (int)(Helper.Oscillate(Main.GlobalTimeWrappedHourly * 5f, 2f, 8f) * 4f);
        if (aura > 0f) {
            DrawHelper.SpriteBatchCache.InheritFrom(spriteBatch);

            if (bestiary) {
                spriteBatch.BeginUI(immediate: true, useScissorRectangle: true);
            }
            else {
                spriteBatch.BeginWorld(shader: true);
            }

            Aequu2Shaders.LegacyMiscEffects.Value.Parameters["uColor"].SetValue(Color.Cyan.ToVector3());
            Aequu2Shaders.LegacyMiscEffects.Value.Parameters["uSecondaryColor"].SetValue(Color.Blue.ToVector3());
            Aequu2Shaders.LegacyMiscEffects.Value.Parameters["uSourceRect"].SetValue(new Vector4(frame.X, frame.Y, frame.Width, frame.Height));
            Aequu2Shaders.LegacyMiscEffects.Value.Parameters["uImageSize0"].SetValue(new Vector2(texture.Width, texture.Height));
            Aequu2Shaders.LegacyMiscEffects.Value.CurrentTechnique.Passes["VerticalGradientPass"].Apply();

            foreach (var v in Helper.CircularVector(3, Main.GlobalTimeWrappedHourly * 2f)) {
                Main.spriteBatch.Draw(texture, drawPosition - screenPos + v * (aura / 4f), frame, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            DrawHelper.SpriteBatchCache.Begin(spriteBatch, SpriteSortMode.Deferred, transform: bestiary ? Main.UIScaleMatrix : Main.GameViewMatrix.EffectMatrix);
        }
    }

    public Vector2 GetEyePos() {
        return GetEyePos(NPC.position);
    }
    public Vector2 GetEyePos(Vector2 position) {
        return position + new Vector2(NPC.direction == 1 ? NPC.width - 4f : 4f, NPC.height / 2f - 2f);
    }
}

public class SpaceSquidFriendly : ModNPC {
    public override bool IsLoadingEnabled(Mod mod) {
        return false;
    }

    public override void SetStaticDefaults() {
        NPCSets.NPCBestiaryDrawOffset[Type] = new NPCSets.NPCBestiaryDrawModifiers(0) { Hide = true, };
        NPCSets.NoTownNPCHappiness[Type] = true;
        NPCSets.ActsLikeTownNPC[Type] = true;
    }

    public override void SetDefaults() {
        NPC.width = 64;
        NPC.height = 48;
        NPC.friendly = true;
        NPC.lifeMax = 2500;
        NPC.defense = 10;
        NPC.damage = 15;
        NPC.dontTakeDamage = true;
        NPC.dontTakeDamageFromHostiles = true;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0f;
        NPC.behindTiles = true;
        NPC.npcSlots = 5f;
    }

    public override bool CanChat() {
        return true;
    }

    public override void AI() {
    }

    public override string GetChat() {
        return $"HELLO {Main.LocalPlayer.name}. EXCUSE TONE OF VOICE - AS USING [[⏁⍀⏃⋏⌇⌰⏃⏁⍜⍀]] TO TRANSLATE VOICE INTO TERRARIAN VOICE.";
    }

    public override void SetChatButtons(ref string button, ref string button2) {
        button = "Option 1";
        button2 = "Rematch";
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName) {
        if (!firstButton) {
            NPC.KillEffects();
            NPC.active = false;
            NPC.NewNPCDirect(Terraria.Entity.GetSource_NaturalSpawn(), NPC.Center, ModContent.NPCType<SpaceSquid>());
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        return true;
    }
}