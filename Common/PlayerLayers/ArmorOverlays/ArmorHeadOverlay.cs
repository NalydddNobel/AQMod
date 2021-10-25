using AQMod.Assets;
using AQMod.Assets.ArmorOverlays;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace AQMod.Common.PlayerLayers.ArmorOverlays
{
    public class ArmorHeadOverlay : ArmorOverlay
    {
        public Color drawColor;
        protected static Color DefaultColor => new Color(250, 250, 250, 0);

        public ArmorHeadOverlay(string path) : base(path)
        {
            drawColor = DefaultColor;
        }

        public ArmorHeadOverlay(TextureAsset texture) : base(texture)
        {
            drawColor = DefaultColor;
        }

        public ArmorHeadOverlay(string path, Color color) : base(path)
        {
            drawColor = color;
        }

        public ArmorHeadOverlay(TextureAsset texture, Color color) : base(texture)
        {
            drawColor = color;
        }

        protected void GetBasicPlayerDrawInfo(PlayerDrawInfo info, out Vector2 headPosition, out float opacity)
        {
            headPosition = new Vector2((int)(info.position.X - Main.screenPosition.X + (-info.drawPlayer.bodyFrame.Width / 2) + (info.drawPlayer.width / 2)), (int)(info.position.Y - Main.screenPosition.Y + info.drawPlayer.height - info.drawPlayer.bodyFrame.Height + 4f)) + info.drawPlayer.headPosition + info.headOrigin;
            opacity = 1f - info.shadow;
        }

        public override void Draw(PlayerDrawInfo info)
        {
            GetBasicPlayerDrawInfo(info, out Vector2 headPosition, out float opacity);
            var clr = drawColor * opacity;
            var texture = Texture.GetValue();
            Main.playerDrawData.Add(new DrawData(texture, headPosition, info.drawPlayer.bodyFrame, clr, info.drawPlayer.headRotation, info.headOrigin, 1f, info.spriteEffects, 0) { shader = info.headArmorShader });
        }
    }
}