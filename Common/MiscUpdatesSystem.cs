using Terraria.ModLoader;

namespace Aequus.Common
{
    public sealed class MiscUpdatesSystem : ModSystem
    {
        public override void PostUpdatePlayers()
        {
            if (Aequus.DayTimeManipulator.Caching)
            {
                Aequus.DayTimeManipulator.Clear();
            }
        }
    }
}