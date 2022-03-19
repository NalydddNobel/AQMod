using AQMod.Dusts;
using AQMod.Items.Materials.Energies;
using AQMod.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Placeable.Torch
{
    public class SparklingTorch : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 10;
            item.height = 12;
            item.maxStack = 999;
            item.holdStyle = 1;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.createTile = ModContent.TileType<Torches>();
            item.value = 50;
            item.flame = true;
            item.rare = ItemRarityID.Blue;
            item.placeStyle = Torches.SparklingTorch;
        }

        public override void HoldItem(Player player)
        {
            Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 10f * player.direction, player.itemLocation.Y - 10f), true);
            if (Main.myPlayer == player.whoAmI)
            {
                Dust.NewDustPerfect(position, ModContent.DustType<SparklerDust>(), new Vector2(2f * player.direction, -2f).RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.75f, 1f) + player.velocity * 0.3f, 0, new Color(255, 255, 255, 255), Main.rand.NextFloat(0.5f, 0.9f));
            }
            Lighting.AddLight(position, 0.9f, 0.9f, 1f);
        }

        public override void PostUpdate()
        {
            Lighting.AddLight((int)((item.position.X + item.width / 2) / 16f), (int)((item.position.Y + item.height / 2) / 16f), 0.9f, 0.9f, 1f);
        }

        public override void AutoLightSelect(ref bool dryTorch, ref bool wetTorch, ref bool glowstick)
        {
            wetTorch = true;
        }

        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Torch, 120);
            recipe.AddIngredient(ModContent.ItemType<CosmicEnergy>());
            recipe.SetResult(this, 120);
            recipe.AddRecipe();
        }
    }
}
