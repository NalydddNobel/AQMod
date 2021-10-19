using AQMod.Assets;
using AQMod.Assets.Textures;
using AQMod.Common;
using AQMod.Common.Config;
using AQMod.Common.CrossMod;
using AQMod.Common.Utilities;
using AQMod.Content;
using AQMod.Content.Dusts;
using AQMod.Effects;
using AQMod.Effects.ScreenEffects;
using AQMod.Items;
using AQMod.Items.Placeable;
using AQMod.Items.Tools.Markers;
using AQMod.Items.Vanities.Dyes;
using AQMod.Items.Vanities.Pets;
using AQMod.Items.Weapons.Magic;
using AQMod.Items.Weapons.Melee;
using AQMod.Items.Weapons.Ranged.Bullet;
using AQMod.Projectiles;
using AQMod.Projectiles.Monster;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Boss.Starite
{
    [AutoloadBossHead()]
    public class OmegaStarite : ModNPC, IModifiableMusicNPC
    {
        public const float CIRCUMFERENCE = 120;
        public const float RADIUS = CIRCUMFERENCE / 2f;
        private const float DEATH_TIME = MathHelper.PiOver4 * 134;

        public class OmegaStariteOrb
        {
            public const int INNER_RING = 5;
            public const float INNER_RING_SCALE = 1f;
            public const float INNER_RING_SCALE_FTW = 1.1f;

            public const int OUTER_RING = 8;
            public const float OUTER_RING_SCALE = 1.1f;
            public const float OUTER_RING_SCALE_EXPERT = 1.2f;
            public const float OUTER_RING_SCALE_FTW = 1.25f;
            public const float OUTER_RING_CIRCUMFERENCE_MULT = 1.5f;

            public const int FTW_RING_3 = 13;
            public const float FTW_RING_3_SCALE = 1.75f;
            public const float FTW_RING_3_CIRCUMFERENCE_MULT = 3f;

            public const int FTW_RING_4 = 24;
            public const float FTW_RING_4_SCALE = 2f;
            public const float FTW_RING_4_CIRCUMFERENCE_MULT = 4.2f;

            public const int SPHERE_COUNT = OUTER_RING + INNER_RING;
            public const int SPHERE_COUNT_FTW = OUTER_RING + INNER_RING;

            public const int SIZE = 50;
            public const int SIZE_HALF = SIZE / 2;

            public Vector3 position;
            public Vector3 drawOffset;
            public float radius;
            public float scale;

            public readonly float defRadius;
            public readonly float defRotation;
            public readonly float maxRotation;

            public OmegaStariteOrb(Vector3 position, float scale = 1f, float radius = CIRCUMFERENCE, float rotation = 0f, float maxRotation = MathHelper.TwoPi / INNER_RING)
            {
                this.position = position;
                this.scale = scale;
                defRadius = radius;
                this.radius = radius;
                defRotation = rotation;
                this.maxRotation = maxRotation;
            }

            public bool CheckCollision()
            {
                return position.Z.Abs() < 1f;
            }
        }

        public float innerRingRotation;
        public float innerRingPitch;
        public float innerRingRoll;

        public float outerRingRotation;
        public float outerRingPitch;
        public float outerRingRoll;

        public int skipDeathTimer;
        public List<OmegaStariteOrb> orbs;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[npc.type] = 7;
            NPCID.Sets.TrailCacheLength[npc.type] = 60;
            Main.npcFrameCount[npc.type] = 14;
        }

        public override void SetDefaults()
        {
            npc.width = 120;
            npc.height = 120;
            npc.lifeMax = 12250;
            npc.damage = 50;
            npc.defense = 18;
            npc.HitSound = SoundID.NPCDeath57;
            npc.DeathSound = SoundID.NPCDeath55;
            npc.aiStyle = -1;
            npc.noGravity = true;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(gold: 18);
            npc.boss = true;
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.Venom] = true;
            npc.buffImmune[BuffID.Bleeding] = true;
            npc.npcSlots = 10f;
            npc.noTileCollide = true;
            npc.trapImmune = true;
            npc.lavaImmune = true;
            bossBag = ModContent.ItemType<StariteBag>();
            if (!AQMod.glimmerEvent.IsActive)
                skipDeathTimer = 600;
            if (AQMod.CanUseAssets)
            {
                music = GetMusic().GetMusicID();
                musicPriority = MusicPriority.BossMedium;
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return new Color(255, 255, 255, 240);
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f) + 4000 * numPlayers;
        }

        public int GetOmegiteDamage()
        {
            return Main.expertMode ? 25 : 30;
        }

        public bool IsOmegaLaserActive()
        {
            return npc.ai[0] == PHASE_OMEGA_LASER && npc.ai[2] < 1200f;
        }

        public const int PHASE3_DEBUG = 1001;
        public const int PHASE3_3D_WORM = 1000;

        public const int PHASE_HYPER_STARITE_PART2_ALT = 8;
        public const int PHASE_OMEGA_LASER_PART0 = 7;
        public const int PHASE_OMEGA_LASER = 6;
        public const int PHASE_STAR_BULLETS = 5;
        public const int PHASE_ASSAULT_PLAYER = 4;
        public const int PHASE_HYPER_STARITE_PART2 = 3;
        public const int PHASE_HYPER_STARITE_PART1 = 2;
        public const int PHASE_HYPER_STARITE_PART0 = 1;
        public const int PHASE_INIT = 0;
        public const int PHASE_DEAD = -1;
        public const int PHASE_NOVA = -2;
        public const int PHASE_GOODBYE = -3;

        private bool PlrCheck()
        {
            npc.TargetClosest(faceTarget: false);
            if (Main.player[npc.target].dead)
            {
                npc.ai[0] = PHASE_GOODBYE;
                npc.ai[1] = 0f;
                npc.ai[2] = 0f;
                npc.ai[3] = 0f;
                npc.localAI[0] = 0f;
                npc.localAI[1] = 0f;
                npc.localAI[2] = 0f;
                npc.localAI[3] = 0f;
                return false;
            }
            return true;
        }

        private void spawnRing(Vector2 center, int ringCount, float radius, float scale)
        {
            float rot = MathHelper.TwoPi / ringCount;
            for (int i = 0; i < ringCount; i++)
            {
                orbs.Add(new OmegaStariteOrb(
                    Vector3.Transform(new Vector3(radius, 0f, 0f), Matrix.CreateFromYawPitchRoll(rot * i, MathHelper.PiOver2, 0f)) + new Vector3(center, 0f),
                    scale,
                    radius,
                    rot * i,
                    rot));
            }
        }

        public bool HardVersion()
        {
            return AQMod.HarderOmegaStarite || AQMod.glimmerEvent.IsActive;
        }

        public void Init()
        {
            npc.TargetClosest(faceTarget: false);
            OmegaStariteSceneManager.OmegaStariteIndexCache = (short)npc.whoAmI;
            orbs = new List<OmegaStariteOrb>();
            var center = npc.Center;
            if (Main.expertMode)
            {
                if (HardVersion())
                {
                    spawnRing(center, OmegaStariteOrb.INNER_RING, CIRCUMFERENCE, 1f);
                    spawnRing(center, OmegaStariteOrb.OUTER_RING, CIRCUMFERENCE * 1.75f, 1.25f);
                }
                else
                {
                    spawnRing(center, OmegaStariteOrb.INNER_RING, CIRCUMFERENCE * 0.8f, OmegaStariteOrb.INNER_RING_SCALE);
                    spawnRing(center, OmegaStariteOrb.OUTER_RING, CIRCUMFERENCE * OmegaStariteOrb.OUTER_RING_CIRCUMFERENCE_MULT, OmegaStariteOrb.OUTER_RING_SCALE_EXPERT);
                }
            }
            else
            {
                spawnRing(center, OmegaStariteOrb.INNER_RING, CIRCUMFERENCE * 0.75f, OmegaStariteOrb.INNER_RING_SCALE);
                spawnRing(center, OmegaStariteOrb.OUTER_RING, CIRCUMFERENCE * OmegaStariteOrb.OUTER_RING_CIRCUMFERENCE_MULT, OmegaStariteOrb.OUTER_RING_SCALE);
            }
            int damage = GetOmegiteDamage();
            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<OmegaStariteCollider>(), damage, 1f, Main.myPlayer, npc.whoAmI);
        }

        public void SetInnerRingRotation(Vector2 center)
        {
            innerRingRotation %= orbs[0].maxRotation;
            for (int i = 0; i < OmegaStariteOrb.INNER_RING; i++)
            {
                orbs[i].position = Vector3.Transform(new Vector3(orbs[i].radius, 0f, 0f), Matrix.CreateFromYawPitchRoll(innerRingPitch, innerRingRoll, orbs[i].defRotation + innerRingRotation)) + new Vector3(center, 0f);
            }
        } // fsr it doesn't look right unless you add them in as X, Y, and Z, instead of Z, X, Y

        public void SetOuterRingRotation(Vector2 center)
        {
            outerRingRotation %= orbs[OmegaStariteOrb.INNER_RING].maxRotation;
            for (int i = OmegaStariteOrb.INNER_RING; i < OmegaStariteOrb.SPHERE_COUNT; i++)
            {
                orbs[i].position = Vector3.Transform(new Vector3(orbs[i].radius, 0f, 0f), Matrix.CreateFromYawPitchRoll(outerRingPitch, outerRingRoll, orbs[i].defRotation + outerRingRotation)) + new Vector3(center, 0f);
            }
        }

        public void Spin(Vector2 center)
        {
            SetInnerRingRotation(center);
            SetOuterRingRotation(center);
        }

        public void SetInnerRingRadius(float newRadius)
        {
            for (int i = 0; i < OmegaStariteOrb.INNER_RING; i++)
            {
                orbs[i].radius = newRadius;
            }
        }

        public void SetOuterRingRadius(float newRadius)
        {
            for (int i = OmegaStariteOrb.INNER_RING; i < OmegaStariteOrb.SPHERE_COUNT; i++)
            {
                orbs[i].radius = newRadius;
            }
        }

        public override void AI()
        {
            if (AQNPC.CheckStariteDeath(npc))
            {
                npc.life = -1;
                OmegaStariteSceneManager.OmegaStariteIndexCache = -1;
                OmegaStariteSceneManager.Scene = 0;
                npc.HitEffect();
                Main.PlaySound(SoundID.Dig, npc.Center);
                npc.active = false;
                return;
            }
            if (skipDeathTimer > 0)
                skipDeathTimer--;
            bool hardVersion = HardVersion();
            Vector2 center = npc.Center;
            Player player = Main.player[npc.target];
            var plrCenter = player.Center;
            float speed = npc.velocity.Length();
            switch ((int)npc.ai[0])
            {
                default:
                {
                    innerRingRotation += 0.0314f;
                    innerRingRoll += 0.0157f;
                    innerRingPitch += 0.01f;

                    outerRingRotation += 0.0157f;
                    outerRingRoll += 0.0314f;
                    outerRingPitch += 0.011f;
                    npc.Center = plrCenter + new Vector2(0f, -CIRCUMFERENCE * 2f);
                }
                break;

                case 1002:
                {
                    npc.ai[2]++;
                    if (npc.ai[2] > 300f)
                    {
                        if (npc.ai[1] > 0.0314)
                            npc.ai[1] -= 0.0005f;
                        else
                        {
                            npc.ai[1] = 0.0314f;
                        }
                        innerRingRotation += npc.ai[1];
                        outerRingRotation += npc.ai[1] * 0.5f;
                        bool innerRing = false;
                        if (orbs[0].radius > orbs[0].defRadius)
                            SetInnerRingRadius(orbs[0].radius - MathHelper.Pi);
                        else
                            innerRing = true;
                        bool outerRing = false;
                        if (orbs[OmegaStariteOrb.INNER_RING].radius > orbs[OmegaStariteOrb.INNER_RING].defRadius)
                            SetOuterRingRadius(orbs[OmegaStariteOrb.INNER_RING].radius - MathHelper.PiOver2 * 3f);
                        else
                            outerRing = true;
                        if (innerRing && outerRing)
                        {
                            SetInnerRingRadius(orbs[0].defRadius);
                            SetOuterRingRadius(orbs[OmegaStariteOrb.INNER_RING].defRadius);
                            if (PlrCheck())
                            {
                                var choices = new List<int>
                                {
                                    PHASE_ASSAULT_PLAYER,
                                };
                                if (npc.life / (float)npc.lifeMax < (Main.expertMode ? 0.5f : 0.33f))
                                    choices.Add(PHASE_STAR_BULLETS);
                                if (choices.Count == 1)
                                    npc.ai[0] = choices[0];
                                else
                                {
                                    npc.ai[0] = choices[Main.rand.Next(choices.Count)];
                                }
                                npc.ai[1] = 0f;
                                npc.ai[2] = 0f;
                                npc.ai[3] = 0f;
                                npc.localAI[1] = 0f;
                            }
                        }
                    }
                    else if ((center - plrCenter).Length() > 1800f)
                    {
                        npc.ai[2] = 300f;
                        innerRingRotation += npc.ai[1];
                        outerRingRotation += npc.ai[1] * 0.5f;
                    }
                    else
                    {
                        if (npc.ai[1] >= 0.0628f)
                            npc.ai[1] = 0.0628f;
                        else
                        {
                            npc.ai[1] += 0.0002f;
                        }
                        innerRingRotation += npc.ai[1];
                        outerRingRotation += npc.ai[1] * 0.5f;
                        npc.localAI[1]++;
                        SetInnerRingRadius(MathHelper.Lerp(orbs[0].radius, orbs[0].defRadius * npc.ai[3] + (float)Math.Sin(npc.localAI[1] * 0.0314f) * 60f, 0.25f));
                        SetOuterRingRadius(MathHelper.Lerp(orbs[OmegaStariteOrb.INNER_RING].radius, orbs[OmegaStariteOrb.INNER_RING].defRadius * (npc.ai[3] + 1f), 0.025f));
                        if (npc.ai[2] > 100f)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (hardVersion || Vector2.Distance(plrCenter, center) > orbs[OmegaStariteOrb.INNER_RING].radius)
                                {
                                    npc.localAI[0]++;
                                    if (npc.localAI[0] > (Main.expertMode ? 3f : 12f))
                                    {
                                        float lifePercent = npc.life / (float)npc.lifeMax;
                                        if (lifePercent < 0.75f)
                                        {
                                            var diff = new Vector2(orbs[OmegaStariteOrb.INNER_RING].position.X, orbs[OmegaStariteOrb.INNER_RING].position.Y) - npc.Center;
                                            var shootDir = Vector2.Normalize(diff).RotatedBy(MathHelper.PiOver2) * 12f;
                                            int type = ModContent.ProjectileType<OmegaBullet>();
                                            int damage = 30;
                                            if (Main.expertMode)
                                                damage = 20;
                                            for (int i = 0; i < OmegaStariteOrb.OUTER_RING; i++)
                                            {
                                                float rot = orbs[i + OmegaStariteOrb.INNER_RING].maxRotation * i;
                                                var position = center + diff.RotatedBy(rot);
                                                Main.PlaySound(SoundID.Trackable, position, 55 + Main.rand.Next(3));
                                                Projectile.NewProjectile(position, shootDir.RotatedBy(rot), type, damage, 1f, player.whoAmI);
                                            }
                                        }
                                        npc.localAI[0] = 0f;
                                    }
                                }
                            }
                        }
                    }
                }
                break;

                case PHASE_OMEGA_LASER_PART0:
                {
                    if (npc.ai[1] == 0f)
                    {
                        innerRingPitch %= MathHelper.Pi;
                        innerRingRoll %= MathHelper.Pi;
                        outerRingPitch %= MathHelper.Pi;
                        outerRingRoll %= MathHelper.Pi;
                    }
                    npc.ai[1] += 0.0002f;
                    const float lerpValue = 0.025f;
                    const float xLerp = 0f;
                    const float yLerp = 0f;
                    innerRingPitch = innerRingPitch.AngleLerp(MathHelper.PiOver2, lerpValue);
                    innerRingRoll = innerRingRoll.AngleLerp(-MathHelper.PiOver2, lerpValue);
                    outerRingPitch = outerRingPitch.AngleLerp(xLerp, lerpValue);
                    outerRingRoll = outerRingRoll.AngleLerp(yLerp, lerpValue);
                    if (npc.ai[1] > 0.0314f)
                    {
                        const float marginOfError = 0.314f;
                        if (innerRingPitch.IsCloseEnoughTo(MathHelper.PiOver2, marginOfError) &&
                            innerRingRoll.IsCloseEnoughTo(-MathHelper.PiOver2, marginOfError) &&
                            outerRingPitch.IsCloseEnoughTo(yLerp, marginOfError) &&
                            outerRingRoll.IsCloseEnoughTo(yLerp, marginOfError))
                        {
                            npc.velocity = Vector2.Normalize(plrCenter - center) * npc.velocity.Length();
                            innerRingPitch = MathHelper.PiOver2;
                            innerRingRoll = -MathHelper.PiOver2;
                            outerRingPitch = xLerp;
                            outerRingRoll = yLerp;
                            if (PlrCheck())
                            {
                                npc.ai[0] = PHASE_OMEGA_LASER;
                                npc.ai[1] = 0f;
                                npc.ai[3] = 3f + (1f - npc.life / (float)npc.lifeMax) * 1.5f;
                            }
                        }
                    }
                    else
                    {
                        innerRingRotation += 0.0314f - npc.ai[1];
                        outerRingRotation += 0.0157f - npc.ai[1] * 0.5f;
                    }
                }
                break;

                case PHASE_OMEGA_LASER:
                {
                    npc.ai[2]++;
                    if (npc.ai[2] > 1200f)
                    {
                        if (npc.ai[1] > 0.0314)
                            npc.ai[1] -= 0.0005f;
                        else
                        {
                            npc.ai[1] = 0.0314f;
                        }
                        innerRingRotation += npc.ai[1];
                        outerRingRotation += npc.ai[1] * 0.5f;
                        bool outerRing = false;
                        if (orbs[OmegaStariteOrb.INNER_RING].radius > orbs[OmegaStariteOrb.INNER_RING].defRadius)
                        {
                            SetOuterRingRadius(orbs[OmegaStariteOrb.INNER_RING].radius - MathHelper.PiOver2 * 3f);
                            if (hardVersion || Vector2.Distance(plrCenter, center) > orbs[OmegaStariteOrb.INNER_RING].radius)
                            {
                                npc.localAI[0]++;
                                if (npc.localAI[0] > (Main.expertMode ? 12f : 60f))
                                {
                                    float lifePercent = npc.life / (float)npc.lifeMax;
                                    if (lifePercent < 0.75f)
                                    {
                                        Main.PlaySound(SoundID.Trackable, npc.Center, 55 + Main.rand.Next(3));
                                        var diff = new Vector2(orbs[OmegaStariteOrb.INNER_RING].position.X, orbs[OmegaStariteOrb.INNER_RING].position.Y) - npc.Center;
                                        var shootDir = Vector2.Normalize(diff).RotatedBy(MathHelper.PiOver2) * 7.5f;
                                        int type = ModContent.ProjectileType<OmegaBullet>();
                                        int damage = 25;
                                        if (Main.expertMode)
                                            damage = 18;
                                        for (int i = 0; i < OmegaStariteOrb.OUTER_RING; i++)
                                        {
                                            float rot = orbs[i + OmegaStariteOrb.INNER_RING].maxRotation * i;
                                            var position = center + diff.RotatedBy(rot);
                                            Main.PlaySound(SoundID.Trackable, position, 55 + Main.rand.Next(3));
                                            Projectile.NewProjectile(position, shootDir.RotatedBy(rot), type, damage, 1f, player.whoAmI);
                                        }
                                    }
                                    npc.localAI[0] = 0f;
                                }
                            }
                        }
                        else
                        {
                            outerRing = true;
                        }
                        if (outerRing)
                        {
                            SetInnerRingRadius(orbs[0].defRadius);
                            SetOuterRingRadius(orbs[OmegaStariteOrb.INNER_RING].defRadius);
                            if (PlrCheck())
                            {
                                var choices = new List<int>
                                {
                                    PHASE_ASSAULT_PLAYER,
                                    PHASE_STAR_BULLETS,
                                };
                                npc.ai[0] = choices[Main.rand.Next(choices.Count)];
                                npc.ai[1] = 0f;
                                npc.ai[2] = 0f;
                                npc.ai[3] = 0f;
                                npc.localAI[0] = 0f;
                                npc.localAI[1] = 0f;
                                npc.localAI[2] = 0f;
                            }
                        }
                    }
                    else if ((center - plrCenter).Length() > 1800f)
                    {
                        npc.ai[2] = 300f;
                        innerRingRotation += npc.ai[1];
                        outerRingRotation += npc.ai[1] * 0.5f;
                    }
                    else
                    {
                        if (npc.ai[1] >= 0.0628f)
                            npc.ai[1] = 0.0628f;
                        else
                        {
                            npc.ai[1] += 0.0002f;
                        }
                        innerRingRotation += npc.ai[1];
                        outerRingRotation += npc.ai[1] * 0.5f;
                        SetOuterRingRadius(MathHelper.Lerp(orbs[OmegaStariteOrb.INNER_RING].radius, orbs[OmegaStariteOrb.INNER_RING].defRadius * (npc.ai[3] + 1f), 0.025f));
                        if (npc.ai[2] > 100f)
                        {
                            if (npc.localAI[1] == 0f)
                            {
                                if (PlrCheck())
                                {
                                    npc.localAI[1] = 1f;
                                    Main.PlaySound(SoundID.Trackable, npc.Center, 188);
                                    if (Main.netMode != NetmodeID.Server)
                                        ScreenShakeManager.AddEffect(new OmegaStariteScreenShake(AQMod.MultIntensity(8), 0.02f * AQMod.EffectIntensity));
                                    int p = Projectile.NewProjectile(center, new Vector2(0f, 0f), ModContent.ProjectileType<OmegaRay>(), 100, 1f, Main.myPlayer, npc.whoAmI);
                                    Main.projectile[p].scale = 0.75f;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            if (innerRingRoll > MathHelper.PiOver2 * 6f)
                                npc.localAI[2] -= Main.expertMode ? 0.001f : 0.00045f;
                            else
                            {
                                npc.localAI[2] += Main.expertMode ? 0.00015f : 0.000085f;
                            }
                            if (Main.netMode != NetmodeID.MultiplayerClient && (hardVersion || Vector2.Distance(plrCenter, center) > orbs[OmegaStariteOrb.INNER_RING].radius))
                            {
                                npc.localAI[0]++;
                                if (npc.localAI[0] > (Main.expertMode ? 3f : 12f))
                                {
                                    float lifePercent = npc.life / (float)npc.lifeMax;
                                    if (lifePercent < 0.75f)
                                    {
                                        var diff = new Vector2(orbs[OmegaStariteOrb.INNER_RING].position.X, orbs[OmegaStariteOrb.INNER_RING].position.Y) - npc.Center;
                                        var shootDir = Vector2.Normalize(diff).RotatedBy(MathHelper.PiOver2) * 12f;
                                        int type = ModContent.ProjectileType<OmegaBullet>();
                                        int damage = 30;
                                        if (Main.expertMode)
                                            damage = 20;
                                        for (int i = 0; i < OmegaStariteOrb.OUTER_RING; i++)
                                        {
                                            float rot = orbs[i + OmegaStariteOrb.INNER_RING].maxRotation * i;
                                            var position = center + diff.RotatedBy(rot);
                                            Main.PlaySound(SoundID.Trackable, position, 55 + Main.rand.Next(3));
                                            Projectile.NewProjectile(position, shootDir.RotatedBy(rot), type, damage, 1f, player.whoAmI);
                                        }
                                    }
                                    npc.localAI[0] = 0f;
                                }
                            }
                            innerRingRoll += npc.localAI[2];
                            if (npc.soundDelay <= 0)
                            {
                                npc.soundDelay = 60;
                                Main.PlaySound(SoundID.Trackable, npc.Center, 189 + Main.rand.Next(3));
                            }
                            if (npc.soundDelay > 0)
                                npc.soundDelay--;
                            if (innerRingRoll > MathHelper.PiOver2 * 7f)
                            {
                                npc.soundDelay = 0;
                                Main.PlaySound(SoundID.Trackable, npc.Center, 188);
                                npc.ai[2] = 1200f;
                                innerRingRoll = -MathHelper.PiOver2;
                            }
                        }
                        else
                        {
                            const int width = (int)(CIRCUMFERENCE * 2f);
                            const int height = 900;
                            Vector2 dustPos = center + new Vector2(-width / 2f, 0f);
                            Dust.NewDust(dustPos, width, height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, AQMod.glimmerEvent.stariteProjectileColor, 2f);
                            Dust.NewDust(dustPos, width, height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, AQMod.glimmerEvent.stariteProjectileColor, 2f);
                            Dust.NewDust(dustPos, width, height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, AQMod.glimmerEvent.stariteProjectileColor, 2f);
                        }
                    }
                }
                break;

                case PHASE_STAR_BULLETS:
                {
                    innerRingRotation += 0.0314f;
                    innerRingRoll += 0.0157f;
                    innerRingPitch += 0.01f;

                    outerRingRotation += 0.0157f;
                    outerRingRoll += 0.0314f;
                    outerRingPitch += 0.011f;

                    npc.ai[1]++;

                    if (npc.ai[2] == 0f)
                    {
                        if (Main.expertMode)
                        {
                            npc.ai[2] = 18f;
                            npc.ai[3] = 96f;
                        }
                        else
                        {
                            npc.ai[2] = 7.65f;
                            npc.ai[3] = 192f;
                        }
                    }

                    if (npc.ai[1] % npc.ai[3] == 0f)
                    {
                        if (PlrCheck())
                        {
                            Main.PlaySound(SoundID.Item125);
                            int type = ModContent.ProjectileType<OmegaBullet>();
                            float speed2 = Main.expertMode ? 12.5f : 5.5f;
                            int damage = 30;
                            if (Main.expertMode)
                                damage = 20;
                            for (int i = 0; i < 5; i++)
                            {
                                var v = new Vector2(0f, -1f).RotatedBy(MathHelper.TwoPi / 5f * i);
                                int p = Projectile.NewProjectile(center + v * RADIUS, v * speed2, type, damage, 1f, player.whoAmI, -60f, speed2);
                                Main.projectile[p].timeLeft += 120;
                            }
                            speed2 *= 1.2f;
                            for (int i = 0; i < 5; i++)
                            {
                                var v = new Vector2(0f, -1f).RotatedBy(MathHelper.TwoPi / 5f * i);
                                Projectile.NewProjectile(center + v * RADIUS, v * speed2, type, damage, 1f, player.whoAmI, -60f, speed2);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    float distance = (center - plrCenter).Length();
                    if (distance > CIRCUMFERENCE * 3.75f)
                        npc.velocity = Vector2.Lerp(npc.velocity, Vector2.Normalize(plrCenter - center) * npc.ai[2], 0.02f);
                    else if (distance < CIRCUMFERENCE * 2.25f)
                    {
                        npc.velocity = Vector2.Lerp(npc.velocity, Vector2.Normalize(center - plrCenter) * npc.ai[2], 0.02f);
                    }
                    else
                    {
                        npc.velocity = Vector2.Lerp(npc.velocity, Vector2.Normalize(plrCenter - center).RotatedBy(MathHelper.PiOver2) * npc.ai[2], 0.02f);
                    }

                    if (npc.ai[1] > 480f)
                    {
                        npc.ai[0] = PHASE_HYPER_STARITE_PART0;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                    }
                }
                break;

                case PHASE_ASSAULT_PLAYER:
                {
                    innerRingRotation += 0.0314f;
                    innerRingRoll += 0.0157f;
                    innerRingPitch += 0.01f;

                    outerRingRotation += 0.0157f;
                    outerRingRoll += 0.0314f;
                    outerRingPitch += 0.011f;

                    if (npc.ai[1] < 0f)
                    {
                        npc.ai[1]++;
                        if (npc.ai[2] == 0f)
                        {
                            if (PlrCheck())
                                npc.ai[2] = Main.expertMode ? 18f : 6f;
                            else
                            {
                                break;
                            }
                        }
                        npc.velocity = Vector2.Lerp(npc.velocity, Vector2.Normalize(plrCenter - center) * npc.ai[2], 0.02f);
                    }
                    else
                    {
                        if (!PlrCheck())
                            break;
                        if (npc.ai[1] == 0f)
                        {
                            Main.PlaySound(SoundID.Trackable, npc.Center, 40 + Main.rand.Next(3));
                            npc.ai[1] = plrCenter.X + player.velocity.X * 20f;
                            npc.ai[2] = plrCenter.Y + player.velocity.Y * 20f;
                        }
                        if ((center - new Vector2(npc.ai[1], npc.ai[2])).Length() < CIRCUMFERENCE)
                        {
                            npc.ai[3]++;
                            if (npc.ai[3] > 5)
                            {
                                npc.ai[0] = PHASE_HYPER_STARITE_PART0;
                                npc.ai[1] = 0f;
                                npc.ai[3] = 0f;
                            }
                            else
                            {
                                npc.ai[1] = -npc.ai[3] * 16;
                                if (hardVersion || Vector2.Distance(plrCenter, center) > 400f)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        float lifePercent = npc.life / (float)npc.lifeMax;
                                        if (Main.expertMode && lifePercent < 0.75f || lifePercent < 0.6f)
                                        {
                                            Main.PlaySound(SoundID.Trackable, npc.Center, 55 + Main.rand.Next(3));
                                            int type = ModContent.ProjectileType<OmegaBullet>();
                                            float speed2 = Main.expertMode ? 12.5f : 5.5f;
                                            int damage = 30;
                                            if (Main.expertMode)
                                                damage = 20;
                                            for (int i = 0; i < 5; i++)
                                            {
                                                var v = new Vector2(0f, -1f).RotatedBy(MathHelper.TwoPi / 5f * i);
                                                int p = Projectile.NewProjectile(center + v * RADIUS, v * speed2, type, damage, 1f, player.whoAmI, -60f, speed2);
                                                Main.projectile[p].timeLeft += 120;
                                            }
                                        }
                                    }
                                }
                                npc.netUpdate = true;
                            }
                            npc.ai[2] = 0f;
                        }
                        else
                        {
                            npc.velocity = Vector2.Lerp(npc.velocity, Vector2.Normalize(new Vector2(npc.ai[1], npc.ai[2]) - center) * 20f, 0.025f);
                        }
                    }
                }
                break;

                case PHASE_HYPER_STARITE_PART2_ALT:
                case PHASE_HYPER_STARITE_PART2:
                {
                    npc.ai[2]++;
                    if (npc.ai[2] > 300f)
                    {
                        if (npc.ai[1] > 0.0314)
                            npc.ai[1] -= 0.0005f;
                        else
                        {
                            npc.ai[1] = 0.0314f;
                        }
                        innerRingRotation += npc.ai[1];
                        outerRingRotation += npc.ai[1] * 0.5f;
                        bool innerRing = false;
                        if (orbs[0].radius > orbs[0].defRadius)
                            SetInnerRingRadius(orbs[0].radius - MathHelper.Pi);
                        else
                        {
                            innerRing = true;
                        }
                        bool outerRing = false;
                        if (orbs[OmegaStariteOrb.INNER_RING].radius > orbs[OmegaStariteOrb.INNER_RING].defRadius)
                            SetOuterRingRadius(orbs[OmegaStariteOrb.INNER_RING].radius - MathHelper.PiOver2 * 3f);
                        else
                        {
                            outerRing = true;
                        }
                        if (innerRing && outerRing)
                        {
                            SetInnerRingRadius(orbs[0].defRadius);
                            SetOuterRingRadius(orbs[OmegaStariteOrb.INNER_RING].defRadius);
                            if (PlrCheck())
                            {
                                var choices = new List<int>
                                {
                                    PHASE_ASSAULT_PLAYER,
                                };
                                if (npc.life / (float)npc.lifeMax < (Main.expertMode ? 0.5f : 0.33f))
                                    choices.Add(PHASE_STAR_BULLETS);
                                if (choices.Count == 1)
                                    npc.ai[0] = choices[0];
                                else
                                {
                                    npc.ai[0] = choices[Main.rand.Next(choices.Count)];
                                }
                                npc.ai[1] = 0f;
                                npc.ai[2] = 0f;
                                npc.ai[3] = 0f;
                            }
                        }
                    }
                    else if ((center - plrCenter).Length() > 1800f)
                    {
                        npc.ai[2] = 300f;
                        innerRingRotation += npc.ai[1];
                        outerRingRotation += npc.ai[1] * 0.5f;
                    }
                    else
                    {
                        if (npc.ai[1] >= 0.0628f)
                            npc.ai[1] = 0.0628f;
                        else
                        {
                            npc.ai[1] += 0.0002f;
                        }
                        innerRingRotation += npc.ai[1];
                        outerRingRotation += npc.ai[1] * 0.5f;
                        SetInnerRingRadius(MathHelper.Lerp(orbs[0].radius, orbs[0].defRadius * npc.ai[3], 0.025f));
                        SetOuterRingRadius(MathHelper.Lerp(orbs[OmegaStariteOrb.INNER_RING].radius, orbs[OmegaStariteOrb.INNER_RING].defRadius * (npc.ai[3] + 1f), 0.025f));
                        if (npc.ai[2] > 100f)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (hardVersion || Vector2.Distance(plrCenter, center) > orbs[OmegaStariteOrb.INNER_RING].radius)
                                {
                                    npc.localAI[0]++;
                                    if (npc.localAI[0] > (Main.expertMode ? 3f : 12f))
                                    {
                                        float lifePercent = npc.life / (float)npc.lifeMax;
                                        if (lifePercent < 0.75f)
                                        {
                                            Main.PlaySound(SoundID.Trackable, npc.Center, 55 + Main.rand.Next(3));
                                            var diff = new Vector2(orbs[OmegaStariteOrb.INNER_RING].position.X, orbs[OmegaStariteOrb.INNER_RING].position.Y) - npc.Center;
                                            var shootDir = Vector2.Normalize(diff).RotatedBy(MathHelper.PiOver2) * 6f;
                                            int type = ModContent.ProjectileType<OmegaBullet>();
                                            int damage = 30;
                                            if (Main.expertMode)
                                                damage = 20;
                                            for (int i = 0; i < OmegaStariteOrb.OUTER_RING; i++)
                                            {
                                                float rot = orbs[i + OmegaStariteOrb.INNER_RING].maxRotation * i;
                                                Projectile.NewProjectile(center + diff.RotatedBy(rot), shootDir.RotatedBy(rot), type, damage, 1f, player.whoAmI);
                                            }
                                        }
                                        npc.localAI[0] = 0f;
                                    }
                                }
                            }
                        }
                    }
                }
                break;

                case PHASE_HYPER_STARITE_PART1:
                {
                    if (npc.ai[1] == 0f)
                    {
                        innerRingPitch %= MathHelper.Pi;
                        innerRingRoll %= MathHelper.Pi;
                        outerRingPitch %= MathHelper.Pi;
                        outerRingRoll %= MathHelper.Pi;
                    }
                    npc.ai[1] += 0.0002f;
                    const float lerpValue = 0.025f;
                    const float xLerp = 0f;
                    const float yLerp = 0f;
                    innerRingPitch = innerRingPitch.AngleLerp(xLerp, lerpValue);
                    innerRingRoll = innerRingRoll.AngleLerp(yLerp, lerpValue);
                    outerRingPitch = outerRingPitch.AngleLerp(xLerp, lerpValue);
                    outerRingRoll = outerRingRoll.AngleLerp(yLerp, lerpValue);
                    if (npc.ai[1] > 0.0314f)
                    {
                        const float marginOfError = 0.314f;
                        if (innerRingPitch.IsCloseEnoughTo(xLerp, marginOfError) &&
                            outerRingPitch.IsCloseEnoughTo(xLerp, marginOfError) &&
                            innerRingRoll.IsCloseEnoughTo(xLerp, marginOfError) &&
                            outerRingRoll.IsCloseEnoughTo(xLerp, marginOfError))
                        {
                            npc.velocity = Vector2.Normalize(plrCenter - center) * npc.velocity.Length();
                            innerRingPitch = 0f;
                            innerRingRoll = 0f;
                            outerRingPitch = 0f;
                            outerRingRoll = 0f;
                            if (PlrCheck())
                            {
                                npc.ai[0] = Main.rand.NextBool() ? PHASE_HYPER_STARITE_PART2 : PHASE_HYPER_STARITE_PART2_ALT;
                                npc.ai[1] = 0f;
                                npc.ai[3] = 3f + (1f - npc.life / (float)npc.lifeMax) * 1.5f;
                            }
                        }
                    }
                    else
                    {
                        innerRingRotation += 0.0314f - npc.ai[1];
                        outerRingRotation += 0.0157f - npc.ai[1] * 0.5f;
                    }
                }
                break;

                case PHASE_HYPER_STARITE_PART0:
                {
                    innerRingRotation += 0.0314f;
                    innerRingRoll += 0.0157f;
                    innerRingPitch += 0.01f;
                    outerRingRotation += 0.0157f;
                    outerRingRoll += 0.0314f;
                    outerRingPitch += 0.011f;
                    SetInnerRingRadius(MathHelper.Lerp(orbs[0].radius, orbs[0].defRadius * 0.75f, 0.1f));
                    SetOuterRingRadius(MathHelper.Lerp(orbs[OmegaStariteOrb.INNER_RING].radius, orbs[OmegaStariteOrb.INNER_RING].defRadius * 0.75f, 0.1f));
                    if (npc.ai[1] == 0f)
                    {
                        if (PlrCheck())
                        {
                            Main.PlaySound(SoundID.Trackable, npc.Center, 40 + Main.rand.Next(3));
                            npc.ai[1] = plrCenter.X + player.velocity.X * 20f;
                            npc.ai[2] = plrCenter.Y + player.velocity.Y * 20f;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if ((center - new Vector2(npc.ai[1], npc.ai[2])).Length() < CIRCUMFERENCE)
                    {
                        if (npc.velocity.Length() < 2f)
                        {
                            SetInnerRingRadius(orbs[0].defRadius);
                            SetOuterRingRadius(orbs[OmegaStariteOrb.INNER_RING].defRadius);
                            if (PlrCheck())
                            {
                                npc.velocity *= 0.1f;
                                if (npc.life / (float)npc.lifeMax < 0.5f)
                                    npc.ai[0] = PHASE_OMEGA_LASER_PART0;
                                else
                                {
                                    npc.ai[0] = PHASE_HYPER_STARITE_PART1;
                                }
                                npc.ai[1] = 0f;
                                npc.ai[2] = 0f;
                            }
                        }
                        else
                        {
                            SetInnerRingRadius(MathHelper.Lerp(orbs[0].radius, orbs[0].defRadius, 0.1f));
                            SetOuterRingRadius(MathHelper.Lerp(orbs[OmegaStariteOrb.INNER_RING].radius, orbs[OmegaStariteOrb.INNER_RING].defRadius, 0.1f));
                            npc.velocity *= 0.925f;
                        }
                    }
                    else
                    {
                        npc.velocity = Vector2.Lerp(npc.velocity, Vector2.Normalize(new Vector2(npc.ai[1], npc.ai[2]) - center) * 30f, 0.025f);
                    }
                }
                break;

                case PHASE_INIT:
                {
                    var choices = new List<int>
                    {
                        PHASE_ASSAULT_PLAYER,
                        PHASE_HYPER_STARITE_PART0,
                    };
                    npc.ai[0] = choices[Main.rand.Next(choices.Count)];
                    Init();
                }
                break;

                case PHASE_DEAD:
                {
                    npc.ai[1] += 0.5f;
                    if (npc.ai[1] > DEATH_TIME * 1.314f)
                    {
                        npc.life = -33333;
                        npc.HitEffect();
                        npc.checkDead();
                    }
                }
                break;

                case PHASE_NOVA:
                {
                    if (npc.ai[1] == 0f)
                    {
                        int target = npc.target;
                        Init();
                        npc.target = target;
                        npc.ai[2] = plrCenter.Y - CIRCUMFERENCE * 2.5f;
                    }
                    if (center.Y > npc.ai[2])
                    {
                        int[] choices = new int[] { PHASE_HYPER_STARITE_PART0, PHASE_ASSAULT_PLAYER };
                        if (OmegaStariteSceneManager.Scene == 1)
                            OmegaStariteSceneManager.Scene = 2;
                        npc.ai[0] = choices[Main.rand.Next(choices.Length)];
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }
                    else
                    {
                        npc.velocity = Vector2.Lerp(npc.velocity, Vector2.Normalize(new Vector2(center.X, npc.ai[2]) - center) * 36f, 0.025f);
                    }
                }
                break;

                case PHASE_GOODBYE:
                {
                    if (npc.timeLeft > 120)
                        npc.timeLeft = 120;
                    npc.velocity.X *= 0.975f;
                    npc.velocity.Y -= 0.2f;
                    innerRingRotation += 0.0314f;
                    innerRingRoll += 0.0157f;
                    innerRingPitch += 0.01f;
                    outerRingRotation += 0.0157f;
                    outerRingRoll += 0.0314f;
                    outerRingPitch += 0.011f;
                }
                break;
            }
            Spin(center);
            if (npc.ai[0] != -1)
            {
                int chance = 10 - (int)speed;
                if (chance < 2 || Main.rand.NextBool(chance))
                {
                    if (speed < 2f)
                    {
                        var spawnPos = new Vector2(RADIUS, 0f);
                        int d = Dust.NewDust(center + spawnPos.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi)), 2, 2, 15);
                        Main.dust[d].velocity = Vector2.Normalize(spawnPos - center) * speed * 0.25f;
                    }
                    else
                    {
                        var spawnPos = new Vector2(RADIUS, 0f).RotatedBy(npc.velocity.ToRotation() - MathHelper.Pi);
                        int d = Dust.NewDust(npc.Center + spawnPos.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)), 2, 2, 15);
                        Main.dust[d].velocity = -npc.velocity * 0.25f;
                    }
                }
                if (Main.rand.NextBool(30))
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 58);
                    Main.dust[d].velocity.X = Main.rand.NextFloat(-4f, 4f);
                    Main.dust[d].velocity.Y = Main.rand.NextFloat(-4f, 4f);
                }
                if (Main.rand.NextBool(30))
                {
                    int g = Gore.NewGore(npc.position + new Vector2(Main.rand.Next(npc.width - 4), Main.rand.Next(npc.height - 4)), new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f)), 16 + Main.rand.Next(2));
                    Main.gore[g].scale *= 0.6f;
                }
            }
            Lighting.AddLight(npc.Center, new Vector3(1.2f, 1.2f, 2.2f));
            foreach (var orb in orbs)
            {
                Lighting.AddLight(new Vector2(orb.position.X, orb.position.Y), new Vector3(0.4f, 0.4f, 1f));
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (npc.ai[0] != -1)
            {
                npc.frameCounter++;
                if (npc.frameCounter >= 6)
                {
                    npc.frameCounter = 0;
                    npc.frame.Y += frameHeight;
                    if (npc.frame.Y >= frameHeight * Main.npcFrameCount[npc.type])
                        npc.frame.Y = 0;
                }
            }
        }

        public override bool CheckDead()
        {
            if (npc.ai[0] == -1f || skipDeathTimer > 0)
            {
                npc.lifeMax = -33333;
                return true;
            }
            npc.ai[0] = -1f;
            npc.ai[1] = 0f;
            npc.ai[2] = 0f;
            npc.ai[3] = 0f;
            npc.velocity = new Vector2(0f, 0f);
            npc.dontTakeDamage = true;
            npc.life = npc.lifeMax;
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life == -33333)
            {
                float rotationOff = MathHelper.TwoPi / 80;
                var center = npc.Center;
                for (int i = 0; i < 80; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 15);
                    Main.dust[d].velocity = new Vector2(20f, 0f).RotatedBy(rotationOff * i);
                }
                for (int i = 0; i < 150; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 55);
                    Main.dust[d].velocity.X += Main.rand.NextFloat(-2, 2);
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(4f, 12f);
                }
                for (int i = 0; i < 100; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 57 + Main.rand.Next(2));
                    Main.dust[d].velocity.X += Main.rand.NextFloat(-6, 6);
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(4f, 12f);
                }
                for (int i = 0; i < 40; i++)
                {
                    Gore.NewGore(center, new Vector2(Main.rand.NextFloat(-12f, 12f), Main.rand.NextFloat(-12f, 12f)), 16 + Main.rand.Next(2));
                }
                rotationOff = MathHelper.TwoPi / 10;
                for (int i = 0; i < orbs.Count; i++)
                {
                    var pos = orbs[i].position;
                    var v = new Vector2(pos.X - OmegaStariteOrb.SIZE_HALF + center.X, pos.Y - OmegaStariteOrb.SIZE_HALF + center.Y);
                    for (int j = 0; j < 10; j++)
                    {
                        int d = Dust.NewDust(npc.position, npc.width, npc.height, 15);
                        Main.dust[d].velocity = new Vector2(20f, 0f).RotatedBy(rotationOff * j);
                    }
                    for (int j = 0; j < 10; j++)
                    {
                        int d = Dust.NewDust(v, OmegaStariteOrb.SIZE, OmegaStariteOrb.SIZE, 55);
                        Main.dust[d].velocity.X += Main.rand.NextFloat(-2, 2);
                        Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                    }
                    for (int j = 0; j < 30; j++)
                    {
                        int d = Dust.NewDust(v, OmegaStariteOrb.SIZE, OmegaStariteOrb.SIZE, 57 + Main.rand.Next(2));
                        Main.dust[d].velocity.X += Main.rand.NextFloat(-2, 2);
                        Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                    }
                    for (int j = 0; j < 10; j++)
                    {
                        Gore.NewGore(new Vector2(pos.X, pos.Y), new Vector2(Main.rand.NextFloat(-9f, 9f), Main.rand.NextFloat(-9f, 9f)), 16 + Main.rand.Next(2));
                    }
                }
            }
            else if (npc.life <= 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 55);
                    Main.dust[d].velocity.X += Main.rand.NextFloat(-2, 2);
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                }
                for (int i = 0; i < 30; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 57 + Main.rand.Next(2));
                    Main.dust[d].velocity.X += Main.rand.NextFloat(-2, 2);
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                }
                for (int i = 0; i < 10; i++)
                {
                    Gore.NewGore(npc.Center, new Vector2(Main.rand.NextFloat(-9f, 9f), Main.rand.NextFloat(-9f, 9f)), 16 + Main.rand.Next(2));
                }
            }
            else
            {
                float x = npc.velocity.X.Abs() * hitDirection;
                for (int i = 0; i < 3; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 55);
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(5f, 12f);
                }
                if (Main.rand.NextBool())
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 57 + Main.rand.Next(2));
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                }
                if (Main.rand.NextBool())
                    Gore.NewGore(npc.Center, new Vector2(Main.rand.NextFloat(-4f, 4f) + x * 0.75f, Main.rand.NextFloat(-4f, 4f)), 16 + Main.rand.Next(2));
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool())
                target.AddBuff(BuffID.OnFire, 120);
            if (Main.expertMode)
            {
                if (Main.rand.NextBool())
                    target.AddBuff(BuffID.Blackout, 120);
                else if (Main.rand.NextBool())
                {
                    target.AddBuff(BuffID.Darkness, 360);
                }
            }
            else if (Main.rand.NextBool())
            {
                target.AddBuff(BuffID.Darkness, 120);
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return npc.ai[0] != -1;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.type == ProjectileID.FallingStar)
            {
                if (damage > 300)
                {
                    AQMod.glimmerEvent.StariteDisco = true;
                    Vector2 velo = projectile.velocity * -1.2f;
                    for (int i = 0; i < 8; i++)
                    {
                        int p2 = Projectile.NewProjectile(projectile.Center, velo.RotatedBy(MathHelper.PiOver4 * i), ModContent.ProjectileType<RainbowStarofHyperApocalypse>(), damage, knockback);
                        Main.projectile[p2].timeLeft = 240;
                    }
                    damage = (int)(damage * 0.25);
                    projectile.active = false;
                    return;
                }
                int p = Projectile.NewProjectile(projectile.Center, projectile.velocity * -1.2f, ModContent.ProjectileType<RainbowStarofHyperApocalypse>(), damage, knockback);
                Main.projectile[p].timeLeft = 240;
                damage = (int)(damage * 0.25);
                projectile.active = false;
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }

        public override bool PreNPCLoot()
        {
            OmegaStariteSceneManager.OmegaStariteIndexCache = -1;
            OmegaStariteSceneManager.Scene = 3;
            return true;
        }

        public override void NPCLoot()
        {
            AQMod.glimmerEvent.deactivationTimer = 275;
            if (Main.rand.NextBool(7))
                Item.NewItem(npc.getRect(), ModContent.ItemType<OmegaStariteTrophy>());
            if (Main.expertMode)
            {
                npc.DropBossBags();
                if (Main.rand.NextBool(4))
                    Item.NewItem(npc.getRect(), ModContent.ItemType<DragonBall>());
            }
            else
            {
                var rect = npc.getRect();
                if (Main.rand.NextBool())
                    Item.NewItem(rect, ModContent.ItemType<CosmicTelescope>());
                int[] choices = new int[]
                {
                    ModContent.ItemType<MagicWand>(),
                    ModContent.ItemType<Galactium>(),
                    ModContent.ItemType<Raygun>(),
                };
                Item.NewItem(rect, choices[Main.rand.Next(choices.Length)]);
                Item.NewItem(rect, ModContent.ItemType<CosmicEnergy>(), Main.rand.NextVRand(2, 5));
                Item.NewItem(rect, ItemID.FallenStar, Main.rand.NextVRand(15, 20));
                Item.NewItem(rect, ItemID.SoulofFlight, Main.rand.NextVRand(2, 5));
            }
            WorldDefeats.DownedStarite = true;
            WorldDefeats.DownedGlimmer = true;
            if (AQMod.glimmerEvent.IsActive)
            {
                switch (Main.rand.Next(3))
                {
                    default:
                    {
                        npc.DropItemInstanced(npc.position, new Vector2(npc.width, npc.height), ModContent.ItemType<EnchantedDye>());
                    }
                    break;

                    case 1:
                    {
                        npc.DropItemInstanced(npc.position, new Vector2(npc.width, npc.height), ModContent.ItemType<RainbowOutlineDye>());
                    }
                    break;

                    case 2:
                    {
                        npc.DropItemInstanced(npc.position, new Vector2(npc.width, npc.height), ModContent.ItemType<DiscoDye>());
                    }
                    break;
                }

                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    var plr = Main.player[i];
                    if (plr.active && npc.playerInteraction[i])
                        Projectile.NewProjectile(npc.Center, new Vector2(Main.rand.NextFloat(2f, 6f) * (Main.rand.NextBool() ? -1f : 1f), -18f), ModContent.ProjectileType<UltimateSwordItemProjectile>(), 0, 0f, i, ModContent.ItemType<UltimateSword>());
                }
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (orbs == null)
            {
                return false;
            }
            drawColor *= 5f;
            if (drawColor.R < 80)
                drawColor.R = 80;
            if (drawColor.G < 80)
                drawColor.G = 80;
            if (drawColor.B < 80)
                drawColor.B = 80;
            var drawPos = npc.Center - Main.screenPosition;
            var sortedOmegites = new List<OmegaStariteOrb>(orbs);
            float intensity = AQMod.EffectIntensity;
            if ((int)npc.ai[0] == -1)
            {
                intensity += npc.ai[1] / 20;
                int range = (int)intensity + 4;
                drawPos += new Vector2(Main.rand.Next(-range, range), Main.rand.Next(-range, range));
                for (int i = 0; i < orbs.Count; i++)
                {
                    sortedOmegites[i].drawOffset = new Vector3(Main.rand.Next(-range, range), Main.rand.Next(-range, range), Main.rand.Next(-range, range));
                }
                ScreenShakeManager.ChannelEffect("OmegaStariteDeathScreenShake", new OmegaStariteScreenShake((int)(range * 0.8f), 0.01f, Math.Max(6 - (int)(range * 0.8), 1)));
            }
            sortedOmegites.Sort((o, o2) => -(o.position.Z + o.drawOffset.Z).CompareTo(o2.position.Z + o2.drawOffset.Z));
            var omegiteTexture = TextureCache.OmegaStariteOrb.GetValue();
            var omegiteFrame = new Rectangle(0, 0, omegiteTexture.Width, omegiteTexture.Height);
            var omegiteOrigin = omegiteFrame.Size() / 2f;
            float xOff = (float)(Math.Sin(Main.GlobalTime * 3f) + 1f);
            var clr3 = new Color(50, 50, 50, 0) * (intensity - ModContent.GetInstance<AQConfigClient>().EffectIntensity + 1f);
            float deathSpotlightScale = 0f;
            if (intensity > 3f)
                deathSpotlightScale = npc.scale * (intensity - 2.1f) * ((float)Math.Sin(npc.ai[1] * 0.1f) + 1f) / 2f;
            var spotlight = TextureCache.Lights[LightID.Spotlight66x66];
            var spotlightOrig = spotlight.Size() / 2f;
            Color spotlightColor;
            if (AQMod.glimmerEvent.StariteDisco)
            {
                spotlightColor = Main.DiscoColor;
                spotlightColor.A = 0;
            }
            else
            {
                spotlightColor = AQMod.StariteAuraColor;
            }
            var drawOmegite = new List<DrawMethod>();
            if (ModContent.GetInstance<AQConfigClient>().EffectQuality >= 1f)
            {
                drawOmegite.Add(delegate (Texture2D texture1, Vector2 position, Rectangle? frame1, Color color, float scale, Vector2 origin1, float rotation, SpriteEffects effects, float layerDepth)
                {
                    spriteBatch.Draw(spotlight, position, null, spotlightColor, rotation, spotlightOrig, scale * 1.33f, SpriteEffects.None, 0f);
                });
            }
            drawOmegite.Add(delegate (Texture2D texture1, Vector2 position, Rectangle? frame1, Color color, float scale, Vector2 origin1, float rotation, SpriteEffects effects, float layerDepth)
            {
                spriteBatch.Draw(omegiteTexture, position, omegiteFrame, drawColor, rotation, origin1, scale, SpriteEffects.None, 0f);
            });
            if (intensity >= 1f)
            {
                drawOmegite.Add(delegate (Texture2D texture1, Vector2 position, Rectangle? frame1, Color color, float scale, Vector2 origin1, float rotation, SpriteEffects effects, float layerDepth)
                {
                    for (int j = 0; j < intensity; j++)
                    {
                        spriteBatch.Draw(omegiteTexture, position + new Vector2(2f + xOff * 2f * j, 0f), omegiteFrame, clr3, rotation, origin1, scale, SpriteEffects.None, 0f);
                        spriteBatch.Draw(omegiteTexture, position + new Vector2(2f - xOff * 2f * j, 0f), omegiteFrame, clr3, rotation, origin1, scale, SpriteEffects.None, 0f);
                    }
                });
            }
            if (intensity > 3f)
            {
                float omegiteDeathDrawScale = deathSpotlightScale * 0.5f;
                drawOmegite.Add(delegate (Texture2D texture1, Vector2 position, Rectangle? frame1, Color color, float scale, Vector2 origin1, float rotation, SpriteEffects effects, float layerDepth)
                {
                    spriteBatch.Draw(spotlight, position, null, drawColor, rotation, spotlightOrig, scale * omegiteDeathDrawScale, SpriteEffects.None, 0f);
                    spriteBatch.Draw(spotlight, position, null, spotlightColor, rotation, spotlightOrig, scale * omegiteDeathDrawScale * 2, SpriteEffects.None, 0f);
                });
            }
            for (int i = 0; i < sortedOmegites.Count; i++)
            {
                if (sortedOmegites[i].position.Z + sortedOmegites[i].drawOffset.Z > 0f)
                {
                    var drawPosition = ThreeDimensionsEffect.GetParralaxPosition(new Vector2(sortedOmegites[i].position.X + sortedOmegites[i].drawOffset.X, sortedOmegites[i].position.Y + sortedOmegites[i].drawOffset.Y), sortedOmegites[i].position.Z * 0.00728f) - Main.screenPosition;
                    var drawScale = ThreeDimensionsEffect.GetParralaxScale(sortedOmegites[i].scale, (sortedOmegites[i].position.Z + sortedOmegites[i].drawOffset.Z) * 0.0314f);
                    foreach (var draw in drawOmegite)
                    {
                        draw.Invoke(
                            omegiteTexture,
                            drawPosition,
                            omegiteFrame,
                            drawColor,
                            drawScale,
                            omegiteOrigin,
                            npc.rotation,
                            SpriteEffects.None,
                            0f);
                    }
                    sortedOmegites.RemoveAt(i);
                    i--;
                }
            }
            Texture2D texture = Main.npcTexture[npc.type];
            var offset = new Vector2(npc.width / 2f, npc.height / 2f);
            Vector2 origin = npc.frame.Size() / 2f;
            float mult = 1f / NPCID.Sets.TrailCacheLength[npc.type];
            var clr = drawColor * 0.25f;
            for (int i = 0; i < intensity; i++)
            {
                spriteBatch.Draw(spotlight, drawPos, null, spotlightColor, npc.rotation, spotlightOrig, npc.scale * 2.5f + i, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(spotlight, drawPos, null, spotlightColor * (1f - (intensity - (int)intensity)), npc.rotation, spotlightOrig, npc.scale * 2.5f + ((int)intensity + 1), SpriteEffects.None, 0f);
            if ((npc.position - npc.oldPos[1]).Length() > 0.01f)
            {
                if (Trailshader.ShouldDrawVertexTrails())
                {
                    var trueOldPos = new List<Vector2>();
                    for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
                    {
                        if (npc.oldPos[i] == new Vector2(0f, 0f))
                            break;
                        trueOldPos.Add(ScreenShakeManager.UpsideDownScreenSupport(npc.oldPos[i] + offset - Main.screenPosition));
                    }
                    if (trueOldPos.Count > 1)
                    {
                        const float radius = CIRCUMFERENCE / 2f;
                        var trailClr = AQMod.glimmerEvent.StariteDisco ? Main.DiscoColor : new Color(35, 85, 255, 120);
                        var trail = new Trailshader(TextureCache.Trails[TrailTextureID.Line], Trailshader.TextureTrail);
                        trail.PrepareVertices(trueOldPos.ToArray(), (p) => new Vector2(radius - p * radius), (p) => trailClr * (1f - p));
                        trail.Draw();
                    }
                }
                else
                {
                    int trailLength = NPCID.Sets.TrailCacheLength[npc.type];
                    for (int i = 0; i < trailLength; i++)
                    {
                        if (npc.oldPos[i] == new Vector2(0f, 0f))
                            break;
                        float progress = 1f - 1f / trailLength * i;
                        var trailClr = AQMod.glimmerEvent.StariteDisco ? Main.DiscoColor : new Color(35, 85, 255, 120);
                        trailClr.A = 0;
                        Main.spriteBatch.Draw(texture, npc.oldPos[i] + offset - Main.screenPosition, npc.frame, trailClr * 0.4f * progress, npc.rotation, origin, npc.scale, SpriteEffects.None, 0f);
                    }
                }
            }
            else
            {
                npc.oldPos[0] = new Vector2(0f, 0f);
            }
            spriteBatch.Draw(texture, drawPos, npc.frame, drawColor, npc.rotation, origin, npc.scale, SpriteEffects.None, 0f);
            for (int j = 0; j < intensity; j++)
            {
                spriteBatch.Draw(texture, drawPos + new Vector2(2f + xOff * 2f * j, 0f), npc.frame, clr3, npc.rotation, origin, npc.scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(texture, drawPos - new Vector2(2f + xOff * 2f * j, 0f), npc.frame, clr3, npc.rotation, origin, npc.scale, SpriteEffects.None, 0f);
            }
            for (int i = 0; i < sortedOmegites.Count; i++)
            {
                var drawPosition = ThreeDimensionsEffect.GetParralaxPosition(new Vector2(sortedOmegites[i].position.X + sortedOmegites[i].drawOffset.X, sortedOmegites[i].position.Y + sortedOmegites[i].drawOffset.Y), sortedOmegites[i].position.Z * 0.00728f) - Main.screenPosition;
                var drawScale = ThreeDimensionsEffect.GetParralaxScale(sortedOmegites[i].scale, (sortedOmegites[i].position.Z + sortedOmegites[i].drawOffset.Z) * 0.0314f);
                foreach (var draw in drawOmegite)
                {
                    draw.Invoke(
                        omegiteTexture,
                        drawPosition,
                        omegiteFrame,
                        drawColor,
                        drawScale,
                        omegiteOrigin,
                        npc.rotation,
                        SpriteEffects.None,
                        0f);
                }
            }
            if (intensity > 3f)
            {
                float intensity2 = intensity - 2f;
                if (npc.ai[1] > DEATH_TIME)
                {
                    float scale = (npc.ai[1] - DEATH_TIME) * 0.2f * ModContent.GetInstance<AQConfigClient>().EffectIntensity;
                    scale *= scale;
                    Main.spriteBatch.Draw(spotlight, drawPos, null, new Color(120, 120, 120, 0) * intensity2, npc.rotation, spotlightOrig, scale, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightColor * intensity2, npc.rotation, spotlightOrig, scale * 2.15f, SpriteEffects.None, 0f);
                }
                else
                {
                    Main.spriteBatch.Draw(spotlight, drawPos, null, new Color(120, 120, 120, 0) * intensity2, npc.rotation, spotlightOrig, deathSpotlightScale, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightColor * intensity2, npc.rotation, spotlightOrig, deathSpotlightScale * 2f, SpriteEffects.None, 0f);
                }
            }
            return false;
        }

        public ModifiableMusic GetMusic() => AQMod.OmegaStariteMusic;
    }
}