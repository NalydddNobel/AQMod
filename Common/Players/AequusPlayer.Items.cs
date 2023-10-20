using System;
using Terraria;

namespace Aequus;

public partial class AequusPlayer {
    public int itemHits;
    /// <summary>
    /// Tracks <see cref="Player.selectedItem"/>
    /// </summary>
    public int lastSelectedItem = -1;
    /// <summary>
    /// When a new cooldown is applied, this gets set to the duration of the cooldown. Does not tick down unlike <see cref="itemCooldown"/>
    /// </summary>
    public ushort itemCooldownMax;
    /// <summary>
    /// When above 0, the cooldown is active. Ticks down by 1 every player update.
    /// </summary>
    public ushort itemCooldown;
    /// <summary>
    /// When above 0, you are in a combo. Ticks down by 1 every player update.
    /// </summary>
    public ushort itemCombo;
    /// <summary>
    /// Increments when the player uses an item. Does not increment when the player is using the alt function of an item.
    /// </summary>
    public ushort itemUsage;
    /// <summary>
    /// A short lived timer which gets set to 30 when the player has a different selected item.
    /// </summary>
    public ushort itemSwitch;

    public void UpdateItemFields() {
        if (itemCombo > 0) {
            itemCombo--;
        }
        if (itemSwitch > 0) {
            itemUsage = 0;
            itemSwitch--;
        }
        else if (Player.itemTime > 0) {
            itemUsage++;
        }
        else {
            itemUsage = 0;
        }
        if (itemCooldown > 0) {
            if (itemCooldownMax == 0) {
                itemCooldown = 0;
                itemCooldownMax = 0;
            }
            else {
                itemCooldown--;
                if (itemCooldown == 0) {
                    itemCooldownMax = 0;
                }
            }
            Player.manaRegen = 0;
            Player.manaRegenDelay = (int)Player.maxRegenDelay;
        }
    }

    public override void PostItemCheck() {
        if (Player.selectedItem != lastSelectedItem) {
            lastSelectedItem = Player.selectedItem;
            itemSwitch = Math.Max((ushort)30, itemSwitch);
            itemUsage = 0;
            itemHits = 0;
        }
    }
}