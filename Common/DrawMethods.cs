using AQMod.Assets.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace AQMod.Common
{
    public static class DrawMethods
    {
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
            Main.spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X - 8, j * 16 - (int)Main.screenPosition.Y - 8) + SpriteUtils.TileZero, new Rectangle(tile.wallFrameX(), tile.wallFrameY() + Main.wallFrame[tile.wall] * 180, 32, 32), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public static void DrawChain_UseLighting(Texture2D chain, Vector2 currentPosition, Vector2 endPosition, Vector2 screenPos)
        {
            int height = chain.Height - 2;
            Vector2 velocity = endPosition - currentPosition;
            int length = (int)(velocity.Length() / height);
            velocity.Normalize();
            velocity *= height;
            float rotation = velocity.ToRotation() + MathHelper.PiOver2;
            Vector2 origin = new Vector2(chain.Width / 2f, chain.Height / 2f);
            for (int i = 0; i < length; i++)
            {
                var position = currentPosition + velocity * i;
                Main.spriteBatch.Draw(chain, position - screenPos, null, Lighting.GetColor((int)(position.X / 16), (int)(position.Y / 16f)), rotation, origin, 1f, SpriteEffects.None, 0f);
            }
        }

        public static void DrawJerryChain(Vector2 currentPosition, Vector2 endPosition)
        {
            var chain = SpriteUtils.Textures.Extras[ExtraID.JerryChain];
            int height = chain.Height - 2;
            var velo = Vector2.Normalize(endPosition + new Vector2(0f, height * 4f) - currentPosition) * height;
            var position = currentPosition;
            Vector2 origin = new Vector2(chain.Width / 2f, chain.Height / 2f);
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
}