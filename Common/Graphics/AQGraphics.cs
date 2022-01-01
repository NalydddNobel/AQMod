using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;

namespace AQMod.Common.Graphics
{
    internal static class AQGraphics
    {
        public static class Data
        {
            public static bool CanUseAssets => !AQMod.Loading && Main.netMode != NetmodeID.Server;
            /// <summary>
            /// Gets the center of the screen's draw coordinates
            /// </summary>
            internal static Vector2 ScreenCenter => new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);
            /// <summary>
            /// Gets the center of the screen's world coordinates
            /// </summary>
            internal static Vector2 WorldScreenCenter => new Vector2(Main.screenPosition.X + (Main.screenWidth / 2f), Main.screenPosition.Y + Main.screenHeight / 2f);
            /// <summary>
            /// The world view point matrix
            /// </summary>
            internal static Matrix WorldViewPoint
            {
                get
                {
                    GraphicsDevice graphics = Main.graphics.GraphicsDevice;
                    Vector2 screenZoom = Main.GameViewMatrix.Zoom;
                    int width = graphics.Viewport.Width;
                    int height = graphics.Viewport.Height;

                    var zoom = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) *
                        Matrix.CreateTranslation(width / 2f, height / -2f, 0) *
                        Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(screenZoom.X, screenZoom.Y, 1f);
                    var projection = Matrix.CreateOrthographic(width, height, 0, 1000);
                    return zoom * projection;
                }
            }
            internal static Vector2 TileZero => Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            internal static Rectangle renderRectangle;
        }

        public static class Rendering
        {
            public static class Culling
            {
                internal static bool InScreen(Vector2 position)
                {
                    return InScreen(new Rectangle((int)position.X, (int)position.Y, 1, 1));
                }
                internal static bool InScreen(Rectangle rectangle)
                {
                    return Data.renderRectangle.Intersects(rectangle);
                }

                internal static void update()
                {
                    Data.renderRectangle = new Rectangle(-20, -20, Main.screenWidth + 20, Main.screenHeight + 20);
                }
            }
 
            public static void DrawTileWithSloping(Tile tile, Texture2D texture, Vector2 drawCoordinates, Color drawColor, int frameX, int frameY, int width, int height)
            {
                if (tile.slope() == 0 && !tile.halfBrick())
                {
                    Main.spriteBatch.Draw(texture, drawCoordinates, new Rectangle(frameX, frameY, width, height), drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
                else if (tile.halfBrick())
                {
                    Main.spriteBatch.Draw(texture, new Vector2(drawCoordinates.X, drawCoordinates.Y + 10), new Rectangle(frameX, frameY, width, 6), drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
                else
                {
                    byte b = tile.slope();
                    for (int i = 0; i < 8; i++)
                    {
                        int num10 = i << 1;
                        Rectangle frame = new Rectangle(frameX, frameY + i * 2, num10, 2);
                        int xOffset = 0;
                        switch (b)
                        {
                            case 2:
                                frame.X = 16 - num10;
                                xOffset = 16 - num10;
                                break;
                            case 3:
                                frame.Width = 16 - num10;
                                break;
                            case 4:
                                frame.Width = 14 - num10;
                                frame.X = num10 + 2;
                                xOffset = num10 + 2;
                                break;
                        }
                        Main.spriteBatch.Draw(texture, new Vector2(drawCoordinates.X + (float)xOffset, drawCoordinates.Y + i * 2), frame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }
                }
            }

            public static void DrawFishingLine_NoLighting_UseCustomOrigin(Color color, Player player, Vector2 bobberPosition, int bobberWidth, int bobberHeight, Vector2 bobberVelocity, float velocitySum, Vector2 lineOrigin)
            {
                var bobberCenter = new Vector2(bobberPosition.X + bobberWidth / 2f, bobberPosition.Y + bobberHeight / 2f);
                int type = player.inventory[player.selectedItem].type;

                Vector2 playerToProjectile = bobberCenter - lineOrigin;
                bool canDraw = true;
                if (playerToProjectile.X == 0f && playerToProjectile.Y == 0f)
                    return;
                float playerToProjectileMagnitude = playerToProjectile.Length();
                playerToProjectileMagnitude = 12f / playerToProjectileMagnitude;
                playerToProjectile *= playerToProjectileMagnitude;
                lineOrigin -= playerToProjectile;
                playerToProjectile = bobberCenter - lineOrigin;
                float widthAdd = bobberWidth * 0.5f;
                float heightAdd = bobberHeight * 0.1f;
                while (canDraw)
                {
                    float height = 12f;
                    float positionMagnitude = playerToProjectile.Length();
                    if (float.IsNaN(positionMagnitude) || float.IsNaN(positionMagnitude))
                        break;

                    if (positionMagnitude < 20f)
                    {
                        height = positionMagnitude - 8f;
                        canDraw = false;
                    }
                    playerToProjectile *= 12f / positionMagnitude;
                    lineOrigin += playerToProjectile;
                    playerToProjectile.X = bobberPosition.X + widthAdd - lineOrigin.X;
                    playerToProjectile.Y = bobberPosition.Y + heightAdd - lineOrigin.Y;
                    if (positionMagnitude > 12f)
                    {
                        float positionInverseMultiplier = 0.3f;
                        float absVelocitySum = Math.Abs(bobberVelocity.X) + Math.Abs(bobberVelocity.Y);
                        if (absVelocitySum > 16f)
                            absVelocitySum = 16f;
                        absVelocitySum = 1f - absVelocitySum / 16f;
                        positionInverseMultiplier *= absVelocitySum;
                        absVelocitySum = positionMagnitude / 80f;
                        if (absVelocitySum > 1f)
                            absVelocitySum = 1f;
                        positionInverseMultiplier *= absVelocitySum;
                        if (positionInverseMultiplier < 0f)
                            positionInverseMultiplier = 0f;
                        absVelocitySum = 1f - velocitySum / 100f;
                        positionInverseMultiplier *= absVelocitySum;
                        if (playerToProjectile.Y > 0f)
                        {
                            playerToProjectile.Y *= 1f + positionInverseMultiplier;
                            playerToProjectile.X *= 1f - positionInverseMultiplier;
                        }
                        else
                        {
                            absVelocitySum = Math.Abs(bobberVelocity.X) / 3f;
                            if (absVelocitySum > 1f)
                                absVelocitySum = 1f;
                            absVelocitySum -= 0.5f;
                            positionInverseMultiplier *= absVelocitySum;
                            if (positionInverseMultiplier > 0f)
                                positionInverseMultiplier *= 2f;
                            playerToProjectile.Y *= 1f + positionInverseMultiplier;
                            playerToProjectile.X *= 1f - positionInverseMultiplier;
                        }
                    }
                    float rotation = playerToProjectile.ToRotation() - MathHelper.PiOver2;
                    Main.spriteBatch.Draw(Main.fishingLineTexture, new Vector2(lineOrigin.X - Main.screenPosition.X + Main.fishingLineTexture.Width * 0.5f, lineOrigin.Y - Main.screenPosition.Y + Main.fishingLineTexture.Height * 0.5f), new Rectangle(0, 0, Main.fishingLineTexture.Width, (int)height), color, rotation, new Vector2(Main.fishingLineTexture.Width * 0.5f, 0f), 1f, SpriteEffects.None, 0f);
                }
            }
            public static void DrawFishingLine_NoLighting(Color color, Player player, Vector2 bobberPosition, int bobberWidth, int bobberHeight, Vector2 bobberVelocity, float velocitySum, Vector2 linePositionOffset)
            {
                var bobberCenter = new Vector2(bobberPosition.X + bobberWidth / 2f, bobberPosition.Y + bobberHeight / 2f);
                Vector2 lineOrigin = player.MountedCenter;
                lineOrigin.Y += player.gfxOffY;
                int type = player.inventory[player.selectedItem].type;
                lineOrigin.X += linePositionOffset.X * player.direction;
                if (player.direction < 0)
                    lineOrigin.X -= 13f;
                lineOrigin.Y -= linePositionOffset.Y * player.gravDir;
                if (player.gravDir == -1)
                    lineOrigin.Y -= 12f;
                lineOrigin = player.RotatedRelativePoint(lineOrigin + new Vector2(8f), true) - new Vector2(8f);
                Vector2 playerToProjectile = bobberCenter - lineOrigin;
                bool canDraw = true;
                if (playerToProjectile.X == 0f && playerToProjectile.Y == 0f)
                    return;
                float playerToProjectileMagnitude = playerToProjectile.Length();
                playerToProjectileMagnitude = 12f / playerToProjectileMagnitude;
                playerToProjectile *= playerToProjectileMagnitude;
                lineOrigin -= playerToProjectile;
                playerToProjectile = bobberCenter - lineOrigin;
                float widthAdd = bobberWidth * 0.5f;
                float heightAdd = bobberHeight * 0.1f;
                while (canDraw)
                {
                    float height = 12f;
                    float positionMagnitude = playerToProjectile.Length();
                    if (float.IsNaN(positionMagnitude) || float.IsNaN(positionMagnitude))
                        break;

                    if (positionMagnitude < 20f)
                    {
                        height = positionMagnitude - 8f;
                        canDraw = false;
                    }
                    playerToProjectile *= 12f / positionMagnitude;
                    lineOrigin += playerToProjectile;
                    playerToProjectile.X = bobberPosition.X + widthAdd - lineOrigin.X;
                    playerToProjectile.Y = bobberPosition.Y + heightAdd - lineOrigin.Y;
                    if (positionMagnitude > 12f)
                    {
                        float positionInverseMultiplier = 0.3f;
                        float absVelocitySum = Math.Abs(bobberVelocity.X) + Math.Abs(bobberVelocity.Y);
                        if (absVelocitySum > 16f)
                            absVelocitySum = 16f;
                        absVelocitySum = 1f - absVelocitySum / 16f;
                        positionInverseMultiplier *= absVelocitySum;
                        absVelocitySum = positionMagnitude / 80f;
                        if (absVelocitySum > 1f)
                            absVelocitySum = 1f;
                        positionInverseMultiplier *= absVelocitySum;
                        if (positionInverseMultiplier < 0f)
                            positionInverseMultiplier = 0f;
                        absVelocitySum = 1f - velocitySum / 100f;
                        positionInverseMultiplier *= absVelocitySum;
                        if (playerToProjectile.Y > 0f)
                        {
                            playerToProjectile.Y *= 1f + positionInverseMultiplier;
                            playerToProjectile.X *= 1f - positionInverseMultiplier;
                        }
                        else
                        {
                            absVelocitySum = Math.Abs(bobberVelocity.X) / 3f;
                            if (absVelocitySum > 1f)
                                absVelocitySum = 1f;
                            absVelocitySum -= 0.5f;
                            positionInverseMultiplier *= absVelocitySum;
                            if (positionInverseMultiplier > 0f)
                                positionInverseMultiplier *= 2f;
                            playerToProjectile.Y *= 1f + positionInverseMultiplier;
                            playerToProjectile.X *= 1f - positionInverseMultiplier;
                        }
                    }
                    float rotation = playerToProjectile.ToRotation() - MathHelper.PiOver2;
                    Main.spriteBatch.Draw(Main.fishingLineTexture, new Vector2(lineOrigin.X - Main.screenPosition.X + Main.fishingLineTexture.Width * 0.5f, lineOrigin.Y - Main.screenPosition.Y + Main.fishingLineTexture.Height * 0.5f), new Rectangle(0, 0, Main.fishingLineTexture.Width, (int)height), color, rotation, new Vector2(Main.fishingLineTexture.Width * 0.5f, 0f), 1f, SpriteEffects.None, 0f);
                }
            }
            public static void DrawFishingLine(Color color, Player player, Vector2 bobberPosition, int bobberWidth, int bobberHeight, Vector2 bobberVelocity, float velocitySum, Vector2 linePositionOffset)
            {
                var bobberCenter = new Vector2(bobberPosition.X + bobberWidth / 2f, bobberPosition.Y + bobberHeight / 2f);
                Vector2 lineOrigin = player.MountedCenter;
                lineOrigin.Y += player.gfxOffY;
                int type = player.inventory[player.selectedItem].type;
                lineOrigin.X += linePositionOffset.X * player.direction;
                if (player.direction < 0)
                    lineOrigin.X -= 13f;
                lineOrigin.Y -= linePositionOffset.Y * player.gravDir;
                if (player.gravDir == -1)
                    lineOrigin.Y -= 12f;
                lineOrigin = player.RotatedRelativePoint(lineOrigin + new Vector2(8f), true) - new Vector2(8f);
                Vector2 playerToProjectile = bobberCenter - lineOrigin;
                bool canDraw = true;
                if (playerToProjectile.X == 0f && playerToProjectile.Y == 0f)
                    return;
                float playerToProjectileMagnitude = playerToProjectile.Length();
                playerToProjectileMagnitude = 12f / playerToProjectileMagnitude;
                playerToProjectile *= playerToProjectileMagnitude;
                lineOrigin -= playerToProjectile;
                playerToProjectile = bobberCenter - lineOrigin;
                float widthAdd = bobberWidth * 0.5f;
                float heightAdd = bobberHeight * 0.1f;
                while (canDraw)
                {
                    float height = 12f;
                    float positionMagnitude = playerToProjectile.Length();
                    if (float.IsNaN(positionMagnitude) || float.IsNaN(positionMagnitude))
                        break;

                    if (positionMagnitude < 20f)
                    {
                        height = positionMagnitude - 8f;
                        canDraw = false;
                    }
                    playerToProjectile *= 12f / positionMagnitude;
                    lineOrigin += playerToProjectile;
                    playerToProjectile.X = bobberPosition.X + widthAdd - lineOrigin.X;
                    playerToProjectile.Y = bobberPosition.Y + heightAdd - lineOrigin.Y;
                    if (positionMagnitude > 12f)
                    {
                        float positionInverseMultiplier = 0.3f;
                        float absVelocitySum = Math.Abs(bobberVelocity.X) + Math.Abs(bobberVelocity.Y);
                        if (absVelocitySum > 16f)
                            absVelocitySum = 16f;
                        absVelocitySum = 1f - absVelocitySum / 16f;
                        positionInverseMultiplier *= absVelocitySum;
                        absVelocitySum = positionMagnitude / 80f;
                        if (absVelocitySum > 1f)
                            absVelocitySum = 1f;
                        positionInverseMultiplier *= absVelocitySum;
                        if (positionInverseMultiplier < 0f)
                            positionInverseMultiplier = 0f;
                        absVelocitySum = 1f - velocitySum / 100f;
                        positionInverseMultiplier *= absVelocitySum;
                        if (playerToProjectile.Y > 0f)
                        {
                            playerToProjectile.Y *= 1f + positionInverseMultiplier;
                            playerToProjectile.X *= 1f - positionInverseMultiplier;
                        }
                        else
                        {
                            absVelocitySum = Math.Abs(bobberVelocity.X) / 3f;
                            if (absVelocitySum > 1f)
                                absVelocitySum = 1f;
                            absVelocitySum -= 0.5f;
                            positionInverseMultiplier *= absVelocitySum;
                            if (positionInverseMultiplier > 0f)
                                positionInverseMultiplier *= 2f;
                            playerToProjectile.Y *= 1f + positionInverseMultiplier;
                            playerToProjectile.X *= 1f - positionInverseMultiplier;
                        }
                    }
                    Color lineColor = Lighting.GetColor((int)lineOrigin.X / 16, (int)(lineOrigin.Y / 16f), color);
                    float rotation = playerToProjectile.ToRotation() - MathHelper.PiOver2;
                    Main.spriteBatch.Draw(Main.fishingLineTexture, new Vector2(lineOrigin.X - Main.screenPosition.X + Main.fishingLineTexture.Width * 0.5f, lineOrigin.Y - Main.screenPosition.Y + Main.fishingLineTexture.Height * 0.5f), new Rectangle(0, 0, Main.fishingLineTexture.Width, (int)height), lineColor, rotation, new Vector2(Main.fishingLineTexture.Width * 0.5f, 0f), 1f, SpriteEffects.None, 0f);
                }
            }

            public static void DrawWall(int i, int j, Texture2D texture, Color color)
            {
                var tile = Main.tile[i, j];
                Main.spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X - 8, j * 16 - (int)Main.screenPosition.Y - 8) + AQGraphics.Data.TileZero, new Rectangle(tile.wallFrameX(), tile.wallFrameY() + Main.wallFrame[tile.wall] * 180, 32, 32), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            public static void DrawChain_UseLighting(Texture2D chain, Vector2 currentPosition, Vector2 endPosition, Vector2 screenPos)
            {
                int height = chain.Height - 2;
                Vector2 velocity = endPosition - currentPosition;
                int length = (int)(velocity.Length() / height);
                velocity.Normalize();
                velocity *= height;
                float rotation = velocity.ToRotation() + MathHelper.PiOver2;
                var origin = new Vector2(chain.Width / 2f, chain.Height / 2f);
                for (int i = 0; i < length; i++)
                {
                    var position = currentPosition + velocity * i;
                    Main.spriteBatch.Draw(chain, position - screenPos, null, Lighting.GetColor((int)(position.X / 16), (int)(position.Y / 16f)), rotation, origin, 1f, SpriteEffects.None, 0f);
                }
            }

            public static void DrawJerryChain(Texture2D chain, Vector2 currentPosition, Vector2 endPosition)
            {
                int height = chain.Height - 2;
                var velo = Vector2.Normalize(endPosition + new Vector2(0f, height * 4f) - currentPosition) * height;
                var position = currentPosition;
                var origin = new Vector2(chain.Width / 2f, chain.Height / 2f);
                for (int i = 0; i < 50; i++)
                {
                    Main.spriteBatch.Draw(chain, position - Main.screenPosition, null, Lighting.GetColor((int)(position.X / 16), (int)(position.Y / 16f)), velo.ToRotation() + MathHelper.PiOver2, origin, 1f, SpriteEffects.None, 0f);
                    velo = Vector2.Normalize(Vector2.Lerp(velo, endPosition - position, 0.01f + MathHelper.Clamp(1f - Vector2.Distance(endPosition, position) / 300f, 0f, 0.99f))) * height;
                    position += velo;
                    float gravity = MathHelper.Clamp(1f - Vector2.Distance(endPosition, position) / 500f, 0f, 1f);
                    velo.Y += gravity;
                    position.Y += 6f * gravity;
                    if (Vector2.Distance(position, endPosition) <= height)
                        break;
                }
            }
        }

        internal delegate void DrawMethod(Texture2D texture, Vector2 position, Rectangle? frame, Color color, float scale, Vector2 origin, float rotation, SpriteEffects effects, float layerDepth);
    }
}