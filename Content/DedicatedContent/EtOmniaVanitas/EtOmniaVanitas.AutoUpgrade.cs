using Aequus.Core.UI;
using Terraria;
using Terraria.DataStructures;

namespace Aequus.Content.DedicatedContent.EtOmniaVanitas;

internal partial class EtOmniaVanitas {
    private GameProgression _checkAutoUpgrade;

    private bool CheckAutoUpgrade(bool playSound = true) {
        var newProgress = EtOmniaVanitasLoader.GetGameProgress();
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
        if (UISystem.TransformSlotContexts.Contains(UISystem.Slot.Context)) {
            CheckAutoUpgrade();
        }
    }
}