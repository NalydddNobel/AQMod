using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable
{
    public abstract class FancyBlockItemBase<TTile> : ModItem where TTile : ModTile
    {
        public virtual bool FancyRandomizationTextureFeature => true;

        public byte randomizedTexture;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<TTile>());
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (FancyRandomizationTextureFeature)
                randomizedTexture = (byte)Main.rand.Next(3);
        }

        public override bool CanStackInWorld(Item item2)
        {
            return randomizedTexture == (item2.ModItem as FancyBlockItemBase<TTile>).randomizedTexture;
        }

        // if this has a randomized texture, send 2 bits, otherwise send one
        // randomizedTexture == 0: 0
        // randomizedTexture == 1: 00
        // randomizedTexture == 2: 01
        public override void NetSend(BinaryWriter writer)
        {
            if (randomizedTexture != 0)
            {
                writer.Write(true);
                writer.Write(randomizedTexture == 2);
            }
            else
            {
                writer.Write(false);
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            if (reader.ReadBoolean())
            {
                randomizedTexture = (byte)(reader.ReadBoolean() ? 2 : 1);
            }
            else
            {
                randomizedTexture = 0;
            }
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (randomizedTexture != 0)
            {
                Main.instance.LoadTiles(ModContent.TileType<TTile>());
                var tileTexture = TextureAssets.Tile[ModContent.TileType<TTile>()].Value;
                var frame = new Rectangle(180 + (randomizedTexture - 1) * 18, 54, 16, 16);

                spriteBatch.Draw(tileTexture, ItemDefaults.WorldDrawPos(Item, TextureAssets.Item[Type].Value),
                    frame, lightColor, rotation, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
                return false;
            }
            return true;
        }
    }
}