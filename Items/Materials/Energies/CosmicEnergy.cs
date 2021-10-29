using AQMod.Common;
using AQMod.Common.Config;
using AQMod.Common.ItemOverlays;
using AQMod.Common.Utilities;
using AQMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Materials.Energies
{
    public class CosmicEnergy : ModItem
    {
        public override string Texture
        {
            get
            {
                string path = AQUtils.GetPath<CosmicEnergy>();
                if (AQMod.AprilFools)
                    return path + "_AprilFools";
                if (ModContent.GetInstance<AQConfigClient>().CosmicEnergyAlt)
                    return path + "_Alt";
                return path;
            }
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[item.type] = true;
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new EnergyOverlayData(outline, spotlight, new Vector2(0f, 0f)), item.type);
        }

        private static Color outline(float colorOffset)
        {
            return Color.Lerp(new Color(210, 150, 255, 180), new Color(160, 50, 255, 180), ((float)Math.Sin(Main.GlobalTime * 2f) + 1f) / 2f);
        }

        private static Color spotlight(float colorOffset)
        {
            return Color.Lerp(new Color(180, 160, 255, 180), new Color(220, 2, 250, 11), ((float)Math.Sin(Main.GlobalTime * 4f) + 1f) / 2f);
        }

        public override void SetDefaults()
        {
            AQItem.energy_SetDefaults(item, ItemRarityID.Green, AQItem.EnergySellValue);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 255);

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return false;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            var color = outline(Main.GlobalTime * 2f);
            color.A = 0;
            AQItem.energy_DoUpdate(item, color, new Vector3(0.6f, 0.1f, 0.65f));
        }
    }
}