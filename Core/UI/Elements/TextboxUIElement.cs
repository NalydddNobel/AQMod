using System;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.Core.UI.Elements;

public class TextboxUIElement : UIElement {
    public int MaxText = -1;
    public bool IsWritingText;
    public bool ShowCarrot;

    public Color Color;
    public int TextOffsetY;

    public string _innerText;
    public string DisplayText;

    public TextboxUIElement(Color color = default(Color), int maxText = -1, int textOffsetY = 5) {
        Color = color;
        MaxText = maxText;
        TextOffsetY = textOffsetY;
        _innerText = "";
        IsWritingText = false;
    }

    public override void Update(GameTime gameTime) {
        if (IsMouseHovering) {
            Main.LocalPlayer.mouseInterface = true;
        }
        if (Main.mouseLeft) {
            IsWritingText = IsMouseHovering;
        }
        if (Main.mouseRight || PlayerInput.Triggers.Current.Inventory) {
            IsWritingText = false;
        }

        if (IsWritingText) {
            PlayerInput.WritingText = true;
            Main.CurrentInputTextTakerOverride = this;
            _innerText = Main.GetInputText(_innerText);
            if (MaxText > 0 && _innerText.Length > MaxText) {
                _innerText = _innerText[..MaxText];
            }
        }

        base.Update(gameTime);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch) {
        if (Color == default(Color)) {
            Color = Color.White;
        }

        Color inventoryBackColor = Color.MultiplyRGBA(Main.inventoryBack);
        CalculatedStyle dimensions = GetDimensions();

        bool hovering = Main.mouseX > dimensions.X && Main.mouseX < dimensions.Y + dimensions.Width && Main.mouseY > dimensions.Y && Main.mouseY < dimensions.Y + dimensions.Height;

        if (IsWritingText) {
            Main.instance.HandleIME();
            Main.instance.DrawWindowsIMEPanel(new Vector2(98f, Main.screenHeight - 36f), 0f);
        }

        string drawnText = DisplayText ?? _innerText;
        DisplayText = null;

        if (ShowCarrot && IsWritingText && Main.GameUpdateCount % 50 < 25) {
            drawnText += "|";
        }

        Vector2 measurement = ChatManager.GetStringSize(FontAssets.MouseText.Value, drawnText, Vector2.One);
        Utils.DrawInvBG(Main.spriteBatch, dimensions.ToRectangle() with { Width = (int)Math.Max(dimensions.Width, measurement.X + 20f) }, inventoryBackColor);

        if (drawnText != "") {
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, drawnText, new Vector2(dimensions.X + 8f, dimensions.Y + dimensions.Height / 2f - measurement.Y / 2f + TextOffsetY), Color.White, 0f, Vector2.Zero, Vector2.One);
        }
    }
}