namespace Aequus.Content.Bosses.Crabson;

public partial class Crabson {
    public struct Arm {
        public Vector2 Position;
        public float Rotation;
        public int Direction;
        public float MouthOpen;

        public Vector2 Center => Position;

        public Vector2 GetIdlePosition(NPC crabson) {
            return crabson.Center + new Vector2(crabson.width * Direction, -crabson.height);
        }

        public void Initialize(NPC crabson, int direction) {
            Direction = direction;
            Rotation = 0f;
            MouthOpen = 0f;
            Position = GetIdlePosition(crabson);
        }
    }

    public Arm LeftArm;
    public Arm RightArm;
}
