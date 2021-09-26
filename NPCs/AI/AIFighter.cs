using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.AI
{
    public abstract class AIFighter : AIClone
    {
        public sealed override void BaseAI()
        {
            if (npc.type == NPCID.Psycho)
            {
                int num = 200;
                if (npc.ai[2] == 0f)
                {
                    npc.alpha = num;
                    npc.TargetClosest();
                    if (!Main.player[npc.target].dead && (Main.player[npc.target].Center - npc.Center).Length() < 170f)
                    {
                        npc.ai[2] = -16f;
                    }
                    if (npc.velocity.X != 0f || npc.velocity.Y < 0f || npc.velocity.Y > 2f || npc.justHit)
                    {
                        npc.ai[2] = -16f;
                    }
                    return;
                }
                if (npc.ai[2] < 0f)
                {
                    if (npc.alpha > 0)
                    {
                        npc.alpha -= num / 16;
                        if (npc.alpha < 0)
                        {
                            npc.alpha = 0;
                        }
                    }
                    npc.ai[2] += 1f;
                    if (npc.ai[2] == 0f)
                    {
                        npc.ai[2] = 1f;
                        npc.velocity.X = npc.direction * 2;
                    }
                    return;
                }
                npc.alpha = 0;
            }
            if (npc.type == 166)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.Next(240) == 0)
                {
                    npc.ai[2] = Main.rand.Next(-480, -60);
                    npc.netUpdate = true;
                }
                if (npc.ai[2] < 0f)
                {
                    npc.TargetClosest();
                    if (npc.justHit)
                    {
                        npc.ai[2] = 0f;
                    }
                    if (Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                    {
                        npc.ai[2] = 0f;
                    }
                }
                if (npc.ai[2] < 0f)
                {
                    npc.velocity.X *= 0.9f;
                    if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
                    {
                        npc.velocity.X = 0f;
                    }
                    npc.ai[2] += 1f;
                    if (npc.ai[2] == 0f)
                    {
                        npc.velocity.X = (float)npc.direction * 0.1f;
                    }
                    return;
                }
            }
            if (npc.type == NPCID.CreatureFromTheDeep)
            {
                if (npc.wet)
                {
                    npc.knockBackResist = 0f;
                    npc.ai[3] = -0.10101f;
                    npc.noGravity = true;
                    Vector2 center = npc.Center;
                    npc.width = 34;
                    npc.height = 24;
                    npc.position.X = center.X - (float)(npc.width / 2);
                    npc.position.Y = center.Y - (float)(npc.height / 2);
                    npc.TargetClosest();
                    if (npc.collideX)
                    {
                        npc.velocity.X = 0f - npc.oldVelocity.X;
                    }
                    if (npc.velocity.X < 0f)
                    {
                        npc.direction = -1;
                    }
                    if (npc.velocity.X > 0f)
                    {
                        npc.direction = 1;
                    }
                    if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].Center, 1, 1))
                    {
                        Vector2 value = Main.player[npc.target].Center - npc.Center;
                        value.Normalize();
                        value *= 5f;
                        npc.velocity = (npc.velocity * 19f + value) / 20f;
                        return;
                    }
                    float num101 = 5f;
                    if (npc.velocity.Y > 0f)
                    {
                        num101 = 3f;
                    }
                    if (npc.velocity.Y < 0f)
                    {
                        num101 = 8f;
                    }
                    Vector2 value2 = new Vector2(npc.direction, -1f);
                    value2.Normalize();
                    value2 *= num101;
                    if (num101 < 5f)
                    {
                        npc.velocity = (npc.velocity * 24f + value2) / 25f;
                    }
                    else
                    {
                        npc.velocity = (npc.velocity * 9f + value2) / 10f;
                    }
                    return;
                }
                npc.knockBackResist = 0.4f * Main.knockBackMultiplier;
                npc.noGravity = false;
                Vector2 center2 = npc.Center;
                npc.width = 18;
                npc.height = 40;
                npc.position.X = center2.X - (float)(npc.width / 2);
                npc.position.Y = center2.Y - (float)(npc.height / 2);
                if (npc.ai[3] == -0.10101f)
                {
                    npc.ai[3] = 0f;
                    float num112 = npc.velocity.Length();
                    num112 *= 2f;
                    if (num112 > 10f)
                    {
                        num112 = 10f;
                    }
                    npc.velocity.Normalize();
                    npc.velocity *= num112;
                    if (npc.velocity.X < 0f)
                    {
                        npc.direction = -1;
                    }
                    if (npc.velocity.X > 0f)
                    {
                        npc.direction = 1;
                    }
                    npc.spriteDirection = npc.direction;
                }
            }
            if (npc.type == NPCID.CultistArcherBlue || npc.type == NPCID.CultistArcherWhite)
            {
                if (npc.ai[3] < 0f)
                {
                    npc.damage = 0;
                    npc.velocity.X *= 0.93f;
                    if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
                    {
                        npc.velocity.X = 0f;
                    }
                    int num121 = (int)(0f - npc.ai[3] - 1f);
                    int num132 = Math.Sign(Main.npc[num121].Center.X - npc.Center.X);
                    if (num132 != npc.direction)
                    {
                        npc.velocity.X = 0f;
                        npc.direction = num132;
                        npc.netUpdate = true;
                    }
                    if (npc.justHit && Main.netMode != NetmodeID.MultiplayerClient && Main.npc[num121].localAI[0] == 0f)
                    {
                        Main.npc[num121].localAI[0] = 1f;
                    }
                    if (npc.ai[0] < 1000f)
                    {
                        npc.ai[0] = 1000f;
                    }
                    if ((npc.ai[0] += 1f) >= 1300f)
                    {
                        npc.ai[0] = 1000f;
                        npc.netUpdate = true;
                    }
                    return;
                }
                if (npc.ai[0] >= 1000f)
                {
                    npc.ai[0] = 0f;
                }
                npc.damage = npc.defDamage;
            }
            if (npc.type == 383 && npc.ai[2] == 0f && npc.localAI[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int num143 = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, 384, npc.whoAmI);
                npc.ai[2] = num143 + 1;
                npc.localAI[0] = -1f;
                npc.netUpdate = true;
                Main.npc[num143].ai[0] = npc.whoAmI;
                Main.npc[num143].netUpdate = true;
            }
            if (npc.type == NPCID.MartianOfficer)
            {
                int num154 = (int)npc.ai[2] - 1;
                if (num154 != -1 && Main.npc[num154].active && Main.npc[num154].type == NPCID.ForceBubble)
                {
                    npc.dontTakeDamage = true;
                }
                else
                {
                    npc.dontTakeDamage = false;
                    npc.ai[2] = 0f;
                    if (npc.localAI[0] == -1f)
                    {
                        npc.localAI[0] = 180f;
                    }
                    if (npc.localAI[0] > 0f)
                    {
                        npc.localAI[0] -= 1f;
                    }
                }
            }
            if (npc.type == 482)
            {
                int num165 = 300;
                int num176 = 120;
                npc.dontTakeDamage = false;
                if (npc.ai[2] < 0f)
                {
                    npc.dontTakeDamage = true;
                    npc.ai[2] += 1f;
                    npc.velocity.X *= 0.9f;
                    if ((double)Math.Abs(npc.velocity.X) < 0.001)
                    {
                        npc.velocity.X = 0.001f * (float)npc.direction;
                    }
                    if (Math.Abs(npc.velocity.Y) > 1f)
                    {
                        npc.ai[2] += 10f;
                    }
                    if (npc.ai[2] >= 0f)
                    {
                        npc.netUpdate = true;
                        npc.velocity.X += (float)npc.direction * 0.3f;
                    }
                    return;
                }
                if (npc.ai[2] < (float)num165)
                {
                    if (npc.justHit)
                    {
                        npc.ai[2] += 15f;
                    }
                    npc.ai[2] += 1f;
                }
                else if (npc.velocity.Y == 0f)
                {
                    npc.ai[2] = -num176;
                    npc.netUpdate = true;
                }
            }
            if (npc.type == NPCID.Medusa)
            {
                int num2 = 180;
                int num13 = 300;
                int num24 = 180;
                int num35 = 60;
                int num46 = 20;
                if (npc.life < npc.lifeMax / 3)
                {
                    num2 = 120;
                    num13 = 240;
                    num24 = 240;
                    num35 = 90;
                }
                if (npc.ai[2] > 0f)
                {
                    npc.ai[2] -= 1f;
                }
                else if (npc.ai[2] == 0f)
                {
                    if (((Main.player[npc.target].Center.X < npc.Center.X && npc.direction < 0) || (Main.player[npc.target].Center.X > npc.Center.X && npc.direction > 0)) && npc.velocity.Y == 0f && npc.Distance(Main.player[npc.target].Center) < 900f && Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                    {
                        npc.ai[2] = -num24 - num46;
                        npc.netUpdate = true;
                    }
                }
                else
                {
                    if (npc.ai[2] < 0f && npc.ai[2] < (float)(-num24))
                    {
                        npc.velocity.X *= 0.9f;
                        if (npc.velocity.Y < -2f || npc.velocity.Y > 4f || npc.justHit)
                        {
                            npc.ai[2] = num2;
                        }
                        else
                        {
                            npc.ai[2] += 1f;
                            if (npc.ai[2] == 0f)
                            {
                                npc.ai[2] = num13;
                            }
                        }
                        float num57 = npc.ai[2] + (float)num24 + (float)num46;
                        if (num57 == 1f)
                        {
                            Main.PlaySound(4, (int)npc.position.X, (int)npc.position.Y, 17);
                        }
                        if (num57 < (float)num46)
                        {
                            Vector2 vector = npc.Top + new Vector2(npc.spriteDirection * 6, 6f);
                            float scaleFactor = MathHelper.Lerp(20f, 30f, (num57 * 3f + 50f) / 182f);
                            Main.rand.NextFloat();
                            for (float num68 = 0f; num68 < 2f; num68 += 1f)
                            {
                                Vector2 vector12 = Vector2.UnitY.RotatedByRandom(6.2831854820251465) * (Main.rand.NextFloat() * 0.5f + 0.5f);
                                Dust obj = Main.dust[Dust.NewDust(vector, 0, 0, 228)];
                                obj.position = vector + vector12 * scaleFactor;
                                obj.noGravity = true;
                                obj.velocity = vector12 * 2f;
                                obj.scale = 0.5f + Main.rand.NextFloat() * 0.5f;
                            }
                        }
                        Lighting.AddLight(npc.Center, 0.9f, 0.75f, 0.1f);
                        return;
                    }
                    if (npc.ai[2] < 0f && npc.ai[2] >= (float)(-num24))
                    {
                        Lighting.AddLight(npc.Center, 0.9f, 0.75f, 0.1f);
                        npc.velocity.X *= 0.9f;
                        if (npc.velocity.Y < -2f || npc.velocity.Y > 4f || npc.justHit)
                        {
                            npc.ai[2] = num2;
                        }
                        else
                        {
                            npc.ai[2] += 1f;
                            if (npc.ai[2] == 0f)
                            {
                                npc.ai[2] = num13;
                            }
                        }
                        float num78 = npc.ai[2] + (float)num24;
                        if (num78 < 180f && (Main.rand.Next(3) == 0 || npc.ai[2] % 3f == 0f))
                        {
                            Vector2 vector17 = npc.Top + new Vector2(npc.spriteDirection * 10, 10f);
                            float scaleFactor2 = MathHelper.Lerp(20f, 30f, (num78 * 3f + 50f) / 182f);
                            Main.rand.NextFloat();
                            for (float num89 = 0f; num89 < 1f; num89 += 1f)
                            {
                                Vector2 vector18 = Vector2.UnitY.RotatedByRandom(6.2831854820251465) * (Main.rand.NextFloat() * 0.5f + 0.5f);
                                Dust obj2 = Main.dust[Dust.NewDust(vector17, 0, 0, 228)];
                                obj2.position = vector17 + vector18 * scaleFactor2;
                                obj2.noGravity = true;
                                obj2.velocity = vector18 * 4f;
                                obj2.scale = 0.5f + Main.rand.NextFloat();
                            }
                        }
                        if (Main.netMode == 2)
                        {
                            return;
                        }
                        Player player = Main.player[Main.myPlayer];
                        _ = Main.myPlayer;
                        if (player.dead || !player.active || player.FindBuffIndex(156) != -1)
                        {
                            return;
                        }
                        Vector2 vector19 = player.Center - npc.Center;
                        if (!(vector19.Length() < 700f))
                        {
                            return;
                        }
                        bool flag = vector19.Length() < 30f;
                        if (!flag)
                        {
                            float x = ((float)Math.PI / 4f).ToRotationVector2().X;
                            Vector2 vector20 = Vector2.Normalize(vector19);
                            if (vector20.X > x || vector20.X < 0f - x)
                            {
                                flag = true;
                            }
                        }
                        if (((player.Center.X < npc.Center.X && npc.direction < 0 && player.direction > 0) || (player.Center.X > npc.Center.X && npc.direction > 0 && player.direction < 0)) && flag && (Collision.CanHitLine(npc.Center, 1, 1, player.Center, 1, 1) || Collision.CanHitLine(npc.Center - Vector2.UnitY * 16f, 1, 1, player.Center, 1, 1) || Collision.CanHitLine(npc.Center + Vector2.UnitY * 8f, 1, 1, player.Center, 1, 1)))
                        {
                            player.AddBuff(156, num35 + (int)npc.ai[2] * -1);
                        }
                        return;
                    }
                }
            }
            if (npc.type == 471)
            {
                if (npc.ai[3] < 0f)
                {
                    npc.knockBackResist = 0f;
                    npc.defense = (int)((double)npc.defDefense * 1.1);
                    npc.noGravity = true;
                    npc.noTileCollide = true;
                    if (npc.velocity.X < 0f)
                    {
                        npc.direction = -1;
                    }
                    else if (npc.velocity.X > 0f)
                    {
                        npc.direction = 1;
                    }
                    npc.rotation = npc.velocity.X * 0.1f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.localAI[3] += 1f;
                        if (npc.localAI[3] > (float)Main.rand.Next(20, 180))
                        {
                            npc.localAI[3] = 0f;
                            Vector2 center3 = npc.Center;
                            center3 += npc.velocity;
                            NPC.NewNPC((int)center3.X, (int)center3.Y, 30);
                        }
                    }
                }
                else
                {
                    npc.localAI[3] = 0f;
                    npc.knockBackResist = 0.35f * Main.knockBackMultiplier;
                    npc.rotation *= 0.9f;
                    npc.defense = npc.defDefense;
                    npc.noGravity = false;
                    npc.noTileCollide = false;
                }
                if (npc.ai[3] == 1f)
                {
                    npc.knockBackResist = 0f;
                    npc.defense += 10;
                }
                if (npc.ai[3] == -1f)
                {
                    npc.TargetClosest();
                    float num100 = 8f;
                    float num102 = 40f;
                    Vector2 value3 = Main.player[npc.target].Center - npc.Center;
                    float num103 = value3.Length();
                    num100 += num103 / 200f;
                    value3.Normalize();
                    value3 *= num100;
                    npc.velocity = (npc.velocity * (num102 - 1f) + value3) / num102;
                    if (num103 < 500f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                    {
                        npc.ai[3] = 0f;
                        npc.ai[2] = 0f;
                    }
                    return;
                }
                if (npc.ai[3] == -2f)
                {
                    npc.velocity.Y -= 0.2f;
                    if (npc.velocity.Y < -10f)
                    {
                        npc.velocity.Y = -10f;
                    }
                    if (Main.player[npc.target].Center.Y - npc.Center.Y > 200f)
                    {
                        npc.TargetClosest();
                        npc.ai[3] = -3f;
                        if (Main.player[npc.target].Center.X > npc.Center.X)
                        {
                            npc.ai[2] = 1f;
                        }
                        else
                        {
                            npc.ai[2] = -1f;
                        }
                    }
                    npc.velocity.X *= 0.99f;
                    return;
                }
                if (npc.ai[3] == -3f)
                {
                    if (npc.direction == 0)
                    {
                        npc.TargetClosest();
                    }
                    if (npc.ai[2] == 0f)
                    {
                        npc.ai[2] = npc.direction;
                    }
                    npc.velocity.Y *= 0.9f;
                    npc.velocity.X += npc.ai[2] * 0.3f;
                    if (npc.velocity.X > 10f)
                    {
                        npc.velocity.X = 10f;
                    }
                    if (npc.velocity.X < -10f)
                    {
                        npc.velocity.X = -10f;
                    }
                    float num104 = Main.player[npc.target].Center.X - npc.Center.X;
                    if ((npc.ai[2] < 0f && num104 > 300f) || (npc.ai[2] > 0f && num104 < -300f))
                    {
                        npc.ai[3] = -4f;
                        npc.ai[2] = 0f;
                    }
                    else if (Math.Abs(num104) > 800f)
                    {
                        npc.ai[3] = -1f;
                        npc.ai[2] = 0f;
                    }
                    return;
                }
                if (npc.ai[3] == -4f)
                {
                    npc.ai[2] += 1f;
                    npc.velocity.Y += 0.1f;
                    if (npc.velocity.Length() > 4f)
                    {
                        npc.velocity *= 0.9f;
                    }
                    int num105 = (int)npc.Center.X / 16;
                    int num106 = (int)(npc.position.Y + (float)npc.height + 12f) / 16;
                    bool flag12 = false;
                    for (int i = num105 - 1; i <= num105 + 1; i++)
                    {
                        if (Main.tile[i, num106] == null)
                        {
                            Main.tile[num105, num106] = new Tile();
                        }
                        if (Main.tile[i, num106].active() && Main.tileSolid[Main.tile[i, num106].type])
                        {
                            flag12 = true;
                        }
                    }
                    if (flag12 && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                    {
                        npc.ai[3] = 0f;
                        npc.ai[2] = 0f;
                    }
                    else if (npc.ai[2] > 300f || npc.Center.Y > Main.player[npc.target].Center.Y + 200f)
                    {
                        npc.ai[3] = -1f;
                        npc.ai[2] = 0f;
                    }
                }
                else
                {
                    if (npc.ai[3] == 1f)
                    {
                        Vector2 center4 = npc.Center;
                        center4.Y -= 70f;
                        npc.velocity.X *= 0.8f;
                        npc.ai[2] += 1f;
                        if (npc.ai[2] == 60f)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                NPC.NewNPC((int)center4.X, (int)center4.Y + 18, 472);
                            }
                        }
                        else if (npc.ai[2] >= 90f)
                        {
                            npc.ai[3] = -2f;
                            npc.ai[2] = 0f;
                        }
                        for (int j = 0; j < 2; j++)
                        {
                            Vector2 value6 = center4;
                            Vector2 value4 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                            value4.Normalize();
                            value4 *= (float)Main.rand.Next(0, 100) * 0.1f;
                            Vector2 position4 = value6 + value4;
                            value4.Normalize();
                            value4 *= (float)Main.rand.Next(50, 90) * 0.1f;
                            int num107 = Dust.NewDust(position4, 1, 1, 27);
                            Main.dust[num107].velocity = -value4 * 0.3f;
                            Main.dust[num107].alpha = 100;
                            if (Main.rand.Next(2) == 0)
                            {
                                Main.dust[num107].noGravity = true;
                                Main.dust[num107].scale += 0.3f;
                            }
                        }
                        return;
                    }
                    npc.ai[2] += 1f;
                    int num108 = 10;
                    if (npc.velocity.Y == 0f && NPC.CountNPCS(472) < num108)
                    {
                        if (npc.ai[2] >= 180f)
                        {
                            npc.ai[2] = 0f;
                            npc.ai[3] = 1f;
                        }
                    }
                    else
                    {
                        if (NPC.CountNPCS(472) >= num108)
                        {
                            npc.ai[2] += 1f;
                        }
                        if (npc.ai[2] >= 360f)
                        {
                            npc.ai[2] = 0f;
                            npc.ai[3] = -2f;
                            npc.velocity.Y -= 3f;
                        }
                    }
                    if (npc.target >= 0 && !Main.player[npc.target].dead && (Main.player[npc.target].Center - npc.Center).Length() > 800f)
                    {
                        npc.ai[3] = -1f;
                        npc.ai[2] = 0f;
                    }
                }
                if (Main.player[npc.target].dead)
                {
                    npc.TargetClosest();
                    if (Main.player[npc.target].dead && npc.timeLeft > 1)
                    {
                        npc.timeLeft = 1;
                    }
                }
            }
            if (npc.type == NPCID.SolarSolenian)
            {
                npc.reflectingProjectiles = false;
                npc.takenDamageMultiplier = 1f;
                int num109 = 6;
                int num110 = 10;
                float scaleFactor3 = 16f;
                if (npc.ai[2] > 0f)
                {
                    npc.ai[2] -= 1f;
                }
                if (npc.ai[2] == 0f)
                {
                    if (((Main.player[npc.target].Center.X < npc.Center.X && npc.direction < 0) || (Main.player[npc.target].Center.X > npc.Center.X && npc.direction > 0)) && Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                    {
                        npc.ai[2] = -1f;
                        npc.netUpdate = true;
                        npc.TargetClosest();
                    }
                }
                else
                {
                    if (npc.ai[2] < 0f && npc.ai[2] > (float)(-num109))
                    {
                        npc.ai[2] -= 1f;
                        npc.velocity.X *= 0.9f;
                        return;
                    }
                    if (npc.ai[2] == (float)(-num109))
                    {
                        npc.ai[2] -= 1f;
                        npc.TargetClosest();
                        Vector2 vec = npc.DirectionTo(Main.player[npc.target].Top + new Vector2(0f, -30f));
                        if (vec.HasNaNs())
                        {
                            vec = Vector2.Normalize(new Vector2(npc.spriteDirection, -1f));
                        }
                        npc.velocity = vec * scaleFactor3;
                        npc.netUpdate = true;
                        return;
                    }
                    if (npc.ai[2] < (float)(-num109))
                    {
                        npc.ai[2] -= 1f;
                        if (npc.velocity.Y == 0f)
                        {
                            npc.ai[2] = 60f;
                        }
                        else if (npc.ai[2] < (float)(-num109 - num110))
                        {
                            npc.velocity.Y += 0.15f;
                            if (npc.velocity.Y > 24f)
                            {
                                npc.velocity.Y = 24f;
                            }
                        }
                        npc.reflectingProjectiles = true;
                        npc.takenDamageMultiplier = 3f;
                        if (npc.justHit)
                        {
                            npc.ai[2] = 60f;
                            npc.netUpdate = true;
                        }
                        return;
                    }
                }
            }
            if (npc.type == 415)
            {
                int num111 = 42;
                int num113 = 18;
                if (npc.justHit)
                {
                    npc.ai[2] = 120f;
                    npc.netUpdate = true;
                }
                if (npc.ai[2] > 0f)
                {
                    npc.ai[2] -= 1f;
                }
                if (npc.ai[2] == 0f)
                {
                    int num114 = 0;
                    for (int k = 0; k < 200; k++)
                    {
                        if (Main.npc[k].active && Main.npc[k].type == 516)
                        {
                            num114++;
                        }
                    }
                    if (num114 > 6)
                    {
                        npc.ai[2] = 90f;
                    }
                    else if (((Main.player[npc.target].Center.X < npc.Center.X && npc.direction < 0) || (Main.player[npc.target].Center.X > npc.Center.X && npc.direction > 0)) && Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                    {
                        npc.ai[2] = -1f;
                        npc.netUpdate = true;
                        npc.TargetClosest();
                    }
                }
                else if (npc.ai[2] < 0f && npc.ai[2] > (float)(-num111))
                {
                    npc.ai[2] -= 1f;
                    if (npc.ai[2] == (float)(-num111))
                    {
                        npc.ai[2] = 180 + 30 * Main.rand.Next(10);
                    }
                    npc.velocity.X *= 0.8f;
                    if (npc.ai[2] == (float)(-num113) || npc.ai[2] == (float)(-num113 - 8) || npc.ai[2] == (float)(-num113 - 16))
                    {
                        for (int l = 0; l < 20; l++)
                        {
                            Vector2 vector21 = npc.Center + Vector2.UnitX * npc.spriteDirection * 40f;
                            Dust obj3 = Main.dust[Dust.NewDust(vector21, 0, 0, 259)];
                            Vector2 vector22 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
                            obj3.position = vector21 + vector22 * 4f;
                            obj3.velocity = vector22 * 2f + Vector2.UnitX * Main.rand.NextFloat() * npc.spriteDirection * 3f;
                            obj3.scale = 0.3f + vector22.X * (float)(-npc.spriteDirection);
                            obj3.fadeIn = 0.7f;
                            obj3.noGravity = true;
                        }
                        if (npc.velocity.X > -0.5f && npc.velocity.X < 0.5f)
                        {
                            npc.velocity.X = 0f;
                        }
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.NewNPC((int)npc.Center.X + npc.spriteDirection * 45, (int)npc.Center.Y + 8, 516, 0, 0f, 0f, 0f, 0f, npc.target);
                        }
                    }
                    return;
                }
            }
            if (npc.type == NPCID.VortexLarva)
            {
                npc.localAI[0] += 1f;
                if (npc.localAI[0] >= 300f)
                {
                    int num186 = (int)npc.Center.X / 16 - 1;
                    int num115 = (int)npc.Center.Y / 16 - 1;
                    if (!Collision.SolidTiles(num186, num186 + 2, num115, num115 + 1) && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.Transform(NPCID.VortexHornet);
                        npc.life = npc.lifeMax;
                        npc.localAI[0] = 0f;
                        return;
                    }
                }
                int num116 = 0;
                num116 = ((npc.localAI[0] < 60f) ? 16 : ((npc.localAI[0] < 120f) ? 8 : ((npc.localAI[0] < 180f) ? 4 : ((npc.localAI[0] < 240f) ? 2 : ((!(npc.localAI[0] < 300f)) ? 1 : 1)))));
                if (Main.rand.Next(num116) == 0)
                {
                    Dust dust = Main.dust[Dust.NewDust(npc.position, npc.width, npc.height, 229)];
                    dust.noGravity = true;
                    dust.scale = 1f;
                    dust.noLight = true;
                    dust.velocity = npc.DirectionFrom(dust.position) * dust.velocity.Length();
                    dust.position -= dust.velocity * 5f;
                    dust.position.X += npc.direction * 6;
                    dust.position.Y += 4f;
                }
            }
            if (npc.type == NPCID.VortexHornet)
            {
                npc.localAI[0] += 1f;
                npc.localAI[0] += Math.Abs(npc.velocity.X) / 2f;
                if (npc.localAI[0] >= 1200f && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int num187 = (int)npc.Center.X / 16 - 2;
                    int num117 = (int)npc.Center.Y / 16 - 3;
                    if (!Collision.SolidTiles(num187, num187 + 4, num117, num117 + 4))
                    {
                        npc.Transform(NPCID.VortexHornetQueen);
                        npc.life = npc.lifeMax;
                        npc.localAI[0] = 0f;
                        return;
                    }
                }
                int num118 = 0;
                num118 = ((npc.localAI[0] < 360f) ? 32 : ((npc.localAI[0] < 720f) ? 16 : ((npc.localAI[0] < 1080f) ? 6 : ((npc.localAI[0] < 1440f) ? 2 : ((!(npc.localAI[0] < 1800f)) ? 1 : 1)))));
                if (Main.rand.Next(num118) == 0)
                {
                    Dust obj4 = Main.dust[Dust.NewDust(npc.position, npc.width, npc.height, 229)];
                    obj4.noGravity = true;
                    obj4.scale = 1f;
                    obj4.noLight = true;
                }
            }
            bool flag21 = false;
            if (npc.velocity.X == 0f)
            {
                flag21 = true;
            }
            if (npc.justHit)
            {
                flag21 = false;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && npc.type == NPCID.Lihzahrd && (double)npc.life <= (double)npc.lifeMax * 0.55)
            {
                npc.Transform(NPCID.LihzahrdCrawler);
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && npc.type == NPCID.Nutcracker && (double)npc.life <= (double)npc.lifeMax * 0.55)
            {
                npc.Transform(NPCID.NutcrackerSpinning);
            }
            int num119 = 60;
            if (npc.type == NPCID.ChaosElemental)
            {
                num119 = 180;
                if (npc.ai[3] == -120f)
                {
                    npc.velocity *= 0f;
                    npc.ai[3] = 0f;
                    Main.PlaySound(SoundID.Item8, npc.position);
                    Vector2 vector23 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    float num120 = npc.oldPos[2].X + (float)npc.width * 0.5f - vector23.X;
                    float num122 = npc.oldPos[2].Y + (float)npc.height * 0.5f - vector23.Y;
                    float num123 = (float)Math.Sqrt(num120 * num120 + num122 * num122);
                    num123 = 2f / num123;
                    num120 *= num123;
                    num122 *= num123;
                    for (int m = 0; m < 20; m++)
                    {
                        int num124 = Dust.NewDust(npc.position, npc.width, npc.height, 71, num120, num122, 200, default(Color), 2f);
                        Main.dust[num124].noGravity = true;
                        Main.dust[num124].velocity.X *= 2f;
                    }
                    for (int n = 0; n < 20; n++)
                    {
                        int num125 = Dust.NewDust(npc.oldPos[2], npc.width, npc.height, 71, 0f - num120, 0f - num122, 200, default(Color), 2f);
                        Main.dust[num125].noGravity = true;
                        Main.dust[num125].velocity.X *= 2f;
                    }
                }
            }
            bool flag22 = false;
            bool flag23 = true;
            if (npc.type == 343 || npc.type == 47 || npc.type == 67 || npc.type == 109 || npc.type == 110 || npc.type == 111 || npc.type == 120 || npc.type == 163 || npc.type == 164 || npc.type == 239 || npc.type == 168 || npc.type == 199 || npc.type == 206 || npc.type == 214 || npc.type == 215 || npc.type == 216 || npc.type == 217 || npc.type == 218 || npc.type == 219 || npc.type == 220 || npc.type == 226 || npc.type == 243 || npc.type == 251 || npc.type == 257 || npc.type == 258 || npc.type == 290 || npc.type == 291 || npc.type == 292 || npc.type == 293 || npc.type == 305 || npc.type == 306 || npc.type == 307 || npc.type == 308 || npc.type == 309 || npc.type == 348 || npc.type == 349 || npc.type == 350 || npc.type == 351 || npc.type == 379 || (npc.type >= 430 && npc.type <= 436) || npc.type == 380 || npc.type == 381 || npc.type == 382 || npc.type == 383 || npc.type == 386 || npc.type == 391 || (npc.type >= 449 && npc.type <= 452) || npc.type == 466 || npc.type == 464 || npc.type == 166 || npc.type == 469 || npc.type == 468 || npc.type == 471 || npc.type == 470 || npc.type == 480 || npc.type == 481 || npc.type == 482 || npc.type == 411 || npc.type == 424 || npc.type == 409 || (npc.type >= 494 && npc.type <= 506) || npc.type == 425 || npc.type == 427 || npc.type == 426 || npc.type == 428 || npc.type == 508 || npc.type == 415 || npc.type == 419 || npc.type == 520 || (npc.type >= 524 && npc.type <= 527) || npc.type == 528 || npc.type == 529 || npc.type == 530 || npc.type == 532)
            {
                flag23 = false;
            }
            bool flag24 = false;
            int num126 = npc.type;
            if (num126 == 425 || num126 == 471)
            {
                flag24 = true;
            }
            bool flag25 = true;
            switch (npc.type)
            {
                case 110:
                case 111:
                case 206:
                case 214:
                case 215:
                case 216:
                case 291:
                case 292:
                case 293:
                case 350:
                case 379:
                case 380:
                case 381:
                case 382:
                case 409:
                case 411:
                case 424:
                case 426:
                case 466:
                case 498:
                case 499:
                case 500:
                case 501:
                case 502:
                case 503:
                case 504:
                case 505:
                case 506:
                case 520:
                if (npc.ai[2] > 0f)
                {
                    flag25 = false;
                }
                break;
            }
            if (!flag24 && flag25)
            {
                if (npc.velocity.Y == 0f && ((npc.velocity.X > 0f && npc.direction < 0) || (npc.velocity.X < 0f && npc.direction > 0)))
                {
                    flag22 = true;
                }
                if (npc.position.X == npc.oldPosition.X || npc.ai[3] >= (float)num119 || flag22)
                {
                    npc.ai[3] += 1f;
                }
                else if ((double)Math.Abs(npc.velocity.X) > 0.9 && npc.ai[3] > 0f)
                {
                    npc.ai[3] -= 1f;
                }
                if (npc.ai[3] > (float)(num119 * 10))
                {
                    npc.ai[3] = 0f;
                }
                if (npc.justHit)
                {
                    npc.ai[3] = 0f;
                }
                if (npc.ai[3] == (float)num119)
                {
                    npc.netUpdate = true;
                }
            }
            if (npc.type == 463 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (npc.localAI[3] > 0f)
                {
                    npc.localAI[3] -= 1f;
                }
                if (npc.justHit && npc.localAI[3] <= 0f && Main.rand.Next(3) == 0)
                {
                    npc.localAI[3] = 30f;
                    int num127 = Main.rand.Next(3, 6);
                    int[] array = new int[num127];
                    int num128 = 0;
                    for (int num129 = 0; num129 < 255; num129++)
                    {
                        if (Main.player[num129].active && !Main.player[num129].dead && Collision.CanHitLine(npc.position, npc.width, npc.height, Main.player[num129].position, Main.player[num129].width, Main.player[num129].height))
                        {
                            array[num128] = num129;
                            num128++;
                            if (num128 == num127)
                            {
                                break;
                            }
                        }
                    }
                    if (num128 > 1)
                    {
                        for (int num130 = 0; num130 < 100; num130++)
                        {
                            int num131 = Main.rand.Next(num128);
                            int num133;
                            for (num133 = num131; num133 == num131; num133 = Main.rand.Next(num128))
                            {
                            }
                            int num134 = array[num131];
                            array[num131] = array[num133];
                            array[num133] = num134;
                        }
                    }
                    Vector2 vector2 = new Vector2(-1f, -1f);
                    for (int num135 = 0; num135 < num128; num135++)
                    {
                        Vector2 vector3 = Main.npc[array[num135]].Center - npc.Center;
                        vector3.Normalize();
                        vector2 += vector3;
                    }
                    vector2.Normalize();
                    for (int num136 = 0; num136 < num127; num136++)
                    {
                        float num137 = Main.rand.Next(8, 13);
                        Vector2 vector4 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                        vector4.Normalize();
                        if (num128 > 0)
                        {
                            vector4 += vector2;
                            vector4.Normalize();
                        }
                        vector4 *= num137;
                        if (num128 > 0)
                        {
                            num128--;
                            vector4 = Main.player[array[num128]].Center - npc.Center;
                            vector4.Normalize();
                            vector4 *= num137;
                        }
                        Projectile.NewProjectile(npc.Center.X, npc.position.Y + (float)(npc.width / 4), vector4.X, vector4.Y, 498, (int)((double)npc.damage * 0.15), 1f);
                    }
                }
            }
            if (npc.type == 460)
            {
                if (npc.velocity.Y < 0f - 0.3f || npc.velocity.Y > 0.3f)
                {
                    npc.knockBackResist = 0f;
                }
                else
                {
                    npc.knockBackResist = 0.25f * Main.knockBackMultiplier;
                }
            }
            if (npc.type == 469)
            {
                npc.knockBackResist = 0.45f * Main.knockBackMultiplier;
                if (npc.ai[2] == 1f)
                {
                    npc.knockBackResist = 0f;
                }
                bool flag26 = false;
                int num138 = (int)npc.Center.X / 16;
                int num139 = (int)npc.Center.Y / 16;
                for (int num140 = num138 - 1; num140 <= num138 + 1; num140++)
                {
                    for (int num141 = num139 - 1; num141 <= num139 + 1; num141++)
                    {
                        if (Main.tile[num140, num141] != null && Main.tile[num140, num141].wall > 0)
                        {
                            flag26 = true;
                            break;
                        }
                    }
                    if (flag26)
                    {
                        break;
                    }
                }
                if (npc.ai[2] == 0f && flag26)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity.Y = -4.6f;
                        npc.velocity.X *= 1.3f;
                    }
                    else if (npc.velocity.Y > 0f)
                    {
                        npc.ai[2] = 1f;
                    }
                }
                if (flag26 && npc.ai[2] == 1f && Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                {
                    Vector2 value5 = Main.player[npc.target].Center - npc.Center;
                    float num142 = value5.Length();
                    value5.Normalize();
                    value5 *= 4.5f + num142 / 300f;
                    npc.velocity = (npc.velocity * 29f + value5) / 30f;
                    npc.noGravity = true;
                    npc.ai[2] = 1f;
                    return;
                }
                npc.noGravity = false;
                npc.ai[2] = 0f;
            }
            if (npc.type == 462 && npc.velocity.Y == 0f && (Main.player[npc.target].Center - npc.Center).Length() < 150f && Math.Abs(npc.velocity.X) > 3f && ((npc.velocity.X < 0f && npc.Center.X > Main.player[npc.target].Center.X) || (npc.velocity.X > 0f && npc.Center.X < Main.player[npc.target].Center.X)))
            {
                npc.velocity.X *= 1.75f;
                npc.velocity.Y -= 4.5f;
                if (npc.Center.Y - Main.player[npc.target].Center.Y > 20f)
                {
                    npc.velocity.Y -= 0.5f;
                }
                if (npc.Center.Y - Main.player[npc.target].Center.Y > 40f)
                {
                    npc.velocity.Y -= 1f;
                }
                if (npc.Center.Y - Main.player[npc.target].Center.Y > 80f)
                {
                    npc.velocity.Y -= 1.5f;
                }
                if (npc.Center.Y - Main.player[npc.target].Center.Y > 100f)
                {
                    npc.velocity.Y -= 1.5f;
                }
                if (Math.Abs(npc.velocity.X) > 7f)
                {
                    if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X = -7f;
                    }
                    else
                    {
                        npc.velocity.X = 7f;
                    }
                }
            }
            if (npc.ai[3] < (float)num119 &&
                (Main.eclipse || !Main.dayTime || (double)npc.position.Y > Main.worldSurface * 16.0
                || Main.invasionType == 1 && (npc.type == NPCID.Yeti || npc.type == NPCID.ElfArcher) ||

                (Main.invasionType == 1 && (npc.type == NPCID.GoblinPeon || npc.type == NPCID.GoblinThief || npc.type == NPCID.GoblinWarrior || npc.type == NPCID.GoblinArcher || npc.type == NPCID.GoblinSummoner))
                || npc.type == NPCID.GoblinScout || (Main.invasionType == 3 && npc.type >= NPCID.PirateDeckhand && npc.type <= NPCID.PirateCaptain) || (Main.invasionType == 4 && (npc.type == 381 || npc.type == 382 || npc.type == 383 || npc.type == 385 || npc.type == 386 || npc.type == 389 || npc.type == 391 || npc.type == 520)) || npc.type == 31 || npc.type == 294 || npc.type == 295 || npc.type == 296 || npc.type == 47 || npc.type == 67 || npc.type == 77 || npc.type == 78 || npc.type == 79 || npc.type == 80 || npc.type == 110 || npc.type == 120 || npc.type == 168 || npc.type == 181 || npc.type == 185 || npc.type == 198 || npc.type == 199 || npc.type == 206 || npc.type == 217 || npc.type == 218 || npc.type == 219 || npc.type == 220 || npc.type == 239 || npc.type == 243 || npc.type == 254 || npc.type == 255 || npc.type == 257 || npc.type == 258 || npc.type == 291 || npc.type == 292 || npc.type == 293 || npc.type == 379 || npc.type == 380 || npc.type == 464 || npc.type == 470 || npc.type == 424 || (npc.type == 411 && (npc.ai[1] >= 180f || npc.ai[1] < 90f)) || npc.type == 409 || npc.type == 425 || npc.type == 429 || npc.type == 427 || npc.type == 428 || npc.type == 508 || npc.type == 415 || npc.type == 419 || (npc.type >= 524 && npc.type <= 527) || npc.type == 528 || npc.type == 529 || npc.type == 530 || npc.type == 532))
            {
                if ((npc.type == NPCID.Zombie || npc.type == NPCID.ZombieXmas || npc.type == NPCID.ZombieSweater || npc.type == 21 || (npc.type >= 449 && npc.type <= 452) || npc.type == 31 || npc.type == 294 || npc.type == 295 || npc.type == 296 || npc.type == 77 || npc.type == 110 || npc.type == 132 || npc.type == 167 || npc.type == 161 || npc.type == 162 || npc.type == 186 || npc.type == 187 || npc.type == 188 || npc.type == 189 || npc.type == 197 || npc.type == 200 || npc.type == 201 || npc.type == 202 || npc.type == 203 || npc.type == 223 || npc.type == NPCID.SkeletonSniper || npc.type == NPCID.TacticalSkeleton || npc.type == NPCID.SkeletonCommando || npc.type == NPCID.ZombieSuperman || npc.type == NPCID.ZombiePixie || npc.type == NPCID.ZombieDoctor || npc.type == NPCID.GreekSkeleton) && Main.rand.Next(1000) == 0)
                {
                    Main.PlaySound(SoundID.ZombieMoan, (int)npc.position.X, (int)npc.position.Y);
                }
                if (npc.type == NPCID.BloodZombie && Main.rand.Next(800) == 0)
                {
                    Main.PlaySound(SoundID.ZombieMoan, (int)npc.position.X, (int)npc.position.Y, npc.type);
                }
                if ((npc.type == NPCID.Mummy || npc.type == NPCID.DarkMummy || npc.type == NPCID.LightMummy) && Main.rand.Next(500) == 0)
                {
                    Main.PlaySound(SoundID.Mummy, (int)npc.position.X, (int)npc.position.Y);
                }
                if (npc.type == NPCID.Vampire && Main.rand.Next(500) == 0)
                {
                    Main.PlaySound(SoundID.Zombie, (int)npc.position.X, (int)npc.position.Y, 7);
                }
                if (npc.type == 162 && Main.rand.Next(500) == 0)
                {
                    Main.PlaySound(29, (int)npc.position.X, (int)npc.position.Y, 6);
                }
                if (npc.type == NPCID.FaceMonster && Main.rand.Next(500) == 0)
                {
                    Main.PlaySound(SoundID.Zombie, (int)npc.position.X, (int)npc.position.Y, 8);
                }
                if (npc.type >= NPCID.RustyArmoredBonesAxe && npc.type <= NPCID.HellArmoredBonesSword && Main.rand.Next(1000) == 0)
                {
                    Main.PlaySound(14, (int)npc.position.X, (int)npc.position.Y);
                }
                npc.TargetClosest();
            }
            else if (!(npc.ai[2] > 0f) || (npc.type != 110 && npc.type != 111 && npc.type != 206 && npc.type != 216 && npc.type != 214 && npc.type != 215 && npc.type != 291 && npc.type != 292 && npc.type != 293 && npc.type != 350 && npc.type != 381 && npc.type != 382 && npc.type != 383 && npc.type != 385 && npc.type != 386 && npc.type != 389 && npc.type != 391 && npc.type != 469 && npc.type != 166 && npc.type != 466 && npc.type != 471 && npc.type != 411 && npc.type != 409 && npc.type != 424 && npc.type != 425 && npc.type != 426 && npc.type != 415 && npc.type != 419 && npc.type != 520))
            {
                if (Main.dayTime && (double)(npc.position.Y / 16f) < Main.worldSurface && npc.timeLeft > 10)
                {
                    npc.timeLeft = 10;
                }
                if (npc.velocity.X == 0f)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.ai[0] += 1f;
                        if (npc.ai[0] >= 2f)
                        {
                            npc.direction *= -1;
                            npc.spriteDirection = npc.direction;
                            npc.ai[0] = 0f;
                        }
                    }
                }
                else
                {
                    npc.ai[0] = 0f;
                }
                if (npc.direction == 0)
                {
                    npc.direction = 1;
                }
            }
            if (npc.type == 159 || npc.type == 349)
            {
                if (npc.type == 159 && ((npc.velocity.X > 0f && npc.direction < 0) || (npc.velocity.X < 0f && npc.direction > 0)))
                {
                    npc.velocity.X *= 0.95f;
                }
                if (npc.velocity.X < -6f || npc.velocity.X > 6f)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity *= 0.8f;
                    }
                }
                else if (npc.velocity.X < 6f && npc.direction == 1)
                {
                    if (npc.velocity.Y == 0f && npc.velocity.X < 0f)
                    {
                        npc.velocity.X *= 0.99f;
                    }
                    npc.velocity.X += 0.07f;
                    if (npc.velocity.X > 6f)
                    {
                        npc.velocity.X = 6f;
                    }
                }
                else if (npc.velocity.X > -6f && npc.direction == -1)
                {
                    if (npc.velocity.Y == 0f && npc.velocity.X > 0f)
                    {
                        npc.velocity.X *= 0.99f;
                    }
                    npc.velocity.X -= 0.07f;
                    if (npc.velocity.X < -6f)
                    {
                        npc.velocity.X = -6f;
                    }
                }
            }
            else if (npc.type == 199)
            {
                if (npc.velocity.X < -4f || npc.velocity.X > 4f)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity *= 0.8f;
                    }
                }
                else if (npc.velocity.X < 4f && npc.direction == 1)
                {
                    if (npc.velocity.Y == 0f && npc.velocity.X < 0f)
                    {
                        npc.velocity.X *= 0.8f;
                    }
                    npc.velocity.X += 0.1f;
                    if (npc.velocity.X > 4f)
                    {
                        npc.velocity.X = 4f;
                    }
                }
                else if (npc.velocity.X > -4f && npc.direction == -1)
                {
                    if (npc.velocity.Y == 0f && npc.velocity.X > 0f)
                    {
                        npc.velocity.X *= 0.8f;
                    }
                    npc.velocity.X -= 0.1f;
                    if (npc.velocity.X < -4f)
                    {
                        npc.velocity.X = -4f;
                    }
                }
            }
            else if (npc.type == 120 || npc.type == 166 || npc.type == 213 || npc.type == 258 || npc.type == 528 || npc.type == 529)
            {
                if (npc.velocity.X < -3f || npc.velocity.X > 3f)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity *= 0.8f;
                    }
                }
                else if (npc.velocity.X < 3f && npc.direction == 1)
                {
                    if (npc.velocity.Y == 0f && npc.velocity.X < 0f)
                    {
                        npc.velocity.X *= 0.99f;
                    }
                    npc.velocity.X += 0.07f;
                    if (npc.velocity.X > 3f)
                    {
                        npc.velocity.X = 3f;
                    }
                }
                else if (npc.velocity.X > -3f && npc.direction == -1)
                {
                    if (npc.velocity.Y == 0f && npc.velocity.X > 0f)
                    {
                        npc.velocity.X *= 0.99f;
                    }
                    npc.velocity.X -= 0.07f;
                    if (npc.velocity.X < -3f)
                    {
                        npc.velocity.X = -3f;
                    }
                }
            }
            else if (npc.type == 461 || npc.type == 27 || npc.type == 77 || npc.type == 104 || npc.type == 163 || npc.type == 162 || npc.type == 196 || npc.type == 197 || npc.type == 212 || npc.type == 257 || npc.type == 326 || npc.type == 343 || npc.type == 348 || npc.type == 351 || (npc.type >= 524 && npc.type <= 527) || npc.type == 530)
            {
                if (npc.velocity.X < -2f || npc.velocity.X > 2f)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity *= 0.8f;
                    }
                }
                else if (npc.velocity.X < 2f && npc.direction == 1)
                {
                    npc.velocity.X += 0.07f;
                    if (npc.velocity.X > 2f)
                    {
                        npc.velocity.X = 2f;
                    }
                }
                else if (npc.velocity.X > -2f && npc.direction == -1)
                {
                    npc.velocity.X -= 0.07f;
                    if (npc.velocity.X < -2f)
                    {
                        npc.velocity.X = -2f;
                    }
                }
            }
            else if (npc.type == 109)
            {
                if (npc.velocity.X < -2f || npc.velocity.X > 2f)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity *= 0.8f;
                    }
                }
                else if (npc.velocity.X < 2f && npc.direction == 1)
                {
                    npc.velocity.X += 0.04f;
                    if (npc.velocity.X > 2f)
                    {
                        npc.velocity.X = 2f;
                    }
                }
                else if (npc.velocity.X > -2f && npc.direction == -1)
                {
                    npc.velocity.X -= 0.04f;
                    if (npc.velocity.X < -2f)
                    {
                        npc.velocity.X = -2f;
                    }
                }
            }
            else if (npc.type == 21 || npc.type == 26 || npc.type == 31 || npc.type == 294 || npc.type == 295 || npc.type == 296 || npc.type == 47 || npc.type == 73 || npc.type == 140 || npc.type == 164 || npc.type == 239 || npc.type == 167 || npc.type == 168 || npc.type == 185 || npc.type == 198 || npc.type == 201 || npc.type == 202 || npc.type == 203 || npc.type == 217 || npc.type == 218 || npc.type == 219 || npc.type == 226 || npc.type == 181 || npc.type == 254 || npc.type == 338 || npc.type == 339 || npc.type == 340 || npc.type == 342 || npc.type == 385 || npc.type == 389 || npc.type == 462 || npc.type == 463 || npc.type == 466 || npc.type == 464 || npc.type == 469 || npc.type == 470 || npc.type == 480 || npc.type == 482 || npc.type == 425 || npc.type == 429)
            {
                float num144 = 1.5f;
                if (npc.type == 294)
                {
                    num144 = 2f;
                }
                else if (npc.type == 295)
                {
                    num144 = 1.75f;
                }
                else if (npc.type == 296)
                {
                    num144 = 1.25f;
                }
                else if (npc.type == 201)
                {
                    num144 = 1.1f;
                }
                else if (npc.type == 202)
                {
                    num144 = 0.9f;
                }
                else if (npc.type == 203)
                {
                    num144 = 1.2f;
                }
                else if (npc.type == 338)
                {
                    num144 = 1.75f;
                }
                else if (npc.type == 339)
                {
                    num144 = 1.25f;
                }
                else if (npc.type == 340)
                {
                    num144 = 2f;
                }
                else if (npc.type == 385)
                {
                    num144 = 1.8f;
                }
                else if (npc.type == 389)
                {
                    num144 = 2.25f;
                }
                else if (npc.type == 462)
                {
                    num144 = 4f;
                }
                else if (npc.type == 463)
                {
                    num144 = 0.75f;
                }
                else if (npc.type == 466)
                {
                    num144 = 3.75f;
                }
                else if (npc.type == 469)
                {
                    num144 = 3.25f;
                }
                else if (npc.type == 480)
                {
                    num144 = 1.5f + (1f - (float)npc.life / (float)npc.lifeMax) * 2f;
                }
                else if (npc.type == 425)
                {
                    num144 = 6f;
                }
                else if (npc.type == 429)
                {
                    num144 = 4f;
                }
                if (npc.type == 21 || npc.type == 201 || npc.type == 202 || npc.type == 203 || npc.type == 342)
                {
                    num144 *= 1f + (1f - npc.scale);
                }
                if (npc.velocity.X < 0f - num144 || npc.velocity.X > num144)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity *= 0.8f;
                    }
                }
                else if (npc.velocity.X < num144 && npc.direction == 1)
                {
                    if (npc.type == 466 && npc.velocity.X < -2f)
                    {
                        npc.velocity.X *= 0.9f;
                    }
                    npc.velocity.X += 0.07f;
                    if (npc.velocity.X > num144)
                    {
                        npc.velocity.X = num144;
                    }
                }
                else if (npc.velocity.X > 0f - num144 && npc.direction == -1)
                {
                    if (npc.type == 466 && npc.velocity.X > 2f)
                    {
                        npc.velocity.X *= 0.9f;
                    }
                    npc.velocity.X -= 0.07f;
                    if (npc.velocity.X < 0f - num144)
                    {
                        npc.velocity.X = 0f - num144;
                    }
                }
                if (npc.velocity.Y == 0f && npc.type == 462 && ((npc.direction > 0 && npc.velocity.X < 0f) || (npc.direction < 0 && npc.velocity.X > 0f)))
                {
                    npc.velocity.X *= 0.9f;
                }
            }
            else if (npc.type >= 269 && npc.type <= 280)
            {
                float num145 = 1.5f;
                if (npc.type == 269)
                {
                    num145 = 2f;
                }
                if (npc.type == 270)
                {
                    num145 = 1f;
                }
                if (npc.type == 271)
                {
                    num145 = 1.5f;
                }
                if (npc.type == 272)
                {
                    num145 = 3f;
                }
                if (npc.type == 273)
                {
                    num145 = 1.25f;
                }
                if (npc.type == 274)
                {
                    num145 = 3f;
                }
                if (npc.type == 275)
                {
                    num145 = 3.25f;
                }
                if (npc.type == 276)
                {
                    num145 = 2f;
                }
                if (npc.type == 277)
                {
                    num145 = 2.75f;
                }
                if (npc.type == 278)
                {
                    num145 = 1.8f;
                }
                if (npc.type == 279)
                {
                    num145 = 1.3f;
                }
                if (npc.type == 280)
                {
                    num145 = 2.5f;
                }
                num145 *= 1f + (1f - npc.scale);
                if (npc.velocity.X < 0f - num145 || npc.velocity.X > num145)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity *= 0.8f;
                    }
                }
                else if (npc.velocity.X < num145 && npc.direction == 1)
                {
                    npc.velocity.X += 0.07f;
                    if (npc.velocity.X > num145)
                    {
                        npc.velocity.X = num145;
                    }
                }
                else if (npc.velocity.X > 0f - num145 && npc.direction == -1)
                {
                    npc.velocity.X -= 0.07f;
                    if (npc.velocity.X < 0f - num145)
                    {
                        npc.velocity.X = 0f - num145;
                    }
                }
            }
            else if (npc.type >= 305 && npc.type <= 314)
            {
                float num146 = 1.5f;
                if (npc.type == 305 || npc.type == 310)
                {
                    num146 = 2f;
                }
                if (npc.type == 306 || npc.type == 311)
                {
                    num146 = 1.25f;
                }
                if (npc.type == 307 || npc.type == 312)
                {
                    num146 = 2.25f;
                }
                if (npc.type == 308 || npc.type == 313)
                {
                    num146 = 1.5f;
                }
                if (npc.type == 309 || npc.type == 314)
                {
                    num146 = 1f;
                }
                if (npc.type < 310)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity.X *= 0.85f;
                        if ((double)npc.velocity.X > -0.3 && (double)npc.velocity.X < 0.3)
                        {
                            npc.velocity.Y = -7f;
                            npc.velocity.X = num146 * (float)npc.direction;
                        }
                    }
                    else if (npc.spriteDirection == npc.direction)
                    {
                        npc.velocity.X = (npc.velocity.X * 10f + num146 * (float)npc.direction) / 11f;
                    }
                }
                else if (npc.velocity.X < 0f - num146 || npc.velocity.X > num146)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity *= 0.8f;
                    }
                }
                else if (npc.velocity.X < num146 && npc.direction == 1)
                {
                    npc.velocity.X += 0.07f;
                    if (npc.velocity.X > num146)
                    {
                        npc.velocity.X = num146;
                    }
                }
                else if (npc.velocity.X > 0f - num146 && npc.direction == -1)
                {
                    npc.velocity.X -= 0.07f;
                    if (npc.velocity.X < 0f - num146)
                    {
                        npc.velocity.X = 0f - num146;
                    }
                }
            }
            else if (npc.type == 67 || npc.type == 220 || npc.type == 428)
            {
                if (npc.velocity.X < -0.5f || npc.velocity.X > 0.5f)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity *= 0.7f;
                    }
                }
                else if (npc.velocity.X < 0.5f && npc.direction == 1)
                {
                    npc.velocity.X += 0.03f;
                    if (npc.velocity.X > 0.5f)
                    {
                        npc.velocity.X = 0.5f;
                    }
                }
                else if (npc.velocity.X > -0.5f && npc.direction == -1)
                {
                    npc.velocity.X -= 0.03f;
                    if (npc.velocity.X < -0.5f)
                    {
                        npc.velocity.X = -0.5f;
                    }
                }
            }
            else if (npc.type == 78 || npc.type == 79 || npc.type == 80)
            {
                float num147 = 1f;
                float num148 = 0.05f;
                if (npc.life < npc.lifeMax / 2)
                {
                    num147 = 2f;
                    num148 = 0.1f;
                }
                if (npc.type == 79)
                {
                    num147 *= 1.5f;
                }
                if (npc.velocity.X < 0f - num147 || npc.velocity.X > num147)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity *= 0.7f;
                    }
                }
                else if (npc.velocity.X < num147 && npc.direction == 1)
                {
                    npc.velocity.X += num148;
                    if (npc.velocity.X > num147)
                    {
                        npc.velocity.X = num147;
                    }
                }
                else if (npc.velocity.X > 0f - num147 && npc.direction == -1)
                {
                    npc.velocity.X -= num148;
                    if (npc.velocity.X < 0f - num147)
                    {
                        npc.velocity.X = 0f - num147;
                    }
                }
            }
            else if (npc.type == 287)
            {
                float num149 = 5f;
                float num150 = 0.2f;
                if (npc.velocity.X < 0f - num149 || npc.velocity.X > num149)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity *= 0.7f;
                    }
                }
                else if (npc.velocity.X < num149 && npc.direction == 1)
                {
                    npc.velocity.X += num150;
                    if (npc.velocity.X > num149)
                    {
                        npc.velocity.X = num149;
                    }
                }
                else if (npc.velocity.X > 0f - num149 && npc.direction == -1)
                {
                    npc.velocity.X -= num150;
                    if (npc.velocity.X < 0f - num149)
                    {
                        npc.velocity.X = 0f - num149;
                    }
                }
            }
            else if (npc.type == 243)
            {
                float num151 = 1f;
                float num152 = 0.07f;
                num151 += (1f - (float)npc.life / (float)npc.lifeMax) * 1.5f;
                num152 += (1f - (float)npc.life / (float)npc.lifeMax) * 0.15f;
                if (npc.velocity.X < 0f - num151 || npc.velocity.X > num151)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity *= 0.7f;
                    }
                }
                else if (npc.velocity.X < num151 && npc.direction == 1)
                {
                    npc.velocity.X += num152;
                    if (npc.velocity.X > num151)
                    {
                        npc.velocity.X = num151;
                    }
                }
                else if (npc.velocity.X > 0f - num151 && npc.direction == -1)
                {
                    npc.velocity.X -= num152;
                    if (npc.velocity.X < 0f - num151)
                    {
                        npc.velocity.X = 0f - num151;
                    }
                }
            }
            else if (npc.type == 251)
            {
                float num153 = 1f;
                float num155 = 0.08f;
                num153 += (1f - (float)npc.life / (float)npc.lifeMax) * 2f;
                num155 += (1f - (float)npc.life / (float)npc.lifeMax) * 0.2f;
                if (npc.velocity.X < 0f - num153 || npc.velocity.X > num153)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity *= 0.7f;
                    }
                }
                else if (npc.velocity.X < num153 && npc.direction == 1)
                {
                    npc.velocity.X += num155;
                    if (npc.velocity.X > num153)
                    {
                        npc.velocity.X = num153;
                    }
                }
                else if (npc.velocity.X > 0f - num153 && npc.direction == -1)
                {
                    npc.velocity.X -= num155;
                    if (npc.velocity.X < 0f - num153)
                    {
                        npc.velocity.X = 0f - num153;
                    }
                }
            }
            else if (npc.type == 386)
            {
                if (npc.ai[2] > 0f)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity.X *= 0.8f;
                    }
                }
                else
                {
                    float num156 = 0.15f;
                    float num157 = 1.5f;
                    if (npc.velocity.X < 0f - num157 || npc.velocity.X > num157)
                    {
                        if (npc.velocity.Y == 0f)
                        {
                            npc.velocity *= 0.7f;
                        }
                    }
                    else if (npc.velocity.X < num157 && npc.direction == 1)
                    {
                        npc.velocity.X += num156;
                        if (npc.velocity.X > num157)
                        {
                            npc.velocity.X = num157;
                        }
                    }
                    else if (npc.velocity.X > 0f - num157 && npc.direction == -1)
                    {
                        npc.velocity.X -= num156;
                        if (npc.velocity.X < 0f - num157)
                        {
                            npc.velocity.X = 0f - num157;
                        }
                    }
                }
            }
            else if (npc.type == 460)
            {
                float num158 = 3f;
                float num159 = 0.1f;
                if (Math.Abs(npc.velocity.X) > 2f)
                {
                    num159 *= 0.8f;
                }
                if ((double)Math.Abs(npc.velocity.X) > 2.5)
                {
                    num159 *= 0.8f;
                }
                if (Math.Abs(npc.velocity.X) > 3f)
                {
                    num159 *= 0.8f;
                }
                if ((double)Math.Abs(npc.velocity.X) > 3.5)
                {
                    num159 *= 0.8f;
                }
                if (Math.Abs(npc.velocity.X) > 4f)
                {
                    num159 *= 0.8f;
                }
                if ((double)Math.Abs(npc.velocity.X) > 4.5)
                {
                    num159 *= 0.8f;
                }
                if (Math.Abs(npc.velocity.X) > 5f)
                {
                    num159 *= 0.8f;
                }
                if ((double)Math.Abs(npc.velocity.X) > 5.5)
                {
                    num159 *= 0.8f;
                }
                num158 += (1f - (float)npc.life / (float)npc.lifeMax) * 3f;
                if (npc.velocity.X < 0f - num158 || npc.velocity.X > num158)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity *= 0.7f;
                    }
                }
                else if (npc.velocity.X < num158 && npc.direction == 1)
                {
                    if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X *= 0.93f;
                    }
                    npc.velocity.X += num159;
                    if (npc.velocity.X > num158)
                    {
                        npc.velocity.X = num158;
                    }
                }
                else if (npc.velocity.X > 0f - num158 && npc.direction == -1)
                {
                    if (npc.velocity.X > 0f)
                    {
                        npc.velocity.X *= 0.93f;
                    }
                    npc.velocity.X -= num159;
                    if (npc.velocity.X < 0f - num158)
                    {
                        npc.velocity.X = 0f - num158;
                    }
                }
            }
            else if (npc.type == 508)
            {
                float num160 = 2.5f;
                float num161 = 40f;
                float num162 = Math.Abs(npc.velocity.X);
                if (num162 > 2.75f)
                {
                    num160 = 3.5f;
                    num161 += 80f;
                }
                else if ((double)num162 > 2.25)
                {
                    num160 = 3f;
                    num161 += 60f;
                }
                if ((double)Math.Abs(npc.velocity.Y) < 0.5)
                {
                    if (npc.velocity.X > 0f && npc.direction < 0)
                    {
                        npc.velocity *= 0.9f;
                    }
                    if (npc.velocity.X < 0f && npc.direction > 0)
                    {
                        npc.velocity *= 0.9f;
                    }
                }
                if (Math.Abs(npc.velocity.Y) > 0.3f)
                {
                    num161 *= 3f;
                }
                if (npc.velocity.X <= 0f && npc.direction < 0)
                {
                    npc.velocity.X = (npc.velocity.X * num161 - num160) / (num161 + 1f);
                }
                else if (npc.velocity.X >= 0f && npc.direction > 0)
                {
                    npc.velocity.X = (npc.velocity.X * num161 + num160) / (num161 + 1f);
                }
                else if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) > 20f && Math.Abs(npc.velocity.Y) <= 0.3f)
                {
                    npc.velocity.X *= 0.99f;
                    npc.velocity.X += (float)npc.direction * 0.025f;
                }
            }
            else if (npc.type == 391 || npc.type == 427 || npc.type == 415 || npc.type == 419 || npc.type == 518 || npc.type == 532)
            {
                float num163 = 5f;
                float num164 = 0.25f;
                float num166 = 0.7f;
                if (npc.type == 427)
                {
                    num163 = 6f;
                    num164 = 0.2f;
                    num166 = 0.8f;
                }
                else if (npc.type == 415)
                {
                    num163 = 4f;
                    num164 = 0.1f;
                    num166 = 0.95f;
                }
                else if (npc.type == 419)
                {
                    num163 = 6f;
                    num164 = 0.15f;
                    num166 = 0.85f;
                }
                else if (npc.type == 518)
                {
                    num163 = 5f;
                    num164 = 0.1f;
                    num166 = 0.95f;
                }
                else if (npc.type == 532)
                {
                    num163 = 5f;
                    num164 = 0.15f;
                    num166 = 0.98f;
                }
                if (npc.velocity.X < 0f - num163 || npc.velocity.X > num163)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity *= num166;
                    }
                }
                else if (npc.velocity.X < num163 && npc.direction == 1)
                {
                    npc.velocity.X += num164;
                    if (npc.velocity.X > num163)
                    {
                        npc.velocity.X = num163;
                    }
                }
                else if (npc.velocity.X > 0f - num163 && npc.direction == -1)
                {
                    npc.velocity.X -= num164;
                    if (npc.velocity.X < 0f - num163)
                    {
                        npc.velocity.X = 0f - num163;
                    }
                }
            }
            else if ((npc.type >= 430 && npc.type <= 436) || npc.type == 494 || npc.type == 495)
            {
                if (npc.ai[2] == 0f)
                {
                    npc.damage = npc.defDamage;
                    float num167 = 1f;
                    num167 *= 1f + (1f - npc.scale);
                    if (npc.velocity.X < 0f - num167 || npc.velocity.X > num167)
                    {
                        if (npc.velocity.Y == 0f)
                        {
                            npc.velocity *= 0.8f;
                        }
                    }
                    else if (npc.velocity.X < num167 && npc.direction == 1)
                    {
                        npc.velocity.X += 0.07f;
                        if (npc.velocity.X > num167)
                        {
                            npc.velocity.X = num167;
                        }
                    }
                    else if (npc.velocity.X > 0f - num167 && npc.direction == -1)
                    {
                        npc.velocity.X -= 0.07f;
                        if (npc.velocity.X < 0f - num167)
                        {
                            npc.velocity.X = 0f - num167;
                        }
                    }
                    if (npc.velocity.Y == 0f && (!Main.dayTime || (double)npc.position.Y > Main.worldSurface * 16.0) && !Main.player[npc.target].dead)
                    {
                        Vector2 vector5 = npc.Center - Main.player[npc.target].Center;
                        int num168 = 50;
                        if (npc.type >= 494 && npc.type <= 495)
                        {
                            num168 = 42;
                        }
                        if (vector5.Length() < (float)num168 && Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                        {
                            npc.velocity.X *= 0.7f;
                            npc.ai[2] = 1f;
                        }
                    }
                }
                else
                {
                    npc.damage = (int)((double)npc.defDamage * 1.5);
                    npc.ai[3] = 1f;
                    npc.velocity.X *= 0.9f;
                    if ((double)Math.Abs(npc.velocity.X) < 0.1)
                    {
                        npc.velocity.X = 0f;
                    }
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= 20f || npc.velocity.Y != 0f || (Main.dayTime && (double)npc.position.Y < Main.worldSurface * 16.0))
                    {
                        npc.ai[2] = 0f;
                    }
                }
            }
            else if (npc.type != 110 && npc.type != 111 && npc.type != 206 && npc.type != 214 && npc.type != 215 && npc.type != 216 && npc.type != 290 && npc.type != 291 && npc.type != 292 && npc.type != 293 && npc.type != 350 && npc.type != 379 && npc.type != 380 && npc.type != 381 && npc.type != 382 && (npc.type < 449 || npc.type > 452) && npc.type != 468 && npc.type != 481 && npc.type != 411 && npc.type != 409 && (npc.type < 498 || npc.type > 506) && npc.type != 424 && npc.type != 426 && npc.type != 520)
            {
                float num169 = 1f;
                if (npc.type == 186)
                {
                    num169 = 1.1f;
                }
                if (npc.type == 187)
                {
                    num169 = 0.9f;
                }
                if (npc.type == 188)
                {
                    num169 = 1.2f;
                }
                if (npc.type == 189)
                {
                    num169 = 0.8f;
                }
                if (npc.type == 132)
                {
                    num169 = 0.95f;
                }
                if (npc.type == 200)
                {
                    num169 = 0.87f;
                }
                if (npc.type == 223)
                {
                    num169 = 1.05f;
                }
                if (npc.type == 489)
                {
                    float num170 = (Main.player[npc.target].Center - npc.Center).Length();
                    num170 *= 0.0025f;
                    if ((double)num170 > 1.5)
                    {
                        num170 = 1.5f;
                    }
                    num169 = ((!Main.expertMode) ? (2.5f - num170) : (3f - num170));
                    num169 *= 0.8f;
                }
                if (npc.type == NPCID.BloodZombie ||
                    npc.type == NPCID.Zombie ||
                    npc.type == NPCID.BaldZombie ||
                    npc.type == NPCID.PincushionZombie ||
                    npc.type == NPCID.SlimedZombie ||
                    npc.type == NPCID.SwampZombie ||
                    npc.type == NPCID.TwiggyZombie ||
                    npc.type == NPCID.FemaleZombie ||
                    npc.type == NPCID.ZombieRaincoat ||
                    npc.type == NPCID.ZombieXmas ||
                    npc.type == NPCID.ZombieSweater)
                {
                    num169 *= 1f + (1f - npc.scale);
                }
                if (npc.velocity.X < 0f - num169 || npc.velocity.X > num169)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity *= 0.8f;
                    }
                }
                else if (npc.velocity.X < num169 && npc.direction == 1)
                {
                    npc.velocity.X += 0.07f;
                    if (npc.velocity.X > num169)
                    {
                        npc.velocity.X = num169;
                    }
                }
                else if (npc.velocity.X > 0f - num169 && npc.direction == -1)
                {
                    npc.velocity.X -= 0.07f;
                    if (npc.velocity.X < 0f - num169)
                    {
                        npc.velocity.X = 0f - num169;
                    }
                }
            }
            if (npc.type >= 277 && npc.type <= 280)
            {
                Lighting.AddLight((int)npc.Center.X / 16, (int)npc.Center.Y / 16, 0.2f, 0.1f, 0f);
            }
            else if (npc.type == 520)
            {
                Lighting.AddLight(npc.Top + new Vector2(0f, 20f), 0.3f, 0.3f, 0.7f);
            }
            else if (npc.type == 525)
            {
                Vector3 rgb = new Vector3(0.7f, 1f, 0.2f) * 0.5f;
                Lighting.AddLight(npc.Top + new Vector2(0f, 15f), rgb);
            }
            else if (npc.type == 526)
            {
                Vector3 rgb2 = new Vector3(1f, 1f, 0.5f) * 0.4f;
                Lighting.AddLight(npc.Top + new Vector2(0f, 15f), rgb2);
            }
            else if (npc.type == 527)
            {
                Vector3 rgb3 = new Vector3(0.6f, 0.3f, 1f) * 0.4f;
                Lighting.AddLight(npc.Top + new Vector2(0f, 15f), rgb3);
            }
            else if (npc.type == NPCID.SolarDrakomire)
            {
                npc.hide = false;
                for (int num171 = 0; num171 < 200; num171++)
                {
                    if (Main.npc[num171].active && Main.npc[num171].type == 416 && Main.npc[num171].ai[0] == (float)npc.whoAmI)
                    {
                        npc.hide = true;
                        break;
                    }
                }
            }
            else if (npc.type == NPCID.MushiLadybug)
            {
                if (npc.velocity.Y != 0f)
                {
                    npc.TargetClosest();
                    npc.spriteDirection = npc.direction;
                    if (Main.player[npc.target].Center.X < npc.position.X && npc.velocity.X > 0f)
                    {
                        npc.velocity.X *= 0.95f;
                    }
                    else if (Main.player[npc.target].Center.X > npc.position.X + (float)npc.width && npc.velocity.X < 0f)
                    {
                        npc.velocity.X *= 0.95f;
                    }
                    if (Main.player[npc.target].Center.X < npc.position.X && npc.velocity.X > -5f)
                    {
                        npc.velocity.X -= 0.1f;
                    }
                    else if (Main.player[npc.target].Center.X > npc.position.X + (float)npc.width && npc.velocity.X < 5f)
                    {
                        npc.velocity.X += 0.1f;
                    }
                }
                else if (Main.player[npc.target].Center.Y + 50f < npc.position.Y && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    npc.velocity.Y = -7f;
                }
            }
            else if (npc.type == 425)
            {
                if (npc.velocity.Y == 0f)
                {
                    npc.ai[2] = 0f;
                }
                if (npc.velocity.Y != 0f && npc.ai[2] == 1f)
                {
                    npc.TargetClosest();
                    npc.spriteDirection = -npc.direction;
                    if (Collision.CanHit(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                    {
                        float num172 = Main.player[npc.target].Center.X - (float)(npc.direction * 400) - npc.Center.X;
                        float num173 = Main.player[npc.target].Bottom.Y - npc.Bottom.Y;
                        if (num172 < 0f && npc.velocity.X > 0f)
                        {
                            npc.velocity.X *= 0.9f;
                        }
                        else if (num172 > 0f && npc.velocity.X < 0f)
                        {
                            npc.velocity.X *= 0.9f;
                        }
                        if (num172 < 0f && npc.velocity.X > -5f)
                        {
                            npc.velocity.X -= 0.1f;
                        }
                        else if (num172 > 0f && npc.velocity.X < 5f)
                        {
                            npc.velocity.X += 0.1f;
                        }
                        if (npc.velocity.X > 6f)
                        {
                            npc.velocity.X = 6f;
                        }
                        if (npc.velocity.X < -6f)
                        {
                            npc.velocity.X = -6f;
                        }
                        if (num173 < -20f && npc.velocity.Y > 0f)
                        {
                            npc.velocity.Y *= 0.8f;
                        }
                        else if (num173 > 20f && npc.velocity.Y < 0f)
                        {
                            npc.velocity.Y *= 0.8f;
                        }
                        if (num173 < -20f && npc.velocity.Y > -5f)
                        {
                            npc.velocity.Y -= 0.3f;
                        }
                        else if (num173 > 20f && npc.velocity.Y < 5f)
                        {
                            npc.velocity.Y += 0.3f;
                        }
                    }
                    if (Main.rand.Next(3) == 0)
                    {
                        Vector2 position2 = npc.Center + new Vector2(npc.direction * -14, -8f) - Vector2.One * 4f;
                        Vector2 velocity = new Vector2(npc.direction * -6, 12f) * 0.2f + Utils.RandomVector2(Main.rand, -1f, 1f) * 0.1f;
                        Dust obj5 = Main.dust[Dust.NewDust(position2, 8, 8, 229, velocity.X, velocity.Y, 100, Color.Transparent, 1f + Main.rand.NextFloat() * 0.5f)];
                        obj5.noGravity = true;
                        obj5.velocity = velocity;
                        obj5.customData = this;
                    }
                    for (int num174 = 0; num174 < 200; num174++)
                    {
                        if (num174 != npc.whoAmI && Main.npc[num174].active && Main.npc[num174].type == npc.type && Math.Abs(npc.position.X - Main.npc[num174].position.X) + Math.Abs(npc.position.Y - Main.npc[num174].position.Y) < (float)npc.width)
                        {
                            if (npc.position.X < Main.npc[num174].position.X)
                            {
                                npc.velocity.X -= 0.05f;
                            }
                            else
                            {
                                npc.velocity.X += 0.05f;
                            }
                            if (npc.position.Y < Main.npc[num174].position.Y)
                            {
                                npc.velocity.Y -= 0.05f;
                            }
                            else
                            {
                                npc.velocity.Y += 0.05f;
                            }
                        }
                    }
                }
                else if (Main.player[npc.target].Center.Y + 100f < npc.position.Y && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    npc.velocity.Y = -5f;
                    npc.ai[2] = 1f;
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[2] += 1f;
                    if (npc.localAI[2] >= (float)(360 + Main.rand.Next(360)) && npc.Distance(Main.player[npc.target].Center) < 400f && Math.Abs(npc.DirectionTo(Main.player[npc.target].Center).Y) < 0.5f && Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                    {
                        npc.localAI[2] = 0f;
                        Vector2 vector6 = npc.Center + new Vector2(npc.direction * 30, 2f);
                        Vector2 vector7 = npc.DirectionTo(Main.player[npc.target].Center) * 7f;
                        if (vector7.HasNaNs())
                        {
                            vector7 = new Vector2(npc.direction * 8, 0f);
                        }
                        int num175 = (Main.expertMode ? 50 : 75);
                        for (int num177 = 0; num177 < 4; num177++)
                        {
                            Vector2 vector8 = vector7 + Utils.RandomVector2(Main.rand, -0.8f, 0.8f);
                            Projectile.NewProjectile(vector6.X, vector6.Y, vector8.X, vector8.Y, 577, num175, 1f, Main.myPlayer);
                        }
                    }
                }
            }
            else if (npc.type == 427)
            {
                if (npc.velocity.Y == 0f)
                {
                    npc.ai[2] = 0f;
                    npc.rotation = 0f;
                }
                else
                {
                    npc.rotation = npc.velocity.X * 0.1f;
                }
                if (npc.velocity.Y != 0f && npc.ai[2] == 1f)
                {
                    npc.TargetClosest();
                    npc.spriteDirection = -npc.direction;
                    if (Collision.CanHit(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                    {
                        float num178 = Main.player[npc.target].Center.X - npc.Center.X;
                        float num179 = Main.player[npc.target].Center.Y - npc.Center.Y;
                        if (num178 < 0f && npc.velocity.X > 0f)
                        {
                            npc.velocity.X *= 0.98f;
                        }
                        else if (num178 > 0f && npc.velocity.X < 0f)
                        {
                            npc.velocity.X *= 0.98f;
                        }
                        if (num178 < -20f && npc.velocity.X > -6f)
                        {
                            npc.velocity.X -= 0.015f;
                        }
                        else if (num178 > 20f && npc.velocity.X < 6f)
                        {
                            npc.velocity.X += 0.015f;
                        }
                        if (npc.velocity.X > 6f)
                        {
                            npc.velocity.X = 6f;
                        }
                        if (npc.velocity.X < -6f)
                        {
                            npc.velocity.X = -6f;
                        }
                        if (num179 < -20f && npc.velocity.Y > 0f)
                        {
                            npc.velocity.Y *= 0.98f;
                        }
                        else if (num179 > 20f && npc.velocity.Y < 0f)
                        {
                            npc.velocity.Y *= 0.98f;
                        }
                        if (num179 < -20f && npc.velocity.Y > -6f)
                        {
                            npc.velocity.Y -= 0.15f;
                        }
                        else if (num179 > 20f && npc.velocity.Y < 6f)
                        {
                            npc.velocity.Y += 0.15f;
                        }
                    }
                    for (int num180 = 0; num180 < 200; num180++)
                    {
                        if (num180 != npc.whoAmI && Main.npc[num180].active && Main.npc[num180].type == npc.type && Math.Abs(npc.position.X - Main.npc[num180].position.X) + Math.Abs(npc.position.Y - Main.npc[num180].position.Y) < (float)npc.width)
                        {
                            if (npc.position.X < Main.npc[num180].position.X)
                            {
                                npc.velocity.X -= 0.05f;
                            }
                            else
                            {
                                npc.velocity.X += 0.05f;
                            }
                            if (npc.position.Y < Main.npc[num180].position.Y)
                            {
                                npc.velocity.Y -= 0.05f;
                            }
                            else
                            {
                                npc.velocity.Y += 0.05f;
                            }
                        }
                    }
                }
                else if (Main.player[npc.target].Center.Y + 100f < npc.position.Y && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    npc.velocity.Y = -5f;
                    npc.ai[2] = 1f;
                }
            }
            else if (npc.type == 426)
            {
                if (npc.ai[1] > 0f && npc.velocity.Y > 0f)
                {
                    npc.velocity.Y *= 0.85f;
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity.Y = -0.4f;
                    }
                }
                if (npc.velocity.Y != 0f)
                {
                    npc.TargetClosest();
                    npc.spriteDirection = npc.direction;
                    if (Collision.CanHit(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                    {
                        float num181 = Main.player[npc.target].Center.X - (float)(npc.direction * 300) - npc.Center.X;
                        if (num181 < 40f && npc.velocity.X > 0f)
                        {
                            npc.velocity.X *= 0.98f;
                        }
                        else if (num181 > 40f && npc.velocity.X < 0f)
                        {
                            npc.velocity.X *= 0.98f;
                        }
                        if (num181 < 40f && npc.velocity.X > -5f)
                        {
                            npc.velocity.X -= 0.2f;
                        }
                        else if (num181 > 40f && npc.velocity.X < 5f)
                        {
                            npc.velocity.X += 0.2f;
                        }
                        if (npc.velocity.X > 6f)
                        {
                            npc.velocity.X = 6f;
                        }
                        if (npc.velocity.X < -6f)
                        {
                            npc.velocity.X = -6f;
                        }
                    }
                }
                else if (Main.player[npc.target].Center.Y + 100f < npc.position.Y && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    npc.velocity.Y = -6f;
                }
                for (int num182 = 0; num182 < 200; num182++)
                {
                    if (num182 != npc.whoAmI && Main.npc[num182].active && Main.npc[num182].type == npc.type && Math.Abs(npc.position.X - Main.npc[num182].position.X) + Math.Abs(npc.position.Y - Main.npc[num182].position.Y) < (float)npc.width)
                    {
                        if (npc.position.X < Main.npc[num182].position.X)
                        {
                            npc.velocity.X -= 0.1f;
                        }
                        else
                        {
                            npc.velocity.X += 0.1f;
                        }
                        if (npc.position.Y < Main.npc[num182].position.Y)
                        {
                            npc.velocity.Y -= 0.1f;
                        }
                        else
                        {
                            npc.velocity.Y += 0.1f;
                        }
                    }
                }
                if (Main.rand.Next(6) == 0 && npc.ai[1] <= 20f)
                {
                    Dust obj6 = Main.dust[Dust.NewDust(npc.Center + new Vector2((npc.spriteDirection == 1) ? 8 : (-20), -20f), 8, 8, 229, npc.velocity.X, npc.velocity.Y, 100)];
                    obj6.velocity = obj6.velocity / 4f + npc.velocity / 2f;
                    obj6.scale = 0.6f;
                    obj6.noLight = true;
                }
                if (npc.ai[1] >= 57f)
                {
                    int num183 = Utils.SelectRandom<int>(Main.rand, 161, 229);
                    Dust obj7 = Main.dust[Dust.NewDust(npc.Center + new Vector2((npc.spriteDirection == 1) ? 8 : (-20), -20f), 8, 8, num183, npc.velocity.X, npc.velocity.Y, 100)];
                    obj7.velocity = obj7.velocity / 4f + npc.DirectionTo(Main.player[npc.target].Top);
                    obj7.scale = 1.2f;
                    obj7.noLight = true;
                }
                if (Main.rand.Next(6) == 0)
                {
                    Dust dust2 = Main.dust[Dust.NewDust(npc.Center, 2, 2, 229)];
                    dust2.position = npc.Center + new Vector2((npc.spriteDirection == 1) ? 26 : (-26), 24f);
                    dust2.velocity.X = 0f;
                    if (dust2.velocity.Y < 0f)
                    {
                        dust2.velocity.Y = 0f;
                    }
                    dust2.noGravity = true;
                    dust2.scale = 1f;
                    dust2.noLight = true;
                }
            }
            else if (npc.type == 185)
            {
                if (npc.velocity.Y == 0f)
                {
                    npc.rotation = 0f;
                    npc.localAI[0] = 0f;
                }
                else if (npc.localAI[0] == 1f)
                {
                    npc.rotation += npc.velocity.X * 0.05f;
                }
            }
            else if (npc.type == 428)
            {
                if (npc.velocity.Y == 0f)
                {
                    npc.rotation = 0f;
                }
                else
                {
                    npc.rotation += npc.velocity.X * 0.08f;
                }
            }
            if (npc.type == NPCID.Vampire && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 vector9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float num188 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector9.X;
                float num184 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector9.Y;
                if ((float)Math.Sqrt(num188 * num188 + num184 * num184) > 300f)
                {
                    npc.Transform(NPCID.VampireBat);
                }
            }
            if (npc.type == NPCID.WallCreeper && Main.netMode != NetmodeID.MultiplayerClient && npc.velocity.Y == 0f)
            {
                int num185 = (int)npc.Center.X / 16;
                int num3 = (int)npc.Center.Y / 16;
                bool flag27 = false;
                for (int num4 = num185 - 1; num4 <= num185 + 1; num4++)
                {
                    for (int num5 = num3 - 1; num5 <= num3 + 1; num5++)
                    {
                        if (Main.tile[num4, num5].wall > 0)
                        {
                            flag27 = true;
                        }
                    }
                }
                if (flag27)
                {
                    npc.Transform(165);
                }
            }
            if (npc.type == NPCID.BloodCrawler && Main.netMode != NetmodeID.MultiplayerClient && npc.velocity.Y == 0f)
            {
                int num6 = (int)npc.Center.X / 16;
                int num7 = (int)npc.Center.Y / 16;
                bool flag2 = false;
                for (int num8 = num6 - 1; num8 <= num6 + 1; num8++)
                {
                    for (int num9 = num7 - 1; num9 <= num7 + 1; num9++)
                    {
                        if (Main.tile[num8, num9].wall > 0)
                        {
                            flag2 = true;
                        }
                    }
                }
                if (flag2)
                {
                    npc.Transform(NPCID.BloodCrawlerWall);
                }
            }
            if (npc.type == NPCID.DesertScorpionWalk && Main.netMode != NetmodeID.MultiplayerClient && npc.velocity.Y == 0f)
            {
                int num10 = (int)npc.Center.X / 16;
                int num11 = (int)npc.Center.Y / 16;
                bool flag3 = false;
                for (int num12 = num10 - 1; num12 <= num10 + 1; num12++)
                {
                    for (int num14 = num11 - 1; num14 <= num11 + 1; num14++)
                    {
                        if (Main.tile[num12, num14].wall > 0)
                        {
                            flag3 = true;
                        }
                    }
                }
                if (flag3)
                {
                    npc.Transform(NPCID.DesertScorpionWall);
                }
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && Main.expertMode && npc.target >= 0 && (npc.type == NPCID.BlackRecluse || npc.type == NPCID.BlackRecluseWall) && Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
            {
                npc.localAI[0] += 1f;
                if (npc.justHit)
                {
                    npc.localAI[0] -= Main.rand.Next(20, 60);
                    if (npc.localAI[0] < 0f)
                    {
                        npc.localAI[0] = 0f;
                    }
                }
                if (npc.localAI[0] > (float)Main.rand.Next(180, 900))
                {
                    npc.localAI[0] = 0f;
                    Vector2 vector10 = Main.player[npc.target].Center - npc.Center;
                    vector10.Normalize();
                    vector10 *= 8f;
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, vector10.X, vector10.Y, 472, 18, 0f, Main.myPlayer);
                }
            }
            if (npc.type == NPCID.BlackRecluse && Main.netMode != NetmodeID.MultiplayerClient && npc.velocity.Y == 0f)
            {
                int num15 = (int)npc.Center.X / 16;
                int num16 = (int)npc.Center.Y / 16;
                bool flag4 = false;
                for (int num17 = num15 - 1; num17 <= num15 + 1; num17++)
                {
                    for (int num18 = num16 - 1; num18 <= num16 + 1; num18++)
                    {
                        if (Main.tile[num17, num18].wall > 0)
                        {
                            flag4 = true;
                        }
                    }
                }
                if (flag4)
                {
                    npc.Transform(NPCID.BlackRecluseWall);
                }
            }
            if (npc.type == NPCID.JungleCreeper && Main.netMode != NetmodeID.MultiplayerClient && npc.velocity.Y == 0f)
            {
                int num19 = (int)npc.Center.X / 16;
                int num20 = (int)npc.Center.Y / 16;
                bool flag5 = false;
                for (int num21 = num19 - 1; num21 <= num19 + 1; num21++)
                {
                    for (int num22 = num20 - 1; num22 <= num20 + 1; num22++)
                    {
                        if (Main.tile[num21, num22].wall > 0)
                        {
                            flag5 = true;
                        }
                    }
                }
                if (flag5)
                {
                    npc.Transform(NPCID.JungleCreeperWall);
                }
            }
            if (npc.type == NPCID.IceGolem)
            {
                if (npc.justHit && Main.rand.Next(3) == 0)
                {
                    npc.ai[2] -= Main.rand.Next(30);
                }
                if (npc.ai[2] < 0f)
                {
                    npc.ai[2] = 0f;
                }
                if (npc.confused)
                {
                    npc.ai[2] = 0f;
                }
                npc.ai[2] += 1f;
                float num23 = Main.rand.Next(30, 900);
                num23 *= (float)npc.life / (float)npc.lifeMax;
                num23 += 30f;
                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] >= num23 && npc.velocity.Y == 0f && !Main.player[npc.target].dead && !Main.player[npc.target].frozen && ((npc.direction > 0 && npc.Center.X < Main.player[npc.target].Center.X) || (npc.direction < 0 && npc.Center.X > Main.player[npc.target].Center.X)) && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    Vector2 vector11 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + 20f);
                    vector11.X += 10 * npc.direction;
                    float num25 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector11.X;
                    float num26 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector11.Y;
                    num25 += (float)Main.rand.Next(-40, 41);
                    num26 += (float)Main.rand.Next(-40, 41);
                    float num27 = (float)Math.Sqrt(num25 * num25 + num26 * num26);
                    npc.netUpdate = true;
                    num27 = 15f / num27;
                    num25 *= num27;
                    num26 *= num27;
                    int num28 = 32;
                    int num29 = 257;
                    vector11.X += num25 * 3f;
                    vector11.Y += num26 * 3f;
                    Projectile.NewProjectile(vector11.X, vector11.Y, num25, num26, num29, num28, 0f, Main.myPlayer);
                    npc.ai[2] = 0f;
                }
            }
            if (npc.type == 251)
            {
                if (npc.justHit)
                {
                    npc.ai[2] -= Main.rand.Next(30);
                }
                if (npc.ai[2] < 0f)
                {
                    npc.ai[2] = 0f;
                }
                if (npc.confused)
                {
                    npc.ai[2] = 0f;
                }
                npc.ai[2] += 1f;
                float num30 = Main.rand.Next(60, 1800);
                num30 *= (float)npc.life / (float)npc.lifeMax;
                num30 += 15f;
                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] >= num30 && npc.velocity.Y == 0f && !Main.player[npc.target].dead && !Main.player[npc.target].frozen && ((npc.direction > 0 && npc.Center.X < Main.player[npc.target].Center.X) || (npc.direction < 0 && npc.Center.X > Main.player[npc.target].Center.X)) && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    Vector2 vector13 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + 12f);
                    vector13.X += 6 * npc.direction;
                    float num31 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector13.X;
                    float num32 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector13.Y;
                    num31 += (float)Main.rand.Next(-40, 41);
                    num32 += (float)Main.rand.Next(-30, 0);
                    float num33 = (float)Math.Sqrt(num31 * num31 + num32 * num32);
                    npc.netUpdate = true;
                    num33 = 15f / num33;
                    num31 *= num33;
                    num32 *= num33;
                    int num34 = 30;
                    int num36 = 83;
                    vector13.X += num31 * 3f;
                    vector13.Y += num32 * 3f;
                    Projectile.NewProjectile(vector13.X, vector13.Y, num31, num32, num36, num34, 0f, Main.myPlayer);
                    npc.ai[2] = 0f;
                }
            }
            if (npc.type == 386)
            {
                if (npc.confused)
                {
                    npc.ai[2] = -60f;
                }
                else
                {
                    if (npc.ai[2] < 60f)
                    {
                        npc.ai[2] += 1f;
                    }
                    if (npc.ai[2] > 0f && NPC.CountNPCS(387) >= 4 * NPC.CountNPCS(386))
                    {
                        npc.ai[2] = 0f;
                    }
                    if (npc.justHit)
                    {
                        npc.ai[2] = -30f;
                    }
                    if (npc.ai[2] == 30f)
                    {
                        int num37 = (int)npc.position.X / 16;
                        int num38 = (int)npc.position.Y / 16;
                        int num39 = (int)npc.position.X / 16;
                        int num40 = (int)npc.position.Y / 16;
                        int num41 = 5;
                        int num42 = 0;
                        bool flag6 = false;
                        int num43 = 2;
                        int num44 = 0;
                        while (!flag6 && num42 < 100)
                        {
                            num42++;
                            int num45 = Main.rand.Next(num37 - num41, num37 + num41);
                            for (int num47 = Main.rand.Next(num38 - num41, num38 + num41); num47 < num38 + num41; num47++)
                            {
                                if ((num47 < num38 - num43 || num47 > num38 + num43 || num45 < num37 - num43 || num45 > num37 + num43) && (num47 < num40 - num44 || num47 > num40 + num44 || num45 < num39 - num44 || num45 > num39 + num44) && Main.tile[num45, num47].nactive())
                                {
                                    bool flag7 = true;
                                    if (Main.tile[num45, num47 - 1].lava())
                                    {
                                        flag7 = false;
                                    }
                                    if (flag7 && Main.tileSolid[Main.tile[num45, num47].type] && !Collision.SolidTiles(num45 - 1, num45 + 1, num47 - 4, num47 - 1))
                                    {
                                        int num48 = NPC.NewNPC(num45 * 16 - npc.width / 2, num47 * 16, 387);
                                        Main.npc[num48].position.Y = num47 * 16 - Main.npc[num48].height;
                                        flag6 = true;
                                        npc.netUpdate = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (npc.ai[2] == 60f)
                    {
                        npc.ai[2] = -120f;
                    }
                }
            }
            if (npc.type == NPCID.GigaZapper)
            {
                if (npc.confused)
                {
                    npc.ai[2] = -60f;
                }
                else
                {
                    if (npc.ai[2] < 20f)
                    {
                        npc.ai[2] += 1f;
                    }
                    if (npc.justHit)
                    {
                        npc.ai[2] = -30f;
                    }
                    if (npc.ai[2] == 20f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.ai[2] = -10 + Main.rand.Next(3) * -10;
                        Projectile.NewProjectile(npc.Center.X, npc.Center.Y + 8f, npc.direction * 6, 0f, 437, 25, 1f, Main.myPlayer);
                    }
                }
            }
            if (npc.type == 110 || npc.type == 111 || npc.type == 206 || npc.type == 214 || npc.type == 215 || npc.type == 216 || npc.type == 290 || npc.type == 291 || npc.type == 292 || npc.type == 293 || npc.type == 350 || npc.type == 379 || npc.type == 380 || npc.type == 381 || npc.type == 382 || (npc.type >= 449 && npc.type <= 452) || npc.type == 468 || npc.type == 481 || npc.type == 411 || npc.type == 409 || (npc.type >= 498 && npc.type <= 506) || npc.type == 424 || npc.type == 426 || npc.type == 520)
            {
                bool flag8 = npc.type == 381 || npc.type == 382 || npc.type == 520;
                bool flag9 = npc.type == 426;
                bool flag10 = true;
                int num49 = -1;
                int num50 = -1;
                if (npc.type == 411)
                {
                    flag8 = true;
                    num49 = 90;
                    num50 = 90;
                    if (npc.ai[1] <= 150f)
                    {
                        flag10 = false;
                    }
                }
                if (npc.confused)
                {
                    npc.ai[2] = 0f;
                }
                else
                {
                    if (npc.ai[1] > 0f)
                    {
                        npc.ai[1] -= 1f;
                    }
                    if (npc.justHit)
                    {
                        npc.ai[1] = 30f;
                        npc.ai[2] = 0f;
                    }
                    int num51 = 70;
                    if (npc.type == 379 || npc.type == 380)
                    {
                        num51 = 80;
                    }
                    if (npc.type == 381 || npc.type == 382)
                    {
                        num51 = 80;
                    }
                    if (npc.type == 520)
                    {
                        num51 = 15;
                    }
                    if (npc.type == 350)
                    {
                        num51 = 110;
                    }
                    if (npc.type == 291)
                    {
                        num51 = 200;
                    }
                    if (npc.type == 292)
                    {
                        num51 = 120;
                    }
                    if (npc.type == 293)
                    {
                        num51 = 90;
                    }
                    if (npc.type == 111)
                    {
                        num51 = 180;
                    }
                    if (npc.type == 206)
                    {
                        num51 = 50;
                    }
                    if (npc.type == 481)
                    {
                        num51 = 100;
                    }
                    if (npc.type == 214)
                    {
                        num51 = 40;
                    }
                    if (npc.type == 215)
                    {
                        num51 = 80;
                    }
                    if (npc.type == 290)
                    {
                        num51 = 30;
                    }
                    if (npc.type == 411)
                    {
                        num51 = 300;
                    }
                    if (npc.type == 409)
                    {
                        num51 = 60;
                    }
                    if (npc.type == 424)
                    {
                        num51 = 180;
                    }
                    if (npc.type == 426)
                    {
                        num51 = 60;
                    }
                    bool flag11 = false;
                    if (npc.type == 216)
                    {
                        if (npc.localAI[2] >= 20f)
                        {
                            flag11 = true;
                        }
                        num51 = ((!flag11) ? 8 : 60);
                    }
                    int num52 = num51 / 2;
                    if (npc.type == 424)
                    {
                        num52 = num51 - 1;
                    }
                    if (npc.type == 426)
                    {
                        num52 = num51 - 1;
                    }
                    if (npc.ai[2] > 0f)
                    {
                        if (flag10)
                        {
                            npc.TargetClosest();
                        }
                        if (npc.ai[1] == (float)num52)
                        {
                            if (npc.type == 216)
                            {
                                npc.localAI[2] += 1f;
                            }
                            float num53 = 11f;
                            if (npc.type == 111)
                            {
                                num53 = 9f;
                            }
                            if (npc.type == 206)
                            {
                                num53 = 7f;
                            }
                            if (npc.type == 290)
                            {
                                num53 = 9f;
                            }
                            if (npc.type == 293)
                            {
                                num53 = 4f;
                            }
                            if (npc.type == 214)
                            {
                                num53 = 14f;
                            }
                            if (npc.type == 215)
                            {
                                num53 = 16f;
                            }
                            if (npc.type == 382)
                            {
                                num53 = 7f;
                            }
                            if (npc.type == 520)
                            {
                                num53 = 8f;
                            }
                            if (npc.type == 409)
                            {
                                num53 = 4f;
                            }
                            if (npc.type >= 449 && npc.type <= 452)
                            {
                                num53 = 7f;
                            }
                            if (npc.type == 481)
                            {
                                num53 = 8f;
                            }
                            if (npc.type == 468)
                            {
                                num53 = 7.5f;
                            }
                            if (npc.type == 411)
                            {
                                num53 = 1f;
                            }
                            if (npc.type >= 498 && npc.type <= 506)
                            {
                                num53 = 7f;
                            }
                            Vector2 vector14 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                            if (npc.type == 481)
                            {
                                vector14.Y -= 14f;
                            }
                            if (npc.type == 206)
                            {
                                vector14.Y -= 10f;
                            }
                            if (npc.type == 290)
                            {
                                vector14.Y -= 10f;
                            }
                            if (npc.type == 381 || npc.type == 382)
                            {
                                vector14.Y += 6f;
                            }
                            if (npc.type == 520)
                            {
                                vector14.Y = npc.position.Y + 20f;
                            }
                            if (npc.type >= 498 && npc.type <= 506)
                            {
                                vector14.Y -= 8f;
                            }
                            if (npc.type == 426)
                            {
                                vector14 += new Vector2(npc.spriteDirection * 2, -12f);
                            }
                            float num54 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector14.X;
                            float num55 = Math.Abs(num54) * 0.1f;
                            if (npc.type == 291 || npc.type == 292)
                            {
                                num55 = 0f;
                            }
                            if (npc.type == 215)
                            {
                                num55 = Math.Abs(num54) * 0.08f;
                            }
                            if (npc.type == 214 || (npc.type == 216 && !flag11))
                            {
                                num55 = 0f;
                            }
                            if (npc.type == 381 || npc.type == 382 || npc.type == 520)
                            {
                                num55 = 0f;
                            }
                            if (npc.type >= 449 && npc.type <= 452)
                            {
                                num55 = Math.Abs(num54) * (float)Main.rand.Next(10, 50) * 0.01f;
                            }
                            if (npc.type == 468)
                            {
                                num55 = Math.Abs(num54) * (float)Main.rand.Next(10, 50) * 0.01f;
                            }
                            if (npc.type == 481)
                            {
                                num55 = Math.Abs(num54) * (float)Main.rand.Next(-10, 11) * 0.0035f;
                            }
                            if (npc.type >= 498 && npc.type <= 506)
                            {
                                num55 = Math.Abs(num54) * (float)Main.rand.Next(1, 11) * 0.0025f;
                            }
                            float num56 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector14.Y - num55;
                            if (npc.type == 291)
                            {
                                num54 += (float)Main.rand.Next(-40, 41) * 0.2f;
                                num56 += (float)Main.rand.Next(-40, 41) * 0.2f;
                            }
                            else if (npc.type == 381 || npc.type == 382 || npc.type == 520)
                            {
                                num54 += (float)Main.rand.Next(-100, 101) * 0.4f;
                                num56 += (float)Main.rand.Next(-100, 101) * 0.4f;
                                num54 *= (float)Main.rand.Next(85, 116) * 0.01f;
                                num56 *= (float)Main.rand.Next(85, 116) * 0.01f;
                                if (npc.type == 520)
                                {
                                    num54 += (float)Main.rand.Next(-100, 101) * 0.6f;
                                    num56 += (float)Main.rand.Next(-100, 101) * 0.6f;
                                    num54 *= (float)Main.rand.Next(85, 116) * 0.015f;
                                    num56 *= (float)Main.rand.Next(85, 116) * 0.015f;
                                }
                            }
                            else if (npc.type == 481)
                            {
                                num54 += (float)Main.rand.Next(-40, 41) * 0.4f;
                                num56 += (float)Main.rand.Next(-40, 41) * 0.4f;
                            }
                            else if (npc.type >= 498 && npc.type <= 506)
                            {
                                num54 += (float)Main.rand.Next(-40, 41) * 0.3f;
                                num56 += (float)Main.rand.Next(-40, 41) * 0.3f;
                            }
                            else if (npc.type != 292)
                            {
                                num54 += (float)Main.rand.Next(-40, 41);
                                num56 += (float)Main.rand.Next(-40, 41);
                            }
                            float num58 = (float)Math.Sqrt(num54 * num54 + num56 * num56);
                            npc.netUpdate = true;
                            num58 = num53 / num58;
                            num54 *= num58;
                            num56 *= num58;
                            int num59 = 35;
                            int num60 = 82;
                            if (npc.type == 111)
                            {
                                num59 = 11;
                            }
                            if (npc.type == 206)
                            {
                                num59 = 37;
                            }
                            if (npc.type == 379 || npc.type == 380)
                            {
                                num59 = 40;
                            }
                            if (npc.type == 350)
                            {
                                num59 = 45;
                            }
                            if (npc.type == 468)
                            {
                                num59 = 50;
                            }
                            if (npc.type == 111)
                            {
                                num60 = 81;
                            }
                            if (npc.type == 379 || npc.type == 380)
                            {
                                num60 = 81;
                            }
                            if (npc.type == 381)
                            {
                                num60 = 436;
                                num59 = 24;
                            }
                            if (npc.type == 382)
                            {
                                num60 = 438;
                                num59 = 30;
                            }
                            if (npc.type == 520)
                            {
                                num60 = 592;
                                num59 = 35;
                            }
                            if (npc.type >= 449 && npc.type <= 452)
                            {
                                num60 = 471;
                                num59 = 20;
                            }
                            if (npc.type >= 498 && npc.type <= 506)
                            {
                                num60 = 572;
                                num59 = 14;
                            }
                            if (npc.type == 481)
                            {
                                num60 = 508;
                                num59 = 18;
                            }
                            if (npc.type == 206)
                            {
                                num60 = 177;
                            }
                            if (npc.type == 468)
                            {
                                num60 = 501;
                            }
                            if (npc.type == 411)
                            {
                                num60 = 537;
                                num59 = (Main.expertMode ? 45 : 60);
                            }
                            if (npc.type == 424)
                            {
                                num60 = 573;
                                num59 = (Main.expertMode ? 45 : 60);
                            }
                            if (npc.type == 426)
                            {
                                num60 = 581;
                                num59 = (Main.expertMode ? 45 : 60);
                            }
                            if (npc.type == 291)
                            {
                                num60 = 302;
                                num59 = 100;
                            }
                            if (npc.type == 290)
                            {
                                num60 = 300;
                                num59 = 60;
                            }
                            if (npc.type == 293)
                            {
                                num60 = 303;
                                num59 = 60;
                            }
                            if (npc.type == 214)
                            {
                                num60 = 180;
                                num59 = 25;
                            }
                            if (npc.type == 215)
                            {
                                num60 = 82;
                                num59 = 40;
                            }
                            if (npc.type == 292)
                            {
                                num59 = 50;
                                num60 = 180;
                            }
                            if (npc.type == 216)
                            {
                                num60 = 180;
                                num59 = 30;
                                if (flag11)
                                {
                                    num59 = 100;
                                    num60 = 240;
                                    npc.localAI[2] = 0f;
                                }
                            }
                            vector14.X += num54;
                            vector14.Y += num56;
                            if (Main.expertMode && npc.type == 290)
                            {
                                num59 = (int)((double)num59 * 0.75);
                            }
                            if (Main.expertMode && npc.type >= 381 && npc.type <= 392)
                            {
                                num59 = (int)((double)num59 * 0.8);
                            }
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (npc.type == 292)
                                {
                                    for (int num61 = 0; num61 < 4; num61++)
                                    {
                                        num54 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector14.X;
                                        num56 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector14.Y;
                                        num58 = (float)Math.Sqrt(num54 * num54 + num56 * num56);
                                        num58 = 12f / num58;
                                        num54 = (num54 += (float)Main.rand.Next(-40, 41));
                                        num56 = (num56 += (float)Main.rand.Next(-40, 41));
                                        num54 *= num58;
                                        num56 *= num58;
                                        Projectile.NewProjectile(vector14.X, vector14.Y, num54, num56, num60, num59, 0f, Main.myPlayer);
                                    }
                                }
                                else if (npc.type == 411)
                                {
                                    Projectile.NewProjectile(vector14.X, vector14.Y, num54, num56, num60, num59, 0f, Main.myPlayer, 0f, npc.whoAmI);
                                }
                                else if (npc.type == 424)
                                {
                                    for (int num62 = 0; num62 < 4; num62++)
                                    {
                                        Projectile.NewProjectile(npc.Center.X - (float)(npc.spriteDirection * 4), npc.Center.Y + 6f, (float)(-3 + 2 * num62) * 0.15f, (float)(-Main.rand.Next(0, 3)) * 0.2f - 0.1f, num60, num59, 0f, Main.myPlayer, 0f, npc.whoAmI);
                                    }
                                }
                                else if (npc.type == 409)
                                {
                                    int num63 = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, 410, npc.whoAmI);
                                    Main.npc[num63].velocity = new Vector2(num54, -6f + num56);
                                }
                                else
                                {
                                    Projectile.NewProjectile(vector14.X, vector14.Y, num54, num56, num60, num59, 0f, Main.myPlayer);
                                }
                            }
                            if (Math.Abs(num56) > Math.Abs(num54) * 2f)
                            {
                                if (num56 > 0f)
                                {
                                    npc.ai[2] = 1f;
                                }
                                else
                                {
                                    npc.ai[2] = 5f;
                                }
                            }
                            else if (Math.Abs(num54) > Math.Abs(num56) * 2f)
                            {
                                npc.ai[2] = 3f;
                            }
                            else if (num56 > 0f)
                            {
                                npc.ai[2] = 2f;
                            }
                            else
                            {
                                npc.ai[2] = 4f;
                            }
                        }
                        if ((npc.velocity.Y != 0f && !flag9) || npc.ai[1] <= 0f)
                        {
                            npc.ai[2] = 0f;
                            npc.ai[1] = 0f;
                        }
                        else if (!flag8 || (num49 != -1 && npc.ai[1] >= (float)num49 && npc.ai[1] < (float)(num49 + num50) && (!flag9 || npc.velocity.Y == 0f)))
                        {
                            npc.velocity.X *= 0.9f;
                            npc.spriteDirection = npc.direction;
                        }
                    }
                    if (npc.type == 468 && !Main.eclipse)
                    {
                        flag8 = true;
                    }
                    else if ((npc.ai[2] <= 0f || flag8) && (npc.velocity.Y == 0f || flag9) && npc.ai[1] <= 0f && !Main.player[npc.target].dead)
                    {
                        bool flag13 = Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);
                        if (npc.type == 520)
                        {
                            flag13 = Collision.CanHitLine(npc.Top + new Vector2(0f, 20f), 0, 0, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);
                        }
                        if (Main.player[npc.target].stealth == 0f && Main.player[npc.target].itemAnimation == 0)
                        {
                            flag13 = false;
                        }
                        if (flag13)
                        {
                            float num64 = 10f;
                            Vector2 vector15 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                            float num65 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector15.X;
                            float num66 = Math.Abs(num65) * 0.1f;
                            float num67 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector15.Y - num66;
                            num65 += (float)Main.rand.Next(-40, 41);
                            num67 += (float)Main.rand.Next(-40, 41);
                            float num69 = (float)Math.Sqrt(num65 * num65 + num67 * num67);
                            float num70 = 700f;
                            if (npc.type == 214)
                            {
                                num70 = 550f;
                            }
                            if (npc.type == 215)
                            {
                                num70 = 800f;
                            }
                            if (npc.type >= 498 && npc.type <= 506)
                            {
                                num70 = 190f;
                            }
                            if (npc.type >= 449 && npc.type <= 452)
                            {
                                num70 = 200f;
                            }
                            if (npc.type == 481)
                            {
                                num70 = 400f;
                            }
                            if (npc.type == 468)
                            {
                                num70 = 400f;
                            }
                            if (num69 < num70)
                            {
                                npc.netUpdate = true;
                                npc.velocity.X *= 0.5f;
                                num69 = num64 / num69;
                                num65 *= num69;
                                num67 *= num69;
                                npc.ai[2] = 3f;
                                npc.ai[1] = num51;
                                if (Math.Abs(num67) > Math.Abs(num65) * 2f)
                                {
                                    if (num67 > 0f)
                                    {
                                        npc.ai[2] = 1f;
                                    }
                                    else
                                    {
                                        npc.ai[2] = 5f;
                                    }
                                }
                                else if (Math.Abs(num65) > Math.Abs(num67) * 2f)
                                {
                                    npc.ai[2] = 3f;
                                }
                                else if (num67 > 0f)
                                {
                                    npc.ai[2] = 2f;
                                }
                                else
                                {
                                    npc.ai[2] = 4f;
                                }
                            }
                        }
                    }
                    if (npc.ai[2] <= 0f || (flag8 && (num49 == -1 || !(npc.ai[1] >= (float)num49) || !(npc.ai[1] < (float)(num49 + num50)))))
                    {
                        float num71 = 1f;
                        float num72 = 0.07f;
                        float num73 = 0.8f;
                        if (npc.type == 214)
                        {
                            num71 = 2f;
                            num72 = 0.09f;
                        }
                        else if (npc.type == 215)
                        {
                            num71 = 1.5f;
                            num72 = 0.08f;
                        }
                        else if (npc.type == 381 || npc.type == 382)
                        {
                            num71 = 2f;
                            num72 = 0.5f;
                        }
                        else if (npc.type == 520)
                        {
                            num71 = 4f;
                            num72 = 1f;
                            num73 = 0.7f;
                        }
                        else if (npc.type == 411)
                        {
                            num71 = 2f;
                            num72 = 0.5f;
                        }
                        else if (npc.type == 409)
                        {
                            num71 = 2f;
                            num72 = 0.5f;
                        }
                        bool flag14 = false;
                        if ((npc.type == 381 || npc.type == 382) && Vector2.Distance(npc.Center, Main.player[npc.target].Center) < 300f && Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                        {
                            flag14 = true;
                            npc.ai[3] = 0f;
                        }
                        if (npc.type == 520 && Vector2.Distance(npc.Center, Main.player[npc.target].Center) < 400f && Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                        {
                            flag14 = true;
                            npc.ai[3] = 0f;
                        }
                        if (npc.velocity.X < 0f - num71 || npc.velocity.X > num71 || flag14)
                        {
                            if (npc.velocity.Y == 0f)
                            {
                                npc.velocity *= num73;
                            }
                        }
                        else if (npc.velocity.X < num71 && npc.direction == 1)
                        {
                            npc.velocity.X += num72;
                            if (npc.velocity.X > num71)
                            {
                                npc.velocity.X = num71;
                            }
                        }
                        else if (npc.velocity.X > 0f - num71 && npc.direction == -1)
                        {
                            npc.velocity.X -= num72;
                            if (npc.velocity.X < 0f - num71)
                            {
                                npc.velocity.X = 0f - num71;
                            }
                        }
                    }
                    if (npc.type == NPCID.MartianWalker)
                    {
                        npc.localAI[2] += 1f;
                        if (npc.localAI[2] >= 6f)
                        {
                            npc.localAI[2] = 0f;
                            npc.localAI[3] = Main.player[npc.target].DirectionFrom(npc.Top + new Vector2(0f, 20f)).ToRotation();
                        }
                    }
                }
            }
            if (npc.type == 109 && Main.netMode != NetmodeID.MultiplayerClient && !Main.player[npc.target].dead)
            {
                if (npc.justHit)
                {
                    npc.ai[2] = 0f;
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] > 450f)
                {
                    Vector2 vector16 = new Vector2(npc.position.X + (float)npc.width * 0.5f - (float)(npc.direction * 24), npc.position.Y + 4f);
                    int num74 = 3 * npc.direction;
                    int num75 = -5;
                    int num76 = Projectile.NewProjectile(vector16.X, vector16.Y, num74, num75, 75, 0, 0f, Main.myPlayer);
                    Main.projectile[num76].timeLeft = 300;
                    npc.ai[2] = 0f;
                }
            }
            bool flag15 = false;
            if (npc.velocity.Y == 0f)
            {
                int num77 = (int)(npc.position.Y + (float)npc.height + 7f) / 16;
                int num189 = (int)npc.position.X / 16;
                int num79 = (int)(npc.position.X + (float)npc.width) / 16;
                for (int num80 = num189; num80 <= num79; num80++)
                {
                    if (Main.tile[num80, num77] == null)
                    {
                        return;
                    }
                    if (Main.tile[num80, num77].nactive() && Main.tileSolid[Main.tile[num80, num77].type])
                    {
                        flag15 = true;
                        break;
                    }
                }
            }
            if (npc.type == 428)
            {
                flag15 = false;
            }
            if (npc.velocity.Y >= 0f)
            {
                int num81 = 0;
                if (npc.velocity.X < 0f)
                {
                    num81 = -1;
                }
                if (npc.velocity.X > 0f)
                {
                    num81 = 1;
                }
                Vector2 position3 = npc.position;
                position3.X += npc.velocity.X;
                int num82 = (int)((position3.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 1) * num81)) / 16f);
                int num83 = (int)((position3.Y + (float)npc.height - 1f) / 16f);
                if (Main.tile[num82, num83] == null)
                {
                    Main.tile[num82, num83] = new Tile();
                }
                if (Main.tile[num82, num83 - 1] == null)
                {
                    Main.tile[num82, num83 - 1] = new Tile();
                }
                if (Main.tile[num82, num83 - 2] == null)
                {
                    Main.tile[num82, num83 - 2] = new Tile();
                }
                if (Main.tile[num82, num83 - 3] == null)
                {
                    Main.tile[num82, num83 - 3] = new Tile();
                }
                if (Main.tile[num82, num83 + 1] == null)
                {
                    Main.tile[num82, num83 + 1] = new Tile();
                }
                if (Main.tile[num82 - num81, num83 - 3] == null)
                {
                    Main.tile[num82 - num81, num83 - 3] = new Tile();
                }
                if ((float)(num82 * 16) < position3.X + (float)npc.width && (float)(num82 * 16 + 16) > position3.X && ((Main.tile[num82, num83].nactive() && !Main.tile[num82, num83].topSlope() && !Main.tile[num82, num83 - 1].topSlope() && Main.tileSolid[Main.tile[num82, num83].type] && !Main.tileSolidTop[Main.tile[num82, num83].type]) || (Main.tile[num82, num83 - 1].halfBrick() && Main.tile[num82, num83 - 1].nactive())) && (!Main.tile[num82, num83 - 1].nactive() || !Main.tileSolid[Main.tile[num82, num83 - 1].type] || Main.tileSolidTop[Main.tile[num82, num83 - 1].type] || (Main.tile[num82, num83 - 1].halfBrick() && (!Main.tile[num82, num83 - 4].nactive() || !Main.tileSolid[Main.tile[num82, num83 - 4].type] || Main.tileSolidTop[Main.tile[num82, num83 - 4].type]))) && (!Main.tile[num82, num83 - 2].nactive() || !Main.tileSolid[Main.tile[num82, num83 - 2].type] || Main.tileSolidTop[Main.tile[num82, num83 - 2].type]) && (!Main.tile[num82, num83 - 3].nactive() || !Main.tileSolid[Main.tile[num82, num83 - 3].type] || Main.tileSolidTop[Main.tile[num82, num83 - 3].type]) && (!Main.tile[num82 - num81, num83 - 3].nactive() || !Main.tileSolid[Main.tile[num82 - num81, num83 - 3].type]))
                {
                    float num84 = num83 * 16;
                    if (Main.tile[num82, num83].halfBrick())
                    {
                        num84 += 8f;
                    }
                    if (Main.tile[num82, num83 - 1].halfBrick())
                    {
                        num84 -= 8f;
                    }
                    if (num84 < position3.Y + (float)npc.height)
                    {
                        float num85 = position3.Y + (float)npc.height - num84;
                        float num86 = 16.1f;
                        if (npc.type == 163 || npc.type == 164 || npc.type == 236 || npc.type == 239 || npc.type == 530)
                        {
                            num86 += 8f;
                        }
                        if (num85 <= num86)
                        {
                            npc.gfxOffY += npc.position.Y + (float)npc.height - num84;
                            npc.position.Y = num84 - (float)npc.height;
                            if (num85 < 9f)
                            {
                                npc.stepSpeed = 1f;
                            }
                            else
                            {
                                npc.stepSpeed = 2f;
                            }
                        }
                    }
                }
            }
            if (flag15)
            {
                int num87 = (int)((npc.position.X + (float)(npc.width / 2) + (float)(15 * npc.direction)) / 16f);
                int num88 = (int)((npc.position.Y + (float)npc.height - 15f) / 16f);
                if (npc.type == 109 || npc.type == 163 || npc.type == 164 || npc.type == 199 || npc.type == 236 || npc.type == 239 || npc.type == 257 || npc.type == 258 || npc.type == 290 || npc.type == 391 || npc.type == 425 || npc.type == 427 || npc.type == 426 || npc.type == 508 || npc.type == 415 || npc.type == 530 || npc.type == 532)
                {
                    num87 = (int)((npc.position.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 16) * npc.direction)) / 16f);
                }
                if (Main.tile[num87, num88] == null)
                {
                    Main.tile[num87, num88] = new Tile();
                }
                if (Main.tile[num87, num88 - 1] == null)
                {
                    Main.tile[num87, num88 - 1] = new Tile();
                }
                if (Main.tile[num87, num88 - 2] == null)
                {
                    Main.tile[num87, num88 - 2] = new Tile();
                }
                if (Main.tile[num87, num88 - 3] == null)
                {
                    Main.tile[num87, num88 - 3] = new Tile();
                }
                if (Main.tile[num87, num88 + 1] == null)
                {
                    Main.tile[num87, num88 + 1] = new Tile();
                }
                if (Main.tile[num87 + npc.direction, num88 - 1] == null)
                {
                    Main.tile[num87 + npc.direction, num88 - 1] = new Tile();
                }
                if (Main.tile[num87 + npc.direction, num88 + 1] == null)
                {
                    Main.tile[num87 + npc.direction, num88 + 1] = new Tile();
                }
                if (Main.tile[num87 - npc.direction, num88 + 1] == null)
                {
                    Main.tile[num87 - npc.direction, num88 + 1] = new Tile();
                }
                Main.tile[num87, num88 + 1].halfBrick();
                if (Main.tile[num87, num88 - 1].nactive() && (TileLoader.IsClosedDoor(Main.tile[num87, num88 - 1]) || Main.tile[num87, num88 - 1].type == 388) && flag23)
                {
                    npc.ai[2] += 1f;
                    npc.ai[3] = 0f;
                    if (npc.ai[2] >= 60f)
                    {
                        if (!Main.bloodMoon && (npc.type == 3 || npc.type == 331 || npc.type == 332 || npc.type == 132 || npc.type == 161 || npc.type == 186 || npc.type == 187 || npc.type == 188 || npc.type == 189 || npc.type == 200 || npc.type == 223 || npc.type == 320 || npc.type == 321 || npc.type == 319))
                        {
                            npc.ai[1] = 0f;
                        }
                        npc.velocity.X = 0.5f * (float)(-npc.direction);
                        int num90 = 5;
                        if (Main.tile[num87, num88 - 1].type == 388)
                        {
                            num90 = 2;
                        }
                        npc.ai[1] += num90;
                        if (npc.type == 27)
                        {
                            npc.ai[1] += 1f;
                        }
                        if (npc.type == 31 || npc.type == 294 || npc.type == 295 || npc.type == 296)
                        {
                            npc.ai[1] += 6f;
                        }
                        npc.ai[2] = 0f;
                        bool flag16 = false;
                        if (npc.ai[1] >= 10f)
                        {
                            flag16 = true;
                            npc.ai[1] = 10f;
                        }
                        if (npc.type == 460)
                        {
                            flag16 = true;
                        }
                        WorldGen.KillTile(num87, num88 - 1, fail: true);
                        if ((Main.netMode != NetmodeID.MultiplayerClient || !flag16) && flag16 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (npc.type == 26)
                            {
                                WorldGen.KillTile(num87, num88 - 1);
                                if (Main.netMode == 2)
                                {
                                    NetMessage.SendData(17, -1, -1, null, 0, num87, num88 - 1);
                                }
                            }
                            else
                            {
                                if (TileLoader.OpenDoorID(Main.tile[num87, num88 - 1]) >= 0)
                                {
                                    bool flag17 = WorldGen.OpenDoor(num87, num88 - 1, npc.direction);
                                    if (!flag17)
                                    {
                                        npc.ai[3] = num119;
                                        npc.netUpdate = true;
                                    }
                                    if (Main.netMode == 2 && flag17)
                                    {
                                        NetMessage.SendData(19, -1, -1, null, 0, num87, num88 - 1, npc.direction);
                                    }
                                }
                                if (Main.tile[num87, num88 - 1].type == 388)
                                {
                                    bool flag18 = WorldGen.ShiftTallGate(num87, num88 - 1, closing: false);
                                    if (!flag18)
                                    {
                                        npc.ai[3] = num119;
                                        npc.netUpdate = true;
                                    }
                                    if (Main.netMode == 2 && flag18)
                                    {
                                        NetMessage.SendData(19, -1, -1, null, 4, num87, num88 - 1);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    int num91 = npc.spriteDirection;
                    if (npc.type == 425)
                    {
                        num91 *= -1;
                    }
                    if ((npc.velocity.X < 0f && num91 == -1) || (npc.velocity.X > 0f && num91 == 1))
                    {
                        if (npc.height >= 32 && Main.tile[num87, num88 - 2].nactive() && Main.tileSolid[Main.tile[num87, num88 - 2].type])
                        {
                            if (Main.tile[num87, num88 - 3].nactive() && Main.tileSolid[Main.tile[num87, num88 - 3].type])
                            {
                                npc.velocity.Y = -8f;
                                npc.netUpdate = true;
                            }
                            else
                            {
                                npc.velocity.Y = -7f;
                                npc.netUpdate = true;
                            }
                        }
                        else if (Main.tile[num87, num88 - 1].nactive() && Main.tileSolid[Main.tile[num87, num88 - 1].type])
                        {
                            npc.velocity.Y = -6f;
                            npc.netUpdate = true;
                        }
                        else if (npc.position.Y + (float)npc.height - (float)(num88 * 16) > 20f && Main.tile[num87, num88].nactive() && !Main.tile[num87, num88].topSlope() && Main.tileSolid[Main.tile[num87, num88].type])
                        {
                            npc.velocity.Y = -5f;
                            npc.netUpdate = true;
                        }
                        else if (npc.directionY < 0 && npc.type != 67 && (!Main.tile[num87, num88 + 1].nactive() || !Main.tileSolid[Main.tile[num87, num88 + 1].type]) && (!Main.tile[num87 + npc.direction, num88 + 1].nactive() || !Main.tileSolid[Main.tile[num87 + npc.direction, num88 + 1].type]))
                        {
                            npc.velocity.Y = -8f;
                            npc.velocity.X *= 1.5f;
                            npc.netUpdate = true;
                        }
                        else if (flag23)
                        {
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                        }
                        if (npc.velocity.Y == 0f && flag21 && npc.ai[3] == 1f)
                        {
                            npc.velocity.Y = -5f;
                        }
                    }
                    if ((npc.type == 31 || npc.type == 294 || npc.type == 295 || npc.type == 296 || npc.type == 47 || npc.type == 77 || npc.type == 104 || npc.type == 168 || npc.type == 196 || npc.type == 385 || npc.type == 389 || npc.type == 464 || npc.type == 470 || (npc.type >= 524 && npc.type <= 527)) && npc.velocity.Y == 0f && Math.Abs(npc.position.X + (float)(npc.width / 2) - (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))) < 100f && Math.Abs(npc.position.Y + (float)(npc.height / 2) - (Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2))) < 50f && ((npc.direction > 0 && npc.velocity.X >= 1f) || (npc.direction < 0 && npc.velocity.X <= -1f)))
                    {
                        npc.velocity.X *= 2f;
                        if (npc.velocity.X > 3f)
                        {
                            npc.velocity.X = 3f;
                        }
                        if (npc.velocity.X < -3f)
                        {
                            npc.velocity.X = -3f;
                        }
                        npc.velocity.Y = -4f;
                        npc.netUpdate = true;
                    }
                    if (npc.type == 120 && npc.velocity.Y < 0f)
                    {
                        npc.velocity.Y *= 1.1f;
                    }
                    if (npc.type == 287 && npc.velocity.Y == 0f && Math.Abs(npc.position.X + (float)(npc.width / 2) - (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))) < 150f && Math.Abs(npc.position.Y + (float)(npc.height / 2) - (Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2))) < 50f && ((npc.direction > 0 && npc.velocity.X >= 1f) || (npc.direction < 0 && npc.velocity.X <= -1f)))
                    {
                        npc.velocity.X = 8 * npc.direction;
                        npc.velocity.Y = -4f;
                        npc.netUpdate = true;
                    }
                    if (npc.type == 287 && npc.velocity.Y < 0f)
                    {
                        npc.velocity.X *= 1.2f;
                        npc.velocity.Y *= 1.1f;
                    }
                    if (npc.type == 460 && npc.velocity.Y < 0f)
                    {
                        npc.velocity.X *= 1.3f;
                        npc.velocity.Y *= 1.1f;
                    }
                }
            }
            else if (flag23)
            {
                npc.ai[1] = 0f;
                npc.ai[2] = 0f;
            }
            if (Main.netMode == 1 || npc.type != 120 || !(npc.ai[3] >= (float)num119))
            {
                return;
            }
            int num92 = (int)Main.player[npc.target].position.X / 16;
            int num93 = (int)Main.player[npc.target].position.Y / 16;
            int num94 = (int)npc.position.X / 16;
            int num95 = (int)npc.position.Y / 16;
            int num96 = 20;
            int num97 = 0;
            bool flag19 = false;
            if (Math.Abs(npc.position.X - Main.player[npc.target].position.X) + Math.Abs(npc.position.Y - Main.player[npc.target].position.Y) > 2000f)
            {
                num97 = 100;
                flag19 = true;
            }
            while (!flag19 && num97 < 100)
            {
                num97++;
                int num98 = Main.rand.Next(num92 - num96, num92 + num96);
                for (int num99 = Main.rand.Next(num93 - num96, num93 + num96); num99 < num93 + num96; num99++)
                {
                    if ((num99 < num93 - 4 || num99 > num93 + 4 || num98 < num92 - 4 || num98 > num92 + 4) && (num99 < num95 - 1 || num99 > num95 + 1 || num98 < num94 - 1 || num98 > num94 + 1) && Main.tile[num98, num99].nactive())
                    {
                        bool flag20 = true;
                        if (npc.type == 32 && Main.tile[num98, num99 - 1].wall == 0)
                        {
                            flag20 = false;
                        }
                        else if (Main.tile[num98, num99 - 1].lava())
                        {
                            flag20 = false;
                        }
                        if (flag20 && Main.tileSolid[Main.tile[num98, num99].type] && !Collision.SolidTiles(num98 - 1, num98 + 1, num99 - 4, num99 - 1))
                        {
                            npc.position.X = num98 * 16 - npc.width / 2;
                            npc.position.Y = num99 * 16 - npc.height;
                            npc.netUpdate = true;
                            npc.ai[3] = -120f;
                        }
                    }
                }
            }
        }

        public virtual bool WiderNPC => false;
        public virtual float SpeedCap => 2f;
        public virtual bool KnocksOnDoors => false;
        public virtual bool OpenDoor()
        {
            npc.ai[1] += 2f;
            return npc.ai[1] > 10f;
        }

        public virtual bool JumpCheck(int tileX, int tileY)
        {
            if (npc.height >= 32 && Main.tile[tileX, tileY - 2].nactive() && Main.tileSolid[Main.tile[tileX, tileY - 2].type])
            {
                if (Main.tile[tileX, tileY - 3].nactive() && Main.tileSolid[Main.tile[tileX, tileY - 3].type])
                {
                    npc.velocity.Y = -8f;
                    npc.netUpdate = true;
                }
                else
                {
                    npc.velocity.Y = -7f;
                    npc.netUpdate = true;
                }
                return true;
            }
            else if (Main.tile[tileX, tileY - 1].nactive() && Main.tileSolid[Main.tile[tileX, tileY - 1].type])
            {
                npc.velocity.Y = -6f;
                npc.netUpdate = true;
                return true;
            }
            else if (npc.position.Y + (float)npc.height - (float)(tileY * 16) > 20f && Main.tile[tileX, tileY].nactive() && !Main.tile[tileX, tileY].topSlope() && Main.tileSolid[Main.tile[tileX, tileY].type])
            {
                npc.velocity.Y = -5f;
                npc.netUpdate = true;
                return true;
            }
            else if (npc.directionY < 0 && (!Main.tile[tileX, tileY + 1].nactive() || !Main.tileSolid[Main.tile[tileX, tileY + 1].type]) && (!Main.tile[tileX + npc.direction, tileY + 1].nactive() || !Main.tileSolid[Main.tile[tileX + npc.direction, tileY + 1].type]))
            {
                npc.velocity.Y = -8f;
                npc.velocity.X *= 1.5f;
                npc.netUpdate = true;
                return true;
            }
            return false;
        }

        public override void AI()
        {
            int targetDelay = 60;
            bool knocksOnDoors = KnocksOnDoors;
            int npcTypeForSomeReason = npc.type;

            npc.TargetClosest(faceTarget: true);

            if (npc.velocity.X < -SpeedCap || npc.velocity.X > SpeedCap)
            {
                if (npc.velocity.Y == 0f)
                {
                    npc.velocity *= 0.8f;
                }
            }
            else if (npc.velocity.X < SpeedCap && npc.direction == 1)
            {
                npc.velocity.X += 0.07f;
                if (npc.velocity.X > SpeedCap)
                {
                    npc.velocity.X = SpeedCap;
                }
            }
            else if (npc.velocity.X > -SpeedCap && npc.direction == -1)
            {
                npc.velocity.X -= 0.07f;
                if (npc.velocity.X < -SpeedCap)
                {
                    npc.velocity.X = -SpeedCap;
                }
            }

            bool jumpCheckIThink = false;
            if (npc.velocity.Y == 0f)
            {
                int num77 = (int)(npc.position.Y + (float)npc.height + 7f) / 16;
                int num189 = (int)npc.position.X / 16;
                int num79 = (int)(npc.position.X + (float)npc.width) / 16;
                for (int num80 = num189; num80 <= num79; num80++)
                {
                    if (Main.tile[num80, num77] == null)
                    {
                        return;
                    }
                    if (Main.tile[num80, num77].nactive() && Main.tileSolid[Main.tile[num80, num77].type])
                    {
                        jumpCheckIThink = true;
                        break;
                    }
                }
            }
            if (npc.velocity.Y >= 0f)
            {
                int num81 = 0;
                if (npc.velocity.X < 0f)
                {
                    num81 = -1;
                }
                if (npc.velocity.X > 0f)
                {
                    num81 = 1;
                }
                Vector2 position3 = npc.position;
                position3.X += npc.velocity.X;
                int num82 = (int)((position3.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 1) * num81)) / 16f);
                int num83 = (int)((position3.Y + (float)npc.height - 1f) / 16f);
                if (Main.tile[num82, num83] == null)
                {
                    Main.tile[num82, num83] = new Tile();
                }
                if (Main.tile[num82, num83 - 1] == null)
                {
                    Main.tile[num82, num83 - 1] = new Tile();
                }
                if (Main.tile[num82, num83 - 2] == null)
                {
                    Main.tile[num82, num83 - 2] = new Tile();
                }
                if (Main.tile[num82, num83 - 3] == null)
                {
                    Main.tile[num82, num83 - 3] = new Tile();
                }
                if (Main.tile[num82, num83 + 1] == null)
                {
                    Main.tile[num82, num83 + 1] = new Tile();
                }
                if (Main.tile[num82 - num81, num83 - 3] == null)
                {
                    Main.tile[num82 - num81, num83 - 3] = new Tile();
                }
                if ((float)(num82 * 16) < position3.X + (float)npc.width && (float)(num82 * 16 + 16) > position3.X && ((Main.tile[num82, num83].nactive() && !Main.tile[num82, num83].topSlope() && !Main.tile[num82, num83 - 1].topSlope() && Main.tileSolid[Main.tile[num82, num83].type] && !Main.tileSolidTop[Main.tile[num82, num83].type]) || (Main.tile[num82, num83 - 1].halfBrick() && Main.tile[num82, num83 - 1].nactive())) && (!Main.tile[num82, num83 - 1].nactive() || !Main.tileSolid[Main.tile[num82, num83 - 1].type] || Main.tileSolidTop[Main.tile[num82, num83 - 1].type] || (Main.tile[num82, num83 - 1].halfBrick() && (!Main.tile[num82, num83 - 4].nactive() || !Main.tileSolid[Main.tile[num82, num83 - 4].type] || Main.tileSolidTop[Main.tile[num82, num83 - 4].type]))) && (!Main.tile[num82, num83 - 2].nactive() || !Main.tileSolid[Main.tile[num82, num83 - 2].type] || Main.tileSolidTop[Main.tile[num82, num83 - 2].type]) && (!Main.tile[num82, num83 - 3].nactive() || !Main.tileSolid[Main.tile[num82, num83 - 3].type] || Main.tileSolidTop[Main.tile[num82, num83 - 3].type]) && (!Main.tile[num82 - num81, num83 - 3].nactive() || !Main.tileSolid[Main.tile[num82 - num81, num83 - 3].type]))
                {
                    float num84 = num83 * 16;
                    if (Main.tile[num82, num83].halfBrick())
                    {
                        num84 += 8f;
                    }
                    if (Main.tile[num82, num83 - 1].halfBrick())
                    {
                        num84 -= 8f;
                    }
                    if (num84 < position3.Y + (float)npc.height)
                    {
                        float num85 = position3.Y + (float)npc.height - num84;
                        float num86 = 16.1f;
                        if (npc.type == 163 || npc.type == 164 || npc.type == 236 || npc.type == 239 || npc.type == 530)
                        {
                            num86 += 8f;
                        }
                        if (num85 <= num86)
                        {
                            npc.gfxOffY += npc.position.Y + (float)npc.height - num84;
                            npc.position.Y = num84 - (float)npc.height;
                            if (num85 < 9f)
                            {
                                npc.stepSpeed = 1f;
                            }
                            else
                            {
                                npc.stepSpeed = 2f;
                            }
                        }
                    }
                }
            }
            if (jumpCheckIThink)
            {
                int tileX = (int)((npc.position.X + npc.width / 2 + (float)(15 * npc.direction)) / 16f);
                int tileY = (int)((npc.position.Y + (float)npc.height - 15f) / 16f);
                if (WiderNPC)
                {
                    tileX = (int)((npc.position.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 16) * npc.direction)) / 16f);
                }
                if (Main.tile[tileX, tileY] == null)
                {
                    Main.tile[tileX, tileY] = new Tile();
                }
                if (Main.tile[tileX, tileY - 1] == null)
                {
                    Main.tile[tileX, tileY - 1] = new Tile();
                }
                if (Main.tile[tileX, tileY - 2] == null)
                {
                    Main.tile[tileX, tileY - 2] = new Tile();
                }
                if (Main.tile[tileX, tileY - 3] == null)
                {
                    Main.tile[tileX, tileY - 3] = new Tile();
                }
                if (Main.tile[tileX, tileY + 1] == null)
                {
                    Main.tile[tileX, tileY + 1] = new Tile();
                }
                if (Main.tile[tileX + npc.direction, tileY - 1] == null)
                {
                    Main.tile[tileX + npc.direction, tileY - 1] = new Tile();
                }
                if (Main.tile[tileX + npc.direction, tileY + 1] == null)
                {
                    Main.tile[tileX + npc.direction, tileY + 1] = new Tile();
                }
                if (Main.tile[tileX - npc.direction, tileY + 1] == null)
                {
                    Main.tile[tileX - npc.direction, tileY + 1] = new Tile();
                }
                Main.tile[tileX, tileY + 1].halfBrick();
                if (knocksOnDoors && Main.tile[tileX, tileY - 1].nactive() && (TileLoader.IsClosedDoor(Main.tile[tileX, tileY - 1]) || Main.tile[tileX, tileY - 1].type == 388))
                {
                    npc.ai[2] += 1f;
                    npc.ai[3] = 0f;
                    if (npc.ai[2] >= 60f)
                    {
                        npc.velocity.X = 0.5f * -npc.direction;
                        npc.ai[2] = 0f;
                        bool openDoor = OpenDoor();
                        WorldGen.KillTile(tileX, tileY - 1, fail: true);
                        if (openDoor && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (TileLoader.OpenDoorID(Main.tile[tileX, tileY - 1]) >= 0)
                            {
                                bool actuallyOpenedDoor = WorldGen.OpenDoor(tileX, tileY - 1, npc.direction);
                                if (!actuallyOpenedDoor)
                                {
                                    npc.ai[3] = targetDelay;
                                    npc.netUpdate = true;
                                }
                                if (Main.netMode == NetmodeID.Server && actuallyOpenedDoor)
                                {
                                    NetMessage.SendData(MessageID.ChangeDoor, -1, -1, null, 0, tileX, tileY - 1, npc.direction);
                                }
                            }
                            if (Main.tile[tileX, tileY - 1].type == 388)
                            {
                                bool flag18 = WorldGen.ShiftTallGate(tileX, tileY - 1, closing: false);
                                if (!flag18)
                                {
                                    npc.ai[3] = targetDelay;
                                    npc.netUpdate = true;
                                }
                                if (Main.netMode == NetmodeID.Server && flag18)
                                {
                                    NetMessage.SendData(MessageID.ChangeDoor, -1, -1, null, 4, tileX, tileY - 1);
                                }
                            }
                        }
                    }
                }
                else
                {
                    int num91 = npc.spriteDirection;
                    if (npc.velocity.X < 0f && num91 == -1 || (npc.velocity.X > 0f && num91 == 1))
                    {
                        if (!JumpCheck(tileX, tileY))
                        {
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                        }
                    }
                }
            }
            else if (knocksOnDoors)
            {
                npc.ai[1] = 0f;
                npc.ai[2] = 0f;
            }
        }
    }
}