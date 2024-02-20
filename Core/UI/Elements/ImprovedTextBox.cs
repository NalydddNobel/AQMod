using Microsoft.Xna.Framework.Input;
using ReLogic.Graphics;
using System;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.Core.UI.Elements;

// TODO -- Add controller support?
// TODO -- Better selection support?
// TODO -- Move cursor to closest letter on Left Click?

/// <summary>Improved text box based off of <see cref="UITextBox"/>.</summary>
public class ImprovedTextBox : UITextPanel<string> {
    protected string _oldText;
    protected string _displayText;

    public string DisplayText => _displayText;

    public bool ShowInputTicker { get; private set; }
    public bool IsWritingText { get; private set; }

    protected int _cursor;

    protected double _cursorMoveDelay;
    protected int _controlCursorMoveOld;
    protected int _controlCursorMove;

    protected int _selectionAnchor;

    protected Vector2 _selectionVisualPosition;
    protected Vector2 _cursorVisualPosition;

    protected int _maxLength;

    protected double _carrotTimer;

    protected DynamicSpriteFont Font => IsLarge ? FontAssets.DeathText.Value : FontAssets.MouseText.Value;

    #region Events
    public delegate void OnTextChangedDelegate(string oldText, string newText);
    public delegate void KeybindDelegate(string inputText, string oldText, string newText);

    public event OnTextChangedDelegate OnTextChanged;
    public event KeybindDelegate OverridePressEnter;
    public event KeybindDelegate OverridePressEsc;
    #endregion

    public ImprovedTextBox(string text, float textScale = 1, bool large = false) : base(text, textScale, large) {
        Unhighlight();
    }

    public override void SetText(string text, float textScale, bool large) {
        text ??= "";

        if (text.Length > _maxLength) {
            text = text[.._maxLength];
        }

        base.SetText(text, textScale, large);

        _cursor = Math.Min(Text.Length, _cursor);
    }

    public void Write(string text) {
        SetText(Text.Insert(_cursor, text));
        _cursor += text.Length;
    }

    public void ToggleText(bool value) {
        if (IsWritingText != value) {
            ToggleText();
        }
    }
    public void ToggleText() {
        IsWritingText = !IsWritingText;
        Main.clrInput();
    }

    public void Unhighlight() {
        _selectionAnchor = -1;
    }
    public void RequestHighlightRecalc() {
        _selectionVisualPosition = Vector2.Zero;
    }

    public void BringCursorToEnd() {
        _cursor = _text.Length;
    }

    public void SetTextMaxLength(int maxLength) {
        _maxLength = maxLength;
    }

    public override void Update(GameTime gameTime) {
        _carrotTimer += gameTime.ElapsedGameTime.TotalSeconds;
        if (_controlCursorMove != 0 && _controlCursorMoveOld == _controlCursorMove) {
            _cursorMoveDelay -= gameTime.ElapsedGameTime.TotalMilliseconds * 0.03f;

            if (_cursorMoveDelay < 0f) {
                _cursorMoveDelay = 0f;
            }
        }
        else {
            _controlCursorMoveOld = _controlCursorMove;
            _cursorMoveDelay = 15f;
        }

        if (IsWritingText) {
            PlayerInput.WritingText = true;
            Main.CurrentInputTextTakerOverride = this;
            Main.LocalPlayer.mouseInterface = true;
            ShowInputTicker = true;
        }
        else {
            ShowInputTicker = false;
        }

        if (IsMouseHovering) {
            Main.LocalPlayer.mouseInterface = true;
        }

        if (!Main.instance.IsActive || (Main.mouseLeft && Main.mouseLeftRelease && !IsMouseHovering)) {
            ToggleText(false);
        }

        base.Update(gameTime);

        if (_oldText != _text) {
            OnTextChanged?.Invoke(_oldText, _text);
            _oldText = _text;
        }
    }

    public override void LeftClick(UIMouseEvent evt) {
        if (!IsWritingText) {
            ToggleText(true);
        }
    }

    public override void Recalculate() {
        base.Recalculate();
        _selectionVisualPosition = Vector2.Zero;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch) {
        //if (HideSelf) {
        //    return;
        //}

        if (IsWritingText) {
            PlayerInput.WritingText = true;
            Main.instance.HandleIME();

            HandleWriting();
        }

        string oldText = _text;
        int displayCursorPosition = _cursor;

        if (_cursor == _text.Length) {
            ModifyDisplayText();
            displayCursorPosition = _displayText.Length;
        }
        else {
            _displayText = _text;
        }

        _text = _displayText;
        base.DrawSelf(spriteBatch);
        _text = oldText;

        CalculatedStyle innerDimensions = GetInnerDimensions();

        _cursorVisualPosition = GetTextCoordinates(displayCursorPosition, _displayText);
        if (_selectionAnchor != -1) {
            DrawHighlight(innerDimensions);
        }

        if (_carrotTimer % 0.66f < 0.33f && ShowInputTicker) {
            Vector2 pos = innerDimensions.Position();
            if (IsLarge) {
                pos.Y -= 8f * TextScale;
            }
            else {
                pos.Y -= 4f * TextScale;
            }

            pos.X += (innerDimensions.Width - TextSize.X) * TextHAlign + _cursorVisualPosition.X - (IsLarge ? 8f : 4f) * TextScale + 8f;
            if (IsLarge) {
                Utils.DrawBorderStringBig(spriteBatch, "|", pos, TextColor, TextScale);
            }
            else {
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, "|", pos, TextColor, 0f, Vector2.Zero, new Vector2(0.75f, 1.3f) * TextScale);
            }
        }

