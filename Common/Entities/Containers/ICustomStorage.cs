using ReLogic.Content;
using System.Collections.Generic;

namespace Aequus.Common.Entities.Containers;

public interface ICustomStorage {
    IEnumerable<Item> Items { get; }
    Asset<Texture2D> InventoryBackTexture => Terraria.GameContent.TextureAssets.InventoryBack5;

    void Open(Player player) { }
    void Close(Player player) { }
    void UpdateUI() { }
    bool CanTransferItems(Player player, Item itemToAccept, Item? itemInSlot, int slot) { return true; }
    void TransferItems(Player player, Item acceptedItem, Item itemInSlot, int slot);
}
