using AQMod.Assets;
using AQMod.Assets.Textures;
using AQMod.Common;
using AQMod.Common.Config;
using AQMod.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace AQMod.Content.CursorDyes
{
    public sealed class CursorDyeSword : CursorDyeTextureReplace
    {
        public CursorDyeSword(Mod mod, string name) : base(mod, name)
        {
        }

        protected override bool HasOutlines => true;
        protected override TEA<CursorType> TextureEnumeratorArray => TextureCache.SwordCursors;
    }
}