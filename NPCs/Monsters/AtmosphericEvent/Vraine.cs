using AQMod.Common.Utilities;
using AQMod.Items.Placeable.Banners;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters.AtmosphericEvent
{
    public class Vraine : ModNPC
    {
        public const int FramesX = 2;

        private bool _setupFrame; // no need to sync this since find frame stuff is client only (I think)
        private int _transparency;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 16;

            NPCID.Sets.TrailingMode[npc.type] = 7;
            NPCID.Sets.TrailCacheLength[npc.type] = 20;
        }

        public override void SetDefaults()
        {
            npc.width = 46;
            npc.height = 36;
            npc.lifeMax = 125;
            npc.damage = 45;
            npc.defense = 15;
            npc.HitSound = SoundID.DD2_GoblinHurt;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.aiStyle = -1;
            npc.noGravity = true;
            npc.knockBackResist = 0.1f;
            npc.value = Item.buyPrice(silver: 3);
            npc.buffImmune[BuffID.OnFire] = true;
            banner = npc.type;
            bannerItem = ModContent.ItemType<VraineBanner>();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 1.3f);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, npc.velocity.X * 0.25f, npc.velocity.Y * 0.25f, 0, default(Color), Main.rand.NextFloat(1f, 1.45f));
                }
                if ((int)npc.ai[1] == 1)
                {
                    Gore.NewGore(npc.Center + Vector2.Normalize(npc.oldVelocity) * npc.width / 2f, npc.velocity * 0.25f, mod.GetGoreSlot("Gores/Brollow_1"));
                }
                else
                {
                    Gore.NewGore(npc.Center + Vector2.Normalize(npc.oldVelocity) * npc.width / 2f, npc.velocity * 0.25f, mod.GetGoreSlot("Gores/Brollow_0"));
                }
                Gore.NewGore(npc.Center, npc.velocity.RotatedBy(-0.1f) * 0.25f, mod.GetGoreSlot("Gores/Brollow_2"));
                Gore.NewGore(npc.Center, npc.velocity.RotatedBy(0.1f) * 0.25f, mod.GetGoreSlot("Gores/Brollow_3"));
            }
            else
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, npc.velocity.X, npc.velocity.Y);
            }
        }

        public override void AI() // ai[1] is temperature (1 = hot, 2 = cold)
        {
            if ((int)npc.ai[1] == 0)
            {
                npc.TargetClosest(faceTarget: false);
                npc.ai[1] = 2f;
                npc.ai[2] = -1f;
                int count = Main.rand.Next(3) + 1;
                int npcX = (int)npc.position.X + npc.width / 2;
                int npcY = (int)npc.position.Y + npc.height / 2;
                int lastNPC = npc.whoAmI;
                int lastNPC2 = npc.whoAmI;
                for (int i = 0; i < count; i++)
                {
                    int n = NPC.NewNPC(npcX + npc.width * (i + 1), npcY, npc.type, npc.whoAmI);
                    Main.npc[n].ai[1] = npc.ai[1];
                    Main.npc[n].ai[2] = lastNPC;
                    Main.npc[n].localAI[0] = npc.localAI[0];
                    Main.npc[n].target = npc.target;
                    lastNPC = n;
                    n = NPC.NewNPC(npcX - npc.width * (i + 1), npcY, npc.type, npc.whoAmI);
                    Main.npc[n].ai[1] = npc.ai[1];
                    Main.npc[n].ai[2] = lastNPC2;
                    Main.npc[n].localAI[0] = npc.localAI[0];
                    Main.npc[n].target = npc.target;
                    lastNPC2 = n;
                }
                npc.netUpdate = true;
            }

            if (npc.collideX && npc.oldVelocity.X.Abs() > 2f)
                npc.velocity.X = -npc.oldVelocity.X * 0.8f;
            if (npc.collideY && npc.oldVelocity.Y.Abs() > 2f)
                npc.velocity.Y = -npc.oldVelocity.Y * 0.8f;

            bool hot = (int)npc.ai[1] == 1;
            var center = npc.Center;
            if (npc.ai[3] > 0f)
                npc.ai[3]--;
            if ((int)npc.ai[2] == -1) // leader
            {
                if (hot)
                {
                    Vector2 difference = Main.player[npc.target].Center - center;
                    float lerpAmount = 0.01f;
                    if (npc.ai[3] <= 0f && difference.Length() > 460f)
                    {
                        npc.ai[1] = 2f;
                        _transparency = 100;
                        npc.ai[3] = _transparency;
                    }
                    float length = npc.velocity.Length();
                    if (Main.expertMode)
                    {
                        if (length < 4.85f)
                        {
                            length = 4.85f;
                        }
                    }
                    else
                    {
                        if (length < 2.65f)
                        {
                            length = 2.65f;
                        }
                    }
                    npc.velocity = Vector2.Normalize(Vector2.Lerp(npc.velocity, difference, lerpAmount)) * length;
                }
                else
                {
                    Vector2 difference = Main.player[npc.target].Center - center;
                    if (difference.Y < 620f && difference.Y > 480f)
                    {
                        npc.velocity = Vector2.Normalize(difference) * (Main.expertMode ? 7.5f : 4.85f);
                        npc.ai[1] = 1f;
                        _transparency = 100;
                        npc.ai[3] = _transparency;
                        npc.netUpdate = true;

                        Main.PlaySound(SoundID.Item1, npc.Center);
                    }
                    else if (difference.Length() < 360f)
                    {
                        npc.ai[1] = 1f;
                        _transparency = 60;
                        npc.ai[3] = _transparency;
                        npc.netUpdate = true;
                    }

                    float maxSpeedY = -8f;
                    if (Main.player[npc.target].velocity.Y < -8f)
                    {
                        maxSpeedY = Main.player[npc.target].velocity.Y;
                    }
                    if (npc.velocity.Y < -maxSpeedY)
                    {
                        npc.velocity.Y += Main.rand.NextFloat(-0.0025f, -0.1f);
                    }

                    float diffX = Main.player[npc.target].position.X + Main.player[npc.target].width / 2f - (npc.position.X + npc.width / 2f);
                    if (Main.player[npc.target].position.X + Main.player[npc.target].width / 2f < npc.position.X)
                    {
                        if (npc.velocity.X > -2f)
                        {
                            npc.velocity.X -= 0.075f;
                        }
                    }
                    else
                    {
                        if (npc.velocity.X < 2f)
                        {
                            npc.velocity.X += 0.075f;
                        }
                    }
                }
            }
            else
            {
                var leader = Main.npc[(int)npc.ai[2]];
                if (!leader.active)
                {
                    int closestNPC = -1;
                    float closestDistance = npc.width * 1.75f;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (i != npc.whoAmI && Main.npc[i].active && Main.npc[i].type == npc.type && (int)Main.npc[i].ai[2] != npc.whoAmI)
                        {
                            var difference = Main.npc[i].Center - center;
                            float differenceLength = difference.Length();
                            if (differenceLength < closestDistance)
                            {
                                closestNPC = i;
                                closestDistance = differenceLength;
                            }
                        }
                    }
                    if (closestNPC == -1)
                    {
                        npc.ai[2] = -1f;
                        return;
                    }
                    npc.ai[2] = closestNPC;
                }
                var leaderMod = (Vraine)leader.modNPC;
                npc.ai[1] = leader.ai[1];
                npc.ai[3] = leader.ai[3];
                _transparency = leaderMod._transparency;
                int offsetX = npc.width;
                if (npc.position.X + npc.width / 2f < leader.position.X + leader.width / 2f)
                {
                    offsetX = -offsetX;
                }
                var gotoPos = new Vector2(leader.position.X + leader.width / 2f + offsetX, leader.position.Y + leader.height / 2f);
                npc.velocity = gotoPos - center;
            }
            npc.rotation = npc.velocity.ToRotation();
        }

        public override void FindFrame(int frameHeight)
        {
            if (!_setupFrame)
            {
                _setupFrame = true;
                npc.frame.Width /= FramesX;
            }
            npc.frame.X = (int)(npc.frame.Width * (npc.ai[1] - 1));
            npc.frameCounter += 1.0d;
            if (npc.frameCounter > 4.0d)
            {
                npc.frameCounter = 0.0d;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y / frameHeight >= Main.npcFrameCount[npc.type])
                {
                    npc.frame.Y = 0;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = Main.npcTexture[npc.type];
            var offset = new Vector2(npc.width / 2f, npc.height / 2f);
            Vector2 origin = npc.frame.Size() / 2f;
            Vector2 drawPos = npc.Center - Main.screenPosition;

            if ((int)npc.localAI[0] == 0)
            {
                float mult = 1f / NPCID.Sets.TrailCacheLength[npc.type];
                var trailColor = drawColor * 0.1f;
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
                {
                    if ((int)npc.oldPos[i].X == 0)
                        break;
                    Main.spriteBatch.Draw(texture, npc.oldPos[i] + offset - Main.screenPosition, npc.frame, trailColor * (mult * (NPCID.Sets.TrailCacheLength[npc.type] - i)), npc.oldRot[i], origin, npc.scale, SpriteEffects.None, 0f);
                }
            }

            if (npc.ai[3] > 0f && _transparency > 0)
            {
                var frame = npc.frame;
                if (npc.ai[1] == 1)
                    frame.X = npc.frame.Width;
                else
                    frame.X = 0;

                    float progress = npc.ai[3] / _transparency;
                Main.spriteBatch.Draw(texture, drawPos, frame, Color.Lerp(drawColor, new Color(0, 0, 0, 0), 1f - progress), npc.rotation, origin, npc.scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, drawPos, npc.frame, Color.Lerp(drawColor, new Color(0, 0, 0, 0), progress), npc.rotation, origin, npc.scale, SpriteEffects.None, 0f);
            }
            else
            {
                Main.spriteBatch.Draw(texture, drawPos, npc.frame, drawColor, npc.rotation, origin, npc.scale, SpriteEffects.None, 0f);
            }

            return false;
        }
    }
}