using Terraria;
using Terraria.ModLoader;

namespace Aequus
{
    /// <summary>
    /// Modifies the stats of wings
    /// </summary>
    public struct PlayerWingModifiers
    {
        /// <summary>
        /// See: <see cref="Player.accRunSpeed"/>
        /// </summary>
        public StatModifier horizontalSpeed;
        /// <summary>
        /// See: <see cref="Player.runAcceleration"/>
        /// </summary>
        public StatModifier horizontalAcceleration;
        /// <summary>
        /// Wings default to 0.5
        /// </summary>
        public StatModifier verticalAscentWhenFalling;
        /// <summary>
        /// Wings default to 0.1
        /// </summary>
        public StatModifier verticalAscentWhenRising;
        /// <summary>
        /// Wings default to 0.5
        /// </summary>
        public StatModifier verticalMaxCanAscendMultiplier;
        /// <summary>
        /// Wings default to 1.5
        /// </summary>
        public StatModifier verticalMaxAscentMultiplier;
        /// <summary>
        /// Wings default to 0.1
        /// </summary>
        public StatModifier verticalConstantAscend;
        public StatModifier wingTime;

        public void ResetEffects()
        {
            horizontalSpeed = StatModifier.Default;
            horizontalAcceleration = StatModifier.Default;
            verticalAscentWhenFalling = StatModifier.Default;
            verticalAscentWhenRising = StatModifier.Default;
            verticalMaxCanAscendMultiplier = StatModifier.Default;
            verticalMaxAscentMultiplier = StatModifier.Default;
            verticalConstantAscend = StatModifier.Default;
            wingTime = StatModifier.Default;
        }
    }
}