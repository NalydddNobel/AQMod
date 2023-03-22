using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Aequus.Common.Rendering.RadonMossFogRenderer;

namespace Aequus.Common.PlayerLayers {
    public class LegsOverlay : PlayerDrawLayer {
        public override Position GetDefaultPosition() {
            return new AfterParent(PlayerDrawLayers.Shoes);
        }

        private void DrawLegs(ref PlayerDrawSet drawInfo, Asset<Texture2D> asset, Color color) {
            if (asset == null || asset.IsDisposed || !asset.IsLoaded) {
                return;
            }

            var legPosition = new Vector2(
                    (int)(drawInfo.Position.X - Main.screenPosition.X - drawInfo.drawPlayer.legFrame.Width / 2 + drawInfo.drawPlayer.width / 2),
                    (int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawInfo.drawPlayer.height - (float)drawInfo.drawPlayer.legFrame.Height + 4f)
                )
                + drawInfo.drawPlayer.legPosition + drawInfo.legVect + drawInfo.legsOffset;
            Rectangle legFrame;

            if (drawInfo.isSitting) {
                legPosition += new Vector2(4f * drawInfo.drawPlayer.direction, -4f);
                legFrame = new(0, 56 * 5, 40, 56);
            }
            else {
                legFrame = drawInfo.drawPlayer.legFrame;
            }
                drawInfo.DrawDataCache.Add(new DrawData(
                asset.Value,
                legPosition,
                legFrame,
                color,
                drawInfo.drawPlayer.legRotation,
                drawInfo.legVect,
                1f, drawInfo.playerEffect, 0) {
                shader = drawInfo.cBody
            });
        }

        protected override void Draw(ref PlayerDrawSet drawInfo) {

            if (drawInfo.drawPlayer.invis) {
                return;
            }

            var aequus = drawInfo.drawPlayer.Aequus();
            DrawLegs(ref drawInfo, aequus.LegsOverlayTexture, drawInfo.colorArmorLegs);
            DrawLegs(ref drawInfo, aequus.LegsOverlayGlowTexture, Color.White with { A = 0 } * (1f - drawInfo.shadow));
        }
    }
}

namespace Aequus {
    partial class AequusPlayer {

        private Asset<Texture2D> legsOverlayTexture;
        /// <summary>
        /// Texture used for drawing a legs overlay. Asset related properties should not be interacted with on a Server.
        /// </summary>
        public Asset<Texture2D> LegsOverlayTexture {
            get {
                if (Main.netMode == NetmodeID.Server) {
                    throw new Exception("Texture property was attempted to be get on server.");
                }
                return legsOverlayTexture;
            }
            set {
                if (Main.netMode == NetmodeID.Server) {
                    throw new Exception("Texture property attempted to be set on server.");
                }
                legsOverlayTexture = value;
            } 
        }
        private Asset<Texture2D> legsOverlayGlowTexture;
        /// <summary>
        /// Texture used for drawing a legs overlay. Asset related properties should not be interacted with on a Server.
        /// </summary>
        public Asset<Texture2D> LegsOverlayGlowTexture {
            get {
                if (Main.netMode == NetmodeID.Server) {
                    throw new Exception("Texture property was attempted to be get on server.");
                }
                return legsOverlayGlowTexture;
            }
            set {
                if (Main.netMode == NetmodeID.Server) {
                    throw new Exception("Texture property attempted to be set on server.");
                }
                legsOverlayGlowTexture = value;
            } 
        }
    }
}