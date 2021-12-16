using AQMod.Items.DrawOverlays;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Materials.Energies
{
    public class AquaticEnergy : ModItem, IItemOverlaysWorldDraw, IItemOverlaysDrawInventory
    {
        private static readonly EnergyOverlay _overlay = new EnergyOverlay(
            () => Color.Lerp(new Color(200, 200, 255, 180), new Color(111, 111, 190, 180), ((float)Math.Sin(Main.GlobalTime * 2f) + 1f) / 2f),
            () => Color.Lerp(new Color(40, 12, 255, 160), new Color(155, 100, 250, 11), ((float)Math.Sin(Main.GlobalTime * 4f) + 1f) / 2f));

        IOverlayDrawWorld IItemOverlaysWorldDraw.WorldDraw => _overlay;
        IOverlayDrawInventory IItemOverlaysDrawInventory.InventoryDraw => _overlay;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[item.type] = true;
        }

        public override void SetDefaults()
        {
            AQItem.Reps.Energy_SetDefaults(item, ItemRarityID.Blue, AQItem.Prices.EnergySellValue);
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
            AQItem.Reps.Energy_DoUpdate(item,
                Color.Lerp(new Color(200, 200, 255, 0), new Color(111, 111, 190, 0), ((float)Math.Sin(Main.GlobalTime * 2f) + 1f) / 2f),
                new Vector3(0.3f, 0.3f, 0.8f));
        }
    }
}