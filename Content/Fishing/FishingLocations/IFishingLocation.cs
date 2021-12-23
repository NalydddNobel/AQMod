using Terraria;

namespace AQMod.Content.Fishing.FishingLocations
{
    public interface IFishingLocation
    {
        string TextKey { get; }
        bool CatchFish(Player player, Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer);
    }
}