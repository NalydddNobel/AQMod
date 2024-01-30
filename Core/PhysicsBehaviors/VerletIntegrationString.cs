using System;

namespace Aequus.Core.PhysicsBehaviors;

public class VerletIntegrationString<T> where T : IVerletIntegrationNode, new() {
    public T[] segments;
    public int accuracy;
    public float segmentLength;
    public Vector2 gravity;
    public float damping;
    public Vector2 StartPos { get; set; }

    public VerletIntegrationString(Vector2 startPoint, int segmentCount, float segmentLength, Vector2 gravity, float damping = 0f, int accuracy = 15) {
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
                segments[i].Position = segments[0].Position;
            }

            segments[i].UpdateNode(damping, gravity);
        }

        for (int i = 0; i < accuracy; i++) {
            ConstrainPoints();
        }
    }

    private void ConstrainPoints() {
        for (int i = 0; i < segments.Length - 1; i++) {
            T segment = segments[i];
            T nextSegment = segments[i + 1];

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
            if (i != 0) {
                segment.Position += segment.MoveNode(changeAmount * -0.5f);
                nextSegment.Position += segment.MoveNode(changeAmount * 0.5f);
            }
            else {
                nextSegment.Position += segment.MoveNode(changeAmount);
            }
        }
    }
}
