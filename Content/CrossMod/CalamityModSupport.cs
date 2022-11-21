using Aequus.Common;
using Aequus.Content.Necromancy;
using System;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    internal class CalamityModSupport : IAddRecipes, ILoadBefore
    {
        public static Mod CalamityMod { get; private set; }

        Type ILoadBefore.LoadBefore => typeof(NecromancyDatabase);

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