using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories
{
    public class Quasicrystal : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.Glowmask();
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 24;
            item.defense = 2;
            item.accessory = true;
            item.rare = ItemRarityID.LightPurple;
            item.value = Item.buyPrice(gold: 10);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.meleeCrit += 5;
            player.magmaStone = true;
            player.starCloak = true;
            player.GetModPlayer<AQPlayer>().hyperCrystal = true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<Amulet>());
            r.AddIngredient(ModContent.ItemType<Ultranium>());
            r.AddIngredient(ItemID.MagmaStone);
            r.AddIngredient(ItemID.StarCloak);
            r.AddIngredient(ModContent.ItemType<Materials.Energies.UltimateEnergy>());
            r.AddTile(TileID.TinkerersWorkbench);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}