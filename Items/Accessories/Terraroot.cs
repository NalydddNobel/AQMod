using AQMod.Items.Materials.Energies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories
{
    public class Terraroot : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.rare = ItemRarityID.LightRed;
            item.value = Item.sellPrice(gold: 4);
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AQPlayer>().chloroTransfer = true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.MusketBall, 999);
            r.AddIngredient(ModContent.ItemType<OrganicEnergy>(), 5);
            r.AddIngredient(ItemID.HellstoneBar, 20);
            r.AddIngredient(ItemID.ShadowScale, 10);
            r.AddIngredient(ItemID.JungleSpores, 8);
            r.AddIngredient(ItemID.Bone, 80);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
            r = new ModRecipe(mod);
            r.AddIngredient(ItemID.MusketBall, 999);
            r.AddIngredient(ModContent.ItemType<OrganicEnergy>(), 5);
            r.AddIngredient(ItemID.HellstoneBar, 20);
            r.AddIngredient(ItemID.TissueSample, 10);
            r.AddIngredient(ItemID.JungleSpores, 8);
            r.AddIngredient(ItemID.Bone, 80);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}