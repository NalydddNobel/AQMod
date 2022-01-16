using AQMod.Common;
using System.Collections.Generic;
using Terraria;

namespace AQMod.Content.Fishing
{
    public sealed class FishLoader : IAutoloadType
    {
        internal static class WorldLayers
        {
            public const int Space = 0;
            public const int Overworld = 1;
            public const int UndergroundLayer = 2;
            public const int CavernLayer = 3;
            public const int HellLayer = 4;
        }

        private static List<FishingItem> _fish;

        public static FishingItem GetFish(int type)
        {
            return _fish.Find((f) => f.item.type == type);
        }

        internal static void AddFish(FishingItem fish)
        {
            if (_fish == null)
                _fish = new List<FishingItem>();
            _fish.Add(fish);
        }

        public static int PoolFish(Player player, AQPlayer aQPlayer, Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer, int questFish, int fishOnHook)
        {
            bool crate = false;
            if (AQItem.Sets.IsCrate(fishOnHook))
            {
                crate = true;
            }
            foreach (var fish in _fish)
            {
                //Main.NewText(fish.Name);
                if (fish.OtherItemsCheck(crate, fishOnHook == Main.anglerQuestItemNetIDs[Main.anglerQuest]))
                {
                    if (fish.RandomCatchFail())
                    {
                        if (fish.ValidCatchingLocation(player, aQPlayer, fishingRod, bait, power, liquidType, worldLayer, questFish))
                        {
                            if (fish.Catch(player, aQPlayer, fishingRod, bait, power, liquidType, poolSize, worldLayer, questFish))
                            {
                                return fish.item.type;
                            }
                        }
                    }
                }
            }
            return 0;
        }

        void IAutoloadType.OnLoad()
        {
            if (_fish == null)
                _fish = new List<FishingItem>();
        }

        void IAutoloadType.Unload()
        {
            _fish = null;
        }
    }
}