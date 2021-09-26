using AQMod.Assets.ItemOverlays;
using AQMod.Assets.Textures;
using AQMod.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Misc.Energies
{
    public class OrganicEnergy : ModItem
    {
        public override string Texture => AQMod.AprilFools ? AQUtils.GetPath(this) + "_AprilFools" : AQUtils.GetPath(this);

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[item.type] = true;
            if (!Main.dedServ)
            {
                ItemOverlayLoader.Register(new EnergyOverlay(GlowID.OrganicEnergy, outline, spotlight), item.type);
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
            return new Color(255, 255, 255, 200);
        }

        private static Color outline(float colorOffset) => new Color(50, (int)(255 - colorOffset), 10, 180);
        private static Color spotlight(float colorOffset) => new Color(50, (int)(255 - colorOffset), 10, 180);

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