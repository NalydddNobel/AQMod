using Aequus.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Bait
{
    public class IchorPopper : ModItem, IModifyFishingPower
    {
        public override void SetDefaults()
        {
            Item.width = 6;
            Item.height = 6;
            Item.bait = 31;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.value = Item.sellPrice(silver: 1);
            Item.rare = ItemRarityID.Green;
        }

        void IModifyFishingPower.ModifyFishingPower(Player player, PlayerFishing fishing, Item fishingRod, ref float fishingLevel)
        {
            if (player.ZoneCrimson)
                fishingLevel += 35;
        }

        //public override void OnEnterWater(Player player, PlayerFishing fishing, Projectile bobber, Tile tile)
        //{
        //    Projectile.NewProjectile(bobber.Center + new Vector2(0f, (byte.MaxValue - tile.liquid) / 16), Vector2.Zero, ModContent.ProjectileType<IchorPopperEffect>(), 0, 0f, player.whoAmI);
        //}

        //public override void AddRecipes()
        //{
        //    var r = new ModRecipe(mod);
        //    r.AddIngredient(ItemID.Ichor, 10);
        //    r.AddIngredient(ItemID.BloodWater);
        //    r.AddTile(ModContent.TileType<FishingCraftingStationTile>());
        //    r.SetResult(this, 10);
        //    r.AddRecipe();
        //}
    }
}