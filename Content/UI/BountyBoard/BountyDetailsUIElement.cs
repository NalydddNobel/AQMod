using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.Content.UI.BountyBoard;

public class BountyDetailsUIElement : UIElement {
    public BountyPostsUIElement.PostInfo postInfo;

    public override void OnInitialize() {
        Width.Set(-20f, 0.65f);
        Height.Set(0f, 0.9f);
        Left.Set(10f, 0.35f);
        VAlign = 0.5f;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch) {
        var dimensions = GetDimensions();
        Helper.DrawRectangle(dimensions.ToRectangle(), Color.Black * 0.5f);

        if (postInfo == null) {
            return;
        }

        var font = FontAssets.DeathText.Value;
        string name = postInfo.DisplayName;
        var textMeasurement = font.MeasureString(name);
        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, name, new Vector2(dimensions.X + dimensions.Width / 2f, dimensions.Y + textMeasurement.Y / 1.5f * 0.66f), Color.White, 0f, textMeasurement / 2f, Vector2.One * 0.66f);

        string description = postInfo.Description;
        var descriptionTextMeasurement = font.MeasureString(description);
        var descriptionTextSize = Vector2.One * 0.4f;
        if ((descriptionTextMeasurement * descriptionTextSize).X > dimensions.Width) {
            descriptionTextSize *= dimensions.Width / (descriptionTextMeasurement * descriptionTextSize).X;
        }
        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, description, new Vector2(dimensions.X + dimensions.Width / 2f, dimensions.Y + textMeasurement.Y / 1.5f * 0.66f + 30f), Color.White, 0f, descriptionTextMeasurement / 2f, descriptionTextSize);
    }
}