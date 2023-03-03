using Aequus;
using Aequus.Items.Misc.Dyes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials
{
    public class OmniGem : ModItem
    {
        public static Asset<Texture2D> Mask { get; private set; }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                Mask = ModContent.Request<Texture2D>($"{Texture}_Mask");
            }
        }

        public override void Unload()
        {
            Mask = null;
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
            ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.Amber;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 50);
            Item.maxStack = 9999;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var texture = TextureAssets.Item[Type].Value;

            Main.spriteBatch.End();
            spriteBatch.Begin_UI(immediate: true, useScissorRectangle: true);

            var drawData = new DrawData(texture, position, frame, itemColor.A > 0 ? itemColor : Main.inventoryBack, 0f, origin, scale, SpriteEffects.None, 0);
            var maskFrame = Mask.Value.Frame(verticalFrames: 3, frameY: 2);

            var effect = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<HueshiftDye>());
            effect.Apply(null, drawData);

            var drawPosition = position + frame.Size() / 2f * scale - origin * scale;
            Main.spriteBatch.Draw(
                Mask.Value,
                drawPosition,
                maskFrame.Frame(0, -1),
                Color.White with { A = 0, } * 0.2f,
                0f,
                maskFrame.Size() / 2f,
                scale, SpriteEffects.None, 0f);

            drawData.Draw(Main.spriteBatch);

            Main.spriteBatch.Draw(
                Mask.Value,
                drawPosition,
                maskFrame,
                Color.White with { A = 0, } * 0.66f,
                0f,
                maskFrame.Size() / 2f,
                scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            spriteBatch.Begin_UI(immediate: false, useScissorRectangle: true);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            var texture = TextureAssets.Item[Type].Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin_World(shader: true);

            var frame = new Rectangle(0, 0, texture.Width, texture.Height);
            var drawPosition = new Vector2(
                Item.position.X - Main.screenPosition.X + frame.Width / 2 + Item.width / 2 - frame.Width / 2,
                Item.position.Y - Main.screenPosition.Y + frame.Height / 2 + Item.height - frame.Height
            );
            var origin = frame.Size() / 2f;
            var drawData = new DrawData(texture, drawPosition, frame, Item.GetAlpha(lightColor), rotation, origin, scale, SpriteEffects.None, 0);

            var effect = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<HueshiftDye>());
            effect.Apply(null, drawData);

            var maskFrame = Mask.Value.Frame(verticalFrames: 3, frameY: 2);

            Main.spriteBatch.Draw(
                Mask.Value,
                drawPosition,
                maskFrame.Frame(0, -1),
                Color.White with { A = 0, } * 0.2f,
                rotation,
                maskFrame.Size() / 2f,
                1f, SpriteEffects.None, 0f);

            drawData.Draw(Main.spriteBatch);

            Main.spriteBatch.Draw(
                Mask.Value,
                drawPosition,
                maskFrame,
                Color.White with { A = 0, } * 0.66f,
                rotation,
                maskFrame.Size() / 2f,
                1f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin_World(shader: false);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Amethyst)
                .AddIngredient(ItemID.Topaz)
                .AddIngredient(ItemID.Sapphire)
                .AddIngredient(ItemID.Emerald)
                .AddIngredient(ItemID.Ruby)
                .AddIngredient(ItemID.Diamond)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}