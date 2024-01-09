using Aequus.Core.PhysicsObjects;
using ReLogic.Utilities;
using System;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Animations;

namespace Aequus.Content.Bosses.Crabson;

public partial class Crabson {
    //public struct CrabsonArmsDrawer {
    //    public record struct ArmPoint(Vector2 Position, Vector2 OldPosition, float Progress);

    //    public List<ArmPoint> generatedPoints;

    //    public Vector2 beizerPoint;
    //    public float beizerPointTransition;

    //    private static Vector2 BezierCurve(Vector2[] bezierPoints, float bezierProgress) {
    //        if (bezierPoints.Length == 1) {
    //            return bezierPoints[0];
    //        }
    //        else {
    //            Vector2[] newBezierPoints = new Vector2[bezierPoints.Length - 1];
    //            for (int i = 0; i < bezierPoints.Length - 1; i++) {
    //                newBezierPoints[i] = bezierPoints[i] * bezierProgress + bezierPoints[i + 1] * (1 - bezierProgress);
    //            }
    //            return BezierCurve(newBezierPoints, bezierProgress);
    //        }
    //    }
    //    private static Vector2 BezierCurveDerivative(Vector2[] bezierPoints, float bezierProgress) {
    //        if (bezierPoints.Length == 2) {
    //            return bezierPoints[0] - bezierPoints[1];
    //        }
    //        else {
    //            Vector2[] newBezierPoints = new Vector2[bezierPoints.Length - 1];
    //            for (int i = 0; i < bezierPoints.Length - 1; i++) {
    //                newBezierPoints[i] = bezierPoints[i] * bezierProgress + bezierPoints[i + 1] * (1 - bezierProgress);
    //            }
    //            return BezierCurveDerivative(newBezierPoints, bezierProgress);
    //        }
    //    }

    //    public CrabsonArmsDrawer() {
    //        generatedPoints = new();
    //        beizerPoint = Vector2.Zero;
    //        beizerPointTransition = 0f;
    //    }

    //    public void Clear() {
    //        generatedPoints.Clear();
    //    }

    //    public void AddArm(NPC npc, Vector2 start, Vector2 end) {
    //        if (Main.netMode == NetmodeID.Server) {
    //            return;
    //        }

    //        Vector2[] bezierPoints = { end, new Vector2(start.X + Math.Sign(start.X - npc.Center.X) * 100f, (end.Y + start.Y) / 2f + 100f), start };

    //        // Code is taken from Star Construct, which is from the Polarities mod
    //        float bezierProgress = 0;
    //        float bezierIncrement = 18 * npc.scale;

    //        int loops = 0;
    //        while (bezierProgress < 1 && loops < 150) {
    //            int innerLoops = 0;
    //            loops++;
    //            var oldPos = BezierCurve(bezierPoints, bezierProgress);

    //            while ((oldPos - BezierCurve(bezierPoints, bezierProgress)).Length() < bezierIncrement && innerLoops < 1000) {
    //                bezierProgress += 0.1f / BezierCurveDerivative(bezierPoints, bezierProgress).Length();
    //                innerLoops++;
    //            }

    //            var newPos = BezierCurve(bezierPoints, bezierProgress);
    //            generatedPoints.Add(new ArmPoint(newPos, oldPos, bezierProgress));
    //        }
    //    }

    //    public void DrawArms(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos) {
    //        var chain = AequusTextures.Crabson_Chain.Value;
    //        foreach (var data in generatedPoints) {
    //            var drawPos = (data.OldPosition + data.Position) / 2f;
    //            var chainFrame = chain.Frame(verticalFrames: 3, frameY: data.Progress < 0.3f ? 2 : data.Progress < 0.6f ? 1 : 0);
    //            spriteBatch.Draw(
    //                chain,
    //                drawPos - screenPos,
    //                chainFrame,
    //                LightHelper.GetLightColor(drawPos),
    //                (data.OldPosition - data.Position).ToRotation(),
    //                chainFrame.Size() / 2f,
    //                npc.scale, SpriteEffects.None, 0f);
    //        }
    //    }
    //}

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
            var eyeFrame = AequusTextures.Crabson_Eyes.Frame(horizontalFrames: 2, verticalFrames: 8, frameY: GetFrame());
            var eyeOrigin = eyeFrame.Size() / 2f;
            float eyeOffsetX = 33f;
            spriteBatch.Draw(
                AequusTextures.Crabson_Eyes,
                eyePosition + new Vector2(-eyeOffsetX, 0f),
                eyeFrame,
                Color.White,
                npc.rotation,
                eyeOrigin,
                npc.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(
                AequusTextures.Crabson_Eyes,
                eyePosition + new Vector2(eyeOffsetX, 0f),
                eyeFrame with { X = eyeFrame.Width },
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

            //var trailOffset = npc.Size / 2f;
            //int trailLength = NPCID.Sets.TrailCacheLength[npc.type];
            //var trailColor = Color.White with { A = 0 } * Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.25f, 0.4f);
            //for (int i = 0; i < trailLength; i++) {

            //    float progress = Helper.CalcProgress(trailLength, i);
            //    var trailClr = trailColor * MathF.Pow(progress, 2f);
            //    var eyeTrailPosition = npc.oldPos[i] + trailOffset + eyeOffset - screenPos + offset;
            //    spriteBatch.Draw(
            //        AequusTextures.Crabson_Eyes,
            //        eyeTrailPosition,
            //        eyeFrame,
            //        trailClr,
            //        npc.rotation,
            //        eyeOrigin,
            //        npc.scale, SpriteEffects.None, 0f);

            //    spriteBatch.Draw(
            //        AequusTextures.Crabson_Pupil,
            //        eyeTrailPosition + pupilPositionOffset,
            //        pupilFrame,
            //        trailClr,
            //        npc.rotation,
            //        pupilOrigin,
            //        npc.scale, SpriteEffects.None, 0f);
            //}
        }
    }

