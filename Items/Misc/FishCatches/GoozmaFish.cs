using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.FishCatches {
    public class GoozmaFish : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 2;
        }

        public override void SetDefaults() {
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item2;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(gold: 2);
        }
    }
}