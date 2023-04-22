﻿using Aequus.Common;
using Aequus.Common.Recipes;
using Aequus.Content.Town.CarpenterNPC.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Permanent
{
    public class GalaxyCommission : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToHoldUpItem();
            Item.width = 24;
            Item.height = 24;
            Item.consumable = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item92;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override bool? UseItem(Player player)
        {
            if (!player.Aequus().usedPermaBuildBuffRange)
            {
                player.Aequus().usedPermaBuildBuffRange = true;
                return true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            AequusRecipes.AddShimmerCraft(ModContent.ItemType<ShutterstockerClipAmmo>(), ModContent.ItemType<GalaxyCommission>(), condition: AequusConditions.DownedOmegaStarite);
        }
    }
}