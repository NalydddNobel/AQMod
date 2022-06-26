using Aequus.Items.Accessories.Vanity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus.Graphics.PlayerRendering
{
    public class EarLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.Head);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            int ears = drawInfo.drawPlayer.Aequus().ears;
            if (ears >= Main.maxItemTypes && ModContent.RequestIfExists<Texture2D>(ItemLoader.GetItem(ears).Texture + "_Ears", out var t))
            {
                var bodyFrame = drawInfo.drawPlayer.bodyFrame;
                var headOrigin = drawInfo.headVect;
                if (drawInfo.drawPlayer.gravDir == 1f)
                {
                    bodyFrame.Height -= 4;
                }
                else
                {
                    headOrigin.Y -= 4f;
                    bodyFrame.Height -= 4;
                }

                var drawPosition = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawInfo.drawPlayer.bodyFrame.Width / 2) + (float)(drawInfo.drawPlayer.width / 2)),
                    (int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawInfo.drawPlayer.height - (float)drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.headPosition + drawInfo.headVect;

                var c = Color.White;
                if (ears == ModContent.ItemType<FishyFins>())
                {
                    c = drawInfo.drawPlayer.skinColor;
                }

                //if (AequusHelpers.debugKey)
                //{
                //    AequusHelpers.dustDebug(drawInfo.helmetOffset + drawPosition + Main.screenPosition);
                //}

                drawInfo.DrawDataCache.Add(new DrawData(t.Value, drawInfo.helmetOffset + drawPosition,
                    bodyFrame, AequusHelpers.GetColor(drawInfo.helmetOffset + drawPosition + Main.screenPosition, c), drawInfo.drawPlayer.headRotation, headOrigin, 1f, drawInfo.playerEffect, 0)
                { shader = drawInfo.cHead, });
            }
        }
    }
}