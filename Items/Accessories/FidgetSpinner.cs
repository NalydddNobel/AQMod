using AQMod.Items.Recipes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories
{
    public class FidgetSpinner : DyeableAccessory
    {
        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.accessory = true;
            item.rare = AQItem.Rarities.OmegaStariteRare;
            item.value = Item.buyPrice(gold: 80);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AQPlayer>().fidgetSpinner = true;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (color != 0)
            {
                var texture = Main.itemTexture[item.type];
                var masktexture = ModContent.GetTexture(this.GetPath("_Mask"));
                drawColor = item.GetAlpha(drawColor);
                Main.spriteBatch.Draw(texture, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
                if (color == 255)
                {
                    drawColor = Main.DiscoColor;
                }
                Main.spriteBatch.Draw(masktexture, position, frame, (DyeColor() * drawColor.Brightness()).UseA(drawColor.A), 0f, origin, scale, SpriteEffects.None, 0f);
                return false;
            }
            return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (color != 0)
            {
                var texture = Main.itemTexture[item.type];
                var masktexture = ModContent.GetTexture(this.GetPath("_Mask"));
                var drawCoordinates = new Vector2(item.position.X - Main.screenPosition.X + texture.Width / 2 + item.width / 2 - texture.Width / 2, item.position.Y - Main.screenPosition.Y + texture.Height / 2 + item.height - texture.Height + 2f);
                var drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
                var drawRotation = rotation;
                var origin = Main.itemTexture[item.type].Size() / 2;
                Color drawColor = item.GetAlpha(lightColor);
                Main.spriteBatch.Draw(texture, drawCoordinates, drawFrame, drawColor, drawRotation, origin, scale, SpriteEffects.None, 0);
                if (color == 255)
                {
                    drawColor = Main.DiscoColor;
                }
                Main.spriteBatch.Draw(masktexture, drawCoordinates, drawFrame, (DyeColor() * drawColor.Brightness()).UseA(drawColor.A), drawRotation, origin, scale, SpriteEffects.None, 0);
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            for (int i = 0; i < Coloring.Paints.Length; i++)
            {
                var r = new DyeablesRecipe(mod, Coloring.Paints[i]);
                r.AddIngredient(ModContent.ItemType<FidgetSpinner>());
                r.AddIngredient(Coloring.PaintToDye[Coloring.Paints[i]]);
                r.AddTile(TileID.DyeVat);
                r.SetResult(this);
                var s = (FidgetSpinner)r.createItem.modItem;
                s.color = r.SetColor;
                r.AddRecipe();
            }
        }
    }
}