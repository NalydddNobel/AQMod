using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials
{
    public class PearlShardWhite : ModItem
    {
        public const int AmountPerPearl = 5;
        public const int InWorldVerticalFrames = 3;
        public virtual int PearlItem => ItemID.WhitePearl;
        public virtual Texture2D DroppedSprite => AequusTextures.PearlShardWhite_Dropped;

        public override void SetDefaults()
        {
            Item.CloneDefaults(PearlItem);
            Item.width = 6;
            Item.height = 6;
            Item.value /= AmountPerPearl;
            Item.rare = Math.Clamp(Item.rare - 1, 0, ItemRarityID.Count);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
            var texture = DroppedSprite;
            var frame = texture.Frame(verticalFrames: InWorldVerticalFrames, frameY: whoAmI % InWorldVerticalFrames);

            spriteBatch.Draw(
                texture, 
                Item.Bottom + new Vector2(0f, -3f) - Main.screenPosition, 
                frame, 
                lightColor, 
                rotation, 
                frame.Size() / 2f, 
                scale, SpriteEffects.None, 0f
            );
            return false;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed) {
            if (Main.netMode == NetmodeID.Server) {
                return;
            }

            var clr = Helper.GetColor(Item.Center).ToVector3();
            int chance = Math.Clamp((int)(clr.Length() * 25f), 0, 99);
            if (chance < 4) {
                return;
            }

            if (Main.rand.NextBool(100 - chance)) {
                var d = Dust.NewDustDirect(
                    Item.TopLeft,
                    Item.width, Item.height,
                    DustID.SilverFlame, 
                    newColor: Color.White,
                    Scale: Main.rand.NextFloat(0.4f, 0.6f)
                );
                d.velocity = Vector2.Zero;
                d.noGravity = true;
                d.fadeIn = d.scale + Main.rand.NextFloat(0.4f, 1f);
            }
        }

        public override void AddRecipes()
        {
            Recipe.Create(PearlItem)
                .AddIngredient(Type, AmountPerPearl)
                .AddTile(TileID.GlassKiln)
                .Register();
        }
    }

    public class PearlShardBlack : PearlShardWhite
    {
        public override int PearlItem => ItemID.BlackPearl;
        public override Texture2D DroppedSprite => AequusTextures.PearlShardBlack_Dropped;
    }

    public class PearlShardPink : PearlShardWhite
    {
        public override int PearlItem => ItemID.PinkPearl;
        public override Texture2D DroppedSprite => AequusTextures.PearlShardPink_Dropped;
    }
}