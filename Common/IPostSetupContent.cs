using Aequus.Common.Utilities;
using System;
using Terraria.ModLoader;

namespace Aequus.Common
{
    internal interface IPostSetupContent : ILoadable
    {
        void PostSetupContent(Aequus aequus);

        public static bool CheckAutoload(Aequus aequus, Type type)
        {
            if (AutoloadUtilities.HasInstanceOf<IPostSetupContent>(type, out var setupContent))
            {
                setupContent.PostSetupContent(aequus);
                return true;
            }
            return false;
        }
    }
}