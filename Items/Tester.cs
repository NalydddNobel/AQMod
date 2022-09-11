using Aequus.Common;
using Aequus.Content.CarpenterBounties;
using Aequus.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items
{
    internal class Tester : ModItem
    {
        public override string Texture => AequusHelpers.GetPath<Gamestar>();

        public override bool IsLoadingEnabled(Mod mod)
        {
            return true;
        }

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToHoldUpItem();
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.width = 20;
            Item.height = 20;
            return;

            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.CrabCrevice.CrabHydrosailia>());
            Item.consumable = false;
            Item.maxStack = 1;
        }

        public override bool? UseItem(Player player)
        {
            int x = AequusHelpers.tileX;
            int y = AequusHelpers.tileY;

            AequusWorld.GenCrabCrevice.GrowWalls(x, y);
            return true;

            AequusWorld.GenCrabCrevice.Grow();
            AequusWorld.GenCrabCrevice.PlaceChests();
            return true;

            AequusWorld.GenCrabCrevice.Generate(null);
            return true;
            AequusWorld.RandomUpdateTile(x, y);
            return true;

            WorldGen.PlaceTile(AequusHelpers.tileX, AequusHelpers.tileY, Item.createTile, plr: player.whoAmI);
            Main.tile[AequusHelpers.tileX, AequusHelpers.tileY].TileFrameX = (short)(28 * Main.rand.Next(4));
            return true;

            player.GetModPlayer<CarpenterBountyPlayer>().CompletedBounties.Clear();
            var rect = Utils.CenteredRectangle(player.Center.ToTileCoordinates().ToVector2(), new Vector2(40f, 40f)).Fluffize(10);
            AequusHelpers.dustDebug(rect.WorldRectangle());
            Main.NewText(CarpenterSystem.BountiesByID[0].CheckConditions(new CarpenterBounty.ConditionInfo(new TileMapCache(rect)), out string reply));
            Main.NewText(reply);
            return true;
        }
    }
}