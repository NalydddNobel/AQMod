using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.UI
{
    public abstract class BaseUserInterface : ModType
    {
        public abstract string Layer { get; }
        public virtual InterfaceScaleType ScaleType { get; }

        protected sealed override void Register()
        {
            AequusUI.RegisterUserInterface(this);
        }

        public sealed override void SetupContent()
        {
            SetStaticDefaults();
        }

        public abstract bool Draw(SpriteBatch spriteBatch);
    }
}