    public struct CrabsonLegsDrawer {
        public const int MaxFrames = 8;

        public bool _floatingWalk;
        public bool _soundsEnabled;

        public int frame;
        public float frameCounter;
        public int soundDelay;
        public SlotId walkingSoundSlot;

        private void UpdateSounds(NPC npc, float magnitudeX, bool aboveAir) {
            if (magnitudeX > 0.5f && !aboveAir) {
                if (!walkingSoundSlot.IsValid || soundDelay <= 0) {
                    walkingSoundSlot = SoundEngine.PlaySound(AequusSounds.CrabsonWalk with { Volume = 0.5f }, npc.Bottom + new Vector2(0f, -12f));
                    soundDelay = 70;
                }
            }
            else if ((aboveAir || magnitudeX < 0.2f) && walkingSoundSlot.IsValid && SoundEngine.TryGetActiveSound(walkingSoundSlot, out var sound)) {
                sound.Stop();
            }
        }

        public void Update(NPC npc) {
            float magnitudeX = Math.Abs(npc.velocity.X);
            if (soundDelay > 0) {
                soundDelay--;
            }

            bool aboveAir = _floatingWalk ? false : Math.Abs(npc.velocity.Y) > 0.001f;
            if (aboveAir) {
                frame = 7;
                frameCounter = 0f;
            }
            else {
                frameCounter += magnitudeX;
                if (frameCounter > 10f) {
                    frame = (frame + 1) % 7;
                    frameCounter = 0f;
                }

                if (magnitudeX < 0.1f) {
                    frame = 0;
                }
            }

            if (!npc.IsABestiaryIconDummy && Main.netMode != NetmodeID.Server) {
                if (_soundsEnabled) {
                    UpdateSounds(npc, magnitudeX, aboveAir);
                }
            }
        }

        public void Draw(NPC npc, SpriteBatch spriteBatch, Vector2 drawPosition, Color bodyDrawColor) {
            var legFrame = AequusTextures.Crabson_Legs.Frame(verticalFrames: MaxFrames, frameY: frame);
            spriteBatch.Draw(
                AequusTextures.Crabson_Legs,
                drawPosition,
                legFrame,
                bodyDrawColor,
                npc.rotation,
                legFrame.Size() / 2f,
                npc.scale, npc.velocity.X < 0f ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        }
    }

    public class CrabsonMood {
        public int laughAnimation;
        public int timeSinceDamagedPlayer;
        public int damageRecievedSinceDamagedPlayer;

        public void OnAIUpdate() {
            timeSinceDamagedPlayer++;
        }

        public void Laugh() {
            laughAnimation = 60;
            timeSinceDamagedPlayer = 0;
            damageRecievedSinceDamagedPlayer = 0;
        }

        public void OnDamageRecieved(NPC.HitInfo hit) {
            damageRecievedSinceDamagedPlayer += hit.Damage;
        }
    }

    private CrabsonEyeDrawer _eyes = new();
    private CrabsonLegsDrawer _legs = new();
    private CrabsonMood _mood;

    private void UpdateDrawEffects() {
        LeftArm.UpdateRope(NPC);
        RightArm.UpdateRope(NPC);
        _mood.OnAIUpdate();
    }

