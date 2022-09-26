using Aequus.Biomes;
using Aequus.Graphics;
using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Healing;
using Aequus.Items.Armor.Vanity;
using Aequus.Items.Boss.Bags;
using Aequus.Items.Boss.Summons;
using Aequus.Items.Misc.Energies;
using Aequus.Items.Placeable.Furniture.BossTrophies;
using Aequus.NPCs.Friendly.Town;
using Aequus.Projectiles.Monster.CrabsonProjs;
using Aequus.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Boss
{
    [AutoloadBossHead]
    public class Crabson : AequusBoss
    {
        public const int ACTION_CLAWSHOTS = 2;
        public const int PHASE_GROUNDBUBBLES = 3;
        public const int ACTION_CLAWSLAMS = 4;
        public const int PHASE2_GROUNDBUBBLES_SPAMMY = 5;
        public const int ACTION_P2_CLAWSHOTS_SHRAPNEL = 6;

        public static int BossHeadID_Claw { get; private set; }
        public Asset<Texture2D> ClawTexture => ModContent.Request<Texture2D>(Texture + "Claw");
        public Asset<Texture2D> ClawChainTexture => ModContent.Request<Texture2D>(Texture + "Claw_Chain");

        public int leftClaw;
        public int rightClaw;
        public int crabson;
        public bool dealContactDamage;

        public int MainAction => (int)Main.npc[crabson].ai[0];
        public NPC Left => Main.npc[leftClaw];
        public NPC Right => Main.npc[rightClaw];
        public NPC CrabsonNPC => Main.npc[crabson];
        public bool IsClaw => NPC.whoAmI != crabson;
        public bool PhaseTwo => Main.npc[NPC.realLife].life * (Main.expertMode ? 2f : 4f) <= NPC.lifeMax;
        public static ConfiguredMusicData music { get; private set; }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                BossHeadID_Claw = Mod.AddBossHeadTexture(this.GetPath() + "Claw_Head_Boss", -1);
                music = new ConfiguredMusicData(MusicID.Boss3);
            }
        }

        public override void Unload()
        {
            music = null;
        }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData()
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Wet,
                    BuffID.Confused,
                    BuffID.Suffocation,
                    BuffID.Lovestruck,
                }
            });

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers(0) { PortraitPositionYOverride = 48f, });

            SnowgraveCorpse.NPCBlacklist.Add(Type);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry);
        }

        public override void SetDefaults()
        {
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

            if (!Main.dedServ && music != null)
            {
                Music = music.GetID();
                SceneEffectPriority = SceneEffectPriority.BossLow;
            }

            this.SetBiome<CrabCreviceBiome>();

            crabson = -1;
            leftClaw = -1;
            rightClaw = -1;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
                    }
                }
            }
            else
            {
                for (int i = 0; i < Math.Min(damage / 20 + 1, 1); i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return dealContactDamage;
        }

        public override void AI()
        {
            SpawnManager.ForceZen(NPC);
            if (NPC.alpha > 0)
            {
                NPC.alpha -= 5;
                if (NPC.alpha < 0)
                {
                    NPC.alpha = 0;
                }
            }
            int loops = Main.getGoodWorld ? 2 : 1;
            for (int k = 0; k < loops; k++)
            {
                if (k >= 1)
                {
                    NPC.position += NPC.velocity;
                }
                Vector2 center = NPC.Center;
                if (Action == ACTION_INIT && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    BodyInit();
                    return;
                }
                if (MainAction == ACTION_GOODBYE)
                {
                    BodyGoodbye();
                    return;
                }
                if (CheckClaws())
                {
                    NPC.realLife = crabson;
                    if (!IsClaw)
                    {
                        dealContactDamage = false;
                        NPC.behindTiles = true;
                        if (!NPC.HasValidTarget || NPC.Distance(Main.player[NPC.target].Center) > 4000f)
                        {
                            NPC.TargetClosest(faceTarget: false);
                            if (!NPC.HasValidTarget || NPC.Distance(Main.player[NPC.target].Center) > 4000f)
                            {
                                SetGoodbyeState();
                                return;
                            }
                        }

                        switch (Action)
                        {
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
                    else
                    {
                        dealContactDamage = false;
                        NPC.behindTiles = false;
                        NPC.noGravity = true;
                        NPC.noTileCollide = true;

                        switch (MainAction)
                        {
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

                        if (crabson == -1 || !Main.npc[crabson].active || Main.npc[crabson].type != NPC.type)
                        {
                            Kill();
                        }
                        else
                        {
                            NPC.target = CrabsonNPC.target;
                        }
                    }
                }
                else
                {
                    if (!IsClaw)
                    {
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (i == NPC.whoAmI || i == leftClaw || i == rightClaw)
                            {
                                continue;
                            }
                            if (Main.npc[i].active && Main.npc[i].type == NPC.type)
                            {
                                var crab = (Crabson)NPC.ModNPC;
                                crab.crabson = NPC.whoAmI;
                                if (leftClaw == -1)
                                {
                                    crab.leftClaw = i;
                                    crab.rightClaw = rightClaw;
                                }
                                else if (rightClaw == -1)
                                {
                                    crab.rightClaw = i;
                                    crab.leftClaw = leftClaw;
                                    return;
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }
                        if (!CheckClaws())
                        {
                            Kill();
                        }
                    }
                    else
                    {
                        if (crabson == -1 || !Main.npc[crabson].active || Main.npc[crabson].type != NPC.type)
                        {
                            Kill();
                        }
                    }
                }
            }
        }
        public void ClawSlams()
        {
            dealContactDamage = true;
            NPC.ai[1]++;
            int time = 10;
            if (NPC.ai[1] + time < 0f)
            {
                NPC.noGravity = false;
                NPC.noTileCollide = false;
            }
            else
            {
                int smashTime = Main.expertMode ? 45 : 75;
                if (NPC.ai[1] > smashTime)
                {
                    NPC.damage = NPC.defDamage * 2;
                    NPC.velocity.X = 0f;
                    NPC.velocity.Y = Main.expertMode ? 32f : 16f;
                    int fallingTime = Main.expertMode ? 10 : 20;
                    if (NPC.ai[1] > smashTime + fallingTime)
                    {
                        NPC.noGravity = true;
                        NPC.noTileCollide = false;
                        if (Main.netMode != NetmodeID.Server && (int)NPC.localAI[0] == 0)
                        {
                            if (Main.netMode != NetmodeID.Server)
                            {
                                SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
                            }
                            //AQMod.Effects.SetShake(2f, 6f);
                        }
                        NPC.localAI[0] = 1f;
                    }
                }
                else
                {
                    NPC.damage = 0;
                    var gotoPos = Main.player[NPC.target].Center;
                    if (NPC.position.Y < Main.player[NPC.target].position.Y - 80)
                    {
                        gotoPos += new Vector2(0f, -320f - NPC.height);
                    }
                    else
                    {
                        gotoPos += new Vector2(200f * NPC.direction, -120f - NPC.height);
                    }
                    NPC.Center = Vector2.Lerp(NPC.Center, gotoPos, 0.05f + 0.75f * (NPC.ai[0] / smashTime));
                }
            }
        }
        public void ClawShots(bool shrapnel)
        {
            var gotoPosition = Main.player[NPC.target].Center + new Vector2(400f * NPC.direction, 0f);

            var difference = gotoPosition - NPC.Center;
            float l = difference.Length();
            if (CrabsonNPC.ai[1] > 30f)
            {
                NPC.ai[1]++;
                int shootTime = Main.expertMode ? 80 : 160;
                if (NPC.whoAmI == leftClaw)
                {
                    if (Right.ai[1] >= NPC.ai[1] - 5f)
                    {
                        NPC.ai[1] = Right.ai[1] - shootTime / 2f;
                    }
                }
                if (NPC.ai[1] > shootTime / 2f)
                {
                    NPC.rotation = MathHelper.Lerp(NPC.rotation, 0.5f, 0.4f);
                }
                if (NPC.ai[1] > shootTime)
                {
                    NPC.ai[1] = 1f;
                    NPC.ai[2] = 0.5f;
                    NPC.position.X += 40f * NPC.direction;
                    NPC.velocity = new Vector2(6f * NPC.direction, 0f);

                    if (Main.netMode != NetmodeID.Server)
                    {
                        SoundEngine.PlaySound(SoundID.Item61, NPC.Center);
                    }

                    ShootProj<CrabsonPearl>(NPC.Center, new Vector2(20f * -NPC.direction, 0f), NPC.damage, ai1: shrapnel ? 1f : 0f);
                }
            }
            if (CrabsonNPC.ai[1] < 30f)
            {
                NPC.rotation *= 0.6f;
                if (NPC.rotation < 0.05f)
                {
                    NPC.rotation = 0f;
                }
            }
            if (l < 0.01f)
            {
                NPC.Center = gotoPosition;
                NPC.velocity = Vector2.Zero;
                NPC.ai[2] = 0f;
            }
            else
            {
                if (NPC.ai[2] > 0f)
                {
                    NPC.ai[2] -= 0.05f;
                    if (NPC.ai[2] < 0f)
                    {
                        NPC.ai[2] = 0f;
                    }
                }
                NPC.velocity = Vector2.Lerp(NPC.velocity, difference / 20f,
                    Math.Max(0.3f - NPC.ai[2], 0f));
            }
        }

        public void BodyGoodbye()
        {
            if (NPC.timeLeft > 20)
            {
                NPC.timeLeft = 20;
            }
            NPC.velocity.X *= 0.9f;
            if (NPC.velocity.Y < 0f)
            {
                NPC.velocity.Y *= 0.8f;
            }
            Right.rotation *= 0.8f;
            Left.rotation *= 0.8f;
            NPC.velocity.Y += 0.1f;
        }
        public void BodyGroundBubblesAttack()
        {
            NPC.ai[1]--;
            var tileCoordinates = Main.player[NPC.target].Center.ToTileCoordinates();
            int randomness = Main.expertMode ? 4 : 3;
            tileCoordinates.X += Main.rand.Next(-randomness, randomness);
            bool toPlayer = true;
            int bubbleTile = 40;
            int start = Main.rand.Next(5, 12);
            for (int j = start; j < 40; j++)
            {
                var tile = Main.tile[tileCoordinates.X, tileCoordinates.Y + j];
                if (tile.HasTile)
                {
                    if (bubbleTile == 40 && tile.SolidType())
                    {
                        bubbleTile = j;
                    }
                    if (j > 20 && (tile.SolidType() || tile.SolidTopType()))
                    {
                        GroundMovement(new Vector2(tileCoordinates.X * 16f + 8f, (tileCoordinates.Y + j) * 16f + NPC.height + 20f));
                        toPlayer = false;
                        break;
                    }
                }
            }
            if (toPlayer)
            {
                GroundMovement(Main.player[NPC.target].Center + new Vector2(0f, 320f + NPC.height));
            }
            if (NPC.ai[1] <= 0f)
            {
                RandomizePhase((int)NPC.ai[0]);
            }
            NPC.ai[2]++;
            float time = Main.expertMode ? 40f : 80f;
            if ((int)NPC.ai[0] == PHASE2_GROUNDBUBBLES_SPAMMY)
            {
                time -= 36f * (1f - NPC.life / NPC.lifeMax);
            }
            if (NPC.ai[2] > time)
            {
                NPC.ai[2] = 0f;
                float shootTime = Main.expertMode ? 30f - (PhaseTwo ? 8f : 0f) : 40f;
                if ((int)NPC.ai[0] == PHASE2_GROUNDBUBBLES_SPAMMY && Main.rand.NextBool())
                {
                    var spawnPos = new Vector2((tileCoordinates.X + Main.rand.Next(-3, 3)) * 16f + 8f, (tileCoordinates.Y - Main.rand.Next(12, 20)) * 16f - 4f);
                    ShootProj<CrabsonBubble>(spawnPos, Vector2.Normalize(Main.player[NPC.target].Center - spawnPos) * 0.01f, NPC.damage, ai0: shootTime / 2f, alpha: 1);
                }
                else
                {
                    if (bubbleTile != 40)
                    {
                        ShootProj<CrabsonBubble>(new Vector2(tileCoordinates.X * 16f + 8f, (tileCoordinates.Y + bubbleTile) * 16f - 4f), new Vector2(0f, -0.01f), NPC.damage, ai0: shootTime);
                    }
                    else
                    {
                        if (Main.netMode != NetmodeID.Server)
                        {
                            SoundEngine.PlaySound(SoundID.Item85.WithVolume(0.7f), NPC.Center);
                        }
                        ShootProj<CrabsonBubble>(new Vector2(NPC.position.X + NPC.width / 2f, NPC.position.Y - 4f), new Vector2(0f, -0.01f), NPC.damage, ai0: 1f);
                    }
                }
            }
        }
        public void BodyMovement()
        {
            NPC.ai[1]--;
            GroundMove();
            if (NPC.ai[1] <= 0f)
            {
                RandomizePhase((int)NPC.ai[0]);
            }
        }
        public void BodyIntro()
        {
            GroundMove(out var tileCoords, out var j);
            NPC.ai[1]++;
            if ((NPC.position.Y + NPC.height - (tileCoords.Y + j) * 16f).Abs() < 20f || NPC.ai[1] > 60f)
            {
                NPC.ai[0] = ACTION_CLAWSHOTS;
                NPC.ai[1] = Main.expertMode ? 240f : 320f;
                Left.ai[1] = 0f;
                Right.ai[1] = 0f;
                Left.ai[2] = 0f;
                Right.ai[2] = 0f;
            }
        }
        public void BodyInit()
        {
            NPC.ai[0] = ACTION_INTRO;
            NPC.netUpdate = true;
            NPC.TargetClosest(faceTarget: false);
            leftClaw = -1;
            rightClaw = -1;
            crabson = NPC.whoAmI;
            NPC.realLife = NPC.whoAmI;
            for (int i = -1; i <= 1; i++)
            {
                if (i != 0)
                {
                    int n = NPC.NewNPC(new EntitySource_Parent(NPC), (int)NPC.position.X + NPC.width / 2, (int)NPC.position.Y + NPC.height / 2, NPC.type);
                    if (i == -1)
                    {
                        leftClaw = n;
                    }
                    else
                    {
                        rightClaw = n;
                    }
                    Main.npc[n].position.X += 150f * i;
                    Main.npc[n].ai[0] = ACTION_INTRO;
                    Main.npc[n].direction = i;
                    Main.npc[n].spriteDirection = i;
                    Main.npc[n].realLife = NPC.whoAmI;
                    Main.npc[n].defense *= 4;
                    Main.npc[n].defDefense = Main.npc[n].defense;
                    Main.npc[n].width += 20;
                    Main.npc[n].height += 50;
                    if (Main.getGoodWorld)
                    {
                        float scale = 2f;
                        Main.npc[n].scale *= scale;
                        Main.npc[n].width = (int)(Main.npc[n].width * scale);
                        Main.npc[n].height = (int)(Main.npc[n].height * scale);
                    }
                }
            }
            var crab = (Crabson)Left.ModNPC;
            crab.rightClaw = rightClaw;
            crab.leftClaw = leftClaw;
            crab.crabson = crabson;

            crab = (Crabson)Right.ModNPC;
            crab.rightClaw = rightClaw;
            crab.leftClaw = leftClaw;
            crab.crabson = crabson;
        }

        private bool CheckClaws()
        {
            return !(leftClaw == -1 || !Left.active || Left.type != NPC.type ||
                rightClaw == -1 || !Right.active || Right.type != NPC.type);
        }
        private void Kill()
        {
            NPC.life = -1;
            NPC.HitEffect();
            NPC.active = false;
        }
        private void SetGoodbyeState()
        {
            NPC.ai[0] = ACTION_GOODBYE;
            NPC.ai[1] = 60f;
        }
        private void GroundMovement(Vector2 location)
        {
            if (NPC.position.Y + NPC.height < location.Y)
            {
                NPC.velocity.Y += 0.65f;

                if (NPC.position.Y + NPC.height < location.Y - 240f && Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                {
                    NPC.velocity.Y *= 0.5f;
                }
            }
            else if ((NPC.position.Y + NPC.height - location.Y).Abs() < 10f)
            {
                NPC.velocity.Y *= 0.75f;
            }
            else
            {
                if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                {
                    NPC.velocity.Y -= 0.05f;
                }
                else
                {
                    NPC.velocity.Y -= 0.1f;
                }
                NPC.velocity.Y = Math.Min(NPC.velocity.Y, Math.Max(Main.player[NPC.target].velocity.Y, 2.5f));
            }
            float differenceX = NPC.position.X + NPC.width / 2f - location.X;
            if (differenceX.Abs() > 100f)
            {
                int sign = Math.Sign(differenceX);
                NPC.velocity.X -= sign * 0.045f;
            }
            else
            {
                NPC.velocity.X *= 0.8f;
            }
        }
        private void GroundMove(out Point tileCoordinates, out int j)
        {
            tileCoordinates = Main.player[NPC.target].Center.ToTileCoordinates();
            bool toPlayer = true;
            for (j = 7; j < 36; j++)
            {
                var tile = Main.tile[tileCoordinates.X, tileCoordinates.Y + j];
                if (tile.HasTile && (tile.SolidType() || tile.SolidTopType()))
                {
                    GroundMovement(new Vector2(tileCoordinates.X * 16f + 8f, (tileCoordinates.Y + j) * 16f));
                    toPlayer = false;
                    break;
                }
            }
            if (toPlayer)
                GroundMovement(Main.player[NPC.target].Center + new Vector2(0f, 128f + NPC.height));
        }
        private void GroundMove()
        {
            GroundMove(out var _, out var _);
        }
        private void RandomizePhase(int current)
        {
            List<int> actions = new List<int>() { ACTION_CLAWSHOTS, PHASE_GROUNDBUBBLES, ACTION_CLAWSLAMS };
            NPC.ai[0] = actions[Main.rand.Next(actions.Count)];
            dealContactDamage = true;
            NPC.localAI[0] = 0f;
            Right.localAI[0] = 0f;
            Left.localAI[0] = 0f;
            NPC.damage = NPC.defDamage;
            Right.damage = Right.defDamage;
            Left.damage = Left.defDamage;
            NPC.defense = NPC.defDefense;
            Right.defense = Right.defDefense;
            Left.defense = Left.defDefense;
            if ((int)NPC.ai[0] == ACTION_CLAWSHOTS)
            {
                NPC.ai[1] = Main.expertMode ? 240f : 320f;
                if (PhaseTwo && (Main.expertMode || Main.rand.NextFloat() < 0.75f))
                {
                    NPC.ai[0] = ACTION_P2_CLAWSHOTS_SHRAPNEL;
                }
                Left.ai[1] = 0f;
                Right.ai[1] = 0f;
                Left.ai[2] = 0f;
                Right.ai[2] = 0f;
            }
            else if ((int)NPC.ai[0] == PHASE_GROUNDBUBBLES)
            {
                if (current == PHASE_GROUNDBUBBLES || current == PHASE2_GROUNDBUBBLES_SPAMMY)
                {
                    RandomizePhase(current);
                    return;
                }
                if (PhaseTwo && (Main.expertMode || Main.rand.NextFloat() < 0.75f))
                {
                    NPC.ai[0] = PHASE2_GROUNDBUBBLES_SPAMMY;
                }
                NPC.ai[1] = Main.expertMode ? 180f : 120f;
                NPC.ai[2] = 0f;
                NPC.defense *= 4;
                Right.defense *= 4;
                Left.defense *= 4;
            }
            else if ((int)NPC.ai[0] == ACTION_CLAWSLAMS)
            {
                float firstSmash = Main.expertMode ? -8f : -60f;
                float secondSmash = Main.expertMode ? -68f : -128f;
                if (PhaseTwo)
                {
                    secondSmash += 28f * (NPC.life * 2f / NPC.lifeMax);
                }
                NPC.ai[1] = Main.expertMode ? 200f : 400f;
                if (Main.rand.NextBool())
                {
                    Left.ai[1] = firstSmash;
                    Right.ai[1] = secondSmash;
                }
                else
                {
                    Right.ai[1] = firstSmash;
                    Left.ai[1] = secondSmash;
                }
            }
            NPC.netUpdate = true;
        }
        private Vector2 ClawIdlePosition()
        {
            return CrabsonNPC.Center + new Vector2(120f * NPC.direction, -48f);
        }
        public int ShootProj<T>(Vector2 position, Vector2 velo, int damage, float ai0 = 0f, float ai1 = 0f, int extraUpdates = 0, int alpha = 0) where T : ModProjectile
        {
            return ShootProj(position, velo, ModContent.ProjectileType<T>(), damage, ai0, ai1, extraUpdates, alpha);
        }
        public int ShootProj(Vector2 position, Vector2 velo, int type, int damage, float ai0 = 0f, float ai1 = 0f, int extraUpdates = 0, int alpha = 0)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int p = Projectile.NewProjectile(new EntitySource_Parent(NPC), position, velo, type, Main.masterMode ? damage / 3 : Main.expertMode ? damage / 2 : damage, 1f, Main.myPlayer, ai0, ai1);
                if (p == -1)
                    return -1;
                Main.projectile[p].extraUpdates += extraUpdates;
                Main.projectile[p].alpha += alpha;
                return p;
            }
            return -1;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool(8))
            {
                target.AddBuff(ModContent.BuffType<Buffs.Debuffs.PickBreak>(), 480);
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(leftClaw);
            writer.Write(rightClaw);
            writer.Write(crabson);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            leftClaw = reader.ReadInt32();
            rightClaw = reader.ReadInt32();
            crabson = reader.ReadInt32();
        }

        public override void BossHeadSpriteEffects(ref SpriteEffects spriteEffects)
        {
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;
        }

        public override void BossHeadSlot(ref int index)
        {
            if (IsClaw && BossHeadID_Claw != -1)
            {
                index = BossHeadID_Claw;
            }
        }

        public static void DrawSaggyChain(SpriteBatch spriteBatch, Texture2D chain, Vector2 currentPosition, Vector2 endPosition, Vector2 screenPos)
        {
            int height = chain.Height + 8;
            var velo = Vector2.Normalize(endPosition + new Vector2(0f, height * 4f) - currentPosition) * height;
            var position = currentPosition;
            var origin = new Vector2(chain.Width / 2f, chain.Height / 2f);
            for (int i = 0; i < 50; i++)
            {
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

        public override void DrawBehind(int index)
        {
            AequusEffects.NPCsBehindAllNPCs.Add(NPC.whoAmI);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                var claw = ClawTexture.Value;
                var origin = new Vector2(16f, 48f);
                RenderClaw(spriteBatch, claw, NPC.Center + new Vector2(-NPC.width * 1.33f, -NPC.height * 1f) - screenPos, Color.White, origin, 0f, NPC.scale, SpriteEffects.None);
                origin.X = claw.Width - origin.X;
                RenderClaw(spriteBatch, claw, NPC.Center + new Vector2(NPC.width * 1.33f, -NPC.height * 1f) - screenPos, Color.White, origin, 0f, NPC.scale, SpriteEffects.FlipHorizontally);
                return true;
            }
            if (IsClaw)
            {
                RenderClaw(NPC, spriteBatch, screenPos, drawColor);
                return false;
            }
            if (AequusEffects.NPCsBehindAllNPCs.RenderingNow)
            {
                var drawCoordinates = NPC.Center;
                var chain = ClawChainTexture.Value;
                DrawSaggyChain(spriteBatch, chain, new Vector2(drawCoordinates.X - 24f, drawCoordinates.Y), Left.position + new Vector2(0f, Left.height / 2f - 24f), screenPos);
                DrawSaggyChain(spriteBatch, chain, new Vector2(drawCoordinates.X + 24f, drawCoordinates.Y), Right.Center + new Vector2(Right.width / 2f, -24f), screenPos);
            }
            return true;
        }
        private void RenderClaw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var claw = ClawTexture.Value;
            var origin = new Vector2(16f, 48f);
            var drawCoords = npc.position + new Vector2(origin.X + (npc.direction == 1 ? npc.width - origin.X * 2f : 0), npc.width / 2f) - screenPos;
            origin.X = npc.direction == 1 ? claw.Width - origin.X : origin.X;
            float rotation = npc.rotation / 2f;
            rotation = npc.direction == -1 ? -rotation : rotation;
            var spriteEffects = npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            RenderClaw(spriteBatch, claw, drawCoords, drawColor, origin, rotation, npc.scale, spriteEffects);
        }
        private void RenderClaw(SpriteBatch spriteBatch, Texture2D claw, Vector2 drawCoords, Color drawColor, Vector2 origin, float rotation, float scale, SpriteEffects spriteEffects)
        {
            int frameHeight = claw.Height / 2;
            var clawFrame = new Rectangle(0, frameHeight, claw.Width, frameHeight - 2);
            spriteBatch.Draw(claw, drawCoords, clawFrame, drawColor, -rotation, origin, scale, spriteEffects, 0f);
            clawFrame.Y = 0;
            spriteBatch.Draw(claw, drawCoords, clawFrame, drawColor, rotation, origin, scale, spriteEffects, 0f);
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            return IsClaw ? false : null;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            this.CreateLoot(npcLoot)
                .AddBossLoot<CrabsonTrophy, CrabsonRelic, CrabsonBag>()

                .SetCondition(new Conditions.NotExpert())
                .Add<CrabsonMask>(chance: 7, stack: 1)
                .Add<AquaticEnergy>(stack: 3)
                .AddOptions(1, ModContent.ItemType<Mendshroom>(), ModContent.ItemType<AmmoBackpack>())
                .RegisterCondition();
        }

        private void CheckClosestSegmentForLoot(byte player, ref float distance, NPC npc)
        {
            float d = npc.Distance(Main.player[player].Center);
            if (Collision.SolidCollision(npc.position, npc.width, npc.height))
            {
                d *= 2f;
            }
            if (d < distance)
            {
                distance = d;
                NPC.Center = npc.Center;
            }
        }
        public override bool PreKill()
        {
            byte player = Player.FindClosest(NPC.position, NPC.width, NPC.height);
            float distance = NPC.Distance(Main.player[player].Center);
            CheckClosestSegmentForLoot(player, ref distance, Left);
            CheckClosestSegmentForLoot(player, ref distance, Right);
            return true;
        }

        public override void OnKill()
        {
            Rectangle rect = NPC.getRect();
            if (!AequusWorld.downedCrabson)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<Exporter>()))
                {
                    NPC.NewNPC(new EntitySource_Parent(NPC), (int)NPC.position.X + NPC.width / 2, (int)NPC.position.Y + NPC.height / 2, ModContent.NPCType<Exporter>());
                }
            }
            AequusWorld.MarkAsDefeated(ref AequusWorld.downedCrabson, NPC.type);
        }
    }
}