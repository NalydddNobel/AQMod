using Aequus.Common.Items.Tooltips;
using Microsoft.Xna.Framework;
using System;
using Terraria.Localization;

namespace Aequus.Common.Items.Components;

public interface IStorageItem {
    /// <summary>
    /// Whether or not this backpack has a valid inventory. This is used to prevent duping exploits using Autopause and weird abusing instance crap.
    /// </summary>
    bool HasValidInventory { get; set; }
    Item[] Inventory { get; set; }
    LocalizedText StorageDisplayName { get; }

    public void Deposit(Item[] inv) {
        int amt = Math.Min(Inventory.Length, inv.Length);
        for (int i = 0; i < amt; i++) {
            if (Inventory[i] == null || Inventory[i].IsAir) {
                continue;
            }

            inv[i] = Inventory[i];
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

    public void AddKeywords(Item item) {
        Keyword tooltip = new(StorageDisplayName.Value, Color.Lerp(Color.SaddleBrown * 1.5f, Color.White, 0.75f), item.type);
        string items = "";
        tooltip.AddLine(Language.GetTextValue("Mods.Aequus.Misc.Contains", ""));
        int itemsAmt = 0;
        for (int i = 0; i < Inventory.Length; i++) {
            if (Inventory[i] != null && !Inventory[i].IsAir) {
                if (!string.IsNullOrEmpty(items)) {
                    items += "  ";
                }
                if (itemsAmt % 5 == 0) {
                    tooltip.AddLine(items);
                    items = "";
                }
                itemsAmt++;
                items += $"[i/s{Inventory[i].stack}:{Inventory[i].type}]";
            }
        }

        if (!string.IsNullOrEmpty(items)) {
            tooltip.AddLine(items);
        }
        if (tooltip.tooltipLines.Count > 1) {
            tooltip.AddLine(TextHelper.AirString);
            KeywordSystem.Tooltips.Add(tooltip);
        }
    }
}