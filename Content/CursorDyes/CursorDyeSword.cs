using AQMod.Assets;
using Terraria.ModLoader;

namespace AQMod.Content.CursorDyes
{
    public sealed class CursorDyeSword : CursorDyeTextureReplace
    {
        public CursorDyeSword(Mod mod, string name) : base(mod, name)
        {
        }

        protected override bool HasOutlines => true;
        protected override TEA<CursorType> TextureEnumeratorArray => OldTextureCache.SwordCursors;
    }
}