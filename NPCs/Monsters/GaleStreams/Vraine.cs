using AQMod.Common;
using AQMod.Content.World;
using AQMod.Gores;
using AQMod.Items.Dyes;
using AQMod.Items.Placeable.Banners;
using AQMod.Items.Potions;
using AQMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters.GaleStreams
{
    public class Vraine : ModNPC, IDecideFallThroughPlatforms
    {
        public const int Temperature = 40;
        public int transitionMax;

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
            npc.lifeMax = 160;
            npc.damage = 40;
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
            npc.noTileCollide = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            if (WorldDefeats.SudoHardmode)
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.375f);
            }
            else
            {
                npc.lifeMax = (int)(npc.lifeMax * 0.8f);
                npc.damage = (int)(npc.damage * 0.8f);
            }
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
                    AQGore.NewGore(npc.Center + Vector2.Normalize(npc.oldVelocity) * npc.width / 2f,
                        npc.velocity * 0.25f, "GaleStreams/brollow_1");
                }
                else
                {
                    AQGore.NewGore(npc.Center + Vector2.Normalize(npc.oldVelocity) * npc.width / 2f,
                        npc.velocity * 0.25f, "GaleStreams/brollow_0");
                }
                AQGore.NewGore(npc.getRect(), npc.velocity.RotatedBy(-0.1f) * 0.25f, "GaleStreams/brollow_2");
                AQGore.NewGore(npc.getRect(), npc.velocity.RotatedBy(0.1f) * 0.25f, "GaleStreams/brollow_3");
            }
            else
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, npc.velocity.X, npc.velocity.Y);
            }
        }

        public override void AI() // ai[1] is temperature (1 = hot, 2 = cold)
        {
            if ((int)npc.ai[1] == -1)
            {
                if (npc.timeLeft > 100)
                {
                    npc.timeLeft = 100;
                    transitionMax = 100;
                    npc.ai[3] = transitionMax;
                    npc.netUpdate = true;
                }
                npc.velocity.Y -= 0.25f;
                npc.GetGlobalNPC<NPCTemperatureManager>().temperature = 0;
                return;
            }
            if ((int)npc.ai[1] == 0)
            {
                npc.TargetClosest(faceTarget: false);
                if (!npc.HasValidTarget)
                {
                    npc.ai[1] = -1f;
                    return;
                }
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
            else if (!npc.HasValidTarget)
            {
                npc.ai[1] = -1f;
            }

            if (npc.collideX && npc.oldVelocity.X.Abs() > 2f)
                npc.velocity.X = -npc.oldVelocity.X * 0.8f;
            if (npc.collideY && npc.oldVelocity.Y.Abs() > 2f)
                npc.velocity.Y = -npc.oldVelocity.Y * 0.8f;

            bool hot = (int)npc.ai[1] == 1;
            npc.coldDamage = !hot;
            var center = npc.Center;
            if (npc.ai[3] > 0f)
            {
                npc.ai[3]--;
                if (npc.ai[3] <= 0)
                {
                    npc.ai[3] = 0f;
                    npc.GetGlobalNPC<NPCTemperatureManager>().temperature = (sbyte)(Temperature * (hot ? 1 : -1));
                    npc.netUpdate = true;
                }
                else if (transitionMax != 0)
                {
                    npc.GetGlobalNPC<NPCTemperatureManager>().temperature = (sbyte)(int)MathHelper.Lerp(npc.GetGlobalNPC<NPCTemperatureManager>().temperature, Temperature * (hot ? 1 : -1), 1f - npc.ai[3] / transitionMax);
                }
            }
            if ((int)npc.ai[2] == -1) // leader
            {
                if (hot)
                {
                    var plrCenter = Main.player[npc.target].Center;
                    Vector2 difference = plrCenter - center;
                    float lerpAmount = 0.01f;
                    if (npc.ai[3] <= 0f && difference.Length() > 460f)
                    {
                        npc.ai[1] = 2f;
                        transitionMax = 100;
                        npc.ai[3] = transitionMax;
                        npc.netUpdate = true;
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
                    npc.direction = npc.velocity.X > 0f ? 1 : -1;
                    if (npc.direction == 1)
                    {
                        if (center.X > plrCenter.X)
                        {
                            npc.ai[1] = 2f;
                            transitionMax = 120;
                            npc.ai[3] = transitionMax;
                            npc.netUpdate = true;
                        }
                    }
                    else
                    {
                        if (center.X < plrCenter.X)
                        {
                            npc.ai[1] = 2f;
                            transitionMax = 120;
                            npc.ai[3] = transitionMax;
                            npc.netUpdate = true;
                        }
                    }
                }
                else
                {
                    if (npc.ai[3] <= 0f)
                    {
                        Vector2 difference = Main.player[npc.target].Center - center;
                        if (npc.ai[3] < 0f)
                        {
                            var gotoVeloc = Vector2.Normalize(difference) * (Main.expertMode ? 7.5f : 4.85f);
                            npc.ai[3]++;
                            npc.velocity = Vector2.Lerp(npc.velocity, gotoVeloc, 0.1f);
                            if (npc.ai[3] == -1)
                            {
                                transitionMax = 100;
                                npc.ai[1] = 1f;
                                npc.ai[3] = transitionMax;
                                npc.velocity = gotoVeloc;
                                Main.PlaySound(SoundID.Item1, npc.Center);
                            }
                        }
                        else
                        {
                            if (difference.Y < 500f && difference.Y > 375f)
                            {
                                npc.netUpdate = true;
                                npc.ai[3] = -20f;
                            }
                            else if (difference.Y > 200f)
                            {
                                npc.velocity.Y *= 0.98f;
                            }
                            else if (difference.Length() < 360f)
                            {
                                npc.ai[1] = 1f;
                                transitionMax = 60;
                                npc.ai[3] = transitionMax;
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
                                    npc.velocity.X -= 0.035f;
                                }
                            }
                            else
                            {
                                if (npc.velocity.X < 2f)
                                {
                                    npc.velocity.X += 0.035f;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (npc.velocity.Y > 0f)
                        {
                            npc.velocity.Y += Main.rand.NextFloat(-0.0025f, -0.1f);
                        }
                        if (npc.velocity.X > -2f && npc.velocity.X < 2f)
                        {
                            npc.velocity.X += 0.035f * npc.direction;
                        }
                        else if (npc.velocity.X < -3f && npc.velocity.X > 3f)
                        {
                            npc.velocity.X *= 0.99f;
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
                transitionMax = leaderMod.transitionMax;

                if ((int)leader.ai[2] == -1)
                {
                    npc.direction = 1;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (i == npc.whoAmI)
                        {
                            break;
                        }
                        if (Main.npc[i].active && Main.npc[i].type == npc.type && i < npc.whoAmI && (int)Main.npc[i].ai[2] == (int)npc.ai[2])
                        {
                            npc.direction = -1;
                            break;
                        }
                    }
                }
                else
                {
                    npc.direction = leader.direction;
                }
                var gotoPosition = leader.Center + new Vector2(-npc.width / 2f, npc.height * npc.direction * 1.5f).RotatedBy(leader.rotation);
                var difference2 = (gotoPosition - center);
                float distance = difference2.Length();
                if (distance < 10f)
                {
                    npc.velocity = Vector2.Lerp(npc.velocity, leader.velocity, 0.8f);
                }
                else
                {
                    float speed = npc.velocity.Length();
                    float leaderSpeed = leader.velocity.Length();
                    if (speed < leaderSpeed * 1.6f)
                    {
                        speed = leaderSpeed * 1.6f;
                    }
                    else if (speed < 2.5f)
                    {
                        speed = 2.5f;
                    }
                    npc.velocity = Vector2.Lerp(npc.velocity, Vector2.Normalize(difference2) * speed, 0.025f);
                }
            }
            npc.rotation = npc.velocity.ToRotation();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(transitionMax);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            transitionMax = reader.ReadInt32();
        }

        public override void FindFrame(int frameHeight)
        {
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

        public override void NPCLoot()
        {
            if (npc.target != -1)
                EventGaleStreams.ProgressEvent(Main.player[npc.target], 2);
            bool anyOthers = NPC.AnyNPCs(npc.type);
            if (!anyOthers)
            {
                if (Main.hardMode && Main.rand.NextBool(30))
                {
                    Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Materials.Energies.AtmosphericEnergy>());
                }
                if (Main.hardMode && Main.rand.NextBool(5))
                {
                    Item.NewItem(npc.getRect(), ModContent.ItemType<Vrang>(), Main.rand.NextVRand(1, 2));
                }
                if (Main.rand.NextBool(10))
                {
                    Item.NewItem(npc.getRect(), ModContent.ItemType<CensorDye>(), Main.rand.NextVRand(1, 3));
                }
            }
            if (Main.rand.NextBool(100))
                Item.NewItem(npc.getRect(), ModContent.ItemType<PeeledCarrot>(), Main.rand.NextVRand(1, 3));
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

            Main.spriteBatch.Draw(texture, drawPos, npc.frame, drawColor, npc.rotation, origin, npc.scale, SpriteEffects.None, 0f);

            if (npc.ai[3] > 0f && transitionMax > 0)
            {
                Texture2D overlayTexture;
                float progress = npc.ai[3] / transitionMax;
                progress *= 2f;
                progress -= 1f;
                if ((int)npc.ai[1] == -1)
                {
                    progress = 1f - progress;
                    overlayTexture = ModContent.GetTexture(this.GetPath("_Hot"));
                }
                else if ((int)npc.ai[1] == 1)
                {
                    overlayTexture = ModContent.GetTexture(this.GetPath("_Hot"));
                }
                else
                {
                    overlayTexture = ModContent.GetTexture(this.GetPath("_Cold"));
                }
                Main.spriteBatch.Draw(overlayTexture, drawPos, npc.frame, Color.Lerp(drawColor, new Color(0, 0, 0, 0), progress), npc.rotation, origin, npc.scale, SpriteEffects.None, 0f);
            }

            return false;
        }

        bool IDecideFallThroughPlatforms.Decide()
        {
            return true;
        }
    }
}