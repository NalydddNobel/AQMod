using Aequus.UI.Drawers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.UI;

namespace Aequus.UI
{
    public sealed class ItemSlotData : UIElement
    {
        public Item item;
        public int X;
        public int Y;
        public float Scale;
        public Texture2D back;

        public bool HasItem => item != null && !item.IsAir;

        public ItemSlotData(int x, int y, Texture2D back)
        {
            X = x;
            Y = y;
            Scale = 1f;
            item = new Item();
            this.back = back;
        }

        public Rectangle GetHitbox()
        {
            return GetHitbox(Scale);
        }

        public Rectangle GetHitbox(float scale)
        {
            var back = TextureAssets.InventoryBack.Value;
            return new Rectangle(X, Y, (int)(back.Width * scale), (int)(back.Height * scale));
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
            spriteBatch.Draw(back, new Vector2(X, Y), null, Color.White, 0f, Vector2.Zero, Main.inventoryScale * Scale, SpriteEffects.None, 0f);
            InvDrawer.Draw(spriteBatch, item, new Vector2(X, Y));
            base.Draw(spriteBatch);
        }

    }
}