using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Aequus.Content.Tiles.Conductive;
public class ConductiveSystem : ModSystem {
    public class ActivationEffect {
        public float electricAnimation;
        public float intensity;
        public int timeActive;

        public void Update() {
            electricAnimation = MathF.Sin(timeActive / (float)WireMax / 4f * MathHelper.TwoPi);
            timeActive++;
        }

        public static float GetDistance(int i, int j) {
            int differenceX = Math.Abs(PoweredLocation.X - i);
            int differenceY = Math.Abs(PoweredLocation.Y - j);
            return Math.Max(differenceX, differenceY);
        }

        public static void Activate(int i, int j, float distance) {
            var point = new Point(i, j);
            float intensity = 1f - Math.Abs(distance / WireMax);
            if (ActivationPoints.TryGetValue(point, out var effect)) {
                effect.timeActive = Math.Max(effect.timeActive, (int)distance);
                effect.intensity = Math.Max(effect.intensity, intensity);
            }
            else {
                ActivationPoints[point] = new() { timeActive = (int)distance, intensity = intensity };
            }
        }
    }

    public static Point PoweredLocation;
    public static int WireMax = 8;
    public static int ActivationDelay = 60;

    public static readonly Dictionary<Point, ActivationEffect> ActivationPoints = new();
    public static Vector2[] ElectricOffsets = new Vector2[7];
    private static readonly List<Point> RemoveActivationPoints = new();

    public override void ClearWorld() {
        ActivationPoints.Clear();
        RemoveActivationPoints.Clear();
    }

    public override void PreUpdateEntities() {
        if (ActivationPoints.Count < 0) {
            return;
        }

        PoweredLocation = Point.Zero;
        for (int i = 0; i < ElectricOffsets.Length; i++) {
            ElectricOffsets[i] = Main.rand.NextVector2Square(-i, i);
        }
        foreach (var p in ActivationPoints) {
            p.Value.Update();
            if (p.Value.timeActive > WireMax * 4) {
                RemoveActivationPoints.Add(p.Key);
            }
        }
        foreach (var p in RemoveActivationPoints) {
            ActivationPoints.Remove(p);
        }
        RemoveActivationPoints.Clear();
    }
}