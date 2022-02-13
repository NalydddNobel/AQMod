namespace AQMod.Common.CrossMod
{
    public sealed class PolaritiesModSupport
    {
        public static bool InFractalDimension()
        {
            return AQMod.polarities.IsActive ? AQUtils.GetValueOrDefault(AQMod.polarities.mod.Call("InFractalDimension"), false) : false;
        }
    }
}