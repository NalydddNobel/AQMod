using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace AQMod.NPCs.AI
{
    public abstract class AIBat : AIClone
    {
        public sealed override void BaseAI()
        {
            if (npc.type == NPCID.Hellbat || npc.type == NPCID.Lavabat)
            {
                int num692 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Fire, npc.velocity.X * 0.2f, npc.velocity.Y * 0.2f, 100, default(Color), 2f);
                Main.dust[num692].noGravity = true;
            }
            if (npc.type == NPCID.IceBat && Main.rand.Next(10) == 0)
            {
                int num693 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 67, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, 90, default(Color), 1.5f);
                Main.dust[num693].noGravity = true;
                Dust dust35 = Main.dust[num693];
                Dust dust81 = dust35;
                dust81.velocity *= 0.2f;
                Main.dust[num693].noLight = true;
            }
            npc.noGravity = true;
            if (npc.collideX)
            {
                npc.velocity.X = npc.oldVelocity.X * -0.5f;
                if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                {
                    npc.velocity.X = 2f;
                }
                if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                {
                    npc.velocity.X = -2f;
                }
            }
            if (npc.collideY)
            {
                npc.velocity.Y = npc.oldVelocity.Y * -0.5f;
                if (npc.velocity.Y > 0f && npc.velocity.Y < 1f)
                {
                    npc.velocity.Y = 1f;
                }
                if (npc.velocity.Y < 0f && npc.velocity.Y > -1f)
                {
                    npc.velocity.Y = -1f;
                }
            }
            if (npc.type == NPCID.FlyingSnake)
            {
                int direction = 1;
                int num694 = 1;
                if (npc.velocity.X < 0f)
                {
                    direction = -1;
                }
                if (npc.velocity.Y < 0f)
                {
                    num694 = -1;
                }
                npc.TargetClosest();
                if (!Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    npc.direction = direction;
                    npc.directionY = num694;
                }
            }
            else
            {
                npc.TargetClosest();
            }
            if (npc.type == NPCID.VampireBat)
            {
                if (npc.position.Y < Main.worldSurface * 16f && Main.dayTime && !Main.eclipse)
                {
                    npc.directionY = -1;
                    npc.direction *= -1;
                }
                if (npc.direction == -1 && npc.velocity.X > -7f)
                {
                    npc.velocity.X -= 0.2f;
                    if (npc.velocity.X > 4f)
                    {
                        npc.velocity.X -= 0.1f;
                    }
                    else if (npc.velocity.X > 0f)
                    {
                        npc.velocity.X += 0.05f;
                    }
                    if (npc.velocity.X < -7f)
                    {
                        npc.velocity.X = -7f;
                    }
                }
                else if (npc.direction == 1 && npc.velocity.X < 7f)
                {
                    npc.velocity.X += 0.2f;
                    if (npc.velocity.X < -4f)
                    {
                        npc.velocity.X += 0.1f;
                    }
                    else if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X -= 0.05f;
                    }
                    if (npc.velocity.X > 7f)
                    {
                        npc.velocity.X = 7f;
                    }
                }
                if (npc.directionY == -1 && npc.velocity.Y > -7f)
                {
                    npc.velocity.Y -= 0.2f;
                    if (npc.velocity.Y > 4f)
                    {
                        npc.velocity.Y -= 0.1f;
                    }
                    else if (npc.velocity.Y > 0f)
                    {
                        npc.velocity.Y += 0.05f;
                    }
                    if (npc.velocity.Y < -7f)
                    {
                        npc.velocity.Y = -7f;
                    }
                }
                else if (npc.directionY == 1 && npc.velocity.Y < 7f)
                {
                    npc.velocity.Y += 0.2f;
                    if (npc.velocity.Y < -4f)
                    {
                        npc.velocity.Y += 0.1f;
                    }
                    else if (npc.velocity.Y < 0f)
                    {
                        npc.velocity.Y -= 0.05f;
                    }
                    if (npc.velocity.Y > 7f)
                    {
                        npc.velocity.Y = 7f;
                    }
                }
            }
            else if (npc.type == NPCID.FlyingSnake)
            {
                if (npc.direction == -1 && npc.velocity.X > -4f)
                {
                    npc.velocity.X -= 0.2f;
                    if (npc.velocity.X > 4f)
                    {
                        npc.velocity.X -= 0.1f;
                    }
                    else if (npc.velocity.X > 0f)
                    {
                        npc.velocity.X += 0.05f;
                    }
                    if (npc.velocity.X < -4f)
                    {
                        npc.velocity.X = -4f;
                    }
                }
                else if (npc.direction == 1 && npc.velocity.X < 4f)
                {
                    npc.velocity.X += 0.2f;
                    if (npc.velocity.X < -4f)
                    {
                        npc.velocity.X += 0.1f;
                    }
                    else if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X -= 0.05f;
                    }
                    if (npc.velocity.X > 4f)
                    {
                        npc.velocity.X = 4f;
                    }
                }
                if (npc.directionY == -1 && npc.velocity.Y > -2.5)
                {
                    npc.velocity.Y -= 0.1f;
                    if (npc.velocity.Y > 2.5)
                    {
                        npc.velocity.Y -= 0.05f;
                    }
                    else if (npc.velocity.Y > 0f)
                    {
                        npc.velocity.Y += 0.03f;
                    }
                    if (npc.velocity.Y < -2.5)
                    {
                        npc.velocity.Y = -2.5f;
                    }
                }
                else if (npc.directionY == 1 && npc.velocity.Y < 2.5)
                {
                    npc.velocity.Y += 0.1f;
                    if (npc.velocity.Y < -2.5)
                    {
                        npc.velocity.Y += 0.05f;
                    }
                    else if (npc.velocity.Y < 0f)
                    {
                        npc.velocity.Y -= 0.03f;
                    }
                    if (npc.velocity.Y > 2.5)
                    {
                        npc.velocity.Y = 2.5f;
                    }
                }
            }
            else
            {
                if (npc.direction == -1 && npc.velocity.X > -4f)
                {
                    npc.velocity.X -= 0.1f;
                    if (npc.velocity.X > 4f)
                    {
                        npc.velocity.X -= 0.1f;
                    }
                    else if (npc.velocity.X > 0f)
                    {
                        npc.velocity.X += 0.05f;
                    }
                    if (npc.velocity.X < -4f)
                    {
                        npc.velocity.X = -4f;
                    }
                }
                else if (npc.direction == 1 && npc.velocity.X < 4f)
                {
                    npc.velocity.X += 0.1f;
                    if (npc.velocity.X < -4f)
                    {
                        npc.velocity.X += 0.1f;
                    }
                    else if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X -= 0.05f;
                    }
                    if (npc.velocity.X > 4f)
                    {
                        npc.velocity.X = 4f;
                    }
                }
                if (npc.directionY == -1 && npc.velocity.Y > -1.5)
                {
                    npc.velocity.Y -= 0.04f;
                    if (npc.velocity.Y > 1.5)
                    {
                        npc.velocity.Y -= 0.05f;
                    }
                    else if (npc.velocity.Y > 0f)
                    {
                        npc.velocity.Y += 0.03f;
                    }
                    if (npc.velocity.Y < -1.5)
                    {
                        npc.velocity.Y = -1.5f;
                    }
                }
                else if (npc.directionY == 1 && npc.velocity.Y < 1.5)
                {
                    npc.velocity.Y += 0.04f;
                    if (npc.velocity.Y < -1.5)
                    {
                        npc.velocity.Y += 0.05f;
                    }
                    else if (npc.velocity.Y < 0f)
                    {
                        npc.velocity.Y -= 0.03f;
                    }
                    if (npc.velocity.Y > 1.5)
                    {
                        npc.velocity.Y = 1.5f;
                    }
                }
            }
            if (npc.type == NPCID.CaveBat ||
                npc.type == NPCID.JungleBat ||
                npc.type == NPCID.Hellbat ||
                npc.type == NPCID.Demon ||
                npc.type == NPCID.VoodooDemon ||
                npc.type == NPCID.GiantBat ||
                npc.type == NPCID.IlluminantBat ||
                npc.type == NPCID.IceBat ||
                npc.type == NPCID.Lavabat ||
                npc.type == NPCID.GiantFlyingFox)
            {
                if (npc.wet)
                {
                    if (npc.velocity.Y > 0f)
                    {
                        npc.velocity.Y *= 0.95f;
                    }
                    npc.velocity.Y -= 0.5f;
                    if (npc.velocity.Y < -4f)
                    {
                        npc.velocity.Y = -4f;
                    }
                    npc.TargetClosest();
                }
                if (npc.type == NPCID.Hellbat)
                {
                    if (npc.direction == -1 && npc.velocity.X > -4f)
                    {
                        npc.velocity.X -= 0.1f;
                        if (npc.velocity.X > 4f)
                        {
                            npc.velocity.X -= 0.07f;
                        }
                        else if (npc.velocity.X > 0f)
                        {
                            npc.velocity.X += 0.03f;
                        }
                        if (npc.velocity.X < -4f)
                        {
                            npc.velocity.X = -4f;
                        }
                    }
                    else if (npc.direction == 1 && npc.velocity.X < 4f)
                    {
                        npc.velocity.X += 0.1f;
                        if (npc.velocity.X < -4f)
                        {
                            npc.velocity.X += 0.07f;
                        }
                        else if (npc.velocity.X < 0f)
                        {
                            npc.velocity.X -= 0.03f;
                        }
                        if (npc.velocity.X > 4f)
                        {
                            npc.velocity.X = 4f;
                        }
                    }
                    if (npc.directionY == -1 && npc.velocity.Y > -1.5)
                    {
                        npc.velocity.Y -= 0.04f;
                        if (npc.velocity.Y > 1.5)
                        {
                            npc.velocity.Y -= 0.03f;
                        }
                        else if (npc.velocity.Y > 0f)
                        {
                            npc.velocity.Y += 0.02f;
                        }
                        if (npc.velocity.Y < -1.5)
                        {
                            npc.velocity.Y = -1.5f;
                        }
                    }
                    else if (npc.directionY == 1 && npc.velocity.Y < 1.5)
                    {
                        npc.velocity.Y += 0.04f;
                        if (npc.velocity.Y < -1.5)
                        {
                            npc.velocity.Y += 0.03f;
                        }
                        else if (npc.velocity.Y < 0f)
                        {
                            npc.velocity.Y -= 0.02f;
                        }
                        if (npc.velocity.Y > 1.5)
                        {
                            npc.velocity.Y = 1.5f;
                        }
                    }
                }
                else
                {
                    if (npc.direction == -1 && npc.velocity.X > -4f)
                    {
                        npc.velocity.X -= 0.1f;
                        if (npc.velocity.X > 4f)
                        {
                            npc.velocity.X -= 0.1f;
                        }
                        else if (npc.velocity.X > 0f)
                        {
                            npc.velocity.X += 0.05f;
                        }
                        if (npc.velocity.X < -4f)
                        {
                            npc.velocity.X = -4f;
                        }
                    }
                    else if (npc.direction == 1 && npc.velocity.X < 4f)
                    {
                        npc.velocity.X += 0.1f;
                        if (npc.velocity.X < -4f)
                        {
                            npc.velocity.X += 0.1f;
                        }
                        else if (npc.velocity.X < 0f)
                        {
                            npc.velocity.X -= 0.05f;
                        }
                        if (npc.velocity.X > 4f)
                        {
                            npc.velocity.X = 4f;
                        }
                    }
                    if (npc.directionY == -1 && npc.velocity.Y > -1.5)
                    {
                        npc.velocity.Y -= 0.04f;
                        if (npc.velocity.Y > 1.5)
                        {
                            npc.velocity.Y -= 0.05f;
                        }
                        else if (npc.velocity.Y > 0f)
                        {
                            npc.velocity.Y += 0.03f;
                        }
                        if (npc.velocity.Y < -1.5)
                        {
                            npc.velocity.Y = -1.5f;
                        }
                    }
                    else if (npc.directionY == 1 && npc.velocity.Y < 1.5)
                    {
                        npc.velocity.Y += 0.04f;
                        if (npc.velocity.Y < -1.5)
                        {
                            npc.velocity.Y += 0.05f;
                        }
                        else if (npc.velocity.Y < 0f)
                        {
                            npc.velocity.Y -= 0.03f;
                        }
                        if (npc.velocity.Y > 1.5)
                        {
                            npc.velocity.Y = 1.5f;
                        }
                    }
                }
            }
            if (npc.type == NPCID.Harpy && npc.wet)
            {
                if (npc.velocity.Y > 0f)
                {
                    npc.velocity.Y *= 0.95f;
                }
                npc.velocity.Y -= 0.5f;
                if (npc.velocity.Y < -4f)
                {
                    npc.velocity.Y = -4f;
                }
                npc.TargetClosest();
            }
            if (npc.type == NPCID.VampireBat && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 vector146 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float num695 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector146.X;
                float num696 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector146.Y;
                float num697 = (float)Math.Sqrt(num695 * num695 + num696 * num696);
                if (num697 < 200f && npc.position.Y + (float)npc.height < Main.player[npc.target].position.Y + (float)Main.player[npc.target].height && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    npc.Transform(NPCID.Vampire);
                }
            }
            npc.ai[1]++;
            if (npc.type == NPCID.VampireBat)
            {
                npc.ai[1]++;
            }
            if (npc.ai[1] > 200f)
            {
                if (!Main.player[npc.target].wet && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    npc.ai[1] = 0f;
                }
                float num698 = 0.2f;
                float num699 = 0.1f;
                float num701 = 4f;
                float num702 = 1.5f;
                if (npc.type == NPCID.Harpy || npc.type == NPCID.Demon || npc.type == NPCID.VoodooDemon)
                {
                    num698 = 0.12f;
                    num699 = 0.07f;
                    num701 = 3f;
                    num702 = 1.25f;
                }
                if (npc.ai[1] > 1000f)
                {
                    npc.ai[1] = 0f;
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] > 0f)
                {
                    if (npc.velocity.Y < num702)
                    {
                        npc.velocity.Y += num699;
                    }
                }
                else if (npc.velocity.Y > 0f - num702)
                {
                    npc.velocity.Y -= num699;
                }
                if (npc.ai[2] < -150f || npc.ai[2] > 150f)
                {
                    if (npc.velocity.X < num701)
                    {
                        npc.velocity.X += num698;
                    }
                }
                else if (npc.velocity.X > 0f - num701)
                {
                    npc.velocity.X -= num698;
                }
                if (npc.ai[2] > 300f)
                {
                    npc.ai[2] = -300f;
                }
            }
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            if (npc.type == NPCID.Harpy)
            {
                npc.ai[0] += 1f;
                if (npc.ai[0] == 30f || npc.ai[0] == 60f || npc.ai[0] == 90f)
                {
                    if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    {
                        float num703 = 6f;
                        Vector2 vector157 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                        float num704 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector157.X + (float)Main.rand.Next(-100, 101);
                        float num705 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector157.Y + (float)Main.rand.Next(-100, 101);
                        float num706 = (float)Math.Sqrt(num704 * num704 + num705 * num705);
                        num706 = num703 / num706;
                        num704 *= num706;
                        num705 *= num706;
                        int num707 = 15;
                        int num708 = 38;
                        int num709 = Projectile.NewProjectile(vector157.X, vector157.Y, num704, num705, num708, num707, 0f, Main.myPlayer);
                        Main.projectile[num709].timeLeft = 300;
                    }
                }
                else if (npc.ai[0] >= (float)(400 + Main.rand.Next(400)))
                {
                    npc.ai[0] = 0f;
                }
            }
            if (npc.type == NPCID.Demon || npc.type == NPCID.VoodooDemon)
            {
                npc.ai[0] += 1f;
                if (npc.ai[0] == 20f || npc.ai[0] == 40f || npc.ai[0] == 60f || npc.ai[0] == 80f)
                {
                    if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    {
                        float num710 = 0.2f;
                        Vector2 vector158 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                        float num712 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector158.X + (float)Main.rand.Next(-100, 101);
                        float num713 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector158.Y + (float)Main.rand.Next(-100, 101);
                        float num714 = (float)Math.Sqrt(num712 * num712 + num713 * num713);
                        num714 = num710 / num714;
                        num712 *= num714;
                        num713 *= num714;
                        int num715 = 21;
                        int num716 = 44;
                        int num717 = Projectile.NewProjectile(vector158.X, vector158.Y, num712, num713, num716, num715, 0f, Main.myPlayer);
                        Main.projectile[num717].timeLeft = 300;
                    }
                }
                else if (npc.ai[0] >= (float)(300 + Main.rand.Next(300)))
                {
                    npc.ai[0] = 0f;
                }
            }
            if (npc.type != NPCID.RedDevil)
            {
                return;
            }
            npc.ai[0] += 1f;
            if (npc.ai[0] == 20f || npc.ai[0] == 40f || npc.ai[0] == 60f || npc.ai[0] == 80f || npc.ai[0] == 100f)
            {
                if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    float num718 = 0.2f;
                    Vector2 vector159 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    float num719 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector159.X + (float)Main.rand.Next(-50, 51);
                    float num720 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector159.Y + (float)Main.rand.Next(-50, 51);
                    float num721 = (float)Math.Sqrt(num719 * num719 + num720 * num720);
                    num721 = num718 / num721;
                    num719 *= num721;
                    num720 *= num721;
                    int num723 = 80;
                    int num724 = 115;
                    vector159 += npc.velocity * 5f;
                    int num725 = Projectile.NewProjectile(vector159.X + num719 * 100f, vector159.Y + num720 * 100f, num719, num720, num724, num723, 0f, Main.myPlayer);
                    Main.projectile[num725].timeLeft = 300;
                }
            }
            else if (npc.ai[0] >= (float)(250 + Main.rand.Next(250)))
            {
                npc.ai[0] = 0f;
            }
        }

        protected virtual float BumpSpeedLoose => 0.5f;

        protected virtual float MaxSpeedX => 4f;
        protected virtual float SpeedX => 0.1f;
        protected virtual float SpeedYMax => 1.5f;
        protected virtual float SpeedY => 0.04f;

        protected virtual float WaterYDecrease => 0.95f;
        protected virtual float WaterBuyanocy => 0.5f;
        protected virtual float WaterYSpeedMax => 4f;

        protected virtual int BobbingStart => 200;
        protected virtual int BobbingEnd => 1000;
        protected virtual int SomethingRelatedToTimersAndBobbing => 150;
        protected virtual float num698 => 0.2f;
        protected virtual float num699 => 0.1f;
        protected virtual float num701 => 4f;
        protected virtual float num702 => 1.5f;

        protected virtual bool PreCheckCollisions()
        {
            npc.TargetClosest();
            return true;
        }

        protected virtual bool PreMoveX()
        {
            return true;
        }

        protected virtual bool PreMoveY()
        {
            return true;
        }

        protected virtual bool PreCheckWater()
        {
            return true;
        }

        protected virtual void InWater()
        {
            npc.TargetClosest();
        }

        protected virtual bool PreDoBobbing()
        {
            return true;
        }

        public override void AI()
        {
            if (PreCheckCollisions())
            {
                if (npc.collideX)
                {
                    npc.velocity.X = npc.oldVelocity.X * -BumpSpeedLoose;
                    if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                    {
                        npc.velocity.X = 2f;
                    }
                    if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                    {
                        npc.velocity.X = -2f;
                    }
                }
                if (npc.collideY)
                {
                    npc.velocity.Y = npc.oldVelocity.Y * -BumpSpeedLoose;
                    if (npc.velocity.Y > 0f && npc.velocity.Y < 1f)
                    {
                        npc.velocity.Y = 1f;
                    }
                    if (npc.velocity.Y < 0f && npc.velocity.Y > -1f)
                    {
                        npc.velocity.Y = -1f;
                    }
                }
            }

            if (PreMoveX())
            {
                if (npc.direction == -1 && npc.velocity.X > -MaxSpeedX)
                {
                    npc.velocity.X -= SpeedX;
                    if (npc.velocity.X > MaxSpeedX)
                    {
                        npc.velocity.X -= SpeedX;
                    }
                    else if (npc.velocity.X > 0f)
                    {
                        npc.velocity.X += SpeedX / 2f;
                    }
                    if (npc.velocity.X < -MaxSpeedX)
                    {
                        npc.velocity.X = -MaxSpeedX;
                    }
                }
                else if (npc.direction == 1 && npc.velocity.X < MaxSpeedX)
                {
                    npc.velocity.X += SpeedX;
                    if (npc.velocity.X < -MaxSpeedX)
                    {
                        npc.velocity.X += SpeedX;
                    }
                    else if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X -= SpeedX / 2f;
                    }
                    if (npc.velocity.X > MaxSpeedX)
                    {
                        npc.velocity.X = MaxSpeedX;
                    }
                }
            }
            if (PreMoveY())
            {
                if (npc.directionY == -1 && npc.velocity.Y > -SpeedYMax)
                {
                    npc.velocity.Y -= SpeedY;
                    if (npc.velocity.Y > SpeedYMax)
                    {
                        npc.velocity.Y -= SpeedY * 1.25f;
                    }
                    else if (npc.velocity.Y > 0f)
                    {
                        npc.velocity.Y += SpeedY * 0.75f;
                    }
                    if (npc.velocity.Y < -SpeedYMax)
                    {
                        npc.velocity.Y = -SpeedYMax;
                    }
                }
                else if (npc.directionY == 1 && npc.velocity.Y < SpeedYMax)
                {
                    npc.velocity.Y += SpeedY;
                    if (npc.velocity.Y < -SpeedYMax)
                    {
                        npc.velocity.Y += SpeedY * 1.25f;
                    }
                    else if (npc.velocity.Y < 0f)
                    {
                        npc.velocity.Y -= SpeedY * 0.75f;
                    }
                    if (npc.velocity.Y > SpeedYMax)
                    {
                        npc.velocity.Y = SpeedYMax;
                    }
                }
            }

            if (PreCheckWater() && npc.wet)
            {
                InWater();
                if (npc.velocity.Y > 0f)
                {
                    npc.velocity.Y *= WaterYDecrease;
                }
                npc.velocity.Y -= WaterBuyanocy;
                if (npc.velocity.Y < -WaterYSpeedMax)
                {
                    npc.velocity.Y = -WaterYSpeedMax;
                }
            }

            if (PreDoBobbing())
            {
                npc.ai[1]++;
                if ((int)npc.ai[1] > BobbingStart)
                {
                    if (!Main.player[npc.target].wet && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    {
                        npc.ai[1] = 0f;
                    }
                    if ((int)npc.ai[1] > BobbingEnd)
                    {
                        npc.ai[1] = 0f;
                    }
                    npc.ai[2]++;
                    if ((int)npc.ai[2] > 0)
                    {
                        if (npc.velocity.Y < num702)
                        {
                            npc.velocity.Y += num699;
                        }
                    }
                    else if (npc.velocity.Y > 0f - num702)
                    {
                        npc.velocity.Y -= num699;
                    }
                    if ((int)npc.ai[2] < -SomethingRelatedToTimersAndBobbing || (int)npc.ai[2] > SomethingRelatedToTimersAndBobbing)
                    {
                        if (npc.velocity.X < num701)
                        {
                            npc.velocity.X += num698;
                        }
                    }
                    else if (npc.velocity.X > 0f - num701)
                    {
                        npc.velocity.X -= num698;
                    }
                    if (npc.ai[2] > SomethingRelatedToTimersAndBobbing * 2)
                    {
                        npc.ai[2] = -SomethingRelatedToTimersAndBobbing * 2;
                    }
                }
            }
        }
    }
}