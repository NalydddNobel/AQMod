using Microsoft.Xna.Framework;

namespace Aequus.Content.CursorDyes
{
    public interface ICursorDye
    {
        int Type { get; set; }

        bool PreDrawCursor(ref Vector2 bonus, ref bool smart)
        {
            return true;
        }
        void PostDrawCursor(Vector2 bonus, bool smart)
        {
        }
        bool DrawThickCursor(ref Vector2 bonus, ref bool smart)
        {
            return true;
        }
        Color? GetCursorColor()
        {
            return null;
        }
    }
}