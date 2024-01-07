using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using Terraria.Audio;

namespace Aequus.Content.Bosses.Crabson;

public partial class Crabson {
    public struct CrabsonArmsDrawer {
        public record struct ArmPoint(Vector2 Position, Vector2 OldPosition, float Progress);

        public List<ArmPoint> generatedPoints;

        public Vector2 beizerPoint;
        public float beizerPointTransition;

        private static Vector2 BezierCurve(Vector2[] bezierPoints, float bezierProgress) {
            if (bezierPoints.Length == 1) {
                return bezierPoints[0];
            }
            else {
                Vector2[] newBezierPoints = new Vector2[bezierPoints.Length - 1];
                for (int i = 0; i < bezierPoints.Length - 1; i++) {
                    newBezierPoints[i] = bezierPoints[i] * bezierProgress + bezierPoints[i + 1] * (1 - bezierProgress);
                }
                return BezierCurve(newBezierPoints, bezierProgress);
            }
        }
        private static Vector2 BezierCurveDerivative(Vector2[] bezierPoints, float bezierProgress) {
            if (bezierPoints.Length == 2) {
                return bezierPoints[0] - bezierPoints[1];
            }
            else {
                Vector2[] newBezierPoints = new Vector2[bezierPoints.Length - 1];
                for (int i = 0; i < bezierPoints.Length - 1; i++) {
                    newBezierPoints[i] = bezierPoints[i] * bezierProgress + bezierPoints[i + 1] * (1 - bezierProgress);
                }
                return BezierCurveDerivative(newBezierPoints, bezierProgress);
            }
        }

        public CrabsonArmsDrawer() {
            generatedPoints = new();
            beizerPoint = Vector2.Zero;
            beizerPointTransition = 0f;
        }

        public void Clear() {
            generatedPoints.Clear();
        }

        public void AddArm(NPC npc, Vector2 start, Vector2 end) {
            if (Main.netMode == NetmodeID.Server) {
                return;
            }

            Vector2[] bezierPoints = { end, new Vector2(start.X + Math.Sign(start.X - npc.Center.X) * 100f, (end.Y + start.Y) / 2f + 100f), start };

            // Code is taken from Star Construct, which is from the Polarities mod
            float bezierProgress = 0;
            float bezierIncrement = 18 * npc.scale;

            int loops = 0;
            while (bezierProgress < 1 && loops < 150) {
                int innerLoops = 0;
                loops++;
                var oldPos = BezierCurve(bezierPoints, bezierProgress);

                while ((oldPos - BezierCurve(bezierPoints, bezierProgress)).Length() < bezierIncrement && innerLoops < 1000) {
                    bezierProgress += 0.1f / BezierCurveDerivative(bezierPoints, bezierProgress).Length();
                    innerLoops++;
                }

                var newPos = BezierCurve(bezierPoints, bezierProgress);
                generatedPoints.Add(new ArmPoint(newPos, oldPos, bezierProgress));
            }
        }

        public void DrawArms(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos) {
            var chain = AequusTextures.Crabson_Chain.Value;
            foreach (var data in generatedPoints) {
                var drawPos = (data.OldPosition + data.Position) / 2f;
                var chainFrame = chain.Frame(verticalFrames: 3, frameY: data.Progress < 0.3f ? 2 : data.Progress < 0.6f ? 1 : 0);
                spriteBatch.Draw(
                    chain,
                    drawPos - screenPos,
                    chainFrame,
                    LightHelper.GetLightColor(drawPos),
                    (data.OldPosition - data.Position).ToRotation(),
                    chainFrame.Size() / 2f,
                    npc.scale, SpriteEffects.None, 0f);
            }
        }
    }

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

    private CrabsonArmsDrawer _arms = new();
    private CrabsonEyeDrawer _eyes = new();
    private CrabsonLegsDrawer _legs = new();
    private readonly CrabsonMood _mood = new();

    private void UpdateMood() {
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
        NPC leftArm = Main.npc[LeftArm];
        NPC rightArm = Main.npc[RightArm];
        _arms.Clear();
        _arms.AddArm(NPC, NPC.Center + chainOffset with { X = -chainOffset.X } + NPC.netOffset, leftArm.Center + chainEndOffset.RotatedBy(leftArm.rotation == 0f ? MathHelper.Pi : leftArm.rotation) + leftArm.netOffset);
        _arms.AddArm(NPC, NPC.Center + chainOffset + NPC.netOffset, rightArm.Center + chainEndOffset.RotatedBy(rightArm.rotation) + rightArm.netOffset);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        return base.PreDraw(spriteBatch, screenPos, drawColor);
    }
}