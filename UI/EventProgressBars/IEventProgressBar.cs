using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Aequus.UI.EventProgressBars
{
    public interface IEventProgressBar
    {
        string Icon { get; set; }
        string EventKey { get; set; }
        Color BackgroundColor { get; }

        bool IsActive();
        float GetEventProgress();

        string ModifyProgressText(string text)
        {
            return text;
        }
        bool PreDraw(Texture2D texture, string eventName, Color nameBGColor, float alpha)
        {
            return true;
        }
    }
}