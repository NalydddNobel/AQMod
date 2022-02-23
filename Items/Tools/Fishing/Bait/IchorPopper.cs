using AQMod.Content.Players;
using AQMod.Projectiles.Fishing.PopperEffects;
using AQMod.Tiles.Furniture;
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

        public override int GetExtraFishingPower(Player player, PlayerFishing fishing)
        {
            if (player.ZoneCrimson)
                return 35;
            return 0;
        }

        public override void OnEnterWater(Player player, PlayerFishing fishing, Projectile bobber, Tile tile)
        {
            Projectile.NewProjectile(bobber.Center + new Vector2(0f, (byte.MaxValue - tile.liquid) / 16), Vector2.Zero, ModContent.ProjectileType<IchorPopperEffect>(), 0, 0f, player.whoAmI);
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Ichor, 10);
            r.AddIngredient(ItemID.BloodWater);
            r.AddTile(ModContent.TileType<FishingCraftingStation>());
            r.SetResult(this, 10);
            r.AddRecipe();
        }
    }
}