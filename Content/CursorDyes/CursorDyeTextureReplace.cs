using AQMod.Assets;
using AQMod.Common;
using AQMod.Common.Config;
using AQMod.Common.Utilities;
using AQMod.Effects;
using AQMod.Effects.Batchers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace AQMod.Content.CursorDyes
{
    public abstract class CursorDyeTextureReplace : CursorDye
    {
        public CursorDyeTextureReplace(Mod mod, string name) : base(mod, name)
        {
        }

        protected virtual bool HasOutlines => true;
        protected abstract TEA<CursorType> TextureEnumeratorArray { get; }

        public override Vector2? DrawThickCursor(bool smart)
        {
            return Vector2.Zero;
        }

        public sealed override bool PreDrawCursor(Player player, AQPlayer drawingPlayer, Vector2 bonus, bool smart)
        {
            var type = smart ? CursorType.SmartCursor : CursorType.Cursor;
            var tea = TextureEnumeratorArray;
            if (tea.ContainsTexture(type, true))
            {
                var texture = tea[type];
                float scale = Main.cursorScale * 0.8f;
                bool outline = HasOutlines && AQConfigClient.Instance.OutlineShader;
                if (outline)
                {
                    try
                    {
                        Main.spriteBatch.End();
                        BatcherTypes.StartShaderBatch_UI(Main.spriteBatch);
                        var effect = GameShaders.Misc["AQMod:OutlineColor"];
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
                    BatcherTypes.StartBatch_UI(Main.spriteBatch);
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
            var tea = TextureEnumeratorArray;
            if (tea.ContainsTexture(type, true))
            {
                var texture = tea[type];
                bool outline = AQConfigClient.Instance.OutlineShader;
                if (outline)
                {
                    try
                    {
                        Main.spriteBatch.End();
                        BatcherTypes.StartShaderBatch_UI(Main.spriteBatch);
                        var effect = GameShaders.Misc["AQMod:OutlineColor"];
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
                    BatcherTypes.StartBatch_UI(Main.spriteBatch);
                }
                return true;
            }
            return false;
        }
    }
}