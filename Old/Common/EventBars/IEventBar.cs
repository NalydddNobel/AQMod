using ReLogic.Content;
using Terraria.Localization;

namespace Aequus.Old.Common.EventBars;

public interface IEventBar {
    Asset<Texture2D> Icon { get; set; }
    LocalizedText DisplayName { get; set; }
    Color BackgroundColor { get; }

    bool IsActive();
    float GetEventProgress();
    string GetProgressText(float progress);
    bool PreDraw(Texture2D texture, string eventName, Color nameBGColor, float alpha);
}