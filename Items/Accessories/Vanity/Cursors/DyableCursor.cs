using Aequus.Content.CursorDyes;
using Aequus.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace Aequus.Items.Accessories.Vanity.Cursors
{
    public class DyableCursor : CursorDyeBase, ItemHooks.IUpdateItemDye
    {
        public static int cursorShaderID;

        public class DyableCursorDye : ICursorDye
        {
            public int Type { get; set; }
            private SpriteBatchCache cache;

            Color? ICursorDye.GetCursorColor() => Color.White;

            bool ICursorDye.PreDrawCursor(ref Vector2 bonus, ref bool smart)
            {
                cache = null;
                if (cursorShaderID != 0)
                {
                    cache = new SpriteBatchCache(Main.spriteBatch);
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, Main.DefaultSamplerState, DepthStencilState.None, cache.rasterizerState, null, cache.transformMatrix);
                    var loc = Main.LocalPlayer.position;
                    Main.LocalPlayer.Center = Main.MouseWorld;
                    GameShaders.Armor.Apply(cursorShaderID, Main.LocalPlayer, new DrawData(TextureAssets.Cursors[0].Value, Main.MouseScreen, TextureAssets.Cursors[0].Value.Bounds, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0));
                    Main.LocalPlayer.position = loc;
                }
                return true;
            }

            void ICursorDye.PostDrawCursor(Vector2 bonus, bool smart)
            {
                if (cache != null)
                {
                    Main.spriteBatch.End();
                    cache.Begin(Main.spriteBatch);
                    cache = null;
                }
            }
        }

        public override ICursorDye InitalizeDye()
        {
            return new DyableCursorDye();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            if (Main.myPlayer == player.whoAmI)
            {
                cursorShaderID = 0;
            }
        }

        void ItemHooks.IUpdateItemDye.UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                cursorShaderID = dyeItem.dye;
            }
        }
    }
}