using Terraria.ModLoader;

namespace AQMod.Content.CursorDyes
{
    public sealed class CursorDyeSword : CursorDyeTextureReplace
    {
        public CursorDyeSword(Mod mod, string name) : base(mod, name)
        {
        }

        protected override bool HasOutlines => true;
        protected override string Path => "AQMod/Assets/Cursors/Sword/Cursor_";
    }
}