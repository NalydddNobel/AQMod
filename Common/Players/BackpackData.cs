using Aequus.Common.Items.Components;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Players;

public abstract class BackpackData : ModType, ILocalizedModType {
    public int Type { get; internal set; }

    public Item[] Inventory { get; private set; }
    protected bool activeOld;
    public bool Active { get; private set; }

    public abstract int Slots { get; }
    public virtual bool SupportsInfoAccessories => false;
    public virtual bool SupportsQuickStack => true;
    public virtual bool SupportsConsumeItem => true;

    public virtual Color SlotColor => Color.White;
    public int slotsToRender;
    public float nextSlotAnimation;

    public string LocalizationCategory => "Items.Backpacks";

    public LocalizedText DisplayName => this.GetLocalization("DisplayName", PrettyPrintName);
    public LocalizedText BuilderSlotTextOn => this.GetLocalization("BackpackEnabled", () => "");
    public LocalizedText BuilderSlotTextOff => this.GetLocalization("BackpackDisabled", () => "");

    public LocalizedText displayName { get; private set; }
    public LocalizedText builderSlotTextOn { get; private set; }
    public LocalizedText builderSlotTextOff { get; private set; }

    public virtual string GetDisplayName(Player player) {
        return displayName.Value;
    }

    public virtual string GetBuilderSlotText(int state) {
        return (state == 0 ? builderSlotTextOn : builderSlotTextOff).Value;
    }

    public abstract bool IsActive(Player player, AequusPlayer aequusPlayer);

    public virtual bool IsVisible() {
        return true;
    }

    protected override void Register() {
        Type = BackpackLoader.Count;
        BackpackLoader.Backpacks.Add(this);
        displayName = DisplayName;
        builderSlotTextOn = BuilderSlotTextOn;
        builderSlotTextOff = BuilderSlotTextOff;
    }

    public virtual void Activate(Player player, AequusPlayer aequusPlayer) {
    }

    /// <summary>
    /// Runs when the Backpack is no longer active. Use this hook to deposit items from the Backpack.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="aequusPlayer"></param>
    public abstract void Deactivate(Player player, AequusPlayer aequusPlayer);

    public void Update(Player player) {
        var aequusPlayer = player.GetModPlayer<AequusPlayer>();
        OnResetEffects(player, aequusPlayer);
        Active = IsActive(player, aequusPlayer);
        if (activeOld != Active) {
            if (activeOld) {
                Deactivate(player, aequusPlayer);
            }
            else {
                Inventory = new Item[Slots];
                for (int i = 0; i < Inventory.Length; i++) {
                    Inventory[i] = new();
                }
                Activate(player, aequusPlayer);
            }
            activeOld = Active;
        }
    }
    protected virtual void OnResetEffects(Player player, AequusPlayer aequusPlayer) {
    }

    public void UpdateItem(Player player, AequusPlayer aequusPlayer, int slot) {
        if (Inventory[slot].type == ItemID.DD2EnergyCrystal && !DD2Event.Ongoing) {
            Inventory[slot].TurnToAir();
        }
        OnUpdateItem(player, aequusPlayer, slot);
    }
    protected virtual void OnUpdateItem(Player player, AequusPlayer aequusPlayer, int slot) {
    }

    public virtual bool CanAcceptItem(int slot, Item incomingItem) {
        return incomingItem.ModItem is not IStorageItem;
    }

    public BackpackData CreateInstance() {
        return (BackpackData)Activator.CreateInstance(GetType());
    }

    public void SaveData(TagCompound tag) {
        if (Active) {
            tag[FullName] = Inventory;
        }
    }

    public void LoadData(TagCompound tag) {
        if (!tag.TryGet<Item[]>(FullName, out var arr)) {
            return;
        }

        Active = true;
        Inventory = arr;
    }
}