using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools.MapMarkers
{
    public class CosmicTelescope : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.consumable = true;
            item.rare = ItemRarityID.Green;
            item.value = Item.sellPrice(gold: 2);
        }

        //public override void GlobeEffects(Player player, TEGlobe globe)
        //{
        //    player.AddBuff(ModContent.BuffType<Buffs.MapMarkers.CosmicMarker>(), 20);
        //}

        //public override void PreAddMarker(Player player, TEGlobe globe)
        //{
        //}
    }
}