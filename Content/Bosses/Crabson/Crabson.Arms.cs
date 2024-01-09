using Aequus.Core.PhysicsObjects;

namespace Aequus.Content.Bosses.Crabson;

public partial class Crabson {
    public struct Arm {
        public Vector2 Position;
        public float Rotation;
        public int Direction;
        public float MouthOpen;

        public RopeChain Rope;

        public Vector2 Center => Position;

        public Vector2 GetIdlePosition(NPC crabson) {
            return crabson.Center + new Vector2(crabson.width * Direction, -crabson.height);
        }
        public Vector2 GetStartPosition(NPC crabson) {
            return crabson.Center + new Vector2(crabson.width / 2f * Direction, 0f);
        }

        public void Initialize(NPC crabson, int direction) {
            Direction = direction;
            Rotation = 0f;
            MouthOpen = 0f;
            Position = GetIdlePosition(crabson);
            Rope = new RopeChain(GetStartPosition(crabson), 16, 30f, Vector2.UnitY * 0.1f);
        }

        public void UpdateRope(NPC crabson) {
            Rope.StartPos = GetStartPosition(crabson);
            RopeChain.RopeSegment endPosition = Rope.segments[^1];
            endPosition.position = Position;
            Rope.Update();
            Position = endPosition.position;
        }
    }

    public Arm LeftArm;
    public Arm RightArm;
}
