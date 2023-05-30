using Aequus.Common.Effects;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials.Energies {
    public abstract class EnergyItemBase : ModItem
    {
        protected abstract Vector3 LightColor { get; }
        public abstract int Rarity { get; }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.Energies;
            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.rare = Rarity;
            Item.value = Item.sellPrice(silver: 10);
            Item.maxStack = Item.CommonMaxStack;
            Item.Aequus().itemGravityCheck = 255;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var color = itemColor.A > 0 ? itemColor : drawColor;
            spriteBatch.Draw(TextureAssets.Item[Type].Value, position, null, color, 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return false;
        }

        // In PostDraw to fix an issue with that one mod which adds auras around dropped items
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            var itemTexture = TextureAssets.Item[Type].Value;
            var frame = new Rectangle(0, 0, itemTexture.Width, itemTexture.Height);
            var origin = frame.Size() / 2f;
            float y = Helper.Wave(Main.GlobalTimeWrappedHourly * 2f, -1f, 1f);
            var drawPosition = new Vector2(Item.position.X - Main.screenPosition.X + origin.X + Item.width / 2 - origin.X, Item.position.Y - Main.screenPosition.Y + origin.Y + Item.height - frame.Height + 4f + y * 1.5f).Floor();

            var rand = LegacyEffects.EffectRand;
            int oldRand = rand.SetRand(whoAmI);
            var rayTexture = AequusTextures.LightRay2;
            var rayOrigin = new Vector2(rayTexture.Value.Width / 2f, rayTexture.Value.Height);
            var rayColor = new Color(LightColor);
            int amt = 3;
            float rot = MathHelper.TwoPi / amt;
            for (int i = 0; i < amt * 2; i++)
            {
                float rayOpacity = Helper.Wave(Main.GlobalTimeWrappedHourly * rand.Rand(0.5f, 0.6f + i % amt * 0.4f), 0.2f, 1f);
                float rayScale = rand.Rand(0.5f, 0.8f) + Helper.Wave(Main.GlobalTimeWrappedHourly * rand.Rand(0.5f, 0.6f + i % amt * 0.4f), -0.1f, 0.1f);
                float rayRotation = rot * i + rand.Rand(rot) + Main.GlobalTimeWrappedHourly * rand.Rand(0.5f + i % amt * 0.3f, 0.6f + i % amt * 0.4f) * (i >= amt ? -1 : 1) + rotation;
                spriteBatch.Draw(rayTexture.Value, drawPosition, null, rayColor.UseA(100) * rayOpacity * rayScale, rayRotation, rayOrigin, new Vector2(rayScale * rand.Rand(1f, 2f) * 0.2f, rayScale * 0.6f), SpriteEffects.None, 0f);
            }
            rand.SetRand(oldRand);

            spriteBatch.Draw(itemTexture, drawPosition, frame, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (Item.timeSinceItemSpawned % 18 == 0)
            {
                var d = Dust.NewDustDirect(Item.position, Item.width, Item.height - 4, ModContent.DustType<EnergyDust>(), 0f, 0f, 0, new Color(LightColor * 2f).HueAdd(Main.rand.NextFloat(-0.05f, 0.05f)).SaturationMultiply(0.3f).UseA(0));
                d.velocity += Vector2.Normalize(d.position - Item.Center) * d.velocity.Length();
                d.alpha = Main.rand.Next(0, 35);
                d.scale = Main.rand.NextFloat(0.95f, 1.15f);
                if (d.scale > 1f)
                    d.noGravity = true;
                d.velocity *= 0.5f;
                d.velocity += Item.velocity * -0.2f;
            }
            Lighting.AddLight(Item.position, LightColor);
        }

        public override void GrabRange(Player player, ref int grabRange)
        {
            grabRange += 96;
        }
    }
}