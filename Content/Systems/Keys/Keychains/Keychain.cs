using Aequus.Common.Drawing.Generative;
using Aequus.Common.Entities.Containers;
using Aequus.Common.Entities.Items;
using System.Collections.Generic;
using Terraria.Audio;

namespace Aequus.Content.Systems.Keys.Keychains;

public partial class Keychain : ModItem, IItemSlotOverride, IStorageItem {
    public const int KEYS_FRAME_COUNT = 5;

#if !DEBUG
    public override bool IsLoadingEnabled(Mod mod) {
        return false;
    }
#endif

    public override void Load() {
        if (!Main.dedServ) {
            KeyTextures = new Paletter(AequusTextures.PaletteKey, AequusTextures.KeychainKeysTemplate);
            Main.QueueMainThreadAction(KeyTextures.Load);
        }
    }

    public override void SetStaticDefaults() {
        ItemID.Sets.WorksInVoidBag[Type] = true;
    }

    public override void Unload() {
        Main.QueueMainThreadAction(() => KeyTextures?.Dispose());
    }

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 16;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(gold: 1);
        Item.useAnimation = 30;
        Item.useTime = 30;
        Item.useStyle = ItemUseStyleID.Swing;
    }

    public override bool? UseItem(Player player) {
        player.GetModPlayer<StorageItemPlayer>().SetStorage(this);
        return true;
    }

    public override void UpdateInfoAccessory(Player player) {
        player.GetModPlayer<KeychainPlayer>().hasKeyChain = true;
    }

    bool IItemSlotOverride.RightClickSlot(ref Item heldItem, Item[] inv, int context, int slot, Player player, AequusPlayer aequus) {
        if (Main.keyState.PressingControl() && Main.mouseRight && Main.mouseRightRelease) {
            Main.LocalPlayer.GetModPlayer<KeychainPlayer>().SpewItems(Main.LocalPlayer.GetSource_ItemUse(Item));
            return true;
        }

        if (heldItem == null || heldItem.IsAir || !Instance<KeychainDatabase>().Values.TryGetValue(heldItem.type, out var value)) {
            return false;
        }

        KeychainPlayer keychain = Main.LocalPlayer.GetModPlayer<KeychainPlayer>();
        if (Main.mouseRight && Main.mouseRightRelease && keychain.AcceptItem(heldItem)) {
            keychain.RefreshKeys();
            SoundEngine.PlaySound(SoundID.Grab);
            return true;
        }

        return false;
    }

    IEnumerable<Item> ICustomStorage.Items {
        get {
            List<Item> keys = Main.LocalPlayer.GetModPlayer<KeychainPlayer>().GetFreshKeys();
            foreach (Item item in keys) {
                yield return item;
            }

            // Add a visual empty slot.
            if (keys.Count <= KeychainPlayer.MAX_KEYS_ALLOWED - 1) {
                yield return null!;
            }
        }
    }

    bool ICustomStorage.CanTransferItems(Player player, Item itemToAccept, Item? itemInSlot, int slot) {
        return Instance<KeychainDatabase>().Values.ContainsKey(itemToAccept.type);
    }

    void ICustomStorage.TransferItems(Player player, Item acceptedItem, Item itemInSlot, int slot) {
        player.GetModPlayer<KeychainPlayer>().AcceptItem(acceptedItem);
    }
}
