using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace Aequus.Common.UI {
    public abstract class UILayer : ModType {
        public abstract string Layer { get; }
        public virtual InterfaceScaleType ScaleType => InterfaceScaleType.UI;

        protected sealed override void Register() {
            UISystem.RegisterUserInterface(this);
        }

        public sealed override void SetupContent() {
            SetStaticDefaults();
        }

        public virtual void OnClearWorld() {
        }
        public virtual void OnPreUpdatePlayers() {
        }
        public virtual void OnUIUpdate(GameTime gameTime) {
        }
        public abstract bool Draw(SpriteBatch spriteBatch);
    }
}
