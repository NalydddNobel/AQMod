using Aequus.Common.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.Common.UI {
    /// <summary>
    /// Used to render any World elements which need to be on the UI layer. This renders after Entity Healthbars are drawn.
    /// </summary>
    public class MiscWorldInterfaceElements : UILayer {
        /// <summary>
        /// A drawdata list to add into, if you know how Player Layers work, this should be familiar. Currently doesn't support shaders.
        /// </summary>
        public static readonly List<IDrawCommand> Commands = new();

        public override string Layer => InterfaceLayers.EntityHealthBars_16;
        public override InterfaceScaleType ScaleType => InterfaceScaleType.Game;

        public interface IDrawCommand {
            void Draw(SpriteBatch spriteBatch);
        }

        public record struct DrawDataCommand(DrawData drawData) : IDrawCommand {
            public void Draw(SpriteBatch spriteBatch) {
                drawData.Draw(spriteBatch);
            }
        }

        public record struct DrawColorCodedStringCommand(DynamicSpriteFont font, string text, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, bool ignoreColors = false) : IDrawCommand {
            public void Draw(SpriteBatch spriteBatch) {
                ChatManager.DrawColorCodedString(spriteBatch, font, text, position, baseColor, rotation, origin, baseScale, maxWidth, ignoreColors);
            }
        }

        public record struct DrawBasicVertexStripCommand(Texture2D texture, Vector2[] lineSegments, float[] lineRotations, VertexStrip.StripColorFunction getColor, VertexStrip.StripHalfWidthFunction getWidth, Vector2 offset = default, bool includeBacksides = true, bool tryStoppingOddBug = true) : IDrawCommand {
            public void Draw(SpriteBatch spriteBatch) {
                AequusDrawing.DrawBasicVertexLine(texture, lineSegments, lineRotations, getColor, getWidth, offset, includeBacksides, tryStoppingOddBug);
            }
        }

        public static void Draw(DrawData drawData) {
            Commands.Add(new DrawDataCommand(drawData));
        }

        public static void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effect, float inactiveLayerDepth = 0f) {
            Draw(new DrawData(texture, position, sourceRect, color, rotation, origin, scale, effect, inactiveLayerDepth));
        }

        public static void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect, float inactiveLayerDepth = 0f) {
            Draw(new DrawData(texture, position, sourceRect, color, rotation, origin, scale, effect, inactiveLayerDepth));
        }

        public static void DrawColorCodedString(DynamicSpriteFont font, string text, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, bool ignoreColors = false) {
            Commands.Add(new DrawColorCodedStringCommand(font, text, position, baseColor, rotation, origin, baseScale, maxWidth, ignoreColors));
        }

        public static void DrawBasicVertexLine(Texture2D texture, Vector2[] lineSegments, float[] lineRotations, VertexStrip.StripColorFunction getColor, VertexStrip.StripHalfWidthFunction getWidth, Vector2 offset = default, bool includeBacksides = true, bool tryStoppingOddBug = true) {
            Commands.Add(new DrawBasicVertexStripCommand(texture, lineSegments, lineRotations, getColor, getWidth, offset, includeBacksides, tryStoppingOddBug));
        }

        public override bool Draw(SpriteBatch spriteBatch) {
            lock (Commands) {
                if (Commands.Count <= 0) {
                    return true;
                }

                foreach (var d in Commands) {
                    d.Draw(Main.spriteBatch);
                }
                Commands.Clear();
            }
            return true;
        }

        public override void OnPreUpdatePlayers() {
            lock (Commands) {
                Commands.Clear();
            }
        }

        public override void OnClearWorld() {
            lock (Commands) {
                Commands.Clear();
            }
        }
    }
}