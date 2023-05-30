using Aequus.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.UI;

namespace Aequus.UI.Elements {
    public class ItemSlotElement : UIElement
    {
        public Item item;
        public Texture2D back;
        public SpriteFrameData icon;
        public bool canHover;
        public bool showItemTooltipOnHover;

        public bool HasItem => item != null && !item.IsAir;

        public ItemSlotElement(Texture2D back, SpriteFrameData icon = null)
        {
            item = new Item();
            this.back = back;
            this.icon = icon;
        }

        public override void Update(GameTime gameTime)
        {
            if (item == null)
            {
                item = new Item();
            }
            if (item.type == ItemID.DD2EnergyCrystal && !DD2Event.Ongoing)
            {
                item.TurnToAir();
            }
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var c = GetDimensions();
            float dimensionMin = Math.Min(c.Width, c.Height);
            float newScale = dimensionMin / 52f;
            float oldScale = Main.inventoryScale;
            Main.inventoryScale = newScale;
            spriteBatch.Draw(back, new Vector2(c.X, c.Y), null, Color.White, 0f, Vector2.Zero, new Vector2(c.Width / 52f, c.Height / 52f), SpriteEffects.None, 0f);
            if (!HasItem && icon != null)
            {
                spriteBatch.Draw(icon.Texture.Value, new Vector2(c.X, c.Y) + back.Size() / 2f * newScale, icon.Frame, Color.White * 0.35f, 0f, icon.Frame.Size() / 2f, Math.Min(icon.Texture.Value.Width, icon.Texture.Value.Height), SpriteEffects.None, 0f);
            }
            var drawLoc = new Vector2(c.X, c.Y);
            if (c.Width > c.Height)
            {
                drawLoc.X += c.Width / 2 - c.Height / 2f;
            }
            else if (c.Height > c.Width)
            {
                drawLoc.Y += c.Height / 2 - c.Width / 2f;
            }
            if (showItemTooltipOnHover && canHover && IsMouseHovering)
            {
                AequusUI.HoverItem(item, ItemSlot.Context.ShopItem);
            }
            ItemSlotRenderer.Draw(spriteBatch, item, drawLoc);
            base.Draw(spriteBatch);
            Main.inventoryScale = oldScale;
        }
    }
}