using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace Aequus.NPCs.BossMonsters.Crabson.Segments;

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
                Helper.GetColor(drawPos),
                (data.OldPosition - data.Position).ToRotation(),
                chainFrame.Size() / 2f,
                npc.scale, SpriteEffects.None, 0f);
        }
    }
}