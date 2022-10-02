using Aequus.Items.Accessories.Summon.Sentry;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Utility
{
    public class HaltingMagnet : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            SentryAccessoriesDatabase.OnAI.Add(Type, SentryAccessoriesDatabase.ApplyEquipFunctional_AI);
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.accessory = true;
            Item.rare = ItemDefaults.RarityOmegaStarite + 1;
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            Item.value = Item.buyPrice(gold: 6);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().antiGravityItemRadius = 360f;
            player.treasureMagnet = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TreasureMagnet)
                .AddIngredient<HaltingMachine>()
                .AddTile(TileID.TinkerersWorkbench)
                .TryRegisterAfter(ItemID.ArchitectGizmoPack);
        }
    }
}