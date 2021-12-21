using Microsoft.Xna.Framework;
using Terraria;

namespace AQMod.Dusts
{
    public class SpawnPatterns
    {
        public static int SpawnDustCentered(Vector2 position, int type, Color color = default(Color), float scale = 1f)
        {
            int d = Dust.NewDust(position, 2, 2, type, 0f, 0f, 0, color, scale);
            Main.dust[d].position.X = position.X - Main.dust[d].frame.Width / 2f;
            Main.dust[d].position.Y = position.Y - Main.dust[d].frame.Height / 2f;
            return d;
        }

        public static int SpawnDustCentered(Vector2 position, int type, Vector2 velocity, Color color = default(Color), float scale = 1f)
        {
            int d = SpawnDustCentered(position, type, color, scale);
            Main.dust[d].velocity = velocity;
            return d;
        }
    }
}