using Aequus.Common;
using Aequus.Content.CarpenterBounties;
using Aequus.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items
{
    internal class Tester : ModItem
    {
        public override string Texture => AequusHelpers.GetPath<Gamestar>();

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
        }

        public override bool? UseItem(Player player)
        {
            Main.NewText(CarpenterSystem.BountiesByID[0].CheckConditions(new TileMapCache(Utils.CenteredRectangle(player.Center.ToTileCoordinates().ToVector2(), new Vector2(30f, 20f)).Fluffize(10)), out string dnc, null));
            return true;
        }
    }
}