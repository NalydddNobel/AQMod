using Aequus.Common;
using Aequus.Common.Recipes;
using Aequus.Content.Town.CarpenterNPC.Misc;
using Aequus.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Unused.Items {
    public class GalaxyCommission : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 0;
        }

        public override void SetDefaults() {
            Item.DefaultToHoldUpItem();
            Item.width = 24;
            Item.height = 24;
            Item.consumable = true;
            Item.rare = ItemRarityID.Gray;
            Item.UseSound = SoundID.Item92;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void AddRecipes() {
            //AequusRecipes.AddShimmerCraft(ModContent.ItemType<ShutterstockerClipAmmo>(), ModContent.ItemType<GalaxyCommission>(), condition: AequusConditions.DownedOmegaStarite);
        }
    }
}