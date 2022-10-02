using Aequus.Items.Armor.Gravetender;
using Aequus.Items.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.Seraphim
{
    [AutoloadEquip(EquipType.Body)]
    public class SeraphimRobes : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.defense = 10;
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<SummonDamageClass>() += 0.2f;
            player.manaCost -= 0.25f;
            player.Aequus().ghostSlotsMax += 2;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GravetenderRobes>()
                .AddIngredient<Hexoplasm>(10)
                .AddTile(TileID.Loom)
                .TryRegisterBefore((ItemID.GravediggerShovel));
        }
    }
}