using AQMod.Content.Players;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Recipes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories
{
    public class GlowString : DyeableAccessory, IUpdateVanity
    {
        public override string Texture => "Terraria/Item_" + ItemID.WhiteString;

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.sellPrice(gold: 2);
            item.rare = ItemRarityID.Green;
            item.accessory = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 100);
        }

        private void SetYoyoStringColor(Player player)
        {
            if (color == 255)
            {
                player.stringColor = 27;
            }
            else
            {
                player.stringColor = color;
            }
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.yoyoString = true;
            SetYoyoStringColor(player);
            player.GetModPlayer<AQPlayer>().glowString = true;
        }
        void IUpdateVanity.UpdateVanitySlot(Player player, AQPlayer aQPlayer, PlayerDrawEffects drawEffects, int slot)
        {
            SetYoyoStringColor(player);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var texture = Main.itemTexture[AQMod.Coloring.PaintToYoyoString[color]];
            for (float f = 0f; f < 1f; f += 0.125f)
            {
                Main.spriteBatch.Draw(texture, position + new Vector2(2f * scale, 0f).RotatedBy(Main.GlobalTime * MathHelper.PiOver4 + f * MathHelper.TwoPi), frame, new Color(50, 50, 50, 0), 0f, origin, scale, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(texture, position, frame, item.GetAlpha(drawColor), 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            try
            {
                var texture = Main.itemTexture[AQMod.Coloring.PaintToYoyoString[color]];
                var frame = texture.Frame();
                var origin = frame.Size() / 2f;
                var drawCoordinates = item.position - Main.screenPosition + origin + new Vector2(item.width / 2 - origin.X, item.height - frame.Height);
                var itemOrigin = frame.Size() / 2f;
                for (float f = 0f; f < 1f; f += 0.125f)
                {
                    Main.spriteBatch.Draw(texture, drawCoordinates + new Vector2(2f * scale, 0f).RotatedBy(Main.GlobalTime * MathHelper.PiOver4 + f * MathHelper.TwoPi), frame, new Color(50, 50, 50, 0), rotation, origin, scale, SpriteEffects.None, 0f);
                }

                var spotlight = ModContent.GetTexture("AQMod/Assets/Lights/Spotlight");
                Main.spriteBatch.Draw(spotlight, drawCoordinates, null, WorldGen.paintColor(color) * 0.2f, 0f, spotlight.Size() / 2f, scale * 0.9f, SpriteEffects.None, 0f);

                Main.spriteBatch.Draw(texture, drawCoordinates, frame, item.GetAlpha(lightColor), rotation, origin, scale, SpriteEffects.None, 0f);
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
            r.AddIngredient(ItemID.WhiteString);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>());
            r.AddTile(TileID.Loom);
            r.SetResult(this);
            r.AddRecipe();
            for (int i = 0; i < AQMod.Coloring.Paints.Length; i++)
            {
                var r2 = new DyeablesRecipe(mod, AQMod.Coloring.Paints[i]);
                r2.AddIngredient(item.type);
                r2.AddIngredient(AQMod.Coloring.PaintToDye[AQMod.Coloring.Paints[i]]);
                r2.AddTile(TileID.DyeVat);
                r2.SetResult(this);
                var g = (GlowString)r2.createItem.modItem;
                g.color = r2.SetColor;
                r2.AddRecipe();
                r2 = new DyeablesRecipe(mod, AQMod.Coloring.Paints[i]);
                r2.AddIngredient(AQMod.Coloring.PaintToYoyoString[AQMod.Coloring.Paints[i]]);
                r2.AddIngredient(ModContent.ItemType<CosmicEnergy>());
                r2.AddTile(TileID.DyeVat);
                r2.SetResult(this);
                g = (GlowString)r2.createItem.modItem;
                g.color = r2.SetColor;
                r2.AddRecipe();
            }
        }
    }
}