using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus {
    public static partial class Helper
    {
        public static int ShaderColorOnlyIndex => ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
        public static ArmorShaderData ShaderColorOnly => GameShaders.Armor.GetSecondaryShader(ShaderColorOnlyIndex, Main.LocalPlayer);

        #region Rendering Shapes
        public static void DrawLineList(List<Vector2> list)
        {
            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new Vector2(frame.Width / 2, 2);

            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - 1; i++)
            {
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates(), Color.White);
                Vector2 scale = new Vector2(1, (diff.Length() + 2) / frame.Height);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

                pos += diff;
            }
        }
        public static void DrawLine(Vector2 start, float rotation, float length, float width, Color color)
        {
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, start, new Rectangle(0, 0, 1, 1), color, rotation, new Vector2(1f, 0.5f), new Vector2(length, width), SpriteEffects.None, 0f);
        }
        public static void DrawLine(Vector2 start, Vector2 end, float width, Color color)
        {
            DrawLine(start, (start - end).ToRotation(), (end - start).Length(), width, color);
        }

        public static void DrawRectangle(Rectangle rect, Vector2 offset, Color color)
        {
            rect.X += (int)offset.X;
            rect.Y += (int)offset.Y;
            DrawRectangle(rect, color);
        }
        public static void DrawRectangle(Rectangle rect, Color color)
        {
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, color);
        }
        #endregion

        #region Ripped Vanilla Render code
        public static void DrawFishingLine(Player player, Vector2 bobberPosition, int bobberWidth, int bobberHeight, Vector2 bobberVelocity, float velocitySum, Vector2 lineOrigin, Color? customColor = null, Func<Vector2, Color, Color> getLighting = null)
        {
            var color = customColor.GetValueOrDefault(Color.White);
            if (getLighting == null)
            {
                getLighting = GetColor;
            }
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
            var texture = TextureAssets.FishingLine.Value;
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
                Main.EntitySpriteDraw(texture, new Vector2(lineOrigin.X - Main.screenPosition.X + texture.Width * 0.5f, lineOrigin.Y - Main.screenPosition.Y + texture.Height * 0.5f),
                    new Rectangle(0, 0, texture.Width, (int)height), getLighting(lineOrigin, color), rotation, new Vector2(texture.Width * 0.5f, 0f), 1f, SpriteEffects.None, 0);
            }
        }

        public static void DrawWall(int i, int j, Texture2D texture, Color color)
        {
            var tile = Main.tile[i, j];
            Main.spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X - 8, j * 16 - (int)Main.screenPosition.Y - 8) + TileDrawOffset, new Rectangle(tile.WallFrameX, tile.WallFrameX + Main.wallFrame[tile.WallType] * 180, 32, 32), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Replica of how Terraria draws tiles. Doesn't have support for color slices so this is only useful on glowmasks.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="texture"></param>
        /// <param name="drawCoordinates"></param>
        /// <param name="fullBlockDrawColor"></param>
        /// <param name="frameX"></param>
        /// <param name="frameY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void DrawTile(int i, int j, Texture2D texture, Vector2 drawCoordinates, Color fullBlockDrawColor, int frameX, int frameY, int width, int height)
        {
            var tile = Main.tile[i, j];

            if (tile.Slope != 0)
            {
                // Weird slope rendering
                byte slopeType = (byte)tile.Slope;
                int num2 = 2;
                for (int k = 0; k < 8; k++)
                {
                    int num3 = k * -2;
                    int num4 = 16 - k * 2;
                    int num5 = 16 - num4;
                    int num6;
                    switch (slopeType)
                    {
                        case 1:
                            num3 = 0;
                            num6 = k * 2;
                            num4 = 14 - k * 2;
                            num5 = 0;
                            break;
                        case 2:
                            num3 = 0;
                            num6 = 16 - k * 2 - 2;
                            num4 = 14 - k * 2;
                            num5 = 0;
                            break;
                        case 3:
                            num6 = k * 2;
                            break;
                        default:
                            num6 = 16 - k * 2 - 2;
                            break;
                    }
                    Main.spriteBatch.Draw(texture, drawCoordinates + new Vector2(num6, k * num2 + num3), new Rectangle(frameX + num6, frameY + num5, num2, num4), fullBlockDrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
                int num7 = (slopeType <= 2) ? 14 : 0;
                Main.spriteBatch.Draw(texture, drawCoordinates + new Vector2(0f, num7), new Rectangle(frameX, frameY + num7, 16, 2), fullBlockDrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                return;
            }

            // Solid tile, merging with nearby half blocks
            if (!TileID.Sets.IgnoresNearbyHalfbricksWhenDrawn[tile.TileType] && Main.tileSolid[tile.TileType] && !TileID.Sets.NotReallySolid[tile.TileType] && !tile.IsHalfBlock && (Main.tile[i - 1, j].IsHalfBlock || Main.tile[i + 1, j].IsHalfBlock))
            {

                if (Main.tile[i - 1, j].IsHalfBlock && Main.tile[i + 1, j].IsHalfBlock)
                {
                    // Merging with both sides if they're half blocks
                    Main.spriteBatch.Draw(texture, drawCoordinates + new Vector2(0f, 8f), new Rectangle(frameX, frameY + 8, width, 8), fullBlockDrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    var value3 = new Rectangle(126, 0, 16, 8);
                    if (Main.tile[i, j - 1].HasTile && !Main.tile[i, j - 1].BottomSlope && Main.tile[i, j - 1].TileType == tile.TileType)
                    {
                        value3 = new(90, 0, 16, 8);
                    }
                    Main.spriteBatch.Draw(texture, drawCoordinates, value3, fullBlockDrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
                else if (Main.tile[i - 1, j].IsHalfBlock)
                {
                    // Merging with left half block
                    int num8 = 4;
                    if (TileID.Sets.AllBlocksWithSmoothBordersToResolveHalfBlockIssue[tile.TileType])
                    {
                        num8 = 2;
                    }
                    Main.spriteBatch.Draw(texture, drawCoordinates + new Vector2(0f, 8f), new Rectangle(frameX, frameY + 8, width, 8), fullBlockDrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(texture, drawCoordinates + new Vector2(num8, 0f), new Rectangle(frameX + num8, frameY, width - num8, height), fullBlockDrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(texture, drawCoordinates, new Rectangle(144, 0, num8, 8), fullBlockDrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    if (num8 == 2)
                    {
                        Main.spriteBatch.Draw(texture, drawCoordinates, new Rectangle(148, 0, 2, 2), fullBlockDrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }
                }
                else if (Main.tile[i + 1, j].IsHalfBlock)
                {
                    // Merging with right half block
                    int num9 = 4;
                    if (TileID.Sets.AllBlocksWithSmoothBordersToResolveHalfBlockIssue[tile.TileType])
                    {
                        num9 = 2;
                    }
                    Main.spriteBatch.Draw(texture, drawCoordinates + new Vector2(0f, 8f), new Rectangle(frameX, frameY + 8, width, 8), fullBlockDrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(texture, drawCoordinates, new Rectangle(frameX, frameY, width - num9, height), fullBlockDrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(texture, drawCoordinates + new Vector2(16 - num9, 0f), new Rectangle(144 + (16 - num9), 0, num9, 8), fullBlockDrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    if (num9 == 2)
                    {
                        Main.spriteBatch.Draw(texture, drawCoordinates + new Vector2(14f, 0f), new Rectangle(156, 0, 2, 2), fullBlockDrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }
                }
                return;
            }

            // Half block drawing edit
            if (tile.IsHalfBlock)
            {
                drawCoordinates.Y += 8f;
                height -= 8;

                // Bottom side of half blocks if the tile below it isn't solid
                if (!Main.tile[i, j + 1].HasTile || !Main.tileSolid[Main.tile[i, j + 1].TileType] || Main.tile[i, j + 1].IsHalfBlock)
                {
                    Main.spriteBatch.Draw(texture, drawCoordinates, new Rectangle(frameX, frameY, width, height).Modified(0, 0, 0, -4), fullBlockDrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(texture, drawCoordinates + new Vector2(0f, 4f), (Rectangle?)new Rectangle(144, 66, width, 4), fullBlockDrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    return;
                }
            }

            Main.spriteBatch.Draw(texture, drawCoordinates, new Rectangle(frameX, frameY, width, height), fullBlockDrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
        #endregion

        #region Lazy info getter methods
        public static void GetItemDrawData(int item, out Rectangle frame)
        {
            frame = Main.itemAnimations[item] == null ? TextureAssets.Item[item].Value.Frame() : Main.itemAnimations[item].GetFrame(TextureAssets.Item[item].Value);
        }
        public static void GetItemDrawData(this Item item, out Rectangle frame)
        {
            GetItemDrawData(item.type, out frame);
        }

        public static void GetDrawInfo(this NPC npc, out Texture2D texture, out Vector2 offset, out Rectangle frame, out Vector2 origin, out int trailLength)
        {
            texture = TextureAssets.Npc[npc.type].Value;
            offset = npc.Size / 2f;
            frame = npc.frame;
            origin = frame.Size() / 2f;
            trailLength = NPCID.Sets.TrailCacheLength[npc.type];
        }
        public static void GetDrawInfo(this Projectile projectile, out Texture2D texture, out Vector2 offset, out Rectangle frame, out Vector2 origin, out int trailLength)
        {
            texture = TextureAssets.Projectile[projectile.type].Value;
            offset = projectile.Size / 2f;
            frame = projectile.Frame();
            origin = frame.Size() / 2f;
            trailLength = ProjectileID.Sets.TrailCacheLength[projectile.type];
        }
        #endregion

        #region Tiles
        public static Vector2 TileDrawOffset => Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

        public static Vector2 GetDrawPosition(this ModTile modTile, int i, int j, TileObjectData objectData)
        {
            var tile = Main.tile[i, j];
            return new Vector2(i * 16f + (objectData?.DrawXOffset).GetValueOrDefault(), j * 16f + (objectData?.DrawYOffset).GetValueOrDefault()) - Main.screenPosition;
        }

        public static Vector2 GetDrawPosition(this ModTile modTile, int i, int j)
        {
            var tile = Main.tile[i, j];
            int style = 0;
            int alt = 0;
            TileObjectData.GetTileInfo(tile, ref style, ref alt);
            return GetDrawPosition(modTile, i, j, TileObjectData.GetTileData(tile.TileType, style, alt));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetDrawPosition(this ModTile modTile, Point pos)
        {
            return GetDrawPosition(modTile, pos.X, pos.Y);
        }
        #endregion

        #region SpriteBatch.Begin(...)
        private const SpriteSortMode SortMode_Default = SpriteSortMode.Deferred;
        private const SpriteSortMode SortMode_Shader = SpriteSortMode.Immediate;

        public static void Begin_Dusts(this SpriteBatch spriteBatch, bool immediate = false, Matrix? overrideMatrix = null)
        {
            spriteBatch.Begin(!immediate ? SortMode_Default : SortMode_Shader, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
        }

        public static void Begin_World(this SpriteBatch spriteBatch, bool shader = false, Matrix? overrideMatrix = null)
        {
            var matrix = overrideMatrix ?? Main.Transform;
            if (!shader)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, matrix);
            }
            else
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, matrix);
            }
        }

        public static void Begin_Tiles(this SpriteBatch spriteBatch, bool shader = false)
        {
            spriteBatch.Begin(!shader ? SortMode_Default : SortMode_Shader, null, null, null, null, null, Matrix.Identity);
        }

        public static void Begin_UI(this SpriteBatch spriteBatch, bool immediate = false, bool useScissorRectangle = false, Matrix? matrix = null)
        {
            RasterizerState rasterizer = null;
            if (useScissorRectangle)
            {
                rasterizer = new RasterizerState
                {
                    CullMode = CullMode.None,
                    ScissorTestEnable = true
                };
            }
            spriteBatch.Begin(!immediate ? SortMode_Default : SortMode_Shader, null, null, null, rasterizer, null, matrix ?? Main.UIScaleMatrix);
        }

        public static class UI
        {
            public static void Begin(SpriteBatch spriteBatch, SpriteSortMode spriteSort, bool useScissorRectangle = false)
            {
            }
            public static void BeginWMatrix(SpriteBatch spriteBatch, bool useScissorRectangle = false, Matrix? matrix = null)
            {
                BeginWMatrix(spriteBatch, SortMode_Default, useScissorRectangle, matrix);
            }
            public static void BeginWMatrix(SpriteBatch spriteBatch, SpriteSortMode spriteSort, bool useScissorRectangle = false, Matrix? matrix = null)
            {
                RasterizerState rasterizer = null;
                if (useScissorRectangle)
                {
                    rasterizer = new RasterizerState
                    {
                        CullMode = CullMode.None,
                        ScissorTestEnable = true
                    };
                }
                spriteBatch.Begin(spriteSort, null, null, null, rasterizer, null, matrix ?? Main.UIScaleMatrix);
            }
        }
        #endregion

        public static void DrawFlare(SpriteBatch sb, Vector2 position, Color color, Vector2 scaleVertical, Vector2 scaleHorizontal, float rotation = 0f) {
            var flare = AequusTextures.Flare.Value;
            var flareOrigin = flare.Size() / 2f;
            sb.Draw(flare, position, null, color, rotation, flareOrigin, scaleVertical, SpriteEffects.None, 0f);
            sb.Draw(flare, position, null, color, rotation + MathHelper.PiOver2, flareOrigin, scaleHorizontal, SpriteEffects.None, 0f);
        }
        
        public static void DrawUIPanel(SpriteBatch sb, Texture2D texture, Rectangle rect, Color color = default(Color))
        {
            Utils.DrawSplicedPanel(sb, texture, rect.X, rect.Y, rect.Width, rect.Height, 10, 10, 10, 10, color == default ? Color.White : color);
        }

        public static void DrawTrail(this ModProjectile modProjectile, Action<Vector2, float> draw)
        {
            int trailLength = ProjectileID.Sets.TrailCacheLength[modProjectile.Type];
            var offset = new Vector2(modProjectile.Projectile.width / 2f, modProjectile.Projectile.height / 2f);
            for (int i = 0; i < trailLength; i++)
            {
                draw(modProjectile.Projectile.oldPos[i] + offset, 1f - 1f / trailLength * i);
            }
        }

        public static void DrawFramedChain(Texture2D chain, Rectangle frame, Vector2 currentPosition, Vector2 endPosition, Vector2 screenPos, Func<Vector2, Color> getLighting = null)
        {
            if (getLighting == null)
            {
                getLighting = GetColor;
            }
            int height = frame.Height - 2;
            Vector2 velocity = endPosition - currentPosition;
            int length = (int)(velocity.Length() / height);
            velocity.Normalize();
            velocity *= height;
            float rotation = velocity.ToRotation() + MathHelper.PiOver2;
            var origin = new Vector2(frame.Width / 2f, frame.Height / 2f);
            for (int i = 0; i < length + 1; i++)
            {
                var position = currentPosition + velocity * i;
                Main.EntitySpriteDraw(chain, position - screenPos, frame, getLighting(position), rotation, origin, 1f, SpriteEffects.None, 0);
            }
        }
        public static void DrawChain(Texture2D chain, Vector2 currentPosition, Vector2 endPosition, Vector2 screenPos, Func<Vector2, Color> getLighting = null)
        {
            DrawFramedChain(chain, chain.Bounds, currentPosition, endPosition, screenPos, getLighting);
        }
    }
}