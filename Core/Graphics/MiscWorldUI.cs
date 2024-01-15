using Aequus.Common.UI;
using Aequus.Core.Graphics.Commands;
using Aequus.Core.UI;
using System;
using Terraria.UI;

namespace Aequus.Core.Graphics {
    /// <summary>
    /// Used to render any World elements which need to be on the UI layer. This renders after Entity Healthbars are drawn.
    /// </summary>
    [Obsolete]
    public class MiscWorldUI : UILayer {
        /// <summary>
        /// A drawdata list to add into, if you know how Player Layers work, this should be familiar. Currently doesn't support shaders.
        /// </summary>
        public static readonly DrawCommandHandler Drawer = new();

        protected override bool DrawSelf() {
            Drawer.InvokeAll(Main.spriteBatch);
            return true;
        }

        public override void OnPreUpdatePlayers() {
            Drawer.Clear();
        }

        public override void OnClearWorld() {
            Drawer.Clear();
        }

        public MiscWorldUI() : base("Remove", InterfaceLayers.EntityHealthBars_16, InterfaceScaleType.Game) { }
    }
}