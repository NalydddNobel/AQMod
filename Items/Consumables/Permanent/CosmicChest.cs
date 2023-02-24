﻿using Aequus.NPCs.ExporterNPC.Shop;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Permanent
{
    public class CosmicChest : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToHoldUpItem();
            Item.width = 24;
            Item.height = 24;
            Item.consumable = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item92;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override bool? UseItem(Player player)
        {
            if (!player.Aequus().usedPermaLootLuck)
            {
                player.Aequus().usedPermaLootLuck = true;
                return true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            AequusRecipes.CreateShimmerTransmutation(ModContent.ItemType<GrandReward>(), ModContent.ItemType<CosmicChest>(), condition: AequusRecipes.ConditionOmegaStarite);
        }
    }
}