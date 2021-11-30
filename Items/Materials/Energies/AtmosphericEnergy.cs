using AQMod.Assets.ItemOverlays;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Materials.Energies
{
    public class AtmosphericEnergy : ModItem
    {
        public override string Texture
        {
            get
            {
                string path = AQUtils.GetPath<AtmosphericEnergy>();
                if (AQMod.AprilFools)
                    return path + "_AprilFools";
                return path;
            }
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[item.type] = true;
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new EnergyOverlay(outline, spotlight, new Vector2(0f, 0f)), item.type);
        }

        private static Color outline(float colorOffset)
        {
            return Color.Lerp(new Color(255, 150, 110, 180), new Color(255, 255, 110, 180), ((float)Math.Sin(Main.GlobalTime * 2f) + 1f) / 2f);
        }

        private static Color spotlight(float colorOffset)
        {
            return Color.Lerp(new Color(255, 255, 150, 180), new Color(255, 120, 50, 11), ((float)Math.Sin(Main.GlobalTime * 4f) + 1f) / 2f);
        }

        public override void SetDefaults()
        {
            AQItem.Similarities.Energy_SetDefaults(item, ItemRarityID.LightRed, AQItem.Prices.EnergySellValue);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255);
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            var color = outline(Main.GlobalTime * 2f);
            color.A = 0;
            AQItem.Similarities.Energy_DoUpdate(item, color, new Vector3(0.6f, 0.1f, 0.65f));
        }
    }
}