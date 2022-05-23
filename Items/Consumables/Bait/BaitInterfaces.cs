using Aequus.Common.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Aequus.Items.Consumables.Bait
{
    internal interface IModifyFishingPower
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="fishing"></param>
        /// <param name="fishingRod"></param>
        /// <param name="fishingLevel"></param>
        void ModifyFishingPower(Player player, PlayerFishing fishing, Item fishingRod, ref float fishingLevel);
        //public virtual void OnEnterWater(Player player, PlayerFishing fishing, Projectile bobber, Tile tile)
        //{
        //}
        //public virtual void OnCatchEffect(Player player, PlayerFishing fishing, Projectile bobber, Tile tile)
        //{
        //}
    }
    internal interface IModifyCatchFish
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attempt"></param>
        /// <param name="itemDrop"></param>
        /// <param name="npcSpawn"></param>
        /// <param name="sonar"></param>
        /// <param name="sonarPosition"></param>
        /// <returns>Return false to prevent Aequus from running fish checks.</returns>
        bool ModifyCatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition);
    }
    internal interface IModifyFishItem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fish"></param>
        void ModifyFishItem(Item fish);
    }
}