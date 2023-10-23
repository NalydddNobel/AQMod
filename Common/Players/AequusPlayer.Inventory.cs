using System;
using Terraria;
using Terraria.DataStructures;

namespace Aequus;

public partial class AequusPlayer {
    /// <summary>
    /// Do not use this for iteration, use extraInventory.Length instead. This stat simply resizes the array when needed during the player updated.
    /// </summary>
    public int extraInventorySlots;

    public Item[] extraInventory = new Item[0];

    private void CheckExtraInventoryMax() {
        if (extraInventorySlots > extraInventory.Length) {
            Array.Resize(ref extraInventory, extraInventorySlots);
            for (int i = 0; i < extraInventorySlots; i++) {
                if (extraInventory[i] == null) {
                    extraInventory[i] = new();
                }
            }
        }
        else if (extraInventorySlots >= 0 && extraInventorySlots < extraInventory.Length) {
            for (int i = extraInventory.Length - 1; i >= extraInventorySlots; i--) {
                Player.QuickSpawnItem(new EntitySource_Misc("Aequus: ExtraInventory"), extraInventory[i].Clone(), extraInventory[i].stack);
            }
            Array.Resize(ref extraInventory, extraInventorySlots);
        }
    }
}