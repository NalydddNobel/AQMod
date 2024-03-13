namespace Aequus.Core.PhysicsBehaviors;

public interface IVerletIntegrationNode {
    Vector2 Position { get; set; }
    Vector2 OldPosition { get; set; }

    Vector2 MoveNode(Vector2 difference);

    public void UpdateNode(float damping, Vector2 gravity) {
        Vector2 velocity = (Position - OldPosition) / (1f + damping) + gravity;
        OldPosition = Position;
        Position += MoveNode(velocity);
    }
}
