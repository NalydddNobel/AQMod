using Aequus.Assets.Gores.Gems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Utilities;

namespace Aequus.Items.Placeable.Nature.BigGems
{
    public abstract class GemDeposit : ModItem
    {
        public const int AmtForRecipe = 10;
        public abstract int Style { get; }
        public abstract int BaseGem { get; }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(BaseGem);
            Item.createTile = ModContent.TileType<BigGemsTile>();
            Item.placeStyle = Style;
            Item.alpha = 0;
            Item.value *= 10;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(BaseGem, AmtForRecipe)
                .AddTile(TileID.Solidifier)
                .Register();
        }
    }

    public class AmethystDeposit : GemDeposit
    {
        public override int Style => BigGemsTile.Amethyst;
        public override int BaseGem => ItemID.Amethyst;
    }

    public class TopazDeposit : GemDeposit
    {
        public override int Style => BigGemsTile.Topaz;
        public override int BaseGem => ItemID.Topaz;
    }

    public class SapphireDeposit : GemDeposit
    {
        public override int Style => BigGemsTile.Sapphire;
        public override int BaseGem => ItemID.Sapphire;
    }

    public class EmeraldDeposit : GemDeposit
    {
        public override int Style => BigGemsTile.Emerald;
        public override int BaseGem => ItemID.Emerald;
    }

    public class RubyDeposit : GemDeposit
    {
        public override int Style => BigGemsTile.Ruby;
        public override int BaseGem => ItemID.Ruby;
    }

    public class DiamondDeposit : GemDeposit
    {
        public override int Style => BigGemsTile.Diamond;
        public override int BaseGem => ItemID.Diamond;
    }

    public class BigGemsTile : ModTile
    {
        public const int Amethyst = 0;
        public const int Topaz = 1;
        public const int Sapphire = 2;
        public const int Emerald = 3;
        public const int Ruby = 4;
        public const int Diamond = 5;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 20, };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.addTile(Type);
            AddMapEntry(Color.Purple, CreateMapEntryName("BigAmethyst"));
            AddMapEntry(Color.Yellow, CreateMapEntryName("BigTopaz"));
            AddMapEntry(Color.Blue, CreateMapEntryName("BigSapphire"));
            AddMapEntry(Color.Green, CreateMapEntryName("BigEmerald"));
            AddMapEntry(Color.Red, CreateMapEntryName("BigRuby"));
            AddMapEntry(Color.White, CreateMapEntryName("BigDiamond"));
            HitSound = SoundID.Shatter;
        }

        public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].TileFrameX / 36);

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.05f;
            g = 0.05f;
            b = 0.05f;
        }

        public override void RandomUpdate(int i, int j)
        {
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        private void ShatterGore<T>(int i, int j) where T : ModGore
        {
            Gore.NewGore(new EntitySource_TileBreak(i, j), new Vector2(i * 16f + Main.rand.Next(16) - 8f, j * 16f + Main.rand.Next(10)), Main.rand.NextVector2Circular(2f, 2f), ModContent.GoreType<T>());
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            if (Main.netMode == NetmodeID.Server)
                return true;

            switch (Main.tile[i, j].TileFrameX / 36)
            {
                default:
                    ShatterGore<AmethystGore>(i, j);
                    type = DustID.GemAmethyst;
                    break;
                case Topaz:
                    ShatterGore<TopazGore>(i, j);
                    type = DustID.GemTopaz;
                    break;
                case Sapphire:
                    ShatterGore<SapphireGore>(i, j);
                    type = DustID.GemSapphire;
                    break;
                case Emerald:
                    ShatterGore<EmeraldGore>(i, j);
                    type = DustID.GemEmerald;
                    break;
                case Ruby:
                    ShatterGore<RubyGore>(i, j);
                    type = DustID.GemRuby;
                    break;
                case Diamond:
                    ShatterGore<DiamondGore>(i, j);
                    type = DustID.GemDiamond;
                    break;
            }
            return true;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            var source = new EntitySource_TileBreak(i, j);
            switch (frameX / 36)
            {
                default:
                    {
                        Item.NewItem(source, i * 16, j * 16, 32, 32, ItemID.Amethyst, Main.rand.Next(6, 11));
                    }
                    break;

                case Topaz:
                    {
                        Item.NewItem(source, i * 16, j * 16, 32, 32, ItemID.Topaz, Main.rand.Next(6, 11));
                    }
                    break;

                case Sapphire:
                    {
                        Item.NewItem(source, i * 16, j * 16, 32, 32, ItemID.Sapphire, Main.rand.Next(6, 11));
                    }
                    break;

                case Emerald:
                    {
                        Item.NewItem(source, i * 16, j * 16, 32, 32, ItemID.Emerald, Main.rand.Next(6, 11));
                    }
                    break;

                case Ruby:
                    {
                        Item.NewItem(source, i * 16, j * 16, 32, 32, ItemID.Ruby, Main.rand.Next(6, 11));
                    }
                    break;

                case Diamond:
                    {
                        Item.NewItem(source, i * 16, j * 16, 32, 32, ItemID.Diamond, Main.rand.Next(6, 11));
                    }
                    break;
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            var tile = Main.tile[i, j];
            if (tile.TileFrameX % 36 > 0 && tile.TileFrameY > 0)
            {
                bool drawSnow = true;
                for (int k = i - 1; k < i + 1; k++)
                {
                    if (!TileID.Sets.IcesSnow[Main.tile[k, j + 1].TileType])
                    {
                        drawSnow = false;
                        break;
                    }
                }

                var rand = new FastRandom(i + j * i);
                rand.NextSeed();
                var texture = TextureAssets.Tile[Type].Value;
                var baseDrawCoords = new Vector2(i * 16f - 16f, j * 16f) - Main.screenPosition + AequusHelpers.TileDrawOffset;
                var drawCoords = new Vector2(baseDrawCoords.X, baseDrawCoords.Y - (drawSnow ? 18 : 12) - rand.Next(4) * 2f);
                var frame = new Rectangle(tile.TileFrameX - 18, tile.TileFrameY + 26, 36, 40);

                var lighting = AequusHelpers.GetLightingSection(i - 1, j - 1, 2, 2);
                float intensity = Math.Clamp((lighting.R + lighting.G + lighting.B) / 255f, 0.2f, 1f);
                var circular = AequusHelpers.CircularVector(4);
                for (int k = 0; k < circular.Length; k++)
                {
                    spriteBatch.Draw(
                        texture,
                        drawCoords + circular[k] * 2f,
                        frame,
                        Color.White.UseA(0) * intensity * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 3f + k * MathHelper.PiOver2, 0.2f, 1f));
                }

                spriteBatch.Draw(
                    texture,
                    drawCoords,
                    frame,
                    lighting);

                if (drawSnow)
                {
                    Main.instance.LoadTiles(TileID.SmallPiles);
                    texture = TextureAssets.Tile[TileID.SmallPiles].Value;
                    frame = new Rectangle(900 + 36 * rand.Next(0, 6), 18, 16, 16);
                    lighting = AequusHelpers.GetLightingSection(i - 1, j, 2, 1);

                    spriteBatch.Draw(
                        texture,
                        new Vector2(baseDrawCoords.X, baseDrawCoords.Y + 4f),
                        frame,
                        lighting);
                    spriteBatch.Draw(
                        texture,
                        new Vector2(baseDrawCoords.X + 16f, baseDrawCoords.Y + 4f),
                        new Rectangle(frame.X + 18, frame.Y, frame.Width, frame.Height),
                        lighting);
                }
                return false;
            }
            return false;
        }

        public static void Generate()
        {
            var rand = WorldGen.genRand;
            for (int i = 0; i < (Main.maxTilesX * Main.maxTilesY) / 20; i++)
            {
                int x = rand.Next(100, Main.maxTilesX - 100);
                int y = rand.Next((int)Main.worldSurface, Main.maxTilesY - 100);
                if (Main.tile[x, y].HasTile && Main.tile[x + 1, y].IsFullySolid() &&
                    TileID.Sets.IcesSnow[Main.tile[x, y].TileType])
                {
                    WorldGen.PlaceTile(x, y - 1, ModContent.TileType<BigGemsTile>(), mute: true, style: rand.Next(Diamond + 1));
                }
            }
        }
    }
}