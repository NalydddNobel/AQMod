using AQMod.Items.Materials.Energies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories
{
    public class NeutralityAmulet : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.accessory = true;
            item.defense = 2;
            item.rare = ItemRarityID.LightRed + 1;
            item.value = Item.sellPrice(gold: 3, silver: 50);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            aQPlayer.holyDamage += 0.1f;
            aQPlayer.unholyDamage += 0.1f;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<DarkAmulet>());
            r.AddIngredient(ModContent.ItemType<LightAmulet>());
            r.AddIngredient(ModContent.ItemType<UltimateEnergy>());
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}