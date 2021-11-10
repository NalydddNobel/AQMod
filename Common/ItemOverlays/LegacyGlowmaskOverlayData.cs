using AQMod.Assets;
using AQMod.Assets.DrawCode;
using AQMod.Assets.ItemOverlays;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Common.ItemOverlays
{
    [Obsolete("Use Assets.ItemOverlays.GlowmaskOverlay instead")]
    public class LegacyGlowmaskOverlayData : ItemOverlayData
    {
        public readonly TextureAsset Texture;
        public Color drawColor;
        public Vector2 drawCoordinates;
        public Vector2 origin;
        public Rectangle drawFrame;
        /// <summary>
        /// Defaults to 0
        /// </summary>
        public int shader;
        public float drawRotation;

        public static Color DefaultGlowmaskColor => new Color(250, 250, 250, 0);

        internal LegacyGlowmaskOverlayData(string path)
        {
            Texture = new TextureAsset(path);
            drawColor = DefaultGlowmaskColor;
            shader = 0;
        }

        internal LegacyGlowmaskOverlayData(string path, Color color)
        {
            Texture = new TextureAsset(path);
            drawColor = color;
            shader = 0;
        }

        internal LegacyGlowmaskOverlayData(string path, Color color, int shader)
        {
            Texture = new TextureAsset(path);
            drawColor = color;
            this.shader = shader;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drawingOnPlayer">Whether this is called on the player's glowmask drawcode</param>
        /// <returns>Whether to draw the glowmask</returns>
        public virtual bool PreDraw(bool drawingOnPlayer, Item item)
        {
            return true;
        }

        public override void DrawHeld(Player player, AQPlayer aQPlayer, Item item, PlayerDrawInfo info)
        {
            var texture = Texture.GetValue();
            if (item.useStyle == ItemUseStyleID.HoldingOut)
            {
                if (Item.staff[item.type])
                {
                    drawRotation = info.drawPlayer.itemRotation + 0.785f * info.drawPlayer.direction;
                    int offsetX1 = 0;
                    int offsetY = 0;
                    origin = new Vector2(0f, Main.itemTexture[info.drawPlayer.inventory[info.drawPlayer.selectedItem].type].Height);
                    if (info.drawPlayer.gravDir == -1f)
                    {
                        if (info.drawPlayer.direction == -1)
                        {
                            drawRotation += 1.57f;
                            origin = new Vector2(Main.itemTexture[info.drawPlayer.inventory[info.drawPlayer.selectedItem].type].Width, 0f);
                            offsetX1 -= Main.itemTexture[info.drawPlayer.inventory[info.drawPlayer.selectedItem].type].Width;
                        }
                        else
                        {
                            drawRotation -= 1.57f;
                            origin = Vector2.Zero;
                        }
                    }
                    else if (info.drawPlayer.direction == -1)
                    {
                        origin = new Vector2(Main.itemTexture[info.drawPlayer.inventory[info.drawPlayer.selectedItem].type].Width, Main.itemTexture[info.drawPlayer.inventory[info.drawPlayer.selectedItem].type].Height);
                        offsetX1 -= Main.itemTexture[info.drawPlayer.inventory[info.drawPlayer.selectedItem].type].Width;
                    }
                    Vector2 holdoutOrigin = Vector2.Zero;
                    ItemLoader.HoldoutOrigin(info.drawPlayer, ref holdoutOrigin);
                    drawCoordinates = new Vector2((int)(info.itemLocation.X - Main.screenPosition.X + origin.X + offsetX1), (int)(info.itemLocation.Y - Main.screenPosition.Y + offsetY));
                    drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
                    origin += holdoutOrigin;
                    if (PreDraw(true, item))
                        Main.playerDrawData.Add(new DrawData(texture, drawCoordinates, drawFrame, drawColor, drawRotation, origin, item.scale, info.spriteEffects, 0) { shader = this.shader });
                    return;
                }
                SpriteEffects spriteEffects = (SpriteEffects)(player.gravDir != 1f ? player.direction != 1 ? 3 : 2 : player.direction != 1 ? 1 : 0);
                Vector2 offset = new Vector2(texture.Width / 2, texture.Height / 2);
                Vector2 holdoutOffset = item.modItem.HoldoutOffset().GetValueOrDefault(new Vector2(10f, 0f)) * player.gravDir;
                int offsetX = (int)holdoutOffset.X;
                offset.Y += holdoutOffset.Y;
                origin = player.direction == -1 ? new Vector2(texture.Width + offsetX, texture.Height / 2) : new Vector2(-offsetX, texture.Height / 2);
                drawCoordinates = new Vector2((int)(player.itemLocation.X - Main.screenPosition.X + offset.X), (int)(player.itemLocation.Y - Main.screenPosition.Y + offset.Y));
                drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
                drawRotation = player.itemRotation;
                if (PreDraw(true, item))
                    Main.playerDrawData.Add(new DrawData(texture, drawCoordinates, drawFrame, drawColor, drawRotation, origin, item.scale, spriteEffects, 0) { shader = this.shader });
                return;
            }
            if (player.gravDir == -1f)
            {
                drawCoordinates = new Vector2((int)(info.itemLocation.X - Main.screenPosition.X), (int)(info.itemLocation.Y - Main.screenPosition.Y));
                drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
                drawRotation = player.itemRotation;
                origin = new Vector2(texture.Width * 0.5f - texture.Width * 0.5f * player.direction, 0f);
                if (PreDraw(true, item))
                    Main.playerDrawData.Add(new DrawData(texture, drawCoordinates, drawFrame, drawColor, drawRotation, origin, item.scale, info.spriteEffects, 0) { shader = this.shader });
                return;
            }
            drawCoordinates = new Vector2((int)(info.itemLocation.X - Main.screenPosition.X), (int)(info.itemLocation.Y - Main.screenPosition.Y));
            drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
            drawRotation = player.itemRotation;
            origin = new Vector2(texture.Width * 0.5f - texture.Width * 0.5f * player.direction, texture.Height);
            if (PreDraw(true, item))
                Main.playerDrawData.Add(new DrawData(texture, drawCoordinates, drawFrame, drawColor, drawRotation, origin, item.scale, info.spriteEffects, 0) { shader = this.shader });
        }

        public override void PostDrawWorld(Item item, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            var texture = Texture.GetValue();
            drawCoordinates = new Vector2(item.position.X - Main.screenPosition.X + texture.Width / 2 + item.width / 2 - texture.Width / 2, item.position.Y - Main.screenPosition.Y + texture.Height / 2 + item.height - texture.Height + 2f);
            drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
            drawRotation = rotation;
            origin = Main.itemTexture[item.type].Size() / 2;
            if (PreDraw(false, item))
            {

                var drawData = new DrawData(texture, drawCoordinates, drawFrame, drawColor, drawRotation, origin, scale, SpriteEffects.None, 0);
                if (shader != 0)
                {
                    Main.spriteBatch.End();
                    BatcherMethods.StartShaderBatch_GeneralEntities(Main.spriteBatch);
                    GameShaders.Armor.Apply(shader, item, drawData);
                }
                drawData.Draw(Main.spriteBatch);
                if (shader != 0)
                {
                    Main.spriteBatch.End();
                    BatcherMethods.StartBatch_GeneralEntities(Main.spriteBatch);
                }
            }
        }
    }
}