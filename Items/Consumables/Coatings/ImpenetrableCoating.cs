using Aequus.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Coatings
{
    [LegacyName("TitaniumPaintbrush", "TitaniumScraper")]
    public class ImpenetrableCoating : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            AequusItem.ApplyCustomCoating.Add(Type, UseCoating);
            AequusItem.RemoveCustomCoating.Add(RemoveCoating);
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.RedPaint);
            Item.paint = 0;
            Item.value = Item.sellPrice(silver: 2);
        }

        public static bool UseCoating(int x, int y, Player player)
        {
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
                    return true;
                }
            }
            return false;
        }
        public static bool RemoveCoating(int x, int y, Player player)
        {
            var t = Main.tile[x, y];
            if (t.Get<AequusTileData>().Uncuttable)
            {
                t.Get<AequusTileData>().Uncuttable = false;
                for (int i = 0; i < 3; i++)
                {
                    var d = Dust.NewDustDirect(new Vector2(x, y) * 16f, 16, 16, DustID.Titanium);
                    d.noGravity = true;
                    d.scale *= 0.9f;
                    d.fadeIn = d.scale + 0.1f;
                }
                return true;
            }
            return false;
        }
    }
}