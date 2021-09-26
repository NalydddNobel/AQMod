using AQMod.Assets.Textures;
using AQMod.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace AQMod.Assets.PlayerLayers
{
    public class PreDraw : PlayerLayerWrapper
    {
        public void Draw(PlayerDrawInfo info)
        {
            int whoAmI = info.drawPlayer.whoAmI;
            var player = info.drawPlayer;
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            var drawingPlayer = player.GetModPlayer<AQVisualsPlayer>();
            if (info.shadow == 0f && aQPlayer.blueSpheres && drawingPlayer.celesteTorusPositions != null)
            {
                var texture = SpriteUtils.Textures.Extras[ExtraID.CelesteTorusProjectile];
                var frame = new Rectangle(0, 0, texture.Width, texture.Height);
                var orig = frame.Size() / 2f;
                for (int i = 0; i < AQPlayer.MaxCelesteTorusOrbs; i++)
                {
                    var position = aQPlayer.GetCelesteTorusPositionOffset(i);
                    float layerValue = Parralax.GetParralaxScale(1f, drawingPlayer.celesteTorusPositions[i].Z * AQVisualsPlayer.CELESTE_Z_MULT);
                    if (layerValue < 1f)
                    {
                        var center = info.position + new Vector2(player.width / 2 + (int)position.X, player.height / 2 + (int)position.Y);
                        Main.playerDrawData.Add(new DrawData(texture, Parralax.GetParralaxPosition(center, drawingPlayer.celesteTorusPositions[i].Z * AQVisualsPlayer.CELESTE_Z_MULT) - Main.screenPosition, frame, Lighting.GetColor((int)(center.X / 16f), (int)(center.Y / 16f)), 0f, orig, Parralax.GetParralaxScale(aQPlayer.celesteTorusScale, drawingPlayer.celesteTorusPositions[i].Z * AQVisualsPlayer.CELESTE_Z_MULT), SpriteEffects.None, 0) { shader = drawingPlayer.cCelesteTorus, ignorePlayerRotation = true });
                    }
                }
            }
        }

        public override Action<PlayerDrawInfo> Apply()
        {
            return Draw;
        }
    }
}