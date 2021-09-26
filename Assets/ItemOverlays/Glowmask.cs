using AQMod.Assets.Textures;
using AQMod.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Assets.ItemOverlays
{
    public class Glowmask : ItemOverlay
    {
        public readonly GlowID glowmask;

        public readonly Color drawColor;

        public static Color DefaultDrawColor => new Color(250, 250, 250, 0);

        public Glowmask(GlowID glowmask)
        {
            this.glowmask = glowmask;
            drawColor = DefaultDrawColor;
        }

        public Glowmask(GlowID glowmask, Color drawColor)
        {
            this.glowmask = glowmask;
            this.drawColor = drawColor;
        }

        public override void DrawHeld(Player player, AQPlayer aQPlayer, Item item, PlayerDrawInfo info)
        {
            Texture2D texture = SpriteUtils.Textures.Glows[glowmask];
            if (item.useStyle == ItemUseStyleID.HoldingOut)
            {
                if (Item.staff[item.type])
                {
                    float rotation = info.drawPlayer.itemRotation + 0.785f * info.drawPlayer.direction;
                    int offsetX1 = 0;
                    int offsetY = 0;
                    Vector2 origin = new Vector2(0f, Main.itemTexture[info.drawPlayer.inventory[info.drawPlayer.selectedItem].type].Height);
                    if (info.drawPlayer.gravDir == -1f)
                    {
                        if (info.drawPlayer.direction == -1)
                        {
                            rotation += 1.57f;
                            origin = new Vector2(Main.itemTexture[info.drawPlayer.inventory[info.drawPlayer.selectedItem].type].Width, 0f);
                            offsetX1 -= Main.itemTexture[info.drawPlayer.inventory[info.drawPlayer.selectedItem].type].Width;
                        }
                        else
                        {
                            rotation -= 1.57f;
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
                    Main.playerDrawData.Add(new DrawData(texture, new Vector2((int)(info.itemLocation.X - Main.screenPosition.X + origin.X + offsetX1), (int)(info.itemLocation.Y - Main.screenPosition.Y + offsetY)), new Rectangle(0, 0, texture.Width, texture.Height), drawColor, rotation, origin + holdoutOrigin, item.scale, info.spriteEffects, 0));
                    return;
                }
                SpriteEffects spriteEffects = (SpriteEffects)(player.gravDir != 1f ? player.direction != 1 ? 3 : 2 : player.direction != 1 ? 1 : 0);
                Vector2 offset = new Vector2(texture.Width / 2, texture.Height / 2);
                Vector2 holdoutOffset = item.modItem.HoldoutOffset().GetValueOrDefault(new Vector2(10f, 0f)) * player.gravDir;
                int offsetX = (int)holdoutOffset.X;
                offset.Y += holdoutOffset.Y;
                Vector2 origin6 = player.direction == -1 ? new Vector2(texture.Width + offsetX, texture.Height / 2) : new Vector2(-offsetX, texture.Height / 2);
                DrawData drawData = new DrawData(texture, new Vector2((int)(player.itemLocation.X - Main.screenPosition.X + offset.X), (int)(player.itemLocation.Y - Main.screenPosition.Y + offset.Y)), (Rectangle?)new Rectangle(0, 0, texture.Width, texture.Height), drawColor, player.itemRotation, origin6, item.scale, spriteEffects, 0);
                Main.playerDrawData.Add(drawData);
                return;
            }
            if (player.gravDir == -1f)
            {
                DrawData drawData = new DrawData(texture, new Vector2((int)(info.itemLocation.X - Main.screenPosition.X), (int)(info.itemLocation.Y - Main.screenPosition.Y)), new Rectangle(0, 0, texture.Width, texture.Height), item.GetAlpha(drawColor), player.itemRotation, new Vector2(texture.Width * 0.5f - texture.Width * 0.5f * player.direction, 0f), item.scale, info.spriteEffects, 0);
                Main.playerDrawData.Add(drawData);
                return;
            }
            DrawData drawData1 = new DrawData(texture, new Vector2((int)(info.itemLocation.X - Main.screenPosition.X), (int)(info.itemLocation.Y - Main.screenPosition.Y)), new Rectangle(0, 0, texture.Width, texture.Height), item.GetAlpha(drawColor), player.itemRotation, new Vector2(texture.Width * 0.5f - texture.Width * 0.5f * player.direction, texture.Height), item.scale, info.spriteEffects, 0);
            Main.playerDrawData.Add(drawData1);
        }

        public override void DrawWorld(Item item, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            var texture = SpriteUtils.Textures.Glows[glowmask];
            Vector2 drawPosition = new Vector2(item.position.X - Main.screenPosition.X + texture.Width / 2 + item.width / 2 - texture.Width / 2, item.position.Y - Main.screenPosition.Y + texture.Height / 2 + item.height - texture.Height + 2f);
            Main.spriteBatch.Draw(texture, drawPosition, null, drawColor, rotation, Main.itemTexture[item.type].Size() / 2, scale, SpriteEffects.None, 0f);
        }
    }
}