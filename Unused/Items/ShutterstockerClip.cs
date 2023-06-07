using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Unused.Items {
    public class ShutterstockerClip : ModItem {
        public override void SetDefaults() {
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Gray;
        }
    }

    public class ShutterstockerClipAmmo : ModItem {
        public override string Texture => ModContent.GetInstance<ShutterstockerClip>().Texture;

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults() {
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Gray;
            Item.value = Item.buyPrice(gold: 1);
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
        }
    }
}