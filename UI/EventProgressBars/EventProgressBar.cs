using Microsoft.Xna.Framework;

namespace Aequus.UI.EventProgressBars
{
    public abstract class EventProgressBar : IEventProgressBar
    {
        public virtual string Icon { get; set; }
        public virtual string EventKey { get; set; }

        public Color backgroundColor;
        Color IEventProgressBar.BackgroundColor => backgroundColor;

        public abstract bool IsActive();

        public abstract float GetEventProgress();
    }
}