using AQMod.Tiles.TileEntities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools.MapMarkers
{
    public abstract class MapMarkerItem : ModItem
    {
        public virtual void GlobeEffects(Player player, TEGlobe globe)
        {
            player.AddBuff(BuffID.Regeneration, 240);
        }

        public virtual void PreAddMarker(Player player, TEGlobe globe)
        {

        }
    }
}