using Aequus.Common;
using Aequus.Content.Necromancy;
using System;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    internal class PolaritiesSupport : IAddRecipes, ILoadBefore
    {
        public static Mod Polarities { get; private set; }

        Type ILoadBefore.LoadBefore => typeof(NecromancyDatabase);

        void ILoadable.Load(Mod mod)
        {
        }

        void IAddRecipes.AddRecipes(Aequus aequus)
        {
            Polarities = null;
            if (ModLoader.TryGetMod("Polarities", out var polarities))
            {
                Polarities = polarities;
            }
        }

        void ILoadable.Unload()
        {
            Polarities = null;
        }
    }
}