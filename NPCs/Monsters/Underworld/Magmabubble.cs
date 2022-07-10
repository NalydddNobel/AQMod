using Aequus.Biomes;
using Aequus.Content.Necromancy;
using Aequus.Items.Misc.Energies;
using Aequus.Items.Placeable.Banners;
using Aequus.Projectiles.Monster;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters.Underworld
{
    public class Magmabubble : ModNPC
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

        public Asset<Texture2D> LegsTexture => ModContent.Request<Texture2D>(Texture + "Legs");

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 11;

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Position = new Vector2(1f, 0f)
            });

            NecromancyDatabase.NPCs.Add(Type, GhostInfo.Two);
        }

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 24;
            NPC.aiStyle = -1;
            NPC.damage = 20;
            NPC.defense = 16;
            NPC.lifeMax = 235;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.scale = 1.1f;
            NPC.alpha = 50;
            NPC.lavaImmune = true;
            NPC.trapImmune = true;
            NPC.value = 150f;
            NPC.knockBackResist = 0.7f;
            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.buffImmune[BuffID.OnFire] = true;
            NPC.buffImmune[BuffID.CursedInferno] = true;
            NPC.buffImmune[BuffID.Confused] = false;
            NPC.SetLiquidSpeeds(lava: 1f);

            Banner = NPC.type;
            BannerItem = ModContent.ItemType<MagmabubbleBanner>();

            this.SetBiome<DemonSiegeBiome>();
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            int count = 1;
            if (NPC.life <= 0)
            {
                count = 20;
                for (int i = -1; i <= 1; i++)
                {
                    NPC.DeathGore("Magmabubble_0", new Vector2(12f * i, 10f));
                    NPC.DeathGore("Magmabubble_1", new Vector2(12f * i, 0f));
                }
            }
            for (int i = 0; i < count; i++)
            {
                var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Torch);
                d.velocity = (d.position - NPC.Center) / 8f;
                if (Main.rand.NextBool(3))
                {
                    d.velocity *= 2f;
                    d.scale *= 1.75f;
                    d.fadeIn = d.scale + Main.rand.NextFloat(0.5f, 0.75f);
                    d.noGravity = true;
                }
            }
            for (int i = 0; i < count; i++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.FoodPiece, newColor: Color.DarkRed);
            }
        }

        private bool checkPlayerSights(int chances = 4)
        {
            if (Collision.CanHitLine(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].width))
            {
                return true;
            }
            else
            {
                NPC.ai[2]++;
                if ((int)NPC.ai[2] > chances)
                {
                    NPC.ai[1] = 3f;
                    NPC.ai[2] = 0f;
                    return false;
                }
                return true;
            }
        }

        public override void AI()
        {
            if (NPC.velocity.Y == 0f)
                NPC.velocity.X *= 0.8f;
            if ((int)NPC.ai[1] == 3)
            {
                NPC.ai[2]++;
                if (NPC.ai[2] > 60f)
                {
                    int cX = 0;
                    int cY = 0;
                    var player = Main.player[NPC.target];
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

                        if (Main.tile[x, y].HasTile && Main.tileSolid[Main.tile[x, y].TileType])
                            continue;
                        if (!Main.tile[x, y + 1].HasTile || !Main.tileSolid[Main.tile[x, y + 1].TileType])
                            continue;
                        int xOff = x - playerX;
                        int yOff = y - playerY;
                        if (Math.Sqrt(xOff * xOff + yOff * yOff) < 4.0)
                            continue;
                        cX = x;
                        cY = y;
                        break;
                    }
                    NPC.position = new Vector2(cX * 16f + 8f - NPC.width / 2f, cY * 16f - NPC.height + 12f);
                    NPC.velocity *= 0.1f;
                    NPC.TargetClosest();
                    NPC.velocity.X += 3f * NPC.direction;
                    NPC.velocity.Y -= 4f;
                    NPC.netUpdate = true;
                    NPC.ai[0] = 2000f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    for (int i = 0; i < 5; i++)
                    {
                        int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch);
                        Main.dust[d].velocity = new Vector2(0f, 5f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                    }
                    for (int i = 0; i < 30; i++)
                    {
                        int d = Dust.NewDust(NPC.position + new Vector2(0f, -8f), NPC.width, NPC.height, DustID.Torch);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity.Y *= 0.2f;
                        Main.dust[d].velocity.X *= 3f;
                        Main.dust[d].scale *= Main.rand.NextFloat(0.5f, 2f);
                    }
                }
                for (int i = 0; i < 3; i++)
                {
                    int d = Dust.NewDust(NPC.position + new Vector2(0f, -8f), NPC.width, NPC.height, DustID.Torch);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity.Y *= 0.2f;
                    Main.dust[d].velocity.X *= 3f;
                    Main.dust[d].scale *= Main.rand.NextFloat(0.5f, 2f);
                }
            }
            else if ((int)NPC.ai[1] == 1)
            {
                NPC.localAI[0]++;
                if (NPC.localAI[0] >= 36f)
                {
                    NPC.localAI[0] = 0f;
                    NPC.ai[1] = -1f;
                    NPC.netUpdate = true;
                }
            }
            else
            {
                if (NPC.collideY)
                {
                    if ((int)NPC.ai[1] == 0)
                    {
                        NPC.ai[1] = 1f;
                        NPC.netUpdate = true;
                        return;
                    }
                }
                bool incrementTimer = true;
                if (NPC.ai[0] <= -20f)
                {
                    if (NPC.velocity.Y == 0f)
                    {
                        int timer = (int)(-NPC.ai[0] - 20);
                        if (timer <= 40 && (int)NPC.ai[1] != 2)
                        {
                            NPC.ai[1] = 2f;
                            checkPlayerSights(chances: 6);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int projectileType = ModContent.ProjectileType<MagmabubbleBullet>();
                                var spawnPosition = NPC.Center;
                                int damage = 20;
                                float speed = 7f;
                                if (Main.expertMode)
                                {
                                    damage = 15;
                                    speed = 11f;
                                }
                                SoundEngine.PlaySound(SoundID.Item85, spawnPosition);
                                for (int i = 0; i < 3; i++)
                                {
                                    var velocity = new Vector2(0f, -1f).RotatedBy(-0.314f + 0.314f * i) * speed;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPosition, velocity, projectileType, damage, 1f, Main.myPlayer);
                                }
                            }
                        }
                        else if (timer == 0)
                        {
                            NPC.ai[1] = 0f;
                            checkPlayerSights(chances: 6);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int projectileType = ModContent.ProjectileType<MagmabubbleBullet>();
                                var spawnPosition = NPC.Center;
                                int damage = 15;
                                float speed = 6f;
                                if (Main.expertMode)
                                {
                                    damage = 20;
                                    speed = 9f;
                                }
                                SoundEngine.PlaySound(SoundID.Item85, spawnPosition);
                                for (int i = 0; i < 3; i++)
                                {
                                    var velocity = new Vector2(0f, -1f).RotatedBy(-0.314f + 0.314f * i) * speed;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPosition, velocity, projectileType, damage, 1f, Main.myPlayer);
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
                    int jumpTime = (int)(NPC.ai[0] % 1000f);
                    if (jumpTime > 40f)
                    {
                        incrementTimer = false;
                        if (NPC.velocity.Y >= 0f)
                        {
                            checkPlayerSights(chances: 8);
                            NPC.TargetClosest();
                            NPC.ai[1] = 0f;
                            NPC.ai[0] += 960f;
                            if (NPC.ai[0] > 3000f)
                                NPC.ai[0] = -100f;
                            NPC.velocity.Y += -7f;
                            NPC.velocity.X += 4f * NPC.direction;
                            NPC.netUpdate = true;
                        }
                    }
                }
                if (incrementTimer)
                    NPC.ai[0]++;
            }
            if (Main.rand.NextBool(3))
                Dust.NewDust(NPC.position + new Vector2(0f, -8f), NPC.width, NPC.height, DustID.Torch);
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.Y != 0 && !NPC.collideY)
            {
                if (NPC.velocity.Y.Abs() < 2f)
                {
                    NPC.frame.Y = frameHeight * FRAME_JUMPGOINGDOWN;
                }
                else if (NPC.velocity.Y < 0f)
                {
                    NPC.frame.Y = frameHeight * FRAME_JUMPUP;
                }
                else
                {
                    NPC.frame.Y = frameHeight * FRAME_JUMPDOWN;
                }
            }
            else if ((int)NPC.ai[1] == 1)
            {
                NPC.frame.Y = frameHeight * (FRAME_JUMPRECOIL0 + (int)(NPC.localAI[0] / 6f));
            }
            else
            {
                int jumpTime = (int)(NPC.ai[0] % 1000f);
                NPC.frameCounter += 1.0d;
                if (NPC.frameCounter >= 6.0d)
                {
                    NPC.frameCounter = 0.0d;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y >= frameHeight * 2)
                        NPC.frame.Y = 0;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            this.CreateLoot(npcLoot)
                .Add<DemonicEnergy>(chance: 20, stack: 1)
                .Add(ItemID.LavaCharm, chance: 16, stack: 1);
        }
        //public override void NPCLoot()
        //{
        //    if (!DemonSiege.IsActive)
        //    {
        //        return;
        //    }
        //    if (Main.rand.NextBool(Main.expertMode ? 12 : 16))
        //        Item.NewItem(NPC.getRect(), ModContent.ItemType<DegenerationRing>());
        //}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var texture = TextureAssets.Npc[Type].Value;
            var legsTexture = LegsTexture.Value;
            var drawPosition = new Vector2(NPC.position.X + NPC.width / 2f, NPC.position.Y + NPC.height / 2f);
            drawPosition.Y -= 10.5f;
            var orig = new Vector2(NPC.frame.Width / 2f, NPC.frame.Height / 2f);

            if (NPC.ai[0] < -20f)
            {
                int timer = (int)(-NPC.ai[0] - 20);
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
                var spotlight = TextureCache.Bloom[0].Value;
                var color = Color.Lerp(Color.Red, Color.OrangeRed, ((float)Math.Sin(Main.GlobalTimeWrappedHourly * 25f) + 1f) / 2f);
                color *= progress;
                var spotlightOrigin = spotlight.Size() / 2f;
                Main.spriteBatch.Draw(spotlight, drawPosition + new Vector2(0f, 2f) - screenPos, null, color, NPC.rotation, spotlightOrigin, (NPC.scale * progress + 0.1f) * 0.6f, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture, drawPosition - screenPos, NPC.frame, new Color(200, 200, 200, 0), NPC.rotation, orig, NPC.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(legsTexture, drawPosition - screenPos, NPC.frame, drawColor, NPC.rotation, orig, NPC.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool? CanFallThroughPlatforms()
        {
            if (Main.player[NPC.target].dead)
            {
                return true;
            }
            else
            {
                return Main.player[NPC.target].position.Y
                    > NPC.position.Y + NPC.height;
            }
        }
    }
}