using Aequus.Common;

namespace Aequus.Content.CrossMod
{
    internal sealed class PolaritiesSupport : LoadableType
    {
        public static ModData Polarities;

        public override void SetStaticDefaults()
        {
            Polarities = new ModData("Polarities");
        }
    }
}