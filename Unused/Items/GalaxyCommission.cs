using Aequus.Common;
using Aequus.Common.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Unused.Items {
    [UnusedContent]
    public class GalaxyCommission : ModItem {
        public override void SetDefaults() {
            Item.DefaultToHoldUpItem();
            Item.width = 24;
            Item.height = 24;
            Item.consumable = true;
            Item.rare = ItemDefaults.RarityShimmerPermanentUpgrade;
            Item.UseSound = SoundID.Item92;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void AddRecipes() {
            //AequusRecipes.AddShimmerCraft(ModContent.ItemType<ShutterstockerClipAmmo>(), ModContent.ItemType<GalaxyCommission>(), condition: AequusConditions.DownedOmegaStarite);
        }
    }
}