using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.UI.Drawers
{
    public static class InvDrawer
    {
        public struct DrawingData
        {
            public Item item;
            public Texture2D texture;
            public Vector2 drawPos;
            public Vector2 origin;
            public Rectangle frame;
            public Color drawColor;
            public Color color;
            public float scale;
            public float scale2;
        }

        public static void Draw(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Item item, Rectangle frame, Color drawColor, Color lightColor, Vector2 origin, float scale, float scale2)
        {
            Texture2D itemTexture = texture;
            if (ItemLoader.PreDrawInInventory(item, spriteBatch, position, frame, drawColor, item.GetColor(Main.inventoryBack), origin, scale * scale2))
            {
                spriteBatch.Draw(itemTexture, position, frame, drawColor, 0f, origin, scale * scale2, SpriteEffects.None, 0f);
                if (item.color != Color.Transparent)
                    spriteBatch.Draw(itemTexture, position, frame, item.GetColor(Main.inventoryBack), 0f, origin, scale * scale2, SpriteEffects.None, 0f);
            }
            ItemLoader.PostDrawInInventory(item, spriteBatch, position, frame, drawColor, item.GetColor(lightColor), origin, scale * scale2);
            if (ItemID.Sets.TrapSigned[item.type])
                spriteBatch.Draw(TextureAssets.Wire.Value, position + new Vector2(40f, 40f) * Main.inventoryScale, new Rectangle(4, 58, 8, 8), Main.inventoryBack, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);
        }
        public static void Draw(SpriteBatch spriteBatch, Item item, Vector2 position, Color? color = null, int maxSize = 32)
        {
            Draw(spriteBatch, GetDrawingDataFor(item, position, color, maxSize));
        }
        public static void Draw(SpriteBatch spriteBatch, DrawingData data)
        {
            Draw(spriteBatch, data.texture, data.drawPos, data.item, data.frame, data.drawColor, data.color, data.origin, data.scale, data.scale2);
        }

        public static DrawingData GetDrawingDataFor(Item item, Vector2 position, Color? color = null, int maxSize = 32)
        {
            Main.instance.LoadItem(item.type);

            var data = new DrawingData
            {
                item = item,
                texture = TextureAssets.Item[item.type].Value
            };

            data.frame = Main.itemAnimations[item.type] == null ? data.texture.Frame() : Main.itemAnimations[item.type].GetFrame(data.texture);
            data.scale2 = 1f;
            data.color = Color.White;

            ItemSlot.GetItemLight(ref data.color, ref data.scale2, item.type);

            data.drawColor = item.GetAlpha(data.color);
            if (color != null)
            {
                float r = data.drawColor.R / 255f;
                float g = data.drawColor.G / 255f;
                float b = data.drawColor.B / 255f;
                float a = data.drawColor.A / 255f;

                var color2 = color.Value;
                float r2 = color2.R / 255f;
                float g2 = color2.G / 255f;
                float b2 = color2.B / 255f;
                float a2 = color2.A / 255f;

                data.drawColor = new Color(r * r2, g * g2, b * b2, a * a2);
            }

            float scale = 1f;
            if (data.frame.Width > maxSize || data.frame.Height > maxSize)
                scale = data.frame.Width <= data.frame.Height ? (float)maxSize / data.frame.Height : (float)maxSize / data.frame.Width;

            data.scale = scale * Main.inventoryScale;

            Vector2 backSize = TextureAssets.InventoryBack.Value.Size() * Main.inventoryScale;
            data.origin = data.frame.Size() * (data.scale2 / 2f - 0.5f);

            data.drawPos = position + backSize / 2f - data.frame.Size() * data.scale / 2f;
            //DrawItemInv(data.drawPos, item, data.frame, data.drawColor, data.color, data.origin, data.scale, data.scale2);
            return data;
        }
    }
}