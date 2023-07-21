using Aequus;
using Aequus.Common;
using Aequus.Common.Effects;
using Aequus.Common.Items.DropRules;
using Aequus.Common.NPCs;
using Aequus.Common.Utilities;
using Aequus.Content.Biomes.CrabCrevice;
using Aequus.Items.Consumables.Permanent;
using Aequus.Items.Consumables.TreasureBag;
using Aequus.Items.Equipment.Vanity.Masks;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Weapons.Ranged.Misc.JunkJet;
using Aequus.NPCs.BossMonsters.Crabson.Projectiles;
using Aequus.NPCs.BossMonsters.Crabson.Rewards;
using Aequus.NPCs.Town.ExporterNPC;
using Aequus.Particles;
using Aequus.Tiles.Furniture.Boss.Relics;
using Aequus.Tiles.Furniture.Boss.Trophies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Aequus.NPCs.BossMonsters.Crabson.Crabson;

namespace Aequus.NPCs.BossMonsters.Crabson.CrabsonOld {
    [AutoloadBossHead]
    public class CrabsonOld : AequusBoss, ICrabson {
        public override string Texture => AequusTextures.Crabson.Path;
        public const float BossProgression = 2.66f;

        public const int ACTION_CLAWSHOTS = 2;
        public const int PHASE_GROUNDBUBBLES = 3;
        public const int ACTION_CLAWSLAMS = 4;
        public const int PHASE2_GROUNDBUBBLES_SPAMMY = 5;
        public const int ACTION_P2_CLAWSHOTS_SHRAPNEL = 6;

        public EyeManager eyeManager;
        public WalkManager walkManager;
        public ArmsManager arms;

        public int leftClaw;
        public int rightClaw;
        public int crabson;
        public bool dealContactDamage;

        public float mouthAnimation;

        public int MainAction => (int)Main.npc[crabson].ai[0];
        public NPC HandLeft => Main.npc[leftClaw];
        public NPC HandRight => Main.npc[rightClaw];
        public NPC CrabsonNPC => Main.npc[crabson];
        public bool IsClaw => NPC.whoAmI != crabson;
        public bool PhaseTwo => Main.npc[NPC.realLife].life * (Main.expertMode ? 2f : 4f) <= NPC.lifeMax;
        public static ConfiguredMusicData music { get; private set; }

        public override void Load() {
            if (!Main.dedServ) {
                music = new ConfiguredMusicData(MusicID.Boss3, MusicID.OtherworldlyBoss1);
            }
        }

        public override void Unload() {
            music = null;
        }

