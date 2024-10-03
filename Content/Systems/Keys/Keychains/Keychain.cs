using Aequus.Common.Drawing.Generative;
using Aequus.Common.Entities.Items;
using Terraria.Audio;

namespace Aequus.Content.Systems.Keys.Keychains;

public partial class Keychain : ModItem, IItemSlotOverride {
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
    }

    public override void UpdateInfoAccessory(Player player) {
        player.GetModPlayer<KeychainPlayer>().hasKeyChain = true;
    }

    bool IItemSlotOverride.RightClickSlot(ref Item heldItem, Item[] inv, int context, int slot, Player player, AequusPlayer aequus) {
        if (Main.keyState.PressingControl() && Main.mouseRight && Main.mouseRightRelease) {
            Main.LocalPlayer.GetModPlayer<KeychainPlayer>().SpewItems(Main.LocalPlayer.GetSource_ItemUse(Item));
            return true;
        }

        var values = Instance<KeychainDatabase>().Values;
        if (heldItem == null || heldItem.IsAir || !values.TryGetValue(heldItem.type, out var value)) {
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
}
