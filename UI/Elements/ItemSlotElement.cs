using Aequus.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace Aequus.UI.Elements
{
    public class ItemSlotElement : UIElement
    {
        public Item item;
        public float Scale;
        public Texture2D back;
        public SpriteFrameData icon;
        public bool canHover;

        public bool HasItem => item != null && !item.IsAir;

        public ItemSlotElement(Texture2D back, SpriteFrameData icon = null)
        {
            Scale = 1f;
            item = new Item();
            this.back = back;
            this.icon = icon;

            Width.Set(pixels: (int)(back.Width * Scale), 0f);
            Height.Set(pixels: (int)(back.Height * Scale), 0f);
        }

        public override void Update(GameTime gameTime)
        {
            if (item == null)
            {
                item = new Item();
            }
            if (item.type == ItemID.DD2EnergyCrystal)
            {
                item.TurnToAir();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var c = GetDimensions();
            float oldScale = Main.inventoryScale;
            Main.inventoryScale = Scale;
            spriteBatch.Draw(back, new Vector2(c.X, c.Y), null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
            if (!HasItem && icon != null)
            {
                spriteBatch.Draw(icon.Texture.Value, new Vector2(c.X, c.Y) + back.Size() / 2f * Scale, icon.Frame, Color.White * 0.35f, 0f, icon.Frame.Size() / 2f, Scale, SpriteEffects.None, 0f);
            }
            ItemSlotRenderer.Draw(spriteBatch, item, new Vector2(c.X, c.Y));
            base.Draw(spriteBatch);
            Main.inventoryScale = oldScale;
        }
    }
}