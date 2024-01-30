using ReLogic.Content;
using Terraria.Localization;

namespace Aequus.Old.Common.EventBars;

public abstract class EventBar : IEventBar {
    public virtual Asset<Texture2D> Icon { get; set; }
    public virtual LocalizedText DisplayName { get; set; }

    public Color backgroundColor;
    Color IEventBar.BackgroundColor => backgroundColor;

    public abstract bool IsActive();

    public abstract float GetEventProgress();

    public virtual string GetProgressText(float progress) {
        return Language.GetTextValue("Game.WaveCleared", $"{(int)(progress * 100)}%");
    }

    public virtual bool PreDraw(Texture2D texture, string eventName, Color nameBGColor, float alpha) {
        return true;
    }
}