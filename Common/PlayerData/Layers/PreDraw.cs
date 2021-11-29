using AQMod.Assets;
using AQMod.Common.Utilities;
using AQMod.Items.Vanities;
using AQMod.Items.Weapons.Summon;
using AQMod.Projectiles;
using AQMod.Projectiles.Summon.ChomperMinion;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AQMod.Common.PlayerData.Layers
{
    public class PreDraw : TempPlayerLayerWrapper
    {
        public override void Draw(PlayerDrawInfo info)
        {
            int whoAmI = info.drawPlayer.whoAmI;
            var player = info.drawPlayer;
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            var drawingPlayer = player.GetModPlayer<AQPlayer>();
            if (info.shadow == 0f)
            {
                if (aQPlayer.blueSpheres && drawingPlayer.celesteTorusOffsetsForDrawing != null)
                {
                    var texture = TextureCache.GetProjectile(ModContent.ProjectileType<CelesteTorusCollider>());
                    var frame = new Rectangle(0, 0, texture.Width, texture.Height);
                    var orig = frame.Size() / 2f;
                    for (int i = 0; i < AQPlayer.MaxCelesteTorusOrbs; i++)
                    {
                        var position = aQPlayer.GetCelesteTorusPositionOffset(i);
                        float layerValue = AQUtils.Projector3D.GetParralaxScale(1f, drawingPlayer.celesteTorusOffsetsForDrawing[i].Z * AQPlayer.CELESTE_Z_MULT);
                        if (layerValue < 1f)
                        {
                            var center = info.position + new Vector2(player.width / 2 + (int)position.X, player.height / 2 + (int)position.Y);
                            Main.playerDrawData.Add(new DrawData(texture, AQUtils.Projector3D.GetParralaxPosition(center, drawingPlayer.celesteTorusOffsetsForDrawing[i].Z * AQPlayer.CELESTE_Z_MULT) - Main.screenPosition, frame, Lighting.GetColor((int)(center.X / 16f), (int)(center.Y / 16f)), 0f, orig, AQUtils.Projector3D.GetParralaxScale(aQPlayer.celesteTorusScale, drawingPlayer.celesteTorusOffsetsForDrawing[i].Z * AQPlayer.CELESTE_Z_MULT), SpriteEffects.None, 0) { shader = drawingPlayer.cCelesteTorus, ignorePlayerRotation = true });
                        }
                    }
                }
                if (aQPlayer.chomper)
                {
                    int count = 0;
                    int type = ModContent.ProjectileType<Chomper>();
                    var texture = TextureCache.GetProjectile(type);
                    int frameHeight = texture.Height / Main.projFrames[type];
                    var frame = new Rectangle(0, 0, texture.Width, frameHeight - 2);
                    var textureOrig = frame.Size() / 2f;
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].active && Main.projectile[i].type == type && Main.projectile[i].owner == info.drawPlayer.whoAmI)
                        {
                            var drawPosition = Main.projectile[i].Center;
                            frame = new Rectangle(0, frameHeight * Main.projectile[i].frame, texture.Width, frameHeight - 2);
                            var drawColor = Lighting.GetColor((int)drawPosition.X / 16, (int)drawPosition.Y / 16);
                            float rotation;
                            if (Main.projectile[i].spriteDirection == -1 && (int)Main.projectile[i].ai[1] > 0f)
                                rotation = Main.projectile[i].rotation - MathHelper.Pi;
                            else
                            {
                                rotation = Main.projectile[i].rotation;
                            }
                            if (Main.myPlayer == info.drawPlayer.whoAmI)
                                DrawChomperChain(info.drawPlayer, Main.projectile[i], drawPosition, drawColor);
                            var chomperHead = (Chomper)Main.projectile[i].modProjectile;
                            if (chomperHead.eatingDelay != 0 && chomperHead.eatingDelay < 35)
                            {
                                float intensity = (10 - chomperHead.eatingDelay) / 2.5f * AQMod.EffectIntensity;
                                drawPosition.X += Main.rand.NextFloat(-intensity, intensity);
                                drawPosition.Y += Main.rand.NextFloat(-intensity, intensity);
                            }
                            drawPosition = new Vector2((int)(drawPosition.X - Main.screenPosition.X), (int)(drawPosition.Y - Main.screenPosition.Y));
                            Main.playerDrawData.Add(
                                new DrawData(texture, drawPosition, frame, drawColor, rotation, textureOrig, Main.projectile[i].scale, Main.projectile[i].spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0)
                                { ignorePlayerRotation = true });
                            if (count == 0)
                            {
                                if (aQPlayer.monoxiderBird)
                                {
                                    var monoxiderHat = TextureCache.GetItem(ModContent.ItemType<MonoxideHat>());
                                    var hatPos = new Vector2(drawPosition.X, drawPosition.Y) + new Vector2(0f, -Main.projectile[i].height / 2).RotatedBy(Main.projectile[i].rotation);
                                    var monoxiderHatOrig = monoxiderHat.Size() / 2f;
                                    hatPos = new Vector2((int)hatPos.X, (int)hatPos.Y);
                                    Main.playerDrawData.Add(
                                        new DrawData(monoxiderHat, hatPos, null, drawColor, Main.projectile[i].rotation, monoxiderHatOrig, Main.projectile[i].scale, SpriteEffects.None, 0)
                                        { ignorePlayerRotation = true });
                                    Main.playerDrawData.Add(
                                        new DrawData(ModContent.GetTexture(AQUtils.GetPath<MonoxideHat>() + "_Glow"), hatPos, null, new Color(250, 250, 250, 0), Main.projectile[i].rotation, monoxiderHatOrig, Main.projectile[i].scale, SpriteEffects.None, 0)
                                        { ignorePlayerRotation = true });
                                    int headFrame = player.bodyFrame.Y / AQPlayer.FRAME_HEIGHT;
                                    if (player.gravDir == -1)
                                        hatPos.Y += player.height + 8f;
                                    Projectiles.Summon.Monoxider.DrawHead(player, aQPlayer, hatPos, ignorePlayerRotation: true);
                                }
                            }
                            count++;
                        }
                    }
                }
            }
        }

        private void DrawChomperChain(Player player, Projectile chomper, Vector2 drawPosition, Color drawColor)
        {
            var chomperHead = (Chomper)chomper.modProjectile;
            int frameWidth = 16;
            var frame = new Rectangle(0, 0, frameWidth - 2, 20);
            var origin = frame.Size() / 2f;
            float offset = chomper.width / 2f + frame.Height / 2f;
            var texture = ModContent.GetTexture(AQUtils.GetPath<Chomper>("_Chain"));
            Main.playerDrawData.Add(new DrawData(texture, new Vector2(drawPosition.X + chomper.width / 2 * -chomper.spriteDirection, drawPosition.Y), frame, drawColor, 0f, origin, chomper.scale, chomper.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0)
            { ignorePlayerRotation = true });
            int height = frame.Height - 4;
            frame.Y += 2;
            var playerCenter = player.Center;
            var chainStart = chomper.Center + new Vector2((chomper.width / 2 + 4) * -chomper.spriteDirection, 0f);
            var velo = Vector2.Normalize(Vector2.Lerp(chainStart + new Vector2(0f, height * 4f) - playerCenter, player.velocity, 0.5f)) * height;
            var position = playerCenter;
            var rand = new UnifiedRandom(chomper.whoAmI + player.name.GetHashCode());
            if (AQMod.EffectQuality >= 1f)
            {
                for (int i = 0; i < 50; i++)
                {
                    Main.playerDrawData.Add(new DrawData(
                        texture, new Vector2((int)(position.X - Main.screenPosition.X), (int)(position.Y - Main.screenPosition.Y)), new Rectangle(frame.X + frameWidth + frameWidth * rand.Next(3), frame.Y, frame.Width, frame.Height), Lighting.GetColor((int)(position.X / 16), (int)(position.Y / 16f)), velo.ToRotation() + MathHelper.PiOver2, origin, 1f, SpriteEffects.None, 0)
                    { ignorePlayerRotation = true });
                    velo = Vector2.Normalize(Vector2.Lerp(velo, chainStart - position, 0.01f + MathHelper.Clamp(1f - Vector2.Distance(chainStart, position) / 100f, 0f, 0.99f))) * height;
                    if (Vector2.Distance(position, chainStart) <= height)
                        break;
                    velo = velo.RotatedBy(Math.Sin(Main.GlobalTime * 6f + i * 0.5f + chomper.whoAmI + rand.NextFloat(-0.02f, 0.02f)) * 0.1f * AQMod.EffectIntensity);
                    position += velo;
                    float gravity = MathHelper.Clamp(1f - Vector2.Distance(chainStart, position) / 60f, 0f, 1f);
                    velo.Y += gravity * 3f;
                    velo.Normalize();
                    velo *= height;
                }
            }
            else
            {
                if (AQMod.EffectQuality < 0.2f)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        Main.playerDrawData.Add(new DrawData(
                            texture, new Vector2((int)(position.X - Main.screenPosition.X), (int)(position.Y - Main.screenPosition.Y)), new Rectangle(frame.X + frameWidth + frameWidth * (i % 3), frame.Y, frame.Width, frame.Height), Lighting.GetColor((int)(position.X / 16), (int)(position.Y / 16f)), velo.ToRotation() + MathHelper.PiOver2, origin, 1f, SpriteEffects.None, 0)
                        { ignorePlayerRotation = true });
                        velo = Vector2.Normalize(Vector2.Lerp(velo, chainStart - position, 0.01f + MathHelper.Clamp(1f - Vector2.Distance(chainStart, position) / 100f, 0f, 0.99f))) * height;
                        if (Vector2.Distance(position, chainStart) <= height)
                            break;
                        position += velo;
                        float gravity = MathHelper.Clamp(1f - Vector2.Distance(chainStart, position) / 60f, 0f, 1f);
                        velo.Y += gravity * 3f;
                        velo.Normalize();
                        velo *= height;
                    }
                }
                else
                {
                    for (int i = 0; i < 50; i++)
                    {
                        Main.playerDrawData.Add(new DrawData(
                            texture, new Vector2((int)(position.X - Main.screenPosition.X), (int)(position.Y - Main.screenPosition.Y)), new Rectangle(frame.X + frameWidth + frameWidth * rand.Next(3), frame.Y, frame.Width, frame.Height), Lighting.GetColor((int)(position.X / 16), (int)(position.Y / 16f)), velo.ToRotation() + MathHelper.PiOver2, origin, 1f, SpriteEffects.None, 0)
                        { ignorePlayerRotation = true });
                        velo = Vector2.Normalize(Vector2.Lerp(velo, chainStart - position, 0.01f + MathHelper.Clamp(1f - Vector2.Distance(chainStart, position) / 100f, 0f, 0.99f))) * height;
                        if (Vector2.Distance(position, chainStart) <= height)
                            break;
                        position += velo;
                        float gravity = MathHelper.Clamp(1f - Vector2.Distance(chainStart, position) / 60f, 0f, 1f);
                        velo.Y += gravity * 3f;
                        velo.Normalize();
                        velo *= height;
                    }
                }
            }
            rand = new UnifiedRandom(chomper.whoAmI + 2 + player.name.GetHashCode());
            texture = ModContent.GetTexture(AQUtils.GetPath<Chomper>("_Leaves"));
            frame.Y -= 2;
            int numLeaves = rand.Next(4) + 3;
            float leafRotation = chomper.rotation;
            if (chomper.spriteDirection == -1 && chomper.rotation.Abs() > MathHelper.PiOver2)
                leafRotation -= MathHelper.Pi;
            float rotOff = MathHelper.PiOver2 / numLeaves;
            float rotStart = leafRotation - MathHelper.PiOver4;
            for (int i = 0; i < numLeaves; i++)
            {
                var leavesPos = drawPosition + new Vector2((offset - rand.NextFloat(2f)) * -chomper.spriteDirection, 0f).RotatedBy(rotStart + rotOff * i) - Main.screenPosition;
                leafRotation = (drawPosition - Main.screenPosition - leavesPos).ToRotation();
                Main.playerDrawData.Add(new DrawData(texture, new Vector2((int)leavesPos.X, leavesPos.Y), new Rectangle(frame.X + frameWidth * rand.Next(4), frame.Y, frame.Width, frame.Height), drawColor, leafRotation + MathHelper.PiOver2, origin, chomper.scale + rand.NextFloat(0.2f), SpriteEffects.None, 0)
                { ignorePlayerRotation = true });
            }
        }
    }
}