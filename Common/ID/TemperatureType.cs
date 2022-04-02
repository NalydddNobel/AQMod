using Microsoft.Xna.Framework;

namespace Aequus.Common.ID
{
    public enum TemperatureType
    {
        Frozen = -4,
        Freezing = -3,
        Colder = -2,
        Cold = -1,
        Neutral = 0,
        Hot = 1,
        Hotter = 2,
        Scorching = 3,
        Melting = 4,
    }

    public class TemperatureTypeConverter
    {
        public static TemperatureType FromTimer(int value)
        {
            return (TemperatureType)((int)MathHelper.Clamp(value, (int)TemperatureType.Frozen * 300, (int)TemperatureType.Melting * 300) / 300);
        }
    }
}