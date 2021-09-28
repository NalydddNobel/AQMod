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
    public class PostDrawHead : PlayerLayerWrapper
    {
        public void Draw(PlayerDrawInfo info)
        {
            var player = info.drawPlayer;
            var aQPlayer = info.drawPlayer.GetModPlayer<AQPlayer>();
            var drawingPlayer = info.drawPlayer.GetModPlayer<GraphicsPlayer>();
            float opacity = 1f - info.shadow;
            const float MagicOffsetForReversedGravity = 8f;
            int headFrame = info.drawPlayer.bodyFrame.Y / GraphicsPlayer.FRAME_HEIGHT;
            float gravityOffset = 0f;
            if (info.drawPlayer.gravDir == -1)
                gravityOffset = MagicOffsetForReversedGravity;
            switch (drawingPlayer.specialHead)
            {
                case SpecialHeadID.OmegaStariteMask:
                {
                    var headOff = new Vector2(-info.drawPlayer.bodyFrame.Width / 2 + (float)(info.drawPlayer.width / 2), info.drawPlayer.height - info.drawPlayer.bodyFrame.Height + gravityOffset + 4f) + info.drawPlayer.headPosition + info.headOrigin;
                    if (info.drawPlayer.gravDir == -1)
                        headOff.Y -= 8f;
                    var headPos = new Vector2((int)(info.position.X - Main.screenPosition.X), (int)(info.position.Y - Main.screenPosition.Y)) + headOff;
                    var clr = new Color(255, 255, 255, 0) * (1f - info.shadow);
                    var texture = DrawUtils.Textures.Glows[GlowID.OmegaStariteMask];
                    Main.playerDrawData.Add(new DrawData(texture, headPos, info.drawPlayer.bodyFrame, clr * 0.8f, info.drawPlayer.headRotation, info.headOrigin, 1f, info.spriteEffects, 0) { shader = info.headArmorShader });
                }
                break;

                case SpecialHeadID.ArachnotronVisor:
                {
                    if (player.merman || player.wereWolf)
                        break;
                    var headOff = new Vector2(-info.drawPlayer.bodyFrame.Width / 2 + (float)(info.drawPlayer.width / 2), info.drawPlayer.height - info.drawPlayer.bodyFrame.Height + gravityOffset + 4f) + info.drawPlayer.headPosition + info.headOrigin;
                    if (info.drawPlayer.gravDir == -1)
                        headOff.Y -= 8f;
                    var headPos = new Vector2((int)(info.position.X - Main.screenPosition.X), (int)(info.position.Y - Main.screenPosition.Y)) + headOff;
                    var clr = new Color(255, 255, 255, 0) * (1f - info.shadow);
                    var texture = DrawUtils.Textures.Extras[ExtraID.ArachnotronVisorGlowmask];
                    Main.playerDrawData.Add(new DrawData(texture, headPos, info.drawPlayer.bodyFrame, clr, info.drawPlayer.headRotation, info.headOrigin, 1f, info.spriteEffects, 0) { shader = info.headArmorShader });
                }
                break;
            }
            if (drawingPlayer.mask >= 0)
            {
                Vector2 position = new Vector2((int)(info.position.X - Main.screenPosition.X - info.drawPlayer.bodyFrame.Width / 2 + info.drawPlayer.width / 2), (int)(info.position.Y - Main.screenPosition.Y + info.drawPlayer.height - info.drawPlayer.bodyFrame.Height + gravityOffset)) + info.drawPlayer.headPosition + info.headOrigin;
                Color color = Lighting.GetColor((int)info.position.X / 16, (int)(info.position.Y + gravityOffset) / 16) * opacity;
                switch ((PlayerMaskID)drawingPlayer.mask)
                {
                    default:
                    {
                        Main.playerDrawData.Add(new DrawData(DrawUtils.Textures.PlayerMasks[(PlayerMaskID)drawingPlayer.mask], position, info.drawPlayer.bodyFrame, color, info.drawPlayer.headRotation, info.headOrigin, 1f, info.spriteEffects, 0) { shader = drawingPlayer.cMask, });
                    }
                    break;

                    case PlayerMaskID.CataMask:
                    {
                        if (drawingPlayer.cMask > 0)
                            drawingPlayer.cataEyeColor = new Color(100, 100, 100, 0);
                        Main.playerDrawData.Add(new DrawData(DrawUtils.Textures.PlayerMasks[PlayerMaskID.CataMask], position, info.drawPlayer.bodyFrame, color, info.drawPlayer.headRotation, info.headOrigin, 1f, info.spriteEffects, 0) { shader = drawingPlayer.cMask, });
                        if (drawingPlayer.mothmanMaskSpecial)
                        {
                            if (info.drawPlayer.headRotation == 0)
                            {
                                var texture = DrawUtils.Textures.Lights[LightID.Spotlight240x66];
                                var frame = new Rectangle(0, 0, texture.Width, texture.Height);
                                var orig = frame.Size() / 2f;
                                var scale = new Vector2((float)(Math.Sin(Main.GlobalTime * 10f) + 1f) * 0.04f + 0.2f, 0.1f);
                                var eyeGlowPos = position + new Vector2(2f * player.direction, Main.OffsetsPlayerHeadgear[headFrame].Y);
                                var eyeGlowColor = drawingPlayer.cataEyeColor;
                                var value = AQUtils.GetGrad(0.25f, 0.45f, scale.X) * 0.5f;
                                var config = ModContent.GetInstance<AQConfigClient>();
                                var colorMult = ModContent.GetInstance<AQConfigClient>().EffectIntensity * (1f - info.shadow);
                                Main.playerDrawData.Add(new DrawData(texture, eyeGlowPos, frame, eyeGlowColor * colorMult, 0f, orig, scale, info.spriteEffects, 0) { shader = drawingPlayer.cMask, });
                                Main.playerDrawData.Add(new DrawData(texture, eyeGlowPos, frame, eyeGlowColor * 0.3f * colorMult, 0f, orig, scale * (1.1f + value * 2), info.spriteEffects, 0) { shader = drawingPlayer.cMask, });
                                if (ModContent.GetInstance<AQConfigClient>().EffectQuality > 0.5f)
                                {
                                    Main.playerDrawData.Add(new DrawData(texture, eyeGlowPos, frame, eyeGlowColor * 0.35f * colorMult, MathHelper.PiOver4, orig, scale * (1f - value) * 0.75f, info.spriteEffects, 0) { shader = drawingPlayer.cMask, });
                                    Main.playerDrawData.Add(new DrawData(texture, eyeGlowPos, frame, eyeGlowColor * 0.35f * colorMult, -MathHelper.PiOver4, orig, scale * (1f - value) * 0.75f, info.spriteEffects, 0) { shader = drawingPlayer.cMask, });
                                    Main.playerDrawData.Add(new DrawData(texture, eyeGlowPos, frame, eyeGlowColor * 0.2f * colorMult, MathHelper.PiOver2, orig, scale * (1f - value), info.spriteEffects, 0) { shader = drawingPlayer.cMask, });
                                }
                                Main.playerDrawData.Add(new DrawData(texture, eyeGlowPos, frame, eyeGlowColor * colorMult, MathHelper.PiOver2, orig, scale * 0.5f, info.spriteEffects, 0) { shader = drawingPlayer.cMask, });
                                if (ModContent.GetInstance<AQConfigClient>().EffectIntensity > 1.5f && ModContent.GetInstance<AQConfigClient>().EffectQuality > 0.5f)
                                    Main.playerDrawData.Add(new DrawData(texture, eyeGlowPos, frame, eyeGlowColor * 0.15f * colorMult, 0f, orig, scale * (2f + value * 3f), info.spriteEffects, 0) { shader = drawingPlayer.cMask, });
                            }
                        }
                    }
                    break;
                }
            }
            if (drawingPlayer.headOverlay >= 0)
            {
                Vector2 position = new Vector2((int)(info.position.X - info.drawPlayer.bodyFrame.Width / 2 + info.drawPlayer.width / 2), (int)(info.position.Y + info.drawPlayer.height - info.drawPlayer.bodyFrame.Height + gravityOffset)) + info.drawPlayer.headPosition + info.headOrigin;
                Color color = Lighting.GetColor((int)info.position.X / 16, (int)info.position.Y / 16) * opacity;
                int shader = drawingPlayer.cHeadOverlay;
                switch ((PlayerHeadOverlayID)drawingPlayer.headOverlay)
                {
                    default:
                    {
                        Main.playerDrawData.Add(new DrawData(DrawUtils.Textures.PlayerHeadOverlays[(PlayerHeadOverlayID)drawingPlayer.headOverlay], position - Main.screenPosition, info.drawPlayer.bodyFrame, color, info.drawPlayer.headRotation, info.headOrigin, 1f, info.spriteEffects, 0) { shader = shader, });
                    }
                    break;

                    case PlayerHeadOverlayID.MonoxideHat:
                    {
                        Main.playerDrawData.Add(new DrawData(DrawUtils.Textures.PlayerHeadOverlays[PlayerHeadOverlayID.MonoxideHat], position - Main.screenPosition, info.drawPlayer.bodyFrame, color, info.drawPlayer.headRotation, info.headOrigin, 1f, info.spriteEffects, 0) { shader = shader, });
                        Main.playerDrawData.Add(new DrawData(DrawUtils.Textures.PlayerHeadOverlays[PlayerHeadOverlayID.MonoxideHatGlow], position - Main.screenPosition, info.drawPlayer.bodyFrame, new Color(opacity * 0.99f, opacity * 0.99f, opacity * 0.99f, 0f), info.drawPlayer.headRotation, info.headOrigin, 1f, info.spriteEffects, 0) { shader = shader, });
                        var value = ModContent.ProjectileType<Projectiles.Minions.Monoxider>();
                        var texture = Main.projectileTexture[value];
                        var frame = new Rectangle(0, 0, texture.Width, texture.Height / Main.projFrames[value] - 2);
                        var drawData = new DrawData(texture, default(Vector2), frame, default(Color), 0f, frame.Size() / 2f, 1f, info.drawPlayer.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
                        var offset = new Vector2(0f, 0f);
                        if (player.gravDir == -1)
                        {
                            offset += new Vector2(0f, player.height + Main.OffsetsPlayerHeadgear[headFrame].Y + MagicOffsetForReversedGravity);
                            drawData.effect |= SpriteEffects.FlipVertically;
                        }
                        else
                        {
                            offset.Y += Main.OffsetsPlayerHeadgear[headFrame].Y;
                        }
                        for (int i = drawingPlayer.hatMinionCarry - 1; i >= 0; i--)
                        {
                            drawData.position = position + new Vector2(((i + 1) % 3 - 1) * 8 * player.direction, -(i / 3 * 10) - 12) + offset;
                            drawData.color = Lighting.GetColor((int)drawData.position.X / 16, (int)drawData.position.Y / 16) * opacity;
                            drawData.position -= Main.screenPosition;
                            Main.playerDrawData.Add(drawData);
                        }
                    }
                    break;

                    case PlayerHeadOverlayID.FishyFins:
                    {
                        Main.playerDrawData.Add(new DrawData(DrawUtils.Textures.PlayerHeadOverlays[(PlayerHeadOverlayID)drawingPlayer.headOverlay], position - Main.screenPosition, info.drawPlayer.bodyFrame, Lighting.GetColor((int)info.position.X / 16, (int)info.position.Y / 16, info.drawPlayer.skinColor), info.drawPlayer.headRotation, info.headOrigin, 1f, info.spriteEffects, 0) { shader = shader, });
                    }
                    break;
                }
            }
        }

        public override Action<PlayerDrawInfo> Apply()
        {
            return Draw;
        }
    }
}