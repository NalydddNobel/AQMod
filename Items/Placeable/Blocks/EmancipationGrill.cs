using Aequus.Content.Town.PhysicistNPC.Analysis;
using Aequus.Tiles.Blocks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Blocks
{
    public class EmancipationGrill : ModItem, ItemHooks.ICustomCanPlace
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            AnalysisSystem.IgnoreItem.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<EmancipationGrillTile>());
            Item.value = Item.buyPrice(silver: 2, copper: 50);
            Item.maxStack = 9999;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 100);
        }

        public bool? CheckCanPlace(Player player, int i, int j)
        {
            return ItemHooks.ICustomCanPlace.BubbleTilePlacement(i, j) ? true : null;
        }
    }
}