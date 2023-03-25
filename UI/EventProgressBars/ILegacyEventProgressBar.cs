using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Aequus.UI.EventProgressBars
{
    public interface ILegacyEventProgressBar
    {
        string Icon { get; set; }
        string EventKey { get; set; }
        Color BackgroundColor { get; }

        bool IsActive();
        float GetEventProgress();
        string GetProgressText(float progress);
        bool PreDraw(Texture2D texture, string eventName, Color nameBGColor, float alpha);
    }
}