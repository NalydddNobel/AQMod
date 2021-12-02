using AQMod.Content.Dusts;
using AQMod.Items.Placeable.Banners;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters.AtmosphericEvent
{
    public class RedSprite : ModNPC
    {
        private bool _setupFrame;
        public int frameIndex;
        public const int FramesX = 3;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 8;
        }

        public override void SetDefaults()
        {
            npc.width = 46;
            npc.height = 36;
            npc.lifeMax = 2750;
            npc.damage = 45;
            npc.defense = 15;
            npc.knockBackResist = 0f;
            npc.HitSound = SoundID.NPCHit30;
            npc.DeathSound = SoundID.NPCDeath33;
            npc.aiStyle = -1;
            npc.noGravity = true;
            npc.value = Item.buyPrice(silver: 30);
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.CursedInferno] = true;
            npc.buffImmune[BuffID.Ichor] = true;
            npc.buffImmune[BuffID.ShadowFlame] = true;
            npc.buffImmune[BuffID.Bleeding] = true;
            banner = npc.type;
            bannerItem = ModContent.ItemType<RedSpriteBanner>();
            npc.noTileCollide = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            var center = npc.Center;
            if (npc.life < 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    if (Main.trackableSoundInstances[SoundID.BlizzardStrongLoop.Style].State == SoundState.Playing)
                    {
                        Main.trackableSoundInstances[SoundID.BlizzardStrongLoop.Style].Stop();
                    }
                }
                for (int i = 0; i < 50; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<RedSpriteDust>());
                    Main.dust[d].velocity = (Main.dust[d].position - center) / 8f;
                }
            }
            else
            {
                for (int i = 0; i < damage / 100; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<RedSpriteDust>());
                    Main.dust[d].velocity = (Main.dust[d].position - center) / 8f;
                }
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

        public const int PHASE_DIRECT_WIND = 1;
        public const int PHASE_SUMMON_NIMBUS = 2;

        public override void AI()
        {
            if ((int)npc.ai[0] == -1)
            {
                if (npc.timeLeft < 20)
                {
                    npc.timeLeft = 20;
                }
                npc.velocity.Y -= 0.25f;
                return;
            }
            var center = npc.Center;
            if ((int)npc.ai[0] == 0)
            {
                npc.TargetClosest(faceTarget: false);
                if (!npc.HasValidTarget)
                {
                    npc.ai[0] = -1;
                    return;
                }
                RandomizePhase();
                npc.velocity = Vector2.Normalize(Main.player[npc.target].Center - center) * 10f;
            }
            if (!npc.HasValidTarget)
            {
                npc.ai[0] = -1;
                return;
            }
            switch ((int)npc.ai[0])
            {
                case PHASE_DIRECT_WIND:
                {
                    if (npc.direction == -1)
                    {
                        if (npc.position.X > Main.player[npc.target].position.X - 100)
                        {
                            if (npc.velocity.X > -20f)
                            {
                                npc.velocity.X -= 0.8f;
                                if (npc.velocity.X > 0f)
                                {
                                    npc.velocity.X *= 0.96f;
                                }
                            }
                        }
                        else
                        {
                            if (Main.player[npc.target].position.X - npc.position.X > 500)
                            {
                                if (npc.velocity.X < 20f)
                                {
                                    npc.velocity.X += 0.4f;
                                    if (npc.velocity.X < 0f)
                                    {
                                        npc.velocity.X *= 0.96f;
                                    }
                                }
                            }
                            else
                            {
                                npc.ai[1] = 1f;
                                if (npc.velocity.X < -4f)
                                    npc.velocity.X *= 0.94f;
                            }
                        }
                    }
                    else
                    {
                        if (npc.position.X < Main.player[npc.target].position.X + 100)
                        {
                            if (npc.velocity.X < 20f)
                            {
                                npc.velocity.X += 0.8f;
                                if (npc.velocity.X < 0f)
                                {
                                    npc.velocity.X *= 0.96f;
                                }
                            }
                        }
                        else
                        {
                            if (npc.position.X - Main.player[npc.target].position.X > 500)
                            {
                                if (npc.velocity.X > -20f)
                                {
                                    npc.velocity.X -= 0.4f;
                                    if (npc.velocity.X > 0f)
                                    {
                                        npc.velocity.X *= 0.96f;
                                    }
                                }
                            }
                            else
                            {
                                npc.ai[1] = 1f;
                                if (npc.velocity.X > 4f)
                                    npc.velocity.X *= 0.94f;
                            }
                        }

                    }
                    if ((int)npc.ai[1] == 1)
                    {
                        npc.ai[3]++;
                        if (npc.ai[3] > 600f)
                        {
                            npc.ai[2] = -1f;
                            npc.ai[3] = 0f;
                            RandomizePhase();
                        }
                        if (npc.ai[2] < 0f)
                        {
                            npc.ai[2] = 0f;
                        }
                        InflictWindDebuff();
                        if (npc.direction == -1)
                        {
                            if (Main.player[npc.target].velocity.X > 2f)
                            {
                                npc.ai[2]++;
                                if (npc.ai[2] > 180f)
                                {
                                    npc.TargetClosest(faceTarget: false);
                                    npc.ai[2] = -1f;
                                    npc.ai[1] = 0f;
                                    npc.localAI[2] = npc.direction;
                                    npc.direction = 1;
                                }
                            }
                        }
                        else
                        {
                            if (Main.player[npc.target].velocity.X < -2f)
                            {
                                npc.ai[2]++;
                                if (npc.ai[2] > 180f)
                                {
                                    npc.TargetClosest(faceTarget: false);
                                    npc.ai[2] = -1f;
                                    npc.ai[1] = 0f;
                                    npc.localAI[2] = npc.direction;
                                    npc.direction = -1;
                                }
                            }
                        }

                        if (Main.netMode != NetmodeID.Server && Main.ambientVolume > 0f)
                        {
                            if (Main.trackableSoundInstances[SoundID.BlizzardStrongLoop.Style].State != SoundState.Playing)
                            {
                                Main.trackableSoundInstances[SoundID.BlizzardStrongLoop.Style].Volume = Main.ambientVolume;
                                Main.trackableSoundInstances[SoundID.BlizzardStrongLoop.Style].Play();
                            }
                        }
                    }
                    npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, (Main.player[npc.target].position.Y - 100 - center.Y) / 4f, 0.1f);
                    npc.rotation = npc.velocity.X * 0.01f;
                }
                break;

                case PHASE_SUMMON_NIMBUS:
                {
                    npc.direction = 0;
                    npc.rotation = Utils.AngleLerp(npc.rotation, 0f, 0.1f);
                    Vector2 gotoPosition = new Vector2(Main.player[npc.target].position.X + Main.player[npc.target].width / 2f, Main.player[npc.target].position.Y - 300f);
                    gotoPosition += new Vector2(40f, 0f).RotatedBy(npc.ai[3]);
                    npc.ai[2] += 0.02f;
                    if ((center - gotoPosition).Length() < 100f)
                    {
                        npc.ai[1]++;
                        if (npc.ai[1] > 90f)
                        {
                            int cloudsAmount = 8;
                            int delayBetweenCloudSpawns = 20;
                            int cloudLifespan = 240;
                            if (Main.expertMode)
                            {
                                delayBetweenCloudSpawns /= 2;
                                cloudLifespan *= 2;
                            }
                            int timer = (int)(npc.ai[1] - 90f) % 10;
                            npc.localAI[2] = timer;
                            if (timer == 0 && (Main.expertMode || npc.ai[1] <= 160f))
                            {
                                Main.PlaySound(SoundID.Item66.WithVolume(1.3f), gotoPosition);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int timer2 = (int)(npc.ai[1] - 90f) % 20;
                                    int direction = 1;
                                    if (timer2 >= 10)
                                    {
                                        direction = -1;
                                    }
                                    var projPosition = new Vector2(center.X + 900f * direction, center.Y + Main.rand.NextFloat(-120f, 60f));
                                    var velocity = Vector2.Normalize(Main.player[npc.target].Center - projPosition);
                                    int damage = 50;
                                    if (Main.expertMode)
                                    {
                                        damage = 30;
                                    }
                                    int type = ProjectileID.JestersArrow;
                                    int p = Projectile.NewProjectile(projPosition, velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * 8f, type, damage, 1f, Main.myPlayer);
                                    Main.projectile[p].hostile = true;
                                    Main.projectile[p].timeLeft = (int)(Vector2.Distance(projPosition, Main.player[npc.target].Center) / Main.projectile[p].velocity.Length());
                                }
                            }
                            if (npc.ai[1] > delayBetweenCloudSpawns * cloudsAmount + 90f)
                            {
                                npc.ai[2] = -1f;
                                npc.localAI[2] = 0f;
                                RandomizePhase();
                                npc.ai[2] = 0f;
                            }
                        }
                    }
                    else
                    {
                        npc.velocity = Vector2.Lerp(npc.velocity, Vector2.Normalize(gotoPosition - center) * 20f, 0.1f);
                    }
                }
                break;
            }

        }

        private void InflictWindDebuff()
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead && Main.player[i].Distance(npc.Center) < 2000f)
                {
                    Main.player[i].AddBuff(ModContent.BuffType<Buffs.Debuffs.RedSpriteWind>(), 4);
                    var aQPlayer = Main.player[i].GetModPlayer<AQPlayer>();
                    aQPlayer.redSpriteWind = (sbyte)-npc.direction;
                    if (npc.direction == -1)
                    {
                        if (Main.player[npc.target].velocity.X < 1f)
                        {
                            if (aQPlayer.temperatureRegen < 10 || aQPlayer.temperature == 0)
                            {
                                aQPlayer.InflictTemperature(10);
                                aQPlayer.temperatureRegen = 20;
                            }
                        }
                    }
                    else
                    {
                        if (Main.player[npc.target].velocity.X > -1f)
                        {
                            if (aQPlayer.temperatureRegen < 10 || aQPlayer.temperature == 0)
                            {
                                aQPlayer.InflictTemperature(10);
                                aQPlayer.temperatureRegen = 20;
                            }
                        }
                    }
                }
            }
        }

        private void RandomizePhase()
        {
            npc.TargetClosest(faceTarget: false);
            if (!npc.HasValidTarget)
            {
                npc.ai[0] = -1;
                return;
            }
            int oldPhase = (int)npc.ai[0];
            if (oldPhase == PHASE_DIRECT_WIND)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    if (Main.trackableSoundInstances[SoundID.BlizzardStrongLoop.Style].State == SoundState.Playing)
                    {
                        Main.trackableSoundInstances[SoundID.BlizzardStrongLoop.Style].Stop();
                    }
                }
            }
            for (int i = 0; i < 50; i++)
            {
                npc.ai[0] = Main.rand.Next(2) + 1;
                if ((int)npc.ai[0] != oldPhase)
                {
                    break;
                }
            }
            npc.ai[1] = 0f;
            npc.localAI[0] = oldPhase;
            npc.localAI[1] = npc.ai[2];
            npc.netUpdate = true;
            frameIndex = 0;
            if ((int)npc.ai[0] == 1)
            {
                npc.direction = Main.rand.NextBool() ? -1 : 1;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            if (!_setupFrame)
            {
                _setupFrame = true;
                npc.frame.Width = npc.frame.Width / FramesX;
            }

            int phase = (int)npc.ai[0];
            if ((int)npc.localAI[0] != 0)
            {
                phase = (int)npc.localAI[0];
            }
            int ai2 = (int)npc.ai[2];
            if ((int)npc.localAI[1] != 0)
            {
                ai2 = (int)npc.localAI[1];
            }
            int direction = npc.direction;
            if ((int)npc.localAI[2] > 0)
            {
                direction = (int)npc.localAI[2];
            }
            switch (phase)
            {
                default:
                {
                    npc.frameCounter += 1.0d;
                    if (npc.frameCounter > 2.0d)
                    {
                        npc.frameCounter = 0.0d;
                        frameIndex++;
                        if (frameIndex >= Main.npcFrameCount[npc.type] * FramesX)
                        {
                            frameIndex = 0;
                        }
                    }
                }
                break;

                case PHASE_DIRECT_WIND:
                {
                    if (direction == -1)
                    {
                        if (ai2 == -1)
                        {
                            npc.frameCounter += 1.0d;
                            if (npc.frameCounter > 4.0)
                            {
                                npc.frameCounter = 0.0;
                                frameIndex--;
                                if (frameIndex < 6)
                                {
                                    frameIndex = 0;
                                    npc.localAI[0] = 0f;
                                    npc.localAI[1] = 0f;
                                    npc.localAI[2] = 0f;
                                }
                            }
                        }
                        else
                        {
                            if (npc.position.X < Main.player[npc.target].position.X)
                            {
                                npc.frameCounter += 1.0d;
                            }
                            if (frameIndex == 0)
                            {
                                if (npc.frameCounter > 16.0)
                                {
                                    frameIndex = 6;
                                }
                            }
                            else
                            {
                                if (npc.frameCounter > 6.0)
                                {
                                    npc.frameCounter = 0.0;
                                    frameIndex++;
                                    if (frameIndex > 8)
                                    {
                                        frameIndex = 8;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (ai2 == -1)
                        {
                            npc.frameCounter += 1.0d;
                            if (npc.frameCounter > 4.0)
                            {
                                npc.frameCounter = 0.0;
                                frameIndex--;
                                if (frameIndex < 9)
                                {
                                    frameIndex = 0;
                                    npc.localAI[0] = 0f;
                                    npc.localAI[1] = 0f;
                                    npc.localAI[2] = 0f;
                                }
                            }
                        }
                        else
                        {
                            if (npc.position.X > Main.player[npc.target].position.X)
                            {
                                npc.frameCounter += 1.0d;
                            }
                            if (frameIndex == 0)
                            {
                                if (npc.frameCounter > 16.0)
                                {
                                    frameIndex = 9;
                                }
                            }
                            else
                            {
                                if (npc.frameCounter > 6.0)
                                {
                                    npc.frameCounter = 0.0;
                                    frameIndex++;
                                    if (frameIndex > 11)
                                    {
                                        frameIndex = 11;
                                    }
                                }
                            }
                        }
                    }
                }
                break;

                case PHASE_SUMMON_NIMBUS:
                {
                    if (ai2 == -1)
                    {
                        npc.frameCounter += 1.0d;
                        if (npc.frameCounter > 4.0)
                        {
                            npc.frameCounter = 0.0;
                            frameIndex--;
                            if (frameIndex < 0)
                            {
                                frameIndex = 0;
                                npc.localAI[0] = 0f;
                                npc.localAI[1] = 0f;
                            }
                        }
                    }
                    else
                    {
                        if (npc.ai[1] > 20f)
                        {
                            npc.frameCounter += 1.0d;
                            if (npc.frameCounter > 4.0)
                            {
                                npc.frameCounter = 0.0;
                                frameIndex++;
                                if (frameIndex > 4)
                                {
                                    frameIndex = 4;
                                }
                            }
                        }
                    }
                }
                break;
            }

            npc.frame.Y = frameIndex * frameHeight;

            if (npc.frame.Y >= frameHeight * Main.npcFrameCount[npc.type])
            {
                npc.frame.X = npc.frame.Width * (npc.frame.Y / (frameHeight * Main.npcFrameCount[npc.type]));
                npc.frame.Y = npc.frame.Y % (frameHeight * Main.npcFrameCount[npc.type]);
            }
            else
            {
                npc.frame.X = 0;
            }
        }

        public override void NPCLoot()
        {
            Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Materials.Energies.AtmosphericEnergy>(), Main.rand.Next(2) + 2);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            var texture = Main.npcTexture[npc.type];
            var drawPosition = npc.Center;
            var origin = new Vector2(npc.frame.Width / 2f, npc.frame.Height / 2f - 14f);
            Vector2 scale = new Vector2(npc.scale, npc.scale);
            float speedX = npc.velocity.X.Abs();
            if (speedX > 8f)
            {
                scale.X += (speedX - 8f) / 120f;
                drawPosition.X -= (scale.X - 1f) * 16f;
            }
            float electric = 3f + (float)Math.Sin(Main.GlobalTime * 5f);
            if ((int)npc.ai[0] == PHASE_SUMMON_NIMBUS)
            {
                electric += npc.localAI[2] / 2f;
            }
            if (electric > 0f)
            {
                for (int i = 0; i < 8; i++)
                {
                    Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition + new Vector2(electric, 0f).RotatedBy(MathHelper.PiOver4 * i), npc.frame, new Color(150, 255, 0, 20), npc.rotation, origin, scale, SpriteEffects.None, 0f);
                }
            }
            Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, npc.frame, drawColor, npc.rotation, origin, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(this.GetTextureobj("_Glow"), drawPosition - Main.screenPosition, npc.frame, Color.White, npc.rotation, origin, scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}