using Aequus;
using Aequus.Items.Consumables.Palettes;
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
            OnOpenBag.WoodenCratePool.Add(Type);
            CavernPalette.CavernChestLoot.Add(Type);
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
            return new Color(255, 255, 255, 255);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            byte value = color;
            if (color != 255)
            {
                value++;
            }
            player.Aequus().glowCore = value;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            try
            {
                var texture = TextureAssets.Item[Type].Value;

                AequusPlayer.teamContext = Main.LocalPlayer.team;
                var coloring = DyeColor();
                AequusPlayer.teamContext = 0;

                foreach (var v in AequusHelpers.CircularVector(4))
                {
                    Main.spriteBatch.Draw(texture, position + v * scale * 2f, frame, coloring.UseA(0) * 0.7f, 0f, origin, scale, SpriteEffects.None, 0f);
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

                if (Item.playerIndexTheItemIsReservedFor >= 0 && Item.playerIndexTheItemIsReservedFor != 255)
                {
                    AequusPlayer.teamContext = Main.player[Item.playerIndexTheItemIsReservedFor].team;
                }
                else
                {
                    AequusPlayer.teamContext = 0;
                }

                var coloring = DyeColor();
                AequusPlayer.teamContext = 0;

                foreach (var v in AequusHelpers.CircularVector(4))
                {
                    Main.spriteBatch.Draw(texture, drawCoordinates + v * scale * 2f, frame, coloring.UseA(0) * 0.3f, rotation, origin, scale, SpriteEffects.None, 0f);
                }

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

        public static void AddLight(Entity entity, byte glowCore, int playerTeam = -1)
        {
            if (glowCore != 255)
            {
                glowCore--;
            }
            Lighting.AddLight(entity.Center, DyeColor(glowCore).ToVector3() * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.7f, 0.9f));
        }
    }
}