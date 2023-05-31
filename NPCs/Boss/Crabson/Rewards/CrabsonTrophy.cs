using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Boss.Crabson.Rewards {
    public class CrabsonTrophy : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<Trophies>(), Trophies.Crabson);
            Item.value = 50000;
            Item.rare = ItemRarityID.Blue;
        }
    }
}