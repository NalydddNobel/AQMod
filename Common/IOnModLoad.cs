using Aequus.Common.Utilities;
using System;
using Terraria.ModLoader;

namespace Aequus.Common
{
    internal interface IOnModLoad : ILoadable
    {
        void OnModLoad(Aequus aequus);

        public static bool CheckAutoload(Aequus aequus, Type type)
        {
            if (AutoloadUtilities.TryGetInstanceOf<IOnModLoad>(type, out var onModLoad))
            {
                onModLoad.OnModLoad(aequus);
                return true;
            }
            return false;
        }
    }
}