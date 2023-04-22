using Aequus.Content;
using Aequus.Content.Town.PhysicistNPC.Analysis;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Utility
{
    public class HaltingMagnet : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            SentryAccessoriesDatabase.OnAI.Add(Type, SentryAccessoriesDatabase.ApplyEquipFunctional_AI);
            AnalysisSystem.IgnoreItem.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.accessory = true;
            Item.rare = ItemRarityID.Orange;
            Item.hasVanityEffects = true;
            Item.value = Item.buyPrice(gold: 6);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().antiGravityItemRadius += 360f;
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