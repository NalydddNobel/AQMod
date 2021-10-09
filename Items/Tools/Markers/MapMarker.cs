using AQMod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools.Markers
{
    public abstract class MapMarker : ModItem
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