using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Placeable.CraftingStations
{
    public class OrbicularStargaizar : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 12;
            item.maxStack = 999;
            item.useTime = 10;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.createTile = ModContent.TileType<Tiles.OrbicularStargaizar>();
            item.value = Item.buyPrice(gold: 20);
            item.consumable = true;
            item.useTurn = true;
            item.autoReuse = true;
            item.rare = ItemRarityID.Pink;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.CrystalBall);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>(), 5);
            r.AddIngredient(ModContent.ItemType<LightMatter>(), 10);
            r.AddIngredient(ItemID.SoulofNight, 10);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}