using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus.Items.Equipment.Accessories.Combat.Passive.CelesteTorus;

public record struct CelesteTorusDrawData(Player Player, Vector3 Rotation, Vector2 WorldPosition, float Radius, float Scale, int OrbsCount) {
    public Vector3 GetVector3(int index) {
        return CelesteTorusProj.GetRot(index, Rotation, Radius, OrbsCount);
    }
}