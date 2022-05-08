using System.Collections.Generic;
using Terraria.ModLoader;

namespace Aequus.Common
{
    public sealed class MiscSystem : ModSystem
    {
        public override void PostUpdatePlayers()
        {
            if (AequusHelpers.Main_dayTime.IsCaching)
            {
                AequusHelpers.Main_dayTime.EndCaching();
            }
        }
    }
}