using AQMod.Assets;
using Terraria.ModLoader;

namespace AQMod.Content.CursorDyes
{
    public sealed class CursorDyeDemon : CursorDyeTextureReplace
    {
        public CursorDyeDemon(Mod mod, string name) : base(mod, name)
        {
        }

        protected override bool HasOutlines => true;
        protected override TEA<CursorType> TextureEnumeratorArray => TextureCache.DemonCursors;
    }
}