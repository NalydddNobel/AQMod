using Aequus.Common.Items;
using Aequus.Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Content.Debug;
public class RGBKeyboardItem : ModItem {
    private class Drawer : IDebugDrawer {
        private SpriteBatch _spriteBatch;

        private Texture2D _texture;

        private bool _disposedValue;

        public Drawer(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice) {
            _spriteBatch = spriteBatch;
            _texture = new Texture2D(graphicsDevice, 4, 4);
            Color[] array = new Color[16];
            for (int i = 0; i < array.Length; i++) {
                array[i] = Color.White;
            }

            _texture.SetData(array);
        }

        public void Begin(Matrix matrix) {
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, matrix);
        }

        public void Begin() {
            _spriteBatch.Begin();
        }

        public void DrawSquare(Vector4 positionAndSize, Color color) {
            _spriteBatch.Draw(_texture, new Vector2(positionAndSize.X, positionAndSize.Y), null, color, 0f, Vector2.Zero, new Vector2(positionAndSize.Z, positionAndSize.W) / 4f, SpriteEffects.None, 1f);
        }

        public void DrawSquare(Vector2 position, Vector2 size, Color color) {
            _spriteBatch.Draw(_texture, position, null, color, 0f, Vector2.Zero, size / 4f, SpriteEffects.None, 1f);
        }

        public void DrawSquareFromCenter(Vector2 center, Vector2 size, float rotation, Color color) {
            _spriteBatch.Draw(_texture, center, null, color, rotation, new Vector2(2f, 2f), size / 4f, SpriteEffects.None, 1f);
        }

        public void DrawLine(Vector2 start, Vector2 end, float width, Color color) {
            Vector2 vector = end - start;
            float rotation = (float)Math.Atan2(vector.Y, vector.X);
            Vector2 vector2 = new Vector2(vector.Length(), width);
            _spriteBatch.Draw(_texture, start, null, color, rotation, new Vector2(0f, 2f), vector2 / 4f, SpriteEffects.None, 1f);
        }

        public void End() {
            _spriteBatch.End();
        }

        protected virtual void Dispose(bool disposing) {
            if (_disposedValue) {
                return;
            }

            if (disposing) {
                if (_spriteBatch != null) {
                    _spriteBatch.Dispose();
                    _spriteBatch = null;
                }

                if (_texture != null) {
                    _texture.Dispose();
                    _texture = null;
                }
            }

            _disposedValue = true;
        }

        public void Dispose() {
            Dispose(disposing: true);
        }
    }

    private Drawer _drawer;

    public override string Texture => AequusTextures.Gamestar.Path;
    public override bool IsLoadingEnabled(Mod mod) {
        return Aequus.DevelopmentFeatures;
    }

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 0;
    }

    public override void SetDefaults() {
        Item.DefaultToHoldUpItem();
        Item.useTime = 2;
        Item.useAnimation = 2;
        Item.color = Color.Orange;
        Item.width = 20;
        Item.height = 20;
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        if (Main.playerInventory || AequusUI.CurrentItemSlot.Context != ItemSlot.Context.HotbarItem) {
            return true;
        }
        try {
            _drawer ??= new(Main.spriteBatch, Main.instance.GraphicsDevice);
            Main.Chroma.DebugDraw(_drawer, Main.MouseScreen, 100f);
        }
        catch {
            _drawer = new(Main.spriteBatch, Main.instance.GraphicsDevice);
        }
        return true;
    }
}
