using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace AQMod.Common.Skies
{
    public class AtmosphericCurrentsSky : AQSky
    {
        public override void Activate(Vector2 position, params object[] args)
        {
        }

        public override void Deactivate(params object[] args)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (SkyDepth(maxDepth, minDepth))
            {

            }
        }

        public override bool IsActive()
        {
            return Main.myPlayer != -1 && Main.player[Main.myPlayer].active && CanEvenSeeTheSkyAtAll();
        }

        public override void Reset()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}