using Aequus.NPCs.BossMonsters;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.RedSprite.Rewards {
    public class RedSpriteTrophy : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<Trophies>(), Trophies.RedSprite);
            Item.value = 50000;
            Item.rare = ItemRarityID.Blue;
            Item.Aequus().itemGravityCheck = 255;
        }
    }
}