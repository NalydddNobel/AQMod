using Terraria;

namespace AQMod.Content.Fishing.FishingLocations
{
    internal class AnywhereFishing : IFishingLocation
    {
        private readonly int _chance;

        public AnywhereFishing(int chance = 1)
        {
            _chance = chance;
        }

        string IFishingLocation.TextKey => "FishingLocation.AnywhereFishing";

        bool IFishingLocation.CatchFish(Player player, Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer)
        {
            return player.ZoneBeach && liquidType == Tile.Liquid_Water && (_chance <= 1 || Main.rand.NextBool(_chance));
        }
    }
}