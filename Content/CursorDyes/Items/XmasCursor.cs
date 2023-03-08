using Microsoft.Xna.Framework;

namespace Aequus.Content.CursorDyes.Items
{
    public class XmasCursor : CursorDyeBase
    {
        public override ICursorDye InitalizeDye()
        {
            return new TextureChangeCursor($"{Texture}/Cursor", new Vector2(0f, -2f));
        }
    }
}