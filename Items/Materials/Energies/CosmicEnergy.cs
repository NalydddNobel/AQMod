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
            item.width = 30;
            item.height = 30;
            item.rare = ItemRarityID.Green;
            item.value = AQItem.EnergySellValue;
            item.maxStack = 999;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 255);

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return false;
        }

        public override void PostUpdate()
        {
            int chance = 15;
            if (item.velocity.Length() > 1f)
                chance = 5;
            if (Main.rand.NextBool(chance))
            {
                var color = outline(Main.GlobalTime * 2f);
                color.A = 0;
                int d = Dust.NewDust(item.position, item.width, item.height, ModContent.DustType<EnergyPulse>(), 0f, 0f, 0, color);
                Main.dust[d].alpha = Main.rand.Next(0, 35);
                Main.dust[d].scale = Main.rand.NextFloat(0.95f, 1.15f);
                if (Main.dust[d].scale > 1f)
                    Main.dust[d].noGravity = true;
                Main.dust[d].velocity = new Vector2(Main.rand.NextFloat(-0.15f, 0.15f), Main.rand.NextFloat(-4f, -2.5f));
            }
            Lighting.AddLight(item.position, new Vector3(0.6f, 0.1f, 0.65f));
        }
    }
}