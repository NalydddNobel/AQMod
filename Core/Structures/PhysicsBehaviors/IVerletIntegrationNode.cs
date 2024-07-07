namespace AequusRemake.Core.Structures.PhysicsBehaviors;

public interface IVerletIntegrationNode {
    Vector2 Position { get; set; }
    Vector2 OldPosition { get; set; }

    Vector2 MoveNode(Vector2 difference);
}
