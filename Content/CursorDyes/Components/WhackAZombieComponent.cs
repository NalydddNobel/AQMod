using AQMod.Assets;
using AQMod.Common.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.CursorDyes.Components
{
    internal class WhackAZombieComponent : CursorDyeTextureChangeComponent
    {
        public WhackAZombieComponent() : base("AQMod/Items/Weapons/Melee/VineSwordCursor")
        {
        }

        public override bool PreRender(bool cursorOverride, bool smart = false)
        {
            string texturePath = _texture;
            if (cursorOverride)
            {
                if (Main.cursorOverride > 0)
                {
                    return true;
                }
            }
            else if (smart)
            {
                return true;
            }
            bool outline = true;
            if (AQProjectile.CountProjectiles(Main.LocalPlayer.HeldItem.shoot) > 0)
            {
                outline = false;
                texturePath += "_press";
            }
            if (ModContent.TextureExists(texturePath))
            {
                var texture = ModContent.GetTexture(texturePath);

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
                    }
                }

                float scale = Main.cursorScale * 0.8f;
                Main.spriteBatch.Draw(texture, new Vector2(Main.mouseX - 8f, Main.mouseY - 8f), null, Color.White, 0f, default(Vector2), scale, SpriteEffects.None, 0f);

                if (outline)
                {
                    Main.spriteBatch.End();
                    BatcherMethods.UI.Begin(Main.spriteBatch, BatcherMethods.Regular);
                }
                else if (ModContent.TextureExists(texturePath + "_outline"))
                {
                    Main.spriteBatch.Draw(ModContent.GetTexture(texturePath + "_outline"), new Vector2(Main.mouseX - 8f, Main.mouseY - 8f), null, Main.MouseBorderColor.UseA(255), 0f, default(Vector2), scale, SpriteEffects.None, 0f);
                }
                return false;
            }
            return true;
        }

    }
}
