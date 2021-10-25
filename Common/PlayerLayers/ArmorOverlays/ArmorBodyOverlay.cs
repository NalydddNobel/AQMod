using AQMod.Assets;
using AQMod.Assets.ArmorOverlays;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace AQMod.Common.PlayerLayers.ArmorOverlays
{
    public class ArmorBodyOverlay : ArmorOverlay
    {
        public Color drawColor;
        protected static Color DefaultColor => new Color(250, 250, 250, 0);

        public ArmorBodyOverlay(string path) : base(path)
        {
            drawColor = DefaultColor;
        }

        public ArmorBodyOverlay(TextureAsset texture) : base(texture)
        {
            drawColor = DefaultColor;
        }

        public ArmorBodyOverlay(string path, Color color) : base(path)
        {
            drawColor = color;
        }

        public ArmorBodyOverlay(TextureAsset texture, Color color) : base(texture)
        {
            drawColor = color;
        }

        protected void GetBasicPlayerDrawInfo(PlayerDrawInfo info, out Vector2 bodyPosition, out float opacity)
        {
            var bodyOff = new Vector2((-info.drawPlayer.bodyFrame.Width / 2) + (float)(info.drawPlayer.width / 2), info.drawPlayer.height - info.drawPlayer.bodyFrame.Height - 2f) + info.drawPlayer.bodyPosition + new Vector2(info.drawPlayer.bodyFrame.Width / 2, info.drawPlayer.bodyFrame.Height / 2);
            bodyPosition = new Vector2((int)(info.position.X - Main.screenPosition.X), (int)(info.position.Y - Main.screenPosition.Y)) + bodyOff;
            opacity = 1f - info.shadow;
        }

        public override void Draw(PlayerDrawInfo info)
        {
            GetBasicPlayerDrawInfo(info, out Vector2 bodyPosition, out float opacity);
            var clr = drawColor * opacity;
            var texture = Texture.GetValue();
            Main.playerDrawData.Add(new DrawData(texture, bodyPosition, info.drawPlayer.bodyFrame, clr, info.drawPlayer.bodyRotation, info.bodyOrigin, 1f, info.spriteEffects, 0) { shader = info.bodyArmorShader });
        }
    }
}