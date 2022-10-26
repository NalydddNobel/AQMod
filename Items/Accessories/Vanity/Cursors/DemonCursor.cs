using Aequus.Content.CursorDyes;
using Aequus.Items.Misc.Energies;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Vanity.Cursors
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