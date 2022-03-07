using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Materials.Energies
{
    public class UltimateEnergy : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 24;
            item.rare = ItemRarityID.Pink;
            item.value = AQItem.Prices.EnergySellValue;
            item.maxStack = 999;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            AQItem.DrawEnergyItemInv(spriteBatch, Coloring.UltimateGrad, item, position, origin, scale);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            AQItem.DrawEnergyItemWorld(spriteBatch, Coloring.UltimateGrad, item, rotation, scale);
            return false;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            AQItem.UpdateEnergyItem(item, Coloring.UltimateGrad.GetColor(Main.GlobalTime), new Vector3(0.3f, 0.3f, 0.8f));
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<AquaticEnergy>());
            r.AddIngredient(ModContent.ItemType<OrganicEnergy>());
            r.AddIngredient(ModContent.ItemType<AtmosphericEnergy>());
            r.AddIngredient(ModContent.ItemType<DemonicEnergy>());
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>());
            r.AddTile(TileID.DemonAltar);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}