        public override void SetStaticDefaults() {
            Main.npcFrameCount[Type] = 4;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData() {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Wet,
                    BuffID.Confused,
                    BuffID.Suffocation,
                    BuffID.Lovestruck,
                }
            });

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers(0) { PortraitPositionYOverride = 48f, });

            SnowgraveCorpse.NPCBlacklist.Add(Type);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
            this.CreateEntry(database, bestiaryEntry);
        }

        public override void SetDefaults() {
            NPC.width = 90;
            NPC.height = 60;
            NPC.lifeMax = 2500;
            NPC.damage = 10;
            NPC.defense = 6;
            NPC.aiStyle = -1;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.boss = true;
            NPC.behindTiles = true;
            NPC.value = Item.buyPrice(gold: 2);
            NPC.lavaImmune = true;
            NPC.trapImmune = true;

            if (!Main.dedServ && music != null) {
                Music = music.GetID();
                SceneEffectPriority = SceneEffectPriority.BossLow;
            }

            this.SetBiome<CrabCreviceBiome>();

            crabson = -1;
            leftClaw = -1;
            rightClaw = -1;
            eyeManager = new();
            walkManager = new();
            arms = new();
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment) {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance);
        }

        public override void HitEffect(NPC.HitInfo hit) {
            if (NPC.life <= 0) {
                if (Main.netMode != NetmodeID.Server) {
                    for (int i = 0; i < 50; i++) {
                        Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
                    }
                }
            }
            else {
                for (int i = 0; i < Math.Min(hit.Damage / 20 + 1, 1); i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) {
            return dealContactDamage;
        }

        public override void AI() {
            AequusSystem.CrabsonNPC = NPC.whoAmI;
            AequusNPC.ForceZen(NPC);
            if (NPC.alpha > 0) {
                NPC.alpha -= 5;
                if (NPC.alpha < 0) {
                    NPC.alpha = 0;
                }
            }
            int loops = Main.getGoodWorld ? 2 : 1;
            for (int k = 0; k < loops; k++) {
                if (k > 0) {
                    NPC.position += NPC.velocity;
                }
                Vector2 center = NPC.Center;
                if (Action == ACTION_INIT && Main.netMode != NetmodeID.MultiplayerClient) {
                    BodyInit();
                    return;
                }
                if (MainAction == ACTION_GOODBYE) {
                    BodyGoodbye();
                    return;
                }
                if (CheckClaws()) {
                    NPC.realLife = crabson;
                    if (!IsClaw) {
                        dealContactDamage = false;
                        NPC.behindTiles = true;
                        if (!NPC.HasValidTarget || NPC.Distance(Main.player[NPC.target].Center) > 4000f) {
                            NPC.TargetClosest(faceTarget: false);
                            if (!NPC.HasValidTarget || NPC.Distance(Main.player[NPC.target].Center) > 4000f) {
                                SetGoodbyeState();
                                return;
                            }
                        }

                        switch (Action) {
                            case ACTION_INTRO:
                                BodyIntro();
                                break;

                            case ACTION_CLAWSHOTS:
                            case ACTION_CLAWSLAMS:
                            case ACTION_P2_CLAWSHOTS_SHRAPNEL:
                                BodyMovement();
                                break;

                            case PHASE_GROUNDBUBBLES:
                            case PHASE2_GROUNDBUBBLES_SPAMMY:
                                BodyGroundBubblesAttack();
                                break;
                        }
                    }
                    else {
                        dealContactDamage = false;
                        NPC.behindTiles = false;
                        NPC.noGravity = true;
                        NPC.noTileCollide = true;

                        switch (MainAction) {
                            case ACTION_CLAWSHOTS:
                            case ACTION_P2_CLAWSHOTS_SHRAPNEL:
                                ClawShots(MainAction == ACTION_P2_CLAWSHOTS_SHRAPNEL);
                                break;

                            case ACTION_CLAWSLAMS:
                                ClawSlams();
                                break;

                            default:
                                NPC.Center = Vector2.Lerp(NPC.Center, ClawIdlePosition(), CrabsonNPC.ai[0] == ACTION_INTRO ? 0.5f : 0.1f);
                                NPC.velocity *= 0.9f;
                                break;
                        }

                        if (crabson == -1 || !Main.npc[crabson].active || Main.npc[crabson].type != NPC.type) {
                            Kill();
                        }
                        else {
                            NPC.target = CrabsonNPC.target;
                        }
                    }
                }
                else {
                    if (!IsClaw) {
                        for (int i = 0; i < Main.maxNPCs; i++) {
                            if (i == NPC.whoAmI || i == leftClaw || i == rightClaw) {
                                continue;
                            }
                            if (Main.npc[i].active && Main.npc[i].type == NPC.type) {
                                var crab = (CrabsonOld)NPC.ModNPC;
                                crab.crabson = NPC.whoAmI;
                                if (leftClaw == -1) {
                                    crab.leftClaw = i;
                                    crab.rightClaw = rightClaw;
                                }
                                else if (rightClaw == -1) {
                                    crab.rightClaw = i;
                                    crab.leftClaw = leftClaw;
                                    return;
                                }
                                else {
                                    return;
                                }
                            }
                        }
                        if (!CheckClaws()) {
                            Kill();
                        }
                    }
                    else {
                        if (crabson == -1 || !Main.npc[crabson].active || Main.npc[crabson].type != NPC.type) {
                            Kill();
                        }
                    }
                }
            }
        }
        public void ClawSlams() {
            dealContactDamage = true;
            NPC.ai[1]++;
            int time = 10;
            if (NPC.ai[1] + time < 0f) {
                NPC.noGravity = false;
                NPC.noTileCollide = false;
            }
            else {
                int smashTime = Main.expertMode ? 45 : 75;
                if (NPC.ai[1] > smashTime) {
                    NPC.damage = NPC.defDamage * 2;
                    NPC.velocity.X = 0f;
                    int fallingTime = Main.expertMode ? 10 : 20;
                    float max = Main.expertMode ? 50f : 20f;
                    NPC.velocity.Y = Math.Min(NPC.velocity.Y + max / fallingTime, max);
                    if (NPC.ai[1] > smashTime + fallingTime) {
                        NPC.noGravity = true;
                        NPC.noTileCollide = false;
                        if (Main.netMode != NetmodeID.Server && (int)NPC.localAI[0] == 0) {
                            if (Main.netMode != NetmodeID.Server) {
                                SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
                            }
                            //AQMod.Effects.SetShake(2f, 6f);
                        }
                        NPC.localAI[0] = 1f;
                    }
                }
                else {
                    NPC.damage = 0;
                    var gotoPos = Main.player[NPC.target].Center;
                    if (NPC.position.Y < Main.player[NPC.target].position.Y - 80) {
                        gotoPos += new Vector2(0f, -320f - NPC.height);
                    }
                    else {
                        gotoPos += new Vector2(200f * NPC.direction, -120f - NPC.height);
                    }
                    NPC.Center = Vector2.Lerp(NPC.Center, gotoPos, 0.05f + 0.75f * (NPC.ai[0] / smashTime));
                }
            }
        }
        public void ClawShots(bool shrapnel) {
            var gotoPosition = Main.player[NPC.target].Center + new Vector2(400f * NPC.direction, 0f);

            var difference = gotoPosition - NPC.Center;
            float l = difference.Length();
            if (CrabsonNPC.ai[1] > 30f) {
                NPC.ai[1]++;
                int shootTime = Main.expertMode ? 80 : 160;
                if (NPC.whoAmI == leftClaw) {
                    if (HandRight.ai[1] >= NPC.ai[1] - 5f) {
                        NPC.ai[1] = HandRight.ai[1] - shootTime / 2f;
                    }
                }
                if (NPC.ai[1] > shootTime / 2f) {
                    mouthAnimation = MathHelper.Lerp(mouthAnimation, -0.4f, 0.3f);
                }
                if (NPC.ai[1] > shootTime) {
                    NPC.ai[1] = 1f;
                    NPC.ai[2] = 0.5f;
                    NPC.position.X += 40f * NPC.direction;
                    NPC.velocity = new Vector2(6f * NPC.direction, 0f);

                    if (Main.netMode != NetmodeID.Server) {
                        SoundEngine.PlaySound(SoundID.Item61, NPC.Center);
                    }

                    ShootProj<CrabsonPearl>(NPC.Center, new Vector2(20f * -NPC.direction, 0f), NPC.damage, ai1: shrapnel ? 1f : 0f);
                }
            }
            if (CrabsonNPC.ai[1] < 30f) {
                NPC.rotation *= 0.6f;
                if (NPC.rotation < 0.05f) {
                    NPC.rotation = 0f;
                }
                mouthAnimation *= 0.9f;
                if (mouthAnimation < 0.05f) {
                    mouthAnimation = 0f;
                }
            }
            if (l < 0.01f) {
                NPC.Center = gotoPosition;
                NPC.velocity = Vector2.Zero;
                NPC.ai[2] = 0f;
            }
            else {
                if (NPC.ai[2] > 0f) {
                    NPC.ai[2] -= 0.05f;
                    if (NPC.ai[2] < 0f) {
                        NPC.ai[2] = 0f;
                    }
                }
                NPC.velocity = Vector2.Lerp(NPC.velocity, difference / 20f,
                    Math.Max(0.3f - NPC.ai[2], 0f));
            }
        }

        public void BodyGoodbye() {
            if (NPC.timeLeft > 20) {
                NPC.timeLeft = 20;
            }
            NPC.velocity.X *= 0.9f;
            if (NPC.velocity.Y < 0f) {
                NPC.velocity.Y *= 0.8f;
            }
            HandRight.rotation *= 0.8f;
            HandLeft.rotation *= 0.8f;
            NPC.velocity.Y += 0.1f;
        }
        public void BodyGroundBubblesAttack() {
            NPC.ai[1]--;
            var tileCoordinates = Main.player[NPC.target].Center.ToTileCoordinates();
            int randomness = Main.expertMode ? 4 : 3;
            tileCoordinates.X += Main.rand.Next(-randomness, randomness);
            bool toPlayer = true;
            int bubbleTile = 40;
            int start = Main.rand.Next(5, 12);
            for (int j = start; j < 40; j++) {
                var tile = Main.tile[tileCoordinates.X, tileCoordinates.Y + j];
                if (tile.HasTile) {
                    if (bubbleTile == 40 && tile.SolidType()) {
                        bubbleTile = j;
                    }
                    if (j > 20 && (tile.SolidType() || tile.SolidTopType())) {
                        GroundMovement(new Vector2(tileCoordinates.X * 16f + 8f, (tileCoordinates.Y + j) * 16f + NPC.height + 20f));
                        toPlayer = false;
                        break;
                    }
                }
            }
            if (toPlayer) {
                GroundMovement(Main.player[NPC.target].Center + new Vector2(0f, 320f + NPC.height));
            }
            if (NPC.ai[1] <= 0f) {
                RandomizePhase((int)NPC.ai[0]);
            }
            NPC.ai[2]++;
            float time = Main.expertMode ? 40f : 80f;
            if ((int)NPC.ai[0] == PHASE2_GROUNDBUBBLES_SPAMMY) {
                time -= 36f * (1f - NPC.life / NPC.lifeMax);
            }
            if (NPC.ai[2] > time) {
                NPC.ai[2] = 0f;
                float shootTime = Main.expertMode ? 30f - (PhaseTwo ? 8f : 0f) : 40f;
                if ((int)NPC.ai[0] == PHASE2_GROUNDBUBBLES_SPAMMY && Main.rand.NextBool()) {
                    var spawnPos = new Vector2((tileCoordinates.X + Main.rand.Next(-3, 3)) * 16f + 8f, (tileCoordinates.Y - Main.rand.Next(12, 20)) * 16f - 4f);
                    ShootProj<CrabsonBubble>(spawnPos, Vector2.Normalize(Main.player[NPC.target].Center - spawnPos) * 0.01f, NPC.damage, ai0: shootTime / 2f, alpha: 1);
                }
                else {
                    if (bubbleTile != 40) {
                        ShootProj<CrabsonBubble>(new Vector2(tileCoordinates.X * 16f + 8f, (tileCoordinates.Y + bubbleTile) * 16f - 4f), new Vector2(0f, -0.01f), NPC.damage, ai0: shootTime);
                    }
                    else {
                        if (Main.netMode != NetmodeID.Server) {
                            SoundEngine.PlaySound(SoundID.Item85.WithVolume(0.7f), NPC.Center);
                        }
                        ShootProj<CrabsonBubble>(new Vector2(NPC.position.X + NPC.width / 2f, NPC.position.Y - 4f), new Vector2(0f, -0.01f), NPC.damage, ai0: 1f);
                    }
                }
            }
        }
        public void BodyMovement() {
            NPC.ai[1]--;
            GroundMove();
            if (NPC.ai[1] <= 0f) {
                RandomizePhase((int)NPC.ai[0]);
            }
        }
        public void BodyIntro() {
            GroundMove(out var tileCoords, out var j);
            NPC.ai[1]++;
            if ((NPC.position.Y + NPC.height - (tileCoords.Y + j) * 16f).Abs() < 20f || NPC.ai[1] > 60f) {
                NPC.ai[0] = ACTION_CLAWSHOTS;
                NPC.ai[1] = Main.expertMode ? 240f : 320f;
                HandLeft.ai[1] = 0f;
                HandRight.ai[1] = 0f;
                HandLeft.ai[2] = 0f;
                HandRight.ai[2] = 0f;
            }
        }
        public void BodyInit() {
            NPC.ai[0] = ACTION_INTRO;
            NPC.netUpdate = true;
            NPC.TargetClosest(faceTarget: false);
            leftClaw = -1;
            rightClaw = -1;
            crabson = NPC.whoAmI;
            NPC.realLife = NPC.whoAmI;
            for (int i = -1; i <= 1; i += 2) {
                int n = NPC.NewNPC(new EntitySource_Parent(NPC), (int)NPC.position.X + NPC.width / 2, (int)NPC.position.Y + NPC.height / 2, NPC.type);
                if (i == -1) {
                    leftClaw = n;
                }
                else {
                    rightClaw = n;
                }
                Main.npc[n].position.X += 150f * i;
                Main.npc[n].ai[0] = ACTION_INTRO;
                Main.npc[n].direction = i;
                Main.npc[n].spriteDirection = i;
                Main.npc[n].realLife = NPC.whoAmI;
                Main.npc[n].defense *= 4;
                Main.npc[n].defDefense = Main.npc[n].defense;
                Main.npc[n].width -= 24;
                Main.npc[n].height += 30;
                if (Main.getGoodWorld) {
                    float scale = 2f;
                    Main.npc[n].scale *= scale;
                    Main.npc[n].width = (int)(Main.npc[n].width * scale);
                    Main.npc[n].height = (int)(Main.npc[n].height * scale);
                }
            }
            var crab = (CrabsonOld)HandLeft.ModNPC;
            crab.rightClaw = rightClaw;
            crab.leftClaw = leftClaw;
            crab.crabson = crabson;

            crab = (CrabsonOld)HandRight.ModNPC;
            crab.rightClaw = rightClaw;
            crab.leftClaw = leftClaw;
            crab.crabson = crabson;
        }

        public override void FindFrame(int frameHeight) {
            eyeManager.Update(NPC);
            walkManager.Update(NPC);
            if (Main.netMode == NetmodeID.Server || NPC.IsABestiaryIconDummy) {
                return;
            }

            Vector2 chainOffset = new(44f, -14f);
            Vector2 chainEndOffset = new(20f, 0f);
            arms.Clear();
            arms.Update(NPC, NPC.Center + chainOffset with { X = -chainOffset.X }, HandLeft.Center + chainEndOffset.RotatedBy(HandLeft.rotation == 0f ? MathHelper.Pi : HandLeft.rotation));
            arms.Update(NPC, NPC.Center + chainOffset, HandRight.Center + chainEndOffset.RotatedBy(HandRight.rotation));
        }
        private bool CheckClaws() {
            return !(leftClaw == -1 || !HandLeft.active || HandLeft.type != NPC.type ||
                rightClaw == -1 || !HandRight.active || HandRight.type != NPC.type);
        }
        private void Kill() {
            NPC.life = -1;
            NPC.HitEffect();
            NPC.active = false;
        }
        private void SetGoodbyeState() {
            NPC.ai[0] = ACTION_GOODBYE;
            NPC.ai[1] = 60f;
        }
        private void GroundMovement(Vector2 location) {
            if (NPC.position.Y + NPC.height < location.Y) {
                NPC.velocity.Y += 0.65f;

                if (NPC.position.Y + NPC.height < location.Y - 240f && Collision.SolidCollision(NPC.position, NPC.width, NPC.height)) {
                    NPC.velocity.Y *= 0.5f;
                }
            }
            else if ((NPC.position.Y + NPC.height - location.Y).Abs() < 10f) {
                NPC.velocity.Y *= 0.75f;
            }
            else {
                if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height)) {
                    NPC.velocity.Y -= 0.05f;
                }
                else {
                    NPC.velocity.Y -= 0.1f;
                }
                NPC.velocity.Y = Math.Min(NPC.velocity.Y, Math.Max(Main.player[NPC.target].velocity.Y, 2.5f));
            }
            float differenceX = NPC.position.X + NPC.width / 2f - location.X;
            if (differenceX.Abs() > 100f) {
                int sign = Math.Sign(differenceX);
                NPC.velocity.X -= sign * 0.045f;
            }
            else {
                NPC.velocity.X *= 0.8f;
            }
        }
        private void GroundMove(out Point tileCoordinates, out int j) {
            tileCoordinates = Main.player[NPC.target].Center.ToTileCoordinates();
            bool toPlayer = true;
            for (j = 7; j < 36; j++) {
                var tile = Main.tile[tileCoordinates.X, tileCoordinates.Y + j];
                if (tile.HasTile && (tile.SolidType() || tile.SolidTopType())) {
                    GroundMovement(new Vector2(tileCoordinates.X * 16f + 8f, (tileCoordinates.Y + j) * 16f));
                    toPlayer = false;
                    break;
                }
            }
            if (toPlayer)
                GroundMovement(Main.player[NPC.target].Center + new Vector2(0f, 128f + NPC.height));
        }
        private void GroundMove() {
            GroundMove(out var _, out var _);
        }
        private void RandomizePhase(int current) {
            List<int> actions = new List<int>() { ACTION_CLAWSHOTS, PHASE_GROUNDBUBBLES, ACTION_CLAWSLAMS };
            NPC.ai[0] = actions[Main.rand.Next(actions.Count)];
            dealContactDamage = true;
            NPC.localAI[0] = 0f;
            HandRight.localAI[0] = 0f;
            HandLeft.localAI[0] = 0f;
            NPC.damage = NPC.defDamage;
            HandRight.damage = HandRight.defDamage;
            HandLeft.damage = HandLeft.defDamage;
            NPC.defense = NPC.defDefense;
            HandRight.defense = HandRight.defDefense;
            HandLeft.defense = HandLeft.defDefense;
            if ((int)NPC.ai[0] == ACTION_CLAWSHOTS) {
                NPC.ai[1] = Main.expertMode ? 240f : 320f;
                if (PhaseTwo && (Main.expertMode || Main.rand.NextFloat() < 0.75f)) {
                    NPC.ai[0] = ACTION_P2_CLAWSHOTS_SHRAPNEL;
                }
                HandLeft.ai[1] = 0f;
                HandRight.ai[1] = 0f;
                HandLeft.ai[2] = 0f;
                HandRight.ai[2] = 0f;
            }
            else if ((int)NPC.ai[0] == PHASE_GROUNDBUBBLES) {
                if (current == PHASE_GROUNDBUBBLES || current == PHASE2_GROUNDBUBBLES_SPAMMY) {
                    RandomizePhase(current);
                    return;
                }
                if (PhaseTwo && (Main.expertMode || Main.rand.NextFloat() < 0.75f)) {
                    NPC.ai[0] = PHASE2_GROUNDBUBBLES_SPAMMY;
                }
                NPC.ai[1] = Main.expertMode ? 180f : 120f;
                NPC.ai[2] = 0f;
                NPC.defense *= 4;
                HandRight.defense *= 4;
                HandLeft.defense *= 4;
            }
            else if ((int)NPC.ai[0] == ACTION_CLAWSLAMS) {
                float firstSmash = Main.expertMode ? -8f : -60f;
                float secondSmash = Main.expertMode ? -68f : -128f;
                if (PhaseTwo) {
                    secondSmash += 28f * (NPC.life * 2f / NPC.lifeMax);
                }
                NPC.ai[1] = Main.expertMode ? 200f : 400f;
                if (Main.rand.NextBool()) {
                    HandLeft.ai[1] = firstSmash;
                    HandRight.ai[1] = secondSmash;
                }
                else {
                    HandRight.ai[1] = firstSmash;
                    HandLeft.ai[1] = secondSmash;
                }
            }
            NPC.netUpdate = true;
        }
        private Vector2 ClawIdlePosition() {
            return CrabsonNPC.Center + new Vector2(120f * NPC.direction, -48f);
        }
        public int ShootProj<T>(Vector2 position, Vector2 velo, int damage, float ai0 = 0f, float ai1 = 0f, int extraUpdates = 0, int alpha = 0) where T : ModProjectile {
            return ShootProj(position, velo, ModContent.ProjectileType<T>(), damage, ai0, ai1, extraUpdates, alpha);
        }
        public int ShootProj(Vector2 position, Vector2 velo, int type, int damage, float ai0 = 0f, float ai1 = 0f, int extraUpdates = 0, int alpha = 0) {
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                int p = Projectile.NewProjectile(new EntitySource_Parent(NPC), position, velo, type, Main.masterMode ? damage / 3 : Main.expertMode ? damage / 2 : damage, 1f, Main.myPlayer, ai0, ai1);
                if (p == -1)
                    return -1;
                Main.projectile[p].extraUpdates += extraUpdates;
                Main.projectile[p].alpha += alpha;
                return p;
            }
            return -1;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo) {
            if (Main.rand.NextBool(8)) {
                target.AddBuff(ModContent.BuffType<Buffs.Debuffs.PickBreak>(), 480);
            }
        }

        public override void SendExtraAI(BinaryWriter writer) {
            writer.Write(leftClaw);
            writer.Write(rightClaw);
            writer.Write(crabson);
        }

        public override void ReceiveExtraAI(BinaryReader reader) {
            leftClaw = reader.ReadInt32();
            rightClaw = reader.ReadInt32();
            crabson = reader.ReadInt32();
        }

        public override void BossHeadSpriteEffects(ref SpriteEffects spriteEffects) {
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;
        }

        public override void BossHeadSlot(ref int index) {
            if (IsClaw) {
                index = NPCHeadLoader.GetBossHeadSlot(AequusTextures.CrabsonClaw_Head_Boss.Path);
            }
        }

        public void DrawSaggyChain2(SpriteBatch spriteBatch, Texture2D chain, Vector2 currentPosition, Vector2 endPosition, Vector2 screenPos) {
            int height = chain.Height + 8;
            var velo = Vector2.Normalize(endPosition + new Vector2(0f, height * 4f) - currentPosition) * height;
            var position = currentPosition;
            var origin = new Vector2(chain.Width / 2f, chain.Height / 2f);
            for (int i = 0; i < 50; i++) {
                spriteBatch.Draw(chain, position - screenPos, null, NPC.GetNPCColorTintedByBuffs(Lighting.GetColor((int)(position.X / 16), (int)(position.Y / 16f))), 0f, origin, 1f, SpriteEffects.None, 0f);
                velo = Vector2.Normalize(Vector2.Lerp(velo, endPosition - position, 0.01f + MathHelper.Clamp(1f - Vector2.Distance(endPosition, position) / 300f, 0f, 0.99f))) * height;
                position += velo;
                float gravity = MathHelper.Clamp(1f - Vector2.Distance(endPosition, position) / 500f, 0.4f, 1f);
                velo.Y += gravity;
                position.Y += 6f * gravity;
                if (Vector2.Distance(position, endPosition) <= height)
                    break;
            }
        }
        public static void DrawSaggyChain(SpriteBatch spriteBatch, Texture2D chain, Vector2 currentPosition, Vector2 endPosition, Vector2 screenPos) {
            int height = chain.Height + 8;
            var velo = Vector2.Normalize(endPosition + new Vector2(0f, height * 4f) - currentPosition) * height;
            var position = currentPosition;
            var origin = new Vector2(chain.Width / 2f, chain.Height / 2f);
            for (int i = 0; i < 50; i++) {
                spriteBatch.Draw(chain, position - screenPos, null, Lighting.GetColor((int)(position.X / 16), (int)(position.Y / 16f)), 0f, origin, 1f, SpriteEffects.None, 0f);
                velo = Vector2.Normalize(Vector2.Lerp(velo, endPosition - position, 0.01f + MathHelper.Clamp(1f - Vector2.Distance(endPosition, position) / 300f, 0f, 0.99f))) * height;
                position += velo;
                float gravity = MathHelper.Clamp(1f - Vector2.Distance(endPosition, position) / 500f, 0.4f, 1f);
                velo.Y += gravity;
                position.Y += 6f * gravity;
                if (Vector2.Distance(position, endPosition) <= height)
                    break;
            }
        }
        public static void DrawSaggyChainTest(SpriteBatch spriteBatch, Texture2D chain, Vector2 currentPosition, Vector2 endPosition, Vector2 screenPos) {
            int height = chain.Height;
            var velo = Vector2.Normalize(endPosition - currentPosition) * (height - 4f);
            var position = currentPosition;
            var origin = new Vector2(chain.Width / 2f, chain.Height / 2f);
            var primCoords = new List<Vector2>();
            float maxWidth = Math.Abs(currentPosition.X - endPosition.X);
            int dir = Math.Sign(currentPosition.X - endPosition.X);
            for (int i = 0; i < 50; i++) {
                primCoords.Add(position);
                float progress = Math.Abs(primCoords[i].X - endPosition.X) / maxWidth;
                position += velo.RotatedBy((float)Math.Sin(progress * (MathHelper.PiOver2 * 3f) + MathHelper.PiOver2) * dir);
                if (Vector2.Distance(position, endPosition) <= height)
                    break;
            }
            float rotation = 0f;
            for (int i = 0; i < primCoords.Count; i++) {
                if (i < primCoords.Count - 1)
                    rotation = (primCoords[i] - primCoords[i + 1]).ToRotation();
                spriteBatch.Draw(chain, primCoords[i] - screenPos, null, Lighting.GetColor((int)(primCoords[i].X / 16), (int)(primCoords[i].Y / 16f)), rotation, origin, 1f, SpriteEffects.None, 0f);
            }
        }

        public override void DrawBehind(int index) {
            LegacyEffects.NPCsBehindAllNPCs.Add(NPC);
        }

        #region Drawing
        protected void DrawClawManual(SpriteBatch spriteBatch, Texture2D claw, Vector2 drawCoords, Color drawColor, Vector2 origin, float rotation, float mouthAnimation, float scale, SpriteEffects spriteEffects) {
            int frameHeight = claw.Height / 4;
            var clawFrame = new Rectangle(0, frameHeight, claw.Width, frameHeight - 2);
            spriteBatch.Draw(claw, drawCoords, clawFrame, drawColor, -mouthAnimation + 0.1f * NPC.direction + rotation, origin, scale, spriteEffects, 0f);
            spriteBatch.Draw(claw, drawCoords, clawFrame with { Y = 0, }, drawColor, mouthAnimation + 0.1f * NPC.direction + rotation, origin, scale, spriteEffects, 0f);
            spriteBatch.Draw(claw, drawCoords, clawFrame with { Y = frameHeight * (Math.Abs(mouthAnimation) > 0.05f ? 3 : 2), }, drawColor, 0.1f * NPC.direction + rotation, origin, scale, spriteEffects, 0f);
        }

        protected void DrawClaw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, float mouthAnimation) {
            var claw = AequusTextures.CrabsonClaw.Value;
            var origin = new Vector2(claw.Width / 2f + 20f, claw.Height / 8f);
            var drawCoords = npc.Center + new Vector2(npc.direction * 10f, -20f) - screenPos;
            if (NPC.ModNPC != null) {
                drawCoords.Y += NPC.ModNPC.DrawOffsetY;
            }
            SpriteEffects spriteEffects;
            bool flip;
            if (npc.rotation == 0f) {
                spriteEffects = npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                flip = npc.direction == 1;
                if (!flip) {
                    origin.X = claw.Width - origin.X;
                }
            }
            else {
                spriteEffects = Math.Abs(npc.rotation) > MathHelper.PiOver2 ? SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally : SpriteEffects.FlipHorizontally;
                flip = spriteEffects.HasFlag(SpriteEffects.FlipVertically);
            }

            DrawClawManual(spriteBatch, claw, drawCoords, drawColor, origin, npc.rotation, flip ? -mouthAnimation : mouthAnimation, npc.scale, spriteEffects);
        }

        private void DrawClawsTelegraph(NPC npc) {
            if (Collision.SolidCollision(npc.position, npc.width, npc.height)) {
                return;
            }
            float opacity = 1f;
            if (npc.ai[1] < -25f) {
                opacity -= (float)Math.Pow((npc.ai[1] + 25f) / 25f, 2f);
            }
            if (npc.ai[1] > 50f) {
                opacity -= (float)Math.Pow((npc.ai[1] - 50f) / 25f, 2f);
            }
            if (opacity <= 0f)
                return;
            var tileCoords = npc.BottomLeft.ToTileCoordinates();
            int min = 0;
            int max = npc.width / 16;
            if (npc.direction == -1) {
                max += 2;
            }
            else {
                min -= 2;
            }
            for (int i = min; i <= max; i++) {
                for (int j = 0; j < 80; j++) {
                    if (WorldGen.InWorld(tileCoords.X + i, tileCoords.Y + j, 25) && Main.tile[tileCoords.X + i, tileCoords.Y + j].IsFullySolid()) {
                        var coords = new Vector2((tileCoords.X + i) * 16f, (tileCoords.Y + j) * 16f + 16f) - Main.screenPosition;
                        var frame = new Rectangle(AequusTextures.Bloom0.Width / 2, 0, 1, AequusTextures.Bloom0.Height / 2);
                        var origin = new Vector2(0f, frame.Height);
                        Main.spriteBatch.Draw(AequusTextures.Bloom0, coords,
                            frame, Color.Blue.UseA(0) * 0.6f * opacity, 0f,
                            origin, new Vector2(16f, 1f), SpriteEffects.None, 0f);
                        if (i == min) {
                            Main.spriteBatch.Draw(AequusTextures.Bloom0, coords,
                                frame, Color.DeepSkyBlue.UseA(0) * 0.6f * opacity, 0f,
                                origin, new Vector2(2f, 2f), SpriteEffects.None, 0f);
                        }
                        if (i == max) {
                            Main.spriteBatch.Draw(AequusTextures.Bloom0, coords + new Vector2(14f, 0f),
                                frame, Color.DeepSkyBlue.UseA(0) * 0.6f * opacity, 0f,
                                origin, new Vector2(2f, 2f), SpriteEffects.None, 0f);
                        }
                        break;
                    }
                }
            }
        }

        private void DrawBody(SpriteBatch spriteBatch, Vector2 screenPos, Vector2 offset, Color bodyDrawColor) {
            offset.Y -= 24;
            var drawPosition = NPC.Center - screenPos + offset;
            spriteBatch.Draw(
                TextureAssets.Npc[NPC.type].Value,
                drawPosition,
                NPC.frame,
                bodyDrawColor,
                NPC.rotation,
                NPC.frame.Size() / 2f,
                NPC.scale, SpriteEffects.None, 0f);

            var legFrame = AequusTextures.Crabson_Legs.Frame(verticalFrames: WalkManager.MaxFrames, frameY: walkManager.frame);
            spriteBatch.Draw(
                AequusTextures.Crabson_Legs,
                drawPosition,
                legFrame,
                bodyDrawColor,
                NPC.rotation,
                legFrame.Size() / 2f,
                NPC.scale, NPC.velocity.X < 0f ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            var eyeOffset = new Vector2(0f, -40f);
            var eyePosition = drawPosition + eyeOffset;
            var eyeFrame = AequusTextures.Crabson_Eyes.Frame(verticalFrames: 8, frameY: eyeManager.GetFrame());
            var eyeOrigin = eyeFrame.Size() / 2f;
            spriteBatch.Draw(
                AequusTextures.Crabson_Eyes,
                eyePosition,
                eyeFrame,
                Color.White,
                NPC.rotation,
                eyeOrigin,
                NPC.scale, SpriteEffects.None, 0f);

            var pupilFrame = AequusTextures.Crabson_Pupil.Frame(verticalFrames: 2, frameY: eyeManager.GetPupilFrame());
            var pupilOrigin = pupilFrame.Size() / 2f;
            spriteBatch.Draw(
                AequusTextures.Crabson_Pupil,
                eyePosition + eyeManager.pupil,
                pupilFrame,
                Color.White,
                NPC.rotation,
                pupilOrigin,
                NPC.scale, SpriteEffects.None, 0f);

            var trailOffset = NPC.Size / 2f;
            int trailLength = NPCID.Sets.TrailCacheLength[Type];
            var trailColor = Color.White with { A = 0 } * Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.25f, 0.4f);
            for (int i = 0; i < trailLength; i++) {

                float progress = Helper.CalcProgress(trailLength, i);
                var trailClr = trailColor * MathF.Pow(progress, 2f);
                var eyeTrailPosition = NPC.oldPos[i] + trailOffset + eyeOffset - screenPos + offset;
                spriteBatch.Draw(
                    AequusTextures.Crabson_Eyes,
                    eyeTrailPosition,
                    eyeFrame,
                    trailClr,
                    NPC.rotation,
                    eyeOrigin,
                    NPC.scale, SpriteEffects.None, 0f);

                spriteBatch.Draw(
                    AequusTextures.Crabson_Pupil,
                    eyeTrailPosition + eyeManager.pupil,
                    pupilFrame,
                    trailClr,
                    NPC.rotation,
                    pupilOrigin,
                    NPC.scale, SpriteEffects.None, 0f);
            }
        }

        private void DrawBestiary(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            Vector2 offset = new Vector2(120f, 58f) * NPC.scale;
            NPC.direction = 1;
            NPC.spriteDirection = 1;
            DrawClaw(NPC, spriteBatch, screenPos + (offset with { X = -offset.X }), Color.White, 0f);
            NPC.direction = -1;
            NPC.spriteDirection = -1;
            DrawClaw(NPC, spriteBatch, screenPos + offset, Color.White, 0f);

            DrawBody(
                spriteBatch,
                screenPos,
                new(0f, 0f),
                Color.White
            );
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            if (NPC.IsABestiaryIconDummy) {
                DrawBestiary(spriteBatch, screenPos, drawColor);
                return false;
            }

            if (IsClaw) {
                DrawClaw(NPC, spriteBatch, screenPos, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), mouthAnimation);
                return false;
            }

            if ((int)NPC.ai[0] == ACTION_CLAWSLAMS) {
                DrawClawsTelegraph(HandLeft);
                DrawClawsTelegraph(HandRight);
            }

            arms.DrawArms(NPC, spriteBatch, screenPos);
            DrawBody(spriteBatch, screenPos, new(), NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)));
            return false;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) {
            scale = 1.5f;
            return IsClaw ? false : null;
        }
        #endregion

        public override void ModifyNPCLoot(NPCLoot npcLoot) {
            int bossBag = ModContent.ItemType<CrabsonBag>();
            npcLoot.Add<FlawlessCondition>(ItemDropRule.Common(ModContent.ItemType<CrabsonTrophy>())).OnFailedConditions(ItemDropRule.Common(ModContent.ItemType<CrabsonTrophy>(), LootBuilder.DroprateTrophy));
            npcLoot.Add(ItemDropRule.BossBag(bossBag));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<CrabsonRelic>()));
            //npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<CrabsonPetItem>(), LootBuilder.MasterPetDroprate));
            npcLoot.AddExpertDrop<MoneyTrashcan>(bossBag);
            npcLoot.Add(LootBuilder.GetDropRule_PerPlayerInstanced<AquaticEnergy>(min: 3, max: 3));
            npcLoot.AddBossLoot(bossBag, ItemDropRule.Common(ModContent.ItemType<CrabsonMask>(), chanceDenominator: LootBuilder.DroprateMask));
            npcLoot.AddBossLoot(bossBag, ItemDropRule.OneFromOptions(1, ModContent.ItemType<JunkJet>()));
        }

        private void CheckClosestSegmentForLoot(byte player, ref float distance, NPC npc) {
            float d = npc.Distance(Main.player[player].Center);
            if (Collision.SolidCollision(npc.position, npc.width, npc.height)) {
                d *= 2f;
            }
            if (Collision.CanHitLine(Main.player[player].position, Main.player[player].width, Main.player[player].height, npc.position, npc.width, npc.height)) {
                d /= 20f;
            }
            if (d < distance) {
                distance = d;
                NPC.Center = npc.Center;
            }
        }
        public override bool PreKill() {
            byte player = Player.FindClosest(NPC.position, NPC.width, NPC.height);
            float distance = NPC.Distance(Main.player[player].Center);
            CheckClosestSegmentForLoot(player, ref distance, HandLeft);
            CheckClosestSegmentForLoot(player, ref distance, HandRight);
            return true;
        }

        public override void OnKill() {
            Rectangle rect = NPC.getRect();
            if (!AequusWorld.downedCrabson) {
                if (!NPC.AnyNPCs(ModContent.NPCType<Exporter>())) {
                    NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.position.X + NPC.width / 2, (int)NPC.position.Y + NPC.height / 2, ModContent.NPCType<Exporter>());
                }
                AequusWorld.MarkAsDefeated(ref AequusWorld.downedCrabson, NPC.type);
            }

            Projectile.NewProjectile(
                NPC.GetSource_Loot(),
                NPC.Center,
                new Vector2(Main.rand.NextFloat(-4f, 4f), -6f),
                ModContent.ProjectileType<CrabsonTreasureChest>(),
                0, 0f, Main.myPlayer);
        }
    }
}