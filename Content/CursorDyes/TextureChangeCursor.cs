﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.CursorDyes {
    public class TextureChangeCursor : ICursorDye
    {
        protected string Texture;
        public int Type { get; set; }

        public Vector2 offset;

        public TextureChangeCursor(string texture, Vector2 offset = default(Vector2))
        {
            Texture = texture;
            offset = offset;
        }

        bool ICursorDye.DrawThickCursor(ref Vector2 bonus, ref bool smart)
        {
            return false;
        }

        bool ICursorDye.PreDrawCursor(ref Vector2 bonus, ref bool smart)
        {
            string texture = Texture;
            smart = Main.SmartCursorIsUsed;
            if (Main.cursorOverride > 0)
            {
                texture = Texture + "_" + Main.cursorOverride;
                if (!ModContent.RequestIfExists<Texture2D>(texture, out var _))
                {
                    return true;
                }
            }
            else if (smart)
            {
                texture = Texture + "_Smart";
            }
            float scale = Main.cursorScale;

            var textureAsset = ModContent.Request<Texture2D>(texture, AssetRequestMode.ImmediateLoad);
            if (ModContent.RequestIfExists<Texture2D>(texture + "_outline", out var outline, AssetRequestMode.ImmediateLoad))
            {
                Main.spriteBatch.Draw(outline.Value, new Vector2(Main.mouseX, Main.mouseY), null, Main.MouseBorderColor.UseA(255), 0f, default(Vector2), scale, SpriteEffects.None, 0f);
            }
            else
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin_UI(immediate: true);
                var d = new DrawData(textureAsset.Value, new Vector2(Main.mouseX, Main.mouseY) + offset, null, Color.White, 0f, default(Vector2), scale, SpriteEffects.None, 0)
                {
                    color = Main.MouseBorderColor.UseA(255)
                };
                GameShaders.Armor.Apply(ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex, null, d);

                foreach (var v in Helper.CircularVector(4))
                {
                    var d2 = d;
                    d2.position += v * 2f;
                    d2.Draw(Main.spriteBatch);
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin_UI(immediate: false);
            }

            Main.spriteBatch.Draw(textureAsset.Value, new Vector2(Main.mouseX, Main.mouseY), null, Color.White, 0f, default(Vector2), scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}