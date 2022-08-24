using Aequus.Buffs.Minion;
using Aequus.Projectiles.Summon.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.Gravetender
{
    [AutoloadEquip(EquipType.Head)]
    public class GravetenderHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.defense = 2;
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<GravetenderWisp>();
            Item.buffType = ModContent.BuffType<GravetenderMinionBuff>();
            Item.value = Item.sellPrice(silver: 20);
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<GravetenderRobes>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = AequusText.GetText("ArmorSetBonus.Gravetender");
            player.Aequus().setGravetender = Item;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<SummonDamageClass>() += 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Cobweb, 50)
                .AddIngredient(ItemID.RottenChunk, 5)
                .AddTile(TileID.Loom)
                .AddCondition(Recipe.Condition.InGraveyardBiome)
                .Register((r) => r.SortBeforeFirstRecipesOf(ItemID.GravediggerShovel));
            CreateRecipe()
                .AddIngredient(ItemID.Cobweb, 50)
                .AddIngredient(ItemID.Vertebrae, 5)
                .AddTile(TileID.Loom)
                .AddCondition(Recipe.Condition.InGraveyardBiome)
                .Register((r) => r.SortBeforeFirstRecipesOf(ItemID.GravediggerShovel));
        }
    }
}