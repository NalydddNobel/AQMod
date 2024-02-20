using Aequus.Common.Renaming;
using System;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.UI;

namespace Aequus.Content.TownNPCs.SkyMerchant.UI;

public class RenameTextBox : UITextBox {
    public RenameTextBox(string text, float textScale = 1, bool large = false) : base(text, textScale, large) {
    }

    public bool IsWritingText { get; private set; }

    private string _oldText;

    public event Action<string, string> OnTextChanged;

    public override void Update(GameTime gameTime) {
        if (IsWritingText) {
            PlayerInput.WritingText = true;
            Main.CurrentInputTextTakerOverride = this;
            ShowInputTicker = true;
        }
        else {
            ShowInputTicker = false;
        }

        base.Update(gameTime);
        if (_oldText != _text) {
            OnTextChanged?.Invoke(_oldText, _text);
            _oldText = _text;
        }
    }

    protected override void DrawSelf(SpriteBatch spriteBatch) {
        if (IsWritingText) {
            PlayerInput.WritingText = true;
            Main.instance.HandleIME();

            string inputText = Main.GetInputText(_text);
            if (Main.inputTextEnter || Main.inputTextEscape) {
                ToggleText();
            }
            SetText(inputText);
        }

        string oldText = _text;

        _text = RenamingSystem.GetColoredDecodedText(_text, pulse: true);
        base.DrawSelf(spriteBatch);
        _text = oldText;

        if (IsWritingText) {
            CalculatedStyle dimensions = GetDimensions();
            Main.instance.DrawWindowsIMEPanel(new Vector2(dimensions.X + dimensions.Width / 2f, dimensions.Y + dimensions.Height), 0.5f);
        }
    }

    public void ToggleText() {
        IsWritingText = !IsWritingText;
        Main.clrInput();
    }
}
