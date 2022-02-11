using AQMod.Common.Graphics.PlayerEquips;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace AQMod.Content.Players
{
    public class EquipWingsOverlay : EquipOverlay
    {
        public Color drawColor;
        protected static Color DefaultColor => new Color(250, 250, 250, 0);

        public EquipWingsOverlay(string path) : base(path)
        {
            drawColor = DefaultColor;
        }

        public EquipWingsOverlay(Texture2D texture) : base(texture)
        {
            drawColor = DefaultColor;
        }

        public EquipWingsOverlay(Ref<Texture2D> asset) : base(asset)
        {
            drawColor = DefaultColor;
        }

        public EquipWingsOverlay(string path, Color color) : base(path)
        {
            drawColor = color;
        }

        public EquipWingsOverlay(Texture2D texture, Color color) : base(texture)
        {
            drawColor = color;
        }

        public EquipWingsOverlay(Ref<Texture2D> asset, Color color) : base(asset)
        {
            drawColor = color;
        }

        protected void GetBasicPlayerDrawInfo(PlayerDrawInfo info, out Vector2 wingPosition, out Rectangle frame, out SpriteEffects effects, out float opacity, int wingFrames = 4)
        {
            frame = new Rectangle(0, Main.wingsTexture[info.drawPlayer.wings].Height / wingFrames * info.drawPlayer.wingFrame, Main.wingsTexture[info.drawPlayer.wings].Width, Main.wingsTexture[info.drawPlayer.wings].Height / wingFrames);
            effects = (info.drawPlayer.gravDir != 1f) ? ((info.drawPlayer.direction == 1) ? SpriteEffects.FlipVertically : (SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically)) : ((info.drawPlayer.direction != 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            wingPosition = new Vector2((int)(info.position.X + info.drawPlayer.width / 2f - Main.screenPosition.X),
                (int)(info.position.Y + info.drawPlayer.height / 2.0 - Main.screenPosition.Y) + info.drawPlayer.mount.PlayerOffset * 0.5f - info.drawPlayer.gravDir * 17f);
            opacity = 1f - info.shadow;
        }

        public override void Draw(PlayerDrawInfo info)
        {
            GetBasicPlayerDrawInfo(info, out Vector2 wingPosition, out var frame, out var effects, out float opacity, 4);
            var clr = drawColor * opacity;
            var texture = Asset.Value;
            Main.playerDrawData.Add(new DrawData(texture, wingPosition, frame, clr, info.drawPlayer.bodyRotation, frame.Size() / 2f, 1f, effects, 0) { shader = info.wingShader });
        }
    }
}