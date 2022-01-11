using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using AQMod.Tiles.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools.Fishing.CrabPots
{
    public class CrabPot : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 10;
            item.height = 10;
            item.consumable = true;
            item.maxStack = 999;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.rare = ItemRarityID.Blue;
            item.shootSpeed = 10f;
        }

        public override bool UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                var center = player.MountedCenter;
                int i = Content.Entities.CrabPot.NewCrabPot(center, Vector2.Normalize(Main.MouseWorld - center) * item.shootSpeed, 0);
                return i != -1;
            }
            return true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddRecipeGroup(AQRecipes.RecipeGroups.CopperOrTin, 8);
            r.AddIngredient(ModContent.ItemType<AquaticEnergy>());
            r.AddIngredient(ModContent.ItemType<CrabShell>());
            r.AddTile(ModContent.TileType<FishingCraftingStation>());
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}