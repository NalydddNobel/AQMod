using AQMod.Assets.Textures;
using AQMod.Common;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace AQMod.Assets.PlayerLayers
{
    public class PostDrawBody : PlayerLayerWrapper
    {
        public static void Draw(PlayerDrawInfo info)
        {
            var drawingPlayer = info.drawPlayer.GetModPlayer<AQVisualsPlayer>();
            switch (drawingPlayer.specialBody)
            {
                case 20:
                {
                    var bodyOff = new Vector2(-info.drawPlayer.bodyFrame.Width / 2 + (float)(info.drawPlayer.width / 2), info.drawPlayer.height - info.drawPlayer.bodyFrame.Height + 4f) + info.drawPlayer.bodyPosition + new Vector2(info.drawPlayer.bodyFrame.Width / 2, info.drawPlayer.bodyFrame.Height / 2);
                    var bodyPos = new Vector2((int)(info.position.X - Main.screenPosition.X), (int)(info.position.Y - Main.screenPosition.Y)) + bodyOff;
                    var clr = new Color(255, 255, 255, 0) * (1f - info.shadow);
                    var drawDiff = info.position - info.drawPlayer.position;
                    var texture = SpriteUtils.Textures.Extras[ExtraID.ArachnotronRibcageGlowmask];
                    var drawData = new DrawData(texture, bodyPos, info.drawPlayer.bodyFrame, clr, info.drawPlayer.bodyRotation, info.bodyOrigin, 1f, info.spriteEffects, 0) { shader = info.bodyArmorShader };
                    Main.playerDrawData.Add(drawData);
                }
                break;
            }
        }

        public override Action<PlayerDrawInfo> Apply()
        {
            return Draw;
        }
    }
}