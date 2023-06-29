using Aequus.Projectiles.Monster;
using Microsoft.Xna.Framework;

namespace Aequus.Items.Weapons.Necromancy.Sceptres {
    public abstract class SceptreProjectileBase : EnemyAttachedProjBase {
        protected Vector2[][] LineCoordinates { get; private set; }
        protected float[][] LineRotations { get; private set; }
        protected int Lines { get; private set; }
        protected int Segments { get; private set; }

        protected void SetLine(int lines, int segments) {
            Lines = lines;
            Segments = segments;
            LineCoordinates = new Vector2[lines][];
            LineRotations = new float[lines][];
            for (int i = 0; i < lines; i++) {
                LineCoordinates[i] = new Vector2[segments];
                LineRotations[i] = new float[segments];
            }
        }
    }
}