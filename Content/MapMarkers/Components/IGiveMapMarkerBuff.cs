using Terraria;

namespace AQMod.Content.MapMarkers.Components
{
    public interface IGiveMapMarkerBuff
    {
        void ToggleMapMarkerBuff(Player player, AQPlayer aQPlayer);
        int BuffType();
        bool BuffEnabled(Player player, AQPlayer aQPlayer);
    }
}