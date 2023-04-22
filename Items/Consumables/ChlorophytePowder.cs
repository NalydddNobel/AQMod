using Aequus.Projectiles.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables
{
    public class ChlorophytePowder : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 0;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 6f;
            Item.shoot = ModContent.ProjectileType<ChlorophytePowderProj>();
            Item.width = 16;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.UseSound = SoundID.Item1;
            Item.useAnimation = 12;
            Item.useTime = 12;
            Item.noMelee = true;
            Item.value = Item.sellPrice(silver: 2);
            Item.rare = ItemRarityID.Lime;
        }

        public override void AddRecipes()
        {
            CreateRecipe(3)
                .AddIngredient<FertilePowder>(3)
                .AddIngredient(ItemID.ChlorophyteOre, 2)
                .AddTile(TileID.Bottles)
                .Register();
        }

        public override bool ConsumeItem(Player player)
        {
            return !Main.rand.NextBool(3);
        }
    }
}