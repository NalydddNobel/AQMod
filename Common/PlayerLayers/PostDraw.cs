using AQMod.Assets;
using AQMod.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace AQMod.Common.PlayerLayers
{
    public class PostDraw : PlayerLayerWrapper
    {
        public override void Draw(PlayerDrawInfo info)
        {
            int whoAmI = info.drawPlayer.whoAmI;
            var aQMod = AQMod.Instance;
            var player = info.drawPlayer;
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            if (Main.myPlayer == info.drawPlayer.whoAmI && AQPlayer.ShouldDrawOldPos(info.drawPlayer) && info.shadow == 0f && AQMod.GameWorldActive)
            {
                if (AQPlayer.oldPosVisual != null && AQPlayer.oldPosVisual.Length >= AQPlayer.oldPosLength)
                {
                    if (AQPlayer.arachnotronHeadTrail)
                    {
                        if (info.shadow == 0f)
                        {
                            var headOff = new Vector2((-info.drawPlayer.bodyFrame.Width / 2) + (float)(info.drawPlayer.width / 2), info.drawPlayer.height - info.drawPlayer.bodyFrame.Height + 10f) + info.drawPlayer.headPosition + info.headOrigin;
                            var clr = new Color(255, 255, 255, 0) * (1f - info.shadow);
                            var drawDiff = info.position - info.drawPlayer.position;
                            var texture = TextureCache.ArachnotronVisorHeadGlow.GetValue();
                            int count = aQPlayer.GetOldPosCountMaxed(AQPlayer.ARACHNOTRON_OLD_POS_LENGTH);
                            var clrMult = 1f / count;
                            for (int i = 0; i < count; i++)
                            {
                                float colorMult = 0.5f * (1f - ((float)Math.Sin((Main.GlobalTime * 8f) - (i * 0.314f)) * 0.2f));
                                var drawData = new DrawData(texture, new Vector2((int)(AQPlayer.oldPosVisual[i].X - Main.screenPosition.X), (int)(AQPlayer.oldPosVisual[i].Y - Main.screenPosition.Y)) + drawDiff + headOff, info.drawPlayer.bodyFrame, clr * (clrMult * (count - i)) * colorMult, info.drawPlayer.bodyRotation, info.bodyOrigin, 1f, info.spriteEffects, 0) { shader = info.headArmorShader };
                                Main.playerDrawData.Add(drawData);
                            }
                        }
                    }
                    if (AQPlayer.arachnotronBodyTrail)
                    {
                        var bodyOff = new Vector2((-info.drawPlayer.bodyFrame.Width / 2) + (float)(info.drawPlayer.width / 2), info.drawPlayer.height - info.drawPlayer.bodyFrame.Height + 4f) + info.drawPlayer.bodyPosition + new Vector2(info.drawPlayer.bodyFrame.Width / 2, info.drawPlayer.bodyFrame.Height / 2);
                        var clr = new Color(255, 255, 255, 0) * (1f - info.shadow);
                        var drawDiff = info.position - info.drawPlayer.position;
                        var texture = TextureCache.ArachnotronRibcageBodyGlow.GetValue();
                        int count = aQPlayer.GetOldPosCountMaxed(AQPlayer.ARACHNOTRON_OLD_POS_LENGTH);
                        if (info.shadow == 0f)
                        {
                            var clrMult = 1f / count;
                            for (int i = 0; i < count; i++)
                            {
                                float colorMult = 0.5f * (1f - ((float)Math.Sin((Main.GlobalTime * 8f) - (i * 0.314f)) * 0.2f));
                                var drawData = new DrawData(texture, new Vector2((int)(AQPlayer.oldPosVisual[i].X - Main.screenPosition.X), (int)(AQPlayer.oldPosVisual[i].Y - Main.screenPosition.Y)) + drawDiff + bodyOff, info.drawPlayer.bodyFrame, clr * (clrMult * (count - i)) * colorMult, info.drawPlayer.bodyRotation, info.bodyOrigin, 1f, info.spriteEffects, 0) { shader = info.bodyArmorShader };
                                Main.playerDrawData.Add(drawData);
                            }
                        }
                    }
                }
                if (AQPlayer.oldPosLength > 0)
                {
                    if (AQPlayer.oldPosVisual == null || AQPlayer.oldPosVisual.Length != AQPlayer.oldPosLength)
                        AQPlayer.oldPosVisual = new Vector2[AQPlayer.oldPosLength];
                    for (int i = AQPlayer.oldPosLength - 1; i > 0; i--)
                    {
                        AQPlayer.oldPosVisual[i] = AQPlayer.oldPosVisual[i - 1];
                    }
                    AQPlayer.oldPosVisual[0] = player.position;
                }
            }

            if (info.shadow == 0f && aQPlayer.blueSpheres && aQPlayer.celesteTorusOffsetsForDrawing != null)
            {
                var texture = TextureCache.GetProjectile(ModContent.ProjectileType<CelesteTorusCollider>());
                var frame = new Rectangle(0, 0, texture.Width, texture.Height);
                var orig = frame.Size() / 2f;
                for (int i = 0; i < AQPlayer.MaxCelesteTorusOrbs; i++)
                {
                    var position = aQPlayer.GetCelesteTorusPositionOffset(i);
                    float layerValue = ThreeDimensionsEffect.GetParralaxScale(1f, aQPlayer.celesteTorusOffsetsForDrawing[i].Z * AQPlayer.CELESTE_Z_MULT);
                    if (layerValue >= 1f)
                    {
                        var center = info.position + new Vector2(player.width / 2 + (int)position.X, player.height / 2 + (int)position.Y);
                        Main.playerDrawData.Add(new DrawData(texture, ThreeDimensionsEffect.GetParralaxPosition(center, aQPlayer.celesteTorusOffsetsForDrawing[i].Z * AQPlayer.CELESTE_Z_MULT) - Main.screenPosition, frame, Lighting.GetColor((int)(center.X / 16f), (int)(center.Y / 16f)), 0f, orig, ThreeDimensionsEffect.GetParralaxScale(aQPlayer.celesteTorusScale, aQPlayer.celesteTorusOffsetsForDrawing[i].Z * AQPlayer.CELESTE_Z_MULT), SpriteEffects.None, 0) { shader = aQPlayer.cCelesteTorus, ignorePlayerRotation = true });
                    }
                }
            }
        }
    }
}