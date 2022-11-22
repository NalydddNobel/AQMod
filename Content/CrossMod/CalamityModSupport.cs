using Aequus.Common;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    internal class CalamityModSupport : IAddRecipes
    {
        public static Mod CalamityMod { get; private set; }

        void ILoadable.Load(Mod mod)
        {
        }

        void IAddRecipes.AddRecipes(Aequus aequus)
        {
            CalamityMod = null;
            if (ModLoader.TryGetMod("CalamityMod", out var calamityMod))
            {
                CalamityMod = calamityMod;
            }
        }

        void ILoadable.Unload()
        {
            CalamityMod = null;
        }
    }
}