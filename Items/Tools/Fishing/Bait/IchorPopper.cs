using AQMod.Effects.WorldEffects;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools.Fishing.Bait
{
    public class IchorPopper : PopperBaitItem
    {
        public override void SetDefaults()
        {
            item.width = 6;
            item.height = 6;
            item.bait = 31;
            item.maxStack = 999;
            item.consumable = true;
            item.value = Item.sellPrice(silver: 1);
            item.rare = ItemRarityID.Green;
        }

        public override int GetExtraFishingPower(Player player, AQPlayer aQPlayer)
        {
            if (player.ZoneCrimson)
                return 35;
            return 0;
        }

        public override void PopperEffects(Player player, AQPlayer aQPlayer, Projectile bobber, Tile tile)
        {
            AQMod.WorldEffects.Add(new FishingPopperEffect((int)bobber.position.X, (int)bobber.position.Y, tile.liquid, 170, default(Color)));
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Ichor, 10);
            r.AddIngredient(ItemID.BloodWater);
            r.AddTile(ModContent.TileType<Tiles.FishingCraftingStation>());
            r.SetResult(this, 10);
            r.AddRecipe();
        }
    }
}