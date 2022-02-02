using AQMod.Buffs;
using AQMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Potions
{
    public class SaintsFlow : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 14;
            item.height = 30;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 10;
            item.useTime = 10;
            item.useTurn = true;
            item.UseSound = SoundID.Item3;
            item.maxStack = 999;
            item.consumable = true;
            item.rare = AQItem.Rarities.OmegaStariteRare - 1;
            item.value = Item.buyPrice(silver: 20);
            item.buffType = ModContent.BuffType<SpeedBoostFood>();
            item.buffTime = 28800;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<NeutronJuice>());
            r.AddIngredient(ModContent.ItemType<LightMatter>(), 2);
            r.AddTile(TileID.Bottles);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}