using AQMod.Common;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.Fishing
{
    public abstract class FishingItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            FishLoader.AddFish(this);
        }

        public virtual bool RandomCatchFail()
        {
            return Main.rand.NextBool(5);
        }
        public virtual bool OtherItemsCheck(bool catchingCrate, bool catchingQuestFish)
        {
            return !catchingCrate && !catchingQuestFish;
        }
        /// <summary>
        /// By default, returns true if the liquid is water
        /// </summary>
        /// <param name="liquidType"></param>
        /// <returns></returns>
        public virtual bool ValidCatchingLocation(Player player, AQPlayer aQPlayer, Item fishingRod, Item bait, int power, int liquidType, int worldLayer, int questFish)
        {
            return liquidType == Tile.Liquid_Water;
        }
        public virtual bool Catch(Player player, AQPlayer aQPlayer, Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer, int questFish)
        {
            return true;
        }
    }
}