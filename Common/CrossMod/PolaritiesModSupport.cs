namespace AQMod.Common.CrossMod
{
    public sealed class PolaritiesModSupport
    {
        public static bool InFractalDimension()
        {
            return AQMod.polarities.IsActive ? GetValueOrDefault(AQMod.polarities.mod.Call("InFractalDimension"), false) : false;
        }

        public static T GetValueOrDefault<T>(object value, T defaultValue)
        {
            return value != null && (value is T wantedValue) ? wantedValue : default;
        }
    }
}