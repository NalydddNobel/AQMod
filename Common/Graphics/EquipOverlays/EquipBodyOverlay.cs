using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace AQMod.Common.Graphics.PlayerEquips
{
    public class EquipBodyOverlay : EquipOverlay
    {
        public Color drawColor;
        protected static Color DefaultColor => new Color(250, 250, 250, 0);

        public EquipBodyOverlay(string path) : base(path)
        {
            drawColor = DefaultColor;
        }

        public EquipBodyOverlay(Texture2D texture) : base(texture)
        {
            drawColor = DefaultColor;
        }

        public EquipBodyOverlay(Ref<Texture2D> asset) : base(asset)
        {
            drawColor = DefaultColor;
        }

        public EquipBodyOverlay(string path, Color color) : base(path)
        {
            drawColor = color;
        }

        public EquipBodyOverlay(Texture2D texture, Color color) : base(texture)
        {
            drawColor = color;
        }

        public EquipBodyOverlay(Ref<Texture2D> asset, Color color) : base(asset)
        {
            drawColor = color;
        }

        protected void GetBasicPlayerDrawInfo(PlayerDrawInfo info, out Vector2 bodyPosition, out float opacity)
        {
            var bodyOff = new Vector2(-info.drawPlayer.bodyFrame.Width / 2 + (float)(info.drawPlayer.width / 2), info.drawPlayer.height - info.drawPlayer.bodyFrame.Height - 2f) + info.drawPlayer.bodyPosition + new Vector2(info.drawPlayer.bodyFrame.Width / 2, info.drawPlayer.bodyFrame.Height / 2);
            bodyPosition = new Vector2((int)(info.position.X - Main.screenPosition.X), (int)(info.position.Y - Main.screenPosition.Y)) + bodyOff;
            opacity = 1f - info.shadow;
        }

        public override void Draw(PlayerDrawInfo info)
        {
            GetBasicPlayerDrawInfo(info, out Vector2 bodyPosition, out float opacity);
            var clr = drawColor * opacity;
            var texture = Asset.Value;
            Main.playerDrawData.Add(new DrawData(texture, bodyPosition, info.drawPlayer.bodyFrame, clr, info.drawPlayer.bodyRotation, info.bodyOrigin, 1f, info.spriteEffects, 0) { shader = info.bodyArmorShader });
        }
    }
}