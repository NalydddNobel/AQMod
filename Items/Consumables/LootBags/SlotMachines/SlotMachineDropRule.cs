using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.LootBags.SlotMachines
{
    public class SlotMachineDropRule : CommonDrop
    {
        public SlotMachineItemBase roulette;
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
        public SlotMachineDropRule(SlotMachineItemBase roulette, int itemID, int rouletteChoice = 0, int dropChance = 1, int amountDroppedMinimum = 1, int amountDroppedMaximum = 1, int chanceNumerator = 1) : base(itemID, dropChance, amountDroppedMinimum, amountDroppedMaximum, chanceNumerator)
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
        public SlotMachineDropRule(int rouletteItemID, int itemID, int rouletteChoice = 0, int dropChance = 1, int amountDroppedMinimum = 1, int amountDroppedMaximum = 1, int chanceNumerator = 1)
            : this(ItemLoader.GetItem(rouletteItemID) as SlotMachineItemBase, itemID, rouletteChoice, dropChance, amountDroppedMinimum, amountDroppedMaximum, chanceNumerator)
        {
        }

        public override bool CanDrop(DropAttemptInfo info)
        {
            return true;
        }

        public override ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
        {
            ItemDropAttemptResult result = default(ItemDropAttemptResult);
            if (roulette?.GetItem() == rouletteChoice || (chanceDenominator > 1 && Main.rand.Next(chanceDenominator) < chanceNumerator))
            {
                result.State = ItemDropAttemptResultState.Success;
                int stack = info.rng.Next(amountDroppedMinimum, amountDroppedMaximum + 1);
                for (int i = 0; i < Main.InventoryItemSlotsCount; i++)
                {
                    var item = info.player.inventory[i];
                    if (item.IsAir)
                    {
                        item.SetDefaults(itemId);
                        item.stack = stack;
                        item.newAndShiny = true;
                        item.Aequus()._inventoryMoveAnchor = SlotMachineItemBase.ItemSpawnLocation;
                        SlotMachineItemBase.ItemSpawnLocation = Vector2.Zero;
                        if (SlotMachineSystem.TimeSpeed < 24f)
                        {
                            SlotMachineSystem.TimeSpeed += 2f;
                        }
                        var oldPosition = item.position;
                        var oldWidth = item.width;
                        var oldHeight = item.height;
                        item.position = Main.LocalPlayer.position;
                        item.width = Main.LocalPlayer.width;
                        item.height = Main.LocalPlayer.height;

                        PopupText.NewText(PopupTextContext.RegularItemPickup, info.player.inventory[i], stack);

                        item.position = oldPosition;
                        item.width = oldWidth;
                        item.height = oldHeight;
                        return result;
                    }
                    else if (item.type == itemId && item.stack < item.maxStack)
                    {
                        int newStack = item.stack + stack;
                        if (newStack > item.maxStack)
                        {
                            stack -= item.maxStack - item.stack;
                            item.stack = item.maxStack;
                            continue;
                        }
                        item.stack += stack;
                        return result;
                    }
                }
                CommonCode.DropItem(info, itemId, stack);
                return result;
            }

            if (chanceDenominator == 1)
            {
                result.State = ItemDropAttemptResultState.DoesntFillConditions;
            }
            else
            {
                result.State = ItemDropAttemptResultState.FailedRandomRoll;
            }
            return result;
        }
    }
}