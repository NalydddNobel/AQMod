using Aequus.Content.CrossMod;
using Aequus.Content.ItemPrefixes.Potions;
using Aequus.Tiles.Ambience;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Biomes.GoreNest.Tiles
{
    public class ManaclePollen : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.PixieDust);
            Item.rare = ItemRarityID.Green;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 200);
        }

        public override void AddRecipes()
        {
            var prefix = PrefixLoader.GetPrefix(ModContent.PrefixType<BoundedPrefix>());
            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                if (prefix.CanRoll(ContentSamples.ItemsByType[i]))
                {
                    var r = Recipe.Create(i, 1)
                        .AddIngredient(i)
                        .AddIngredient(Type)
                        .TryRegisterAfter(i);
                    r.createItem.Prefix(prefix.Type);
                    MagicStorage.AddBlacklistedItemData(i, prefix.Type);
                }
            }
        }
    }

    public class ManacleSeeds : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ManacleTile>());
            Item.value = Item.sellPrice(silver: 2);
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Blue;
        }
    }

    public class ManacleTile : HerbTileBase
    {
        protected override int[] GrowableTiles => new int[]
        {
            TileID.Ash,
            TileID.Hellstone,
            TileID.Obsidian,
            TileID.ObsidianBrick,
            TileID.HellstoneBrick,
        };

        protected override Color MapColor => new Color(75, 2, 17, 255);
        public override Vector3 GlowColor => new Vector3(0.66f, 0.15f, 0.1f);
        protected override int DrawOffsetY => -10;

        public override bool IsBlooming(int i, int j)
        {
            return Main.dayTime && Main.time < 17100;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            var clr = GlowColor;
            float multiplier = Math.Max(Main.tile[i, j].TileFrameX / 56, 0.1f);
            r = clr.X * multiplier;
            g = clr.Y * multiplier;
            b = clr.Z * multiplier;
        }

        public override bool Drop(int i, int j)
        {
            bool regrowth = Main.player[Player.FindClosest(new Vector2(i * 16f, j * 16f), 16, 16)].HeldItemFixed().type == ItemID.StaffofRegrowth;
            if (Main.tile[i, j].TileFrameX >= FrameShiftX)
            {
                Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 16, 16), ModContent.ItemType<ManaclePollen>(), regrowth ? Main.rand.Next(1, 3) : 1);
            }
            if (CanBeHarvestedWithStaffOfRegrowth(i, j))
            {
                Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 16, 16), ModContent.ItemType<ManacleSeeds>(), regrowth ? Main.rand.Next(1, 6) : Main.rand.Next(1, 4));
            }
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 6;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            var texture = TextureAssets.Tile[Type].Value;
            var effects = SpriteEffects.None;
            SetSpriteEffects(i, j, ref effects);
            var frame = new Rectangle(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY, FrameWidth, FrameHeight);
            var offset = (AequusHelpers.TileDrawOffset - Main.screenPosition).Floor();
            var groundPosition = new Vector2(i * 16f + 8f, j * 16f + 16f).Floor();
            spriteBatch.Draw(texture, groundPosition + offset, frame, Lighting.GetColor(i, j), 0f, new Vector2(FrameWidth / 2f, FrameHeight - 2f), 1f, effects, 0f);
            return false;
        }
    }
}