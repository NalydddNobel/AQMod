﻿using AQMod.Assets.ItemOverlays;
using AQMod.Assets.Textures;
using AQMod.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Misc.Energies
{
    public class AtmosphericEnergy : ModItem
    {
        public override string Texture => AQMod.AprilFools ? AQUtils.GetPath(this) + "_AprilFools" : AQUtils.GetPath(this);

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[item.type] = true;
            if (!Main.dedServ)
            {
                GlowID id = GlowID.AtmosphericEnergy;
                if (AQMod.AprilFools)
                {
                    id = GlowID.AtmosphericEnergyAprilFools;
                }
                ItemOverlayLoader.Register(new EnergyOverlay(id, outline, spotlight), item.type);
            }
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
            return new Color(255, 255, 255, 255);
        }

        private static Color outline(float colorOffset) => new Color(215, (int)(160 + colorOffset), (int)(150 - colorOffset / 2), 180);
        private static Color spotlight(float colorOffset) => new Color(130, (int)(160 - colorOffset / 2f), (int)(45 + colorOffset), 255);

        public override void PostUpdate()
        {
            int chance = 15;
            if (item.velocity.Length() > 1f)
                chance = 5;
            if (Main.rand.NextBool(chance))
            {
                int d = Dust.NewDust(item.position, item.width, item.height, 64);
                Main.dust[d].scale = Main.rand.NextFloat(0.7f, 1.5f);
                if (Main.dust[d].scale > 1f)
                    Main.dust[d].noGravity = true;
                Main.dust[d].velocity = new Vector2(Main.rand.NextFloat(2f, 4f), 0f).RotatedBy((Main.dust[d].position - item.Center).ToRotation() + Main.rand.NextFloat(-0.1f, 0.1f));
            }
            Lighting.AddLight(item.position, new Vector3(0.6f, 0.1f, 0.65f));
        }
    }
}