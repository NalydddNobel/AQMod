using Terraria;
using Terraria.Utilities;

namespace AQMod.Effects.Screen
{
    public abstract class ScreenFX
    {
        protected UnifiedRandom _random;

        public void Setup()
        {
            _random = new UnifiedRandom("SPLIT".GetHashCode() + (int)Main.GameUpdateCount);
            OnCreate();
        }

        protected virtual void OnCreate()
        {
        }

        public abstract bool UpdateBiomeVisuals { get; }

        public virtual void AdoptChannel(ScreenFX effect)
        {
        }

        /// <returns> Whether this instance should NOT be removed from the list.</returns>
        public abstract bool Update();
        public abstract void Apply();
    }
}