using Aequus.Common;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    internal sealed class PolaritiesSupport : IPostSetupContent
    {
        public static ModData Polarities;

        void ILoadable.Load(Mod mod)
        {
        }

        void IPostSetupContent.PostSetupContent(Aequus aequus)
        {
            Polarities = new ModData("Polarities");
        }

        void ILoadable.Unload()
        {
        }
    }
}