using Aequus.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Aequus.Items.Accessories
{
    public class GlowCore : DyeableAccessory
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(silver: 5);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
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
                var texture = TextureAssets.Item[Type].Value;
                var coloring = DyeColor();
                for (float f = 0f; f < 1f; f += 0.125f)
                {
                    Main.spriteBatch.Draw(texture, position + new Vector2(2f * scale, 0f).RotatedBy(Main.GlobalTimeWrappedHourly * MathHelper.PiOver4 + f * MathHelper.TwoPi), frame, coloring.UseA(0) * 0.7f, 0f, origin, scale, SpriteEffects.None, 0f);
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
                var texture = TextureAssets.Item[Type].Value;
                var frame = texture.Frame();
                var origin = frame.Size() / 2f;
                var drawCoordinates = Item.position - Main.screenPosition + origin + new Vector2(Item.width / 2 - origin.X, Item.height - frame.Height);
                var itemOrigin = frame.Size() / 2f;
                var coloring = DyeColor();
                foreach (var v in AequusHelpers.CircularVector(4))
                {
                    Main.spriteBatch.Draw(texture, drawCoordinates + v * scale, frame, coloring.UseA(0) * 0.3f, rotation, origin, scale, SpriteEffects.None, 0f);
                }

                var spotlight = TextureCache.Bloom[0].Value;
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
            ColorRecipes<GlowCore>();
        }
    }
}