using AQMod.Content.CrossMod;
using AQMod.Items.Weapons.Magic;
using AQMod.Items.Weapons.Melee.Flails;
using AQMod.Items.Weapons.Ranged;
using AQMod.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Placeable
{
    public class CrabsonTrophy : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.value = 50000;
            item.rare = ItemRarityID.Blue;
            item.createTile = ModContent.TileType<Trophies>();
            item.placeStyle = Trophies.Crabson;
        }

        public override void AddRecipes()
        {
            if (FargosQOLStuff.FargowiltasActive)
            {
                var r = new ModRecipe(mod);
                r.AddIngredient(item.type);
                r.AddTile(TileID.Solidifier);
                r.SetResult(ModContent.ItemType<JerryClawFlail>());
                r.AddRecipe();
                r = new ModRecipe(mod);
                r.AddIngredient(item.type);
                r.AddTile(TileID.Solidifier);
                r.SetResult(ModContent.ItemType<CinnabarBow>());
                r.AddRecipe();
                r = new ModRecipe(mod);
                r.AddIngredient(item.type);
                r.AddTile(TileID.Solidifier);
                r.SetResult(ModContent.ItemType<Bubbler>());
                r.AddRecipe();
            }
        }
    }
}