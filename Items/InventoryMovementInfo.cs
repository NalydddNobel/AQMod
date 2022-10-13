using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Aequus.Items
{
    public struct InventoryMovementInfo
    {
        public Item item;
        public SpriteBatch spriteBatch;
        public Vector2 position;
        public Rectangle frame;
        public Color drawColor;
        public Color itemColor;
        public Vector2 origin;
        public float scale;
    }
}