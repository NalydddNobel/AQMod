using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace AQMod.Content
{
    public static class InterfaceHookableNPC
    {
        public static void RenderUI(int closestNPC)
        {
            var texture = Tex.MeathookNPC.Texture.Value;
            var frame = texture.Frame();
            var origin = new Vector2(frame.Width / 2f, 0f);
            var drawPosition = Main.npc[closestNPC].Bottom + new Vector2(0f, 12f);
            Main.LocalPlayer.GetModPlayer<AQPlayer>().meathookTarget = closestNPC;
            Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, frame, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}