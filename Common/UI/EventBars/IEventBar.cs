using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Localization;

namespace Aequus.Common.UI.EventBars;

public interface IEventBar {
    Asset<Texture2D> Icon { get; set; }
    LocalizedText DisplayName { get; set; }
    Color BackgroundColor { get; }

    bool IsActive();
    float GetEventProgress();
    string GetProgressText(float progress);
    bool PreDraw(Texture2D texture, string eventName, Color nameBGColor, float alpha);
}