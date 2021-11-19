using Terraria;

namespace AQMod.Items
{
    public interface ICustomPickupText
    {
        /// <summary>
        /// Runs when the game wants to spawn pickup text for this item.
        /// </summary>
        /// <param name="newItem"></param>
        /// <param name="stack"></param>
        /// <param name="noStack"></param>
        /// <param name="longText"></param>
        /// <returns>Whether or not to override vanilla's item text spawning</returns>
        bool OnSpawnText(Item newItem, int stack, bool noStack, bool longText);
    }
}