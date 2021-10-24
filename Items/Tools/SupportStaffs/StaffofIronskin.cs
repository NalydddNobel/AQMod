using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools.SupportStaffs
{
    public class StaffofIronskin : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.useAnimation = 17;
            item.useTime = 17;
            item.buffType = ModContent.BuffType<Buffs.SupportBuffs.IronskinSupport>();
            item.buffTime = 18000;
            item.UseSound = SoundID.Item2;
            item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.StoneBlock, 25);
            r.AddIngredient(ItemID.IronskinPotion, 4);
            r.AddIngredient(ItemID.Topaz, 3);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}