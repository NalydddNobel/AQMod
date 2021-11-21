using AQMod.Assets.Graphics.Particles;
using AQMod.Assets.Graphics.ParticlesLayers;
using AQMod.Items.Materials.Energies;
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
            item.createTile = ModContent.TileType<Tiles.Torches>();
            item.flame = true;
            item.value = 50;
            item.rare = ItemRarityID.Blue;
            item.placeStyle = Tiles.Torches.SparklingTorch;
        }

        public override void HoldItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI && Main.rand.Next(player.itemAnimation > 0 ? 30 : 44) == 0)
            {
                float scale = Main.rand.NextFloat(0.88f, 1f);
                var particlePos = new Vector2(player.itemLocation.X + (12f + Main.rand.NextFloat(-2f, 2f)) * player.direction, player.itemLocation.Y - (20f + Main.rand.NextFloat(-2f, 2f)) * player.gravDir);
                var particleVelocity = new Vector2(player.velocity.X * 0.1f, player.velocity.Y * 0.1f - 3f + Main.rand.NextFloat(-3f, 1f));
                ParticleLayers.AddParticle_PostDrawPlayers(new MonoParticleEmber(particlePos,
                    particleVelocity, 
                    new Color(240, 240, 255, 0) * scale,
                    scale));
                ParticleLayers.AddParticle_PostDrawPlayers(new SparkleParticle(particlePos,
                    particleVelocity, 
                    new Color(240, 240, 255, 0) * scale,
                    scale * 0.33f));
                ParticleLayers.AddParticle_PostDrawPlayers(new SparkleParticle(particlePos,
                    particleVelocity,
                    new Color(200, 200, 240, 0) * scale,
                    scale * 0.1f)
                { rotation = MathHelper.PiOver4 });
            }
            Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);
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
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Torch, 50);
            recipe.AddIngredient(ModContent.ItemType<CosmicEnergy>());
            recipe.SetResult(this, 50);
            recipe.AddRecipe();
        }
    }
}
