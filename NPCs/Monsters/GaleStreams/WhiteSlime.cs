using AQMod.Dusts;
using AQMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters.GaleStreams
{
    public class WhiteSlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 20;
        }

        public override void SetDefaults()
        {
            npc.width = 38;
            npc.height = 26;
            npc.aiStyle = -1;
            npc.damage = 60;
            npc.defense = 10;
            npc.lifeMax = 210;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.alpha = 50;
            npc.lavaImmune = true;
            npc.trapImmune = true;
            npc.noGravity = true;
            npc.value = Item.buyPrice(silver: 10);
            npc.knockBackResist = 0.2f;
            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.CursedInferno] = true;
            npc.buffImmune[BuffID.ShadowFlame] = true;
            npc.buffImmune[BuffID.Confused] = false;
            npc.SetLiquidSpeed(water: 1f, lava: 1f);
            banner = npc.type;
            bannerItem = ModContent.ItemType<Items.Placeable.Banners.WhiteSlimeBanner>();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.75f);
            if (AQMod.SudoHardmode)
            {
                npc.lifeMax *= 2;
                npc.knockBackResist = 0f;
            }
        }

        public override void AI()
        {
            var dustRect = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);

            if ((int)npc.ai[1] == 1)
            {
                npc.noTileCollide = false;
                if (npc.velocity.Y == 0f)
                {
                    npc.velocity.X *= 0.8f;
                    dustRect.Y += dustRect.Height / 3 * 2;
                    dustRect.Height /= 3;
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
                        if (npc.velocity.X.Abs() <= 3f)
                        {
                            if (npc.localAI[0] == 0 && Main.netMode != NetmodeID.Server)
                            {
                                AQSound.Play(SoundType.NPCHit, AQSound.Paths.Boowomp, npc.Center, 0.9f);
                            }
                            npc.localAI[0]++;
                        }
                    }
                }
            }
            else
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    FunnyCOmeJoke();
                }

                if (npc.velocity.Y < -0.1f || (npc.HasValidTarget && npc.position.Y + npc.height - 2 < Main.player[npc.target].position.Y + Main.player[npc.target].height - 10))
                    npc.noTileCollide = true;
                else
                    npc.noTileCollide = false;

                int jumpTime = (int)(npc.ai[0] % 1000f);
                if (npc.velocity.Y == 0f)
                {
                    npc.velocity.X *= 0.8f;
                    bool incrementTimer = true;
                    if (jumpTime > 40f)
                    {
                        npc.TargetClosest();
                        npc.ai[1] = 0f;
                        npc.ai[0] += 960f;
                        if (npc.ai[0] > 5000f)
                        {
                            npc.ai[0] = -80f;
                            npc.ai[1] = 1f;
                            npc.velocity.Y += -18f;
                            npc.velocity.X += 6f * npc.direction;
                        }
                        else if (npc.ai[0] > 3000f && npc.ai[0] < 4000f)
                        {
                            npc.velocity.Y += -9.5f;
                            npc.velocity.X += 9f * npc.direction;
                        }
                        else
                        {
                            npc.velocity.Y += -13.5f;
                            npc.velocity.X += 6f * npc.direction;
                        }
                        npc.noTileCollide = true;
                        npc.netUpdate = true;
                        incrementTimer = false;
                    }
                    if (incrementTimer)
                    {
                        npc.ai[0] += 1.3f;
                        if (Main.expertMode)
                        {
                            npc.ai[0] += 2f;
                        }
                    }
                }
            }

            npc.velocity.Y += 0.5f;

            int d = Dust.NewDust(dustRect.TopLeft(), dustRect.Width, dustRect.Height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(120, 120, 120, 0));
            Main.dust[d].velocity *= 0.1f;
            Main.dust[d].scale = Main.rand.NextFloat(1f, 1.5f);
        }

        private void FunnyCOmeJoke()
        {
            var myRect = npc.getRect();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (i != npc.whoAmI && Main.npc[i].active)
                {
                    if (Main.npc[i].type == NPCID.MotherSlime)
                    {
                        if (Main.npc[i].getRect().Intersects(myRect))
                        {
                            var normal = Vector2.Normalize(npc.Center - Main.npc[i].Center);
                            npc.Center = Main.npc[i].Center + normal * (Main.npc[i].width + 12f);
                            npc.velocity += normal * 10f;
                            SoundID.Item86.Play(npc.Center);
                            int n = NPC.NewNPC((int)npc.position.X + npc.width / 2, (int)npc.position.Y + npc.height / 2, NPCID.BabySlime);
                            if (n > -1)
                            {
                                Main.npc[n].SpawnedFromStatue = true;
                                Main.npc[n].value = 0;
                            }
                        }
                    }
                    else if (Main.npc[i].netID == NPCID.BlackSlime)
                    {
                        if (Main.npc[i].getRect().Intersects(myRect))
                        {
                            var normal = Vector2.Normalize(npc.Center - Main.npc[i].Center);
                            npc.Center = Main.npc[i].Center + normal * 60f;
                            npc.velocity += normal * 10f;
                            SoundID.Item86.Play(npc.Center);
                            NPC n = new NPC();
                            n.SetDefaults(NPCID.MotherSlime);
                            float healthDifference = n.lifeMax / (float)Main.npc[i].lifeMax;
                            Main.npc[i].type = n.type;
                            Main.npc[i].netID = n.netID;
                            Main.npc[i].position.X = Main.npc[i].position.X + Main.npc[i].width / 2f;
                            Main.npc[i].position.Y = Main.npc[i].position.Y + Main.npc[i].height;
                            int life = (int)(Main.npc[i].life * healthDifference);
                            Main.npc[i].SetDefaults(n.type);
                            Main.npc[i].position.X -= Main.npc[i].width / 2f;
                            Main.npc[i].position.Y -= Main.npc[i].height;
                            Main.npc[i].life = life;
                        }
                    }
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (npc.velocity.Y != 0)
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
                        npc.frame.Y = frameHeight * 10 + frameHeight * (((int)npc.localAI[0] - 120) / 3);
                    }
                    else
                    {
                        npc.frame.Y = frameHeight * 10;
                    }
                }
                else
                {
                    npc.frame.Y = frameHeight * (((int)npc.localAI[0] / 3) + 3);
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
            drawPosition.Y -= 1.5f;
            var screenPos = Main.screenPosition;
            var orig = new Vector2(npc.frame.Width / 2f, npc.frame.Height / 2f);

            if (npc.frame.Y == npc.frame.Height)
                drawPosition.Y += 0.1f;

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
            if (npc.target != -1)
                Content.WorldEvents.GaleStreams.GaleStreams.ProgressEvent(Main.player[npc.target], 10);

            Item.NewItem(npc.getRect(), ItemID.Gel, Main.rand.Next(9) + 10);

            if (Main.rand.NextBool())
                Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Materials.Energies.AtmosphericEnergy>());

            if (AQMod.SudoHardmode && Main.rand.NextBool(4))
            {
                Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Weapons.Magic.Umystick>());
            }
            if (Main.rand.NextBool(8))
            {
                Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Foods.GaleStreams.CinnamonRoll>());
            }
        }
    }
}