using AQMod.Common;
using AQMod.Common.ItemOverlays;
using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items
{
    public class OrganicEnergy : ModItem
    {
        public override string Texture
        {
            get
            {
                string path = AQUtils.GetPath<OrganicEnergy>();
                if (AQMod.AprilFools)
                    return path + "_AprilFools";
                return path;
            }
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[item.type] = true;
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new EnergyOverlayData(outline, spotlight), item.type);
        }

        private static Color outline(float colorOffset)
        {
            return Color.Lerp(new Color(120, 255, 60, 180), new Color(160, 250, 70, 180), ((float)Math.Sin(Main.GlobalTime * 2f) + 1f) / 2f);
        }

        private static Color spotlight(float colorOffset)
        {
            return Color.Lerp(new Color(160, 250, 70, 180), new Color(120, 255, 60, 11), ((float)Math.Sin(Main.GlobalTime * 4f) + 1f) / 2f);
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.rare = ItemRarityID.Green;
            item.value = AQItem.EnergySellValue;
            item.maxStack = 999;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 200);
        }

        public override void PostUpdate()
        {
            int chance = 15;
            if (item.velocity.Length() > 1f)
                chance = 5;
            if (Main.rand.NextBool(chance))
            {
                int d = Dust.NewDust(item.position, item.width, item.height, 44);
                Main.dust[d].scale = Main.rand.NextFloat(0.7f, 1.5f);
                if (Main.dust[d].scale > 1f)
                    Main.dust[d].noGravity = true;
                Main.dust[d].velocity = new Vector2(Main.rand.NextFloat(2f, 4f), 0f).RotatedBy((Main.dust[d].position - item.Center).ToRotation() + Main.rand.NextFloat(-0.1f, 0.1f));
            }
            Lighting.AddLight(item.position, new Vector3(0.5f, 1f, 0.3f));
        }
    }
}