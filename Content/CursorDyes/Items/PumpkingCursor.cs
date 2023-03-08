namespace Aequus.Content.CursorDyes.Items
{
    public class PumpkingCursor : CursorDyeBase
    {
        public override ICursorDye InitalizeDye()
        {
            return new TextureChangeCursor($"{Texture}/Cursor");
        }
    }
}