using Aequus.Common.UI;
using Terraria;
using Terraria.DataStructures;
using Terraria.UI;

namespace Aequus.Content.DedicatedContent.EtOmniaVanitas;

internal partial class EtOmniaVanitas {
    private GameProgression _checkAutoUpgrade;

    private bool CheckAutoUpgrade(bool playSound = true) {
        var newProgress = GetGameProgress();
        if (_checkAutoUpgrade == newProgress) {
            return false;
        }

        _checkAutoUpgrade = newProgress;
        return UpgradeIntoStrongest(playSound);
    }

    public override void UpdateInventory(Player player) {
        CheckAutoUpgrade();
    }

    public override void OnCreated(ItemCreationContext context) {
        if (context is RecipeItemCreationContext || context is JourneyDuplicationItemCreationContext) {
            _checkAutoUpgrade = TierLock;
            CheckAutoUpgrade(playSound: false);
        }
    }

    private void CheckAutoUpgradeWhenDrawing() {
        if (ValidItemTransformSlotContexts.Contains(UISystem.CurrentItemSlot.Context)) {
            CheckAutoUpgrade();
        }
    }
}