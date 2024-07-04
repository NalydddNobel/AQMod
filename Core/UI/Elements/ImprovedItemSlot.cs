using Terraria.GameContent.Events;
using Terraria.UI;

namespace AequusRemake.Core.UI.Elements;

public class ImprovedItemSlot : UIElement {
    private UIElement _icon;

    public readonly int Context;
    public readonly float Scale;

    public readonly float MaxItemDimensions;

    public bool CanHover { get; set; }
    public bool ShowItemTooltip { get; set; }

    protected Item _item;
    public Item Item { get => _item; set => _item = value; }
    public bool HasItem => _item != null && !_item.IsAir;

    #region Events
    public delegate void HoverItemDelegate(Item item);
    public delegate void ItemSwapDelegate(Item mouseItem, Item slotItem);
    public delegate bool ItemTransferConditionDelegate(Item item);

    public event HoverItemDelegate WhileHoveringItem;
    public event ItemSwapDelegate OnItemSwap;
    public event ItemTransferConditionDelegate CanPutItemIntoSlot;
    public event ItemTransferConditionDelegate CanTakeItemFromSlot;
    #endregion

    public ImprovedItemSlot(int slotContext, float scale, float maxSize = 32f) {
        Context = slotContext;
        Scale = scale;
        MaxItemDimensions = maxSize;
    }

    /// <summary>Adds the UI Element as an icon to display when no item is in the slot.</summary>
    public void SetIcon(UIElement icon) {
        if (_icon != null) {
            RemoveChild(_icon);
        }

        _icon = icon;

        UpdateIcon();
    }

    public override void Update(GameTime gameTime) {
        _item ??= new Item();
        if (_item.type == ItemID.DD2EnergyCrystal && !DD2Event.Ongoing) {
            _item.TurnToAir();
        }

        if (CanHover && IsMouseHovering) {
            Main.LocalPlayer.mouseInterface = true;
        }

        base.Update(gameTime);
    }

    public override void LeftClick(UIMouseEvent evt) {
        _item ??= new();

        HandleLeftClickActions();

        base.LeftClick(evt);

        UpdateIcon();
    }

    private void HandleLeftClickActions() {
        Item mouseItem = Main.mouseItem ??= new();
        if (mouseItem.IsAir) {
            if (!_item.IsAir) {
                if (CanTakeItemFromSlot?.Invoke(_item) == true) {
                    SwapItems();
                }
                return;
            }
        }
        else {
            if (_item.IsAir) {
                if (CanPutItemIntoSlot?.Invoke(mouseItem) == true) {
                    SwapItems();
                }
                return;
            }

            ItemLoader.TryStackItems(_item, Main.mouseItem, out int numTransferred);
            if (numTransferred > 0) {
                OnItemSwap?.Invoke(Main.mouseItem, _item);
            }
        }

        void SwapItems() {
            Main.mouseItem = _item?.Clone();
            _item = mouseItem?.Clone();

            OnItemSwap?.Invoke(Main.mouseItem, _item);
        }
    }

    public override void RightClick(UIMouseEvent evt) {
        base.RightClick(evt);
        UpdateIcon();
    }

    public void UpdateIcon() {
        if (_icon != null) {
            if (_item != null && !_item.IsAir) {
                RemoveChild(_icon);
            }
            else {
                Append(_icon);
            }
        }
    }

    protected override void DrawSelf(SpriteBatch spriteBatch) {
        _item ??= new Item();
        CalculatedStyle c = GetDimensions();

        float oldScale = Main.inventoryScale;
        Main.inventoryScale = Scale;

        ItemSlotDrawHelper.DrawFullItem(_item, Context, 0, spriteBatch, c.Center() - new Vector2(26f * Main.inventoryScale), c.Center(), Main.inventoryScale, MaxItemDimensions, Color.White, Color.White);

        Main.inventoryScale = oldScale;

        if (IsMouseHovering) {
            if (WhileHoveringItem != null) {
                WhileHoveringItem.Invoke(_item);
            }
            else if (HasItem && ShowItemTooltip && CanHover) {
                ExtendUI.HoverItem(_item, Context);
            }
        }
    }
}
