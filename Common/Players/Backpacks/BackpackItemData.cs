using Aequus.Common.Items.Components;
using Terraria;

namespace Aequus.Common.Players.Backpacks;

public abstract class BackpackItemData : BackpackData {
    public IStorageItem BackpackItem;

    private string _nameCache;

    public override Item[] Inventory => BackpackItem?.Inventory;

    public override bool IsActive(Player player) {
        return BackpackItem != null && BackpackItem.HasValidInventory;
    }

    public override string GetDisplayName(Player player) {
        if (BackpackItem != null) {
            _nameCache = BackpackItem.StorageDisplayName.Value;
        }
        if (IsActive(player) && !string.IsNullOrEmpty(_nameCache)) {
            return _nameCache;
        }
        return base.GetDisplayName(player);
    }

    public override void ResetEffects(Player player) {
        BackpackItem = null;
    }
}