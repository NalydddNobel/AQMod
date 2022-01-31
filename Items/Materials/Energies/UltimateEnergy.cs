using AQMod.Items.DrawOverlays;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Materials.Energies
{
    public class UltimateEnergy : ModItem, IItemOverlaysWorldDraw, IItemOverlaysDrawInventory
    {
        private static readonly EnergyOverlay _overlay = new EnergyOverlay(
             () => new Color(210 + Main.DiscoR, 210 + Main.DiscoG, 210 + Main.DiscoB, 180),
             () => new Color(230 + Main.DiscoB / 2, 230 + Main.DiscoR / 2, 230 + Main.DiscoG / 2, 180), new Vector2(0f, -2f));

        IOverlayDrawWorld IItemOverlaysWorldDraw.WorldDraw => _overlay;
        IOverlayDrawInventory IItemOverlaysDrawInventory.InventoryDraw => _overlay;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[item.type] = true;
        }

        public override void SetDefaults()
        {
            AQItem.Commons.Energy_SetDefaults(item, ItemRarityID.Lime, Item.sellPrice(gold: 1));
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return false;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            AQItem.Commons.Energy_DoUpdate(item, new Color(210 + Main.DiscoR, 210 + Main.DiscoG, 210 + Main.DiscoB, 0), new Vector3(0.7f, 0.7f, 0.7f));
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