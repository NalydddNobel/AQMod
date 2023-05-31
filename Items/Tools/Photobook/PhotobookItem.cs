using Aequus.Content.Town.CarpenterNPC.Photobook.UI;
using Aequus.Items;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.Photobook {
    [LegacyName("Photobook")]
    public class PhotobookItem : ModItem, ItemHooks.IUpdateVoidBag {
        public const int PhotoStorage = 20;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(PhotoStorage);

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToHoldUpItem();
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = AequusSounds.photobookopen;
            Item.value = Item.buyPrice(gold: 1);
        }

        public override bool? UseItem(Player player) {
            if (Main.myPlayer == player.whoAmI) {
                Main.playerInventory = false;
                Main.npcChatText = "";
                Main.editChest = false;
                Main.editSign = false;
                if (Aequus.UserInterface.CurrentState != null) {
                    Aequus.UserInterface.SetState(null);
                    return true;
                }
                Aequus.UserInterface.SetState(new PhotobookUIState() { scale = 1.5f, });
            }
            return true;
        }

        public override void UpdateInventory(Player player) {
            player.GetModPlayer<PhotobookPlayer>().hasPhotobook = true;
        }

        public virtual void UpdateBank(Player player, AequusPlayer aequus, int slot, int bank) {
            player.GetModPlayer<PhotobookPlayer>().hasPhotobook = true;
        }
    }
}