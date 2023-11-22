using Aequus.Common.Items.Components;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Players.Backpacks;

public abstract class BackpackData : ModType, ILocalizedModType {
    public int Type { get; internal set; }

    public abstract Item[] Inventory { get; }
    public int slotCount;

    protected bool activeOld;

    public virtual bool SupportsInfoAccessories => false;
    public virtual bool SupportsQuickStack => true;
    public virtual bool SupportsConsumeItem => true;
    public virtual bool SupportsGolfBalls => true;

    public virtual Color SlotColor => Color.White;
    public virtual Color FavoritedSlotColor => SlotColor.SaturationMultiply(0.5f) * 1.33f;
    public virtual Color NewAndShinySlotColor => SlotColor.HueAdd(0.05f) * 1.25f;

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
        return BackpackLoader.Backpacks[Type].displayName.Value;
    }

    public virtual string GetBuilderSlotText(int state) {
        return (state == 0 ? builderSlotTextOn : builderSlotTextOff).Value;
    }

    public abstract bool IsActive(Player player);

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
    public virtual void Deactivate(Player player, AequusPlayer aequusPlayer) {
    }

    public void Update(Player player) {
        var aequusPlayer = player.GetModPlayer<AequusPlayer>();
        OnUpdate(player, aequusPlayer);
        bool isActive = IsActive(player);
        if (activeOld != isActive) {
            if (activeOld) {
                Deactivate(player, aequusPlayer);
            }
            else {
                Activate(player, aequusPlayer);
                slotCount = Inventory.Length;
            }
            activeOld = isActive;
        }
    }
    protected virtual void OnUpdate(Player player, AequusPlayer aequusPlayer) {
    }

    public virtual void ResetEffects(Player player) {
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

    public virtual void SaveData(TagCompound tag) {
    }

    public virtual void LoadData(TagCompound tag) {
    }
}