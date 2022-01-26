using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class SparrowWings : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.rare = ItemRarityID.LightRed;
            item.accessory = true;
            item.value = Item.sellPrice(gold: 6);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.wingTimeMax = 160;
        }

        public override bool WingUpdate(Player player, bool inUse)
        {
            if (inUse && Main.rand.NextBool(4))
            {
                int width = 20;
                int height = 20;
                Vector2 pos = player.direction == 1
                    ? new Vector2(player.position.X - width, player.position.Y + player.height / 2f - height)
                    : new Vector2(player.position.X + player.width, player.position.Y + player.height / 2f - height);
                if (player.gravDir == -1)
                    pos.Y += 8f;
                if (Main.rand.NextBool(4))
                {
                    Gore.NewGore(pos + new Vector2(Main.rand.Next(width), Main.rand.Next(height)), player.velocity * -0.2f, Main.rand.NextBool(4) ? 16 : 17, 0.4f);
                }
                else
                {
                    int d = Dust.NewDust(pos, width, height, 15, 0f, 0f, 0, default(Color), Main.rand.NextFloat(1f, 1.6f));
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity += -player.velocity;
                    Main.dust[d].velocity *= 0.1f;
                }
            }
            return false;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.75f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 0.75f;
            maxAscentMultiplier = 2f;
            constantAscend = 0.15f;
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 8f;
            acceleration *= 2f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FallenStar, 25);
            recipe.AddIngredient(ModContent.ItemType<CosmicEnergy>(), 5);
            recipe.AddIngredient(ModContent.ItemType<LightMatter>(), 8);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
