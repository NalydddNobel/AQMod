using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace AQMod.Assets
{
    public interface ITextureComponent
    {
        bool IsNull { get; }
        string Path { get; }
        void LoadValue(object context);
        Ref<Texture2D> GetRef(object context);
    }
}