using AQMod.Common;
using AQMod.Common.CrossMod.BossChecklist;
using AQMod.Common.Graphics.Particles;
using AQMod.Content.Dusts;
using AQMod.Items.Placeable.Banners;
using AQMod.Localization;
using AQMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters.GaleStreams
{
    [AutoloadBossHead()]
    public class SpaceSquid : ModNPC, ISetupContentType
    {
        private bool _setupFrame;
        public int frameIndex;
        public const int FramesX = 8;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.width = 80;
            npc.height = 120;
            npc.lifeMax = 4000;
            npc.damage = 45;
            npc.defense = 15;
            npc.knockBackResist = 0f;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.aiStyle = -1;
            npc.noGravity = true;
            npc.value = Item.buyPrice(gold: 2);
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.CursedInferno] = true;
            npc.buffImmune[BuffID.Ichor] = true;
            npc.buffImmune[BuffID.ShadowFlame] = true;
            npc.buffImmune[BuffID.Bleeding] = true;
            banner = npc.type;
            bannerItem = ModContent.ItemType<RedSpriteBanner>();
            npc.noTileCollide = true;

            var aQNPC = npc.GetGlobalNPC<AQNPC>();
            aQNPC.temperature = -40;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            var center = npc.Center;
            if (npc.life < 0)
            {
                for (int i = 0; i < 60; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<SpaceSquidBlood>(), 0f, 0f, 0, default(Color), Main.rand.NextFloat(0.8f, 1.3f));
                    Main.dust[d].velocity = (Main.dust[d].position - center) / 12f;
                }
                for (int i = 0; i < 10; i++)
                {
                    var spawnPos = npc.position + new Vector2(Main.rand.Next(npc.width), Main.rand.Next(20));
                    ParticleLayers.AddParticle_PostDrawPlayers(new SpaceSquidSnowflake(spawnPos, new Vector2(Main.rand.NextFloat(-4f, 4f), -Main.rand.NextFloat(1f, 4f)), default(Color), Main.rand.NextFloat(0.6f, 1.2f)));
                }
                for (int i = 0; i < 3; i++)
                {
                    Gore.NewGore(new Vector2(npc.position.X + npc.width / 2f + 10 * (i - 1), npc.position.Y + npc.height - 30f), new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(1f, 4f)), ModGore.GetGoreSlot("AQMod/Gores/GaleStreams/SpaceSquid_2"));
                }

                Gore.NewGore(new Vector2(npc.position.X + (npc.direction == 1 ? npc.width : 0), npc.position.Y + 40f), new Vector2(Main.rand.NextFloat(-4f, 4f), -Main.rand.NextFloat(1f, 4f)), ModGore.GetGoreSlot("AQMod/Gores/GaleStreams/SpaceSquid_0"));
                Gore.NewGore(new Vector2(npc.position.X + npc.width / 2f, npc.position.Y + 20f), new Vector2(Main.rand.NextFloat(-4f, 4f), -Main.rand.NextFloat(1f, 4f)), ModGore.GetGoreSlot("AQMod/Gores/GaleStreams/SpaceSquid_1"));
            }
            else
            {
                for (int i = 0; i < damage / 100; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<SpaceSquidBlood>());
                    Main.dust[d].velocity = (Main.dust[d].position - center) / 12f;
                }

                if (Main.rand.NextBool(20))
                {
                    var spawnPos = npc.position + new Vector2(Main.rand.Next(-10, 10) + npc.width, Main.rand.Next(20));
                    ParticleLayers.AddParticle_PostDrawPlayers(new SpaceSquidSnowflake(spawnPos, new Vector2(Main.rand.NextFloat(-4f, 4f), -Main.rand.NextFloat(1f, 4f))));
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

        public const int Phase_Goodbye = -1;
        public const int Phase_SpaceGun = 1;
        public const int Phase_ChangeDirection = 2;

        public override void AI()
        {
            if ((int)npc.ai[0] == Phase_Goodbye)
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
                    npc.ai[0] = Phase_Goodbye;
                    return;
                }
                if (Main.player[npc.target].position.X + Main.player[npc.target].width / 2f < npc.position.X + npc.width / 2f)
                {
                    npc.direction = -1;
                }
                else
                {
                    npc.direction = 1;
                }
                npc.spriteDirection = npc.direction;
                AdvancePhase();
                npc.velocity = -Vector2.Normalize(Main.player[npc.target].Center - center) * 20f;
            }
            if (!npc.HasValidTarget)
            {
                npc.ai[0] = Phase_Goodbye;
                return;
            }
            switch ((int)npc.ai[0])
            {
                case Phase_SpaceGun:
                {
                    npc.velocity *= 0.8f;
                    if (Main.player[npc.target].position.X + Main.player[npc.target].width / 2f < npc.position.X + npc.width / 2f)
                    {
                        npc.direction = -1;
                        if (npc.position.X > Main.player[npc.target].position.X - 100)
                        {
                            if (npc.velocity.X > -10f)
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
                            if (Main.player[npc.target].position.X - npc.position.X > 500)
                            {
                                if (npc.velocity.X < 10f)
                                {
                                    npc.velocity.X += 0.2f;
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
                        npc.direction = 1;
                        if (npc.position.X < Main.player[npc.target].position.X + 100)
                        {
                            if (npc.velocity.X < 10f)
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
                            if (npc.position.X - Main.player[npc.target].position.X > 500)
                            {
                                if (npc.velocity.X > -10f)
                                {
                                    npc.velocity.X -= 0.2f;
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
                    if (npc.ai[1] > 120f)
                    {
                        if ((int)npc.ai[1] >= 370)
                        {
                            AdvancePhase(Phase_SpaceGun);
                        }
                        else
                        {
                            int timer = (int)(npc.ai[1] - 120) % 50;
                            if (timer == 0)
                            {
                                frameIndex = 8;
                                SoundID.Item33.Play(npc.Center);
                                var spawnPosition = new Vector2(npc.position.X + (npc.direction == 1 ? npc.width : 0), npc.position.Y + npc.height / 2f);
                                var velocity = new Vector2(12f * -npc.direction, 0f);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(spawnPosition, velocity, ModContent.ProjectileType<Projectiles.Monster.GaleStreams.SpaceSquidLaser>(), 50, 1f, Main.myPlayer);
                                    Projectile.NewProjectile(spawnPosition, velocity.RotatedBy(MathHelper.PiOver4), ModContent.ProjectileType<Projectiles.Monster.GaleStreams.SpaceSquidLaser>(), 50, 1f, Main.myPlayer);
                                    Projectile.NewProjectile(spawnPosition, velocity.RotatedBy(-MathHelper.PiOver4), ModContent.ProjectileType<Projectiles.Monster.GaleStreams.SpaceSquidLaser>(), 50, 1f, Main.myPlayer);
                                }
                            }
                        }
                        npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, (Main.player[npc.target].position.Y - center.Y) / 4f, 0.005f);
                    }
                    else
                    {
                        npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, (Main.player[npc.target].position.Y - center.Y) / 4f, 0.1f);
                    }
                    npc.ai[1]++;
                    npc.rotation = npc.velocity.X * 0.01f;
                }
                break;

                case Phase_ChangeDirection:
                {
                    npc.velocity *= 0.8f;
                    npc.ai[1]++;
                    if (npc.ai[1] > 20f)
                    {
                        npc.spriteDirection = npc.direction;
                        AdvancePhase(Phase_ChangeDirection);
                    }
                }
                break;
            }
        }

        private void AdvancePhase(int curPhase = -1)
        {
            npc.TargetClosest(faceTarget: false);
            npc.netUpdate = true;
            if (!npc.HasValidTarget)
            {
                npc.ai[0] = Phase_Goodbye;
                return;
            }
            if (curPhase != Phase_ChangeDirection && npc.direction != npc.spriteDirection)
            {
                npc.ai[0] = Phase_ChangeDirection;
                npc.ai[1] = 0f;
                frameIndex = 19;
                return;
            }
            for (int i = 0; i < 50; i++)
            {
                npc.ai[0] = Phase_SpaceGun;
            }
            npc.ai[1] = 0f;
            frameIndex = 0;
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

            switch ((int)npc.ai[0])
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

                case -1:
                frameIndex = 20;
                break;

                case Phase_SpaceGun:
                {
                    if ((int)npc.ai[1] >= 370)
                    {
                        if (frameIndex < 13)
                        {
                            frameIndex = 13;
                        }
                        npc.frameCounter += 1.0d;
                        if (npc.frameCounter > 4.0d)
                        {
                            npc.frameCounter = 0.0d;
                            frameIndex++;
                            if (frameIndex > 7)
                            {
                                frameIndex = 0;
                            }
                        }
                    }
                    else if (frameIndex > 7)
                    {
                        npc.frameCounter += 1.0d;
                        if (npc.frameCounter > 4.0d)
                        {
                            npc.frameCounter = 0.0d;
                            frameIndex++;
                            if (frameIndex > 12)
                            {
                                frameIndex = 12;
                            }
                        }
                    }
                    else
                    {
                        npc.frameCounter += 1.0d;
                        if (npc.frameCounter > 4.0d)
                        {
                            npc.frameCounter = 0.0d;
                            frameIndex++;
                            if (frameIndex > 7)
                            {
                                frameIndex = 0;
                            }
                        }
                    }
                }
                break;

                case 2:
                if (frameIndex < 19)
                {
                    frameIndex = 19;
                }
                npc.frameCounter += 1.0d;
                if (npc.frameCounter > 4.0d)
                {
                    npc.frameCounter = 0.0d;
                    frameIndex++;
                    if (frameIndex > 23)
                    {
                        frameIndex = 23;
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
            if (npc.target != -1)
                Content.WorldEvents.GaleStreams.GaleStreams.ProgressEvent(Main.player[npc.target], 40);
            WorldDefeats.DownedSpaceSquid = true;
            Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Materials.Energies.AtmosphericEnergy>(), Main.rand.Next(2) + 2);
            Item.NewItem(npc.getRect(), ItemID.SoulofFlight, Main.rand.Next(5) + 2);
            //Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Materials.Fluorescence>(), Main.rand.Next(10) + 10 + (Main.expertMode ? Main.rand.Next(5) : 0));

            if (Main.rand.NextBool(8))
            {
                Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Foods.GaleStreams.PeeledCarrot>());
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            var texture = Main.npcTexture[npc.type];
            var drawPosition = npc.Center;
            var origin = new Vector2(npc.frame.Width / 2f, npc.frame.Height / 2f - 14f);
            Vector2 scale = new Vector2(npc.scale, npc.scale);
            float aura = 3f + (float)Math.Sin(Main.GlobalTime * 5f);
            float speedX = npc.velocity.X.Abs();
            var effects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
            {
                effects = SpriteEffects.FlipHorizontally;
            }
            if (speedX > 8f)
            {
                scale.X += (speedX - 8f) / 120f;
                drawPosition.X -= (scale.X - 1f) * 16f;
            }
            if (aura > 0f)
            {
                var auraTexture = this.GetTextureobj("_Aura");
                for (int i = 0; i < 8; i++)
                {
                    Main.spriteBatch.Draw(auraTexture, drawPosition - Main.screenPosition + new Vector2(aura, 0f).RotatedBy(MathHelper.PiOver4 * i), npc.frame, new Color(255, 255, 255, 20), npc.rotation, origin, scale, effects, 0f);
                }
            }


            Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, npc.frame, drawColor, npc.rotation, origin, scale, effects, 0f);
            Main.spriteBatch.Draw(this.GetTextureobj("_Glow"), drawPosition - Main.screenPosition, npc.frame, Color.White, npc.rotation, origin, scale, effects, 0f);
            return false;
        }

        void ISetupContentType.SetupContent()
        {
            try
            {
                var bossChecklist = ModLoader.GetMod("BossChecklist");
                if (bossChecklist == null)
                    return;
                new MinibossEntry(
                    () => WorldDefeats.DownedSpaceSquid,
                    6.68f,
                    ModContent.NPCType<SpaceSquid>(),
                    AQText.chooselocalizationtext(
                        en_US: "Space Squid",
                        zh_Hans: null),
                    0,
                    new List<int>()
                    {
                        ModContent.ItemType<Items.Materials.Energies.AtmosphericEnergy>(),
                        ModContent.ItemType<Items.Foods.GaleStreams.PeeledCarrot>(),
                    },
                    new List<int>()
                    {
                        //ModContent.ItemType<Items.BossItems.RedSpriteTrophy>(),
                        //ModContent.ItemType<Items.Vanities.Dyes.RedSpriteDye>(),
                    },
                    AQText.chooselocalizationtext(
                        en_US: "Occasionally appears during the Gale Streams!",
                        zh_Hans: null),
                    "AQMod/Assets/BossChecklist/SpaceSquid").AddEntry(bossChecklist);
            }
            catch (Exception e)
            {
                mod.Logger.Error("An error occured when setting up boss checklist entries.");
                mod.Logger.Error(e.Message);
                mod.Logger.Error(e.StackTrace);
            }
        }
    }
}