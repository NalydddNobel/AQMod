using System;
using Terraria;
using Terraria.Localization;

namespace Aequus.Common.Items.Components;

public interface IStorageItem {
    Item[] Inventory { get; set; }
    LocalizedText StorageDisplayName { get; }

    public void Deposit(Item[] inv) {
        int amt = Math.Min(Inventory.Length, inv.Length);
        for (int i = 0; i < amt; i++) {
            if (Inventory[i] == null || Inventory[i].IsAir) {
                continue;
            }

            inv[i] = Inventory[i].Clone();
        }
    }

    public void Inherit(Item backpack, Item[] inv) {
        if (Inventory == null || Inventory.Length != inv.Length) {
            Inventory = new Item[inv.Length];
        }

        bool anyChanges = false;
        for (int i = 0; i < inv.Length; i++) {
            if (Inventory[i] == null) {
                Inventory[i] = new();
            }

            if (Inventory[i].IsNetStateDifferent(inv[i])) {
                Inventory[i] = inv[i].Clone();
                anyChanges = true;
            }
        }
        if (anyChanges) {
            backpack.NetStateChanged();
        }
    }

    public void EnsureInventory(int slotAmount) {
        Inventory ??= new Item[slotAmount];
        if (Inventory.Length != slotAmount) {
            var inventory = Inventory;
            Array.Resize(ref inventory, slotAmount);
            Inventory = inventory;
        }
        for (int i = 0; i < slotAmount; i++) {
            if (Inventory[i] == null) {
                Inventory[i] = new();
            }
        }
    }
}