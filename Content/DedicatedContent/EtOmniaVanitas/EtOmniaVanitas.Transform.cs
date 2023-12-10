using Aequus.Common.Items.Components;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Aequus.Content.DedicatedContent.EtOmniaVanitas;

internal partial class EtOmniaVanitas {
    void ITransformItem.HoldItemTransform(Player player) {
        // Do nothing
    }

    public void Transform(Player player) {
        SoundEngine.PlaySound(SoundID.Unlock);

        var totalProgress = EtOmniaVanitasLoader.GetGameProgress();
        for (int i = (int)TierLock + 1; i <= (int)totalProgress; i++) {
            var progress = (GameProgression)i;
            if (EtOmniaVanitasLoader.ProgressionToItem.TryGetValue(progress, out var item)) {
                Item.Transform(item.Type);
                return;
            }
        }

        Item.Transform(EtOmniaVanitasLoader.Tier1.Type);
    }

    //public override void OnCreated(ItemCreationContext context) {
    //    if (context == null || context is InitializationItemCreationContext) {
    //        return;
    //    }

    //    _checkAutoUpgrade = TierLock;
    //    CheckAutoUpgrade(playSound: false);
    //    Mod.Logger.Debug(context.GetType().FullName);
    //}

    public bool UpgradeIntoStrongest(bool playSound) {
        for (int i = (int)EtOmniaVanitasLoader.GetGameProgress(); i > (int)TierLock; i--) {
            var progress = (GameProgression)i;
            if (EtOmniaVanitasLoader.ProgressionToItem.TryGetValue(progress, out var item)) {
                Item.Transform(item.Type);
                if (playSound) {
                    SoundEngine.PlaySound(SoundID.Unlock);
                }
                return true;
            }
        }
        return false;
    }
}