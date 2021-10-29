using AQMod.Assets;
using AQMod.Assets.PlayerLayers.EquipOverlays;
using AQMod.Assets.Textures;
using AQMod.Common.Config;
using AQMod.Common.Utilities;
using AQMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace AQMod.Common.PlayerData.Layers
{
    public class PostDrawHead : TempPlayerLayerWrapper
    {
        public override void Draw(PlayerDrawInfo info)
        {
            var player = info.drawPlayer;
            var aQPlayer = info.drawPlayer.GetModPlayer<AQPlayer>();
            float opacity = 1f - info.shadow;
            const float MagicOffsetForReversedGravity = 8f;
            int headFrame = info.drawPlayer.bodyFrame.Y / AQPlayer.FRAME_HEIGHT;
            float gravityOffset = 0f;
            AQMod.ArmorOverlays.InvokeArmorOverlay(EquipLayering.Head, info);
            if (info.drawPlayer.gravDir == -1)
                gravityOffset = MagicOffsetForReversedGravity;
            if (aQPlayer.mask >= 0)
            {
                Vector2 position = new Vector2((int)(info.position.X - Main.screenPosition.X - info.drawPlayer.bodyFrame.Width / 2 + info.drawPlayer.width / 2), (int)(info.position.Y - Main.screenPosition.Y + info.drawPlayer.height - info.drawPlayer.bodyFrame.Height + gravityOffset)) + info.drawPlayer.headPosition + info.headOrigin;
                Color color = Lighting.GetColor((int)info.position.X / 16, (int)(info.position.Y + gravityOffset) / 16) * opacity;
                switch ((PlayerMaskID)aQPlayer.mask)
                {
                    default:
                    {
                        Main.playerDrawData.Add(new DrawData(TextureCache.PlayerMasks[(PlayerMaskID)aQPlayer.mask], position, info.drawPlayer.bodyFrame, color, info.drawPlayer.headRotation, info.headOrigin, 1f, info.spriteEffects, 0) { shader = aQPlayer.cMask, });
                    }
                    break;

                    case PlayerMaskID.CataMask:
                    {
                        if (aQPlayer.cMask > 0)
                            aQPlayer.cataEyeColor = new Color(100, 100, 100, 0);
                        Main.playerDrawData.Add(new DrawData(TextureCache.PlayerMasks[PlayerMaskID.CataMask], position, info.drawPlayer.bodyFrame, color, info.drawPlayer.headRotation, info.headOrigin, 1f, info.spriteEffects, 0) { shader = aQPlayer.cMask, });
                        if (aQPlayer.mothmanMaskSpecial)
                        {
                            if (info.drawPlayer.headRotation == 0)
                            {
                                var texture = TextureCache.Lights[LightID.Spotlight240x66];
                                var frame = new Rectangle(0, 0, texture.Width, texture.Height);
                                var orig = frame.Size() / 2f;
                                var scale = new Vector2((float)(Math.Sin(Main.GlobalTime * 10f) + 1f) * 0.04f + 0.2f, 0.1f);
                                var eyeGlowPos = position + new Vector2(2f * player.direction, Main.OffsetsPlayerHeadgear[headFrame].Y);
                                var eyeGlowColor = aQPlayer.cataEyeColor;
                                var value = AQUtils.GetGrad(0.25f, 0.45f, scale.X) * 0.5f;
                                var config = ModContent.GetInstance<AQConfigClient>();
                                var colorMult = ModContent.GetInstance<AQConfigClient>().EffectIntensity * (1f - info.shadow);
                                Main.playerDrawData.Add(new DrawData(texture, eyeGlowPos, frame, eyeGlowColor * colorMult, 0f, orig, scale, info.spriteEffects, 0) { shader = aQPlayer.cMask, });
                                Main.playerDrawData.Add(new DrawData(texture, eyeGlowPos, frame, eyeGlowColor * 0.3f * colorMult, 0f, orig, scale * (1.1f + value * 2), info.spriteEffects, 0) { shader = aQPlayer.cMask, });
                                if (ModContent.GetInstance<AQConfigClient>().EffectQuality > 0.5f)
                                {
                                    Main.playerDrawData.Add(new DrawData(texture, eyeGlowPos, frame, eyeGlowColor * 0.35f * colorMult, MathHelper.PiOver4, orig, scale * (1f - value) * 0.75f, info.spriteEffects, 0) { shader = aQPlayer.cMask, });
                                    Main.playerDrawData.Add(new DrawData(texture, eyeGlowPos, frame, eyeGlowColor * 0.35f * colorMult, -MathHelper.PiOver4, orig, scale * (1f - value) * 0.75f, info.spriteEffects, 0) { shader = aQPlayer.cMask, });
                                    Main.playerDrawData.Add(new DrawData(texture, eyeGlowPos, frame, eyeGlowColor * 0.2f * colorMult, MathHelper.PiOver2, orig, scale * (1f - value), info.spriteEffects, 0) { shader = aQPlayer.cMask, });
                                }
                                Main.playerDrawData.Add(new DrawData(texture, eyeGlowPos, frame, eyeGlowColor * colorMult, MathHelper.PiOver2, orig, scale * 0.5f, info.spriteEffects, 0) { shader = aQPlayer.cMask, });
                                if (ModContent.GetInstance<AQConfigClient>().EffectIntensity > 1.5f && ModContent.GetInstance<AQConfigClient>().EffectQuality > 0.5f)
                                    Main.playerDrawData.Add(new DrawData(texture, eyeGlowPos, frame, eyeGlowColor * 0.15f * colorMult, 0f, orig, scale * (2f + value * 3f), info.spriteEffects, 0) { shader = aQPlayer.cMask, });
                            }
                        }
                    }
                    break;
                }
            }
            if (aQPlayer.headOverlay >= 0)
            {
                Vector2 position = new Vector2((int)(info.position.X - Main.screenPosition.X - info.drawPlayer.bodyFrame.Width / 2 + info.drawPlayer.width / 2), (int)(info.position.Y - Main.screenPosition.Y + info.drawPlayer.height - info.drawPlayer.bodyFrame.Height + gravityOffset)) + info.drawPlayer.headPosition + info.headOrigin;
                Color color = Lighting.GetColor((int)info.position.X / 16, (int)info.position.Y / 16) * opacity;
                int shader = aQPlayer.cHeadOverlay;
                switch ((PlayerHeadOverlayID)aQPlayer.headOverlay)
                {
                    default:
                    {
                        Main.playerDrawData.Add(new DrawData(TextureCache.PlayerHeadOverlays[(PlayerHeadOverlayID)aQPlayer.headOverlay], position, info.drawPlayer.bodyFrame, color, info.drawPlayer.headRotation, info.headOrigin, 1f, info.spriteEffects, 0) { shader = shader, });
                    }
                    break;

                    case PlayerHeadOverlayID.MonoxideHat:
                    {
                        Main.playerDrawData.Add(new DrawData(TextureCache.PlayerHeadOverlays[PlayerHeadOverlayID.MonoxideHat], position, info.drawPlayer.bodyFrame, color, info.drawPlayer.headRotation, info.headOrigin, 1f, info.spriteEffects, 0) { shader = shader, });
                        Main.playerDrawData.Add(new DrawData(TextureCache.PlayerHeadOverlays[PlayerHeadOverlayID.MonoxideHatGlow], position, info.drawPlayer.bodyFrame, new Color(opacity * 0.99f, opacity * 0.99f, opacity * 0.99f, 0f), info.drawPlayer.headRotation, info.headOrigin, 1f, info.spriteEffects, 0) { shader = shader, });
                        if (aQPlayer.monoxiderBird && !aQPlayer.chomper)
                        {
                            var hatPos = position;
                            if (player.gravDir == -1)
                                hatPos.Y += player.height + Main.OffsetsPlayerHeadgear[headFrame].Y + 8f;
                            else
                            {
                                hatPos.Y += Main.OffsetsPlayerHeadgear[headFrame].Y;
                            }
                            Projectiles.Summon.Monoxider.DrawHead(player, aQPlayer, hatPos, ignorePlayerRotation: false);
                        }
                    }
                    break;

                    case PlayerHeadOverlayID.FishyFins:
                    {
                        Main.playerDrawData.Add(new DrawData(TextureCache.PlayerHeadOverlays[(PlayerHeadOverlayID)aQPlayer.headOverlay], position, info.drawPlayer.bodyFrame, Lighting.GetColor((int)info.position.X / 16, (int)info.position.Y / 16, info.drawPlayer.skinColor), info.drawPlayer.headRotation, info.headOrigin, 1f, info.spriteEffects, 0) { shader = shader, });
                    }
                    break;
                }
            }
        }
    }
}