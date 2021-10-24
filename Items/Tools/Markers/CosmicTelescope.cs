using AQMod.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools.Markers
{
    public class CosmicTelescope : MapMarker
    {
        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.consumable = true;
            item.rare = ItemRarityID.Green;
            item.value = Item.sellPrice(gold: 2);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Glass);
            recipe.AddIngredient(ModContent.ItemType<UltimateEnergy>());
            recipe.AddIngredient(ItemID.CobaltBar, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Glass);
            recipe.AddIngredient(ModContent.ItemType<UltimateEnergy>());
            recipe.AddIngredient(ItemID.PalladiumBar, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void GlobeEffects(Player player, TEGlobe globe)
        {
            player.AddBuff(ModContent.BuffType<CosmicMarkerBuff>(), 20);
        }

        public override void PreAddMarker(Player player, TEGlobe globe)
        {
            var rectangle = new Rectangle(globe.Position.X * 16, globe.Position.Y * 16, 32, 32);
            for (int i = 0; i < 12; i++)
            {
                int d = Dust.NewDust(new Vector2(rectangle.X, rectangle.Y), rectangle.Width, rectangle.Height, 261);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity.X *= 0.2f;
                Main.dust[d].velocity.Y = -Main.rand.NextFloat(1f, 3f);
            }
        }
    }

    public class CosmicMarkerBuff : ModBuff
    {
        public override void SetDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<AQPlayer>().cosmicMap = true;
        }
    }
}