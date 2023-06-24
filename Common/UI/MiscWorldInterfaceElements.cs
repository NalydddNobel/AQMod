using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.UI;

namespace Aequus.Common.UI {
    /// <summary>
    /// Used to render any World elements which need to be on the UI layer. This renders after Entity Healthbars are drawn.
    /// </summary>
    public class MiscWorldInterfaceElements : UILayer {
        /// <summary>
        /// A drawdata list to add into, if you know how Player Layers work, this should be familiar. Currently doesn't support shaders.
        /// </summary>
        public static readonly List<DrawData> DrawData = new();

        public override string Layer => AequusUI.InterfaceLayers.EntityHealthBars_16;
        public override InterfaceScaleType ScaleType => InterfaceScaleType.Game;

        public override bool Draw(SpriteBatch spriteBatch) {
            if (DrawData.Count <= 0) {
                return true;
            }

            foreach (var d in DrawData) {
                d.Draw(Main.spriteBatch);
            }
            DrawData.Clear();
            return true;
        }

        public override void OnClearWorld() {
            DrawData.Clear();
        }
    }
}