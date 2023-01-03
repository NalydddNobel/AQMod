using Aequus.Common;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    public class ThoriumModSupport : IAddRecipes
    {
        public static Mod ThoriumMod { get; private set; }

        void ILoadable.Load(Mod mod)
        {
            ThoriumMod = null;
            if (ModLoader.TryGetMod("ThoriumMod", out var thoriumMod))
            {
                ThoriumMod = thoriumMod;
            }
        }

        void IAddRecipes.AddRecipes(Aequus aequus)
        {
        }

        void ILoadable.Unload()
        {
            ThoriumMod = null;
        }
    }
}