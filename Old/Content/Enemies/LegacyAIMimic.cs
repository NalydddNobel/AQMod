using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Old.Content.Enemies;

public abstract class LegacyAIMimic : ModNPC {
    protected virtual int JumpTimer => NPC.ai[1] == 0f ? 12 : 20;

    protected virtual void Jump() {
        if (NPC.ai[1] == 2f) {
            NPC.velocity.X = NPC.direction * 2.5f;
            NPC.velocity.Y = -8f;
            NPC.ai[1] = 0f;
        }
        else {
            NPC.velocity.X = NPC.direction * 3.5f;
            NPC.velocity.Y = -4f;
        }
    }

    public virtual void OrientJumpingDirection() {
        if (NPC.direction == 1 && NPC.velocity.X < 1f) {
            NPC.velocity.X += 0.1f;
        }
        else if (NPC.direction == -1 && NPC.velocity.X > -1f) {
            NPC.velocity.X -= 0.1f;
        }
    }

    public override void AI() {
        if (NPC.ai[0] <= 0f) {
            NPC.ShowNameOnHover = false;
            NPC.TargetClosest();
            var target = Main.player[NPC.target];
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            if (NPC.velocity.X != 0f || NPC.velocity.Y < 0f || NPC.velocity.Y > 0.3) {
                NPC.ai[0]++;
                NPC.netUpdate = true;
                return;
            }
            var rectangle3 = new Rectangle((int)target.position.X, (int)target.position.Y, target.width, target.height);
            if (new Rectangle((int)NPC.position.X - 100, (int)NPC.position.Y - 100, NPC.width + 200, NPC.height + 200).Intersects(rectangle3) || NPC.life < NPC.lifeMax) {
                NPC.ai[0] = 1f;
                NPC.netUpdate = true;
            }
            return;
        }
        NPC.ShowNameOnHover = true;
        if (NPC.velocity.Y == 0f) {
            NPC.ai[2] += 1f;
            int timer = 20;
            if (NPC.ai[1] == 0f)
                timer = 12;
            if (NPC.ai[2] < timer) {
                NPC.velocity.X *= 0.9f;
                return;
            }
            NPC.ai[2] = 0f;
            NPC.TargetClosest();
            if (NPC.direction == 0)
                NPC.direction = -1;
            NPC.spriteDirection = NPC.direction;
            NPC.ai[1] += 1f;
            Jump();
            NPC.netUpdate = true;
        }
        else {
            OrientJumpingDirection();
        }
    }

    public override void FindFrame(int frameHeight) {
        if (NPC.ai[0] <= 0f) {
            NPC.frameCounter = 0.0;
            NPC.frame.Y = 0;
        }
        else {
            int frameCount = 3;
            if (NPC.velocity.Y == 0f) {
                NPC.frameCounter -= 1.0;
            }
            else {
                NPC.frameCounter += 1.0;
            }
            if (NPC.frameCounter < 0.0)
                NPC.frameCounter = 0.0;
            if (NPC.frameCounter > frameCount * 4)
                NPC.frameCounter = frameCount * 4;
            if (NPC.frameCounter < frameCount) {
                NPC.frame.Y = frameHeight;
            }
            else if (NPC.frameCounter < frameCount * 2) {
                NPC.frame.Y = frameHeight * 2;
            }
            else if (NPC.frameCounter < frameCount * 3) {
                NPC.frame.Y = frameHeight * 3;
            }
            else if (NPC.frameCounter < frameCount * 4) {
                NPC.frame.Y = frameHeight * 4;
            }
            else if (NPC.frameCounter < frameCount * 5) {
                NPC.frame.Y = frameHeight * 5;
            }
            else if (NPC.frameCounter < frameCount * 6) {
                NPC.frame.Y = frameHeight * 4;
            }
            else if (NPC.frameCounter < frameCount * 7) {
                NPC.frame.Y = frameHeight * 3;
            }
            else {
                NPC.frame.Y = frameHeight * 2;
                if (NPC.frameCounter >= frameCount * 8)
                    NPC.frameCounter = frameCount;
            }
        }
    }
}