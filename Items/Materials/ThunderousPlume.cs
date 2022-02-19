using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Materials
{
    public class ThunderousPlume : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.sellPrice(silver: 10);
            item.rare = ItemRarityID.LightPurple;
            item.maxStack = 999;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return (lightColor * 0.2f).UseA(255);
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var clr = Main.LocalPlayer.FX().NalydGradientPersonal.GetColor(Main.GlobalTime) * 0.2f;
            var texture = Main.itemTexture[item.type];
            var wave = AQUtils.Wave(Main.GlobalTime * 10f, 0f, 1f);
            var n = new Vector2(wave, 0f).RotatedBy(Main.GlobalTime * 2.9f);
            clr.A = 0;
            for (int i = 1; i <= 4; i++)
            {
                spriteBatch.Draw(texture, position + n * i * scale, frame, clr, 0f, origin, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, position - n * i * scale, frame, clr, 0f, origin, scale, SpriteEffects.None, 0);
            }
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            var clr = Main.LocalPlayer.FX().NalydGradientPersonal.GetColor(Main.GlobalTime) * 0.2f;
            var texture = Main.itemTexture[item.type];
            var frame = texture.Frame();
            var origin = frame.Size() / 2f;
            var position = new Vector2(item.position.X - Main.screenPosition.X + texture.Width / 2 + item.width / 2 - texture.Width / 2, item.position.Y - Main.screenPosition.Y + texture.Height / 2 + item.height - texture.Height + 2f);
            var wave = AQUtils.Wave(Main.GlobalTime * 10f, 0f, 1f);
            var n = new Vector2(wave, 0f).RotatedBy(Main.GlobalTime * 2.9f);
            clr.A = 0;
            for (int i = 1; i <= 4; i++)
            {
                spriteBatch.Draw(texture, position + n * i * scale, frame, clr, rotation, origin, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, position - n * i * scale, frame, clr, rotation, origin, scale, SpriteEffects.None, 0);
            }
        }
    }
}