using AQMod.Buffs;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Potions
{
    public class NoonPotion : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 26;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useTurn = true;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.rare = ItemRarityID.LightRed;
            item.value = AQItem.Prices.PotionValue;
            item.maxStack = 30;
            item.buffTime = 28800;
            item.buffType = ModContent.BuffType<NoonBuff>();
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.ShinePotion);
            r.AddIngredient(ItemID.NightOwlPotion);
            r.AddIngredient(ItemID.FallenStar);
            r.AddIngredient(ItemID.SoulofLight);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}