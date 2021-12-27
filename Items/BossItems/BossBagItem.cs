using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.BossItems
{
    public abstract class BossBagItem : ModItem
    {
        protected virtual bool CanDropDevArmor => true;
        protected virtual int rarity => ItemRarityID.Expert;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.BossBag[Type] = true;
            ItemID.Sets.PreHardmodeLikeBossBag[Type] = !CanDropDevArmor;
            AQItem.CreativeMode.TreasureBagItem(this);
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.rare = rarity;
            Item.expert = true;
        }

        public override bool CanRightClick() => true;

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Lerp(lightColor, Color.White, 0.4f);
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.White.ToVector3() * 0.4f);

            if (Item.timeSinceItemSpawned % 12 == 0)
            {
                Vector2 center = Item.Center + new Vector2(0f, Item.height * -0.1f);
                var direction = Main.rand.NextVector2CircularEdge(Item.width * 0.6f, Item.height * 0.6f);
                float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
                var velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);

                Dust dust = Dust.NewDustPerfect(center + direction * distance, DustID.SilverFlame, velocity);
                dust.scale = 0.5f;
                dust.fadeIn = 1.1f;
                dust.noGravity = true;
                dust.noLight = true;
                dust.alpha = 0;
            }
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            var texture = TextureAssets.Item[Item.type].Value;
            Rectangle frame;
            if (Main.itemAnimations[Item.type] != null)
            {
                frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
            }
            else
            {
                frame = texture.Frame();
            }
            var frameOrigin = frame.Size() / 2f;
            var offset = new Vector2(Item.width / 2 - frameOrigin.X, Item.height - frame.Height);
            var drawPos = Item.position - Main.screenPosition + frameOrigin + offset;
            float time = Main.GlobalTimeWrappedHourly;
            float timer = Item.timeSinceItemSpawned / 240f + time * 0.04f;
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