using Aequus.Items;
using Microsoft.Xna.Framework.Graphics;

namespace Aequus.UI
{
    public class ItemMovementInterfaceRenderer : BaseUserInterface
    {
        public override string Layer => "Vanilla: Interface Logic 4";

        public override bool Draw(SpriteBatch spriteBatch)
        {
            AequusItem.DrawMovingItems();
            return true;
        }
    }
}