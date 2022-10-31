using Aequus.Common;
using Aequus.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Energies
{
    public abstract class EnergyItemBase : ModItem
    {
        protected abstract Vector3 LightColor { get; }
        public abstract int Rarity { get; }
        public abstract ref StaticMiscShaderInfo Shader { get; }
        public abstract ref Asset<Texture2D> Aura { get; }
        protected virtual Vector2 BloomOffset => Vector2.Zero;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                Aura = ModContent.Request<Texture2D>(Texture + "_Aura");
                Shader = new StaticMiscShaderInfo("MiscEffects", "Aequus:" + Name, "TextureScrollingPass", true);
                Shader.ShaderData
                    .UseImage1(ModContent.Request<Texture2D>("Aequus/Assets/Effects/Textures/" + Name + "Gradient", AssetRequestMode.ImmediateLoad))
                    .UseSaturation(0.75f);
            }
        }

        public override void Unload()
        {
            Shader = null;
            Aura = null;
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.Energies;
            SacrificeTotal = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.rare = Rarity;
            Item.value = Item.sellPrice(silver: 10);
            Item.maxStack = 9999;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var coloring = new Color(255, 255, 255, 200) * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly, 0.8f, 1f);

            SpriteBatchCache cache = null;
            if (Aequus.HQ)
            {
                cache = new SpriteBatchCache(spriteBatch);
                Main.spriteBatch.End();
                Begin.UI.BeginWMatrix(spriteBatch, Begin.Shader, useScissorRectangle: true, cache.transformMatrix);
                var drawData = new DrawData(Aura.Value, position, null, coloring, 0f, origin, scale, SpriteEffects.None, 0);
                Shader.ShaderData.Apply(drawData);

                drawData.Draw(spriteBatch);

                Main.spriteBatch.End();
                cache.Begin(spriteBatch);
            }

            spriteBatch.Draw(TextureAssets.Item[Type].Value, position, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);

            if (Aequus.HQ)
            {
                Main.spriteBatch.End();
                Begin.UI.BeginWMatrix(spriteBatch, Begin.Shader, useScissorRectangle: true, cache.transformMatrix);
                var drawData = new DrawData(Aura.Value, position, null, coloring.UseA(0) * 0.33f, 0f, origin, scale, SpriteEffects.None, 0);
                Shader.ShaderData.Apply(drawData);

                drawData.Draw(spriteBatch);

                Main.spriteBatch.End();
                cache.Begin(spriteBatch);
            }
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            var itemTexture = TextureAssets.Item[Type].Value;
            var frame = new Rectangle(0, 0, itemTexture.Width, itemTexture.Height);
            Vector2 origin = frame.Size() / 2f;
            var drawPosition = new Vector2(Item.position.X - Main.screenPosition.X + origin.X + Item.width / 2 - origin.X, Item.position.Y - Main.screenPosition.Y + origin.Y + Item.height - frame.Height);
            drawPosition = new Vector2((int)drawPosition.X, drawPosition.Y);
            var coloring = new Color(255, 255, 255, 200) * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly, 0.8f, 1f);

            if (Aequus.HQ)
            {
                Main.spriteBatch.End();
                Begin.GeneralEntities.BeginShader(spriteBatch);
                var drawData = new DrawData(Aura.Value, drawPosition, frame, coloring, rotation, origin, scale, SpriteEffects.None, 0);
                Shader.ShaderData.Apply(drawData);

                drawData.Draw(spriteBatch);

                Main.spriteBatch.End();
                Begin.GeneralEntities.Begin(spriteBatch);
            }

            spriteBatch.Draw(itemTexture, drawPosition, frame, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);

            if (Aequus.HQ)
            {
                Main.spriteBatch.End();
                Begin.GeneralEntities.BeginShader(spriteBatch);
                var drawData = new DrawData(Aura.Value, drawPosition, frame, coloring.UseA(0) * 0.33f, rotation, origin, scale, SpriteEffects.None, 0);
                Shader.ShaderData.Apply(drawData);

                drawData.Draw(spriteBatch);

                Main.spriteBatch.End();
                Begin.GeneralEntities.Begin(spriteBatch);
            }
            return false;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            //if (Item.timeSinceItemSpawned % 12 == 0)
            //{
            //    int d = Dust.NewDust(Item.position, Item.width, Item.height - 4, ModContent.DustType<EnergyDust>(), 0f, 0f, 0, Gradient.GetColor(Main.GlobalTimeWrappedHourly).UseA(0));
            //    Main.dust[d].alpha = Main.rand.Next(0, 35);
            //    Main.dust[d].scale = Main.rand.NextFloat(0.95f, 1.15f);
            //    if (Main.dust[d].scale > 1f)
            //        Main.dust[d].noGravity = true;
            //    Main.dust[d].velocity = new Vector2(Main.rand.NextFloat(-0.15f, 0.15f), Main.rand.NextFloat(-3.5f, -1.75f));
            //}
            Lighting.AddLight(Item.position, LightColor);
        }

    }
}