using AQMod.Common;
using AQMod.Common.Graphics;
using AQMod.Effects;
using AQMod.Items.Armor.Vanity.BossMasks;
using AQMod.Items.Dyes;
using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Misc;
using AQMod.Items.Placeable.Furniture;
using AQMod.Items.Weapons.Magic;
using AQMod.Items.Weapons.Melee;
using AQMod.Items.Weapons.Ranged;
using AQMod.NPCs.Friendly;
using AQMod.Projectiles.Monster;
using AQMod.Sounds;
using AQMod.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Bosses
{
    [AutoloadBossHead]
    public class Crabson : AQBoss
    {
        public const int Phase_Goodbye = -1;
        public const int Phase_Initalize = 0;
        public const int Phase_Intro = 1;
        public const int Phase_ClawShots = 2;
        public const int Phase_GroundBubbles = 3;
        public const int Phase_ClawSlams = 4;
        public const int Phase_HomingBubbles = 5;
        public const int Phase_ClawShotsShrapnal = 6;

        public int leftClaw;
        public int rightClaw;
        public int crabson;
        public bool contactDamage;

        public NPC Left => Main.npc[leftClaw];
        public NPC Right => Main.npc[rightClaw];
        public NPC CrabsonNPC => Main.npc[crabson];
        public bool IsClaw => npc.whoAmI != crabson;
        public bool PhaseTwo => (Main.npc[npc.realLife].life * (Main.expertMode ? 2f : 4f)) <= npc.lifeMax;

        public override bool Autoload(ref string name)
        {
            if (base.Autoload(ref name))
            {
                mod.AddBossHeadTexture(this.GetPath("Claw_Head_Boss"));
                return true;
            }
            return false;
        }

        public override void SetDefaults()
        {
            npc.width = 90;
            npc.height = 60;
            npc.lifeMax = 2500;
            npc.damage = 20;
            npc.defense = 6;
            npc.aiStyle = -1;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.knockBackResist = 0f;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.boss = true;
            npc.behindTiles = true;
            bossBag = ModContent.ItemType<CrabsonBag>();
            npc.buffImmune[BuffID.Suffocation] = true;
            if (AQMod.UseAssets)
            {
                music = GetMusic().GetMusicID();
                musicPriority = MusicPriority.BossLow;
            }
            crabson = -1;
            leftClaw = -1;
            rightClaw = -1;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f);
            if (AQMod.calamityMod.IsActive)
            {
                npc.lifeMax *= 3;
                npc.defense += 6;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood);
                    }
                }
            }
            else
            {
                for (int i = 0; i < Math.Min(damage / 20 + 1, 1); i++)
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood);
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return contactDamage;
        }

        private void SpawnClaws(Vector2 center)
        {
            leftClaw = -1;
            rightClaw = -1;
            crabson = npc.whoAmI;
            npc.realLife = npc.whoAmI;
            for (int i = -1; i <= 1; i++)
            {
                if (i != 0)
                {
                    int n = NPC.NewNPC((int)center.X, (int)center.Y, npc.type);
                    if (i == -1)
                    {
                        leftClaw = n;
                    }
                    else
                    {
                        rightClaw = n;
                    }
                    Main.npc[n].position.X += 150f * i;
                    Main.npc[n].ai[0] = Phase_Intro;
                    Main.npc[n].direction = i;
                    Main.npc[n].spriteDirection = i;
                    Main.npc[n].realLife = npc.whoAmI;
                    Main.npc[n].defense *= 4;
                    Main.npc[n].defDefense = Main.npc[n].defense;
                    Main.npc[n].width += 20;
                    Main.npc[n].height += 50;
                    //Main.npc[n].dontTakeDamage = true;
                }
            }
            var crab = (Crabson)Left.modNPC;
            crab.rightClaw = rightClaw;
            crab.leftClaw = leftClaw;
            crab.crabson = crabson;

            crab = (Crabson)Right.modNPC;
            crab.rightClaw = rightClaw;
            crab.leftClaw = leftClaw;
            crab.crabson = crabson;
        }
        private bool CheckClaws()
        {
            return !(leftClaw == -1 || !Left.active || Left.type != npc.type ||
                rightClaw == -1 || !Right.active || Right.type != npc.type);
        }
        private void Kill()
        {
            npc.life = -1;
            npc.HitEffect();
            npc.active = false;
        }
        private void SetGoodbyeState()
        {
            npc.ai[0] = Phase_Goodbye;
            npc.ai[1] = 60f;
        }
        private void GroundMovement(Vector2 location)
        {
            if (npc.position.Y + npc.height < location.Y)
            {
                npc.velocity.Y += 0.65f;

                if (npc.position.Y + npc.height < location.Y - 240f && Collision.SolidCollision(npc.position, npc.width, npc.height))
                {
                    npc.velocity.Y *= 0.5f;
                }
            }
            else if ((npc.position.Y + npc.height - location.Y).Abs() < 10f)
            {
                npc.velocity.Y *= 0.75f;
            }
            else
            {
                if (Collision.SolidCollision(npc.position, npc.width, npc.height))
                {
                    npc.velocity.Y -= 0.05f;
                }
                else
                {
                    npc.velocity.Y -= 0.1f;
                }
                npc.velocity.Y = Math.Min(npc.velocity.Y, Math.Max(Main.player[npc.target].velocity.Y, 2.5f));
            }
            float differenceX = npc.position.X + npc.width / 2f - location.X;
            if (differenceX.Abs() > 100f)
            {
                int sign = Math.Sign(differenceX);
                npc.velocity.X -= sign * 0.085f;
            }
            else
            {
                npc.velocity.X *= 0.8f;
            }
        }
        private void Movement1()
        {
            Movement1(out var _, out var _);
        }
        private void Movement1(out Point tileCoordinates, out int j)
        {
            tileCoordinates = Main.player[npc.target].Center.ToTileCoordinates();
            bool toPlayer = true;
            for (j = 7; j < 36; j++)
            {
                if (Main.tile[tileCoordinates.X, tileCoordinates.Y + j] == null)
                {
                    Main.tile[tileCoordinates.X, tileCoordinates.Y + j] = new Tile();
                    continue;
                }
                var tile = Main.tile[tileCoordinates.X, tileCoordinates.Y + j];
                if (tile.active() && (tile.Solid() || tile.SolidTop()))
                {
                    GroundMovement(new Vector2(tileCoordinates.X * 16f + 8f, (tileCoordinates.Y + j) * 16f));
                    toPlayer = false;
                    break;
                }
            }
            if (toPlayer)
                GroundMovement(Main.player[npc.target].Center + new Vector2(0f, 128f + npc.height));
        }
        private void RandomizePhase(int current)
        {
            List<int> actions = new List<int>() { Phase_ClawShots, Phase_GroundBubbles, Phase_ClawSlams };
            npc.ai[0] = actions[Main.rand.Next(actions.Count)];
            contactDamage = true;
            npc.localAI[0] = 0f;
            Right.localAI[0] = 0f;
            Left.localAI[0] = 0f;
            npc.damage = npc.defDamage;
            Right.damage = Right.defDamage;
            Left.damage = Left.defDamage;
            npc.defense = npc.defDefense;
            Right.defense = Right.defDefense;
            Left.defense = Left.defDefense;
            if ((int)npc.ai[0] == Phase_ClawShots)
            {
                npc.ai[1] = Main.expertMode ? 240f : 320f;
                if (PhaseTwo && (Main.expertMode || Main.rand.NextFloat() < 0.75f))
                {
                    npc.ai[0] = Phase_ClawShotsShrapnal;
                }
                Left.ai[1] = 0f;
                Right.ai[1] = 0f;
                Left.ai[2] = 0f;
                Right.ai[2] = 0f;
            }
            else if ((int)npc.ai[0] == Phase_GroundBubbles)
            {
                if (current == Phase_GroundBubbles || current == Phase_HomingBubbles)
                {
                    RandomizePhase(current);
                    return;
                }
                if (PhaseTwo && (Main.expertMode || Main.rand.NextFloat() < 0.75f))
                {
                    npc.ai[0] = Phase_HomingBubbles;
                }
                npc.ai[1] = Main.expertMode ? 180f : 120f;
                npc.ai[2] = 0f;
                npc.defense *= 4;
                Right.defense *= 4;
                Left.defense *= 4;
            }
            else if ((int)npc.ai[0] == Phase_ClawSlams)
            {
                float firstSmash = Main.expertMode ? -8f : -60f;
                float secondSmash = Main.expertMode ? -68f : -128f;
                if (PhaseTwo)
                {
                    secondSmash += 28f * (npc.life * 2f / npc.lifeMax);
                }
                npc.ai[1] = Main.expertMode ? 200f : 400f;
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
            npc.netUpdate = true;
        }
        private Vector2 ClawIdlePosition()
        {
            return CrabsonNPC.Center + new Vector2(120f * npc.direction, -48f);
        }
        public int ShootProj<T>(Vector2 position, Vector2 velo, int damage, float ai0 = 0f, float ai1 = 0f, int extraUpdates = 0, int alpha = 0) where T : ModProjectile
        {
            return ShootProj(position, velo, ModContent.ProjectileType<T>(), damage, ai0, ai1, extraUpdates, alpha);
        }
        public int ShootProj(Vector2 position, Vector2 velo, int type, int damage, float ai0 = 0f, float ai1 = 0f, int extraUpdates = 0, int alpha = 0)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int p = Projectile.NewProjectile(position, velo, type, Main.expertMode ? damage / 3 : damage, 1f, Main.myPlayer, ai0, ai1);
                if (p == -1)
                    return -1;
                Main.projectile[p].extraUpdates += extraUpdates;
                Main.projectile[p].alpha += alpha;
                return p;
            }
            return -1;
        }
        public override void AI()
        {
            if ((int)npc.ai[0] == Phase_Goodbye)
            {
                if (npc.timeLeft > 20)
                {
                    npc.timeLeft = 20;
                }
                npc.velocity.X *= 0.9f;
                if (npc.velocity.Y < 0f)
                {
                    npc.velocity.Y *= 0.8f;
                }
                Right.rotation *= 0.8f;
                Left.rotation *= 0.8f;
                npc.velocity.Y += 0.1f;
                return;
            }
            Vector2 center = npc.Center;
            if ((int)npc.ai[0] == Phase_Initalize && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.ai[0] = Phase_Intro;
                npc.netUpdate = true;
                npc.TargetClosest(faceTarget: false);
                SpawnClaws(center);
            }
            if (CheckClaws())
            {
                if (!IsClaw)
                {
                    contactDamage = false;
                    npc.behindTiles = true;
                    if (!npc.HasValidTarget || (npc.Distance(Main.npc[npc.target].Center) > 4000f))
                    {
                        npc.TargetClosest(faceTarget: false);
                        if (!npc.HasValidTarget)
                        {
                            SetGoodbyeState();
                            return;
                        }
                    }
                    if ((int)npc.ai[0] == Phase_Intro)
                    {
                        Movement1(out var tileCoords, out var j);
                        npc.ai[1]++;
                        if ((npc.position.Y + npc.height - (tileCoords.Y + j) * 16f).Abs() < 20f || npc.ai[1] > 60f)
                        {
                            npc.ai[0] = Phase_ClawShots;
                            npc.ai[1] = Main.expertMode ? 240f : 320f;
                            Left.ai[1] = 0f;
                            Right.ai[1] = 0f;
                            Left.ai[2] = 0f;
                            Right.ai[2] = 0f;
                        }
                    }
                    else if ((int)npc.ai[0] == Phase_ClawShots || (int)npc.ai[0] == Phase_ClawSlams || (int)npc.ai[0] == Phase_ClawShotsShrapnal)
                    {
                        npc.ai[1]--;
                        Movement1();
                        if (npc.ai[1] <= 0f)
                        {
                            RandomizePhase((int)npc.ai[0]);
                        }
                    }
                    else if ((int)npc.ai[0] == Phase_GroundBubbles || (int)npc.ai[0] == Phase_HomingBubbles)
                    {
                        npc.ai[1]--;
                        var tileCoordinates = Main.player[npc.target].Center.ToTileCoordinates();
                        int randomness = Main.expertMode ? 4 : 3;
                        tileCoordinates.X += Main.rand.Next(-randomness, randomness);
                        bool toPlayer = true;
                        int bubbleTile = 40;
                        int start = Main.rand.Next(5, 12);
                        for (int j = start; j < 40; j++)
                        {
                            if (Main.tile[tileCoordinates.X, tileCoordinates.Y + j] == null)
                            {
                                Main.tile[tileCoordinates.X, tileCoordinates.Y + j] = new Tile();
                                continue;
                            }
                            var tile = Main.tile[tileCoordinates.X, tileCoordinates.Y + j];
                            if (tile.active())
                            {
                                if (bubbleTile == 40 && tile.Solid())
                                {
                                    bubbleTile = j;
                                }
                                if (j > 20 && (tile.Solid() || tile.SolidTop()))
                                {
                                    GroundMovement(new Vector2(tileCoordinates.X * 16f + 8f, (tileCoordinates.Y + j) * 16f + npc.height + 20f));
                                    toPlayer = false;
                                    break;
                                }
                            }
                        }
                        if (toPlayer)
                        {
                            GroundMovement(Main.player[npc.target].Center + new Vector2(0f, 320f + npc.height));
                        }
                        if (npc.ai[1] <= 0f)
                        {
                            RandomizePhase((int)npc.ai[0]);
                        }
                        npc.ai[2]++;
                        float time = Main.expertMode ? 40f : 80f;
                        if ((int)npc.ai[0] == Phase_HomingBubbles)
                        {
                            time -= 36f * (1f - npc.life / npc.lifeMax);
                        }
                        if (npc.ai[2] > time)
                        {
                            npc.ai[2] = 0f;
                            float shootTime = Main.expertMode ? 30f - (PhaseTwo ? 8f : 0f) : 40f;
                            if ((int)npc.ai[0] == Phase_HomingBubbles && Main.rand.NextBool())
                            {
                                var spawnPos = new Vector2((tileCoordinates.X + Main.rand.Next(-3, 3)) * 16f + 8f, (tileCoordinates.Y - Main.rand.Next(12, 20)) * 16f - 4f);
                                ShootProj<CrabsonBubble>(spawnPos, Vector2.Normalize(Main.player[npc.target].Center - spawnPos) * 0.01f, npc.damage, ai0: shootTime / 2f, alpha: 1);
                            }
                            else
                            {
                                if (bubbleTile != 40)
                                {
                                    ShootProj<CrabsonBubble>(new Vector2(tileCoordinates.X * 16f + 8f, (tileCoordinates.Y + bubbleTile) * 16f - 4f), new Vector2(0f, -0.01f), npc.damage, ai0: shootTime);
                                }
                                else
                                {
                                    if (Main.netMode != NetmodeID.Server)
                                        SoundID.Item85.Play(npc.Center, 0.7f);
                                    ShootProj<CrabsonBubble>(new Vector2(npc.position.X + npc.width / 2f, npc.position.Y - 4f), new Vector2(0f, -0.01f), npc.damage, ai0: 1f);
                                }
                            }
                        }
                    }
                }
                else
                {
                    contactDamage = false;
                    npc.behindTiles = false;
                    npc.noGravity = true;
                    npc.noTileCollide = true;
                    if (CrabsonNPC.ai[0] == Phase_ClawShots || CrabsonNPC.ai[0] == Phase_ClawShotsShrapnal)
                    {
                        var gotoPosition = Main.player[npc.target].Center + new Vector2(400f * npc.direction, 0f);

                        var difference = gotoPosition - center;
                        float l = difference.Length();
                        if (CrabsonNPC.ai[1] > 30f)
                        {
                            npc.ai[1]++;
                            int shootTime = Main.expertMode ? 80 : 160;
                            if (npc.whoAmI == leftClaw)
                            {
                                if (Right.ai[1] >= npc.ai[1] - 5f)
                                {
                                    npc.ai[1] = Right.ai[1] - shootTime / 2f;
                                }
                            }
                            if (npc.ai[1] > shootTime / 2f)
                            {
                                npc.rotation = MathHelper.Lerp(npc.rotation, 0.5f, 0.4f);
                            }
                            if (npc.ai[1] > shootTime)
                            {
                                npc.ai[1] = 1f;
                                npc.ai[2] = 0.5f;
                                npc.position.X += 40f * npc.direction;
                                npc.velocity = new Vector2(16f * npc.direction, 0f);
                                Main.PlaySound(SoundID.Item61, npc.Center);
                                ShootProj<CrabsonPearl>(npc.Center, new Vector2(20f * -npc.direction, 0f), npc.damage, ai1: (int)CrabsonNPC.ai[0] == Phase_ClawShotsShrapnal ? 1f : 0f);
                            }
                        }
                        if (CrabsonNPC.ai[1] < 30f)
                        {
                            npc.rotation *= 0.6f;
                            if (npc.rotation < 0.05f)
                            {
                                npc.rotation = 0f;
                            }
                        }
                        if (l < 0.01f)
                        {
                            npc.Center = gotoPosition;
                            npc.velocity = Vector2.Zero;
                            npc.ai[2] = 0f;
                        }
                        else
                        {
                            if (npc.ai[2] > 0f)
                            {
                                npc.ai[2] -= 0.05f;
                                if (npc.ai[2] < 0f)
                                {
                                    npc.ai[2] = 0f;
                                }
                            }
                            npc.velocity = Vector2.Lerp(npc.velocity, Vector2.Normalize(difference) * difference.Length() / 20f, Math.Max(0.7f - npc.ai[2], 0.01f));
                        }
                    }
                    else if ((int)CrabsonNPC.ai[0] == Phase_ClawSlams)
                    {
                        contactDamage = true;
                        npc.ai[1]++;
                        int time = 10;
                        if (npc.ai[1] + time < 0f)
                        {
                            npc.noGravity = false;
                            npc.noTileCollide = false;
                        }
                        else
                        {
                            int smashTime = Main.expertMode ? 45 : 75;
                            if (npc.ai[1] > smashTime)
                            {
                                npc.damage = npc.defDamage * 2;
                                npc.velocity.X = 0f;
                                npc.velocity.Y = Main.expertMode ? 32f : 16f;
                                int fallingTime = Main.expertMode ? 10 : 20;
                                if (npc.ai[1] > smashTime + fallingTime)
                                {
                                    npc.noGravity = false;
                                    npc.noTileCollide = false;
                                    if (Main.netMode != NetmodeID.Server && (int)npc.localAI[0] == 0)
                                    {
                                        SoundID.Item14.Play(npc.Center);
                                        FX.AddShake(2f, 2f);
                                    }
                                    npc.localAI[0] = 1f;
                                }
                            }
                            else
                            {
                                npc.damage = 0;
                                var gotoPos = Main.player[npc.target].Center;
                                if (npc.position.Y < Main.player[npc.target].position.Y - 80)
                                {
                                    gotoPos += new Vector2(0f, -320f - npc.height);
                                }
                                else
                                {
                                    gotoPos += new Vector2(200f * npc.direction, -120f - npc.height);
                                }
                                npc.Center = Vector2.Lerp(npc.Center, gotoPos, 0.05f + 0.4f * (npc.ai[0] / smashTime));
                            }
                        }
                    }
                    else
                    {
                        npc.Center = Vector2.Lerp(npc.Center, ClawIdlePosition(), CrabsonNPC.ai[0] == Phase_Intro ? 0.5f : 0.1f);
                    }
                    if (crabson == -1 || !Main.npc[crabson].active || Main.npc[crabson].type != npc.type)
                    {
                        Kill();
                    }
                    else
                    {
                        npc.target = CrabsonNPC.target;
                    }
                }
            }
            else
            {
                if (!IsClaw)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (i == npc.whoAmI || i == leftClaw || i == rightClaw)
                        {
                            continue;
                        }
                        if (Main.npc[i].active && Main.npc[i].type == npc.type)
                        {
                            var crab = (Crabson)npc.modNPC;
                            crab.crabson = npc.whoAmI;
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
                    if (crabson == -1 || !Main.npc[crabson].active || Main.npc[crabson].type != npc.type)
                    {
                        Kill();
                    }
                }
            }
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

        public override ModifiableMusic GetMusic() => AQMod.CrabsonMusic;

        public override void BossHeadSpriteEffects(ref SpriteEffects spriteEffects)
        {
            if (npc.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;
        }

        public override void BossHeadSlot(ref int index)
        {
            if (IsClaw)
            {
                index = ModContent.GetModBossHeadSlot(this.GetPath("Claw_Head_Boss"));
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (IsClaw)
            {
                var texture = ModContent.GetTexture(this.GetPath("Claw"));
                int frameHeight = texture.Height / 2;
                var frame = new Rectangle(0, frameHeight, texture.Width, frameHeight - 2);
                var origin = new Vector2(16f, 48f);
                var drawPosition = npc.position + new Vector2(origin.X + (npc.direction == 1 ? npc.width - origin.X * 2f : 0), npc.width / 2f);
                origin.X = npc.direction == 1 ? texture.Width - origin.X : origin.X;
                var spriteEffects = npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                float rotation = npc.rotation / 2f;
                rotation = npc.direction == -1 ? -rotation : rotation;
                Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, frame, drawColor, -rotation, origin, npc.scale, spriteEffects, 0f);
                frame.Y = 0;
                Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, frame, drawColor, rotation, origin, npc.scale, spriteEffects, 0f);
                return false;
            }
            if (DrawHelper.NPCsBehindAllNPCs.drawingNow)
            {
                var drawCoordinates = npc.Center;
                var chain = ModContent.GetTexture(this.GetPath("Claw_Chain"));
                AQGraphics.Rendering.JerryChain(chain, new Vector2(drawCoordinates.X - 24f, drawCoordinates.Y), Left.position + new Vector2(0f, Left.height / 2f - 24f));
                AQGraphics.Rendering.JerryChain(chain, new Vector2(drawCoordinates.X + 24f, drawCoordinates.Y), Right.Center + new Vector2(Right.width / 2f, -24f));
            }
            else
            {
                DrawHelper.NPCsBehindAllNPCs.Add(npc.whoAmI);
                //if ((int)npc.ai[0] == Phase_Goodbye)
                //{
                //    FX.cameraFocusReset = 20;
                //    if ((int)npc.ai[1] == 6)
                //    {
                //        FX.CameraFocus = npc.Center;
                //        FX.cameraFocusNPC = -1;
                //        FX.SetFlash(FX.CameraFocus, 4f * AQConfigClient.Instance.FlashIntensity, 10f);
                //        FX.SetShake(20f, 10f);
                //    }
                //    else if (npc.ai[1] > 6f)
                //    {
                //        if (FX.cameraFocusNPC != npc.whoAmI)
                //        {
                //            FX.cameraFocusLerp = 0.001f;
                //        }
                //        FX.cameraFocusNPC = npc.whoAmI;
                //    }
                //}
            }
            return true;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return IsClaw ? false : (bool?)null;
        }

        public override void NPCLoot()
        {
            Rectangle rect = npc.getRect();
            if (!WorldDefeats.DownedCrabson && !NPC.AnyNPCs(ModContent.NPCType<Robster>()))
            {
                var claw = Main.rand.NextBool() ? Right : Left;
                NPC.NewNPC((int)claw.position.X + claw.width / 2, (int)claw.position.Y + claw.height / 2, ModContent.NPCType<Robster>());
            }
            WorldDefeats.DownedCrabson = true;
            LootDrops.DropItemChance(npc, ModContent.ItemType<CrabsonTrophy>(), 10);
            if (Main.expertMode)
            {
                npc.DropBossBags();
                return;
            }
            LootDrops.DropItemChance(npc, ModContent.ItemType<CrabsonMask>(), 7);
            LootDrops.DropItemChance(npc, ModContent.ItemType<BreakdownDye>(), 2);
            LootDrops.DropItem(npc, ModContent.ItemType<AquaticEnergy>(), 3, 5);
            LootDrops.DropItem(npc, ModContent.ItemType<CrustaciumBlob>(), 50, 120);
            LootDrops.DropItem(npc, new int[] { ModContent.ItemType<Bubbler>(), ModContent.ItemType<CinnabarBow>(), ModContent.ItemType<JerryClawFlail>(), ModContent.ItemType<Crabsol>() });
        }
    }
}