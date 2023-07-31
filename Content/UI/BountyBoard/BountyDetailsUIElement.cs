using Aequus.Common.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.Content.UI.BountyBoard;

public class BountyDetailsUIElement : UIElement {
    private BountyPostsUIElement.PostInfo _postInfo;
    public BountyPostsUIElement.PostInfo PostInfo {
        get {
            return _postInfo;
        }
        set {
            RemoveAllChildren();
            _postInfo = value;
            if (value == null) {
                return;
            }

            PostDescription = PostInfo.challenge.GetStepDescriptions();
            for (int i = 0; i < PostDescription.Length; i++) {
                PostDescription[i] = "• " + PostDescription[i];
            }
            SetupPanels();
        }
    }
    private string[] PostDescription;

    public override void OnInitialize() {
        Width.Set(-20f, 0.65f);
        Height.Set(0f, 0.9f);
        Left.Set(10f, 0.35f);
        VAlign = 0.5f;
    }

    private void SetupPanels() {
        var uiPanel = new UIPanel {
            BackgroundColor = new Color(68, 99, 164) * 0.825f
        };
        uiPanel.Width.Set(120, 0.01f);
        uiPanel.Height.Set(60, 0.01f);
        uiPanel.HAlign = 0.5f;
        uiPanel.Top.Set(-uiPanel.Height.Pixels - 2f, 1f - uiPanel.Height.Percent);
        Append(uiPanel);

        int itemCount = 0;
        float slotSize = 46f;
        foreach (var item in _postInfo.challenge.GetRewards()) {
            if (item == null || item.IsAir) {
                continue;
            }
            var uiSlot = new AequusItemSlotElement(TextureAssets.InventoryBack.Value) {
                item = item,
                showItemTooltipOnHover = true,
                canHover = true
            };
            uiSlot.Left.Set(-slotSize - itemCount * slotSize - 2f, 1f);
            uiSlot.Top.Set(-slotSize - 2f, 1f);
            uiSlot.Width.Set(slotSize, 0f);
            uiSlot.Height.Set(slotSize, 0f);
            Append(uiSlot);
            itemCount++;
        }
    }

    protected override void DrawSelf(SpriteBatch spriteBatch) {
        var dimensions = GetDimensions();
        Helper.DrawRectangle(dimensions.ToRectangle(), Color.Black * 0.5f);

        if (PostInfo == null) {
            return;
        }

        var font = FontAssets.DeathText.Value;
        string name = PostInfo.DisplayName;
        var textMeasurement = font.MeasureString(name);
        var textPosition = new Vector2(dimensions.X + dimensions.Width / 2f, dimensions.Y + textMeasurement.Y / 1.5f * 0.66f);
        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, name, textPosition, Color.Yellow, 0f, textMeasurement / 2f, Vector2.One * 0.66f);

        string description = PostInfo.Description;
        var descriptionTextMeasurement = font.MeasureString(description);
        var descriptionTextSize = Vector2.One * 0.4f;
        if ((descriptionTextMeasurement * descriptionTextSize).X > dimensions.Width) {
            descriptionTextSize *= dimensions.Width / (descriptionTextMeasurement * descriptionTextSize).X;
        }
        textPosition.Y += textMeasurement.Y / 2f + 4f;
        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, description, textPosition, Color.White, 0f, descriptionTextMeasurement / 2f, descriptionTextSize);

        if (PostDescription == null) {
            return;
        }

        var stepsText = PostDescription;
        var stepsTextPosition = new Vector2(dimensions.X + 8f, textPosition.Y + 50f);
        font = FontAssets.MouseText.Value;
        for (int i = 0; i < stepsText.Length; i++) {
            string text = stepsText[i];
            if (string.IsNullOrEmpty(text)) {
                continue;
            }

            var stepTextSize = ChatManager.GetStringSize(font, text, Vector2.One);
            var stepTextScale = Vector2.One;
            if (stepTextSize.X > (dimensions.Width - 20f)) {
                stepTextScale *= (dimensions.Width - 20f) / stepTextSize.X;
            }
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, text, stepsTextPosition, Color.LightGray * 1.25f, 0f, new Vector2(0f, stepTextSize.Y / 2f), stepTextScale);
            stepsTextPosition.Y += 30f;
        }
    }
}