using ReLogic.Content;
using System;
using Terraria.GameContent.Events;
using Terraria.UI;

namespace AequusRemake.Core.GUI.Elements;

public class AequusRemakeItemSlotElement : UIElement {
    public readonly Texture2D InventoryBackTexture;
    public readonly Asset<Texture2D> IconTexture;
    public readonly Rectangle IconFrame;
    public readonly int Context;

    public event Action<Item, int> WhileHoveringItem;
    public Func<Item, bool> CanPutItemIntoSlot { get; set; }
    public Func<Item, bool> CanTakeItemFromSlot { get; set; }

    public bool CanHover { get; set; }

    public bool ShowTooltip { get; set; }

    protected Item _item;
    public Item Item { get => _item; set => _item = value; }
    public bool HasItem => _item != null && !_item.IsAir;

    public event Action<Item, Item> OnItemSwap;

    public AequusRemakeItemSlotElement(int slotContext, Texture2D back, Asset<Texture2D> icon = null, Rectangle? frame = null) {
        _item = new Item();
        InventoryBackTexture = back;
        IconTexture = icon;
        if (IconTexture != null) {
            IconFrame = frame ?? IconTexture.Frame();
        }
        Context = slotContext;
    }

    public override void Update(GameTime gameTime) {
        _item ??= new Item();
        if (_item.type == ItemID.DD2EnergyCrystal && !DD2Event.Ongoing) {
            _item.TurnToAir();
        }

        if (ShowTooltip && CanHover && IsMouseHovering) {
            Main.LocalPlayer.mouseInterface = true;
        }

        base.Update(gameTime);
    }

    public override void LeftClick(UIMouseEvent evt) {
        Item mouseItem = Main.mouseItem;
        if ((mouseItem != null && !mouseItem.IsAir && CanPutItemIntoSlot?.Invoke(mouseItem) == true) || (_item != null && !_item.IsAir && (mouseItem == null || mouseItem.IsAir) && CanTakeItemFromSlot?.Invoke(_item) == true)) {
            Main.mouseItem = _item?.Clone();
            _item = mouseItem?.Clone();
            OnItemSwap?.Invoke(Main.mouseItem, _item);
        }
        base.LeftClick(evt);
    }

    public override void Draw(SpriteBatch spriteBatch) {
        var c = GetDimensions();
        float dimensionMin = Math.Min(c.Width, c.Height);
        float newScale = dimensionMin / 52f;
        float oldScale = Main.inventoryScale;
        Main.inventoryScale = newScale;
        spriteBatch.Draw(InventoryBackTexture, new Vector2(c.X, c.Y), null, Color.White, 0f, Vector2.Zero, new Vector2(c.Width / 52f, c.Height / 52f), SpriteEffects.None, 0f);
        if (!HasItem && IconTexture != null) {
            spriteBatch.Draw(IconTexture.Value, new Vector2(c.X, c.Y) + InventoryBackTexture.Size() / 2f * newScale, IconFrame, Color.White * 0.35f, 0f, IconFrame.Size() / 2f, 1f, SpriteEffects.None, 0f);
        }

        Vector2 drawLoc = new Vector2(c.X, c.Y);
        if (c.Width > c.Height) {
            drawLoc.X += c.Width / 2 - c.Height / 2f;
        }
        else if (c.Height > c.Width) {
            drawLoc.Y += c.Height / 2 - c.Width / 2f;
        }

        ItemSlotDrawHelper.DrawFullItem(_item, Context, 0, spriteBatch, new Vector2(c.X, c.Y), drawLoc + InventoryBackTexture.Size() / 2f * Main.inventoryScale, Main.inventoryScale, 32f, Color.White, Color.White);
        base.Draw(spriteBatch);
        Main.inventoryScale = oldScale;

        if (IsMouseHovering) {
            if (WhileHoveringItem != null) {
                WhileHoveringItem.Invoke(_item, Context);
            }
            else if (HasItem && ShowTooltip && CanHover) {
                ExtendUI.HoverItem(_item, Context);
            }
        }
    }
}