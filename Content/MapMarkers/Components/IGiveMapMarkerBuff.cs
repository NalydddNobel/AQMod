using Terraria;

namespace AQMod.Content.MapMarkers.Components
{
    public interface IGiveMapMarkerBuff
    {
        void ApplyMapMarker(Player player, AQPlayer aQPlayer); 
        void ToggleMapMarkerBuff(Player player, AQPlayer aQPlayer);
        int BuffType();
    }
}