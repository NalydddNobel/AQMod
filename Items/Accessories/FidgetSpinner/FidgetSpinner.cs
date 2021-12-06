using AQMod.Common.DeveloperTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.FidgetSpinner
{
    public class FidgetSpinner : ModItem
    {
        public byte clr = 0;

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.accessory = true;
            item.rare = AQItem.Rarities.OmegaStariteRare;
            item.value = Item.buyPrice(gold: 50);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AQPlayer>().fidgetSpinner = true;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (clr != Constants.Paint.None)
            {
                if (clr == 255)
                {
                    var texture = ModContent.GetTexture(this.GetPath("_Rainbow"));
                    var glowTexture = ModContent.GetTexture(this.GetPath("_Rainbow_Glow"));
                    Main.spriteBatch.Draw(texture, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(glowTexture, position, frame, Main.DiscoColor, 0f, origin, scale, SpriteEffects.None, 0f);
                    return false;
                }
                else
                {
                    string path = this.GetPath("_" + Constants.Paint.GetClrName(clr));
                    if (ModContent.TextureExists(path))
                    {
                        var texture = ModContent.GetTexture(path);
                        Main.spriteBatch.Draw(texture, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
                        return false;
                    }
                }
            }
            return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (clr != Constants.Paint.None)
            {
                if (clr == 255)
                {
                    var texture = ModContent.GetTexture(this.GetPath("_Rainbow"));
                    var glowTexture = ModContent.GetTexture(this.GetPath("_Rainbow_Glow"));
                    var drawCoordinates = new Vector2(item.position.X - Main.screenPosition.X + texture.Width / 2 + item.width / 2 - texture.Width / 2, item.position.Y - Main.screenPosition.Y + texture.Height / 2 + item.height - texture.Height + 2f);
                    var drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
                    var drawRotation = rotation;
                    var origin = Main.itemTexture[item.type].Size() / 2;
                    Main.spriteBatch.Draw(texture, drawCoordinates, drawFrame, item.GetAlpha(lightColor), drawRotation, origin, scale, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(glowTexture, drawCoordinates, drawFrame, Main.DiscoColor, drawRotation, origin, scale, SpriteEffects.None, 0);
                    return false;
                }
                else
                {
                    string path = this.GetPath("_" + Constants.Paint.GetClrName(clr));
                    if (ModContent.TextureExists(path))
                    {
                        var texture = ModContent.GetTexture(path);
                        var drawCoordinates = new Vector2(item.position.X - Main.screenPosition.X + texture.Width / 2 + item.width / 2 - texture.Width / 2, item.position.Y - Main.screenPosition.Y + texture.Height / 2 + item.height - texture.Height + 2f);
                        var drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
                        var drawRotation = rotation;
                        var origin = Main.itemTexture[item.type].Size() / 2;
                        Main.spriteBatch.Draw(texture, drawCoordinates, drawFrame, item.GetAlpha(lightColor), drawRotation, origin, scale, SpriteEffects.None, 0);
                        return false;
                    }
                }
            }
            return base.PreDrawInWorld(spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
        }

        public override void AddRecipes()
        {
            clrRecipe(ItemID.RedDye, Constants.Paint.Red);
            clrRecipe(ItemID.OrangeDye, Constants.Paint.Orange);
            clrRecipe(ItemID.YellowDye, Constants.Paint.Yellow);
            clrRecipe(ItemID.LimeDye, Constants.Paint.Lime);
            clrRecipe(ItemID.GreenDye, Constants.Paint.Green);
            clrRecipe(ItemID.TealDye, Constants.Paint.Teal);
            clrRecipe(ItemID.CyanDye, Constants.Paint.Cyan);
            clrRecipe(ItemID.SkyBlueDye, Constants.Paint.SkyBlue);
            clrRecipe(ItemID.BlueDye, Constants.Paint.Blue);
            clrRecipe(ItemID.PurpleDye, Constants.Paint.Purple);
            clrRecipe(ItemID.VioletDye, Constants.Paint.Violet);
            clrRecipe(ItemID.BrownDye, Constants.Paint.Brown);
            clrRecipe(ItemID.SilverDye, Constants.Paint.None);
            clrRecipe(ItemID.ShadowDye, Constants.Paint.Shadow);
            clrRecipe(ItemID.RainbowDye, 255);
        }

        private void clrRecipe(int dye, byte clr)
        {
            AQRecipes.r_FidgetSpinnerRecipe.ConstructRecipe(dye, clr, this);
        }
    }
}