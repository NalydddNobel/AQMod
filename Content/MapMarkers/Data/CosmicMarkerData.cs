using AQMod.Content.MapMarkers.Components;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.MapMarkers.Data
{
    public sealed class CosmicMarkerData : MapMarkerData, IGiveMapMarkerBuff
    {
        public void ApplyMapMarker(Player player, AQPlayer aQPlayer)
        {
            player.AddBuff(BuffType(), 16);
        }

        public int BuffType()
        {
            return ModContent.BuffType<Buffs.MapMarkers.CosmicMarker>();
        }

        public void ToggleMapMarkerBuff(Player player, AQPlayer aQPlayer)
        {
            aQPlayer.showCosmicMap = !aQPlayer.showCosmicMap;
        }
    }
}