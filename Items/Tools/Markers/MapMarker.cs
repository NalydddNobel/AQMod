using AQMod.Common;
using AQMod.Content;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Items.Misc.Markers
{
    public abstract class MapMarker : ModItem
    {
        public abstract int GetID();

        public sealed override bool UseItem(Player player)
        {
            int id = GetID();
            var mapMarkerPlayer = player.GetModPlayer<MapMarkerPlayer>();
            if (mapMarkerPlayer.MarkersObtained[id])
            {
                return false;
            }
            else
            {
                mapMarkerPlayer.MarkersObtained[id] = true;
                mapMarkerPlayer.MarkersHidden[id] = false;
                return true;
            }
        }

        public abstract string Apply(Player player, AQPlayer aQPlayer, string mouseText, MapMarkerPlayer mapMarkerPlayer);
    }
}