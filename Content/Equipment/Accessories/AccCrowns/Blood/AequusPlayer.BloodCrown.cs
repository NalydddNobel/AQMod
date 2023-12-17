using Aequus.Content.Equipment.Accessories.AccCrowns.Blood;
using Aequus.Core.Generator;
using Terraria;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects]
    public bool accBloodCrown;

    private void UpdateBloodCrownInner(Item equip) {
        var player = Player;
        player.ApplyEquipFunctional(equip, player.hideVisibleAccessory[BloodCrown.SlotId]);
        if (equip.wingSlot != -1) {
            player.wingTimeMax *= 2;
        }
    }

    private void UpdateBloodCrown() {
        if (!accBloodCrown) {
            BloodCrown.TooltipUUID = 0;
            return;
        }

        var player = Player;
        var equip = player.armor[BloodCrown.SlotId];
        if (equip == null || equip.IsAir) {
            BloodCrown.TooltipUUID = 0;
            return;
        }
        
        if (Main.myPlayer == player.whoAmI && (BloodCrown.statTickUpdate == 0 || BloodCrown.TooltipUUID == 0)) {
            // Update UUID
            if (equip.TryGetGlobalItem<BloodCrownGlobalItem>(out var globalItem) && (globalItem._localUUID != BloodCrown.TooltipUUID || BloodCrown.TooltipUUID == 0)) {
                int uuid = Main.rand.Next();
                BloodCrown.TooltipUUID = uuid;
                globalItem._localUUID = uuid;
            }

            // Record previous stats for comparison
            BloodCrown.StatComparer.Record(player);

            int prefix = equip.prefix;
            equip.prefix = 0;
            try {
                UpdateBloodCrownInner(equip);
            }
            catch {
            }
            equip.prefix = prefix;

            // Measure before-and-after stats for tooltip
            BloodCrown.StatComparer.Measure(player);

            BloodCrown.statTickUpdate = BloodCrown.StatTickUpdateRate;
        }
        else {
            if (Main.myPlayer == player.whoAmI && BloodCrown.statTickUpdate > 0) {
                BloodCrown.statTickUpdate--;
            }
            UpdateBloodCrownInner(equip);
        }
    }
}