using Aequus.Common.UI;
using Aequus.Core.Graphics.Commands;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace Aequus.Core.Graphics {
    /// <summary>
    /// Used to render any World elements which need to be on the UI layer. This renders after Entity Healthbars are drawn.
    /// </summary>
    public class MiscWorldUI : UILayer {
        /// <summary>
        /// A drawdata list to add into, if you know how Player Layers work, this should be familiar. Currently doesn't support shaders.
        /// </summary>
        public static readonly DrawCommandHandler Drawer = new();

        public override string Layer => InterfaceLayers.EntityHealthBars_16;
        public override InterfaceScaleType ScaleType => InterfaceScaleType.Game;

        public override bool Draw(SpriteBatch spriteBatch) {
            Drawer.InvokeAll(spriteBatch);
            return true;
        }

        public override void OnGameUpdate() {
            Drawer.Clear();
        }

        public override void OnClearWorld() {
            Drawer.Clear();
        }
    }
}