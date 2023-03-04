using Aequus.Common.Net;
using Terraria;

namespace Aequus.Tiles
{
    public class TileHooks
    {
        public interface IDontRunVanillaRandomUpdate
        {
        }

        public interface IOnPlaceTile
        {
            bool? OnPlaceTile(int i, int j, bool mute, bool forced, int plr, int style);
        }

        /// <summary>
        /// Provides a method to be called when recieving the <see cref="PacketUniqueTileInteraction"/> packet. Allowing for effects to be synced on the network easier.
        /// </summary>
        public interface IUniqueTileInteractions
        {
            void Interact(Player player, int i, int j);
        }
    }
}