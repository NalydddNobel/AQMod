using AQMod.Assets;
using AQMod.Common.Graphics;
using AQMod.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.CursorDyes.Components
{
    public class CursorDyeTextureChangeComponent : ICursorDyeComponent
    {
        protected readonly string _texture;
        protected readonly Func<bool> shouldDrawOutline;

        public CursorDyeTextureChangeComponent(string texture)
        {
            _texture = texture;
            shouldDrawOutline = () => true;
        }
        public CursorDyeTextureChangeComponent(string texture, Func<bool> shouldDrawOutline)
        {
            _texture = texture;
            this.shouldDrawOutline = shouldDrawOutline;
        }

        void ICursorDyeComponent.OnUpdateUI()
        {
        }

        public virtual bool PreRender(bool cursorOverride, bool smart = false)
        {
            string texturePath = _texture;
            if (cursorOverride)
            {
                if (Main.cursorOverride > 0)
                {
                    texturePath += CursorDyeManager.InternalGetOverrideName(Main.cursorOverride);
                }
                else if (smart)
                {
                    texturePath += "_smart";
                }
            }
            else if (smart)
            {
                texturePath += "_smart";
            }
            if (ModContent.TextureExists(texturePath))
            {
                var texture = ModContent.GetTexture(texturePath);

                bool outline = shouldDrawOutline();
                if (outline)
                {
                    try
                    {
                        Main.spriteBatch.End();
                        BatcherMethods.UI.Begin(Main.spriteBatch, BatcherMethods.Shader);
                        var effect = LegacyEffectCache.s_OutlineColor;
                        effect.UseColor(Main.MouseBorderColor);
                        effect.UseImageSize(texture.Size());
                        effect.Apply(null);
                    }
                    catch
                    {
                    }
                }

                float scale = Main.cursorScale * 0.8f;
                Main.spriteBatch.Draw(texture, new Vector2(Main.mouseX, Main.mouseY), null, Color.White, 0f, default(Vector2), scale, SpriteEffects.None, 0f);

                if (outline)
                {
                    Main.spriteBatch.End();
                    BatcherMethods.UI.Begin(Main.spriteBatch, BatcherMethods.Regular);
                }
                else if (ModContent.TextureExists(texturePath + "_outline"))
                {
                    Main.spriteBatch.Draw(ModContent.GetTexture(texturePath + "_outline"), new Vector2(Main.mouseX, Main.mouseY), null, Main.MouseBorderColor.UseA(255), 0f, default(Vector2), scale, SpriteEffects.None, 0f);
                }
                return false;
            }
            return true;
        }

        public virtual void PostRender(bool cursorOverride, bool smart)
        {
        }
    }
}