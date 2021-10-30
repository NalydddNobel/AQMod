using AQMod.Tiles.TileEntities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools.MapMarkers
{
    public class DungeonMap : MapMarkerItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.consumable = true;
            item.rare = ItemRarityID.Green;
            item.value = Item.sellPrice(gold: 2);
        }

        public override void PreAddMarker(Player player, TEGlobe globe)
        {
            var rectangle = new Rectangle(globe.Position.X * 16, globe.Position.Y * 16, 32, 32);
            for (int i = 0; i < 12; i++)
            {
                int d = Dust.NewDust(new Vector2(rectangle.X, rectangle.Y), rectangle.Width, rectangle.Height, 234);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity.X *= 0.2f;
                Main.dust[d].velocity.Y = -Main.rand.NextFloat(4f, 6f);
            }
        }

        public override void GlobeEffects(Player player, TEGlobe globe)
        {
            player.AddBuff(ModContent.BuffType<Buffs.MapMarkers.DungeonMarker>(), 20);
        }
    }
}