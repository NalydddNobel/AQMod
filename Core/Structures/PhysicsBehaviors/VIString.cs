using System;

namespace AequusRemake.Core.Structures.PhysicsBehaviors;

public class VIString<T> where T : IVerletIntegrationNode, new() {
    public T[] segments;
    public int accuracy;
    public float segmentLength;
    public Vector2 gravity;
    public float damping;
    public Vector2 StartPos { get; set; }

    public VIString(Vector2 startPoint, int segmentCount, float segmentLength, Vector2 gravity, float damping = 0f, int accuracy = 15) {
        segments = new T[segmentCount];
        for (int i = 0; i < segmentCount; i++) {
            T value = new T();
            value.Position = value.OldPosition = startPoint + gravity.SafeNormalize(Vector2.Zero) * i;
            segments[i] = value;
        }

        StartPos = startPoint;

        this.segmentLength = segmentLength;
        this.gravity = gravity;
        this.damping = damping;
        this.accuracy = Math.Max(1, accuracy);
    }

    public virtual void Update() {
        segments[0].Position = StartPos;

        for (int i = 0; i < segments.Length; i++) {
            if (segments[i].Position.HasNaNs()) {
                segments[i].Position = segments[0].Position + Vector2.UnitY;
            }

            UpdateNode(ref segments[i]);
        }

        for (int i = 0; i < accuracy; i++) {
            ConstrainPoints();
        }
    }

    private void UpdateNode(ref T node) {
        Vector2 velocity = (node.Position - node.OldPosition) / (1f + damping) + gravity;
        node.OldPosition = node.Position;
        node.Position += node.MoveNode(velocity);
    }

    private void ConstrainPoints() {
        for (int i = 0; i < segments.Length - 1; i++) {
            ConstrainPointsInner(ref segments[i], ref segments[i + 1], i);
        }
    }

    private void ConstrainPointsInner(ref T segment, ref T nextSegment, int index) {
        float dist = (segment.Position - nextSegment.Position).Length();
        float error = MathF.Abs(dist - segmentLength);
        Vector2 changeDirection = Vector2.Zero;

        if (dist > segmentLength) {
            changeDirection = (segment.Position - nextSegment.Position).SafeNormalize(Vector2.Zero);
        }
        else if (dist < segmentLength) {
            changeDirection = (nextSegment.Position - segment.Position).SafeNormalize(Vector2.Zero);
        }

        Vector2 changeAmount = changeDirection * error;
        if (index != 0) {
            segment.Position += segment.MoveNode(changeAmount * -0.5f);
            nextSegment.Position += segment.MoveNode(changeAmount * 0.5f);
        }
        else {
            nextSegment.Position += segment.MoveNode(changeAmount);
        }
    }
}
