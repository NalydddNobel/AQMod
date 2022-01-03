using AQMod.Common;
using AQMod.Common.CrossMod.BossChecklist;
using AQMod.Common.Graphics.Particles;
using AQMod.Dusts;
using AQMod.Dusts.GaleStreams;
using AQMod.Effects.ScreenEffects;
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

        internal static Vector2 GetEyePosition(NPC npc)
        {
            if (npc.direction == -1)
            {
                return npc.position + new Vector2(4f, npc.height / 2f - 2f);
            }
            return npc.position + new Vector2(npc.width - 4f, npc.height / 2f - 2f);
        }

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
            bannerItem = ModContent.ItemType<SpaceSquidBanner>();
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
        public const int Phase_SnowflakeSpiral = 3;

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
                    bool runOtherAis = true;
                    bool noDeathray = true;
                    if (Main.expertMode && npc.life * 2 < npc.lifeMax)
                    {
                        noDeathray = false;
                        if ((int)npc.ai[1] == 202)
                        {
                            npc.ai[2] = 0f;
                            if (Main.netMode != NetmodeID.Server && (Main.player[Main.myPlayer].Center - center).Length() < 2000f)
                            {
                                AQSound.LegacyPlay(SoundType.Item, "Sounds/Item/SpaceSquid/ShootDeathray");
                            }
                        }
                        if ((int)npc.ai[1] >= 245)
                        {
                            runOtherAis = false;
                            if ((int)npc.ai[2] < 1)
                            {
                                npc.ai[2]++;
                                npc.velocity.X = -npc.direction * 12.5f;
                                if (Main.netMode != NetmodeID.Server && AQConfigClient.c_Screenshakes && (Main.player[Main.myPlayer].Center - center).Length() < 2000f)
                                {
                                    ScreenShakeManager.AddShake(new BasicScreenShake(4, 8));
                                }
                                int p = Projectile.NewProjectile(GetEyePosition(npc), new Vector2(0f, 0f), ModContent.ProjectileType<Projectiles.Monster.GaleStreams.SpaceSquidDeathray>(), 70, 1f, Main.myPlayer);
                                Main.projectile[p].ai[0] = npc.whoAmI + 1;
                                Main.projectile[p].direction = npc.direction;
                            }
                            if (npc.velocity.Length() > 2f)
                            {
                                npc.velocity *= 0.92f;
                                if (npc.velocity.Length() < 2f)
                                {
                                    npc.velocity = Vector2.Normalize(npc.velocity) * 2f;
                                }
                            }
                        }
                        else if ((int)npc.ai[1] > 200)
                        {
                            var eyePos = GetEyePosition(npc);
                            SpawnPatterns.SpawnDustCentered(eyePos, ModContent.DustType<MonoDust>(), new Vector2(0f, 0f), new Color(10, 255, 20, 0), 0.9f);
                            int spawnChance = 3 - (int)(npc.ai[1] - 210) / 8;
                            if (spawnChance <= 1 || Main.rand.NextBool(spawnChance))
                            {
                                var spawnPos = eyePos + new Vector2(Main.rand.NextFloat(-60f, 60f), Main.rand.NextFloat(-60f, 60f));
                                SpawnPatterns.SpawnDustCentered(spawnPos, ModContent.DustType<MonoDust>(), (eyePos - spawnPos) / 8f + npc.velocity, new Color(10, 200, 20, 0), Main.rand.NextFloat(0.9f, 1.35f));
                            }
                            if (spawnChance <= 1)
                            {
                                var spawnPos = eyePos + new Vector2(Main.rand.NextFloat(-120f, 120f), Main.rand.NextFloat(-120f, 120f));
                                SpawnPatterns.SpawnDustCentered(spawnPos, ModContent.DustType<MonoDust>(), (eyePos - spawnPos) / 12f + npc.velocity, new Color(10, 200, 20, 0), Main.rand.NextFloat(0.5f, 0.75f));
                            }
                        }
                        if ((int)npc.ai[1] >= 330)
                        {
                            AdvancePhase(Phase_SpaceGun);
                        }
                    }
                    if (runOtherAis)
                    {
                        if (npc.ai[1] >= 120f)
                        {
                            if ((int)npc.ai[1] >= 200)
                            {
                                if (noDeathray && (int)npc.ai[1] >= 240)
                                {
                                    if (Main.player[npc.target].position.X + Main.player[npc.target].width / 2f < npc.position.X + npc.width / 2f)
                                    {
                                        npc.direction = -1;
                                    }
                                    else
                                    {
                                        npc.direction = 1;
                                    }
                                    AdvancePhase(Phase_SpaceGun);
                                    npc.velocity *= 0.95f;
                                }
                            }
                            else
                            {
                                int timer = (int)(npc.ai[1] - 120) % 10;
                                if (timer == 0)
                                {
                                    frameIndex = 8;
                                    if (Main.netMode != NetmodeID.Server)
                                    {
                                        AQSound.LegacyPlay(SoundType.Item, "Sounds/Item/SpaceSquid/ShootLaser");
                                    }
                                    var spawnPosition = new Vector2(npc.position.X + (npc.direction == 1 ? npc.width + 20f : -20), npc.position.Y + npc.height / 2f);
                                    var velocity = new Vector2(20f * npc.direction, 0f);
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(spawnPosition, velocity, ModContent.ProjectileType<Projectiles.Monster.GaleStreams.SpaceSquidLaser>(), 30, 1f, Main.myPlayer);
                                        Projectile.NewProjectile(spawnPosition, velocity.RotatedBy(MathHelper.PiOver4), ModContent.ProjectileType<Projectiles.Monster.GaleStreams.SpaceSquidLaser>(), 40, 1f, Main.myPlayer);
                                        Projectile.NewProjectile(spawnPosition, velocity.RotatedBy(-MathHelper.PiOver4), ModContent.ProjectileType<Projectiles.Monster.GaleStreams.SpaceSquidLaser>(), 40, 1f, Main.myPlayer);
                                    }
                                }
                            }
                            npc.velocity.X = MathHelper.Lerp(npc.velocity.X, (Main.player[npc.target].position.X - npc.direction * 300f - center.X) / 16f, 0.001f);
                            npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, (Main.player[npc.target].position.Y + 6f - center.Y) / 8f, 0.01f);
                        }
                        else
                        {
                            npc.velocity.X = MathHelper.Lerp(npc.velocity.X, (Main.player[npc.target].position.X - npc.direction * 300f - center.X) / 16f, 0.05f);
                            npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, (Main.player[npc.target].position.Y + 6f - center.Y) / 8f, 0.1f);
                        }
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
                        AdvancePhase((int)npc.ai[2]);
                    }
                }
                break;

                case Phase_SnowflakeSpiral:
                {
                    var gotoPosition = new Vector2(205f * -npc.direction, 0f).RotatedBy(npc.ai[1] * 0.01f);
                    gotoPosition = Main.player[npc.target].Center + new Vector2(gotoPosition.X * 2f, gotoPosition.Y);
                    if (Main.player[npc.target].position.X + Main.player[npc.target].width / 2f < npc.position.X + npc.width / 2f)
                    {
                        npc.direction = -1;
                    }
                    else
                    {
                        npc.direction = 1;
                    }
                    if (npc.spriteDirection != npc.direction)
                    {
                        if (frameIndex >= 24)
                        {
                            frameIndex = 19;
                        }
                    }
                    if ((int)npc.ai[1] == 0)
                    {
                        npc.ai[1] = Main.rand.NextFloat(MathHelper.Pi * 100f);
                        npc.netUpdate = true;
                    }
                    npc.ai[2]++;
                    if (npc.ai[2] > 120f)
                    {
                        npc.ai[1] += 0.2f;
                        if ((int)npc.ai[3] == 0)
                        {
                            npc.ai[3] = Main.rand.NextFloat(MathHelper.Pi * 6f);
                            npc.netUpdate = true;
                        }
                        npc.ai[3]++;
                        npc.velocity = Vector2.Lerp(npc.velocity, Vector2.Normalize(gotoPosition - npc.Center) * 10f, 0.006f);
                        int timeBetweenShots = 3 + npc.life / (npc.lifeMax / 3);
                        int timer = (int)(npc.ai[2] - 60) % timeBetweenShots;
                        if (timer == 0)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                var velocity = new Vector2(10f, 0f).RotatedBy(npc.ai[3] * 0.12f);
                                Projectile.NewProjectile(npc.Center + velocity * 4f, velocity, ModContent.ProjectileType<Projectiles.Monster.GaleStreams.SpaceSquidSnowflake>(), 20, 1f, Main.myPlayer);
                            }
                            if (Main.netMode != NetmodeID.Server)
                            {
                                AQSound.LegacyPlay(SoundType.Item, "Sounds/Item/Combo", npc.Center);
                            }
                        }
                        if (npc.ai[2] > 180f + (6 - timeBetweenShots) * 40f)
                        {
                            npc.ai[2] = 0f;
                            npc.ai[3] = 0f;
                            if (Main.player[npc.target].position.X + Main.player[npc.target].width / 2f < npc.position.X + npc.width / 2f)
                            {
                                npc.direction = -1;
                            }
                            else
                            {
                                npc.direction = 1;
                            }
                            AdvancePhase(Phase_SnowflakeSpiral);
                        }
                    }
                    else
                    {
                        npc.ai[1]++;
                        npc.velocity = Vector2.Lerp(npc.velocity, Vector2.Normalize(gotoPosition - npc.Center) * 20f, 0.01f);
                    }
                    npc.rotation = Utils.AngleLerp(npc.rotation, 0f, 0.1f);
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
                npc.ai[1] = 0f;
                npc.ai[2] = npc.ai[0];
                npc.ai[0] = Phase_ChangeDirection;
                frameIndex = 19;
                return;
            }
            int[] selectablePhases = new int[] { Phase_SpaceGun, Phase_SnowflakeSpiral };
            for (int i = 0; i < 50; i++)
            {
                npc.ai[0] = selectablePhases[Main.rand.Next(selectablePhases.Length)];
                if ((int)npc.ai[0] != curPhase)
                {
                    break;
                }
            }
            npc.ai[1] = 0f;
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

                case Phase_Goodbye:
                frameIndex = 20;
                break;

                case Phase_SpaceGun:
                {
                    if ((int)npc.ai[1] >= 200)
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
                            if (frameIndex > 18)
                            {
                                frameIndex = 18;
                            }
                        }
                    }
                    else if (frameIndex > 13)
                    {
                        npc.frameCounter += 1.0d;
                        if (frameIndex > 18)
                        {
                            frameIndex = 18;
                        }
                        if (npc.frameCounter > 4.0d)
                        {
                            npc.frameCounter = 0.0d;
                            frameIndex--;
                            if (frameIndex == 13)
                            {
                                frameIndex = 0;
                            }
                        }
                    }
                    else if (frameIndex > 7)
                    {
                        npc.frameCounter += 1.0d;
                        if (npc.frameCounter > 3.0d)
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

                case Phase_ChangeDirection:
                {
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
                }
                break;

                case Phase_SnowflakeSpiral:
                {
                    if (npc.direction != npc.spriteDirection)
                    {
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
                                frameIndex = 24;
                                npc.spriteDirection = npc.direction;
                            }
                        }
                    }
                    else
                    {
                        if (frameIndex < 24)
                        {
                            frameIndex = 24;
                        }
                        npc.frameCounter += 1.0d;
                        if (npc.frameCounter > 4.0d)
                        {
                            npc.frameCounter = 0.0d;
                            frameIndex++;
                            if (frameIndex > 28)
                            {
                                frameIndex = 25;
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
            if (npc.target != -1)
                Content.World.Events.GaleStreams.GaleStreams.ProgressEvent(Main.player[npc.target], 40);
            WorldDefeats.DownedSpaceSquid = true;
            Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Materials.Energies.AtmosphericEnergy>(), Main.rand.Next(2) + 2);
            Item.NewItem(npc.getRect(), ItemID.SoulofFlight, Main.rand.Next(5) + 2);
            Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Materials.SiphonTentacle>(), Main.rand.Next(10) + 10 + (Main.expertMode ? Main.rand.Next(5) : 0));

            if (Main.rand.NextBool(2))
            {
                Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Vanities.Dyes.FrostbiteDye>());
            }
            if (Main.rand.NextBool(8))
            {
                Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Foods.GaleStreams.PeeledCarrot>());
            }
            if (Main.rand.NextBool(10))
            {
                Item.NewItem(npc.getRect(), ModContent.ItemType<Items.BossItems.SpaceSquidTrophy>());
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            var texture = Main.npcTexture[npc.type];
            var drawPosition = npc.Center;
            var origin = new Vector2(npc.frame.Width / 2f, npc.frame.Height / 2f);
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
                    Main.spriteBatch.Draw(auraTexture, drawPosition - Main.screenPosition + new Vector2(aura, 0f).RotatedBy(MathHelper.PiOver4 * i), npc.frame, new Color(120, 120, 120, 20), npc.rotation, origin, scale, effects, 0f);
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
                        ItemID.SoulofFlight,
                        ModContent.ItemType<Items.Materials.Energies.AtmosphericEnergy>(),
                        ModContent.ItemType<Items.Materials.SiphonTentacle>(),
                        ModContent.ItemType<Items.Foods.GaleStreams.PeeledCarrot>(),
                    },
                    new List<int>()
                    {
                        ModContent.ItemType<Items.BossItems.SpaceSquidTrophy>(),
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