using System;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.LootBags.Roulettes
{
    public class RouletteDropRule : CommonDrop
    {
        public RouletteBase roulette;
        public int rouletteChoice;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roulette">An instance of the Roulette, for a better lookup.</param>
        /// <param name="itemID">The dropped item ID</param>
        /// <param name="rouletteChoice">The item which needs to be rolled for a 100% drop</param>
        /// <param name="dropChance">A drop chance of 100% means it will only drop when chosen by the scrolling item list (Since if it already drops 100% of the time, it might as well not be on the list.)</param>
        /// <param name="amountDroppedMinimum"></param>
        /// <param name="amountDroppedMaximum"></param>
        /// <param name="chanceNumerator"></param>
        public RouletteDropRule(RouletteBase roulette, int itemID, int rouletteChoice = 0, int dropChance = 1, int amountDroppedMinimum = 1, int amountDroppedMaximum = 1, int chanceNumerator = 1) : base(itemID, dropChance, amountDroppedMinimum, amountDroppedMaximum, chanceNumerator)
        {
            if (rouletteChoice <= 0)
                rouletteChoice = itemID;

            this.roulette = roulette;
            this.rouletteChoice = rouletteChoice;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rouletteItemID">The Item ID of the roulette</param>
        /// <param name="itemID">The dropped item ID</param>
        /// <param name="rouletteChoice">The item which needs to be rolled for a 100% drop</param>
        /// <param name="dropChance">A drop chance of 100% means it will only drop when chosen by the scrolling item list (Since if it already drops 100% of the time, it might as well not be on the list.)</param>
        /// <param name="amountDroppedMinimum"></param>
        /// <param name="amountDroppedMaximum"></param>
        /// <param name="chanceNumerator"></param>
        public RouletteDropRule(int rouletteItemID, int itemID, int rouletteChoice = 0, int dropChance = 1, int amountDroppedMinimum = 1, int amountDroppedMaximum = 1, int chanceNumerator = 1) 
            : this(ItemLoader.GetItem(rouletteItemID) as RouletteBase, itemID, rouletteChoice, dropChance, amountDroppedMinimum, amountDroppedMaximum, chanceNumerator)
        {
        }

        public override bool CanDrop(DropAttemptInfo info)
        {
            return true;
        }

        public override ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
        {
            ItemDropAttemptResult result;
            if (roulette?.GetItem() == rouletteChoice || (chanceDenominator > 1 && Main.rand.Next(chanceDenominator) < chanceNumerator))
            {
                CommonCode.DropItem(info, itemId, info.rng.Next(amountDroppedMinimum, amountDroppedMaximum + 1));
                result = default(ItemDropAttemptResult);
                result.State = ItemDropAttemptResultState.Success;
                return result;
            }

            if (chanceDenominator == 1)
            {
                result = default(ItemDropAttemptResult);
                result.State = ItemDropAttemptResultState.DoesntFillConditions;
            }
            else
            {
                result = default(ItemDropAttemptResult);
                result.State = ItemDropAttemptResultState.FailedRandomRoll;
            }
            return result;
        }
    }
}