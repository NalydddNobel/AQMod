using AQMod.Assets;
using AQMod.Common.ID;
using AQMod.Content.World.Events.DemonSiege;
using AQMod.Items.Materials.Energies;
using AQMod.Projectiles.Monster;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters.DemonSiege
{
    public class Magmalbubble : ModNPC, IDecideFallThroughPlatforms
    {
        public const int FRAME_IDLE0 = 0;
        public const int FRAME_IDLE1 = 1;
        public const int FRAME_JUMPUP = 2;
        public const int FRAME_JUMPGOINGDOWN = 3;
        public const int FRAME_JUMPDOWN = 4;

        public const int FRAME_JUMPRECOIL0 = 5;
        public const int FRAME_JUMPRECOIL1 = 6;
        public const int FRAME_JUMPRECOIL2 = 7;
        public const int FRAME_JUMPRECOIL3 = 8;
        public const int FRAME_JUMPRECOIL4 = 9;
        public const int FRAME_JUMPRECOIL5 = 10;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 11;
        }

        public override void SetDefaults()
        {
            npc.width = 24;
            npc.height = 24;
            npc.aiStyle = -1;
            npc.damage = 20;
            npc.defense = 16;
            npc.lifeMax = 100;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.scale = 1.1f;
            npc.alpha = 50;
            npc.lavaImmune = true;
            npc.trapImmune = true;
            npc.value = 150f;
            npc.knockBackResist = 0.7f;
            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.CursedInferno] = true;
            npc.buffImmune[BuffID.Confused] = false;
            npc.SetLiquidSpeed(lava: 1f);
            banner = npc.type;
            bannerItem = ModContent.ItemType<Items.Placeable.Banners.MagmabubbleBanner>();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.65f);
            if (AQMod.calamityMod.IsActive)
            {
                npc.lifeMax = (int)(npc.lifeMax * 2.5f);
                npc.damage = (int)(npc.damage * 1.5f);
                npc.defense *= 2;
            }
        }

        private bool checkPlayerSights(int chances = 4)
        {
            if (Collision.CanHitLine(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].width))
            {
                return true;
            }
            else
            {
                npc.ai[2]++;
                if ((int)npc.ai[2] > chances)
                {
                    npc.ai[1] = 3f;
                    npc.ai[2] = 0f;
                    return false;
                }
                return true;
            }
        }

        public override void AI()
        {
            if (npc.velocity.Y == 0f)
                npc.velocity.X *= 0.8f;
            if ((int)npc.ai[1] == 3)
            {
                npc.ai[2]++;
                if (npc.ai[2] > 60f)
                {
                    int cX = 0;
                    int cY = 0;
                    var player = Main.player[npc.target];
                    int playerX = (int)((player.position.X + player.width / 2f) / 16f);
                    int playerY = (int)((player.position.Y + player.height / 2f) / 16f);
                    for (int i = 0; i < 1000; i++)
                    {
                        int x = playerX + Main.rand.Next(-40, 40);
                        int y = playerY + Main.rand.Next(-20, 20);
                        if (i >= 999)
                        {
                            cX = x;
                            cY = y;
                            break;
                        }
                        if (x < 10)
                        {
                            x = 10;
                        }
                        else if (x > Main.maxTilesX - 10)
                        {
                            x = Main.maxTilesX - 10;
                        }
                        if (y < 10)
                        {
                            y = 10;
                        }
                        else if (y > Main.maxTilesY - 10)
                        {
                            y = Main.maxTilesY - 10;
                        }
                        if (Main.tile[x, y] == null)
                            Main.tile[x, y] = new Tile();
                        if (Main.tile[x, y].active() && Main.tileSolid[Main.tile[x, y].type])
                            continue;
                        if (!Main.tile[x, y + 1].active() || !Main.tileSolid[Main.tile[x, y + 1].type])
                            continue;
                        int xOff = x - playerX;
                        int yOff = y - playerY;
                        if (Math.Sqrt(xOff * xOff + yOff * yOff) < 4.0)
                            continue;
                        cX = x;
                        cY = y;
                        break;
                    }
                    npc.position = new Vector2(cX * 16f + 8f - npc.width / 2f, cY * 16f - npc.height + 12f);
                    npc.velocity *= 0.1f;
                    npc.TargetClosest_UpdateToFaceTowardsUndetectableTargetsToo();
                    npc.velocity.X += 3f * npc.direction;
                    npc.velocity.Y -= 4f;
                    npc.netUpdate = true;
                    npc.ai[0] = 2000f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    for (int i = 0; i < 5; i++)
                    {
                        int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Fire);
                        Main.dust[d].velocity = new Vector2(0f, 5f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                    }
                    for (int i = 0; i < 30; i++)
                    {
                        int d = Dust.NewDust(npc.position + new Vector2(0f, -8f), npc.width, npc.height, DustID.Fire);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity.Y *= 0.2f;
                        Main.dust[d].velocity.X *= 3f;
                        Main.dust[d].scale *= Main.rand.NextFloat(0.5f, 2f);
                    }
                }
                for (int i = 0; i < 3; i++)
                {
                    int d = Dust.NewDust(npc.position + new Vector2(0f, -8f), npc.width, npc.height, DustID.Fire);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity.Y *= 0.2f;
                    Main.dust[d].velocity.X *= 3f;
                    Main.dust[d].scale *= Main.rand.NextFloat(0.5f, 2f);
                }
            }
            else if ((int)npc.ai[1] == 1)
            {
                npc.localAI[0]++;
                if (npc.localAI[0] >= 36f)
                {
                    npc.localAI[0] = 0f;
                    npc.ai[1] = -1f;
                    npc.netUpdate = true;
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
                    if (npc.velocity.Y == 0f)
                    {
                        int timer = (int)(-npc.ai[0] - 20);
                        if (timer <= 40 && (int)npc.ai[1] != 2)
                        {
                            npc.ai[1] = 2f;
                            checkPlayerSights(chances: 6);
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
                            checkPlayerSights(chances: 6);
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
                        if (npc.velocity.Y >= 0f)
                        {
                            checkPlayerSights(chances: 8);
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
            if (Main.rand.NextBool(3))
                Dust.NewDust(npc.position + new Vector2(0f, -8f), npc.width, npc.height, DustID.Fire);
        }

        public override void FindFrame(int frameHeight)
        {
            if (npc.velocity.Y != 0 && !npc.collideY)
            {
                if (npc.velocity.Y.Abs() < 2f)
                {
                    npc.frame.Y = frameHeight * FRAME_JUMPGOINGDOWN;
                }
                else if (npc.velocity.Y < 0f)
                {
                    npc.frame.Y = frameHeight * FRAME_JUMPUP;
                }
                else
                {
                    npc.frame.Y = frameHeight * FRAME_JUMPDOWN;
                }
            }
            else if ((int)npc.ai[1] == 1)
            {
                npc.frame.Y = frameHeight * (FRAME_JUMPRECOIL0 + (int)(npc.localAI[0] / 6f));
            }
            else
            {
                int jumpTime = (int)(npc.ai[0] % 1000f);
                npc.frameCounter += 1.0d;
                if (npc.frameCounter >= 6.0d)
                {
                    npc.frameCounter = 0.0d;
                    npc.frame.Y += frameHeight;
                    if (npc.frame.Y >= frameHeight * 2)
                        npc.frame.Y = 0;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            var texture = Main.npcTexture[npc.type];
            var legsTexture = this.GetTextureobj("_Legs");
            var drawPosition = new Vector2(npc.position.X + npc.width / 2f, npc.position.Y + npc.height / 2f);
            drawPosition.Y -= 10.5f;
            var screenPos = Main.screenPosition;
            var orig = new Vector2(npc.frame.Width / 2f, npc.frame.Height / 2f);

            if (npc.ai[0] < -20f)
            {
                int timer = (int)(-npc.ai[0] - 20);
                float progress = 0f;
                if (timer > 40)
                {
                    progress = (timer - 40f) / 40f;
                }
                else if (timer < 40)
                {
                    progress = timer / 40f;
                }
                progress = 1f - progress;
                var spotlight = AQTextures.Lights[LightTex.Spotlight10x50];
                var color = Color.Lerp(Color.Red, Color.Yellow, ((float)Math.Sin(Main.GlobalTime * 25f) + 1f) / 2f);
                color *= progress;
                var spotlightOrigin = spotlight.Size() / 2f;
                for (int i = 0; i < 3; i++)
                {
                    var offset = new Vector2(0f, -1f).RotatedBy(-0.314f + 0.314f * i) * 10f;
                    float rotation = offset.ToRotation();
                    Main.spriteBatch.Draw(spotlight, drawPosition + offset - screenPos, null, color, npc.rotation + rotation + MathHelper.PiOver2, spotlightOrigin, npc.scale * progress + 0.1f, SpriteEffects.None, 0f);
                }
            }

            Main.spriteBatch.Draw(texture, drawPosition - screenPos, npc.frame, new Color(200, 200, 200, 0), npc.rotation, orig, npc.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(legsTexture, drawPosition - screenPos, npc.frame, drawColor, npc.rotation, orig, npc.scale, SpriteEffects.None, 0f);
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
            if (Main.rand.NextBool(12))
                Item.NewItem(npc.getRect(), ItemID.LavaCharm);
            if (Main.rand.NextBool(3))
            {
                if (Main.rand.NextBool())
                {
                    Item.NewItem(npc.getRect(), EventDemonSiege.HellBanners[Main.rand.Next(EventDemonSiege.HellBanners.Count)]);
                }
                else
                {
                    Item.NewItem(npc.getRect(), ItemID.LavaLamp);
                }
            }
            if (Main.rand.NextBool(Main.expertMode ? 12 : 16) && EventDemonSiege.IsActive)
                Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Accessories.DegenerationRing>());
            if (Main.rand.NextBool())
                Item.NewItem(npc.getRect(), ModContent.ItemType<DemonicEnergy>());
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