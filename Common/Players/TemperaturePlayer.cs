using Aequus.Common.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Aequus.Common.Players
{
    public sealed class TemperaturePlayer : ModPlayer
    {
        /// <summary>
        /// The player's current temperature. Ticks down by 1 every frame
        /// </summary>
        public int currentTemperature;
        /// <summary>
        /// Clamps <see cref="currentTemperature"/> using this as the maximum
        /// </summary>
        public int maxTemperature;
        /// <summary>
        /// Clamps <see cref="currentTemperature"/> using this as the minimum
        /// </summary>
        public int minTemperature;
        /// <summary>
        /// Whenever the <see cref="currentTemperature"/> decrements, it uses this value
        /// </summary>
        public int temperatureRegen;

        /// <summary>
        /// Helper which converts <see cref="currentTemperature"/> to a <see cref="TemperatureType"/> enum
        /// </summary>
        public TemperatureType CurrentTemperature => TemperatureTypeConverter.FromTimer(currentTemperature);
        /// <summary>
        /// Helper which converts <see cref="maxTemperature"/> to a <see cref="TemperatureType"/> enum
        /// </summary>
        public TemperatureType MaxTemperature => TemperatureTypeConverter.FromTimer(minTemperature);
        /// <summary>
        /// Helper which converts <see cref="minTemperature"/> to a <see cref="TemperatureType"/> enum
        /// </summary>
        public TemperatureType MinTemperature => TemperatureTypeConverter.FromTimer(maxTemperature);

        public override void PostUpdateMiscEffects()
        {
            currentTemperature = (int)MathHelper.Clamp(currentTemperature, minTemperature, maxTemperature);
        }
    }
}