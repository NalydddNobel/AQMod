using Terraria.Localization;

namespace AQMod.Localization
{
    public class TextCallback
    {
        protected byte callbackDelay = 10;

        public virtual void UpdateCallback()
        {
            if (callbackDelay > 0)
            {
                callbackDelay--;
            }
        }

        public virtual LocalizedText OnMissingText(string key, LocalizedText baseText)
        {
            if (callbackDelay == 0)
            {
                AQMod.GetInstance().Logger.Error("Missing text: " + key);
            }
            else
            {
                callbackDelay = 30;
            }
            return baseText;
        }
    }
}