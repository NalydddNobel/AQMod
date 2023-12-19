using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria.GameContent.Events;
using Terraria.UI;

namespace Aequus.Common.UI.Elements {
    public class AequusItemSlotElement : UIElement {
        public Item item;
        public Texture2D back;
        public Asset<Texture2D> Icon;
        public Rectangle IconFrame;
        public bool canHover;
        public bool showItemTooltipOnHover;

        public bool HasItem => item != null && !item.IsAir;

        public AequusItemSlotElement(Texture2D back, Asset<Texture2D> icon = null, Rectangle? frame = null) {
            item = new Item();
            this.back = back;
            Icon = icon;
            if (Icon != null) {
                IconFrame = frame ?? Icon.Frame();
            }
        }

        public override void Update(GameTime gameTime) {
            item ??= new Item();
            if (item.type == ItemID.DD2EnergyCrystal && !DD2Event.Ongoing) {
                item.TurnToAir();
            }
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch) {
            var c = GetDimensions();
            float dimensionMin = Math.Min(c.Width, c.Height);
            float newScale = dimensionMin / 52f;
            float oldScale = Main.inventoryScale;
            Main.inventoryScale = newScale;
            spriteBatch.Draw(back, new Vector2(c.X, c.Y), null, Color.White, 0f, Vector2.Zero, new Vector2(c.Width / 52f, c.Height / 52f), SpriteEffects.None, 0f);
            if (!HasItem && Icon != null) {
                spriteBatch.Draw(Icon.Value, new Vector2(c.X, c.Y) + back.Size() / 2f * newScale, IconFrame, Color.White * 0.35f, 0f, IconFrame.Size() / 2f, Math.Min(IconFrame.Width, IconFrame.Height), SpriteEffects.None, 0f);
            }
            var drawLoc = new Vector2(c.X, c.Y);
            if (c.Width > c.Height) {
                drawLoc.X += c.Width / 2 - c.Height / 2f;
            }
            else if (c.Height > c.Width) {
                drawLoc.Y += c.Height / 2 - c.Width / 2f;
            }
            if (showItemTooltipOnHover && canHover && IsMouseHovering) {
                UIHelper.HoverItem(item, ItemSlot.Context.ShopItem);
            }

            ItemSlotRenderer.Draw(spriteBatch, item, drawLoc);
            base.Draw(spriteBatch);
            Main.inventoryScale = oldScale;
        }
    }
}