using Aequus.Content.Equipment.Accessories.AccCrowns.Blood;
using Aequus.Core.Generator;
using Terraria;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects]
    public bool accBloodCrown;
    public int _bloodCrownUUID;

    private void UpdateCrownOfBlood() {
        if (!accBloodCrown) {
            _bloodCrownUUID = 0;
            return;
        }

        var player = Player;
        var equip = player.armor[CrownOfBlood.SlotId];
        if (equip == null || equip.IsAir) {
            _bloodCrownUUID = 0;
            return;
        }
        
        // Update UUID
        if (equip.TryGetGlobalItem<CrownOfBloodGlobalItem>(out var globalItem) && (globalItem._uuid != _bloodCrownUUID || _bloodCrownUUID == 0)) {
            int uuid = Main.rand.Next();
            _bloodCrownUUID = uuid;
            globalItem._uuid = uuid;
        }

        // Record previous stats for comparison
        if (Main.myPlayer == player.whoAmI) {
            foreach (var tracker in Boosts.TrackedStats) {
                tracker.Record(player);
            }
        }

        player.ApplyEquipFunctional(equip, player.hideVisibleAccessory[CrownOfBlood.SlotId]);
        if (equip.wingSlot != -1) {
            player.wingTimeMax *= 2;
        }

        // Measure before-and-after stats for tooltip
        if (Main.myPlayer == player.whoAmI) {
            foreach (var tracker in Boosts.TrackedStats) {
                tracker.Measure(player);
            }
        }
    }
}