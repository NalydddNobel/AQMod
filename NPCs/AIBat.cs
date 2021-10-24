using Terraria;
using Terraria.ModLoader;

namespace AQMod.NPCs
{
    public abstract class AIBat : ModNPC
    {
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
                        npc.velocity.X = 2f;
                    if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                        npc.velocity.X = -2f;
                }
                if (npc.collideY)
                {
                    npc.velocity.Y = npc.oldVelocity.Y * -BumpSpeedLoose;
                    if (npc.velocity.Y > 0f && npc.velocity.Y < 1f)
                        npc.velocity.Y = 1f;
                    if (npc.velocity.Y < 0f && npc.velocity.Y > -1f)
                        npc.velocity.Y = -1f;
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
                        npc.velocity.X = -MaxSpeedX;
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
                        npc.velocity.X = MaxSpeedX;
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
                        npc.velocity.Y = -SpeedYMax;
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
                        npc.velocity.Y = SpeedYMax;
                }
            }

            if (PreCheckWater() && npc.wet)
            {
                InWater();
                if (npc.velocity.Y > 0f)
                    npc.velocity.Y *= WaterYDecrease;
                npc.velocity.Y -= WaterBuyanocy;
                if (npc.velocity.Y < -WaterYSpeedMax)
                    npc.velocity.Y = -WaterYSpeedMax;
            }

            if (PreDoBobbing())
            {
                npc.ai[1]++;
                if ((int)npc.ai[1] > BobbingStart)
                {
                    if (!Main.player[npc.target].wet && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                        npc.ai[1] = 0f;
                    if ((int)npc.ai[1] > BobbingEnd)
                        npc.ai[1] = 0f;
                    npc.ai[2]++;
                    if ((int)npc.ai[2] > 0)
                    {
                        if (npc.velocity.Y < num702)
                            npc.velocity.Y += num699;
                    }
                    else if (npc.velocity.Y > 0f - num702)
                    {
                        npc.velocity.Y -= num699;
                    }
                    if ((int)npc.ai[2] < -SomethingRelatedToTimersAndBobbing || (int)npc.ai[2] > SomethingRelatedToTimersAndBobbing)
                    {
                        if (npc.velocity.X < num701)
                            npc.velocity.X += num698;
                    }
                    else if (npc.velocity.X > 0f - num701)
                    {
                        npc.velocity.X -= num698;
                    }
                    if (npc.ai[2] > SomethingRelatedToTimersAndBobbing * 2)
                        npc.ai[2] = -SomethingRelatedToTimersAndBobbing * 2;
                }
            }
        }
    }
}