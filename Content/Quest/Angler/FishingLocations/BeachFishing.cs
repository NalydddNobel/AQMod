using Terraria;

namespace AQMod.Content.Quest.Angler.FishingLocations
{
    internal class BeachFishing : IFishingLocation
    {
        private int _chance;

        public BeachFishing(int chance = 1)
        {
            _chance = chance;
        }

        string IFishingLocation.TextKey => "FishingLocation.BeachFishing";

        bool IFishingLocation.CatchFish(Player player, Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer)
        {
            return player.ZoneBeach && (_chance <= 1 || Main.rand.NextBool(_chance));
        }
    }
}