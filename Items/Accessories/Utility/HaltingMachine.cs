using Aequus.Content;
using Aequus.Content.Town.PhysicistNPC.Analysis;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Utility
{
    public class HaltingMachine : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            SentryAccessoriesDatabase.OnAI.Add(Type, SentryAccessoriesDatabase.ApplyEquipFunctional_AI);
            AnalysisSystem.IgnoreItem.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            Item.value = Item.buyPrice(gold: 5);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().antiGravityItemRadius = 300f;
        }
    }
}