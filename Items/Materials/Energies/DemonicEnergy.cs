using AQMod.Assets.ItemOverlays;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Materials.Energies
{
    public class DemonicEnergy : ModItem
    {
        public override string Texture
        {
            get
            {
                string path = AQUtils.GetPath<DemonicEnergy>();
                if (AQMod.AprilFools)
                    return path + "_AprilFools";
                return path;
            }
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[item.type] = true;
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new EnergyOverlay(outline, spotlight, new Vector2(2f, 0f)), item.type);
        }

        private static Color outline(float colorOffset)
        {
            return Color.Lerp(new Color(255, 190, 30, 180), new Color(255, 230, 30, 180), ((float)Math.Sin(Main.GlobalTime * 2f) + 1f) / 2f);
        }

        private static Color spotlight(float colorOffset)
        {
            return Color.Lerp(new Color(230, 150, 10, 10), new Color(255, 255, 100, 180), ((float)Math.Sin(Main.GlobalTime * 4f) + 1f) / 2f);
        }

        public override void SetDefaults()
        {
            AQItem.Similarities.Energy_SetDefaults(item, ItemRarityID.Green, AQItem.Prices.EnergySellValue);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return false;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            var color = outline(Main.GlobalTime * 2f);
            color.A = 0;
            AQItem.Similarities.Energy_DoUpdate(item, color, new Vector3(0.8f, 0.6f, 0.3f));
        }
    }
}