    public override void FindFrame(int frameHeight) {
        if (_mood.laughAnimation > 0) {
            NPC.frame.Y = frameHeight * (2 + _mood.laughAnimation / 6 % 2);
            _mood.laughAnimation--;
        }
        else {
            NPC.frame.Y = 0;
        }
        _eyes.Update(NPC, _mood);
        _legs.Update(NPC);
        if (Main.netMode == NetmodeID.Server || NPC.IsABestiaryIconDummy) {
            return;
        }

        Vector2 chainOffset = new(44f, -14f);
        Vector2 chainEndOffset = new(20f, 0f);
        //_arms.Clear();
        //_arms.AddArm(NPC, NPC.Center + chainOffset with { X = -chainOffset.X } + NPC.netOffset, LeftArm.Center + chainEndOffset.RotatedBy(LeftArm.Rotation == 0f ? MathHelper.Pi : LeftArm.Rotation));
        //_arms.AddArm(NPC, NPC.Center + chainOffset + NPC.netOffset, RightArm.Center + chainEndOffset.RotatedBy(RightArm.Rotation));
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        if (NPC.IsABestiaryIconDummy) {
            DrawBestiary(spriteBatch, screenPos, drawColor);
            return false;
        }

        //_arms.DrawArms(NPC, spriteBatch, screenPos);
        DrawArm(spriteBatch, screenPos, LeftArm);
        DrawArm(spriteBatch, screenPos, RightArm);
        DrawBody(spriteBatch, screenPos, new Vector2(), NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)));
        DrawClaw(LeftArm, spriteBatch, screenPos, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(LightHelper.GetLightColor(LeftArm.Center))));
        DrawClaw(RightArm, spriteBatch, screenPos, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(LightHelper.GetLightColor(RightArm.Center))));
        return false;
    }

    private void DrawArm(SpriteBatch spriteBatch, Vector2 screenPosition, Arm arm) {
        Texture2D texture = AequusTextures.Crabson_Chain.Value;
        for (int i = 1; i < arm.Rope.segments.Count; i++) {
            RopeChain.RopeSegment segment = arm.Rope.segments[i];
            Rectangle frame = texture.Frame(verticalFrames: 3, frameY: 0);
            float rotation = (arm.Rope.segments[i - 1].position - segment.position).ToRotation();
            spriteBatch.Draw(texture, segment.position - screenPosition, frame, LightHelper.GetLightColor(segment.position), rotation, frame.Size() / 2f, 1f, SpriteEffects.None, 0f);
        }
    }

    private void DrawBestiary(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        Vector2 offset = new Vector2(120f, 58f) * NPC.scale;
        NPC.direction = 1;
        NPC.spriteDirection = 1;
        DrawClaw(LeftArm, spriteBatch, screenPos + (offset with { X = -offset.X }), Color.White);
        NPC.direction = -1;
        NPC.spriteDirection = -1;
        DrawClaw(RightArm, spriteBatch, screenPos + offset, Color.White);

        DrawBody(
            spriteBatch,
            screenPos,
            new(0f, 0f),
            Color.White
        );
    }

    private void DrawBody(SpriteBatch spriteBatch, Vector2 screenPos, Vector2 offset, Color bodyDrawColor) {
        offset.Y -= 24;
        var drawPosition = NPC.Center - screenPos + offset;
        spriteBatch.Draw(
            TextureAssets.Npc[NPC.type].Value,
            drawPosition,
            NPC.frame,
            bodyDrawColor,
            NPC.rotation,
            NPC.frame.Size() / 2f,
            NPC.scale, SpriteEffects.None, 0f);

        _legs.Draw(NPC, spriteBatch, drawPosition, bodyDrawColor);
        _eyes.Draw(NPC, spriteBatch, drawPosition, screenPos, offset, new Vector2(0f, -40f));
    }

    protected void DrawClaw(Arm arm, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        var claw = AequusTextures.CrabsonClaw.Value;
        var origin = new Vector2(claw.Width / 2f + 20f, claw.Height / 8f);
        var drawCoords = arm.Center + new Vector2(arm.Direction * 10f, -20f) - screenPos;
        if (NPC.ModNPC != null) {
            drawCoords.Y += NPC.ModNPC.DrawOffsetY;
        }
        SpriteEffects spriteEffects;
        bool flip;
        if (arm.Rotation == 0f) {
            spriteEffects = arm.Direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            flip = arm.Direction == 1;
            if (!flip) {
                origin.X = claw.Width - origin.X;
            }
        }
        else {
            spriteEffects = Math.Abs(arm.Rotation) > MathHelper.PiOver2 ? SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally : SpriteEffects.FlipHorizontally;
            flip = spriteEffects.HasFlag(SpriteEffects.FlipVertically);
        }

        float rotation = (arm.Rope.segments[^2].position - arm.Rope.segments[^1].position).ToRotation();
        DrawClawManual(spriteBatch, claw, drawCoords, drawColor, origin, arm.Rotation + rotation, flip ? -arm.MouthOpen : arm.MouthOpen, NPC.scale, spriteEffects);
    }

    protected void DrawClawManual(SpriteBatch spriteBatch, Texture2D claw, Vector2 drawCoords, Color drawColor, Vector2 origin, float rotation, float mouthAnimation, float scale, SpriteEffects spriteEffects) {
        int frameHeight = claw.Height / 4;
        var clawFrame = new Rectangle(0, frameHeight, claw.Width, frameHeight - 2);
        spriteBatch.Draw(claw, drawCoords, clawFrame, drawColor, -mouthAnimation + 0.1f * NPC.direction + rotation, origin, scale, spriteEffects, 0f);
        spriteBatch.Draw(claw, drawCoords, clawFrame with { Y = 0, }, drawColor, mouthAnimation + 0.1f * NPC.direction + rotation, origin, scale, spriteEffects, 0f);
        spriteBatch.Draw(claw, drawCoords, clawFrame with { Y = frameHeight * (Math.Abs(mouthAnimation) > 0.05f ? 3 : 2), }, drawColor, 0.1f * NPC.direction + rotation, origin, scale, spriteEffects, 0f);
    }
}