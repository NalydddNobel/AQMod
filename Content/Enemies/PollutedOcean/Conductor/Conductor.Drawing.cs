using Aequus.Core.Initialization;
using System;
using Terraria.GameContent;

namespace Aequus.Content.Enemies.PollutedOcean.Conductor;

public partial class Conductor : IPostPopulateItemDropDatabase {
    public override void FindFrame(int frameHeight) {
        int frame = 0;
        switch (State) {
            case A_TARGETING: {
                    if (NPC.localAI[0] < 1f) {
                        frame = 0;
                    }
                    else {
                        NPC.frameCounter += Math.Abs(NPC.velocity.X) * 0.8f;
                        if (NPC.frameCounter > 24.0) {
                            NPC.frameCounter = 0;
                        }
                        frame = 1 + (int)Math.Min(NPC.frameCounter / 6, 3.0);
                    }
                }
                break;

            case A_ATTACKING: {
                    NPC.frameCounter++;
                    if (NPC.frameCounter > 60.0) {
                        NPC.frameCounter = 0;
                    }
                    if (NPC.frameCounter >= 30.0) {
                        frame = 13 - (int)Math.Min((NPC.frameCounter - 30.0) / 5, 6.0);
                    }
                    else {
                        frame = 7 + (int)Math.Min(NPC.frameCounter / 5, 6.0);
                    }
                }
                break;

            case A_SLIDE_BACK: {
                    NPC.frameCounter++;
                    if (NPC.frameCounter > 6.0) {
                        NPC.frameCounter = 0;
                    }
                    frame = 5 + (int)Math.Min(NPC.frameCounter / 3, 1.0);
                }
                break;

            case A_TELEPORT: {
                }
                break;
        }

        NPC.frame.Y = frame * frameHeight;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        Texture2D texture = TextureAssets.Npc[Type].Value;
        Rectangle frame = NPC.frame;
        Vector2 origin = frame.Size() / 2f;
        origin.X -= 14f * NPC.spriteDirection;
        Vector2 drawCoordinates = NPC.Bottom + new Vector2(0f, origin.Y - frame.Height + 4f + NPC.gfxOffY);
        drawColor = NPC.GetAlpha(NPC.GetNPCColorTintedByBuffs(drawColor));
        SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        spriteBatch.Draw(texture, drawCoordinates - screenPos, frame, drawColor, NPC.rotation, origin, NPC.scale, effects, 0f);
        return false;
    }
}
