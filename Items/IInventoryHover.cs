using Terraria;

namespace AQMod.Items
{
    public interface IInventoryHover
    {
        /// <summary>
        /// Return true to prevent override actions like favoriting and trashing.
        /// </summary>
        /// <param name="inv"></param>
        /// <param name="context"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        bool CursorHover(Item[] inv, int context, int slot);
    }
}