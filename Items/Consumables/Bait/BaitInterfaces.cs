using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Aequus.Items.Consumables.Bait
{
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
        void ModifyFishItem(Player player, Item fish);
    }
    internal interface IModifyFishAttempt
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bobber"></param>
        /// <param name="fisher"></param>
        /// <returns>Return false to prevent vanilla rolling fish</returns>
        bool OnItemRoll(Projectile bobber, ref FishingAttempt fisher);
    }
}