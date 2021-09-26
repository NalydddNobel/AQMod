using AQMod.Assets.ItemOverlays;
using AQMod.Assets.Textures;
using AQMod.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Misc.Energies
{
    public class DemonicEnergy : ModItem
    {
        public override string Texture => AQMod.AprilFools ? AQUtils.GetPath(this) + "_AprilFools" : AQUtils.GetPath(this);

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[item.type] = true;
            if (!Main.dedServ)
            {
                ItemOverlayLoader.Register(new EnergyOverlay(GlowID.DemonicEnergy, outline, spotlight), item.type);
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

        private static Color outline(float colorOffset) => new Color((int)(255 - colorOffset), (int)(100 + colorOffset / 2), 10, 180);
        private static Color spotlight(float colorOffset) => new Color((int)(150 + colorOffset), (int)(180 - colorOffset), 10, 255);

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            return true;
        }

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
                int d = Dust.NewDust(item.position, item.width, item.height, DustID.Fire);
                Main.dust[d].scale = Main.rand.NextFloat(0.7f, 1.5f);
                if (Main.dust[d].scale > 1f)
                    Main.dust[d].noGravity = true;
                Main.dust[d].velocity = new Vector2(Main.rand.NextFloat(2f, 4f), 0f).RotatedBy((Main.dust[d].position - item.Center).ToRotation() + Main.rand.NextFloat(-0.1f, 0.1f));
            }
            Lighting.AddLight(item.position, new Vector3(0.8f, 0.6f, 0.3f));
        }
    }
}
