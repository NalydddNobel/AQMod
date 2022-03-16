using AQMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Items.Misc
{
    public abstract class TreasureBag : ModItem
    {
        protected abstract int InternalRarity { get; }

        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 24;
            item.height = 24;
            item.rare = InternalRarity;
            item.expert = true;
        }

        public override bool CanRightClick() => true;

        public override void PostUpdate()
        {
            return;
            Lighting.AddLight(item.Center, Color.White.ToVector3() * 0.4f);

            if (Main.GameUpdateCount % 12 == 0)
            {
                Vector2 center = item.Center + new Vector2(0f, item.height * -0.1f);
                var direction = Main.rand.NextVector2CircularEdge(item.width * 0.6f, item.height * 0.6f);
                float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
                var velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);

                var d = Dust.NewDustPerfect(center + direction * distance, ModContent.DustType<SilverFlameClone>(), velocity);
                d.scale = 0.5f;
                d.fadeIn = 1.1f;
                d.noGravity = true;
                d.noLight = true;
                d.alpha = 0;
            }
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return true;
            var texture = Main.itemTexture[item.type];
            Rectangle frame;
            if (Main.itemAnimations[item.type] != null)
            {
                frame = Main.itemAnimations[item.type].GetFrame(texture);
            }
            else
            {
                frame = texture.Frame();
            }
            var frameOrigin = frame.Size() / 2f;
            var offset = new Vector2(item.width / 2 - frameOrigin.X, item.height - frame.Height);
            var drawPos = item.position - Main.screenPosition + frameOrigin + offset;
            float time = Main.GlobalTime;
            float timer = Main.GameUpdateCount / 240f + time * 0.04f;
            time %= 4f;
            time /= 2f;
            if (time >= 1f)
            {
                time = 2f - time;
            }
            time = time * 0.5f + 0.5f;
            for (float i = 0f; i < 1f; i += 0.25f)
            {
                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy((i + timer) * MathHelper.TwoPi) * time, frame, new Color(90, 70, 255, 50), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }
            for (float i = 0f; i < 1f; i += 0.34f)
            {
                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy((i + timer) * MathHelper.TwoPi) * time, frame, new Color(140, 120, 255, 77), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }
            return true;
        }
    }
}