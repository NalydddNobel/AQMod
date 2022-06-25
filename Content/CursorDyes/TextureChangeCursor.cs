using Aequus.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.CursorDyes
{
    public class TextureChangeCursor : ICursorDye
    {
        protected string Texture;
        public int Type { get; set; }

        public TextureChangeCursor(string texture)
        {
            Texture = texture;
        }

        bool ICursorDye.DrawThickCursor(ref Vector2 bonus, ref bool smart)
        {
            return false;
        }

        bool ICursorDye.PreDrawCursor(ref Vector2 bonus, ref bool smart)
        {
            string texture = Texture;
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
            float scale = Main.cursorScale * 0.8f;

            var textureAsset = ModContent.Request<Texture2D>(texture);
            if (ModContent.RequestIfExists<Texture2D>(texture + "_outline", out var outline))
            {
                Main.spriteBatch.Draw(outline.Value, new Vector2(Main.mouseX, Main.mouseY), null, Main.MouseBorderColor.UseA(255), 0f, default(Vector2), scale, SpriteEffects.None, 0f);
            }
            else
            {
                Main.spriteBatch.End();
                Begin.UI.Begin(Main.spriteBatch, Begin.Shader);
                var d = new DrawData(textureAsset.Value, new Vector2(Main.mouseX, Main.mouseY), null, Color.White, 0f, default(Vector2), scale, SpriteEffects.None, 0);
                d.color = Main.MouseBorderColor.UseA(255);
                GameShaders.Armor.Apply(ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex, null, d);

                foreach (var v in AequusHelpers.CircularVector(4))
                {
                    var d2 = d;
                    d2.position += v * 2f;
                    d2.Draw(Main.spriteBatch);
                }

                Main.spriteBatch.End();
                Begin.UI.Begin(Main.spriteBatch, Begin.Regular);
            }

            Main.spriteBatch.Draw(textureAsset.Value, new Vector2(Main.mouseX, Main.mouseY), null, Color.White, 0f, default(Vector2), scale, SpriteEffects.None, 0f);
            return false;
        }


        //public virtual bool PreRender(bool cursorOverride, bool smart = false)
        //{
        //    string texturePath = Texture;
        //    if (cursorOverride)
        //    {
        //        if (Main.cursorOverride > 0)
        //        {
        //            texturePath += CursorDyeManager.InternalGetOverrideName(Main.cursorOverride);
        //        }
        //        else if (smart)
        //        {
        //            texturePath += "_smart";
        //        }
        //    }
        //    else if (smart)
        //    {
        //        texturePath += "_smart";
        //    }
        //    if (ModContent.TextureExists(texturePath))
        //    {
        //        var texture = ModContent.GetTexture(texturePath);

        //        bool outline = shouldDrawOutline();
        //        if (outline)
        //        {
        //            try
        //            {
        //                Main.spriteBatch.End();
        //                BatcherMethods.UI.Begin(Main.spriteBatch, BatcherMethods.Shader);
        //                var effect = LegacyEffectCache.s_OutlineColor;
        //                effect.UseColor(Main.MouseBorderColor);
        //                effect.UseImageSize(texture.Size());
        //                effect.Apply(null);
        //            }
        //            catch
        //            {
        //            }
        //        }

        //        float scale = Main.cursorScale * 0.8f;
        //        Main.spriteBatch.Draw(texture, new Vector2(Main.mouseX, Main.mouseY), null, Color.White, 0f, default(Vector2), scale, SpriteEffects.None, 0f);

        //        if (outline)
        //        {
        //            Main.spriteBatch.End();
        //            BatcherMethods.UI.Begin(Main.spriteBatch, BatcherMethods.Regular);
        //        }
        //        else if (ModContent.TextureExists(texturePath + "_outline"))
        //        {
        //            Main.spriteBatch.Draw(ModContent.GetTexture(texturePath + "_outline"), new Vector2(Main.mouseX, Main.mouseY), null, Main.MouseBorderColor.UseA(255), 0f, default(Vector2), scale, SpriteEffects.None, 0f);
        //        }
        //        return false;
        //    }
        //    return true;
        //}
    }
}