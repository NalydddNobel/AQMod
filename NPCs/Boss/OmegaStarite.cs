using AQMod.Assets;
using AQMod.Assets.Effects.Trails;
using AQMod.Buffs.Debuffs;
using AQMod.Common;
using AQMod.Content.World.Events;
using AQMod.Dusts;
using AQMod.Items.BossItems.Starite;
using AQMod.Projectiles.Monster.OmegaStarite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Boss
{
    [AutoloadBossHead()]
    public class OmegaStarite : ModNPC
    {
        public const float CIRCUMFERENCE = 120;
        public const float RADIUS = CIRCUMFERENCE / 2f;
        private const float DEATH_TIME = MathHelper.PiOver4 * 134;

        public static class Scene
        {
            public static class ID
            {
                public const byte NoScene = 0;
            }

            public static int IndexCache { get; internal set; } = -1;
            public static byte CurrentScene { get; internal set; } = ID.NoScene;
            public static bool ShouldShowUltimateSword => true;
        }

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
            NPCID.Sets.TrailingMode[NPC.type] = 7;
            NPCID.Sets.TrailCacheLength[NPC.type] = 60;
            NPCID.Sets.DebuffImmunitySets[NPC.type] = new NPCDebuffImmunityData() { ImmuneToAllBuffsThatAreNotWhips = true, };
            Main.npcFrameCount[NPC.type] = 14;
        }

        public override void SetDefaults()
        {
            NPC.width = 120;
            NPC.height = 120;
            NPC.lifeMax = 9500;
            NPC.damage = 50;
            NPC.defense = 18;
            NPC.HitSound = SoundID.NPCDeath57;
            NPC.DeathSound = SoundID.NPCDeath55;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(gold: 18);
            NPC.boss = true;
            NPC.npcSlots = 10f;
            NPC.noTileCollide = true;
            NPC.trapImmune = true;
            NPC.lavaImmune = true;
            BossBag = ModContent.ItemType<StariteBag>();

            if (!GlimmerEvent.IsActive)
                skipDeathTimer = 600;
            //if (AQGraphics.AllowedToUseAssets)
            //{
            //    Music = GetMusic().GetMusicID();
            //    MusicPriority = MusicPriority.BossMedium;
            //    if (AprilFoolsJoke.Active)
            //    {
            //        NPC.GivenName = "Omega Starite, Living Galaxy the Omega Being";
            //    }
            //}
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return new Color(255, 255, 255, 240);
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f) + 4000 * numPlayers;
        }

        public int GetOmegiteDamage()
        {
            return Main.expertMode ? 25 : 30;
        }

        public bool IsOmegaLaserActive()
        {
            return NPC.ai[0] == PHASE_OMEGA_LASER && NPC.ai[2] < 1200f;
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

        public void Init()
        {
            NPC.TargetClosest(faceTarget: false);
            Scene.IndexCache = (short)NPC.whoAmI;
            orbs = new List<OmegaStariteOrb>();
            var center = NPC.Center;
            if (Main.expertMode)
            {
                spawnRing(center, OmegaStariteOrb.INNER_RING, CIRCUMFERENCE, 1f);
                spawnRing(center, OmegaStariteOrb.OUTER_RING, CIRCUMFERENCE * 1.75f, 1.25f);
            }
            else
            {
                spawnRing(center, OmegaStariteOrb.INNER_RING, CIRCUMFERENCE * 0.75f, OmegaStariteOrb.INNER_RING_SCALE);
                spawnRing(center, OmegaStariteOrb.OUTER_RING, CIRCUMFERENCE * OmegaStariteOrb.OUTER_RING_CIRCUMFERENCE_MULT, OmegaStariteOrb.OUTER_RING_SCALE);
            }
            int damage = GetOmegiteDamage();
            Projectile.NewProjectile(NPC.GetProjectileSpawnSource(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Monster.OmegaStarite.OmegaStarite>(), damage, 1f, Main.myPlayer, NPC.whoAmI);
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
                orbs[i].position = Vector3.Transform(new Vector3(orbs[i].radius, 0f, 0f),
                    Matrix.CreateFromYawPitchRoll(outerRingPitch, outerRingRoll, orbs[i].defRotation + outerRingRotation)) + new Vector3(center, 0f);
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
            if (GlimmerEvent.ShouldKillStar(NPC))
            {
                NPC.life = -1;
                Scene.CurrentScene = Scene.ID.NoScene;
                Scene.IndexCache = -1;
                NPC.HitEffect();
                SoundEngine.PlaySound(SoundID.Dig, NPC.Center);
                NPC.active = false;
                return;
            }
            if (skipDeathTimer > 0)
                skipDeathTimer--;
            Vector2 center = NPC.Center;
            Player player = Main.player[NPC.target];
            var plrCenter = player.Center;
            float speed = NPC.velocity.Length();
            switch ((int)NPC.ai[0])
            {
                default:
                    {
                        innerRingRotation += 0.0314f;
                        innerRingRoll += 0.0157f;
                        innerRingPitch += 0.01f;

                        outerRingRotation += 0.0157f;
                        outerRingRoll += 0.0314f;
                        outerRingPitch += 0.011f;
                        NPC.Center = plrCenter + new Vector2(0f, -CIRCUMFERENCE * 2f);
                    }
                    break;

                case PHASE_OMEGA_LASER_PART0:
                    {
                        if (NPC.ai[1] == 0f)
                        {
                            innerRingPitch %= MathHelper.Pi;
                            innerRingRoll %= MathHelper.Pi;
                            outerRingPitch %= MathHelper.Pi;
                            outerRingRoll %= MathHelper.Pi;
                        }
                        NPC.ai[1] += 0.0002f;
                        const float lerpValue = 0.025f;
                        const float xLerp = 0f;
                        const float yLerp = 0f;
                        innerRingPitch = innerRingPitch.AngleLerp(MathHelper.PiOver2, lerpValue);
                        innerRingRoll = innerRingRoll.AngleLerp(-MathHelper.PiOver2, lerpValue);
                        outerRingPitch = outerRingPitch.AngleLerp(xLerp, lerpValue);
                        outerRingRoll = outerRingRoll.AngleLerp(yLerp, lerpValue);
                        if (NPC.ai[1] > 0.0314f)
                        {
                            const float marginOfError = 0.314f;
                            if (innerRingPitch.IsCloseEnoughTo(MathHelper.PiOver2, marginOfError) &&
                                innerRingRoll.IsCloseEnoughTo(-MathHelper.PiOver2, marginOfError) &&
                                outerRingPitch.IsCloseEnoughTo(yLerp, marginOfError) &&
                                outerRingRoll.IsCloseEnoughTo(yLerp, marginOfError))
                            {
                                NPC.velocity = Vector2.Normalize(plrCenter - center) * NPC.velocity.Length();
                                innerRingPitch = MathHelper.PiOver2;
                                innerRingRoll = -MathHelper.PiOver2;
                                outerRingPitch = xLerp;
                                outerRingRoll = yLerp;
                                if (PlrCheck())
                                {
                                    NPC.ai[0] = PHASE_OMEGA_LASER;
                                    NPC.ai[1] = 0f;
                                    NPC.ai[3] = 3f + (1f - NPC.life / (float)NPC.lifeMax) * 1.5f;
                                }
                            }
                        }
                        else
                        {
                            innerRingRotation += 0.0314f - NPC.ai[1];
                            outerRingRotation += 0.0157f - NPC.ai[1] * 0.5f;
                        }
                    }
                    break;

                case PHASE_OMEGA_LASER:
                    {
                        NPC.ai[2]++;
                        if (NPC.ai[2] > 1200f)
                        {
                            if (NPC.ai[1] > 0.0314)
                            {
                                NPC.ai[1] -= 0.0005f;
                            }
                            else
                            {
                                NPC.ai[1] = 0.0314f;
                            }
                            innerRingRotation += NPC.ai[1];
                            outerRingRotation += NPC.ai[1] * 0.5f;
                            bool outerRing = false;
                            if (orbs[OmegaStariteOrb.INNER_RING].radius > orbs[OmegaStariteOrb.INNER_RING].defRadius)
                            {
                                SetOuterRingRadius(orbs[OmegaStariteOrb.INNER_RING].radius - MathHelper.PiOver2 * 3f);
                                if (Vector2.Distance(plrCenter, center) > orbs[OmegaStariteOrb.INNER_RING].radius)
                                {
                                    NPC.localAI[0]++;
                                    if (NPC.localAI[0] > (Main.expertMode ? 12f : 60f))
                                    {
                                        float lifePercent = NPC.life / (float)NPC.lifeMax;
                                        if (lifePercent < 0.75f)
                                        {
                                            SoundEngine.PlaySound(SoundID.DD2_DarkMageCastHeal, NPC.Center);
                                            var diff = new Vector2(orbs[OmegaStariteOrb.INNER_RING].position.X, orbs[OmegaStariteOrb.INNER_RING].position.Y) - NPC.Center;
                                            var shootDir = Vector2.Normalize(diff).RotatedBy(MathHelper.PiOver2) * 7.5f;
                                            int type = ModContent.ProjectileType<OmegaBullet>();
                                            int damage = 25;
                                            if (Main.expertMode)
                                                damage = 18;
                                            for (int i = 0; i < OmegaStariteOrb.OUTER_RING; i++)
                                            {
                                                float rot = orbs[i + OmegaStariteOrb.INNER_RING].maxRotation * i;
                                                var position = center + diff.RotatedBy(rot);
                                                SoundEngine.PlaySound(SoundID.DD2_DarkMageCastHeal, position);
                                                Projectile.NewProjectile(NPC.GetProjectileSpawnSource(), position, shootDir.RotatedBy(rot), type, damage, 1f, player.whoAmI);
                                            }
                                        }
                                        NPC.localAI[0] = 0f;
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
                                    NPC.ai[0] = choices[Main.rand.Next(choices.Count)];
                                    NPC.ai[1] = 0f;
                                    NPC.ai[2] = 0f;
                                    NPC.ai[3] = 0f;
                                    NPC.localAI[0] = 0f;
                                    NPC.localAI[1] = 0f;
                                    NPC.localAI[2] = 0f;
                                }
                            }
                        }
                        else if ((center - plrCenter).Length() > 1800f)
                        {
                            NPC.ai[2] = 300f;
                            innerRingRotation += NPC.ai[1];
                            outerRingRotation += NPC.ai[1] * 0.5f;
                        }
                        else
                        {
                            if (NPC.ai[1] >= 0.0628f)
                            {
                                NPC.ai[1] = 0.0628f;
                            }
                            else
                            {
                                NPC.ai[1] += 0.0002f;
                            }
                            innerRingRotation += NPC.ai[1];
                            outerRingRotation += NPC.ai[1] * 0.5f;
                            SetOuterRingRadius(MathHelper.Lerp(orbs[OmegaStariteOrb.INNER_RING].radius, orbs[OmegaStariteOrb.INNER_RING].defRadius * (NPC.ai[3] + 1f), 0.025f));
                            if (NPC.ai[2] > 100f)
                            {
                                if (NPC.localAI[1] == 0f)
                                {
                                    if (PlrCheck())
                                    {
                                        NPC.localAI[1] = 1f;
                                        SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, NPC.Center);
                                        //if (Main.netMode != NetmodeID.Server)
                                        //    ScreenShakeManager.AddShake(new OmegaStariteScreenShake(AQMod.MultIntensity(8), 0.02f * AQConfigClient.c_EffectIntensity));
                                        int p = Projectile.NewProjectile(NPC.GetProjectileSpawnSource(), center, new Vector2(0f, 0f), ModContent.ProjectileType<OmegaRay>(), 100, 1f, Main.myPlayer, NPC.whoAmI);
                                        Main.projectile[p].scale = 0.75f;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                if (innerRingRoll > MathHelper.PiOver2 * 6f)
                                {
                                    NPC.localAI[2] -= Main.expertMode ? 0.001f : 0.00045f;
                                }
                                else
                                {
                                    NPC.localAI[2] += Main.expertMode ? 0.00015f : 0.000085f;
                                }
                                if (Main.netMode != NetmodeID.MultiplayerClient && Vector2.Distance(plrCenter, center) > orbs[OmegaStariteOrb.INNER_RING].radius)
                                {
                                    NPC.localAI[0]++;
                                    if (NPC.localAI[0] > (Main.expertMode ? 3f : 12f))
                                    {
                                        float lifePercent = NPC.life / (float)NPC.lifeMax;
                                        if (lifePercent < 0.75f)
                                        {
                                            var diff = new Vector2(orbs[OmegaStariteOrb.INNER_RING].position.X, orbs[OmegaStariteOrb.INNER_RING].position.Y) - NPC.Center;
                                            var shootDir = Vector2.Normalize(diff).RotatedBy(MathHelper.PiOver2) * 12f;
                                            int type = ModContent.ProjectileType<OmegaBullet>();
                                            int damage = 30;
                                            if (Main.expertMode)
                                                damage = 20;
                                            for (int i = 0; i < OmegaStariteOrb.OUTER_RING; i++)
                                            {
                                                float rot = orbs[i + OmegaStariteOrb.INNER_RING].maxRotation * i;
                                                var position = center + diff.RotatedBy(rot);
                                                SoundEngine.PlaySound(SoundID.DD2_DarkMageCastHeal, position);
                                                Projectile.NewProjectile(NPC.GetProjectileSpawnSource(), position, shootDir.RotatedBy(rot), type, damage, 1f, player.whoAmI);
                                            }
                                        }
                                        NPC.localAI[0] = 0f;
                                    }
                                }
                                innerRingRoll += NPC.localAI[2];
                                if (NPC.soundDelay <= 0)
                                {
                                    NPC.soundDelay = 60;
                                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, NPC.Center);
                                }
                                if (NPC.soundDelay > 0)
                                    NPC.soundDelay--;
                                if (innerRingRoll > MathHelper.PiOver2 * 7f)
                                {
                                    NPC.soundDelay = 0;
                                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, NPC.Center);
                                    NPC.ai[2] = 1200f;
                                    innerRingRoll = -MathHelper.PiOver2;
                                }
                            }
                            else
                            {
                                const int width = (int)(CIRCUMFERENCE * 2f);
                                const int height = 900;
                                Vector2 dustPos = center + new Vector2(-width / 2f, 0f);
                                var options = ClientOptions.Instance;
                                Dust.NewDust(dustPos, width, height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, options.StariteProjectileColoring, 2f);
                                Dust.NewDust(dustPos, width, height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, options.StariteProjectileColoring, 2f);
                                Dust.NewDust(dustPos, width, height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, options.StariteProjectileColoring, 2f);
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

                        NPC.ai[1]++;

                        if (NPC.ai[2] == 0f)
                        {
                            if (Main.expertMode)
                            {
                                NPC.ai[2] = 18f;
                                NPC.ai[3] = 96f;
                            }
                            else
                            {
                                NPC.ai[2] = 7.65f;
                                NPC.ai[3] = 192f;
                            }
                        }

                        if (NPC.ai[1] % NPC.ai[3] == 0f)
                        {
                            if (PlrCheck())
                            {
                                SoundEngine.PlaySound(SoundID.Item125);
                                int type = ModContent.ProjectileType<OmegaBullet>();
                                float speed2 = Main.expertMode ? 12.5f : 5.5f;
                                int damage = 30;
                                if (Main.expertMode)
                                    damage = 20;
                                for (int i = 0; i < 5; i++)
                                {
                                    var v = new Vector2(0f, -1f).RotatedBy(MathHelper.TwoPi / 5f * i);
                                    int p = Projectile.NewProjectile(NPC.GetProjectileSpawnSource(), center + v * RADIUS, v * speed2, type, damage, 1f, player.whoAmI, -60f, speed2);
                                    Main.projectile[p].timeLeft += 120;
                                }
                                speed2 *= 1.2f;
                                for (int i = 0; i < 5; i++)
                                {
                                    var v = new Vector2(0f, -1f).RotatedBy(MathHelper.TwoPi / 5f * i);
                                    Projectile.NewProjectile(NPC.GetProjectileSpawnSource(), center + v * RADIUS, v * speed2, type, damage, 1f, player.whoAmI, -60f, speed2);
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        float distance = (center - plrCenter).Length();
                        if (distance > CIRCUMFERENCE * 3.75f)
                        {
                            NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(plrCenter - center) * NPC.ai[2], 0.02f);
                        }
                        else if (distance < CIRCUMFERENCE * 2.25f)
                        {
                            NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(center - plrCenter) * NPC.ai[2], 0.02f);
                        }
                        else
                        {
                            NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(plrCenter - center).RotatedBy(MathHelper.PiOver2) * NPC.ai[2], 0.02f);
                        }

                        if (NPC.ai[1] > 480f)
                        {
                            NPC.ai[0] = PHASE_HYPER_STARITE_PART0;
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                            NPC.ai[3] = 0f;
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

                        if (NPC.ai[1] < 0f)
                        {
                            NPC.ai[1]++;
                            if (NPC.ai[2] == 0f)
                            {
                                if (PlrCheck())
                                {
                                    NPC.ai[2] = Main.expertMode ? 18f : 6f;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(plrCenter - center) * NPC.ai[2], 0.02f);
                        }
                        else
                        {
                            if (!PlrCheck())
                                break;
                            if (NPC.ai[1] == 0f)
                            {
                                SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack, NPC.Center);
                                NPC.ai[1] = plrCenter.X + player.velocity.X * 20f;
                                NPC.ai[2] = plrCenter.Y + player.velocity.Y * 20f;
                            }
                            if ((center - new Vector2(NPC.ai[1], NPC.ai[2])).Length() < CIRCUMFERENCE)
                            {
                                NPC.ai[3]++;
                                if (NPC.ai[3] > 5)
                                {
                                    NPC.ai[0] = PHASE_HYPER_STARITE_PART0;
                                    NPC.ai[1] = 0f;
                                    NPC.ai[3] = 0f;
                                }
                                else
                                {
                                    NPC.ai[1] = -NPC.ai[3] * 16;
                                    if (Vector2.Distance(plrCenter, center) > 400f)
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            float lifePercent = NPC.life / (float)NPC.lifeMax;
                                            if (Main.expertMode && lifePercent < 0.75f || lifePercent < 0.6f)
                                            {
                                                SoundEngine.PlaySound(SoundID.DD2_DarkMageCastHeal, NPC.Center);
                                                int type = ModContent.ProjectileType<OmegaBullet>();
                                                float speed2 = Main.expertMode ? 12.5f : 5.5f;
                                                int damage = 30;
                                                if (Main.expertMode)
                                                    damage = 20;
                                                for (int i = 0; i < 5; i++)
                                                {
                                                    var v = new Vector2(0f, -1f).RotatedBy(MathHelper.TwoPi / 5f * i);
                                                    int p = Projectile.NewProjectile(NPC.GetProjectileSpawnSource(), center + v * RADIUS, v * speed2, type, damage, 1f, player.whoAmI, -60f, speed2);
                                                    Main.projectile[p].timeLeft += 120;
                                                }
                                            }
                                        }
                                    }
                                    NPC.netUpdate = true;
                                }
                                NPC.ai[2] = 0f;
                            }
                            else
                            {
                                NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(new Vector2(NPC.ai[1], NPC.ai[2]) - center) * 20f, 0.025f);
                            }
                        }
                    }
                    break;

                case PHASE_HYPER_STARITE_PART2_ALT:
                case PHASE_HYPER_STARITE_PART2:
                    {
                        NPC.ai[2]++;
                        if (NPC.ai[2] > 300f)
                        {
                            if (NPC.ai[1] > 0.0314)
                            {
                                NPC.ai[1] -= 0.0005f;
                            }
                            else
                            {
                                NPC.ai[1] = 0.0314f;
                            }
                            innerRingRotation += NPC.ai[1];
                            outerRingRotation += NPC.ai[1] * 0.5f;
                            bool innerRing = false;
                            if (orbs[0].radius > orbs[0].defRadius)
                            {
                                SetInnerRingRadius(orbs[0].radius - MathHelper.Pi);
                            }
                            else
                            {
                                innerRing = true;
                            }
                            bool outerRing = false;
                            if (orbs[OmegaStariteOrb.INNER_RING].radius > orbs[OmegaStariteOrb.INNER_RING].defRadius)
                            {
                                SetOuterRingRadius(orbs[OmegaStariteOrb.INNER_RING].radius - MathHelper.PiOver2 * 3f);
                            }
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
                                    if (NPC.life / (float)NPC.lifeMax < (Main.expertMode ? 0.5f : 0.33f))
                                        choices.Add(PHASE_STAR_BULLETS);
                                    if (choices.Count == 1)
                                    {
                                        NPC.ai[0] = choices[0];
                                    }
                                    else
                                    {
                                        NPC.ai[0] = choices[Main.rand.Next(choices.Count)];
                                    }
                                    NPC.ai[1] = 0f;
                                    NPC.ai[2] = 0f;
                                    NPC.ai[3] = 0f;
                                }
                            }
                        }
                        else if ((center - plrCenter).Length() > 1800f)
                        {
                            NPC.ai[2] = 300f;
                            innerRingRotation += NPC.ai[1];
                            outerRingRotation += NPC.ai[1] * 0.5f;
                        }
                        else
                        {
                            if (NPC.ai[1] >= 0.0628f)
                            {
                                NPC.ai[1] = 0.0628f;
                            }
                            else
                            {
                                NPC.ai[1] += 0.0002f;
                            }
                            innerRingRotation += NPC.ai[1];
                            outerRingRotation += NPC.ai[1] * 0.5f;
                            SetInnerRingRadius(MathHelper.Lerp(orbs[0].radius, orbs[0].defRadius * NPC.ai[3], 0.025f));
                            SetOuterRingRadius(MathHelper.Lerp(orbs[OmegaStariteOrb.INNER_RING].radius, orbs[OmegaStariteOrb.INNER_RING].defRadius * (NPC.ai[3] + 1f), 0.025f));
                            if (NPC.ai[2] > 100f)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    if (Vector2.Distance(plrCenter, center) > orbs[OmegaStariteOrb.INNER_RING].radius)
                                    {
                                        NPC.localAI[0]++;
                                        if (NPC.localAI[0] > (Main.expertMode ? 3f : 12f))
                                        {
                                            float lifePercent = NPC.life / (float)NPC.lifeMax;
                                            if (lifePercent < 0.75f)
                                            {
                                                SoundEngine.PlaySound(SoundID.DD2_DarkMageCastHeal, NPC.Center);
                                                var diff = new Vector2(orbs[OmegaStariteOrb.INNER_RING].position.X, orbs[OmegaStariteOrb.INNER_RING].position.Y) - NPC.Center;
                                                var shootDir = Vector2.Normalize(diff).RotatedBy(MathHelper.PiOver2) * 6f;
                                                int type = ModContent.ProjectileType<OmegaBullet>();
                                                int damage = 30;
                                                if (Main.expertMode)
                                                    damage = 20;
                                                for (int i = 0; i < OmegaStariteOrb.OUTER_RING; i++)
                                                {
                                                    float rot = orbs[i + OmegaStariteOrb.INNER_RING].maxRotation * i;
                                                    Projectile.NewProjectile(NPC.GetProjectileSpawnSource(), center + diff.RotatedBy(rot), shootDir.RotatedBy(rot), type, damage, 1f, player.whoAmI);
                                                }
                                            }
                                            NPC.localAI[0] = 0f;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;

                case PHASE_HYPER_STARITE_PART1:
                    {
                        if (NPC.ai[1] == 0f)
                        {
                            innerRingPitch %= MathHelper.Pi;
                            innerRingRoll %= MathHelper.Pi;
                            outerRingPitch %= MathHelper.Pi;
                            outerRingRoll %= MathHelper.Pi;
                        }
                        NPC.ai[1] += 0.0002f;
                        const float lerpValue = 0.025f;
                        const float xLerp = 0f;
                        const float yLerp = 0f;
                        innerRingPitch = innerRingPitch.AngleLerp(xLerp, lerpValue);
                        innerRingRoll = innerRingRoll.AngleLerp(yLerp, lerpValue);
                        outerRingPitch = outerRingPitch.AngleLerp(xLerp, lerpValue);
                        outerRingRoll = outerRingRoll.AngleLerp(yLerp, lerpValue);
                        if (NPC.ai[1] > 0.0314f)
                        {
                            const float marginOfError = 0.314f;
                            if (innerRingPitch.IsCloseEnoughTo(xLerp, marginOfError) &&
                                outerRingPitch.IsCloseEnoughTo(xLerp, marginOfError) &&
                                innerRingRoll.IsCloseEnoughTo(xLerp, marginOfError) &&
                                outerRingRoll.IsCloseEnoughTo(xLerp, marginOfError))
                            {
                                NPC.velocity = Vector2.Normalize(plrCenter - center) * NPC.velocity.Length();
                                innerRingPitch = 0f;
                                innerRingRoll = 0f;
                                outerRingPitch = 0f;
                                outerRingRoll = 0f;
                                if (PlrCheck())
                                {
                                    NPC.ai[0] = Main.rand.NextBool() ? PHASE_HYPER_STARITE_PART2 : PHASE_HYPER_STARITE_PART2_ALT;
                                    NPC.ai[1] = 0f;
                                    NPC.ai[3] = 3f + (1f - NPC.life / (float)NPC.lifeMax) * 1.5f;
                                }
                            }
                        }
                        else
                        {
                            innerRingRotation += 0.0314f - NPC.ai[1];
                            outerRingRotation += 0.0157f - NPC.ai[1] * 0.5f;
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
                        if (NPC.ai[1] == 0f)
                        {
                            if (PlrCheck())
                            {
                                SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack, NPC.Center);
                                NPC.ai[1] = plrCenter.X + player.velocity.X * 20f;
                                NPC.ai[2] = plrCenter.Y + player.velocity.Y * 20f;
                            }
                            else
                            {
                                break;
                            }
                        }
                        if ((center - new Vector2(NPC.ai[1], NPC.ai[2])).Length() < CIRCUMFERENCE)
                        {
                            if (NPC.velocity.Length() < 2f)
                            {
                                SetInnerRingRadius(orbs[0].defRadius);
                                SetOuterRingRadius(orbs[OmegaStariteOrb.INNER_RING].defRadius);
                                if (PlrCheck())
                                {
                                    NPC.velocity *= 0.1f;
                                    if (NPC.life / (float)NPC.lifeMax < 0.5f)
                                    {
                                        NPC.ai[0] = PHASE_OMEGA_LASER_PART0;
                                    }
                                    else
                                    {
                                        NPC.ai[0] = PHASE_HYPER_STARITE_PART1;
                                    }
                                    NPC.ai[1] = 0f;
                                    NPC.ai[2] = 0f;
                                }
                            }
                            else
                            {
                                SetInnerRingRadius(MathHelper.Lerp(orbs[0].radius, orbs[0].defRadius, 0.1f));
                                SetOuterRingRadius(MathHelper.Lerp(orbs[OmegaStariteOrb.INNER_RING].radius, orbs[OmegaStariteOrb.INNER_RING].defRadius, 0.1f));
                                NPC.velocity *= 0.925f;
                            }
                        }
                        else
                        {
                            NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(new Vector2(NPC.ai[1], NPC.ai[2]) - center) * 30f, 0.025f);
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
                        NPC.ai[0] = choices[Main.rand.Next(choices.Count)];
                        Init();
                    }
                    break;

                case PHASE_DEAD:
                    {
                        NPC.ai[1] += 0.5f;
                        if (NPC.ai[1] > DEATH_TIME * 1.314f)
                        {
                            NPC.life = -33333;
                            NPC.HitEffect();
                            NPC.checkDead();
                        }
                    }
                    break;

                case PHASE_NOVA:
                    {
                        if (NPC.ai[1] == 0f)
                        {
                            int target = NPC.target;
                            Init();
                            NPC.target = target;
                            NPC.ai[2] = plrCenter.Y - CIRCUMFERENCE * 2.5f;
                        }
                        if (center.Y > NPC.ai[2])
                        {
                            int[] choices = new int[] { PHASE_HYPER_STARITE_PART0, PHASE_ASSAULT_PLAYER };
                            if (Scene.CurrentScene == 1)
                                Scene.CurrentScene = 2;
                            NPC.ai[0] = choices[Main.rand.Next(choices.Length)];
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                        }
                        else
                        {
                            NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(new Vector2(center.X, NPC.ai[2]) - center) * 36f, 0.025f);
                        }
                    }
                    break;

                case PHASE_GOODBYE:
                    {
                        if (NPC.timeLeft > 120)
                            NPC.timeLeft = 120;
                        NPC.velocity.X *= 0.975f;
                        NPC.velocity.Y -= 0.2f;
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
            if (NPC.ai[0] != -1)
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
                        var spawnPos = new Vector2(RADIUS, 0f).RotatedBy(NPC.velocity.ToRotation() - MathHelper.Pi);
                        int d = Dust.NewDust(NPC.Center + spawnPos.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)), 2, 2, 15);
                        Main.dust[d].velocity = -NPC.velocity * 0.25f;
                    }
                }
                if (Main.rand.NextBool(30))
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 58);
                    Main.dust[d].velocity.X = Main.rand.NextFloat(-4f, 4f);
                    Main.dust[d].velocity.Y = Main.rand.NextFloat(-4f, 4f);
                }
                if (Main.rand.NextBool(30))
                {
                    int g = Gore.NewGore(NPC.position + new Vector2(Main.rand.Next(NPC.width - 4), Main.rand.Next(NPC.height - 4)), new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f)), 16 + Main.rand.Next(2));
                    Main.gore[g].scale *= 0.6f;
                }
            }
            Lighting.AddLight(NPC.Center, new Vector3(1.2f, 1.2f, 2.2f));
            foreach (var orb in orbs)
            {
                Lighting.AddLight(new Vector2(orb.position.X, orb.position.Y), new Vector3(0.4f, 0.4f, 1f));
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.ai[0] != -1)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter >= 6)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
                        NPC.frame.Y = 0;
                }
            }
        }

        public override bool CheckDead()
        {
            if (NPC.ai[0] == -1f || skipDeathTimer > 0)
            {
                NPC.lifeMax = -33333;
                return true;
            }
            //NPC.GetGlobalNPC<NoHitManager>().dontHitPlayers = true;
            NPC.ai[0] = -1f;
            NPC.ai[1] = 0f;
            NPC.ai[2] = 0f;
            NPC.ai[3] = 0f;
            NPC.velocity = new Vector2(0f, 0f);
            NPC.dontTakeDamage = true;
            NPC.life = NPC.lifeMax;
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life == -33333)
            {
                //if (NoHitManager.HasBeenNoHit(npc, Main.myPlayer))
                //{
                //    NoHitManager.PlayNoHitJingle(NPC.Center);
                //}
                float rotationOff = MathHelper.TwoPi / 80;
                var center = NPC.Center;
                for (int i = 0; i < 80; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 15);
                    Main.dust[d].velocity = new Vector2(20f, 0f).RotatedBy(rotationOff * i);
                }
                for (int i = 0; i < 150; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 55);
                    Main.dust[d].velocity.X += Main.rand.NextFloat(-2, 2);
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(4f, 12f);
                }
                for (int i = 0; i < 100; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 57 + Main.rand.Next(2));
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
                        int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 15);
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
            else if (NPC.life <= 0)
            {
                //if (skipDeathTimer > 0 && NoHitManager.HasBeenNoHit(npc, Main.myPlayer))
                //{
                //    NoHitManager.PlayNoHitJingle(NPC.Center);
                //}
                for (int i = 0; i < 50; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 55);
                    Main.dust[d].velocity.X += Main.rand.NextFloat(-2, 2);
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                }
                for (int i = 0; i < 30; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 57 + Main.rand.Next(2));
                    Main.dust[d].velocity.X += Main.rand.NextFloat(-2, 2);
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                }
                for (int i = 0; i < 10; i++)
                {
                    Gore.NewGore(NPC.Center, new Vector2(Main.rand.NextFloat(-9f, 9f), Main.rand.NextFloat(-9f, 9f)), 16 + Main.rand.Next(2));
                }
            }
            else
            {
                float x = NPC.velocity.X.Abs() * hitDirection;
                for (int i = 0; i < 3; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 55);
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(5f, 12f);
                }
                if (Main.rand.NextBool())
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 57 + Main.rand.Next(2));
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                }
                if (Main.rand.NextBool())
                    Gore.NewGore(NPC.Center, new Vector2(Main.rand.NextFloat(-4f, 4f) + x * 0.75f, Main.rand.NextFloat(-4f, 4f)), 16 + Main.rand.Next(2));
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.expertMode)
            {
                if (Main.rand.NextBool())
                    target.AddBuff(ModContent.BuffType<BlueFire>(), 120);
                if (Main.rand.NextBool())
                    target.AddBuff(BuffID.Blackout, 360);
            }
            else
            {
                if (Main.rand.NextBool())
                    target.AddBuff(BuffID.OnFire, 120);
                if (Main.rand.NextBool())
                    target.AddBuff(BuffID.Darkness, 120);
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return NPC.ai[0] != -1;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.type == ProjectileID.FallingStar)
            {
                if (damage > 300)
                {
                    GlimmerEvent.DiscoParty = true;
                    Vector2 velo = projectile.velocity * -1.2f;
                    for (int i = 0; i < 8; i++)
                    {
                        int p2 = Projectile.NewProjectile(NPC.GetProjectileSpawnSource(), projectile.Center, velo.RotatedBy(MathHelper.PiOver4 * i), ModContent.ProjectileType<RainbowStarofHyperApocalypse>(), damage, knockback);
                        Main.projectile[p2].timeLeft = 240;
                    }
                    damage = (int)(damage * 0.25);
                    projectile.active = false;
                    return;
                }
                int p = Projectile.NewProjectile(NPC.GetProjectileSpawnSource(), projectile.Center, projectile.velocity * -1.2f, ModContent.ProjectileType<RainbowStarofHyperApocalypse>(), damage, knockback);
                Main.projectile[p].timeLeft = 240;
                damage = (int)(damage * 0.25);
                projectile.active = false;
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }

        public override bool PreKill()
        {
            Scene.IndexCache = -1;
            Scene.CurrentScene = 3;
            return base.PreKill();
        }

        public override void OnKill()
        {
            GlimmerEvent.deactivationDelay = 275;
            WorldFlags.DownedOmegaStarite = true;
            WorldFlags.CompletedGlimmerEvent = true;
            if (GlimmerEvent.IsActive)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if (Main.player[i].active && NPC.playerInteraction[i])
                    {
                        WorldFlags.ObtainedUltimateSword = true;
                        Projectile.NewProjectile(NPC.GetProjectileSpawnSource(), NPC.Center, new Vector2(Main.rand.NextFloat(2f, 6f) * (Main.rand.NextBool() ? -1f : 1f), -18f), ModContent.ProjectileType<Projectiles.UltimateSword>(), 0, 0f, i);
                    }
                }
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
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
            var drawPos = NPC.Center - Main.screenPosition;
            var sortedOmegites = new List<OmegaStariteOrb>(orbs);
            var options = ClientOptions.Instance;
            float intensity = options.FXIntensity;
            if ((int)NPC.ai[0] == -1)
            {
                intensity += NPC.ai[1] / 20;
                int range = (int)intensity + 4;
                drawPos += new Vector2(Main.rand.Next(-range, range), Main.rand.Next(-range, range));
                for (int i = 0; i < orbs.Count; i++)
                {
                    sortedOmegites[i].drawOffset = new Vector3(Main.rand.Next(-range, range), Main.rand.Next(-range, range), Main.rand.Next(-range, range));
                }
                //ScreenShakeManager.ChannelEffect("OmegaStariteDeathScreenShake", new OmegaStariteScreenShake((int)(range * 0.8f), 0.01f, Math.Max(6 - (int)(range * 0.8), 1)));
            }
            sortedOmegites.Sort((o, o2) => -(o.position.Z + o.drawOffset.Z).CompareTo(o2.position.Z + o2.drawOffset.Z));
            var orbAsset = ModContent.Request<Texture2D>(this.GetPath("_Orb"), AssetRequestMode.AsyncLoad);
            if (orbAsset.Value == null)
            {
                return false;
            }
            var orbTexture = orbAsset.Value;
            var orbFrame = new Rectangle(0, 0, orbTexture.Width, orbTexture.Height);
            var orbOrigin = orbFrame.Size() / 2f;
            float xOff = (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 3f) + 1f);
            var clr3 = new Color(50, 50, 50, 0) * (intensity - options.FXIntensity + 1f);
            float deathSpotlightScale = 0f;
            if (intensity > 3f)
                deathSpotlightScale = NPC.scale * (intensity - 2.1f) * ((float)Math.Sin(NPC.ai[1] * 0.1f) + 1f) / 2f;
            var spotlight = AQTextures.Lights.Request(LightTex.Spotlight66x66);
            if (spotlight == null)
            {
                return false;
            }
            var spotlightOrig = spotlight.Size() / 2f;
            Color spotlightColor;
            if (GlimmerEvent.DiscoParty)
            {
                spotlightColor = Main.DiscoColor;
                spotlightColor.A = 0;
            }
            else
            {
                spotlightColor = options.StariteAuraColoring;
            }
            var drawOmegite = new List<AQGraphics.DrawMethod>();
            if (options.FXQuality >= 1f)
            {
                drawOmegite.Add(delegate (Texture2D texture1, Vector2 position, Rectangle? frame1, Color color, float scale, Vector2 origin1, float rotation, SpriteEffects effects, float layerDepth)
                {
                    spriteBatch.Draw(spotlight, position, null, spotlightColor, rotation, spotlightOrig, scale * 1.33f, SpriteEffects.None, 0f);
                });
            }
            drawOmegite.Add(delegate (Texture2D texture1, Vector2 position, Rectangle? frame1, Color color, float scale, Vector2 origin1, float rotation, SpriteEffects effects, float layerDepth)
            {
                spriteBatch.Draw(orbTexture, position, orbFrame, drawColor, rotation, origin1, scale, SpriteEffects.None, 0f);
            });
            if (intensity >= 1f)
            {
                drawOmegite.Add(delegate (Texture2D texture1, Vector2 position, Rectangle? frame1, Color color, float scale, Vector2 origin1, float rotation, SpriteEffects effects, float layerDepth)
                {
                    for (int j = 0; j < intensity; j++)
                    {
                        spriteBatch.Draw(orbTexture, position + new Vector2(2f + xOff * 2f * j, 0f), orbFrame, clr3, rotation, origin1, scale, SpriteEffects.None, 0f);
                        spriteBatch.Draw(orbTexture, position + new Vector2(2f - xOff * 2f * j, 0f), orbFrame, clr3, rotation, origin1, scale, SpriteEffects.None, 0f);
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
                    var drawPosition = AQUtils.OmegaStariteHelper.GetParralaxPosition(new Vector2(sortedOmegites[i].position.X + sortedOmegites[i].drawOffset.X, sortedOmegites[i].position.Y + sortedOmegites[i].drawOffset.Y), sortedOmegites[i].position.Z * 0.00728f) - Main.screenPosition;
                    var drawScale = AQUtils.OmegaStariteHelper.GetParralaxScale(sortedOmegites[i].scale, (sortedOmegites[i].position.Z + sortedOmegites[i].drawOffset.Z) * 0.0314f);
                    foreach (var draw in drawOmegite)
                    {
                        draw.Invoke(
                            orbTexture,
                            drawPosition,
                            orbFrame,
                            drawColor,
                            drawScale,
                            orbOrigin,
                            NPC.rotation,
                            SpriteEffects.None,
                            0f);
                    }
                    sortedOmegites.RemoveAt(i);
                    i--;
                }
            }
            var texture = TextureAssets.Npc[NPC.type].Value;
            var offset = new Vector2(NPC.width / 2f, NPC.height / 2f);
            Vector2 origin = NPC.frame.Size() / 2f;
            float mult = 1f / NPCID.Sets.TrailCacheLength[NPC.type];
            var clr = drawColor * 0.25f;
            for (int i = 0; i < intensity; i++)
            {
                spriteBatch.Draw(spotlight, drawPos, null, spotlightColor, NPC.rotation, spotlightOrig, NPC.scale * 2.5f + i, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(spotlight, drawPos, null, spotlightColor * (1f - (intensity - (int)intensity)), NPC.rotation, spotlightOrig, NPC.scale * 2.5f + ((int)intensity + 1), SpriteEffects.None, 0f);
            if ((NPC.position - NPC.oldPos[1]).Length() > 0.01f)
            {
                var trueOldPos = new List<Vector2>();
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    if (NPC.oldPos[i] == new Vector2(0f, 0f))
                        break;
                    trueOldPos.Add(NPC.oldPos[i] + offset - Main.screenPosition);
                }
                if (trueOldPos.Count > 1)
                {
                    AQVertexStrip.ReversedGravity(trueOldPos);
                    const float radius = CIRCUMFERENCE / 2f;
                    Vector2[] arr;
                    arr = trueOldPos.ToArray();
                    if (arr.Length > 1)
                    {
                        var trailClr = GlimmerEvent.DiscoParty ? Main.DiscoColor : new Color(35, 85, 255, 120);
                        var trail = new AQVertexStrip(TextureAssets.Extra[ExtrasID.RainbowRodTrailShape].Value, AQVertexStrip.TextureTrail);
                        trail.PrepareVertices(arr, (p) => new Vector2(radius - p * radius), (p) => trailClr * (1f - p));
                        trail.Draw();
                    }
                }
            }
            else
            {
                NPC.oldPos[0] = new Vector2(0f, 0f);
            }
            spriteBatch.Draw(texture, drawPos, NPC.frame, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            for (int j = 0; j < intensity; j++)
            {
                spriteBatch.Draw(texture, drawPos + new Vector2(2f + xOff * 2f * j, 0f), NPC.frame, clr3, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(texture, drawPos - new Vector2(2f + xOff * 2f * j, 0f), NPC.frame, clr3, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            }
            for (int i = 0; i < sortedOmegites.Count; i++)
            {
                var drawPosition = AQUtils.OmegaStariteHelper.GetParralaxPosition(new Vector2(sortedOmegites[i].position.X + sortedOmegites[i].drawOffset.X, sortedOmegites[i].position.Y + sortedOmegites[i].drawOffset.Y), sortedOmegites[i].position.Z * 0.00728f) - Main.screenPosition;
                var drawScale = AQUtils.OmegaStariteHelper.GetParralaxScale(sortedOmegites[i].scale, (sortedOmegites[i].position.Z + sortedOmegites[i].drawOffset.Z) * 0.0314f);
                foreach (var draw in drawOmegite)
                {
                    draw.Invoke(
                        orbTexture,
                        drawPosition,
                        orbFrame,
                        drawColor,
                        drawScale,
                        orbOrigin,
                        NPC.rotation,
                        SpriteEffects.None,
                        0f);
                }
            }
            if (intensity > 3f)
            {
                float intensity2 = intensity - 2f;
                if (NPC.ai[1] > DEATH_TIME)
                {
                    float scale = (NPC.ai[1] - DEATH_TIME) * 0.2f * options.FXIntensity;
                    scale *= scale;
                    Main.spriteBatch.Draw(spotlight, drawPos, null, new Color(120, 120, 120, 0) * intensity2, NPC.rotation, spotlightOrig, scale, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightColor * intensity2, NPC.rotation, spotlightOrig, scale * 2.15f, SpriteEffects.None, 0f);
                }
                else
                {
                    Main.spriteBatch.Draw(spotlight, drawPos, null, new Color(120, 120, 120, 0) * intensity2, NPC.rotation, spotlightOrig, deathSpotlightScale, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightColor * intensity2, NPC.rotation, spotlightOrig, deathSpotlightScale * 2f, SpriteEffects.None, 0f);
                }
            }
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(skipDeathTimer);

            writer.Write(innerRingPitch);
            writer.Write(innerRingRoll);
            writer.Write(innerRingRotation);

            writer.Write(outerRingPitch);
            writer.Write(outerRingRoll);
            writer.Write(outerRingRotation);

            writer.Write(orbs.Count);
            for (int i = 0; i < orbs.Count; i++)
            {
                writer.Write(orbs[i].radius);
                writer.Write(orbs[i].scale);
                writer.Write(orbs[i].defRotation);
                writer.Write(orbs[i].maxRotation);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            skipDeathTimer = reader.ReadInt32();

            innerRingPitch = reader.ReadSingle();
            innerRingRoll = reader.ReadSingle();
            innerRingRotation = reader.ReadSingle();

            outerRingPitch = reader.ReadSingle();
            outerRingRoll = reader.ReadSingle();
            outerRingRotation = reader.ReadSingle();

            int count = reader.ReadInt32();
            if (orbs == null)
            {
                orbs = new List<OmegaStariteOrb>();
            }
            for (int i = 0; i < count; i++)
            {
                if (orbs.Count <= i)
                {
                    orbs.Add(new OmegaStariteOrb(Vector3.Zero, reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
                }
                else
                {
                    orbs[i].radius = reader.ReadSingle();
                    orbs[i].scale = reader.ReadSingle();
                    reader.ReadSingle();
                    reader.ReadSingle(); // useless.
                }
            }
            Spin(NPC.Center);
        }
    }
}