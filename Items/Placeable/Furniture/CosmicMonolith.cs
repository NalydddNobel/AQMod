using Aequus.Graphics;
using Aequus.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Furniture
{
    public class CosmicMonolith : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.BloodMoonMonolith);
            Item.accessory = true;
            Item.vanity = true;
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            Item.createTile = ModContent.TileType<CosmicMonolithTile>();
            Item.placeStyle = 0;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CosmicMonolithScene.Active = 10;
        }

        public override void UpdateVanity(Player player)
        {
            CosmicMonolithScene.Active = 10;
        }
    }
}