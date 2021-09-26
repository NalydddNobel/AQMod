using AQMod.Assets.Textures;
using AQMod.Common;
using AQMod.Common.CursorDyes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.CursorDyes
{
    public sealed class CursorDyeSword : CursorDye
    {
        public CursorDyeSword(Mod mod, string name) : base(mod, name)
        {
        }

        public override Vector2? DrawThickCursor(bool smart)
        {
            return Vector2.Zero;
        }

        public override bool PreDrawCursor(Player player, AQVisualsPlayer drawingPlayer, Vector2 bonus, bool smart)
        {
            Texture2D texture = smart ? SpriteUtils.Textures.Extras[ExtraID.PickaxeCursor] : SpriteUtils.Textures.Extras[ExtraID.SwordCursor];
            Main.spriteBatch.Draw(texture, new Vector2(Main.mouseX, Main.mouseY), null, Color.White, 0f, default(Vector2), Main.cursorScale, SpriteEffects.None, 0f);
            return true;
        }

        public override bool PreDrawCursorOverrides(Player player, AQVisualsPlayer drawingPlayer)
        {
            switch (Main.cursorOverride)
            {
                case 2:
                {
                    Main.spriteBatch.Draw(SpriteUtils.Textures.Extras[ExtraID.CompassCursor], new Vector2(Main.mouseX, Main.mouseY), null, Color.White, 0f, default(Vector2), Main.cursorScale, SpriteEffects.None, 0f);
                    return true;
                }

                case 3:
                {
                    Main.spriteBatch.Draw(SpriteUtils.Textures.Extras[ExtraID.DiamondCursor], new Vector2(Main.mouseX, Main.mouseY), null, Color.White, 0f, default(Vector2), Main.cursorScale, SpriteEffects.None, 0f);
                    return true;
                }

                case 6:
                {
                    Main.spriteBatch.Draw(SpriteUtils.Textures.Extras[ExtraID.BundleCursor], new Vector2(Main.mouseX, Main.mouseY), null, Color.White, 0f, default(Vector2), Main.cursorScale, SpriteEffects.None, 0f);
                    return true;
                }
            }
            return false;
        }
    }
}