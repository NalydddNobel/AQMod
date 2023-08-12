using Aequus.Common.Carpentry;
using Aequus.Common.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc {
    public class CarpenterResetSheet : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults() {
            Item.DefaultToHoldUpItem();
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(gold: 1);
            Item.consumable = true;
            Item.UseSound = SoundID.Item92;
        }

        public override bool? UseItem(Player player) {
            if (Main.myPlayer == player.whoAmI)
                CarpentrySystem.ResetBounties();
            player.GetModPlayer<CarpentryPlayer>().CollectedBounties.Clear();
            player.GetModPlayer<CarpentryPlayer>().SelectedBounty = -1;
            return true;
        }
    }
}