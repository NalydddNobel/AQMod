using AQMod.Assets.Enumerators;
using AQMod.Assets.Textures;
using AQMod.Common;
using AQMod.Common.Config;
using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace AQMod.Assets.PlayerLayers
{
    public class PostDraw : PlayerLayerWrapper
    {
        public void Draw(PlayerDrawInfo info)
        {
            int whoAmI = info.drawPlayer.whoAmI;
            var aQMod = AQMod.Instance;
            var player = info.drawPlayer;
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            var drawingPlayer = player.GetModPlayer<GraphicsPlayer>();
            switch (drawingPlayer.specialHead)
            {
                case SpecialHeadID.ArachnotronVisor:
                {
                    if (info.drawPlayer.mount.Active || info.drawPlayer.frozen || info.drawPlayer.stoned || drawingPlayer.mask >= 0 || drawingPlayer.oldPosVisual == null || drawingPlayer.oldPosVisual.Length < drawingPlayer.oldPosLength)
                        break;
                    var headOff = new Vector2(-info.drawPlayer.bodyFrame.Width / 2 + (float)(info.drawPlayer.width / 2), info.drawPlayer.height - info.drawPlayer.bodyFrame.Height + 10f) + info.drawPlayer.headPosition + info.headOrigin;
                    var clr = new Color(255, 255, 255, 0) * (1f - info.shadow);
                    var drawDiff = info.position - info.drawPlayer.position;
                    var texture = DrawUtils.Textures.Extras[ExtraID.ArachnotronVisorGlowmask];
                    int count = drawingPlayer.GetOldPosCountMaxed(GraphicsPlayer.ARACHNOTRON_OLD_POS_LENGTH);
                    if (info.shadow == 0f)
                    {
                        var clrMult = 1f / count;
                        for (int i = 0; i < count; i++)
                        {
                            var drawData = new DrawData(texture, new Vector2((int)(drawingPlayer.oldPosVisual[i].X - Main.screenPosition.X), (int)(drawingPlayer.oldPosVisual[i].Y - Main.screenPosition.Y)) + drawDiff + headOff, info.drawPlayer.bodyFrame, clr * (clrMult * (count - i)), info.drawPlayer.bodyRotation, info.bodyOrigin, 1f, info.spriteEffects, 0) { shader = info.headArmorShader };
                            Main.playerDrawData.Add(drawData);
                        }
                    }
                    if (AQConfigClient.Instance.EffectQuality >= 1f)
                    {
                        int frame = info.drawPlayer.bodyFrame.Y / GraphicsPlayer.FRAME_HEIGHT;
                        texture = DrawUtils.Textures.Lights[LightID.Spotlight30x30];
                        var orig = texture.Size() / 2f;
                        clr = new Color(128, 20, 20, 0) * (1f - info.shadow);
                        var drawPos = new Vector2((int)(info.position.X - Main.screenPosition.X), (int)(info.position.Y - Main.screenPosition.Y) - 12f) + headOff;
                        if (info.drawPlayer.direction == 1)
                        {
                            drawPos += new Vector2(3f, 3f + Main.OffsetsPlayerHeadgear[frame].Y);
                        }
                        else
                        {
                            drawPos += new Vector2(-3f, 3f + Main.OffsetsPlayerHeadgear[frame].Y);
                        }
                        if (info.drawPlayer.gravDir == -1)
                            drawPos.Y -= 8f + Main.OffsetsPlayerHeadgear[frame].Y * 2f;
                        if (count > 0 && (drawingPlayer.oldPosVisual[count - 1] - info.drawPlayer.position).Length() < 4f)
                        {
                            var value = (float)Math.Sin(drawingPlayer.arachnotronGlowTimer * MathHelper.TwoPi * 0.01f);
                            float lerpValue = (value + 1f) / 2f;
                            var scale = 0.33f + (value + 0.5f) * 0.066f;
                            var clr2 = Color.Lerp(clr, new Color(180, 50, 50, 0) * (1f - info.shadow), lerpValue);
                            var drawData = new DrawData(texture, drawPos, null, clr2, 0f, orig, new Vector2(scale, scale * 1.5f), SpriteEffects.None, 0) { shader = info.bodyArmorShader };
                            Main.playerDrawData.Add(drawData);
                            drawData = new DrawData(texture, drawPos + new Vector2(4, 0f), null, clr2, 0f, orig, scale * 0.5f, SpriteEffects.None, 0) { shader = info.bodyArmorShader };
                            Main.playerDrawData.Add(drawData);
                            drawData = new DrawData(texture, drawPos + new Vector2(-4, 0f), null, clr2, 0f, orig, scale * 0.5f, SpriteEffects.None, 0) { shader = info.bodyArmorShader };
                            Main.playerDrawData.Add(drawData);
                        }
                        else
                        {
                            var drawData = new DrawData(texture, drawPos, null, clr, 0f, orig, new Vector2(0.33f, 0.495f), SpriteEffects.None, 0) { shader = info.bodyArmorShader };
                            Main.playerDrawData.Add(drawData);
                            drawData = new DrawData(texture, drawPos + new Vector2(4, 0f), null, clr * 0.5f, 0f, orig, 0.165f, SpriteEffects.None, 0) { shader = info.bodyArmorShader };
                            Main.playerDrawData.Add(drawData);
                            drawData = new DrawData(texture, drawPos + new Vector2(-4, 0f), null, clr * 0.5f, 0f, orig, 0.165f, SpriteEffects.None, 0) { shader = info.bodyArmorShader };
                            Main.playerDrawData.Add(drawData);
                        }
                    }
                }
                break;
            }
            switch (drawingPlayer.specialBody)
            {
                case SpecialBodyID.ArachnotronRibcage:
                {
                    if (info.drawPlayer.mount.Active || info.drawPlayer.frozen || info.drawPlayer.stoned || drawingPlayer.oldPosVisual == null || drawingPlayer.oldPosVisual.Length < drawingPlayer.oldPosLength)
                        break;
                    var bodyOff = new Vector2(-info.drawPlayer.bodyFrame.Width / 2 + (float)(info.drawPlayer.width / 2), info.drawPlayer.height - info.drawPlayer.bodyFrame.Height + 4f) + info.drawPlayer.bodyPosition + new Vector2(info.drawPlayer.bodyFrame.Width / 2, info.drawPlayer.bodyFrame.Height / 2);
                    var clr = new Color(255, 255, 255, 0) * (1f - info.shadow);
                    var drawDiff = info.position - info.drawPlayer.position;
                    var texture = DrawUtils.Textures.Extras[ExtraID.ArachnotronRibcageGlowmask];
                    int count = drawingPlayer.GetOldPosCountMaxed(GraphicsPlayer.ARACHNOTRON_OLD_POS_LENGTH);
                    if (info.shadow == 0f)
                    {
                        var clrMult = 1f / count;
                        for (int i = 0; i < count; i++)
                        {
                            var drawData = new DrawData(texture, new Vector2((int)(drawingPlayer.oldPosVisual[i].X - Main.screenPosition.X), (int)(drawingPlayer.oldPosVisual[i].Y - Main.screenPosition.Y)) + drawDiff + bodyOff, info.drawPlayer.bodyFrame, clr * (clrMult * (count - i)), info.drawPlayer.bodyRotation, info.bodyOrigin, 1f, info.spriteEffects, 0) { shader = info.bodyArmorShader };
                            Main.playerDrawData.Add(drawData);
                        }
                    }
                    if (ModContent.GetInstance<AQConfigClient>().EffectQuality >= 1f)
                    {
                        int frame = info.drawPlayer.bodyFrame.Y / GraphicsPlayer.FRAME_HEIGHT;
                        texture = DrawUtils.Textures.Lights[LightID.Spotlight30x30];
                        var orig = texture.Size() / 2f;
                        clr = new Color(128, 20, 20, 0) * (1f - info.shadow);
                        var drawPos = new Vector2((int)(info.position.X - Main.screenPosition.X), (int)(info.position.Y - Main.screenPosition.Y)) + bodyOff;
                        if (info.drawPlayer.direction == 1)
                        {
                            drawPos += new Vector2(3f, 3f + Main.OffsetsPlayerHeadgear[frame].Y);
                        }
                        else
                        {
                            drawPos += new Vector2(-3f, 3f + Main.OffsetsPlayerHeadgear[frame].Y);
                        }
                        if (info.drawPlayer.gravDir == -1)
                            drawPos.Y -= 8f + Main.OffsetsPlayerHeadgear[frame].Y * 2f;
                        if (count > 0 && (drawingPlayer.oldPosVisual[count - 1] - info.drawPlayer.position).Length() < 4f)
                        {
                            var value = (float)Math.Sin(drawingPlayer.arachnotronGlowTimer * MathHelper.TwoPi * 0.01f);
                            float lerpValue = (value + 1f) / 2f;
                            var drawData = new DrawData(texture, drawPos, null, Color.Lerp(clr, new Color(180, 50, 50, 0) * (1f - info.shadow), lerpValue), 0f, orig, 0.33f + (value + 0.5f) * 0.066f, SpriteEffects.None, 0) { shader = info.bodyArmorShader };
                            Main.playerDrawData.Add(drawData);
                        }
                        else
                        {
                            var drawData = new DrawData(texture, drawPos, null, clr, 0f, orig, 0.33f, SpriteEffects.None, 0) { shader = info.bodyArmorShader };
                            Main.playerDrawData.Add(drawData);
                        }
                    }
                }
                break;
            }
            if (info.shadow == 0f && aQPlayer.blueSpheres && drawingPlayer.celesteTorusPositions != null)
            {
                var texture = DrawUtils.Textures.Extras[ExtraID.CelesteTorusProjectile];
                var frame = new Rectangle(0, 0, texture.Width, texture.Height);
                var orig = frame.Size() / 2f;
                for (int i = 0; i < AQPlayer.MaxCelesteTorusOrbs; i++)
                {
                    var position = aQPlayer.GetCelesteTorusPositionOffset(i);
                    float layerValue = Parralax.GetParralaxScale(1f, drawingPlayer.celesteTorusPositions[i].Z * GraphicsPlayer.CELESTE_Z_MULT);
                    if (layerValue >= 1f)
                    {
                        var center = info.position + new Vector2(player.width / 2 + (int)position.X, player.height / 2 + (int)position.Y);
                        Main.playerDrawData.Add(new DrawData(texture, Parralax.GetParralaxPosition(center, drawingPlayer.celesteTorusPositions[i].Z * GraphicsPlayer.CELESTE_Z_MULT) - Main.screenPosition, frame, Lighting.GetColor((int)(center.X / 16f), (int)(center.Y / 16f)), 0f, orig, Parralax.GetParralaxScale(aQPlayer.celesteTorusScale, drawingPlayer.celesteTorusPositions[i].Z * GraphicsPlayer.CELESTE_Z_MULT), SpriteEffects.None, 0) { shader = drawingPlayer.cCelesteTorus, ignorePlayerRotation = true });
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