using ReLogic.Content;
using Terraria.Localization;

namespace Aequus.Old.Common.EventBars;

public abstract class EventBar : IEventBar {
    public virtual Asset<Texture2D> Icon { get; set; }
    public virtual LocalizedText DisplayName { get; set; }

    public Color backgroundColor;
    Color IEventBar.BackgroundColor => backgroundColor;

    public abstract System.Boolean IsActive();

    public abstract System.Single GetEventProgress();

    public virtual System.String GetProgressText(System.Single progress) {
        return Language.GetTextValue("Game.WaveCleared", $"{(System.Int32)(progress * 100)}%");
    }

    public virtual System.Boolean PreDraw(Texture2D texture, System.String eventName, Color nameBGColor, System.Single alpha) {
        return true;
    }
}