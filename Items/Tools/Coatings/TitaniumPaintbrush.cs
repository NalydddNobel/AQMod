using Aequus.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.Coatings
{
    public class TitaniumPaintbrush : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.Green;
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer != player.whoAmI)
            {
                return true;
            }

            int x = Player.tileTargetX;
            int y = Player.tileTargetY;
            if (Main.tile[x, y].HasTile && player.IsInTileInteractionRange(x, y))
            {
                var t = Main.tile[x, y];
                if (!t.Get<AequusTileData>().Uncuttable)
                {
                    t.Get<AequusTileData>().Uncuttable = true;
                    for (int i = 0; i < 3; i++)
                    {
                        var d = Dust.NewDustDirect(new Vector2(x, y) * 16f, 16, 16, DustID.Titanium);
                        d.noGravity = true;
                        d.scale *= 0.9f;
                        d.fadeIn = d.scale + 0.1f;
                    }
                }
            }
            return true;
        }
    }
}