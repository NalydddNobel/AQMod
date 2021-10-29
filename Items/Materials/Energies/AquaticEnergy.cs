using AQMod.Common;
using AQMod.Common.ItemOverlays;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Materials.Energies
{
    public class AquaticEnergy : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[item.type] = true;
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new EnergyOverlayData(outline, spotlight, new Vector2(0f, 0f)), item.type);
        }

        private static Color outline(float colorOffset)
        {
            return Color.Lerp(new Color(200, 200, 255, 180), new Color(111, 111, 190, 180), ((float)Math.Sin(Main.GlobalTime * 2f) + 1f) / 2f);
        }

        private static Color spotlight(float colorOffset)
        {
            return Color.Lerp(new Color(40, 12, 255, 160), new Color(155, 100, 250, 11), ((float)Math.Sin(Main.GlobalTime * 4f) + 1f) / 2f);
        }

        public override void SetDefaults()
        {
            AQItem.energy_SetDefaults(item, ItemRarityID.Blue, AQItem.EnergySellValue);
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
            AQItem.energy_DoUpdate(item, color, new Vector3(0.3f, 0.3f, 0.8f));
        }
    }
}