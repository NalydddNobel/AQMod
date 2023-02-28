using Aequus.Content.CursorDyes;
using Terraria.ModLoader;

namespace Aequus.Content.CursorDyes.Items
{
    [LegacyName("SwordCursorDye")]
    public class SwordCursor : CursorDyeBase
    {
        public override ICursorDye InitalizeDye()
        {
            return new TextureChangeCursor($"{Texture}/SwordCursor");
        }
    }
}