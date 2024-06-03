using Aequus.Common.Items.Components;
using Aequus.Core.Graphics;
using Aequus.DataSets;
using Newtonsoft.Json;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Aequus.Content.Items.Tools.Keychain;

public partial class Keychain : ModItem, IRightClickOverrideWhenHovered, IOnConsumedInRecipe {
    public const int KEYS_FRAME_COUNT = 5;

    public override void Load() {
        if (!Main.dedServ) {
            KeyTextures = new Paletter(AequusTextures.PaletteKey, AequusTextures.KeychainKeysTemplate);
            Main.QueueMainThreadAction(KeyTextures.Load);
        }
    }

    public override void SetStaticDefaults() {
        ItemSets.WorksInVoidBag[Type] = true;
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

    bool IRightClickOverrideWhenHovered.RightClickOverrideWhenHovered(ref Item heldItem, Item[] inv, int context, int slot, Player player, AequusPlayer aequus) {
        if (Main.keyState.PressingControl() && Main.mouseRight && Main.mouseRightRelease) {
            Main.LocalPlayer.GetModPlayer<KeychainPlayer>().SpewItems(Main.LocalPlayer.GetSource_ItemUse(Item));
            return true;
        }

        if (heldItem == null || heldItem.IsAir || !ItemDataSet.KeychainData.TryGetValue(heldItem.type, out var value)) {
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

    public void OnConsumedInRecipe(Item createdItem, RecipeItemCreationContext context) {
        //Main.LocalPlayer.GetModPlayer<KeychainPlayer>().SpewItems(new EntitySource_Misc("Recipe"));
    }

    public override void OnResearched(bool fullyResearched) {
        //Main.LocalPlayer.GetModPlayer<KeychainPlayer>().SpewItems(new EntitySource_Misc("Research"));
    }

    public record struct KeychainInfo([JsonProperty] bool NonConsumable, [JsonProperty] Color Color);
}
