using Microsoft.Xna.Framework;
using Terraria;

namespace AQMod.Effects.WorldEffects
{
    public class FishingPopperEffect : WorldVisualEffect
    {
        public FishingPopperEffect(int x, int y, byte waterLevel, int dustType, Color dustColor) : base(x, y)
        {
            spawnX = x;
            this.dustType = dustType;
            this.dustColor = dustColor;
            this.y += (byte.MaxValue - waterLevel) / 16;
        }

        public int spawnX;
        public int xOffset => x - spawnX;
        public int dustType;
        public Color dustColor;

        public override bool Update()
        {
            int xOff = xOffset;
            if (xOff > 20)
                return false;
            x++;
            if (xOff % 4 == 0)
            {
                int d = Dust.NewDust(new Vector2(x, y), 2, 2, dustType, 0f, 0f, 0, dustColor);
                Main.dust[d].velocity.X = 0f;
                Main.dust[d].velocity.Y = -10f;
                d = Dust.NewDust(new Vector2(spawnX - xOff, y), 2, 2, dustType, 0f, 0f, 0, dustColor);
                Main.dust[d].velocity.X = 0f;
                Main.dust[d].velocity.Y = -10f;
            }
            return true;
        }
    }
}