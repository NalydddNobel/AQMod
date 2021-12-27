using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AQMod.Content.World.Events.ProgressBars
{
    public abstract class EventProgressBar
    {
        public abstract Texture2D IconTexture { get; }
        public abstract string EventName { get; }
        public abstract Color NameBGColor { get; }
        public abstract float EventProgress { get; }
        public abstract bool IsActive();
        public virtual string ModifyProgressText(string text)
        {
            return text;
        }
        public virtual bool PreDraw(Texture2D texture, string eventName, Color nameBGColor, float alpha)
        {
            return true;
        }
    }
}