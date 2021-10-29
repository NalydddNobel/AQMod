using AQMod.Common;
using AQMod.Common.ItemOverlays;
using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Materials.Energies
{
    public class UltimateEnergy : ModItem
    {
        public override string Texture
        {
            get
            {
                string path = AQUtils.GetPath<UltimateEnergy>();
                if (AQMod.AprilFools)
                    return path + "_AprilFools";
                return path;
            }
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[item.type] = true;
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new EnergyOverlayData(outline, spotlight, new Vector2(0f, -2f)), item.type);
        }

        private static Color outline(float colorOffset)
        {
            return new Color(210 + Main.DiscoR, 210 + Main.DiscoG, 210 + Main.DiscoB, 180);
        }

        private static Color spotlight(float colorOffset)
        {
            return new Color(230 + Main.DiscoB / 2, 230 + Main.DiscoR / 2, 230 + Main.DiscoG / 2, 180);
        }

        public override void SetDefaults()
        {
            AQItem.energy_SetDefaults(item, ItemRarityID.Lime, Item.sellPrice(gold: 1));
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
            var color = outline(Main.GlobalTime * 2f);
            color.A = 0;
            AQItem.energy_DoUpdate(item, color, new Vector3(0.7f, 0.7f, 0.7f));
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