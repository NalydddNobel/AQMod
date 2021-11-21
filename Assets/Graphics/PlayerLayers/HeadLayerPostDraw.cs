using AQMod.Common.PlayerData;
using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace AQMod.Assets.Graphics.PlayerLayers
{
    public class HeadLayerPostDraw : TempPlayerHeadLayerWrapper
    {
        protected override void Draw(PlayerHeadDrawInfo info)
        {
            var player = info.drawPlayer;
            AQPlayer aQPlayer = info.drawPlayer.GetModPlayer<AQPlayer>();
            var drawingPlayer = info.drawPlayer.GetModPlayer<AQPlayer>();
            if (drawingPlayer.mask >= 0)
            {
                var drawData = new DrawData(TextureCache.PlayerMasks[(PlayerMaskID)drawingPlayer.mask], new Vector2((int)(info.drawPlayer.position.X - Main.screenPosition.X - info.drawPlayer.bodyFrame.Width / 2 + info.drawPlayer.width / 2), (int)(info.drawPlayer.position.Y - Main.screenPosition.Y + info.drawPlayer.height - info.drawPlayer.bodyFrame.Height)) + info.drawPlayer.headPosition + info.drawOrigin, info.drawPlayer.bodyFrame, info.armorColor, info.drawPlayer.headRotation, info.drawOrigin, info.scale, info.spriteEffects, 0);
                GameShaders.Armor.Apply(drawingPlayer.cMask, player, drawData);
                drawData.Draw(Main.spriteBatch);
                Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            }
            if (drawingPlayer.headOverlay != -1)
            {
                var drawData = new DrawData(TextureCache.PlayerHeadOverlays[(PlayerHeadOverlayID)drawingPlayer.headOverlay], new Vector2((int)(info.drawPlayer.position.X - Main.screenPosition.X - info.drawPlayer.bodyFrame.Width / 2 + info.drawPlayer.width / 2), (int)(info.drawPlayer.position.Y - Main.screenPosition.Y + info.drawPlayer.height - info.drawPlayer.bodyFrame.Height)) + info.drawPlayer.headPosition + info.drawOrigin, info.drawPlayer.bodyFrame, info.armorColor, info.drawPlayer.headRotation, info.drawOrigin, info.scale, info.spriteEffects, 0);
                if ((PlayerHeadOverlayID)drawingPlayer.headOverlay == PlayerHeadOverlayID.FishyFins)
                    drawData.color = player.skinColor;
                drawData.position = new Vector2((int)drawData.position.X, (int)drawData.position.Y);
                GameShaders.Armor.Apply(drawingPlayer.cHeadOverlay, player, drawData);
                drawData.Draw(Main.spriteBatch);
                Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            }
        }
    }
}
