using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials
{
    public class PearlShardWhite : ModItem
    {
        public const int AmountPerPearl = 5;
        public virtual int PearlItem => ItemID.WhitePearl;

        public override void SetDefaults()
        {
            Item.CloneDefaults(PearlItem);
            Item.width = 16;
            Item.height = 16;
            Item.value /= AmountPerPearl;
            Item.rare = Math.Clamp(Item.rare - 1, 0, ItemRarityID.Count);
        }

        public override void AddRecipes()
        {
            Recipe.Create(PearlItem)
                .AddIngredient(Type, AmountPerPearl)
                .AddTile(TileID.GlassKiln)
                .Register();
        }
    }

    public class PearlShardBlack : PearlShardWhite
    {
        public override int PearlItem => ItemID.BlackPearl;
    }

    public class PearlShardPink : PearlShardWhite
    {
        public override int PearlItem => ItemID.PinkPearl;
    }
}