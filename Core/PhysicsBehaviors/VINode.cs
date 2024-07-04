namespace AequusRemake.Core.PhysicsBehaviors;

public struct VINode : IVerletIntegrationNode {
    public Vector2 Position { get; set; }
    public Vector2 OldPosition { get; set; }

    public VINode(Vector2 pos) {
        Position = pos;
        OldPosition = pos;
    }

    public Vector2 MoveNode(Vector2 difference) {
        return difference;
    }
}
