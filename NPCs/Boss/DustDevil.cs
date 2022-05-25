using Aequus.Graphics;
using Aequus.Projectiles.Monster.DustDevil;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Boss
{
    [AutoloadBossHead]
    public class DustDevil : AequusBoss
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }

        public const int PHASE_GOODBYE = -1;
        public const int PHASE_INTRO = 0;
        public const int PHASE_TORNADOBULLETS = 1;

        public bool PhaseTwo => NPC.life * (Main.expertMode ? 2f : 4f) <= NPC.lifeMax;

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

        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 100;
            NPC.lifeMax = 14500;
            NPC.damage = 50;
            NPC.defense = 12;
            NPC.aiStyle = -1;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.boss = true;
            NPC.value = Item.buyPrice(gold: 10);
            NPC.lavaImmune = true;
            NPC.trapImmune = true;
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return PhaseTwo ? Color.Red : Color.White;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override void AI()
        {
            switch ((int)NPC.ai[0])
            {
                case PHASE_GOODBYE:
                    {
                        NPC.timeLeft = Math.Min(NPC.timeLeft, 100);
                        NPC.velocity.X *= 0.8f;
                        if (NPC.velocity.Y > 0f)
                        {
                            NPC.velocity.Y *= 0.8f;
                        }
                        NPC.velocity.Y -= 0.05f;
                    }
                    break;

                case PHASE_INTRO:
                    {
                        if ((int)NPC.ai[1] == 0)
                        {
                            NPC.ai[1]++;
                            if (!PlrCheck())
                            {
                                return;
                            }
                        }
                        RandomizePhase();
                    }
                    break;

                case PHASE_TORNADOBULLETS:
                    {
                        var gotoPosition = Main.player[NPC.target].Center + new Vector2(0f, -NPC.height - 300);
                        NPC.ai[3]++;
                        if (NPC.ai[3] < 30f)
                        {
                            NPC.ai[1] = gotoPosition.X;
                            NPC.ai[2] = gotoPosition.Y;
                        }
                        else
                        {
                            NPC.ai[1] = MathHelper.Lerp(NPC.ai[1], gotoPosition.X, 0.001f);
                            NPC.ai[2] = MathHelper.Lerp(NPC.ai[2], gotoPosition.Y, 0.001f);
                        }

                        NPC.velocity = GetTo(new Vector2(NPC.ai[1], NPC.ai[2]), addSpeedX: 0.8f, addSpeedY: 0.4f);

                        if (NPC.ai[3] > 50f && NPC.ai[3] < 100f)
                        {
                            int amt = 4;
                            if (Main.getGoodWorld)
                            {
                                amt *= 2;
                            }
                            int ticksPerShot = (int)(50f / amt);
                            if ((NPC.ai[3] - 50f) <= ticksPerShot * amt 
                                && (int)(NPC.ai[3] - 50f) % ticksPerShot == 0)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(10f, 0f), 
                                        ModContent.ProjectileType<DustDevilTornadoBullet>(), NPC.damage, 1f, Main.myPlayer, 180f, 20f);
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(-10f, 0f), 
                                        ModContent.ProjectileType<DustDevilTornadoBullet>(), NPC.damage, 1f, Main.myPlayer, 180f, 20f);
                                }
                                SoundID.Item1?.PlaySound(NPC.Center);
                            }
                        }
                        if (NPC.ai[3] > 160f)
                        {
                            NPC.ai[3] = 0f;
                        }
                    }
                    break;
            }
        }

        public void RandomizePhase(int? phase = null, float? hpRatio = null)
        {
            int p = phase ?? (int)NPC.ai[0];
            float hp = hpRatio ?? NPC.life / (float)NPC.lifeMax;
            if (Main.getGoodWorld && PhaseTwo)
            {
                hp /= 4f;
            }

            var l = InnerGetChooseablePhases(p, hp);
            SetPhase(l[Main.rand.Next(l.Count)]);
        }
        public List<int> InnerGetChooseablePhases(int phase, float hpRatio)
        {
            var l = new List<int>()
            {
                PHASE_TORNADOBULLETS,
            };
            return l;
        }
        public void SetPhase(int phase)
        {
            ClearAI();
            NPC.ai[0] = phase;
        }
        public bool PlrCheck()
        {
            NPC.TargetClosest(faceTarget: false);
            NPC.netUpdate = true;
            if (Main.player[NPC.target].dead)
            {
                NPC.ai[0] = PHASE_GOODBYE;
                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
                NPC.ai[3] = 0f;
                NPC.localAI[0] = 0f;
                NPC.localAI[1] = 0f;
                NPC.localAI[2] = 0f;
                NPC.localAI[3] = 0f;
                return false;
            }
            return true;
        }

        public Vector2 GetTo(Vector2 spot, float addSpeedX = 0.8f, float addSpeedY = 0.4f, float maxSpeed = 20f, float wrongWayMultiplier = 0.96f, float minDistance = 50f)
        {
            if (NPC.Distance(spot) < minDistance)
            {
                return NPC.velocity;
            }

            var velocity = NPC.velocity;
            if (NPC.position.X < spot.X)
            {
                if (velocity.X < maxSpeed)
                {
                    velocity.X += addSpeedX;
                    if (velocity.X < 0f)
                    {
                        velocity.X *= wrongWayMultiplier;
                    }
                }
            }
            else
            {
                if (velocity.X > -maxSpeed)
                {
                    velocity.X -= addSpeedX;
                    if (velocity.X > 0f)
                    {
                        velocity.X *= wrongWayMultiplier;
                    }
                }
            }

            if (NPC.position.Y < spot.Y)
            {
                if (velocity.Y < maxSpeed)
                {
                    velocity.Y += addSpeedX;
                    if (velocity.Y < 0f)
                    {
                        velocity.Y *= wrongWayMultiplier;
                    }
                }
            }
            else
            {
                if (velocity.Y > -maxSpeed)
                {
                    velocity.Y -= addSpeedX;
                    if (velocity.Y > 0f)
                    {
                        velocity.Y *= wrongWayMultiplier;
                    }
                }
            }

            return velocity;
        }
        public Vector2 GetTo(Vector2 spot, float addSpeed = 0.8f, float maxSpeed = 20f, float wrongWayMultiplier = 0.96f, float minDistance = 50f)
        {
            return GetTo(spot, addSpeed, addSpeed, maxSpeed, wrongWayMultiplier, minDistance);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }
    }
}