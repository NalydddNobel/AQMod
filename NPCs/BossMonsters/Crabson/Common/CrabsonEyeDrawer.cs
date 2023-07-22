using Aequus.NPCs.BossMonsters.Crabson.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;

namespace Aequus.NPCs.BossMonsters.Crabson.Segments;

public struct CrabsonEyeDrawer {
    public int frame;
    public int eyeJitter;

    public Vector2 pupilPositionOffset;

    public void Update(NPC npc, CrabsonMood mood) {
        eyeJitter++;

        // Get happy, idiot!
        if (mood.laughAnimation > 0) {
            frame = 1;
            pupilPositionOffset *= 0.5f;
            return;
        }
        // Get madder after 40 seconds without damaging the player
        if (mood.timeSinceDamagedPlayer > 2400) {
            frame = 3;
        }
        // Get mid after 20 seconds without damaging the player
        else if (mood.timeSinceDamagedPlayer > 1200) {
            frame = 2;
        }
        else {
            frame = 0;
        }

        if (!npc.HasPlayerTarget) {
            pupilPositionOffset *= 0.9f;
            return;
        }

        pupilPositionOffset = Vector2.Lerp(pupilPositionOffset, npc.DirectionTo(Main.player[npc.target].Center) * 3f, 0.05f);
    }

    public int GetFrame() {
        return frame * 2 + GetJitterFrame();
    }

    public int GetJitterFrame() {
        return eyeJitter / 3 % 2;
    }

    public void Draw(NPC npc, SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, Vector2 offset, Vector2 eyeOffset) {
        var eyePosition = drawPosition + eyeOffset;
        var eyeFrame = AequusTextures.Crabson_Eyes.Frame(verticalFrames: 8, frameY: GetFrame());
        var eyeOrigin = eyeFrame.Size() / 2f;
        spriteBatch.Draw(
            AequusTextures.Crabson_Eyes,
            eyePosition,
            eyeFrame,
            Color.White,
            npc.rotation,
            eyeOrigin,
            npc.scale, SpriteEffects.None, 0f);

        var pupilFrame = AequusTextures.Crabson_Pupil.Frame(verticalFrames: 2, frameY: GetJitterFrame());
        var pupilOrigin = pupilFrame.Size() / 2f;
        spriteBatch.Draw(
            AequusTextures.Crabson_Pupil,
            eyePosition + pupilPositionOffset,
            pupilFrame,
            Color.White,
            npc.rotation,
            pupilOrigin,
            npc.scale, SpriteEffects.None, 0f);

        var trailOffset = npc.Size / 2f;
        int trailLength = NPCID.Sets.TrailCacheLength[npc.type];
        var trailColor = Color.White with { A = 0 } * Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.25f, 0.4f);
        for (int i = 0; i < trailLength; i++) {

            float progress = Helper.CalcProgress(trailLength, i);
            var trailClr = trailColor * MathF.Pow(progress, 2f);
            var eyeTrailPosition = npc.oldPos[i] + trailOffset + eyeOffset - screenPos + offset;
            spriteBatch.Draw(
                AequusTextures.Crabson_Eyes,
                eyeTrailPosition,
                eyeFrame,
                trailClr,
                npc.rotation,
                eyeOrigin,
                npc.scale, SpriteEffects.None, 0f);

            spriteBatch.Draw(
                AequusTextures.Crabson_Pupil,
                eyeTrailPosition + pupilPositionOffset,
                pupilFrame,
                trailClr,
                npc.rotation,
                pupilOrigin,
                npc.scale, SpriteEffects.None, 0f);
        }
    }
}