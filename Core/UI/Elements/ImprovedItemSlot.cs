using Terraria.GameContent.Events;
using Terraria.UI;

namespace Aequus.Core.UI.Elements;

public class ImprovedItemSlot : UIElement {
    private UIElement _icon;

    public readonly float MaxSize;
    public readonly int Context;

    public bool CanHover { get; set; }
    public bool ShowItemTooltip { get; set; }

    protected Item _item;
    public Item Item { get => _item; set => _item = value; }
    public bool HasItem => _item != null && !_item.IsAir;

    #region Events
    public delegate void HoverItemDelegate(Item item);
    public delegate bool ItemSwapDelegate(Item mouseItem, Item slotItem);
    public delegate bool ItemTransferConditionDelegate(Item item);

    public event HoverItemDelegate WhileHoveringItem;
    public event ItemSwapDelegate OnItemSwap;
    public event ItemTransferConditionDelegate CanPutItemIntoSlot;
    public event ItemTransferConditionDelegate CanTakeItemFromSlot;
    #endregion

    public ImprovedItemSlot(int slotContext, float maxSize = 32f) {
        Context = slotContext;
        MaxSize = maxSize;
    }

    /// <summary>Adds the UI Element as an icon to display when no item is in the slot.</summary>
    public void SetIcon(UIElement icon) {
        _icon = icon;
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
        Item mouseItem = (Main.mouseItem ??= new());
        _item ??= new();

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

            ItemLoader.TryStackItems(_item, mouseItem, out _);
        }

        base.LeftClick(evt);

        void SwapItems() {
            Main.mouseItem = _item?.Clone();
            _item = mouseItem?.Clone();

            OnItemSwap?.Invoke(Main.mouseItem, _item);
        }
    }

    public override void RightClick(UIMouseEvent evt) {
        base.RightClick(evt);
    }
}
