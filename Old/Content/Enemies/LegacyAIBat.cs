namespace Aequus.Old.Content.Enemies;

public abstract class LegacyAIBat : ModNPC {
    protected virtual System.Single BumpSpeedLoose => 0.5f;

    protected virtual System.Single MaxSpeedX => 4f;
    protected virtual System.Single SpeedX => 0.1f;
    protected virtual System.Single SpeedYMax => 1.5f;
    protected virtual System.Single SpeedY => 0.04f;

    protected virtual System.Single WaterYDecrease => 0.95f;
    protected virtual System.Single WaterBuyanocy => 0.5f;
    protected virtual System.Single WaterYSpeedMax => 4f;

    protected virtual System.Int32 BobbingStart => 200;
    protected virtual System.Int32 BobbingEnd => 1000;
    protected virtual System.Int32 SomethingRelatedToTimersAndBobbing => 150;
    protected virtual System.Single num698 => 0.2f;
    protected virtual System.Single num699 => 0.1f;
    protected virtual System.Single num701 => 4f;
    protected virtual System.Single num702 => 1.5f;

    protected virtual System.Boolean PreCheckCollisions() {
        NPC.TargetClosest();
        return true;
    }

    protected virtual System.Boolean PreMoveX() {
        return true;
    }

    protected virtual System.Boolean PreMoveY() {
        return true;
    }

    protected virtual System.Boolean ShouldApplyWaterEffects() {
        return true;
    }

    protected virtual void InWater() {
        NPC.TargetClosest();
    }

    protected virtual System.Boolean PreDoBobbing() {
        return true;
    }

    public override void AI() {
        if (PreCheckCollisions()) {
            if (NPC.collideX) {
                NPC.velocity.X = NPC.oldVelocity.X * -BumpSpeedLoose;
                if (NPC.direction == -1 && NPC.velocity.X > 0f && NPC.velocity.X < 2f)
                    NPC.velocity.X = 2f;
                if (NPC.direction == 1 && NPC.velocity.X < 0f && NPC.velocity.X > -2f)
                    NPC.velocity.X = -2f;
            }
            if (NPC.collideY) {
                NPC.velocity.Y = NPC.oldVelocity.Y * -BumpSpeedLoose;
                if (NPC.velocity.Y > 0f && NPC.velocity.Y < 1f)
                    NPC.velocity.Y = 1f;
                if (NPC.velocity.Y < 0f && NPC.velocity.Y > -1f)
                    NPC.velocity.Y = -1f;
            }
        }

        if (PreMoveX()) {
            if (NPC.direction == -1 && NPC.velocity.X > -MaxSpeedX) {
                NPC.velocity.X -= SpeedX;
                if (NPC.velocity.X > MaxSpeedX) {
                    NPC.velocity.X -= SpeedX;
                }
                else if (NPC.velocity.X > 0f) {
                    NPC.velocity.X += SpeedX / 2f;
                }
                if (NPC.velocity.X < -MaxSpeedX)
                    NPC.velocity.X = -MaxSpeedX;
            }
            else if (NPC.direction == 1 && NPC.velocity.X < MaxSpeedX) {
                NPC.velocity.X += SpeedX;
                if (NPC.velocity.X < -MaxSpeedX) {
                    NPC.velocity.X += SpeedX;
                }
                else if (NPC.velocity.X < 0f) {
                    NPC.velocity.X -= SpeedX / 2f;
                }
                if (NPC.velocity.X > MaxSpeedX)
                    NPC.velocity.X = MaxSpeedX;
            }
        }
        if (PreMoveY()) {
            if (NPC.directionY == -1 && NPC.velocity.Y > -SpeedYMax) {
                NPC.velocity.Y -= SpeedY;
                if (NPC.velocity.Y > SpeedYMax) {
                    NPC.velocity.Y -= SpeedY * 1.25f;
                }
                else if (NPC.velocity.Y > 0f) {
                    NPC.velocity.Y += SpeedY * 0.75f;
                }
                if (NPC.velocity.Y < -SpeedYMax)
                    NPC.velocity.Y = -SpeedYMax;
            }
            else if (NPC.directionY == 1 && NPC.velocity.Y < SpeedYMax) {
                NPC.velocity.Y += SpeedY;
                if (NPC.velocity.Y < -SpeedYMax) {
                    NPC.velocity.Y += SpeedY * 1.25f;
                }
                else if (NPC.velocity.Y < 0f) {
                    NPC.velocity.Y -= SpeedY * 0.75f;
                }
                if (NPC.velocity.Y > SpeedYMax)
                    NPC.velocity.Y = SpeedYMax;
            }
        }

        if (ShouldApplyWaterEffects() && NPC.wet) {
            InWater();
            if (NPC.velocity.Y > 0f)
                NPC.velocity.Y *= WaterYDecrease;
            NPC.velocity.Y -= WaterBuyanocy;
            if (NPC.velocity.Y < -WaterYSpeedMax)
                NPC.velocity.Y = -WaterYSpeedMax;
        }

        if (PreDoBobbing()) {
            NPC.ai[1]++;
            if ((System.Int32)NPC.ai[1] > BobbingStart) {
                if (!Main.player[NPC.target].wet && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                    NPC.ai[1] = 0f;
                if ((System.Int32)NPC.ai[1] > BobbingEnd)
                    NPC.ai[1] = 0f;
                NPC.ai[2]++;
                if ((System.Int32)NPC.ai[2] > 0) {
                    if (NPC.velocity.Y < num702)
                        NPC.velocity.Y += num699;
                }
                else if (NPC.velocity.Y > 0f - num702) {
                    NPC.velocity.Y -= num699;
                }
                if ((System.Int32)NPC.ai[2] < -SomethingRelatedToTimersAndBobbing || (System.Int32)NPC.ai[2] > SomethingRelatedToTimersAndBobbing) {
                    if (NPC.velocity.X < num701)
                        NPC.velocity.X += num698;
                }
                else if (NPC.velocity.X > 0f - num701) {
                    NPC.velocity.X -= num698;
                }
                if (NPC.ai[2] > SomethingRelatedToTimersAndBobbing * 2)
                    NPC.ai[2] = -SomethingRelatedToTimersAndBobbing * 2;
            }
        }
    }
}