        if (IsWritingText) {
            CalculatedStyle dimensions = GetDimensions();
            Main.instance.DrawWindowsIMEPanel(new Vector2(dimensions.X + dimensions.Width / 2f, dimensions.Y + dimensions.Height), 0.5f);
        }
    }

    private void DrawHighlight(CalculatedStyle dimensions) {
        if (_selectionVisualPosition == Vector2.Zero) {
            _selectionVisualPosition = GetTextCoordinates(_selectionAnchor, _text);
        }

        Vector2 pos = dimensions.Position();
        pos.X += (dimensions.Width - TextSize.X) * TextHAlign - (IsLarge ? 8f : 4f) * TextScale + 10f;
        pos.Y += dimensions.Height / 2f;

        Color lineColor = IsWritingText ? Color.Blue with { A = 0 } : Color.Gray * 0.4f;
        DrawHelper.DrawLine(new Vector2(pos.X + _cursorVisualPosition.X, pos.Y), new Vector2(pos.X + _selectionVisualPosition.X, pos.Y), 20f * TextScale, lineColor);
        //DrawHelper.DrawLine(dimensions.Position(), dimensions.Center(), 8f, Color.Blue with { A = 0 });
    }

    private Vector2 GetTextCoordinates(int cursor, string text) {
        cursor = Math.Clamp(cursor, 0, text.Length);
        text = text[..cursor];
        return new Vector2(ChatManager.GetStringSize(Font, text, new Vector2(TextScale)).X - 6f, IsLarge ? 32f : 16f) * TextScale;
    }

    private void HandleWriting() {
        if (Main.keyState.IsKeyDown(Keys.Back) && _selectionAnchor != -1) {
            if (_selectionAnchor < _cursor) {
                _selectionAnchor++;
            }
            else if (_cursor < _text.Length - 1) {
                _cursor++;
            }

            if (_selectionAnchor >= _text.Length) {
                Unhighlight();
            }
            else {
                DeleteSelection();
            }
            _carrotTimer = 0f;
        }

        bool swap = false;
        if (_selectionAnchor != -1) {
            if (_cursor > _selectionAnchor) {
                Utils.Swap(ref _cursor, ref _selectionAnchor);
                swap = true;
            }
        }

        int oldCursor = _cursor;
        string oldText = _text;
        string oldInputText = _text[.._cursor];
        string inputText = Main.GetInputText(oldInputText);

        _text = inputText + _text[_cursor..];

        if (Main.inputTextEnter) {
            PressEnter(inputText, oldText, _text);
        }
        if (Main.inputTextEscape) {
            PressEsc(inputText, oldText, _text);
        }

        if (oldInputText.Length != inputText.Length) {
            _cursor = inputText.Length;
            if (_selectionAnchor != -1) {
                _selectionAnchor++;
            }
            DeleteSelection();
            _carrotTimer = 0f;
        }
        else if (swap) {
            Utils.Swap(ref _cursor, ref _selectionAnchor);
        }

        if (_text.Length > _maxLength) {
            _text = oldText;
            _cursor = oldCursor;
        }

        if (Main.keyState.IsKeyDown(Keys.Left)) {
            if (_cursorMoveDelay <= 0 || _controlCursorMove == 0) {
                StepCursor(-1);
            }

            _controlCursorMove = -1;
        }
        else if (Main.keyState.IsKeyDown(Keys.Right)) {
            if (_cursorMoveDelay <= 0 || _controlCursorMove == 0) {
                StepCursor(1);
            }

            _controlCursorMove = 1;
        }
        else {
            _controlCursorMove = 0;
        }
    }

    private void DeleteSelection() {
        if (_selectionAnchor == -1 || _selectionAnchor == _cursor) {
            Unhighlight();
            return;
        }

        int start = Math.Clamp(_selectionAnchor, 0, _text.Length);
        int end = Math.Clamp(_cursor, 0, _text.Length);
        if (start > end) {
            Utils.Swap(ref start, ref end);
        }

        _text = _text.Remove(start, end - start);
        _cursor = start;
        Unhighlight();
    }

    private void StepCursor(int step) {
        int oldPosition = _cursor;

        if (Main.keyState.IsKeyDown(Keys.LeftShift) || Main.keyState.IsKeyDown(Keys.RightShift)) {
            if (_selectionAnchor == -1) {
                _selectionAnchor = _cursor;
            }
        }
        else {
            if (_selectionAnchor != -1) {
                // Snap to edges of selection if no longer holding shift
                if (step < 0 && _selectionAnchor < _cursor) {
                    _cursor = _selectionAnchor + 1;
                }
                if (step > 0 && _selectionAnchor > _cursor) {
                    _cursor = _selectionAnchor - 1;
                }

                Unhighlight();
            }
        }
        if (Main.keyState.IsKeyDown(Keys.LeftAlt)) {

        }
        _cursor = Math.Clamp(_cursor + step, 0, _text.Length);

        if (oldPosition != _cursor) {
            _carrotTimer = 0f;
            RequestHighlightRecalc();
        }
    }

    public virtual void PressEnter(string inputText, string oldText, string newText) {
        if (OverridePressEnter != null) {
            OverridePressEnter?.Invoke(inputText, oldText, newText);
        }
        else {
            ToggleText(false);
        }
    }
    public virtual void PressEsc(string inputText, string oldText, string newText) {
        if (OverridePressEsc != null) {
            OverridePressEsc?.Invoke(inputText, oldText, newText);
        }
        else {
            ToggleText(false);
        }
    }

    public virtual void ModifyDisplayText() { }
}
