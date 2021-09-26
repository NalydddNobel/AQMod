using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Common.CursorDyes
{
    public abstract class CursorDye
    {
        public readonly string Mod;
        public readonly string Name;

        public CursorDye(Mod mod, string name)
        {
            Mod = mod.Name;
            Name = name;
        }

        internal bool PredicateMatch(string mod, string name)
        {
            return mod == Mod && name == Name;
        }

        public virtual bool ApplyColor(Player player, AQVisualsPlayer drawingPlayer, out Color newCursorColor)
        {
            newCursorColor = default(Color);
            return false;
        }

        public virtual Vector2? DrawThickCursor(bool smart)
        {
            return null;
        }

        public virtual bool PreDrawCursor(Player player, AQVisualsPlayer drawingPlayer, Vector2 bonus, bool smart)
        {
            return false;
        }

        public virtual void PostDrawCursor(Player player, AQVisualsPlayer drawingPlayer, Vector2 bonus, bool smart)
        {

        }

        public virtual bool PreDrawCursorOverrides(Player player, AQVisualsPlayer drawingPlayer)
        {
            return false;
        }

        public virtual void PostDrawCursorOverrides(Player player, AQVisualsPlayer drawingPlayer)
        {

        }
    }
}