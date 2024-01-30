using ReLogic.Content;
using Terraria.Localization;

namespace Aequus.Old.Common.EventBars;

public interface IEventBar {
    Asset<Texture2D> Icon { get; set; }
    LocalizedText DisplayName { get; set; }
    Color BackgroundColor { get; }

    System.Boolean IsActive();
    System.Single GetEventProgress();
    System.String GetProgressText(System.Single progress);
    System.Boolean PreDraw(Texture2D texture, System.String eventName, Color nameBGColor, System.Single alpha);
}