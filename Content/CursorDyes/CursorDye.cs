using AQMod.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.CursorDyes
{
    public abstract class CursorDye : ContentItem
    {
        public CursorDye(Mod mod, string name) : base(mod, name)
        {
        }

        public virtual bool ApplyColor(Player player, GraphicsPlayer drawingPlayer, out Color newCursorColor)
        {
            newCursorColor = default(Color);
            return false;
        }

        public virtual Vector2? DrawThickCursor(bool smart)
        {
            return null;
        }

        public virtual bool PreDrawCursor(Player player, GraphicsPlayer drawingPlayer, Vector2 bonus, bool smart)
        {
            return false;
        }

        public virtual void PostDrawCursor(Player player, GraphicsPlayer drawingPlayer, Vector2 bonus, bool smart)
        {
        }

        public virtual bool PreDrawCursorOverrides(Player player, GraphicsPlayer drawingPlayer)
        {
            return false;
        }

        public virtual void PostDrawCursorOverrides(Player player, GraphicsPlayer drawingPlayer)
        {
        }
    }
}