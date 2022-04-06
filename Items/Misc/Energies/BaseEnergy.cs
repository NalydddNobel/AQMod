using Aequus.Common.Utilities.Coloring;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Energies
{
    public abstract class BaseEnergy : ModItem
    {
        protected abstract IColorGradient Gradient { get; }
        protected abstract Vector3 LightColor { get; }
        public abstract int Rarity { get; }
        protected virtual Vector2 BloomOffset => Vector2.Zero;
        protected Asset<Texture2D> Aura;

        public override void SetStaticDefaults()
        {
            Aura = ModContent.Request<Texture2D>(Texture + "_Aura");
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            this.SetResearch(15);
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.rare = Rarity;
            Item.value = ItemPrices.EnergySellValue;
            Item.maxStack = 999;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            spriteBatch.Draw(Aura.Value, position, null, Gradient.GetColor(Main.GlobalTimeWrappedHourly).UseA(0), 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureAssets.Item[Type].Value, position, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            var itemTexture = TextureAssets.Item[Type].Value;
            var frame = new Rectangle(0, 0, itemTexture.Width, itemTexture.Height);
            Vector2 origin = frame.Size() / 2f;
            var drawPosition = new Vector2(Item.position.X - Main.screenPosition.X + origin.X + Item.width / 2 - origin.X, Item.position.Y - Main.screenPosition.Y + origin.Y + Item.height - frame.Height);
            drawPosition = new Vector2((int)drawPosition.X, drawPosition.Y);
            var coloring = Gradient.GetColor(Main.GlobalTimeWrappedHourly).UseA(0);

            spriteBatch.Draw(Aura.Value, drawPosition, frame, coloring, rotation, origin, scale, SpriteEffects.None, 0f);

            var bloomTexture = Aequus.MyTex("Assets/Bloom");
            var bloomOrigin = bloomTexture.Size() / 2f;
            float bloomScale = (scale + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 4f) * 0.1f + 0.65f) * 36f;
            spriteBatch.Draw(bloomTexture, drawPosition, null, coloring, rotation, bloomOrigin, scale / bloomTexture.Width, SpriteEffects.None, 0f);

            spriteBatch.Draw(itemTexture, drawPosition, frame, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (Item.timeSinceItemSpawned % 12 == 0)
            {
                int d = Dust.NewDust(Item.position, Item.width, Item.height - 4, ModContent.DustType<EnergyDust>(), 0f, 0f, 0, Gradient.GetColor(Main.GlobalTimeWrappedHourly).UseA(0));
                Main.dust[d].alpha = Main.rand.Next(0, 35);
                Main.dust[d].scale = Main.rand.NextFloat(0.95f, 1.15f);
                if (Main.dust[d].scale > 1f)
                    Main.dust[d].noGravity = true;
                Main.dust[d].velocity = new Vector2(Main.rand.NextFloat(-0.15f, 0.15f), Main.rand.NextFloat(-3.5f, -1.75f));
            }
            Lighting.AddLight(Item.position, LightColor);
        }

    }
}