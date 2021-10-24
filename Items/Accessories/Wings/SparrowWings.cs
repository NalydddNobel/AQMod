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
            player.wingTimeMax = 100;
        }

        public override bool WingUpdate(Player player, bool inUse)
        {
            if (inUse)
            {
                player.wingFrameCounter++;
                if (player.wingFrameCounter > 6)
                {
                    player.wingFrame++;
                    player.wingFrameCounter = 0;
                    if (player.wingFrame >= 3)
                        player.wingFrame = 0;
                }
                if (Main.rand.NextBool(7))
                {
                    int width = 20;
                    int height = 20;
                    Vector2 pos = player.direction == 1
                        ? new Vector2(player.position.X - width, player.position.Y + player.height / 2f - height)
                        : new Vector2(player.position.X + player.width, player.position.Y + player.height / 2f - height);
                    if (player.gravDir == -1)
                        pos.Y += 8f;
                    if (Main.rand.NextBool())
                    {
                        Gore.NewGore(pos + new Vector2(Main.rand.Next(width), Main.rand.Next(height)), player.velocity * -0.2f, 16 + Main.rand.Next(2), 0.5f);
                    }
                    else
                    {
                        Dust.NewDust(pos, width, height, 58);
                    }
                }
            }
            else if (player.velocity.Y != 0f)
            {
                player.wingFrame = 2;
            }
            else
            {
                player.wingFrame = 0;
            }
            return true;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.5f;
            ascentWhenRising = 0.1f;
            maxCanAscendMultiplier = 0.5f;
            maxAscentMultiplier = 1.5f;
            constantAscend = 0.1f;
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            //speed = 8f;
            //acceleration *= 2f;
            speed = 6f;
            acceleration *= 1.8f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FallenStar, 25);
            recipe.AddIngredient(ModContent.ItemType<CosmicEnergy>(), 15);
            recipe.AddIngredient(ItemID.SoulofFlight, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
