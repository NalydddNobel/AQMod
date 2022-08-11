using Microsoft.Xna.Framework;

namespace Aequus.UI.EventProgressBars
{
    public abstract class LegacyEventProgressBar : ILegacyEventProgressBar
    {
        public virtual string Icon { get; set; }
        public virtual string EventKey { get; set; }

        public Color backgroundColor;
        Color ILegacyEventProgressBar.BackgroundColor => backgroundColor;

        public abstract bool IsActive();

        public abstract float GetEventProgress();
    }
}