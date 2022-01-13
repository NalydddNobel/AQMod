using Terraria.ModLoader;

namespace AQMod.Content.CursorDyes
{
    public sealed class CursorDyeDemon : LegacyCursorDyeTextureReplace
    {
        public CursorDyeDemon(Mod mod, string name) : base(mod, name)
        {
        }

        protected override bool HasOutlines => true;
        protected override string Path => "AQMod/Assets/Cursors/Demon/Cursor_";
    }
}