using Terraria.ModLoader;

namespace Aequus.Content.CursorDyes.Items
{
    [LegacyName("DemonicCursorDye")]
    public class DemonCursor : CursorDyeBase
    {
        public override ICursorDye InitalizeDye()
        {
            return new TextureChangeCursor($"{Texture}/DemonCursor");
        }
    }
}