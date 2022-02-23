using AQMod.Content.Players;
using AQMod.Projectiles.Fishing.PopperEffects;
using AQMod.Tiles.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools.Fishing.Bait
{
    public class MysticPopper : PopperBaitItem
    {
        public override void SetDefaults()
        {
            item.width = 6;
            item.height = 6;
            item.bait = 50;
            item.maxStack = 999;
            item.consumable = true;
            item.value = Item.sellPrice(silver: 1);
            item.rare = ItemRarityID.Green;
        }

        public override int GetExtraFishingPower(Player player, PlayerFishing fishing)
        {
            if (player.ZoneHoly)
                return 20;
            return 0;
        }

        public override void OnEnterWater(Player player, PlayerFishing fishing, Projectile bobber, Tile tile)
        {
            Projectile.NewProjectile(bobber.Center + new Vector2(0f, (byte.MaxValue - tile.liquid) / 16), Vector2.Zero, ModContent.ProjectileType<MysticPopperEffect>(), 0, 0f, player.whoAmI);
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.PixieDust, 10);
            r.AddIngredient(ItemID.UnicornHorn);
            r.AddIngredient(ItemID.HolyWater);
            r.AddTile(ModContent.TileType<FishingCraftingStation>());
            r.SetResult(this, 10);
            r.AddRecipe();
        }
    }
}