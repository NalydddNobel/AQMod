using Aequus.Common;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    public class ColoredDamageTypesSupport : IPostSetupContent
    {
        public static Mod ColoredDamageTypes { get; private set; }

        void ILoadable.Load(Mod mod)
        {
        }

        void IPostSetupContent.PostSetupContent(Aequus aequus)
        {
            if (ModLoader.TryGetMod("ColoredDamageTypes", out var coloredDamageTypes))
            {
                ColoredDamageTypes = coloredDamageTypes;
                coloredDamageTypes.Call("AddDamageType", NecromancyDamageClass.Instance,
                    NecromancyDamageClass.NecromancyDamageColor, NecromancyDamageClass.NecromancyDamageColor, NecromancyDamageClass.NecromancyDamageColor * 1.25f);
            }
        }

        void ILoadable.Unload()
        {
            ColoredDamageTypes = null;
        }
    }
}