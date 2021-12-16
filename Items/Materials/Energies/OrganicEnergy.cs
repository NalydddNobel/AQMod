using AQMod.Assets.LegacyItemOverlays;
using AQMod.Items.DrawOverlays;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Materials.Energies
{
    public class OrganicEnergy : ModItem, IItemOverlaysWorldDraw, IItemOverlaysDrawInventory
    {
        private static readonly EnergyOverlay _overlay = new EnergyOverlay(
          () => Color.Lerp(new Color(120, 255, 60, 180), new Color(160, 250, 70, 180), ((float)Math.Sin(Main.GlobalTime * 2f) + 1f) / 2f),
          () => Color.Lerp(new Color(160, 250, 70, 180), new Color(120, 255, 60, 11), ((float)Math.Sin(Main.GlobalTime * 4f) + 1f) / 2f));

        IOverlayDrawWorld IItemOverlaysWorldDraw.WorldDraw => _overlay;
        IOverlayDrawInventory IItemOverlaysDrawInventory.InventoryDraw => _overlay;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[item.type] = true;
        }

        public override void SetDefaults()
        {
            AQItem.Reps.Energy_SetDefaults(item, ItemRarityID.Green, AQItem.Prices.EnergySellValue);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 200);
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            AQItem.Reps.Energy_DoUpdate(item, 
                Color.Lerp(new Color(120, 255, 60, 0), new Color(160, 250, 70, 0), ((float)Math.Sin(Main.GlobalTime * 2f) + 1f) / 2f), new Vector3(0.5f, 1f, 0.3f));
        }
    }
}