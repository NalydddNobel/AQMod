using AQMod.Assets.ItemOverlays;
using AQMod.Common.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Assets.Graphics.ItemOverlays
{
    public class EnchantmentOverlay : ItemOverlayData
    {
        public override void DrawHeld(Player player, AQPlayer aQPlayer, Item item, PlayerDrawInfo info)
        {
            var texture = item.GetTexture();
            Vector2 drawCoordinates;
            float drawRotation;
            Vector2 origin;
            Rectangle drawFrame;
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
                    Main.playerDrawData.Add(new DrawData(texture, drawCoordinates, drawFrame, new Color(255, 255, 255, 255), drawRotation, origin, item.scale, info.spriteEffects, 0) { shader = GameShaders.Armor.GetShaderIdFromItemId(ModContent.ItemType<Items.Vanities.Dyes.EnchantedDye>()) });
                    return;
                }
                var spriteEffects = (SpriteEffects)(player.gravDir != 1f ? player.direction != 1 ? 3 : 2 : player.direction != 1 ? 1 : 0);
                var offset = new Vector2(texture.Width / 2, texture.Height / 2);
                Vector2 holdoutOffset = item.modItem.HoldoutOffset().GetValueOrDefault(new Vector2(10f, 0f)) * player.gravDir;
                int offsetX = (int)holdoutOffset.X;
                offset.Y += holdoutOffset.Y;
                origin = player.direction == -1 ? new Vector2(texture.Width + offsetX, texture.Height / 2) : new Vector2(-offsetX, texture.Height / 2);
                drawCoordinates = new Vector2((int)(player.itemLocation.X - Main.screenPosition.X + offset.X), (int)(player.itemLocation.Y - Main.screenPosition.Y + offset.Y));
                drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
                drawRotation = player.itemRotation;
                Main.playerDrawData.Add(new DrawData(texture, drawCoordinates, drawFrame, new Color(255, 255, 255, 255), drawRotation, origin, item.scale, spriteEffects, 0) { shader = GameShaders.Armor.GetShaderIdFromItemId(ModContent.ItemType<Items.Vanities.Dyes.EnchantedDye>()) });
                return;
            }
            if (player.gravDir == -1f)
            {
                drawCoordinates = new Vector2((int)(info.itemLocation.X - Main.screenPosition.X), (int)(info.itemLocation.Y - Main.screenPosition.Y));
                drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
                drawRotation = player.itemRotation;
                origin = new Vector2(texture.Width * 0.5f - texture.Width * 0.5f * player.direction, 0f);
                Main.playerDrawData.Add(new DrawData(texture, drawCoordinates, drawFrame, new Color(255, 255, 255, 255), drawRotation, origin, item.scale, info.spriteEffects, 0) { shader = GameShaders.Armor.GetShaderIdFromItemId(ModContent.ItemType<Items.Vanities.Dyes.EnchantedDye>()) });
                return;
            }
            drawCoordinates = new Vector2((int)(info.itemLocation.X - Main.screenPosition.X), (int)(info.itemLocation.Y - Main.screenPosition.Y));
            drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
            drawRotation = player.itemRotation;
            origin = new Vector2(texture.Width * 0.5f - texture.Width * 0.5f * player.direction, texture.Height);
            Main.playerDrawData.Add(new DrawData(texture, drawCoordinates, drawFrame, new Color(255, 255, 255, 255), drawRotation, origin, item.scale, info.spriteEffects, 0) { shader = GameShaders.Armor.GetShaderIdFromItemId(ModContent.ItemType<Items.Vanities.Dyes.EnchantedDye>()) });
        }

        public override bool PreDrawWorld(Item item, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Main.spriteBatch.End();
            BatcherMethods.GeneralEntities.BeginShader(Main.spriteBatch);

            var frame = new Rectangle(0, 0, Main.itemTexture[item.type].Width, Main.itemTexture[item.type].Height);
            var drawPosition = new Vector2(item.position.X - Main.screenPosition.X + frame.Width / 2 + item.width / 2 - frame.Width / 2, item.position.Y - Main.screenPosition.Y + frame.Height / 2 + item.height - frame.Height);
            Vector2 origin = frame.Size() / 2f;
            var drawData = new DrawData(Main.itemTexture[item.type], drawPosition, frame, item.GetAlpha(lightColor), rotation, origin, scale, SpriteEffects.None, 0);

            var effect = EffectCache.s_Enchant;
            effect.Apply(drawData);
            drawData.Draw(Main.spriteBatch);

            Main.spriteBatch.End();
            BatcherMethods.GeneralEntities.Begin(Main.spriteBatch);
            return true;
        }

        public override bool PreDrawInventory(Player player, AQPlayer aQPlayer, Item item, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Main.spriteBatch.End();
            BatcherMethods.UI.Begin(Main.spriteBatch, BatcherMethods.Shader);
            var drawData = new DrawData(item.GetTexture(), position, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0);
            var effect = EffectCache.s_Enchant;
            effect.Apply(drawData);
            drawData.Draw(Main.spriteBatch);
            Main.spriteBatch.End();
            BatcherMethods.UI.Begin(Main.spriteBatch, BatcherMethods.Regular);
            return true;
        }
    }
}