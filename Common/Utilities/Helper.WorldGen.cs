using Microsoft.Xna.Framework;
using Terraria.WorldBuilding;

namespace Aequus
{
    partial class Helper { 
        public static float CircleDistanceInterval(int originX, int originY, int x, int y, int horizontalRadius, int verticalRadius) {
            return new Vector2((x - originX) / (float)horizontalRadius, (y - originY) / (float)verticalRadius).Length();
        }
        public static float CircleDistanceInterval(Point origin, int x, int y, int horizontalRadius, int verticalRadius) {
            return CircleDistanceInterval(origin.X, origin.Y, x, y, horizontalRadius, verticalRadius);
        }
    }
}