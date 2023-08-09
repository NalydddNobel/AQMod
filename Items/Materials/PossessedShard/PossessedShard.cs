using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Materials.PossessedShard {
    public class PossessedShard : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 10;
        }

        public override void SetDefaults() {
            Item.width = 14;
            Item.height = 14;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemDefaults.RarityEarlyHardmode - 1;
            Item.value = Item.sellPrice(silver: 7);
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.Lerp(Color.White, lightColor, Helper.Wave(Main.GlobalTimeWrappedHourly * 2.5f, 0.66f, 1f));
        }
    }
}