using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class EdgelorthWings : ModItem
    {
        public override bool Autoload(ref string name)
        {
            return AQMod.AprilFools;
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.rare = ItemRarityID.Yellow;
            item.accessory = true;
            item.value = Item.sellPrice(gold: 12);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.wingTimeMax = 166;
        }

        public override bool WingUpdate(Player player, bool inUse)
        {
            if (inUse)
            {
                player.wingFrameCounter++;
                if (player.wingFrameCounter > 4)
                {
                    player.wingFrame++;
                    player.wingFrameCounter = 0;
                    if (player.wingFrame >= 6)
                        player.wingFrame = 0;
                }
            }
            else if (player.velocity.Y != 0f)
            {
                player.wingFrame = 0;
            }
            else
            {
                player.wingFrame = 2;
            }
            return true;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 1f;
            ascentWhenRising = 0.2666f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 1.666f;
            constantAscend = 0.2666f;
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 16.666f;
            acceleration *= 2.666f;
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
