using Aequus.Common.Utilities;
using System;
using Terraria.ModLoader;

namespace Aequus.Common
{
    internal interface IAddRecipes : ILoadable
    {
        void AddRecipes(Aequus aequus);

        public static bool CheckAutoload(Aequus aequus, Type type)
        {
            if (AutoloadUtilities.TryGetInstanceOf<IAddRecipes>(type, out var addRecipes))
            {
                addRecipes.AddRecipes(aequus);
                return true;
            }
            return false;
        }
    }
}