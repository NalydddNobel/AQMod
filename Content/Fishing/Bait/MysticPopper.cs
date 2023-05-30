using Aequus.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Fishing.Bait {
    public class MysticPopper : ModItem, ItemHooks.IModifyFishingPower
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 6;
            Item.height = 6;
            Item.bait = 50;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.value = Item.sellPrice(silver: 1);
            Item.rare = ItemRarityID.Green;
        }

        void ItemHooks.IModifyFishingPower.ModifyFishingPower(Player player, AequusPlayer fishing, Item fishingRod, ref float fishingLevel)
        {
            if (player.ZoneHallow)
                fishingLevel += 0.2f;
        }

        //public override void OnEnterWater(Player player, PlayerFishing fishing, Projectile bobber, Tile tile)
        //{
        //    Projectile.NewProjectile(bobber.Center + new Vector2(0f, (byte.MaxValue - tile.liquid) / 16), Vector2.Zero, ModContent.ProjectileType<MysticPopperEffect>(), 0, 0f, player.whoAmI);
        //}

        public override void AddRecipes()
        {
            CreateRecipe(10)
                .AddIngredient(ItemID.HolyWater, 10)
                .AddIngredient(ItemID.PixieDust, 10)
                .AddIngredient(ItemID.UnicornHorn, 1)
                .AddTile(TileID.Bottles)
                .TryRegisterBefore(ItemID.EnchantedNightcrawler);
        }
    }
}