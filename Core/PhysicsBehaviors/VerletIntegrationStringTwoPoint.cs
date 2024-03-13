namespace Aequus.Core.PhysicsBehaviors;

public class VerletIntegrationStringTwoPoint<T> : VerletIntegrationString<T> where T : IVerletIntegrationNode, new() {
    public Vector2 EndPosition { get; set; }

    public VerletIntegrationStringTwoPoint(Vector2 startPoint, Vector2 endPoint, int segmentCount, float segmentLength, Vector2 gravity, float damping = 0f, int accuracy = 15)
        : base(startPoint, segmentCount, segmentLength, gravity, damping, accuracy) {
        EndPosition = endPoint;
    }

    public override void Update() {
        segments[^1].Position = EndPosition;
        base.Update();
    }
}
