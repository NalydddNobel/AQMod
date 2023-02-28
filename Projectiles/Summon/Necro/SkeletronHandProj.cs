using Aequus.Content;
using Aequus.Content.Necromancy;
using Aequus.Content.Necromancy.Renderer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon.Necro
{
    public class SkeletronHandProj : ModProjectile
    {
        public override string Texture => $"{Aequus.VanillaTexture}NPC_{NPCID.SkeletronHand}";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Type] = true;
            PushableEntities.AddProj(Type);
        }

        public int NPCIndex => (int)Projectile.ai[0];
        public NPC NPC => Main.npc[(int)Projectile.ai[0]];

        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 52;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.aiStyle = -1;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            Projectile.timeLeft = 2;
            Projectile.direction = -Projectile.spriteDirection;
            var target = Projectile.FindTargetWithinRange(800f);
            if (target == null)
            {
                Projectile.ai[1] = 0f;
            }
            if (!Main.npc[NPCIndex].active)
            {
                Projectile.ai[1] += 10f;
                if (Projectile.ai[1] > 50f || Main.netMode != NetmodeID.Server)
                {
                    Projectile.Kill();
                }
            }
            if (Projectile.ai[1] == 0f || Projectile.ai[1] == 3f)
            {
                if (target == null)
                {
                    if (Projectile.position.Y > Main.npc[NPCIndex].position.Y - 100f)
                    {
                        if (Projectile.velocity.Y > 0f)
                        {
                            Projectile.velocity.Y *= 0.96f;
                        }
                        Projectile.velocity.Y -= 0.07f;
                        if (Projectile.velocity.Y > 6f)
                        {
                            Projectile.velocity.Y = 6f;
                        }
                    }
                    else if (Projectile.position.Y < Main.npc[NPCIndex].position.Y - 100f)
                    {
                        if (Projectile.velocity.Y < 0f)
                        {
                            Projectile.velocity.Y *= 0.96f;
                        }
                        Projectile.velocity.Y += 0.07f;
                        if (Projectile.velocity.Y < -6f)
                        {
                            Projectile.velocity.Y = -6f;
                        }
                    }
                    if (Projectile.position.X + Projectile.width / 2 > Main.npc[NPCIndex].position.X + Main.npc[NPCIndex].width / 2 - 120f * Projectile.direction)
                    {
                        if (Projectile.velocity.X > 0f)
                        {
                            Projectile.velocity.X *= 0.96f;
                        }
                        Projectile.velocity.X -= 0.1f;
                        if (Projectile.velocity.X > 8f)
                        {
                            Projectile.velocity.X = 8f;
                        }
                    }
                    if (Projectile.position.X + Projectile.width / 2 < Main.npc[NPCIndex].position.X + Main.npc[NPCIndex].width / 2 - 120f * Projectile.direction)
                    {
                        if (Projectile.velocity.X < 0f)
                        {
                            Projectile.velocity.X *= 0.96f;
                        }
                        Projectile.velocity.X += 0.1f;
                        if (Projectile.velocity.X < -8f)
                        {
                            Projectile.velocity.X = -8f;
                        }
                    }
                }
                else
                {
                    Projectile.localAI[0] += 1f;
                    //if (Main.expertMode)
                    //{
                    //	Projectile.localAI[0] += 0.5f;
                    //}
                    if (Projectile.localAI[0] >= 300f)
                    {
                        Projectile.ai[1] += 1f;
                        Projectile.localAI[0] = 0f;
                        Projectile.netUpdate = true;
                    }
                    //if (Main.expertMode)
                    //{
                    //	if (Projectile.position.Y > Main.npc[NPC].position.Y + 230f)
                    //	{
                    //		if (Projectile.velocity.Y > 0f)
                    //		{
                    //			Projectile.velocity.Y *= 0.96f;
                    //		}
                    //		Projectile.velocity.Y -= 0.04f;
                    //		if (Projectile.velocity.Y > 3f)
                    //		{
                    //			Projectile.velocity.Y = 3f;
                    //		}
                    //	}
                    //	else if (Projectile.position.Y < Main.npc[NPC].position.Y + 230f)
                    //	{
                    //		if (Projectile.velocity.Y < 0f)
                    //		{
                    //			Projectile.velocity.Y *= 0.96f;
                    //		}
                    //		Projectile.velocity.Y += 0.04f;
                    //		if (Projectile.velocity.Y < -3f)
                    //		{
                    //			Projectile.velocity.Y = -3f;
                    //		}
                    //	}
                    //	if (Projectile.position.X + (float)(Projectile.width / 2) > Main.npc[NPC].position.X + (float)(Main.npc[NPC].width / 2) - 200f * Projectile.direction)
                    //	{
                    //		if (Projectile.velocity.X > 0f)
                    //		{
                    //			Projectile.velocity.X *= 0.96f;
                    //		}
                    //		Projectile.velocity.X -= 0.07f;
                    //		if (Projectile.velocity.X > 8f)
                    //		{
                    //			Projectile.velocity.X = 8f;
                    //		}
                    //	}
                    //	if (Projectile.position.X + (float)(Projectile.width / 2) < Main.npc[NPC].position.X + (float)(Main.npc[NPC].width / 2) - 200f * Projectile.direction)
                    //	{
                    //		if (Projectile.velocity.X < 0f)
                    //		{
                    //			Projectile.velocity.X *= 0.96f;
                    //		}
                    //		Projectile.velocity.X += 0.07f;
                    //		if (Projectile.velocity.X < -8f)
                    //		{
                    //			Projectile.velocity.X = -8f;
                    //		}
                    //	}
                    //}
                    if (Projectile.position.Y > Main.npc[NPCIndex].position.Y)
                    {
                        if (Projectile.velocity.Y > 0f)
                        {
                            Projectile.velocity.Y *= 0.96f;
                        }
                        Projectile.velocity.Y -= 0.04f;
                        if (Projectile.velocity.Y > 3f)
                        {
                            Projectile.velocity.Y = 3f;
                        }
                    }
                    else if (Projectile.position.Y < Main.npc[NPCIndex].position.Y)
                    {
                        if (Projectile.velocity.Y < 0f)
                        {
                            Projectile.velocity.Y *= 0.96f;
                        }
                        Projectile.velocity.Y += 0.04f;
                        if (Projectile.velocity.Y < -3f)
                        {
                            Projectile.velocity.Y = -3f;
                        }
                    }
                    if (Projectile.position.X + Projectile.width / 2 > Main.npc[NPCIndex].position.X + Main.npc[NPCIndex].width / 2 - 200f * Projectile.direction)
                    {
                        if (Projectile.velocity.X > 0f)
                        {
                            Projectile.velocity.X *= 0.96f;
                        }
                        Projectile.velocity.X -= 0.07f;
                        if (Projectile.velocity.X > 8f)
                        {
                            Projectile.velocity.X = 8f;
                        }
                    }
                    if (Projectile.position.X + Projectile.width / 2 < Main.npc[NPCIndex].position.X + Main.npc[NPCIndex].width / 2 - 200f * Projectile.direction)
                    {
                        if (Projectile.velocity.X < 0f)
                        {
                            Projectile.velocity.X *= 0.96f;
                        }
                        Projectile.velocity.X += 0.07f;
                        if (Projectile.velocity.X < -8f)
                        {
                            Projectile.velocity.X = -8f;
                        }
                    }
                }
                Vector2 vector101 = new Vector2(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f);
                float num684 = Main.npc[NPCIndex].position.X + Main.npc[NPCIndex].width / 2 - 200f * Projectile.direction - vector101.X;
                float num685 = Main.npc[NPCIndex].position.Y + 230f - vector101.Y;
                float num686 = (float)Math.Sqrt(num684 * num684 + num685 * num685);
                Projectile.rotation = (float)Math.Atan2(num685, num684) + 1.57f;
            }
            else if (Projectile.ai[1] == 1f)
            {
                Vector2 vector113 = new Vector2(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f);
                float num687 = Main.npc[NPCIndex].position.X + Main.npc[NPCIndex].width / 2 - 200f * Projectile.direction - vector113.X;
                float num688 = Main.npc[NPCIndex].position.Y + 230f - vector113.Y;
                float num689 = (float)Math.Sqrt(num687 * num687 + num688 * num688);
                Projectile.rotation = (float)Math.Atan2(num688, num687) + 1.57f;
                Projectile.velocity.X *= 0.95f;
                Projectile.velocity.Y -= 0.1f;
                //if (Main.expertMode)
                //{
                //	Projectile.velocity.Y -= 0.06f;
                //	if (Projectile.velocity.Y < -13f)
                //	{
                //		Projectile.velocity.Y = -13f;
                //	}
                //}
                //else
                if (Projectile.velocity.Y < -8f)
                {
                    Projectile.velocity.Y = -8f;
                }
                if (Projectile.position.Y < Main.npc[NPCIndex].position.Y - 200f)
                {
                    Projectile.ai[1] = 2f;
                    vector113 = new Vector2(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f);
                    num687 = target.position.X + target.width / 2 - vector113.X;
                    num688 = target.position.Y + target.height / 2 - vector113.Y;
                    num689 = (float)Math.Sqrt(num687 * num687 + num688 * num688);
                    num689 = (18f / num689); //((!Main.expertMode) ?  (18f / num689) : (21f / num689));
                    Projectile.velocity.X = num687 * num689;
                    Projectile.velocity.Y = num688 * num689;
                    Projectile.netUpdate = true;
                }
            }
            else if (Projectile.ai[1] == 2f)
            {
                if (Projectile.position.Y > target.position.Y || Projectile.velocity.Y < 0f)
                {
                    Projectile.ai[1] = 3f;
                }
            }
            else if (Projectile.ai[1] == 4f)
            {
                Vector2 vector124 = new Vector2(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f);
                float num690 = Main.npc[NPCIndex].position.X + Main.npc[NPCIndex].width / 2 - 200f * Projectile.direction - vector124.X;
                float num691 = Main.npc[NPCIndex].position.Y + 230f - vector124.Y;
                float num693 = (float)Math.Sqrt(num690 * num690 + num691 * num691);
                Projectile.rotation = (float)Math.Atan2(num691, num690) + 1.57f;
                Projectile.velocity.Y *= 0.95f;
                Projectile.velocity.X += 0.1f * (0f - Projectile.direction);
                //if (Main.expertMode)
                //{
                //	Projectile.velocity.X += 0.07f * (0f - Projectile.direction);
                //	if (Projectile.velocity.X < -12f)
                //	{
                //		Projectile.velocity.X = -12f;
                //	}
                //	else if (Projectile.velocity.X > 12f)
                //	{
                //		Projectile.velocity.X = 12f;
                //	}
                //}
                //else 
                if (Projectile.velocity.X < -8f)
                {
                    Projectile.velocity.X = -8f;
                }
                else if (Projectile.velocity.X > 8f)
                {
                    Projectile.velocity.X = 8f;
                }
                if (Projectile.position.X + Projectile.width / 2 < Main.npc[NPCIndex].position.X + Main.npc[NPCIndex].width / 2 - 500f || Projectile.position.X + Projectile.width / 2 > Main.npc[NPCIndex].position.X + Main.npc[NPCIndex].width / 2 + 500f)
                {
                    //TargetClosest();
                    Projectile.ai[1] = 5f;
                    vector124 = new Vector2(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f);
                    num690 = target.position.X + target.width / 2 - vector124.X;
                    num691 = target.position.Y + target.height / 2 - vector124.Y;
                    num693 = (float)Math.Sqrt(num690 * num690 + num691 * num691);
                    num693 = ((!Main.expertMode) ? (17f / num693) : (22f / num693));
                    Projectile.velocity.X = num690 * num693;
                    Projectile.velocity.Y = num691 * num693;
                    Projectile.netUpdate = true;
                }
            }
            else if (Projectile.ai[1] == 5f && ((Projectile.velocity.X > 0f && Projectile.position.X + Projectile.width / 2 > target.position.X + target.width / 2) || (Projectile.velocity.X < 0f && Projectile.position.X + Projectile.width / 2 < target.position.X + target.width / 2)))
            {
                Projectile.ai[1] = 0f;
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            GhostRenderer.GetColorTarget(Main.player[Projectile.owner], NPC.GetGlobalNPC<NecromancyNPC>().renderLayer).Projs.Add(Projectile);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var screenPos = Main.screenPosition;
            var vector5 = new Vector2(Projectile.position.X + Projectile.width * 0.5f - 5f * Projectile.spriteDirection, Projectile.position.Y + 20f);
            for (int j = 0; j < 2; j++)
            {
                float num6 = Main.npc[NPCIndex].position.X + Main.npc[NPCIndex].width / 2 - vector5.X;
                float num7 = Main.npc[NPCIndex].position.Y + (float)(Main.npc[NPCIndex].height / 2 - 60f) - vector5.Y;
                float num8 = 0f;
                if (j == 0)
                {
                    num6 -= 200f * -Projectile.spriteDirection;
                    num7 += 130f;
                    num8 = (float)Math.Sqrt(num6 * num6 + num7 * num7);
                    num8 = 92f / num8;
                    vector5.X += num6 * num8;
                    vector5.Y += num7 * num8;
                }
                else
                {
                    num6 -= 50f * -Projectile.spriteDirection;
                    num7 += 80f;
                    num8 = (float)Math.Sqrt(num6 * num6 + num7 * num7);
                    num8 = 60f / num8;
                    vector5.X += num6 * num8;
                    vector5.Y += num7 * num8;
                }
                float rotation5 = (float)Math.Atan2(num7, num6) - 1.57f;
                Color color5 = Lighting.GetColor((int)vector5.X / 16, (int)(vector5.Y / 16f));
                Main.EntitySpriteDraw(TextureAssets.BoneArm.Value, new Vector2(vector5.X - screenPos.X, vector5.Y - screenPos.Y), (Rectangle?)new Rectangle(0, 0, TextureAssets.BoneArm.Width(), TextureAssets.BoneArm.Height()), color5, rotation5, new Vector2(TextureAssets.BoneArm.Width() * 0.5f, TextureAssets.BoneArm.Height() * 0.5f), 1f, 0, 0);
                if (j == 0)
                {
                    vector5.X += num6 * num8 / 2f;
                    vector5.Y += num7 * num8 / 2f;
                }
                else if (Aequus.GameWorldActive)
                {
                    vector5.X += num6 * num8 - 16f;
                    vector5.Y += num7 * num8 - 6f;
                    Vector2 position = new Vector2(vector5.X, vector5.Y);
                    float speedX = num6 * 0.02f;
                    float speedY = num7 * 0.02f;
                    var d = Dust.NewDustDirect(position, 30, 10, DustID.Blood, speedX, speedY, Scale: 2f);
                    d.noGravity = true;
                }
            }
            Projectile.GetDrawInfo(out var t, out var off, out var frame, out var origin, out int _);
            Main.EntitySpriteDraw(t, Projectile.position + off - screenPos + new Vector2(6f * -Projectile.spriteDirection, -10f), frame, Helper.GetColor(Projectile.position + off), Projectile.rotation, origin, Projectile.scale, Projectile.GetSpriteEffect(), 0);
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }
    }
}