using Aequus.Items.HookEquips;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Aequus.Common
{
    public sealed class MiscSystem : ModSystem
    {
        public override void OnWorldLoad()
        {
            HookEquipsProjectile.modulesCombinedDamage?.Clear();
            HookEquipsProjectile.modulesCombinedDamage = new Dictionary<int, int>();
        }

        public override void PostUpdatePlayers()
        {
            if (Aequus.DayTimeManipulator.Caching)
            {
                Aequus.DayTimeManipulator.Clear();
            }
        }
    }
}