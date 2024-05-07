using System;
using Terraria.GameContent;

namespace Aequus.Content.Enemies.PollutedOcean.Conductor;

public partial class Conductor {
    public const int FRAME_IDLE = 0;

    public const int FRAME_WALK_S = 1;
    public const int FRAME_WALK_E = 4;
    public const int FRAME_WALK_RATE = 6;
    public const int FRAME_WALK_FRAMES = FRAME_WALK_E - FRAME_WALK_S + 1;

    public const int FRAME_SLIDE_S = 5;
    public const int FRAME_SLIDE_E = 6;
    public const int FRAME_SLIDE_RATE = 3;
    public const int FRAME_SLIDE_FRAMES = FRAME_SLIDE_E - FRAME_SLIDE_S + 1;

    public const int FRAME_CONDUCT_S = 7;
    public const int FRAME_CONDUCT_E = 13;
    public const int FRAME_CONDUCT_RATE = 5;
    public const int FRAME_CONDUCT_FRAMES = FRAME_CONDUCT_E - FRAME_CONDUCT_S + 1;

    public const int FRAME_CONDUCT_ATTACK_START_TIME_OFFSET = -24;

    public const int FRAME_CONDUCT_ATTACK_S = 14;
    public const int FRAME_CONDUCT_ATTACK_E = 17;
    public const int FRAME_CONDUCT_ATTACK_RATE = 7;
    public const int FRAME_CONDUCT_ATTACK_END_RATE = 26;

    /// <summary><see cref="Main.npcFrameCount"/></summary>
    public const int FRAME_COUNT = 18;

    public override void HitEffect(NPC.HitInfo hit) {
        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        var source = NPC.GetSource_FromThis();

        if (NPC.life <= 0) {
            for (int i = 0; i < 20; i++) {
                Terraria.Dust.NewDust(NPC.position, NPC.width, NPC.height, Main.rand.NextBool(3) ? DustID.Iron : DustID.Bone, 2.5f * hit.HitDirection, -2.5f);
            }

            NPC.NewGore(AequusTextures.ConductorGoreHead, NPC.position, NPC.velocity, Scale: NPC.scale);
            for (int i = 0; i < 2; i++) {
                Gore.NewGore(source, NPC.position + new Vector2(0f, 20f), NPC.velocity, 43, NPC.scale);
                Gore.NewGore(source, NPC.position + new Vector2(0f, 34f), NPC.velocity, 44, NPC.scale);
            }
        }
        else {
            for (int i = 0; i < hit.Damage / (double)NPC.lifeMax * 50f; i++) {
                Terraria.Dust.NewDust(NPC.position, NPC.width, NPC.height, Main.rand.NextBool(3) ? DustID.Iron : DustID.Bone, hit.HitDirection, -1f);
            }
        }
    }

    public override void FindFrame(int frameHeight) {
        int frame = 0;
        switch (State) {
            case A_TARGETING: {
                    NPC.localAI[3] = 0f;
                    if (NPC.localAI[0] < 1f) {
                        frame = FRAME_IDLE;
                    }
                    else {
                        NPC.frameCounter += Math.Abs(NPC.velocity.X) * 0.8f;
                        double walkAnimation = FRAME_WALK_FRAMES * FRAME_WALK_RATE;
                        if (NPC.frameCounter > walkAnimation) {
                            NPC.frameCounter = 0;
                        }
                        frame = FRAME_WALK_S + (int)Math.Min(NPC.frameCounter / FRAME_WALK_RATE, FRAME_WALK_FRAMES - 1);
                    }
                }
                break;

            case A_ATTACKING: {
                    if (NPC.localAI[3] > 2f) {
                        break;
                    }

                    GetAttackTimings(out _, out _, out int attackTime);

                    frame = NPC.frame.Y / frameHeight;

                    //if (frame >= 12 && frame < 14) {
                    //    Main.NewText(NPC.ai[1] + " | " + attackTime);
                    //}
                    if (NPC.localAI[3] == 0f) {
                        NPC.frameCounter = 0.0;
                    }
                    if (NPC.ai[1] < attackTime - 12) {
                        NPC.localAI[3] = 1f;
                        NPC.frameCounter++;

                        double conductAnimation = FRAME_CONDUCT_RATE * (FRAME_CONDUCT_FRAMES - 1);

                        if (NPC.frameCounter > conductAnimation * 2) {
                            NPC.frameCounter = 0;
                        }
                        if (NPC.frameCounter >= conductAnimation) {
                            frame = 13 - (int)Math.Min((NPC.frameCounter - conductAnimation) / FRAME_CONDUCT_RATE, FRAME_CONDUCT_FRAMES - 1);
                        }
                        else {
                            frame = 7 + (int)Math.Min(NPC.frameCounter / FRAME_CONDUCT_RATE, FRAME_CONDUCT_FRAMES - 1);
                        }
                    }
                    else {
                        if (NPC.localAI[3] == 0f) {
                            NPC.localAI[3] = 2f;
                            NPC.frameCounter = 0.0;
                        }

                        frame = Math.Max(frame, FRAME_CONDUCT_ATTACK_S);

                        if (frame >= FRAME_CONDUCT_ATTACK_E) {
                            if (++NPC.frameCounter > FRAME_CONDUCT_ATTACK_END_RATE) {
                                frame = 0;
                                NPC.localAI[3] = 3f;
                            }
                        }
                        else if (++NPC.frameCounter > FRAME_CONDUCT_ATTACK_RATE) {
                            NPC.frameCounter = 0.0;
                            frame++;
                        }
                    }
                }
                break;

            case A_SLIDE_BACK: {
                    NPC.frameCounter++;
                    double slideAnimation = FRAME_SLIDE_RATE * FRAME_SLIDE_FRAMES;
                    if (NPC.frameCounter > slideAnimation) {
                        NPC.frameCounter = 0;
                    }
                    frame = FRAME_SLIDE_S + (int)Math.Min(NPC.frameCounter / FRAME_SLIDE_RATE, FRAME_SLIDE_FRAMES - 1);
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
        Vector2 drawCoordinates = NPC.Bottom + new Vector2(0f, origin.Y - frame.Height + 4f + NPC.gfxOffY) - screenPos;
        drawColor = NPC.GetAlpha(NPC.GetNPCColorTintedByBuffs(drawColor));
        SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        spriteBatch.Draw(texture, drawCoordinates, frame, drawColor, NPC.rotation, origin, NPC.scale, effects, 0f);

        switch (State) {
            case A_ATTACKING: {
                    if (frame.Y == 0) {
                        break;
                    }

                    const int FRAME_COUNT = 5;
                    const int FRAME_RATE = 6;
                    var waveTexture = AequusTextures.Conductor_Wave.Value;
                    Rectangle waveFrame = waveTexture.Frame(verticalFrames: FRAME_COUNT, frameY: (int)Main.GameUpdateCount / FRAME_RATE % FRAME_COUNT);
                    spriteBatch.Draw(waveTexture, drawCoordinates + new Vector2(9f * -NPC.spriteDirection, -1f), waveFrame, drawColor, NPC.rotation, origin, NPC.scale, effects, 0f);
                }
                break;
        }

        return false;
    }
}
