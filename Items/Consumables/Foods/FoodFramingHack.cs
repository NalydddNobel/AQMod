using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;

namespace Aequus.Items.Consumables.Foods
{
    internal sealed class FoodFramingHack : DrawAnimation
    {
        public override Rectangle GetFrame(Texture2D texture, int frameCounterOverride = -1)
        {
            return texture.Frame(verticalFrames: 3, frameY: 0);
        }
    }
}