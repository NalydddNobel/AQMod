using AQMod.Assets;
using AQMod.Buffs.Debuffs;
using AQMod.Common;
using AQMod.Common.Configuration;
using AQMod.Common.CrossMod.BossChecklist;
using AQMod.Common.Graphics;
using AQMod.Common.ID;
using AQMod.Common.NoHitting;
using AQMod.Content.World.Events.GlimmerEvent;
using AQMod.Dusts;
using AQMod.Effects;
using AQMod.Effects.ScreenEffects;
using AQMod.Effects.Trails.Rendering;
using AQMod.Items.BossItems.Starite;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Tools.Map;
using AQMod.Items.Vanities.Dyes;
using AQMod.Items.Vanities.Pets;
using AQMod.Items.Weapons.Magic;
using AQMod.Items.Weapons.Ranged;
using AQMod.Localization;
using AQMod.Projectiles.Monster.Starite;
using AQMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Boss
{
    [AutoloadBossHead()]
    public class OmegaStarite : AQBoss, IModifiableMusicNPC
    {
        public const float Circumference = 120;
        public const float Radius = Circumference / 2f;
        private const float DEATH_TIME = MathHelper.PiOver4 * 134;

        public const int InnerRingSegmentCount = 5;
        public const float InnerRingScale = 1f;

        public const int OuterRingSegmentCount = 8;
        public const float OuterRingScale_Normal = 1.1f;
        public const float OuterRingSegment_Expert = 1.2f;
        public const float OuterRingCircumferenceMultiplier_Normal = 1.5f;
        public const float OuterRingCircumferenceMultiplier_Expert = 1.75f;

        public class Ring
        {
            public readonly byte amountOfSegments;
            public readonly float rotationOrbLoop;
            public readonly float originalRadiusFromOrigin;
            public readonly float scale;
            public readonly Vector3[] CachedPositions;
            public readonly Rectangle[] CachedHitboxes;

            public float pitch;
            public float roll;
            public float yaw;
            public float radiusFromOrigin;

            public Vector3 rotationVelocity;

            public Ring (int amount, float radiusFromOrigin, float scale)
            {
                amountOfSegments = (byte)amount;
                rotationOrbLoop = MathHelper.TwoPi / amountOfSegments;
                originalRadiusFromOrigin = radiusFromOrigin;
                this.radiusFromOrigin = originalRadiusFromOrigin;
                this.scale = scale;
                CachedPositions = new Vector3[amountOfSegments];
                CachedHitboxes = new Rectangle[amountOfSegments];
            }

            /// <summary>
            /// Creates a Ring through a net package
            /// </summary>
            /// <param name="reader"></param>
            public Ring(BinaryReader reader)
            {
                amountOfSegments = reader.ReadByte();
                rotationOrbLoop = MathHelper.TwoPi / amountOfSegments;
                originalRadiusFromOrigin = reader.ReadSingle();
                radiusFromOrigin = originalRadiusFromOrigin;
                scale = reader.ReadSingle();
                CachedPositions = new Vector3[amountOfSegments];
                CachedHitboxes = new Rectangle[amountOfSegments];
            }

            public static Ring[] FromNetPackage(BinaryReader reader)
            {
                byte amount = reader.ReadByte();
                var rings = new Ring[amount];
                for (byte i = 0; i < amount; i++)
                {
                    rings[i] = new Ring(reader);
                }
                return rings;
            }

            public void Update(Vector2 origin)
            {
                pitch += rotationVelocity.X;
                roll += rotationVelocity.Y;
                yaw = (yaw + rotationVelocity.Z) % rotationOrbLoop;
                int i = 0;
                for (float r = 0f; i < amountOfSegments; r += rotationOrbLoop)
                {
                    CachedPositions[i] = Vector3.Transform(new Vector3(radiusFromOrigin, 0f, 0f), Matrix.CreateFromYawPitchRoll(pitch, roll, r + yaw)) + new Vector3(origin, 0f);
                    CachedHitboxes[i] = Utils.CenteredRectangle(new Vector2(CachedPositions[i].X, CachedPositions[i].Y), new Vector2(50f, 50f) * scale);
                    i++;
                }
            }

            public static void SendNetPackage(BinaryWriter writer, Ring[] rings)
            {
                for (byte i = 0; i < rings.Length; i++)
                {
                    rings[i].SendNetPackage(writer);
                }
            }

            public void SendNetPackage(BinaryWriter writer)
            {
                writer.Write(pitch);
                writer.Write(roll);
                writer.Write(yaw);
            }

            public void RecieveNetPackage(BinaryReader reader)
            {
                pitch = reader.ReadSingle();
                roll = reader.ReadSingle();
                yaw = reader.ReadSingle();
            }
        }

        public Ring[] rings;
        public int skipDeathTimer;
        private byte _hitShake;

        public override BossEntry? BossChecklistEntry => new BossEntry(
            () => WorldDefeats.DownedStarite,
            6.1f,
            ModContent.NPCType<OmegaStarite>(),
            AQText.chooselocalizationtext(en_US: "Omega Starite", zh_Hans: "终末之星"),
            ModContent.ItemType<NovaFruit>(),
            new List<int>() {
                ModContent.ItemType<Items.Accessories.CelesteTorus>(),
                ModContent.ItemType<Items.Weapons.Melee.UltimateSword>(),
                ModContent.ItemType<CosmicTelescope>(),
                ModContent.ItemType<Raygun>(),
                ModContent.ItemType<MagicWand>(),
                ModContent.ItemType<CosmicEnergy>(),
                ItemID.SoulofFlight,
                ItemID.FallenStar,
            },
            new List<int>() {
                ModContent.ItemType<OmegaStariteTrophy>(),
                ModContent.ItemType<OmegaStariteMask>(),
                ModContent.ItemType<DragonBall>(),
                ModContent.ItemType<EnchantedDye>(),
                ModContent.ItemType<RainbowOutlineDye>(),
                ModContent.ItemType<DiscoDye>(),
            },
            AQText.chooselocalizationtext(
                en_US: "Summoned by using an [i:" + ModContent.ItemType<NovaFruit>() + "] at night. Can also be summoned by interacting with the sword located at the center of the Glimmer Event.",
                zh_Hans: null),
            "AQMod/Assets/BossChecklist/OmegaStarite"
        );

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
            npc.lifeMax = 9500;
            npc.damage = 50;
            npc.defense = 18;
            npc.DeathSound = SoundID.NPCDeath55;
            npc.aiStyle = -1;
            npc.noGravity = true;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(gold: 18);
            npc.boss = true;
            AQNPC.ImmuneToAllBuffs(npc);
            npc.npcSlots = 10f;
            npc.noTileCollide = true;
            npc.trapImmune = true;
            npc.lavaImmune = true;
            bossBag = ModContent.ItemType<StariteBag>();

            if (!GlimmerEvent.IsGlimmerEventCurrentlyActive())
                skipDeathTimer = 600;
            if (AQGraphics.CanUseAssets)
            {
                music = GetMusic().GetMusicID();
                musicPriority = MusicPriority.BossMedium;
                if (AprilFoolsJoke.Active)
                {
                    npc.GivenName = "Omega Starite, Living Galaxy the Omega Being";
                }
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

        public bool IsOmegaLaserActive()
        {
            return npc.ai[0] == PHASE_OMEGA_LASER && npc.ai[2] < 1200f;
        }

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
            npc.netUpdate = true;
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

        public void Initialize()
        {
            npc.TargetClosest(faceTarget: false);
            OmegaStariteScenes.OmegaStariteIndexCache = (short)npc.whoAmI;
            var center = npc.Center;
            rings = new Ring[2];
            if (Main.expertMode)
            {
                rings[0] = new Ring(InnerRingSegmentCount, Circumference, InnerRingScale);
                rings[1] = new Ring(OuterRingSegmentCount, Circumference * OuterRingCircumferenceMultiplier_Expert, OuterRingSegment_Expert);
            }
            else
            {
                rings[0] = new Ring(InnerRingSegmentCount, Circumference * 0.75f, InnerRingScale);
                rings[1] = new Ring(OuterRingSegmentCount, Circumference * OuterRingCircumferenceMultiplier_Normal, OuterRingScale_Normal);
            }
            rings[0].Update(center);
            rings[1].Update(center);
            int damage = Main.expertMode ? 25 : 30;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(npc.Center, Vector2.Zero,
                    ModContent.ProjectileType<Projectiles.Monster.Starite.OmegaStarite>(), damage, 1f, Main.myPlayer, npc.whoAmI);
            }
        }

        public override void AI()
        {
            if (Main.dayTime)
            {
                npc.life = -1;
                OmegaStariteScenes.OmegaStariteIndexCache = -1;
                OmegaStariteScenes.SceneType = 0;
                npc.HitEffect();
                Main.PlaySound(SoundID.Dig, npc.Center);
                npc.active = false;
                return;
            }
            //Main.NewText(npc.ai[0]);
            //AQMod.Instance.Logger.Debug(npc.ai[0]);
            if (skipDeathTimer > 0)
                skipDeathTimer--;
            Vector2 center = npc.Center;
            Player player = Main.player[npc.target];
            var plrCenter = player.Center;
            float speed = npc.velocity.Length();
            switch ((int)npc.ai[0])
            {
                default:
                    {
                        LerpToDefaultRotationVelocity();
                        npc.Center = plrCenter + new Vector2(0f, -Circumference * 2f);
                    }
                    break;

                case PHASE_OMEGA_LASER_PART0:
                    {
                        if (npc.ai[1] == 0f)
                        {
                            CullRingRotations();
                        }
                        npc.ai[1] += 0.0002f;
                        const float lerpValue = 0.025f;
                        const float xLerp = 0f;
                        const float yLerp = 0f;

                        rings[0].rotationVelocity *= 0.95f;
                        rings[1].rotationVelocity *= 0.95f;

                        rings[0].pitch = rings[0].pitch.AngleLerp(MathHelper.PiOver2, lerpValue);
                        rings[0].roll = rings[0].roll.AngleLerp(-MathHelper.PiOver2, lerpValue);
                        rings[1].pitch = rings[1].pitch.AngleLerp(xLerp, lerpValue);
                        rings[1].roll = rings[1].roll.AngleLerp(yLerp, lerpValue);
                        if (npc.ai[1] > 0.0314f)
                        {
                            const float marginOfError = 0.314f;
                            if (rings[0].pitch.IsCloseEnoughTo(MathHelper.PiOver2, marginOfError) &&
                                rings[0].roll.IsCloseEnoughTo(-MathHelper.PiOver2, marginOfError) &&
                                rings[1].pitch.IsCloseEnoughTo(yLerp, marginOfError) &&
                                rings[1].roll.IsCloseEnoughTo(yLerp, marginOfError))
                            {
                                npc.velocity = Vector2.Normalize(plrCenter - center) * npc.velocity.Length();
                                rings[0].pitch = MathHelper.PiOver2;
                                rings[0].roll = -MathHelper.PiOver2;
                                rings[1].pitch = xLerp;
                                rings[1].roll = yLerp;
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
                            rings[0].yaw += 0.0314f - npc.ai[1];
                            rings[1].yaw += 0.0157f - npc.ai[1] * 0.5f;
                        }
                    }
                    break;

                case PHASE_OMEGA_LASER:
                    {
                        npc.ai[2]++;
                        if (npc.ai[2] > 1200f)
                        {
                            if (npc.ai[1] > 0.0314)
                            {
                                npc.ai[1] -= 0.0005f;
                            }
                            else
                            {
                                npc.ai[1] = 0.0314f;
                            }
                            rings[0].yaw += npc.ai[1];
                            rings[1].yaw += npc.ai[1] * 0.5f;
                            bool outerRing = false;
                            if (rings[1].radiusFromOrigin > rings[1].originalRadiusFromOrigin)
                            {
                                rings[1].radiusFromOrigin -= MathHelper.PiOver2 * 3f;
                                if (Vector2.Distance(plrCenter, center) > rings[0].radiusFromOrigin)
                                {
                                    ShootProjectilesFromOuterRing(endingPhase: true);
                                }
                            }
                            else
                            {
                                outerRing = true;
                            }
                            if (outerRing)
                            {
                                ResetRingsRadiusFromOrigin();
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
                            npc.ai[2] = 1200f;
                            rings[0].yaw += npc.ai[1];
                            rings[1].yaw += npc.ai[1] * 0.5f;
                        }
                        else
                        {
                            if (npc.ai[1] >= 0.0628f)
                            {
                                npc.ai[1] = 0.0628f;
                            }
                            else
                            {
                                npc.ai[1] += 0.0002f;
                            }
                            rings[0].yaw += npc.ai[1];
                            rings[1].yaw += npc.ai[1] * 0.5f;
                            rings[1].radiusFromOrigin = MathHelper.Lerp(rings[1].radiusFromOrigin, rings[1].originalRadiusFromOrigin * (npc.ai[3] + 1f), 0.025f);
                            if (npc.ai[2] > 100f)
                            {
                                if (npc.localAI[1] == 0f)
                                {
                                    if (PlrCheck())
                                    {
                                        npc.localAI[1] = 1f;
                                        Main.PlaySound(SoundID.Trackable, npc.Center, 188);
                                        if (Main.netMode != NetmodeID.Server)
                                            ScreenShakeManager.AddShake(new BasicScreenShake(24, AQGraphics.MultIntensity(4)));
                                        int p = Projectile.NewProjectile(center, new Vector2(0f, 0f), ModContent.ProjectileType<OmegaRay>(), 100, 1f, Main.myPlayer, npc.whoAmI);
                                        Main.projectile[p].scale = 0.75f;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                if (rings[0].roll > MathHelper.PiOver2 * 6f)
                                {
                                    npc.localAI[2] -= Main.expertMode ? 0.001f : 0.00045f;
                                }
                                else
                                {
                                    npc.localAI[2] += Main.expertMode ? 0.00015f : 0.000085f;
                                }
                                if (Main.netMode != NetmodeID.MultiplayerClient && (Vector2.Distance(plrCenter, center) > rings[0].radiusFromOrigin))
                                {
                                    ShootProjectilesFromOuterRing(endingPhase: false);
                                }
                                rings[0].roll += npc.localAI[2];
                                if (npc.soundDelay <= 0)
                                {
                                    npc.soundDelay = 60;
                                    Main.PlaySound(SoundID.Trackable, npc.Center, 189 + Main.rand.Next(3));
                                }
                                if (npc.soundDelay > 0)
                                    npc.soundDelay--;
                                if (rings[0].roll > MathHelper.PiOver2 * 7f)
                                {
                                    npc.soundDelay = 0;
                                    Main.PlaySound(SoundID.Trackable, npc.Center, 188);
                                    npc.ai[2] = 1200f;
                                    rings[0].roll = -MathHelper.PiOver2;
                                }
                            }
                            else
                            {
                                const int width = (int)(Circumference * 2f);
                                const int height = 900;
                                Vector2 dustPos = center + new Vector2(-width / 2f, 0f);
                                Dust.NewDust(dustPos, width, height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, GlimmerEvent.stariteProjectileColoring, 2f);
                                Dust.NewDust(dustPos, width, height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, GlimmerEvent.stariteProjectileColoring, 2f);
                                Dust.NewDust(dustPos, width, height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, GlimmerEvent.stariteProjectileColoring, 2f);
                            }
                        }
                    }
                    break;

                case PHASE_STAR_BULLETS:
                    {
                        LerpToDefaultRotationVelocity();

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
                                AQSound.Play(SoundType.Item, "OmegaStarite/starbullets", npc.Center, 0.3f, 0.5f);
                                //Main.PlaySound(SoundID.Item125);
                                int type = ModContent.ProjectileType<OmegaBullet>();
                                float speed2 = Main.expertMode ? 12.5f : 5.5f;
                                int damage = 30;
                                if (Main.expertMode)
                                    damage = 20;
                                for (int i = 0; i < 5; i++)
                                {
                                    var v = new Vector2(0f, -1f).RotatedBy(MathHelper.TwoPi / 5f * i);
                                    int p = Projectile.NewProjectile(center + v * Radius, v * speed2, type, damage, 1f, player.whoAmI, -60f, speed2);
                                    Main.projectile[p].timeLeft += 120;
                                }
                                speed2 *= 1.2f;
                                for (int i = 0; i < 5; i++)
                                {
                                    var v = new Vector2(0f, -1f).RotatedBy(MathHelper.TwoPi / 5f * i);
                                    Projectile.NewProjectile(center + v * Radius, v * speed2, type, damage, 1f, player.whoAmI, -60f, speed2);
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        float distance = (center - plrCenter).Length();
                        if (distance > Circumference * 3.75f)
                        {
                            npc.velocity = Vector2.Lerp(npc.velocity, Vector2.Normalize(plrCenter - center) * npc.ai[2], 0.02f);
                        }
                        else if (distance < Circumference * 2.25f)
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
                        LerpToDefaultRotationVelocity();

                        if (npc.ai[1] < 0f)
                        {
                            npc.ai[1]++;
                            if (npc.ai[2] == 0f)
                            {
                                if (PlrCheck())
                                {
                                    npc.ai[2] = Main.expertMode ? 18f : 6f;
                                }
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
                            if ((center - new Vector2(npc.ai[1], npc.ai[2])).Length() < Circumference)
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
                                    if (Vector2.Distance(plrCenter, center) > 400f)
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            float lifePercent = npc.life / (float)npc.lifeMax;
                                            if ((Main.expertMode && lifePercent < 0.75f) || lifePercent < 0.6f)
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
                                                    int p = Projectile.NewProjectile(center + v * Radius, v * speed2, type, damage, 1f, player.whoAmI, -60f, speed2);
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
                            {
                                npc.ai[1] -= 0.0005f;
                            }
                            else
                            {
                                npc.ai[1] = 0.0314f;
                            }
                            rings[0].yaw += npc.ai[1];
                            rings[1].yaw += npc.ai[1] * 0.5f;

                            PullInRingsTransition();
                        }
                        else if ((center - plrCenter).Length() > 1800f)
                        {
                            npc.ai[2] = 300f;
                            rings[0].yaw += npc.ai[1];
                            rings[1].yaw += npc.ai[1] * 0.5f;
                        }
                        else
                        {
                            if (npc.ai[1] >= 0.0628f)
                            {
                                npc.ai[1] = 0.0628f;
                            }
                            else
                            {
                                npc.ai[1] += 0.0002f;
                            }
                            rings[0].yaw += npc.ai[1];
                            rings[1].yaw += npc.ai[1] * 0.5f;
                            rings[0].radiusFromOrigin = MathHelper.Lerp(rings[0].radiusFromOrigin, rings[0].originalRadiusFromOrigin * npc.ai[3], 0.025f);
                            rings[1].radiusFromOrigin = MathHelper.Lerp(rings[1].radiusFromOrigin, rings[1].originalRadiusFromOrigin * (npc.ai[3] + 1f), 0.025f);
                            if (npc.ai[2] > 100f)
                            {
                                    if (Vector2.Distance(plrCenter, center) > rings[0].radiusFromOrigin)
                                    {
                                    ShootProjectilesFromOuterRing();
                                    }
                            }
                        }
                    }
                    break;

                case PHASE_HYPER_STARITE_PART1:
                    {
                        if (npc.ai[1] == 0f)
                        {
                            rings[0].pitch %= MathHelper.Pi;
                            rings[0].roll %= MathHelper.Pi;
                            rings[1].pitch %= MathHelper.Pi;
                            rings[1].roll %= MathHelper.Pi;
                        }
                        npc.ai[1] += 0.0002f;
                        const float lerpValue = 0.025f;
                        const float xLerp = 0f;
                        const float yLerp = 0f;

                        rings[0].rotationVelocity *= 0.95f;
                        rings[1].rotationVelocity *= 0.95f;

                        rings[0].pitch = rings[0].pitch.AngleLerp(xLerp, lerpValue);
                        rings[0].roll = rings[0].roll.AngleLerp(yLerp, lerpValue);
                        rings[1].pitch = rings[1].pitch.AngleLerp(xLerp, lerpValue);
                        rings[1].roll = rings[1].roll.AngleLerp(yLerp, lerpValue);
                        if (npc.ai[1] > 0.0314f)
                        {
                            const float marginOfError = 0.314f;
                            if (rings[0].pitch.IsCloseEnoughTo(xLerp, marginOfError) &&
                                rings[1].pitch.IsCloseEnoughTo(xLerp, marginOfError) &&
                                rings[0].roll.IsCloseEnoughTo(xLerp, marginOfError) &&
                                rings[1].roll.IsCloseEnoughTo(xLerp, marginOfError))
                            {
                                npc.velocity = Vector2.Normalize(plrCenter - center) * npc.velocity.Length();
                                rings[0].pitch = 0f;
                                rings[0].roll = 0f;
                                rings[1].pitch = 0f;
                                rings[1].roll = 0f;
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
                            rings[0].yaw += 0.0314f - npc.ai[1];
                            rings[1].yaw += 0.0157f - npc.ai[1] * 0.5f;
                        }
                    }
                    break;

                case PHASE_HYPER_STARITE_PART0:
                    {
                        LerpToDefaultRotationVelocity();
                        if (npc.ai[1] == 0f)
                        {
                            if (PlrCheck())
                            {
                                Main.PlaySound(SoundID.Trackable, npc.Center, 40 + Main.rand.Next(3));
                                npc.ai[1] = plrCenter.X + player.velocity.X * 20f;
                                npc.ai[2] = plrCenter.Y + player.velocity.Y * 20f;
                                npc.netUpdate = true;
                            }
                            else
                            {
                                break;
                            }
                        }
                        if ((center - new Vector2(npc.ai[1], npc.ai[2])).Length() < Circumference)
                        {
                            if (npc.velocity.Length() < 2f)
                            {
                                ResetRingsRadiusFromOrigin();
                                if (PlrCheck())
                                {
                                    npc.velocity *= 0.1f;
                                    if (npc.life / (float)npc.lifeMax < 0.5f)
                                    {
                                        npc.ai[0] = PHASE_OMEGA_LASER_PART0;
                                    }
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
                        Initialize();
                        npc.netUpdate = true;
                    }
                    break;

                case PHASE_DEAD:
                    {
                        rings[0].rotationVelocity *= 0f;
                        rings[1].rotationVelocity *= 0f;
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
                            Initialize();
                            npc.netUpdate = true;
                            npc.target = target;
                            npc.ai[2] = plrCenter.Y - Circumference * 2.5f; 
                        }
                        LerpToDefaultRotationVelocity();
                        if (center.Y > npc.ai[2])
                        {
                            int[] choices = new int[] { PHASE_HYPER_STARITE_PART0, PHASE_ASSAULT_PLAYER };
                            if (OmegaStariteScenes.SceneType == 1)
                                OmegaStariteScenes.SceneType = 2;
                            npc.ai[0] = choices[Main.rand.Next(choices.Length)];
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                            npc.netUpdate = true;
                        }
                        else
                        {
                            //if (Main.netMode != NetmodeID.Server) //Could not get this to work!
                            //{
                            //    int id = mod.GetSoundSlot(SoundType.Item, "Sounds/Item/OmegaStarite/novaspawn");
                            //    float length = Vector2.Distance(npc.Center, Main.player[npc.target].Center);
                            //    if (!_playedSpawnSound)
                            //    {
                            //        Main.soundInstanceItem[id].Stop();
                            //        Main.soundInstanceItem[id] = Main.soundItem[id].CreateInstance();
                            //    }
                            //    if (length > 1000f)
                            //    {
                            //        Main.soundInstanceItem[id].Volume = 0.1f;
                            //    }
                            //    else
                            //    {
                            //        Main.soundInstanceItem[id].Volume = 1f - length / 1250f;
                            //    }
                            //    if (!_playedSpawnSound)
                            //    {
                            //        Main.soundInstanceItem[id].Play();
                            //        _playedSpawnSound = true;
                            //    }
                            //    Main.soundInstanceItem[id].Pitch = 0f;
                            //}
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

                        rings[0].yaw += 0.0314f;
                        rings[0].roll += 0.0157f;
                        rings[0].pitch += 0.01f;
                        rings[1].yaw += 0.0157f;
                        rings[1].roll += 0.0314f;
                        rings[1].pitch += 0.011f;
                    }
                    break;
            }
            for (int i = 0; i < rings.Length; i++)
            {
                rings[i].Update(center + npc.velocity);
            }
            if (npc.ai[0] != -1)
            {
                int chance = 10 - (int)speed;
                if (chance < 2 || Main.rand.NextBool(chance))
                {
                    if (speed < 2f)
                    {
                        var spawnPos = new Vector2(Radius, 0f);
                        int d = Dust.NewDust(center + spawnPos.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi)), 2, 2, 15);
                        Main.dust[d].velocity = Vector2.Normalize(spawnPos - center) * speed * 0.25f;
                    }
                    else
                    {
                        var spawnPos = new Vector2(Radius, 0f).RotatedBy(npc.velocity.ToRotation() - MathHelper.Pi);
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
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }
            Lighting.AddLight(npc.Center, new Vector3(1.2f, 1.2f, 2.2f));
            for (int i = 0; i < rings.Length; i++)
            {
                for (int j = 0; j < rings[i].amountOfSegments; j++)
                {
                    Lighting.AddLight(new Vector2(rings[i].CachedPositions[i].X, rings[i].CachedPositions[i].Y), new Vector3(0.4f, 0.4f, 1f));
                }
            }
        }

        private void LerpToDefaultRotationVelocity()
        {
            rings[0].rotationVelocity = Vector3.Lerp(rings[0].rotationVelocity, new Vector3(0.01f, 0.0157f, 0.0314f), 0.1f);
            rings[1].rotationVelocity = Vector3.Lerp(rings[1].rotationVelocity, new Vector3(0.011f, 0.0314f, 0.0157f), 0.1f);
        }

        private void ShootProjectilesFromOuterRing(bool endingPhase = false)
        {
            npc.localAI[0]++;
            int delay = (Main.expertMode ? 12 : 60);
            if (!endingPhase && Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 1000f)
            {
                delay /= 2;
            }
            if (npc.localAI[0] > delay )
            {
                float lifePercent = npc.life / (float)npc.lifeMax;
                if (lifePercent < 0.75f)
                {
                    float speed = 7.5f;
                    float distance = Vector2.Distance(Main.player[npc.target].Center, npc.Center);
                    if (distance > 1000f)
                    {
                        speed *= 1f + (distance - 1000f) / 200f;
                        if (speed > 20)
                        {
                            speed = 20f;
                        }
                    }
                    Main.PlaySound(SoundID.DD2_DarkMageHealImpact, npc.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        var diff = new Vector2(rings[1].CachedPositions[0].X, rings[1].CachedPositions[0].Y) - npc.Center;
                        var shootDir = Vector2.Normalize(diff).RotatedBy(MathHelper.PiOver2) * speed;
                        int type = ModContent.ProjectileType<OmegaBullet>();
                        int damage = 25;
                        if (Main.expertMode)
                            damage = 18;
                        for (int i = 0; i < rings[1].amountOfSegments; i++)
                        {
                            float rot = rings[1].rotationOrbLoop * i;
                            var position = npc.Center + diff.RotatedBy(rot);
                            Main.PlaySound(SoundID.Trackable, position, 55 + Main.rand.Next(3));
                            Projectile.NewProjectile(position, shootDir.RotatedBy(rot), type, damage, 1f, Main.myPlayer);
                        }
                    }
                }
                npc.localAI[0] = 0f;
            }
        }

        private void CullRingRotations()
        {
            for (int i = 0; i < rings.Length; i++)
            {
                rings[i].pitch %= MathHelper.TwoPi;
                rings[i].roll %= MathHelper.TwoPi;
            }
        }

        private void ResetRingsRadiusFromOrigin()
        {
            for (int i = 0; i < rings.Length; i++)
            {
                rings[i].radiusFromOrigin = rings[i].originalRadiusFromOrigin;
            }
        }

        private void PullInRingsTransition()
        {
            bool innerRing = false;
            bool outerRing = false;

            if (rings[0].radiusFromOrigin > rings[0].originalRadiusFromOrigin)
            {
                rings[0].radiusFromOrigin -= MathHelper.Pi;
            }
            else
            {
                innerRing = true;
            }
            if (rings[1].radiusFromOrigin > rings[1].originalRadiusFromOrigin)
            {
                rings[1].radiusFromOrigin -= MathHelper.PiOver2 * 3f;
            }
            else
            {
                outerRing = true;
            }

            if (innerRing && outerRing && Main.netMode != NetmodeID.MultiplayerClient)
            {
                rings[0].radiusFromOrigin = rings[0].originalRadiusFromOrigin;
                rings[1].radiusFromOrigin = rings[1].originalRadiusFromOrigin;
                if (PlrCheck())
                {
                    var choices = new List<int>
                    {
                        PHASE_ASSAULT_PLAYER,
                    };
                    if (npc.life / (float)npc.lifeMax < (Main.expertMode ? 0.5f : 0.33f))
                        choices.Add(PHASE_STAR_BULLETS);
                    if (choices.Count == 1)
                    {
                        npc.ai[0] = choices[0];
                    }
                    else
                    {
                        npc.ai[0] = choices[Main.rand.Next(choices.Count)];
                    }
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.localAI[1] = 0f;
                    npc.netUpdate = true;
                }
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
            npc.GetGlobalNPC<NoHitManager>().dontHitPlayers = true;
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
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }
            if (npc.life == -33333)
            {
                if (NoHitManager.HasBeenNoHit(npc, Main.myPlayer))
                {
                    NoHitManager.PlayNoHitJingle(npc.Center);
                }
                var center = npc.Center;
                for (int k = 0; k < 60; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 58, npc.velocity.X * 0.1f, npc.velocity.Y * 0.1f, 150, default(Color), 0.8f);
                }
                for (float f = 0f; f < 1f; f += 0.02f)
                {
                    Dust.NewDustPerfect(npc.Center, ModContent.DustType<MonoSparkleDust>(), Vector2.UnitY.RotatedBy(f * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (4f + Main.rand.NextFloat() * 4f), 150, Color.CornflowerBlue).noGravity = true;
                }
                for (float f = 0f; f < 1f; f += 0.05f)
                {
                    Dust.NewDustPerfect(npc.Center, ModContent.DustType<MonoSparkleDust>(), Vector2.UnitY.RotatedBy(f * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (2f + Main.rand.NextFloat() * 3f), 150, Color.Gold).noGravity = true;
                }
                if (AQGraphics.Cull_WorldPosition(npc.getRect()))
                {
                    for (int k = 0; k < 7; k++)
                    {
                        Gore.NewGore(npc.Center, Main.rand.NextVector2CircularEdge(0.5f, 0.5f) * npc.velocity.Length(), Utils.SelectRandom(Main.rand, 16, 17, 17, 17, 17, 17, 17, 17));
                    }
                }
                for (int i = 0; i < rings.Length; i++)
                {
                    for (int j = 0; j < rings[i].amountOfSegments; j++)
                    {
                        for (int k = 0; k < 30; k++)
                        {
                            Dust.NewDust(rings[i].CachedHitboxes[j].TopLeft(), rings[i].CachedHitboxes[j].Width, rings[i].CachedHitboxes[j].Height, 58, npc.velocity.X * 0.1f, npc.velocity.Y * 0.1f, 150, default(Color), 0.8f);
                        }
                        for (float f = 0f; f < 1f; f += 0.125f)
                        {
                            Dust.NewDustPerfect(rings[i].CachedHitboxes[j].Center.ToVector2(), ModContent.DustType<MonoSparkleDust>(), Vector2.UnitY.RotatedBy(f * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (4f + Main.rand.NextFloat() * 4f), 150, Color.CornflowerBlue).noGravity = true;
                        }
                        for (float f = 0f; f < 1f; f += 0.25f)
                        {
                            Dust.NewDustPerfect(rings[i].CachedHitboxes[j].Center.ToVector2(), ModContent.DustType<MonoSparkleDust>(), Vector2.UnitY.RotatedBy(f * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (2f + Main.rand.NextFloat() * 3f), 150, Color.Gold).noGravity = true;
                        }
                        if (AQGraphics.Cull_WorldPosition(rings[i].CachedHitboxes[j]))
                        {
                            for (int k = 0; k < 7; k++)
                            {
                                Gore.NewGore(npc.Center, Main.rand.NextVector2CircularEdge(0.5f, 0.5f) * npc.velocity.Length(), Utils.SelectRandom(Main.rand, 16, 17, 17, 17, 17, 17, 17, 17));
                            }
                        }
                    }
                }
            }
            else if (npc.life <= 0)
            {
                AQSound.Play(SoundType.NPCHit, "OmegaStarite/hit" + Main.rand.Next(3), npc.Center, 0.6f);
                if (skipDeathTimer > 0)
                {
                    if (NoHitManager.HasBeenNoHit(npc, Main.myPlayer))
                    {
                        NoHitManager.PlayNoHitJingle(npc.Center);
                    }
                    for (int i = 0; i < rings.Length; i++)
                    {
                        for (int j = 0; j < rings[i].amountOfSegments; j++)
                        {
                            for (int k = 0; k < 30; k++)
                            {
                                Dust.NewDust(rings[i].CachedHitboxes[j].TopLeft(), rings[i].CachedHitboxes[j].Width, rings[i].CachedHitboxes[j].Height, 58, npc.velocity.X * 0.1f, npc.velocity.Y * 0.1f, 150, default(Color), 0.8f);
                            }
                            for (float f = 0f; f < 1f; f += 0.125f)
                            {
                                Dust.NewDustPerfect(rings[i].CachedHitboxes[j].Center.ToVector2(), ModContent.DustType<MonoSparkleDust>(), Vector2.UnitY.RotatedBy(f * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (4f + Main.rand.NextFloat() * 4f), 150, Color.CornflowerBlue).noGravity = true;
                            }
                            for (float f = 0f; f < 1f; f += 0.25f)
                            {
                                Dust.NewDustPerfect(rings[i].CachedHitboxes[j].Center.ToVector2(), ModContent.DustType<MonoSparkleDust>(), Vector2.UnitY.RotatedBy(f * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (2f + Main.rand.NextFloat() * 3f), 150, Color.Gold).noGravity = true;
                            }
                            if (AQGraphics.Cull_WorldPosition(rings[i].CachedHitboxes[j]))
                            {
                                for (int k = 0; k < 7; k++)
                                {
                                    Gore.NewGore(npc.Center, Main.rand.NextVector2CircularEdge(0.5f, 0.5f) * 12f, Utils.SelectRandom(Main.rand, 16, 17, 17, 17, 17, 17, 17, 17));
                                }
                            }
                        }
                    }
                }
                for (int k = 0; k < 60; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 58, npc.velocity.X * 0.1f, npc.velocity.Y * 0.1f, 150, default(Color), 0.8f);
                }
                for (float f = 0f; f < 1f; f += 0.02f)
                {
                    Dust.NewDustPerfect(npc.Center, ModContent.DustType<MonoSparkleDust>(), Vector2.UnitY.RotatedBy(f * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (4f + Main.rand.NextFloat() * 4f), 150, Color.CornflowerBlue).noGravity = true;
                }
                for (float f = 0f; f < 1f; f += 0.05f)
                {
                    Dust.NewDustPerfect(npc.Center, ModContent.DustType<MonoSparkleDust>(), Vector2.UnitY.RotatedBy(f * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (2f + Main.rand.NextFloat() * 3f), 150, Color.Gold).noGravity = true;
                }
                if (AQGraphics.Cull_WorldPosition(npc.getRect()))
                {
                    for (int k = 0; k < 7; k++)
                    {
                        Gore.NewGore(npc.Center, Main.rand.NextVector2CircularEdge(0.5f, 0.5f) * 6f, Utils.SelectRandom(Main.rand, 16, 17, 17, 17, 17, 17, 17, 17));
                    }
                }

            }
            else
            {
                AQSound.Play(SoundType.NPCHit, "OmegaStarite/hit" + Main.rand.Next(3), npc.Center, 0.6f);
                byte shake = (byte)MathHelper.Clamp((int)(damage / 8), 4, 10);
                if (shake > _hitShake)
                {
                    _hitShake = shake;
                }
                float x = npc.velocity.X.Abs() * hitDirection;
                if (Main.rand.NextBool())
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 58);
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = Main.rand.NextFloat(2f, 6f);
                }
                if (Main.rand.NextBool(7))
                    Gore.NewGore(npc.Center, new Vector2(Main.rand.NextFloat(-4f, 4f) + x * 0.75f, Main.rand.NextFloat(-4f, 4f)), 16 + Main.rand.Next(2));
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
            return npc.ai[0] != -1;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (rings == null)
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
            drawPos.X = (int)drawPos.X;
            drawPos.Y = (int)drawPos.Y;
            var positions = new List<Vector4>(); 
            for (int i = 0; i < rings.Length; i++)
            {
                for (int j = 0; j < rings[i].amountOfSegments; j++)
                {
                    positions.Add(new Vector4((int)rings[i].CachedPositions[j].X, (int)rings[i].CachedPositions[j].Y, (int)rings[i].CachedPositions[j].Z, rings[i].scale));
                }
            }
            float intensity = AQConfigClient.c_EffectIntensity;

            if ((int)npc.ai[0] == -1)
            {
                intensity += npc.ai[1] / 20;
                int range = (int)intensity + 4;
                drawPos += new Vector2(Main.rand.Next(-range, range), Main.rand.Next(-range, range));
                for (int i = 0; i < positions.Count; i++)
                {
                    positions[i] += new Vector4(Main.rand.Next(-range, range), Main.rand.Next(-range, range), Main.rand.Next(-range, range), 0f);
                }
                ScreenShakeManager.ChannelEffect("OmegaStariteDeathScreenShake", new OmegaStariteScreenShake((int)(range * 0.8f), 0.01f, Math.Max(6 - (int)(range * 0.8), 1)));
            }
            else if (_hitShake > 0)
            {
                drawPos += new Vector2(Main.rand.Next(-_hitShake, _hitShake), Main.rand.Next(-_hitShake, _hitShake));
                _hitShake--;
            }
            positions.Sort((o, o2) => -o.Z.CompareTo(o2.Z));
            var omegiteTexture = ModContent.GetTexture(this.GetPath("_Orb"));
            var omegiteFrame = new Rectangle(0, 0, omegiteTexture.Width, omegiteTexture.Height);
            var omegiteOrigin = omegiteFrame.Size() / 2f;
            float xOff = (float)(Math.Sin(Main.GlobalTime * 3f) + 1f);
            var clr3 = new Color(50, 50, 50, 0) * (intensity - ModContent.GetInstance<AQConfigClient>().EffectIntensity + 1f);
            float deathSpotlightScale = 0f;
            if (intensity > 3f)
                deathSpotlightScale = npc.scale * (intensity - 2.1f) * ((float)Math.Sin(npc.ai[1] * 0.1f) + 1f) / 2f;
            var spotlight = AQTextures.Lights[LightTex.Spotlight66x66];
            var spotlightOrig = spotlight.Size() / 2f;
            Color spotlightColor;
            if (GlimmerEvent.stariteDiscoParty)
            {
                spotlightColor = Main.DiscoColor;
                spotlightColor.A = 0;
            }
            else
            {
                spotlightColor = ModContent.GetInstance<StariteConfig>().AuraColoring;
            }
            var drawOmegite = new List<AQGraphics.DrawMethod>();
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
            for (int i = 0; i < positions.Count; i++)
            {
                if (positions[i].Z > 0f)
                {
                    var drawPosition = AQUtils.OmegaStarite3DHelper.GetParralaxPosition(new Vector2(positions[i].X, positions[i].Y), positions[i].Z * 0.00728f) - Main.screenPosition;
                    var drawScale = AQUtils.OmegaStarite3DHelper.GetParralaxScale(positions[i].W, positions[i].Z * 0.0314f);
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
                    positions.RemoveAt(i);
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
                if (PrimitivesRenderer.ShouldDrawVertexTrails())
                {
                    var trueOldPos = new List<Vector2>();
                    for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
                    {
                        if (npc.oldPos[i] == new Vector2(0f, 0f))
                            break;
                        trueOldPos.Add(npc.oldPos[i] + offset - Main.screenPosition);
                    }
                    if (trueOldPos.Count > 1)
                    {
                        PrimitivesRenderer.ReversedGravity(trueOldPos);
                        const float radius = Circumference / 2f;
                        Vector2[] arr;
                        arr = trueOldPos.ToArray();
                        if (arr.Length > 1)
                        {
                            var trailClr = GlimmerEvent.stariteDiscoParty ? Main.DiscoColor : new Color(35, 85, 255, 120);
                            var trail = new PrimitivesRenderer(AQTextures.Trails[TrailTex.Line], PrimitivesRenderer.TextureTrail);
                            trail.PrepareVertices(arr, (p) => new Vector2(radius - p * radius), (p) => trailClr * (1f - p));
                            trail.Draw();
                        }
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
                        var trailClr = GlimmerEvent.stariteDiscoParty ? Main.DiscoColor : new Color(35, 85, 255, 120);
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
            for (int i = 0; i < positions.Count; i++)
            {
                var drawPosition = AQUtils.OmegaStarite3DHelper.GetParralaxPosition(new Vector2(positions[i].X, positions[i].Y), positions[i].Z * 0.00728f) - Main.screenPosition;
                var drawScale = AQUtils.OmegaStarite3DHelper.GetParralaxScale(positions[i].W, positions[i].Z * 0.0314f);
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

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.type == ProjectileID.FallingStar)
            {
                if (damage > 300)
                {
                    GlimmerEvent.stariteDiscoParty = true;
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
            OmegaStariteScenes.OmegaStariteIndexCache = -1;
            OmegaStariteScenes.SceneType = 3;
            return true;
        }

        public override void NPCLoot()
        {
            GlimmerEvent.deactivationDelay = 275;
            var noHitManager = npc.GetGlobalNPC<NoHitManager>();
            bool anyoneNoHit = false;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (NoHitManager.HasBeenNoHit(npc, i))
                {
                    anyoneNoHit = true;
                    AQItem.DropInstancedItem(i, npc.getRect(), ModContent.ItemType<Items.Placeable.Furniture.AStrangeIdea>());
                }
            }
            if (anyoneNoHit || Main.rand.NextBool(10))
                Item.NewItem(npc.getRect(), ModContent.ItemType<OmegaStariteTrophy>());
            if (Main.expertMode)
            {
                npc.DropBossBags();
                if (Main.netMode == NetmodeID.Server)
                {
                    int item = Item.NewItem(npc.getRect(), ModContent.ItemType<DragonBall>(), 1, noBroadcast: true);
                    Main.itemLockoutTime[item] = 54000;
                    for (int i = 0; i < 255; i++)
                    {
                        var plr = Main.player[i];
                        if (plr.active && npc.playerInteraction[i] && (!noHitManager.hitPlayer[i] || Main.rand.NextBool(4)))
                        {
                            NetMessage.SendData(MessageID.InstancedItem, i, -1, null, item);
                        }
                    }
                    Main.item[item].active = false;
                }
                else if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Item.NewItem(npc.getRect(), ModContent.ItemType<DragonBall>());
                }
            }
            else
            {
                var rect = npc.getRect();
                if (Main.rand.NextBool())
                    Item.NewItem(rect, ModContent.ItemType<CosmicTelescope>());
                int[] choices = new int[]
                {
                    ModContent.ItemType<MagicWand>(),
                    ModContent.ItemType<Raygun>(),
                };
                Item.NewItem(rect, choices[Main.rand.Next(choices.Length)]);
                Item.NewItem(rect, ModContent.ItemType<CosmicEnergy>(), Main.rand.NextVRand(2, 5));
                Item.NewItem(rect, ItemID.FallenStar, Main.rand.NextVRand(15, 20));
                Item.NewItem(rect, ItemID.SoulofFlight, Main.rand.NextVRand(2, 5));
            }
            WorldDefeats.DownedStarite = true;
            WorldDefeats.DownedGlimmer = true;
            if (GlimmerEvent.IsGlimmerEventCurrentlyActive())
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
                    WorldDefeats.ObtainedUltimateSword = true;
                    var plr = Main.player[i];
                    if (plr.active && npc.playerInteraction[i])
                        Projectile.NewProjectile(npc.Center, new Vector2(Main.rand.NextFloat(2f, 6f) * (Main.rand.NextBool() ? -1f : 1f), -18f), ModContent.ProjectileType<Projectiles.UltimateSword>(), 0, 0f, i, ModContent.ItemType<Items.Weapons.Melee.UltimateSword>());
                }
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(skipDeathTimer);

            writer.Write(rings[0].pitch);
            writer.Write(rings[0].roll);
            writer.Write(rings[0].yaw);

            writer.Write(rings[1].pitch);
            writer.Write(rings[1].roll);
            writer.Write(rings[1].yaw);

            rings[0].SendNetPackage(writer);
            rings[1].SendNetPackage(writer);
            //writer.Write(orbs.Count);
            //for (int i = 0; i < orbs.Count; i++)
            //{
            //    writer.Write(orbs[i].radius);
            //    writer.Write(orbs[i].scale);
            //    writer.Write(orbs[i].defRotation);
            //    writer.Write(orbs[i].maxRotation);
            //}
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            skipDeathTimer = reader.ReadInt32();

            rings[0].pitch = reader.ReadSingle();
            rings[0].roll = reader.ReadSingle();
            rings[0].yaw = reader.ReadSingle();

            rings[1].pitch = reader.ReadSingle();
            rings[1].roll = reader.ReadSingle();
            rings[1].yaw = reader.ReadSingle();

            rings[0].RecieveNetPackage(reader);
            rings[1].RecieveNetPackage(reader);
        }

        public ModifiableMusic GetMusic() => AQMod.OmegaStariteMusic;
    }
}