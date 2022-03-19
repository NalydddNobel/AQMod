using AQMod.Items.Recipes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories
{
    public sealed class GlowBand : DyeableAccessory
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.sellPrice(silver: 5);
            item.rare = ItemRarityID.Green;
            item.accessory = true;
            item.defense = 1;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 200);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            Lighting.AddLight(player.Center, DyeColor().ToVector3() * 0.8f);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            try
            {
                var texture = Main.itemTexture[item.type];
                var coloring = AQUtils.MultColorsThenDiv(DyeColor(), item.GetAlpha(drawColor));
                for (float f = 0f; f < 1f; f += 0.125f)
                {
                    Main.spriteBatch.Draw(texture, position + new Vector2(2f * scale, 0f).RotatedBy(Main.GlobalTime * MathHelper.PiOver4 + f * MathHelper.TwoPi), frame, coloring.UseA(0) * 0.7f, 0f, origin, scale, SpriteEffects.None, 0f);
                }
                spriteBatch.Draw(texture, position, frame, coloring, 0f, origin, scale, SpriteEffects.None, 0f);
            }
            catch
            {
                return true;
            }
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            try
            {
                var texture = Main.itemTexture[Coloring.PaintToYoyoString[color]];
                var frame = texture.Frame();
                var origin = frame.Size() / 2f;
                var drawCoordinates = item.position - Main.screenPosition + origin + new Vector2(item.width / 2 - origin.X, item.height - frame.Height);
                var itemOrigin = frame.Size() / 2f;
                var coloring = AQUtils.MultColorsThenDiv(DyeColor(), item.GetAlpha(lightColor));
                for (float f = 0f; f < 1f; f += 0.125f)
                {
                    Main.spriteBatch.Draw(texture, drawCoordinates + new Vector2(2f * scale, 0f).RotatedBy(Main.GlobalTime * MathHelper.PiOver4 + f * MathHelper.TwoPi), frame, coloring.UseA(0) * 0.3f, rotation, origin, scale, SpriteEffects.None, 0f);
                }

                var spotlight = ModContent.GetTexture("AQMod/Assets/Lights/Spotlight");
                Main.spriteBatch.Draw(spotlight, drawCoordinates, null, DyeColor() * 0.2f, 0f, spotlight.Size() / 2f, scale * 0.9f, SpriteEffects.None, 0f);

                Main.spriteBatch.Draw(texture, drawCoordinates, frame, coloring, rotation, origin, scale, SpriteEffects.None, 0f);
            }
            catch
            {
                return true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Shackle);
            r.AddIngredient(ItemID.Glowstick, 50);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(this);
            r.AddRecipe();
            for (int i = 0; i < Coloring.Paints.Length; i++)
            {
                var r2 = new DyeablesRecipe(mod, Coloring.Paints[i]);
                r2.AddIngredient(item.type);
                r2.AddIngredient(Coloring.PaintToDye[Coloring.Paints[i]]);
                r2.AddTile(TileID.DyeVat);
                r2.SetResult(this);
                var dyeable = (DyeableAccessory)r2.createItem.modItem;
                dyeable.color = r2.SetColor;
                r2.AddRecipe();

                r2 = new DyeablesRecipe(mod, Coloring.Paints[i]);
                r2.AddIngredient(ItemID.Shackle);
                r2.AddIngredient(Coloring.PaintToDye[Coloring.Paints[i]]);
                r2.AddIngredient(ItemID.Glowstick, 50);
                r2.AddTile(TileID.WorkBenches);
                r2.SetResult(this);
                dyeable = (DyeableAccessory)r2.createItem.modItem;
                dyeable.color = r2.SetColor;
                r2.AddRecipe();
            }
        }
    }
}