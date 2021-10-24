using AQMod.Common.Utilities;
using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Placeable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Fishing.Rods
{
    public class CrabRod : ModItem
    {
        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.WoodFishingPole);
            item.fishingPole = 18;
            item.shootSpeed = 10f;
            item.rare = ItemRarityID.Blue;
            item.shoot = ModContent.ProjectileType<CrabBobber>();
        }


        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<CrabShell>(), 8);
            r.AddIngredient(ModContent.ItemType<AquaticEnergy>());
            r.AddIngredient(ModContent.ItemType<ExoticCoral>(), 40);
            r.AddTile(ModContent.TileType<Tiles.FishingCraftingStation>());
            r.SetResult(this);
            r.AddRecipe();
        }
    }

    public class CrabBobber : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.BobberWooden);
            drawOriginOffsetY = -8;
        }

        public override bool PreDrawExtras(SpriteBatch spriteBatch)
        {
            var player = Main.player[projectile.owner];
            if (!projectile.bobber || player.inventory[player.selectedItem].holdStyle <= 0)
                return false;
            DrawMethods.DrawFishingLine(new Color(175, 146, 146, 255), player, projectile.position, projectile.width, projectile.height, projectile.velocity, projectile.localAI[0], new Vector2(45f, 39f));
            return false;
        }
    }
}