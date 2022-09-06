using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.UI
{
    public class OmniPaintUI : ILoadable
    {
        public struct CoatingUIElement
        {
            public int itemID;
            public Asset<Texture2D> texture;

            public CoatingUIElement(int itemID, Asset<Texture2D> texture)
            {
                this.itemID = itemID;
                this.texture = texture;
            }
        }

        public static OmniPaintUI Instance { get; private set; }

        public bool Enabled;
        public bool IsVisible => Enabled && !Main.playerInventory && IsPaintbrush.Contains(Main.LocalPlayer.HeldItem.type);
        public Dictionary<byte, int> PaintToItemID { get; set; }
        public List<CoatingUIElement> Coatings { get; set; }
        public HashSet<int> IsPaintbrush { get; private set; }

        void ILoadable.Load(Mod mod)
        {
            Instance = this;
            PaintToItemID = new Dictionary<byte, int>();
            IsPaintbrush = new HashSet<int>()
            {
                ItemID.Paintbrush,
                ItemID.PaintRoller,
                ItemID.SpectrePaintbrush,
                ItemID.SpectrePaintRoller,
            };
            Coatings = new List<CoatingUIElement>();
        }

        void ILoadable.Unload()
        {
            Instance = null;
            PaintToItemID?.Clear();
            PaintToItemID = null;
            IsPaintbrush?.Clear();
            IsPaintbrush = null;
            Coatings?.Clear();
            Coatings = null;
        }

        public void Render(SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            float scale = 1.25f;
            var texture = ModContent.Request<Texture2D>(Aequus.AssetsPath + "UI/PainterIcons").Value;
            var frame = texture.Frame(horizontalFrames: 3, verticalFrames: 2, 0, 0, -2, -2);
            var originPoint = new Vector2(470f, 20f);
            var bg = new Rectangle((int)originPoint.X - 6, (int)originPoint.Y - 6, (frame.Width + 2) * 8 + 16, (frame.Height - 2) * 5 + 16).MultiplyWH(scale);
            Utils.DrawInvBG(spriteBatch, bg);
            if (bg.Contains(Main.mouseX, Main.mouseY))
                Main.LocalPlayer.mouseInterface = true;

            Utils.DrawSplicedPanel(spriteBatch, TextureAssets.InventoryBack18.Value, bg.X + 2, bg.Y + bg.Height / 5 * 4 - 4, bg.Width - 48, bg.Height / 5 + 4, 10, 10, 10, 10, (AequusUI.InventoryBackColor * 0.75f).UseA(255));

            byte paintIDToShow = 0;
            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    byte paint = (byte)(i + j * 8 + 1);
                    if (paint >= 32)
                        break;
                    var position = originPoint + new Vector2((frame.Width + 2) * i, (frame.Height - 2) * j) * scale;
                    if (paintIDToShow == 0 && new Rectangle((int)position.X - 3, (int)position.Y - 3, frame.Width + 8, frame.Height + 6).Contains(Main.mouseX, Main.mouseY))
                    {
                        spriteBatch.Draw(texture, position, frame.Frame(0, 1, -2, 0), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                        paintIDToShow = paint;
                        if (Main.mouseLeft && Main.mouseLeftRelease)
                        {
                            SoundEngine.PlaySound(SoundID.MenuTick);
                            Main.LocalPlayer.Aequus().omniPaint = paint;
                        }
                    }

                    DrawPaintBucket(spriteBatch, texture, position, frame, scale, paint);
                }
            }
            DrawPaintBucket(spriteBatch, texture, new Vector2(originPoint.X + 2, bg.Y + bg.Height / 5 * 4 - 4 + 2f), frame, scale, Main.LocalPlayer.Aequus().omniPaint);

            Utils.DrawSplicedPanel(spriteBatch, TextureAssets.InventoryBack18.Value, bg.X + bg.Width - 44, bg.Y + bg.Height / 5 * 4 - 4 - 20, 42, 44, 10, 10, 10, 10, (AequusUI.InventoryBackColor * 0.75f).UseA(255));

            if (paintIDToShow > 0)
            {
                if (!PaintToItemID.TryGetValue(paintIDToShow, out int itemType))
                {
                    itemType = TryFindMatchingPaintID(paintIDToShow);
                    PaintToItemID.Add(paintIDToShow, itemType);
                }

                if (itemType > 0)
                {
                    Main.instance.LoadItem(itemType);
                    var itemTexture = TextureAssets.Item[itemType].Value;

                    var drawCoords = new Vector2(bg.X + bg.Width - 10, bg.Y + bg.Height - 12);
                    var origin = new Vector2(itemTexture.Width, itemTexture.Height);
                    spriteBatch.Draw(itemTexture, drawCoords + new Vector2(2f, 2f) * scale, null, Color.Black * 0.5f, 0f, origin, scale, SpriteEffects.None, 0f);
                    spriteBatch.Draw(itemTexture, drawCoords, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
                }
            }
        }

        private void DrawPaintBucket(SpriteBatch spriteBatch, Texture2D textureCache, Vector2 position, Rectangle frame, float scale, byte paintColor)
        {
            spriteBatch.Draw(textureCache, position, frame, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(textureCache, position, frame.Frame(1, 0, -2, -2), WorldGen.paintColor(paintColor), 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        private int TryFindMatchingPaintID(byte paintID)
        {
            for (int i = ItemID.RedPaint; i < ItemLoader.ItemCount; i++)
            {
                if (ContentSamples.ItemsByType[i].paint == paintID)
                {
                    return i;
                }
            }
            return 0;
        }
    }
}