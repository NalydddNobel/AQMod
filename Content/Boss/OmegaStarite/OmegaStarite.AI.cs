using Aequus.Biomes;
using Aequus.Content.Boss.OmegaStarite.Projectiles;
using Aequus.Items.Fishing.QuestFish;
using Aequus.NPCs.GlobalNPCs;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.Boss.OmegaStarite
{
    public partial class OmegaStarite
    {
        public const int ACTION_LASER_ORBITAL_2 = 8;
        public const int ACTION_LASER_ORBITAL_1 = 7;
        public const int ACTION_STARS = 6;
        public const int ACTION_ASSAULT = 5;
        public const int ACTION_ORBITAL_3 = 4;
        public const int ACTION_ORBITAL_2 = 3;
        public const int ACTION_ORBITAL_1 = 2;
        public const int ACTION_UNUSED = -2;
        public const int ACTION_DEAD = -3;

        public void Assault(Vector2 center, Vector2 plrCenter, Player player)
        {
            LerpToDefaultRotationVelocity();

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
                        return;
                    }
                }
                NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(plrCenter - center) * NPC.ai[2], 0.02f);
            }
            else
            {
                if (!PlrCheck())
                    return;
                if (NPC.ai[1] <= 60f || Vector2.Distance(new Vector2(NPC.ai[1], NPC.ai[2]), plrCenter) > 600f)
                {
                    NPC.ai[1] = plrCenter.X + player.velocity.X * 20f;
                    NPC.ai[2] = plrCenter.Y + player.velocity.Y * 20f;
                }
                if ((center - new Vector2(NPC.ai[1], NPC.ai[2])).Length() < DIAMETER)
                {
                    NPC.ai[3]++;
                    if (NPC.ai[3] > 5)
                    {
                        NPC.ai[0] = ACTION_ORBITAL_1;
                        NPC.ai[1] = 0f;
                        NPC.ai[3] = 0f;
                    }
                    else
                    {
                        if (NPC.ai[3] >= 5)
                        {
                            SoundEngine.PlaySound(ATK_Transition, NPC.Center);
                        }
                        NPC.ai[1] = -NPC.ai[3] * 16;
                        float lifePercent = NPC.life / (float)NPC.lifeMax;
                        if (Main.getGoodWorld || (Main.expertMode && lifePercent < 0.75f) || lifePercent < 0.6f)
                        {
                            SoundEngine.PlaySound(ATK_Stars, NPC.Center);
                            ScreenShake.SetShake(20f, 0.9f, NPC.Center);
                            int type = ModContent.ProjectileType<OmegaStariteBullet>();
                            float speed2 = Main.expertMode ? 12.5f : 5.5f;
                            int damage = 35;
                            if (Main.expertMode)
                                damage = 30;
                            float rot = MathHelper.TwoPi / (Main.getGoodWorld ? 10f : 5f) + 0.01f;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (float f = 0f; f < MathHelper.TwoPi; f += rot)
                                {
                                    var v = (f - MathHelper.PiOver2).ToRotationVector2();
                                    int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), center + v * RADIUS, v * speed2, type, damage, 1f, player.whoAmI, -60f, speed2);
                                    Main.projectile[p].timeLeft += 120;
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

        public void Intro(Vector2 center, Vector2 plrCenter)
        {
            if (NPC.ai[2] == 0f)
            {
                NPC.ai[2] = plrCenter.Y;
            }
            LerpToDefaultRotationVelocity();
            if (center.Y > NPC.ai[2])
            {
                int[] choices = new int[] { ACTION_ORBITAL_1, ACTION_ASSAULT };
                NPC.ai[0] = choices[Main.rand.Next(choices.Length)];
                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
                NPC.netUpdate = true;
            }
            else
            {
                float fallSpeed = Main.getGoodWorld ? 56f : 36f;
                NPC.velocity.X = 0f;
                NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, fallSpeed, 0.025f);
            }
        }

        public void Die()
        {
            for (int i = 0; i < rings.Count; i++)
            {
                rings[i].rotationVelocity *= 0f;
            }
            if (NPC.ai[1] > 20f && NPC.ai[1] < DEATHTIME * 1f)
            {
                for (int i = 0; i < NPC.ai[1] / 40f; i++)
                {
                    var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Unit() * NPC.ai[1] * Main.rand.NextFloat(0.2f, 3f) * 3f, ModContent.DustType<MonoDust>(), newColor: Color.Lerp(Color.HotPink, Color.White, Math.Min(Main.rand.NextFloat(1f) - NPC.ai[1] / 10f, 1f)).UseA(0));
                    d.velocity *= 0.2f;
                    d.velocity += (NPC.Center - d.position) / 8f;
                    d.scale = Main.rand.NextFloat(0.3f, 2f + NPC.ai[1] / 30f);
                    d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                }
            }
            NPC.ai[1] += 0.5f;
            if ((int)(NPC.ai[1] * 2f) == 40)
            {
                SoundEngine.PlaySound(Dead_1 with { Volume = 0.66f, Pitch = -0.05f, }, NPC.Center);
            }
            if (NPC.ai[1] > DEATHTIME * 1.314f)
            {
                NPC.life = -33333;
                NPC.HitEffect();
                NPC.checkDead();
            }
        }

        public void Goodbye()
        {
            if (NPC.timeLeft > 120)
                NPC.timeLeft = 120;
            NPC.velocity.X *= 0.975f;
            NPC.velocity.Y -= 0.2f;

            rings[0].yaw += 0.0314f;
            rings[0].roll += 0.0157f;
            rings[0].pitch += 0.01f;
            rings[1].yaw += 0.0157f;
            rings[1].roll += 0.0314f;
            rings[1].pitch += 0.011f;
        }

        private void Initalize(bool bestiaryDummy = false)
        {
            if (!bestiaryDummy)
                NPC.TargetClosest(faceTarget: false);
            else if (!Main.getGoodWorld)
                NPC.scale *= 0.5f;
            SetupRings();
            if (Main.netMode != NetmodeID.MultiplayerClient && !NPC.IsABestiaryIconDummy)
            {
                int damage = Main.expertMode ? 30 : 60;
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero,
                    ModContent.ProjectileType<OmegaStariteProj>(), damage, 1f, Main.myPlayer, NPC.whoAmI + 1);
            }
        }

        private bool PlrCheck()
        {
            NPC.TargetClosest(faceTarget: false);
            NPC.netUpdate = true;
            if (!NPC.HasValidTarget || NPC.Distance(Main.player[NPC.target].Center) > 4000f)
            {
                NPC.ai[0] = ACTION_GOODBYE;
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

        private void LerpToDefaultRotationVelocity()
        {
            rings[0].rotationVelocity = Vector3.Lerp(rings[0].rotationVelocity, new Vector3(0.01f, 0.0157f, 0.0314f), 0.1f);
            rings[1].rotationVelocity = Vector3.Lerp(rings[1].rotationVelocity, new Vector3(0.011f, 0.0314f, 0.0157f), 0.1f);
            if (rings.Count > 2)
            {
                rings[2].rotationVelocity = Vector3.Lerp(rings[2].rotationVelocity, new Vector3(0.012f, 0.0186f, 0.0214f), 0.1f);
            }
        }

        private bool ShootProjsFromRing(bool endingPhase, OmegaStariteRing ring)
        {
            int delay = Main.expertMode ? 12 : 60;
            if (!endingPhase && Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > 1000f)
            {
                delay /= 2;
            }
            if (NPC.localAI[0] > delay)
            {
                if (Main.getGoodWorld || NPC.life / (float)NPC.lifeMax < 0.75f)
                {
                    float speed = 7.5f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        var diff = new Vector2(ring.CachedPositions[0].X, ring.CachedPositions[0].Y) - NPC.Center;
                        var shootDir = Vector2.Normalize(diff).RotatedBy(MathHelper.PiOver2) * speed;
                        int type = ModContent.ProjectileType<OmegaStariteBullet>();
                        int damage = 12;
                        if (Main.expertMode)
                            damage = 9;
                        for (int i = 0; i < ring.OrbCount; i++)
                        {
                            float rot = ring.rotationOrbLoop * i;
                            var position = NPC.Center + diff.RotatedBy(rot);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), position, shootDir.RotatedBy(rot), type, damage, 1f, Main.myPlayer);
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public void CullRingRotations()
        {
            for (int i = 0; i < rings.Count; i++)
            {
                rings[i].pitch %= MathHelper.TwoPi;
                rings[i].roll %= MathHelper.TwoPi;
            }
        }

        public void ResetRingsRadiusFromOrigin()
        {
            for (int i = 0; i < rings.Count; i++)
            {
                rings[i].radiusFromOrigin = rings[i].OriginalRadiusFromOrigin;
            }
        }

        public void PullInRingsTransition()
        {
            bool allRingsSet = true;
            float[] transitionSpeed = new float[rings.Count];
            transitionSpeed[0] = MathHelper.Pi;
            for (int i = 1; i < rings.Count; i++)
            {
                transitionSpeed[i] = MathHelper.PiOver2 * (3f + 2.5f * i);
            }
            for (int i = 0; i < rings.Count; i++)
            {
                if (rings[i].radiusFromOrigin > rings[i].OriginalRadiusFromOrigin)
                {
                    rings[i].radiusFromOrigin -= transitionSpeed[i];
                    allRingsSet = false;
                }
            }

            if (allRingsSet && Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < rings.Count; i++)
                {
                    rings[i].radiusFromOrigin = rings[i].OriginalRadiusFromOrigin;
                }
                if (PlrCheck())
                {
                    var choices = new List<int>
                    {
                        ACTION_ASSAULT,
                    };
                    SoundEngine.PlaySound(ATK_Transition, NPC.Center);
                    if (NPC.life / (float)NPC.lifeMax < (Main.expertMode ? 0.5f : 0.33f))
                        choices.Add(ACTION_STARS);
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
                    NPC.localAI[1] = 0f;
                    NPC.netUpdate = true;
                }
            }
        }

        public override void AI()
        {
            AI_CheckDaytime();
            PassivelyKillFallenStars();
            SpawnsManager.ForceZen(NPC);
            GlimmerBiome.omegaStarite = NPC.whoAmI;
            var center = NPC.Center;
            var player = Main.player[NPC.target];
            var plrCenter = player.Center;
            float speed = NPC.velocity.Length();
            switch ((int)NPC.ai[0])
            {
                default:
                    {

                        LerpToDefaultRotationVelocity();
                        NPC.Center = plrCenter + new Vector2(0f, -DIAMETER * 2f);
                    }
                    break;

                case ACTION_LASER_ORBITAL_2:
                    {
                        if (NPC.ai[1] == 0f)
                        {
                            CullRingRotations();
                        }
                        NPC.ai[1] += 0.0002f;
                        bool allRingsSet = true;

                        rings[0].rotationVelocity *= 0.95f;

                        rings[0].pitch = rings[0].pitch.AngleLerp(MathHelper.PiOver2, 0.025f);
                        rings[0].roll = rings[0].roll.AngleLerp(-MathHelper.PiOver2, 0.025f);

                        if (!rings[0].pitch.CloseEnough(MathHelper.PiOver2, 0.314f) || !rings[0].roll.CloseEnough(-MathHelper.PiOver2, 0.314f))
                        {
                            allRingsSet = false;
                        }
                        for (int i = 1; i < rings.Count; i++)
                        {
                            rings[i].rotationVelocity *= 0.95f;

                            rings[i].pitch = rings[i].pitch.AngleLerp(0f, 0.025f);
                            rings[i].roll = rings[i].roll.AngleLerp(0f, 0.025f);
                            if (allRingsSet && (!rings[i].pitch.CloseEnough(0f, 0.314f) || !rings[i].roll.CloseEnough(0f, 0.314f)))
                            {
                                allRingsSet = false;
                            }
                        }

                        if (NPC.ai[1] > 0.0314f)
                        {
                            if (allRingsSet)
                            {
                                NPC.velocity = Vector2.Normalize(plrCenter - center) * NPC.velocity.Length();
                                rings[0].pitch = MathHelper.PiOver2;
                                rings[0].roll = -MathHelper.PiOver2;
                                for (int i = 1; i < rings.Count; i++)
                                {
                                    rings[i].pitch = 0f;
                                    rings[i].roll = 0f;
                                }
                                if (PlrCheck())
                                {
                                    NPC.ai[0] = ACTION_LASER_ORBITAL_1;
                                    NPC.ai[1] = 0f;
                                    NPC.ai[3] = 3f + (1f - NPC.life / (float)NPC.lifeMax) * 1.5f;
                                }
                            }
                        }
                        else
                        {
                            rings[0].yaw += 0.0314f - NPC.ai[1];
                            for (int i = 1; i < rings.Count; i++)
                            {
                                rings[i].yaw += 0.0157f - NPC.ai[1] * 0.5f;
                            }
                        }
                    }
                    break;

                case ACTION_LASER_ORBITAL_1:
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
                            rings[0].yaw += NPC.ai[1];
                            rings[1].yaw += NPC.ai[1] * 0.5f;
                            bool ringsSet = false;
                            if (rings[1].radiusFromOrigin > rings[1].OriginalRadiusFromOrigin)
                            {
                                rings[1].radiusFromOrigin -= MathHelper.PiOver2 * 3f;
                                NPC.localAI[0] += 1f + NPC.ai[1] * 10f;
                                if (Main.getGoodWorld)
                                {
                                    bool shot = false;
                                    for (int i = 0; i < rings.Count; i++)
                                    {
                                        shot |= ShootProjsFromRing(endingPhase: true, rings[i]);
                                    }
                                    if (shot)
                                    {
                                        SoundEngine.PlaySound(ATK_OrbitalStars, NPC.Center);
                                        NPC.localAI[0] = 0f;
                                    }
                                }
                                else if (Vector2.Distance(plrCenter, center) > rings[0].radiusFromOrigin)
                                {
                                    if (ShootProjsFromRing(endingPhase: true, rings[1]))
                                    {
                                        SoundEngine.PlaySound(ATK_OrbitalStars, NPC.Center);
                                        NPC.localAI[0] = 0f;
                                    }
                                }
                            }
                            else
                            {
                                ringsSet = true;
                            }
                            for (int i = 2; i < rings.Count; i++)
                            {
                                rings[i].radiusFromOrigin -= MathHelper.PiOver2 * 3f;
                                if (rings[i].radiusFromOrigin > rings[i].OriginalRadiusFromOrigin)
                                {
                                    ringsSet = false;
                                }
                            }
                            if (ringsSet)
                            {
                                ResetRingsRadiusFromOrigin();
                                if (PlrCheck())
                                {
                                    SoundEngine.PlaySound(ATK_Transition, NPC.Center);
                                    var choices = new List<int>
                                    {
                                        ACTION_ASSAULT,
                                        ACTION_STARS,
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
                            NPC.ai[2] = 1200f;
                            rings[0].yaw += NPC.ai[1];
                            for (int i = 1; i < rings.Count; i++)
                            {
                                rings[i].yaw += NPC.ai[1] * 0.5f;
                            }
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
                            rings[0].yaw += NPC.ai[1];
                            for (int i = 1; i < rings.Count; i++)
                            {
                                rings[i].yaw += NPC.ai[1] * 0.5f;
                                rings[i].radiusFromOrigin = MathHelper.Lerp(rings[i].radiusFromOrigin, rings[i].OriginalRadiusFromOrigin * (NPC.ai[3] + i), 0.025f);
                            }
                            if ((int)NPC.ai[2] == 5)
                            {
                                SoundEngine.PlaySound(ATK_LaserCharge, NPC.Center);
                            }
                            if (NPC.ai[2] > 100f)
                            {
                                if (NPC.localAI[1] == 0f)
                                {
                                    if (PlrCheck())
                                    {
                                        NPC.localAI[1] = 1f;
                                        if (Main.netMode != NetmodeID.Server)
                                        {
                                            ScreenShake.SetShake(70f, where: NPC.Center);
                                            Flash(NPC.Center, 0.66f);
                                            SoundEngine.PlaySound(ATK_LaserAmbient, NPC.Center);
                                        }
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), center, new Vector2(0f, 0f), ModContent.ProjectileType<OmegaStariteDeathray>(), 100, 1f, Main.myPlayer, NPC.whoAmI);
                                            if (Main.getGoodWorld)
                                            {
                                                p = Projectile.NewProjectile(NPC.GetSource_FromAI(), center, new Vector2(0f, 0f), ModContent.ProjectileType<OmegaStariteDeathray>(), 100, 1f, Main.myPlayer, NPC.whoAmI);
                                                ((OmegaStariteDeathray)Main.projectile[p].ModProjectile).rotationOffset = MathHelper.Pi;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                if (rings[0].roll > MathHelper.PiOver2 * 6f)
                                {
                                    NPC.localAI[2] -= Main.expertMode ? 0.001f : 0.00045f;
                                }
                                else
                                {
                                    NPC.localAI[2] += Main.expertMode ? 0.00015f : 0.000085f;
                                }
                                NPC.localAI[0] += 1f + NPC.ai[1] * 10f;
                                if (Main.getGoodWorld)
                                {
                                    bool shot = false;
                                    for (int i = 1; i < rings.Count; i++)
                                    {
                                        shot |= ShootProjsFromRing(endingPhase: false, rings[i]);
                                    }
                                    if (shot)
                                    {
                                        SoundEngine.PlaySound(ATK_OrbitalStars, NPC.Center);
                                        NPC.localAI[0] = 0f;
                                    }
                                }
                                else if (Vector2.Distance(plrCenter, center) > rings[0].radiusFromOrigin)
                                {
                                    if (ShootProjsFromRing(endingPhase: false, rings[1]))
                                    {
                                        SoundEngine.PlaySound(ATK_OrbitalStars, NPC.Center);
                                        NPC.localAI[0] = 0f;
                                    }
                                }
                                rings[0].roll += NPC.localAI[2];
                                if (NPC.soundDelay <= 0)
                                {
                                    NPC.soundDelay = 60;
                                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalIdleLoop, NPC.Center);
                                }
                                if (NPC.soundDelay > 0)
                                    NPC.soundDelay--;
                                if (rings[0].roll > MathHelper.PiOver2 * 7f)
                                {
                                    NPC.soundDelay = 0;
                                    SoundEngine.FindActiveSound(ATK_LaserAmbient)?.Stop();
                                    SoundEngine.PlaySound(ATK_LaserEnd, NPC.Center);
                                    NPC.ai[2] = 1200f;
                                    rings[0].roll = -MathHelper.PiOver2;
                                }
                            }
                            else
                            {
                                const int width = (int)(DIAMETER * 2f);
                                const int height = 900;
                                Vector2 dustPos = center + new Vector2(-width / 2f, 0f);
                                Dust.NewDust(dustPos, width, height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, GlimmerBiome.CosmicEnergyColor, 2f);
                                Dust.NewDust(dustPos, width, height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, GlimmerBiome.CosmicEnergyColor, 2f);
                                Dust.NewDust(dustPos, width, height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, GlimmerBiome.CosmicEnergyColor, 2f);
                            }
                        }
                    }
                    break;

                case ACTION_STARS:
                    {
                        LerpToDefaultRotationVelocity();

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
                                Shake(30f, 0.92f);
                                SoundEngine.PlaySound(ATK_StarBarrage, NPC.Center);

                                int type = ModContent.ProjectileType<OmegaStariteBullet>();
                                float speed2 = Main.expertMode ? 12.5f : 5.5f;
                                int damage = 35;
                                if (Main.expertMode)
                                    damage = 30;
                                float rot = MathHelper.TwoPi / (Main.getGoodWorld ? 10f : 5f);
                                for (int i = 0; i < (Main.getGoodWorld ? 3 : 2); i++)
                                {
                                    for (float f = 0f; f < MathHelper.TwoPi; f += rot)
                                    {
                                        var v = (f - MathHelper.PiOver2).ToRotationVector2();
                                        int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), center + v * RADIUS, v * speed2, type, damage, 1f, player.whoAmI, -60f, speed2);
                                        Main.projectile[p].timeLeft += 120;
                                    }
                                    speed2 *= 1.2f;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        float distance = (center - plrCenter).Length();
                        if (distance > DIAMETER * 3.75f)
                        {
                            NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(plrCenter - center) * NPC.ai[2], 0.02f);
                        }
                        else if (distance < DIAMETER * 2.25f)
                        {
                            NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(center - plrCenter) * NPC.ai[2], 0.02f);
                        }
                        else
                        {
                            NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(plrCenter - center).RotatedBy(MathHelper.PiOver2) * NPC.ai[2], 0.02f);
                        }

                        if (NPC.ai[1] > 480f)
                        {
                            NPC.ai[0] = ACTION_ORBITAL_1;
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                            NPC.ai[3] = 0f;
                        }
                    }
                    break;

                case ACTION_ASSAULT:
                    Assault(center, plrCenter, player);
                    break;

                case ACTION_ORBITAL_3:
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
                            rings[0].yaw += NPC.ai[1];
                            for (int i = 1; i < rings.Count; i++)
                            {
                                rings[i].yaw += NPC.ai[1] * 0.5f;
                            }

                            PullInRingsTransition();
                        }
                        else if ((center - plrCenter).Length() > 1800f)
                        {
                            NPC.ai[2] = 300f;
                            rings[0].yaw += NPC.ai[1];
                            for (int i = 1; i < rings.Count; i++)
                            {
                                rings[i].yaw += NPC.ai[1] * 0.5f;
                            }
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
                            rings[0].yaw += NPC.ai[1];
                            rings[0].radiusFromOrigin = MathHelper.Lerp(rings[0].radiusFromOrigin, rings[0].OriginalRadiusFromOrigin * NPC.ai[3], 0.025f);
                            for (int i = 1; i < rings.Count; i++)
                            {
                                rings[i].yaw += NPC.ai[1] * 0.5f;
                                rings[i].radiusFromOrigin = MathHelper.Lerp(rings[i].radiusFromOrigin, rings[i].OriginalRadiusFromOrigin * (NPC.ai[3] + i), 0.025f);
                            }
                            if (NPC.ai[2] > 100f)
                            {
                                NPC.localAI[0] += 1f + NPC.ai[1] * 10f;
                                if (Main.getGoodWorld)
                                {
                                    bool shot = false;
                                    for (int i = 0; i < rings.Count; i++)
                                    {
                                        shot |= ShootProjsFromRing(endingPhase: false, rings[i]);
                                    }
                                    if (shot)
                                    {
                                        SoundEngine.PlaySound(ATK_OrbitalStars, NPC.Center);
                                        NPC.localAI[0] = 0f;
                                    }
                                }
                                else if (Vector2.Distance(plrCenter, center) > rings[0].radiusFromOrigin)
                                {
                                    if (ShootProjsFromRing(endingPhase: false, rings[1]))
                                    {
                                        SoundEngine.PlaySound(ATK_OrbitalStars, NPC.Center);
                                        NPC.localAI[0] = 0f;
                                    }
                                }
                            }
                        }
                    }
                    break;

                case ACTION_ORBITAL_2:
                    {
                        if (NPC.ai[1] == 0f)
                        {
                            rings[0].pitch %= MathHelper.Pi;
                            rings[0].roll %= MathHelper.Pi;
                            rings[1].pitch %= MathHelper.Pi;
                            rings[1].roll %= MathHelper.Pi;
                        }
                        NPC.ai[1] += 0.0002f;

                        bool allRingsSet = true;
                        for (int i = 0; i < rings.Count; i++)
                        {
                            rings[i].rotationVelocity *= 0.95f;
                            rings[i].pitch = rings[i].pitch.AngleLerp(0f, 0.025f);
                            rings[i].roll = rings[i].roll.AngleLerp(0f, 0.025f);
                            if (allRingsSet && (!rings[i].pitch.CloseEnough(0f, 0.314f) || !rings[i].roll.CloseEnough(0f, 0.314f)))
                            {
                                allRingsSet = false;
                            }
                        }
                        if (NPC.ai[1] > 0.0314f)
                        {
                            if (allRingsSet)
                            {
                                NPC.velocity = Vector2.Normalize(plrCenter - center) * NPC.velocity.Length();
                                for (int i = 0; i < rings.Count; i++)
                                {
                                    rings[i].pitch = 0f;
                                    rings[i].roll = 0f;
                                }
                                if (PlrCheck())
                                {
                                    NPC.ai[0] = ACTION_ORBITAL_3;
                                    NPC.ai[1] = 0f;
                                    NPC.ai[3] = 3f + (1f - NPC.life / (float)NPC.lifeMax) * 1.5f;
                                }
                            }
                        }
                        else
                        {
                            rings[0].yaw += 0.0314f - NPC.ai[1];
                            for (int i = 1; i < rings.Count; i++)
                            {
                                rings[i].yaw += 0.0157f - NPC.ai[1] * 0.5f;
                            }
                        }
                    }
                    break;

                case ACTION_ORBITAL_1:
                    {
                        LerpToDefaultRotationVelocity();
                        if (NPC.ai[1] == 0f)
                        {
                            if (PlrCheck())
                            {
                                NPC.ai[1] = plrCenter.X + player.velocity.X * 20f;
                                NPC.ai[2] = plrCenter.Y + player.velocity.Y * 20f;
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                break;
                            }
                        }
                        if ((center - new Vector2(NPC.ai[1], NPC.ai[2])).Length() < DIAMETER)
                        {
                            if (NPC.velocity.Length() < 2f)
                            {
                                ResetRingsRadiusFromOrigin();
                                if (PlrCheck())
                                {
                                    NPC.velocity *= 0.1f;
                                    if (NPC.life / (float)NPC.lifeMax < 0.5f)
                                    {
                                        NPC.ai[0] = ACTION_LASER_ORBITAL_2;
                                    }
                                    else
                                    {
                                        NPC.ai[0] = ACTION_ORBITAL_2;
                                    }
                                    NPC.ai[1] = 0f;
                                    NPC.ai[2] = 0f;
                                }
                            }
                            else
                            {
                                NPC.velocity *= 0.925f;
                            }
                        }
                        else
                        {
                            NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(new Vector2(NPC.ai[1], NPC.ai[2]) - center) * 30f, 0.025f);
                        }
                    }
                    break;

                case ACTION_INTRO:
                    var r = NPC.getRect();
                    r.Inflate(24, 24);
                    for (int i = 0; i < Main.maxItems; i++)
                    {
                        if (Main.item[i].active && Main.item[i].type == ItemID.Burger)
                        {
                            if (r.Intersects(Main.item[i].getRect()))
                            {
                                NPC.ai[0] = -100f;
                                NPC.ai[1] = i;
                                Main.item[i].noGrabDelay = 120;
                                NPC.ai[2] = 0f;
                                NPC.ai[3] = 0f;
                                NPC.velocity *= 0.5f;
                                NPC.netUpdate = true;
                                return;
                            }
                        }
                    }
                    Intro(center, plrCenter);
                    break;

                case ACTION_INIT:
                    int target = NPC.target;
                    if (!NPC.HasValidTarget)
                    {
                        NPC.TargetClosest(faceTarget: false);
                        target = NPC.target;
                    }
                    Initalize();
                    NPC.netUpdate = true;
                    NPC.target = target;
                    NPC.ai[0] = ACTION_INTRO;
                    NPC.ai[2] = plrCenter.Y - DIAMETER * 2.5f;
                    break;

                case ACTION_GOODBYE:
                    Goodbye();
                    break;

                case ACTION_DEAD:
                    Die();
                    break;

                case -100:
                    {
                        int item = (int)NPC.ai[1];
                        if (!Main.item[item].active)
                        {
                            SoundEngine.PlaySound(ATK_Transition, NPC.Center);
                            NPC.ai[0] = ACTION_ASSAULT;
                            return;
                        }
                        Main.item[item].noGrabDelay = 120;
                        NPC.velocity *= 0.95f;
                        for (int i = 0; i < rings.Count; i++)
                        {
                            rings[i].rotationVelocity *= 0.99f;
                        }
                        NPC.ai[3]++;
                        NPC.localAI[3]++;
                        if ((int)NPC.localAI[3] == 100)
                            SoundEngine.PlaySound(Dead_1 with { Volume = 0.66f, Pitch = -0.05f, }, NPC.Center);
                        if (NPC.ai[3] > 60f)
                        {
                            Main.item[item].velocity = Vector2.Zero;
                            if (NPC.ai[3] < 1000f)
                            {
                                float amt = (float)Math.Sin((NPC.ai[3] * 0.02f - 60f) * 0.5f) * 2f;
                                NPC.ai[2] += amt;
                                if (amt < -1f)
                                {
                                    NPC.ai[3] = 1000f;
                                }
                            }
                            else
                            {
                                if (NPC.ai[2] < 10f)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<BrickFish>(), 1);
                                        if (Main.netMode == NetmodeID.SinglePlayer)
                                        {
                                            Main.NewText(Language.GetTextValue("Mods.Aequus.OmegaStariteEasterEgg", NPC.TypeName), TextHelper.BossSummonMessage);
                                        }
                                        else
                                        {
                                            TextHelper.Broadcast("Mods.Aequus.OmegaStariteEasterEgg", TextHelper.BossSummonMessage, Lang.GetNPCName(Type).Key);
                                        }
                                    }
                                    Main.item[item].stack--;
                                    if (Main.item[item].stack <= 0)
                                    {
                                        Main.item[item].TurnToAir();
                                        Main.item[item].active = false;
                                    }
                                    SoundEngine.PlaySound(SoundID.Item2, NPC.Center);
                                    NPC.life = -33333;
                                    NPC.KillEffects();
                                    return;
                                }
                                else
                                {
                                    NPC.ai[2] -= 1f + (NPC.ai[3] - 1000f) * 0.01f;
                                }
                            }
                            Main.item[item].Center = Vector2.Lerp(
                                Main.item[item].Center,
                                NPC.Center + Vector2.Normalize(Main.item[item].Center - NPC.Center) * NPC.ai[2],
                                0.075f);
                        }
                        else
                        {
                            NPC.ai[2] = Vector2.Distance(NPC.Center, Main.item[item].Center);
                        }
                    }
                    break;
            }
            for (int i = 0; i < rings.Count; i++)
            {
                rings[i].Update(center + NPC.velocity);
            }
            if (NPC.ai[0] != ACTION_DEAD && Action != -100)
            {
                int chance = 10 - (int)speed;
                if (chance < 2 || Main.rand.NextBool(chance))
                {
                    if (speed < 2f)
                    {
                        var spawnPos = new Vector2(RADIUS, 0f);
                        int d = Dust.NewDust(center + spawnPos.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi)), 2, 2, DustID.MagicMirror);
                        Main.dust[d].velocity = Vector2.Normalize(spawnPos - center) * speed * 0.25f;
                    }
                    else
                    {
                        var spawnPos = new Vector2(RADIUS, 0f).RotatedBy(NPC.velocity.ToRotation() - MathHelper.Pi);
                        int d = Dust.NewDust(NPC.Center + spawnPos.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)), 2, 2, DustID.MagicMirror);
                        Main.dust[d].velocity = -NPC.velocity * 0.25f;
                    }
                }
                if (Main.rand.NextBool(30))
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Enchanted_Pink);
                    Main.dust[d].velocity.X = Main.rand.NextFloat(-4f, 4f);
                    Main.dust[d].velocity.Y = Main.rand.NextFloat(-4f, 4f);
                }
                if (Main.rand.NextBool(30))
                {
                    int g = Gore.NewGore(new EntitySource_HitEffect(NPC), NPC.position + new Vector2(Main.rand.Next(NPC.width - 4), Main.rand.Next(NPC.height - 4)), new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f)), 16 + Main.rand.Next(2));
                    Main.gore[g].scale *= 0.6f;
                }
            }
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }
            Lighting.AddLight(NPC.Center, new Vector3(1.2f, 1.2f, 2.2f));
            for (int i = 0; i < rings.Count; i++)
            {
                for (int j = 0; j < rings[i].OrbCount; j++)
                {
                    Lighting.AddLight(new Vector2(rings[i].CachedPositions[i].X, rings[i].CachedPositions[i].Y), new Vector3(0.4f, 0.4f, 1f));
                }
            }
        }
    }
}