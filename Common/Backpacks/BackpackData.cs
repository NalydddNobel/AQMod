using System;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Backpacks;

public abstract class BackpackData : ModType, ILocalizedModType {
    private const string InventoryTagKey = "Inventory";

    public int Type { get; internal set; }

    public Item[] Inventory { get; internal set; }
    public int slotCount;

    protected bool activeOld;

    public abstract int Capacity { get; }

    public virtual bool SupportsInfoAccessories => false;
    public virtual bool SupportsQuickStack => true;
    public virtual bool SupportsConsumeItem => true;
    public virtual bool SupportsGolfBalls => true;

    public virtual Color SlotColor => Color.White;
    public virtual Color FavoritedSlotColor => SlotColor.SaturationMultiply(0.5f) * 1.33f;
    public virtual Color NewAndShinySlotColor => SlotColor.HueAdd(0.05f) * 1.25f;

    public int slotsToRender;
    public float nextSlotAnimation;

    public virtual string LocalizationCategory => "Items.Backpacks";

    public virtual LocalizedText DisplayName => this.GetLocalization("DisplayName", PrettyPrintName);

    protected LocalizedText DisplayNameCache { get; private set; }

    public virtual string GetDisplayName(Player player) {
        return BackpackLoader.Backpacks[Type].DisplayNameCache.Value;
    }

    public abstract bool IsActive(Player player);

    public virtual bool IsVisible() {
        return true;
    }

    protected virtual void OnUpdate(Player player) {
    }
    public virtual void Activate(Player player) {
    }

    /// <summary>
    /// Runs when the Backpack is no longer active. Use this hook to deposit items from the Backpack.
    /// </summary>
    /// <param name="player"></param>
    public virtual void Deactivate(Player player) {
    }

    public virtual void ResetEffects(Player player) {
    }

    protected virtual void OnUpdateItem(Player player, AequusPlayer aequusPlayer, int slot) {
    }

    public virtual BackpackData CreateInstance() => (BackpackData)MemberwiseClone();

    protected virtual void SaveExtraData(TagCompound tag) { }

    protected virtual void LoadExtraData(TagCompound tag) { }

    public virtual bool CanAcceptItem(int slot, Item incomingItem) => true;

    /// <summary>Return false to override slot drawing.</summary>
    public virtual bool PreDrawSlot(Vector2 position, int slot) {
        return true;
    }
    public virtual void PostDrawSlot(Vector2 position, int slot) { }

    public void SaveData(TagCompound tag) {
        SaveExtraData(tag);
        if (Inventory != null) {
            for (int i = 0; i < Inventory.Length; i++) {
                // Save inventory if there's atleast 1 item inside the backpack
                if (Inventory[i] != null && !Inventory[i].IsAir) {
                    tag[InventoryTagKey] = Inventory;
                    break;
                }
            }
        }
    }

    public void LoadData(TagCompound tag) {
        if (tag.TryGet(InventoryTagKey, out Item[] inventory)) {
            Inventory = inventory;
        }
        LoadExtraData(tag);
    }

    public void UpdateItem(Player player, AequusPlayer aequusPlayer, int slot) {
        if (Inventory[slot].type == ItemID.DD2EnergyCrystal && !DD2Event.Ongoing) {
            Inventory[slot].TurnToAir();
        }
        OnUpdateItem(player, aequusPlayer, slot);
    }

    public void Update(Player player) {
        OnUpdate(player);
        bool isActive = IsActive(player);
        if (activeOld != isActive) {
            if (activeOld) {
                Deactivate(player);
            }
            else {
                Activate(player);

                int capacity = Capacity;
                if (Inventory == null) {
                    Inventory = new Item[capacity];
                }
                else {
                    if (capacity < Inventory.Length) {
                        IEntitySource source = new EntitySource_Misc("Aequus: Backpack");
                        for (int i = capacity; i < Inventory.Length; i++) {
                            player.QuickSpawnItem(source, Inventory[i], Inventory[i].stack);
                        }
                    }

                    Item[] arr = Inventory;
                    Array.Resize(ref arr, capacity);
                    Inventory = arr;
                }

                for (int i = 0; i < Inventory.Length; i++) {
                    Inventory[i] ??= new();
                }

                slotCount = Inventory.Length;
            }
            activeOld = isActive;
        }
    }

    protected override void Register() {
        Type = BackpackLoader.Count;
        BackpackLoader.Backpacks.Add(this);
        DisplayNameCache = DisplayName;
    }
}