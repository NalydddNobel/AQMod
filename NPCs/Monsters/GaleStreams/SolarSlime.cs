using AQMod.Items.Materials.Energies;
using AQMod.Projectiles.Monster;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters.GaleStreams
{
    public class SolarSlime : ModNPC, IDecideFallThroughPlatforms
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 20;
        }

        public override void SetDefaults()
        {
            npc.width = 24;
            npc.height = 24;
            npc.aiStyle = -1;
            npc.damage = 60;
            npc.defense = 10;
            npc.lifeMax = 210;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.scale = 1f;
            npc.alpha = 50;
            npc.lavaImmune = true;
            npc.trapImmune = true;
            npc.value = Item.buyPrice(silver: 10);
            npc.knockBackResist = 0.2f;
            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.CursedInferno] = true;
            npc.buffImmune[BuffID.ShadowFlame] = true;
            npc.buffImmune[BuffID.Confused] = false;
            npc.SetLiquidSpeed(water: 1f, lava: 1f);
            banner = npc.type;
            bannerItem = ModContent.ItemType<Items.Placeable.Banners.SolarSlimeBanner>();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.65f);
            if (Main.hardMode)
            {
                npc.lifeMax *= 2;
                npc.knockBackResist = 0f;
            }
        }

        public override void AI()
        {
            if (npc.velocity.Y == 0f)
                npc.velocity.X *= 0.8f;
            if ((int)npc.ai[1] == 1)
            {
                if (npc.localAI[0] > 21f)
                {
                    if (npc.localAI[0] > 120f)
                    {
                        npc.localAI[0]++;
                        if (npc.localAI[0] > 147f)
                        {
                            npc.localAI[0] = 0f;
                            npc.ai[1] = -1f;
                            npc.netUpdate = true;
                        }
                    }
                    else
                    {
                        npc.localAI[0] += Main.rand.Next(5);
                    }
                }
                else
                {
                    npc.localAI[0]++;
                }
            }
            else
            {
                if (npc.collideY)
                {
                    if ((int)npc.ai[1] == 0)
                    {
                        npc.ai[1] = 1f;
                        npc.netUpdate = true;
                        return;
                    }
                }
                bool incrementTimer = true;
                if (npc.ai[0] <= -20f)
                {
                    if (npc.collideY && npc.velocity.Y >= 0f)
                    {
                        int timer = (int)(-npc.ai[0] - 20);
                        if (timer <= 40 && (int)npc.ai[1] != 2)
                        {
                            npc.ai[1] = 2f;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int projectileType = ModContent.ProjectileType<Magmabub>();
                                var spawnPosition = npc.Center;
                                int damage = 20;
                                float speed = 7f;
                                if (Main.expertMode)
                                {
                                    damage = 15;
                                    speed = 11f;
                                }
                                Main.PlaySound(SoundID.Item85, spawnPosition);
                                for (int i = 0; i < 3; i++)
                                {
                                    var velocity = new Vector2(0f, -1f).RotatedBy(-0.314f + 0.314f * i) * speed;
                                    Projectile.NewProjectile(spawnPosition, velocity, projectileType, damage, 1f, Main.myPlayer);
                                }
                            }
                        }
                        else if (timer == 0)
                        {
                            npc.ai[1] = 0f;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int projectileType = ModContent.ProjectileType<Magmabub>();
                                var spawnPosition = npc.Center;
                                int damage = 15;
                                float speed = 6f;
                                if (Main.expertMode)
                                {
                                    damage = 20;
                                    speed = 9f;
                                }
                                Main.PlaySound(SoundID.Item85, spawnPosition);
                                for (int i = 0; i < 3; i++)
                                {
                                    var velocity = new Vector2(0f, -1f).RotatedBy(-0.314f + 0.314f * i) * speed;
                                    Projectile.NewProjectile(spawnPosition, velocity, projectileType, damage, 1f, Main.myPlayer);
                                }
                            }
                        }
                    }
                    else
                    {
                        incrementTimer = false;
                    }
                }
                else
                {
                    int jumpTime = (int)(npc.ai[0] % 1000f);
                    if (jumpTime > 40f)
                    {
                        incrementTimer = false;
                        if (npc.collideY && npc.velocity.Y >= 0f)
                        {
                            npc.TargetClosest();
                            npc.ai[1] = 0f;
                            npc.ai[0] += 960f;
                            if (npc.ai[0] > 3000f)
                                npc.ai[0] = -100f;
                            npc.velocity.Y += -7f;
                            npc.velocity.X += 4f * npc.direction;
                            npc.netUpdate = true;
                        }
                    }
                }
                if (incrementTimer)
                    npc.ai[0]++;
            }
            int d = Dust.NewDust(npc.position + new Vector2(0f, -8f), npc.width, npc.height, ModContent.DustType<Content.Dusts.MonoDust>(), 0f, 0f, 0, new Color(255, 255, 240, 0));
            Main.dust[d].velocity *= 0.1f;
            Main.dust[d].scale = Main.rand.NextFloat(1f, 1.5f);
        }

        public override void FindFrame(int frameHeight)
        {
            if (npc.velocity.Y != 0 && !npc.collideY)
            {
                if (npc.velocity.Y < 0f)
                {
                    npc.frame.Y = frameHeight;
                }
                else
                {
                    npc.frame.Y = frameHeight * 2;
                }
            }
            else if ((int)npc.ai[1] == 1)
            {
                if (npc.localAI[0] > 21f)
                {
                    if (npc.localAI[0] > 120f)
                    {
                        npc.frame.Y = frameHeight * 7 + frameHeight * (((int)npc.localAI[0] - 120) / 3);
                    }
                    else
                    {
                        npc.frame.Y = frameHeight * 7;
                    }
                }
                else
                {
                    npc.frame.Y = frameHeight * ((int)npc.localAI[0] / 3);
                }
            }
            else
            {
                int jumpTime = (int)(npc.ai[0] % 1000f);
                npc.frameCounter += 1.0d;
                if (npc.frameCounter >= 6.0d)
                {
                    npc.frameCounter = 0.0d;
                    npc.frame.Y += frameHeight;
                    if (npc.frame.Y > frameHeight)
                        npc.frame.Y = 0;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            var texture = Main.npcTexture[npc.type];
            var drawPosition = new Vector2(npc.position.X + npc.width / 2f, npc.position.Y + npc.height / 2f);
            drawPosition.Y -= 2f;
            var screenPos = Main.screenPosition;
            var orig = new Vector2(npc.frame.Width / 2f, npc.frame.Height / 2f);

            Main.spriteBatch.Draw(texture, drawPosition - screenPos, npc.frame, new Color(255, 255, 255, npc.alpha), npc.rotation, orig, npc.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            int count = 1;
            if (npc.life <= 0)
                count = 20;
            for (int i = 0; i < count; i++)
            {
                Dust.NewDust(npc.position + new Vector2(0f, -8f), npc.width, npc.height, DustID.Fire);
            }
        }

        public override void NPCLoot()
        {
        }

        bool IDecideFallThroughPlatforms.Decide()
        {
            if (Main.player[npc.target].dead)
            {
                return true;
            }
            else
            {
                return Main.player[npc.target].position.Y
                    > npc.position.Y + npc.height;
            }
        }
    }

}