using Aequus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.UI
{
    public class ItemSlotRenderer : ILoadable
    {
        public const int InventoryBackFramesX = 1;
        public static int InventoryBackFramesY = 3;
        public static Asset<Texture2D> InventoryBack { get; private set; }

        public static Rectangle InventoryBackFrame(int frameX = 0, int frameY = 0)
        {
            return InventoryBack.Value.Frame(horizontalFrames: InventoryBackFramesX, verticalFrames: InventoryBackFramesY, frameX: frameX, frameY: frameY);
        }

        public struct ItemDrawData
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

        [Obsolete("Vanilla now draws items differently. This method is now out of date.")]
        public static ItemDrawData GetDrawData(Item item, Vector2 position, Color? color = null, int maxSize = 32)
        {
            Main.instance.LoadItem(item.type);

            var data = new ItemDrawData
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
            Draw(spriteBatch, GetDrawData(item, position, color, maxSize));
        }
        public static void Draw(SpriteBatch spriteBatch, ItemDrawData data)
        {
            Draw(spriteBatch, data.texture, data.drawPos, data.item, data.frame, data.drawColor, data.color, data.origin, data.scale, data.scale2);
        }

        [Obsolete("Just use position.")]
        public static Vector2 InventoryItemGetCorner(Vector2 position, Rectangle itemFrame, float itemScale)
        {
            return position;
        }

        public static void DrawUIBack(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle itemFrame, float itemScale, Color color, float progress = 1f)
        {
            var backFrame = InventoryBackFrame(frameY: 0);
            int frameY = (int)(backFrame.Height * progress);
            var uiFrame = new Rectangle(0, backFrame.Height - frameY, backFrame.Width, frameY);
            position.Y += (backFrame.Height - frameY) * Main.inventoryScale;
            spriteBatch.Draw(texture, position, uiFrame, color, 0f, backFrame.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f);
        }

        void ILoadable.Load(Mod mod)
        {
            if (!Main.dedServ)
            {
                InventoryBack = ModContent.Request<Texture2D>("Aequus/Assets/UI/InventoryBack");
            }
        }

        void ILoadable.Unload()
        {
            InventoryBack = null;
        }
    }
}