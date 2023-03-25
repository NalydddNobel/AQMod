using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

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

        public virtual string GetProgressText(float progress) {
            return Language.GetTextValue("Game.WaveCleared", $"{(int)(progress * 100)}%");
        }
        public virtual bool PreDraw(Texture2D texture, string eventName, Color nameBGColor, float alpha) {
            return true;
        }
    }
}