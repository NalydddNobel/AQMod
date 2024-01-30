using System;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Backpacks;

public abstract class BackpackData : ModType, ILocalizedModType {
    private const String InventoryTagKey = "Inventory";

    public Int32 Type { get; internal set; }

    public Item[] Inventory { get; internal set; }
    public Int32 slotCount;

    protected Boolean activeOld;

    public abstract Int32 Capacity { get; }

    public virtual Boolean SupportsInfoAccessories => false;
    public virtual Boolean SupportsQuickStack => true;
    public virtual Boolean SupportsConsumeItem => true;
    public virtual Boolean SupportsGolfBalls => true;

    public virtual Single SlotHue => 0f;

    public Int32 slotsToRender;
    public Single nextSlotAnimation;

    public virtual String LocalizationCategory => "Items.Backpacks";

    public virtual LocalizedText DisplayName => this.GetLocalization("DisplayName", PrettyPrintName);

    protected LocalizedText DisplayNameCache { get; private set; }

    public virtual String GetDisplayName(Player player) {
        return BackpackLoader.Backpacks[Type].DisplayNameCache.Value;
    }

    public abstract Boolean IsActive(Player player);

    public virtual Boolean IsVisible() {
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

    protected virtual void OnUpdateItem(Player player, AequusPlayer aequusPlayer, Int32 slot) {
    }

    public virtual BackpackData CreateInstance() => (BackpackData)MemberwiseClone();

    protected virtual void SaveExtraData(TagCompound tag) { }

    protected virtual void LoadExtraData(TagCompound tag) { }

    public virtual Boolean CanAcceptItem(Int32 slot, Item incomingItem) => true;

    /// <summary>Return false to override slot drawing.</summary>
    public virtual Boolean PreDrawSlot(SpriteBatch spriteBatch, Vector2 slotCenter, Vector2 slotTopLeft, Int32 slot) => true;
    public virtual void PostDrawSlot(SpriteBatch spriteBatch, Vector2 slotCenter, Vector2 slotTopLeft, Int32 slot) { }

    public void SaveData(TagCompound tag) {
        SaveExtraData(tag);
        if (Inventory != null) {
            for (Int32 i = 0; i < Inventory.Length; i++) {
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

    public void UpdateItem(Player player, AequusPlayer aequusPlayer, Int32 slot) {
        if (Inventory[slot].type == ItemID.DD2EnergyCrystal && !DD2Event.Ongoing) {
            Inventory[slot].TurnToAir();
        }
        OnUpdateItem(player, aequusPlayer, slot);
    }

    public void Update(Player player) {
        OnUpdate(player);
        Boolean isActive = IsActive(player);
        if (activeOld != isActive) {
            if (activeOld) {
                Deactivate(player);
            }
            else {
                Activate(player);

                Int32 capacity = Capacity;
                if (Inventory == null) {
                    Inventory = new Item[capacity];
                }
                else {
                    if (capacity < Inventory.Length) {
                        IEntitySource source = new EntitySource_Misc("Aequus: Backpack");
                        for (Int32 i = capacity; i < Inventory.Length; i++) {
                            player.QuickSpawnItem(source, Inventory[i], Inventory[i].stack);
                        }
                    }

                    Item[] arr = Inventory;
                    Array.Resize(ref arr, capacity);
                    Inventory = arr;
                }

                for (Int32 i = 0; i < Inventory.Length; i++) {
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

    private Single _hueRenderedWith;
    public Texture2D InventoryBack { get; private set; }
    public Texture2D InventoryBackFavorited { get; private set; }
    public Texture2D InventoryBackNewItem { get; private set; }

    public Boolean CheckTextures() {
        Single wantedHue = SlotHue;
        if (_hueRenderedWith != wantedHue) {
            GetHuedItemSlots(wantedHue);
        }

        return InventoryBack != null && InventoryBackFavorited != null && InventoryBackNewItem != null;
    }

    private void GetHuedItemSlots(Single wantedHue) {
        Main.QueueMainThreadAction(() => {
            InventoryBack = HueSingleTexture2D(TextureAssets.InventoryBack.Value, wantedHue);
            InventoryBackFavorited = HueSingleTexture2D(TextureAssets.InventoryBack10.Value, wantedHue);
            InventoryBackNewItem = HueSingleTexture2D(TextureAssets.InventoryBack15.Value, wantedHue);
        });
    }

    private static Texture2D HueSingleTexture2D(Texture2D baseTexture, Single hue) {
        Color[] textureColorsExtracted = new Color[baseTexture.Width * baseTexture.Height];
        baseTexture.GetData(textureColorsExtracted);

        for (Int32 k = 0; k < textureColorsExtracted.Length; k++) {
            Color color = textureColorsExtracted[k];
            Byte velocity = Math.Max(Math.Max(color.R, color.G), color.B);
            textureColorsExtracted[k] = color.HueAdd(hue) with { A = color.A };
        }

        try {
            Texture2D resultTexture = new Texture2D(Main.instance.GraphicsDevice, baseTexture.Width, baseTexture.Height);
            resultTexture.SetData(textureColorsExtracted);
            return resultTexture;
        }
        catch (Exception ex) {
            Aequus.Log.Error(ex);

            // return null if error occurs, this will retry rendering the textures next frame
            return null;
        }
    }
}