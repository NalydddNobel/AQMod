using AequusRemake.Core.Graphics.Textures;
using System;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace AequusRemake.Content.Backpacks;

public abstract class BackpackData : ModType, ILocalizedModType {
    private const string InventoryTagKey = "Inventory";

    public int Type { get; internal set; }

    private Item[] _inventory;
    public Item[] Inventory { get => _inventory; internal set => _inventory = value; }
    public int slotCount;

    protected bool activeOld;

    public abstract int Capacity { get; }

    public virtual bool SupportsInfoAccessories => false;
    public virtual bool SupportsQuickStack => true;
    public virtual bool SupportsConsumeItem => true;
    public virtual bool SupportsGolfBalls => true;

    public virtual float SlotHue => 0f;

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

    protected virtual void OnUpdateItem(Player player, AequusPlayer AequusRemakePlayer, int slot) {
    }

    public virtual BackpackData CreateInstance() {
        return (BackpackData)MemberwiseClone();
    }

    protected virtual void SaveExtraData(TagCompound tag) { }

    protected virtual void LoadExtraData(TagCompound tag) { }

    public virtual bool CanAcceptItem(int slot, Item incomingItem) {
        return true;
    }

    /// <summary>Return false to override slot drawing.</summary>
    public virtual bool PreDrawSlot(SpriteBatch spriteBatch, Vector2 slotCenter, Vector2 slotTopLeft, int slot) {
        return true;
    }

    public virtual void PostDrawSlot(SpriteBatch spriteBatch, Vector2 slotCenter, Vector2 slotTopLeft, int slot) { }

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

    public void UpdateItem(Player player, AequusPlayer AequusRemakePlayer, int slot) {
        if (Inventory[slot].type == ItemID.DD2EnergyCrystal && !DD2Event.Ongoing) {
            Inventory[slot].TurnToAir();
        }
        OnUpdateItem(player, AequusRemakePlayer, slot);
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
                        IEntitySource source = new EntitySource_Misc("AequusRemake: Backpack");
                        for (int i = capacity; i < Inventory.Length; i++) {
                            player.QuickSpawnItem(source, Inventory[i], Inventory[i].stack);
                        }
                    }

                    Array.Resize(ref _inventory, capacity);
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

    #region Rendering
    private float _hueRenderedWith;
    public Texture2D InventoryBack { get; private set; }
    public Texture2D InventoryBackFavorited { get; private set; }
    public Texture2D InventoryBackNewItem { get; private set; }

    public bool CheckTextures() {
        float wantedHue = SlotHue;
        if (_hueRenderedWith != wantedHue) {
            GetHuedItemSlots(wantedHue);
            _hueRenderedWith = wantedHue;
        }

        return InventoryBack != null && InventoryBackFavorited != null && InventoryBackNewItem != null;
    }

    private void GetHuedItemSlots(float wantedHue) {
        Main.QueueMainThreadAction(() => {
            InventoryBack = HueSingleTexture2D(TextureAssets.InventoryBack.Value, wantedHue);
            InventoryBackFavorited = HueSingleTexture2D(TextureAssets.InventoryBack10.Value, wantedHue);
            InventoryBackNewItem = HueSingleTexture2D(TextureAssets.InventoryBack15.Value, wantedHue);
        });
    }

    private static Texture2D HueSingleTexture2D(Texture2D baseTexture, float hue) {
        return TextureGen.PerPixel(new EffectHueAdd(hue), baseTexture);
    }
    #endregion

    internal void EnsureCapacity(int capacity) {
        if (_inventory == null) {
            _inventory = new Item[capacity];
            UnNullItems();
        }
        else if (_inventory.Length < capacity) {
            Array.Resize(ref _inventory, capacity);
            UnNullItems();
        }
    }

    internal void UnNullItems() {
        for (int i = 0; i < _inventory.Length; i++) {
            if (_inventory[i] == null) {
                _inventory[i] = new Item();
            }
        }
    }

    /// <returns>Whether any items needed syncing.</returns>
    public bool SyncNetStates(Player player, BackpackData cloneBackpack) {
        if (Inventory == null || cloneBackpack.Inventory == null || cloneBackpack.Inventory.Length < Inventory.Length) {
            return false;
        }

        bool returnValue = false;
        for (int i = 0; i < Inventory.Length; i++) {
            if (Inventory[i] != null && cloneBackpack.Inventory[i] != null && Inventory[i].IsNetStateDifferent(cloneBackpack.Inventory[i])) {
                GetPacket<BackpackPlayerSyncPacket>().SendSingleItem(player, this, i);
                returnValue |= true;
            }
        }

        return returnValue;
    }

    public void CopyClientState(Player player, BackpackData cloneBackpack) {
        if (Inventory == null) {
            return;
        }

        cloneBackpack.EnsureCapacity(Inventory.Length);

        try {
            for (int i = 0; i < Inventory.Length; i++) {
                if (Inventory[i] == null) {
                    Inventory[i] = new();
                }
                if (cloneBackpack.Inventory[i] == null) {
                    cloneBackpack.Inventory[i] = new();
                }
                Inventory[i].CopyNetStateTo(cloneBackpack.Inventory[i]);
            }
        }
        catch (Exception ex) {
            cloneBackpack.UnNullItems();
            AequusRemake.Instance.Logger.Error(ex);
        }
    }
}