using Aequus.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Unused.Items {
    [UnusedContent]
    public class ShutterstockerClip : ModItem {
        public override void SetDefaults() {
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Blue;
        }
    }

    [UnusedContent]
    public class ShutterstockerClipAmmo : ModItem {
        public override string Texture => ModContent.GetInstance<ShutterstockerClip>().Texture;

        public override void SetDefaults() {
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
        }
    }
}