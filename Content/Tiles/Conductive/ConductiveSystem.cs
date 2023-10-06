using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Tiles.Conductive;
public class ConductiveSystem : ModSystem {
    public class ActivationEffect {
        public Vector2 tileDrawOffset;
        public float intensity;

        public void Update() {
            intensity -= 0.033f;
            tileDrawOffset = Main.rand.NextVector2Square(-intensity, intensity) * 4f;
        }
    }

    public static readonly Dictionary<Point, ActivationEffect> ActivationPoints = new();
    private static readonly List<Point> RemoveActivationPoints = new();

    public override void ClearWorld() {
        ActivationPoints.Clear();
        RemoveActivationPoints.Clear();
    }

    public override void PreUpdateEntities() {
        foreach (var p in ActivationPoints) {
            p.Value.Update();
            if (p.Value.intensity <= 0f) {
                RemoveActivationPoints.Add(p.Key);
            }
        }
        foreach (var p in RemoveActivationPoints) {
            ActivationPoints.Remove(p);
        }
        RemoveActivationPoints.Clear();
    }
}