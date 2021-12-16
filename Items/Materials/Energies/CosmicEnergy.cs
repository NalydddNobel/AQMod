using AQMod.Items.DrawOverlays;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Materials.Energies
{
    public class CosmicEnergy : ModItem, IItemOverlaysWorldDraw, IItemOverlaysDrawInventory
    {
        public override string Texture
        {
            get
            {
                string path = AQUtils.GetPath<CosmicEnergy>();
                if (ModContent.GetInstance<AQConfigClient>().CosmicEnergyAlt)
                    return path + "_Alt";
                return path;
            }
        }

        private static readonly EnergyOverlay _overlay = new EnergyOverlay(
            () => Color.Lerp(new Color(210, 150, 255, 180), new Color(160, 50, 255, 180), ((float)Math.Sin(Main.GlobalTime * 2f) + 1f) / 2f),
            () => Color.Lerp(new Color(180, 160, 255, 180), new Color(220, 2, 250, 11), ((float)Math.Sin(Main.GlobalTime * 4f) + 1f) / 2f));

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

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 255);

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return false;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            AQItem.Reps.Energy_DoUpdate(item,
                Color.Lerp(new Color(210, 150, 255, 0), new Color(160, 50, 255, 0), ((float)Math.Sin(Main.GlobalTime * 2f) + 1f) / 2f), new Vector3(0.6f, 0.1f, 0.65f));
        }
    }
}