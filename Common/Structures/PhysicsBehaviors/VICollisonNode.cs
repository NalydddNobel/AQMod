using System;

namespace Aequus.Common.Structures.PhysicsBehaviors;

/// <summary>A Verlet Integrated node with Tile Collision.</summary>
public struct VICollisonNode : IVerletIntegrationNode {
    public Vector2 Position { get; set; }
    public Vector2 OldPosition { get; set; }

    public VICollisonNode(Vector2 pos) {
        Position = pos;
        OldPosition = pos;
    }

    public Vector2 MoveNode(Vector2 difference) {
        Vector2 newVelocity = Collision.noSlopeCollision(Position - new Vector2(3), difference, 6, 6, true, true);
        Vector2 final = difference;

        if (Math.Abs(newVelocity.X) < Math.Abs(difference.X)) {
            final.X = 0;
        }

        if (Math.Abs(newVelocity.Y) < Math.Abs(difference.Y)) {
            final.Y = 0;
        }

        return final;
    }
}
