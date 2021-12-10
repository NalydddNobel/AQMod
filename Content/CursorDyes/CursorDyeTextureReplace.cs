using AQMod.Assets;
using AQMod.Assets.Cursors;
using AQMod.Common.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.CursorDyes
{
    public abstract class CursorDyeTextureReplace : CursorDye
    {
        public CursorDyeTextureReplace(Mod mod, string name) : base(mod, name)
        {
            Textures = new CursorTextureArray(Path);
        }

        protected virtual bool HasOutlines => true;
        protected CursorTextureArray Textures { get; }
        protected abstract string Path { get; }

        public override Vector2? DrawThickCursor(bool smart)
        {
            return Vector2.Zero;
        }

        public sealed override bool PreDrawCursor(Player player, AQPlayer drawingPlayer, Vector2 bonus, bool smart)
        {
            var type = smart ? CursorType.SmartCursor : CursorType.Cursor;
            if (Textures.GetCursorTexture(type, out var texture))
            {
                float scale = Main.cursorScale * 0.8f;
                bool outline = HasOutlines;
                if (outline)
                {
                    try
                    {
                        Main.spriteBatch.End();
                        BatcherMethods.UI.Begin(Main.spriteBatch, BatcherMethods.Shader);
                        var effect = EffectCache.s_OutlineColor;
                        effect.UseColor(Main.MouseBorderColor);
                        effect.UseImageSize(texture.Size());
                        effect.Apply(null);
                    }
                    catch
                    {
                        outline = false;
                    }
                }
                Main.spriteBatch.Draw(texture, new Vector2(Main.mouseX, Main.mouseY), null, Color.White, 0f, default(Vector2), scale, SpriteEffects.None, 0f);
                if (outline)
                {
                    Main.spriteBatch.End();
                    BatcherMethods.UI.Begin(Main.spriteBatch, BatcherMethods.Regular);
                }
                return true;
            }
            return false;
        }

        public override bool PreDrawCursorOverrides(Player player, AQPlayer drawingPlayer)
        {
            if (Main.cursorOverride < 0)
            {
                return false;
            }
            var type = (CursorType)Main.cursorOverride;
            if (type >= CursorType.Count)
            {
                return false;
            }
            var tea = Textures;
            if (Textures.GetCursorTexture(type, out var texture))
            {
                bool outline = HasOutlines;
                if (outline)
                {
                    try
                    {
                        Main.spriteBatch.End();
                        BatcherMethods.UI.Begin(Main.spriteBatch, BatcherMethods.Shader);
                        var effect = EffectCache.s_OutlineColor;
                        effect.UseColor(Main.MouseBorderColor);
                        effect.UseImageSize(texture.Size());
                        effect.Apply(null);
                    }
                    catch
                    {
                        outline = false;
                    }
                }

                float scale = Main.cursorScale * 0.8f;
                Main.spriteBatch.Draw(texture, new Vector2(Main.mouseX, Main.mouseY), null, Color.White, 0f, default(Vector2), scale, SpriteEffects.None, 0f);

                if (outline)
                {
                    Main.spriteBatch.End();
                    BatcherMethods.UI.Begin(Main.spriteBatch, BatcherMethods.Regular);
                }
                return true;
            }
            return false;
        }
    }
}