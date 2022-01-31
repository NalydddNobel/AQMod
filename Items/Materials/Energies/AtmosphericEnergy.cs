using AQMod.Items.DrawOverlays;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Materials.Energies
{
    public class AtmosphericEnergy : ModItem, IItemOverlaysWorldDraw, IItemOverlaysDrawInventory
    {
        private static readonly EnergyOverlay _overlay = new EnergyOverlay(
            () => Color.Lerp(new Color(255, 150, 110, 180), new Color(255, 255, 110, 180), ((float)Math.Sin(Main.GlobalTime * 2f) + 1f) / 2f),
            () => Color.Lerp(new Color(255, 255, 150, 180), new Color(255, 120, 50, 11), ((float)Math.Sin(Main.GlobalTime * 4f) + 1f) / 2f));

        IOverlayDrawWorld IItemOverlaysWorldDraw.WorldDraw => _overlay;
        IOverlayDrawInventory IItemOverlaysDrawInventory.InventoryDraw => _overlay;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[item.type] = true;
        }

        public override void SetDefaults()
        {
            AQItem.Commons.Energy_SetDefaults(item, ItemRarityID.LightRed, AQItem.Prices.EnergySellValue);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255);
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            AQItem.Commons.Energy_DoUpdate(item, Color.Lerp(new Color(255, 150, 110, 0), new Color(255, 255, 110, 0), ((float)Math.Sin(Main.GlobalTime * 2f) + 1f) / 2f), new Vector3(0.6f, 0.1f, 0.65f));
        }
    }
}