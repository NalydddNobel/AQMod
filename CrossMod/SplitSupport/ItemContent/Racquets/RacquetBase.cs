using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.CrossMod.SplitSupport.ItemContent.Racquets {
    public abstract class RacquetBase : ModItem {
        public abstract int BallCount { get; }

        public override bool IsLoadingEnabled(Mod mod) {
            return Aequus.DevelopmentFeatures;
        }

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 0;
        }

        public override void SetDefaults() {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.DamageType = DamageClass.Melee;
        }
    }
}