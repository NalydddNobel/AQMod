using Aequus.Items.Accessories.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus.Graphics.PlayerLayers
{
    public class HeadLayers : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.Head);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            var aequus = drawInfo.drawPlayer.Aequus();
            if (aequus.equippedHat >= Main.maxItemTypes && ModContent.RequestIfExists<Texture2D>(ItemLoader.GetItem(aequus.equippedHat).Texture + "_Hat", out var hatTexture))
            {
                DrawHeadTexture(ref drawInfo, hatTexture.Value, (v) => v != Vector2.Zero ? AequusHelpers.GetColor(v) : Color.White, aequus.cHat);
            }

            if (aequus.equippedEars >= Main.maxItemTypes && ModContent.RequestIfExists<Texture2D>(ItemLoader.GetItem(aequus.equippedEars).Texture + "_Ears", out var earsTexture))
            {
                var c = Color.White;
                if (aequus.equippedEars == ModContent.ItemType<FishyFins>())
                {
                    c = drawInfo.drawPlayer.skinColor;
                }
                DrawHeadTexture(ref drawInfo, earsTexture.Value, (v) => v != Vector2.Zero ? AequusHelpers.GetColor(v, c) : c, aequus.cEars);
            }

            if (aequus.equippedMask >= Main.maxItemTypes && ModContent.RequestIfExists<Texture2D>(ItemLoader.GetItem(aequus.equippedMask).Texture + "_Mask", out var maskTexture))
            {
                DrawHeadTexture(ref drawInfo, maskTexture.Value, (v) => v != Vector2.Zero ? AequusHelpers.GetColor(v) : Color.White, aequus.cMask);
            }

            if (aequus.equippedEyes >= Main.maxItemTypes && ModContent.RequestIfExists<Texture2D>(ItemLoader.GetItem(aequus.equippedEyes).Texture + "_Eyes", out var foreHeadTexture))
            {
                DrawHeadTexture(ref drawInfo, foreHeadTexture.Value, (v) => v != Vector2.Zero ? AequusHelpers.GetColor(v) : Color.White, aequus.cEyes);
            }
        }

        public void DrawHeadTexture(ref PlayerDrawSet drawInfo, Texture2D texture, Func<Vector2, Color> getColor, int shader)
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

            var drawPosition = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - drawInfo.drawPlayer.bodyFrame.Width / 2 + drawInfo.drawPlayer.width / 2),
                (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.headPosition + drawInfo.headVect;

            var lightingPosition = drawInfo.headOnlyRender ? Vector2.Zero : (drawInfo.helmetOffset + drawPosition + Main.screenPosition);
            drawInfo.DrawDataCache.Add(new DrawData(texture, drawInfo.helmetOffset + drawPosition,
                bodyFrame, getColor(lightingPosition) * (1f - drawInfo.shadow), drawInfo.drawPlayer.headRotation, headOrigin, 1f, drawInfo.playerEffect, 0)
            { shader = shader, });
        }
    }
}