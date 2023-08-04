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

namespace Aequus.Tiles.Misc.BigGems {
    public class BigGemsTile : ModTile {
        public const int Amethyst = 0;
        public const int Topaz = 1;
        public const int Sapphire = 2;
        public const int Emerald = 3;
        public const int Ruby = 4;
        public const int Diamond = 5;
        private const int MaxGemIDs = 6;

        private static Color[] GemColor;

        public override void SetStaticDefaults() {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 20, };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.addTile(Type);
            GemColor = new[] {
                Color.Purple,
                Color.Yellow,
                Color.Blue,
                Color.Green,
                Color.Red,
                Color.White,
            };
            foreach (var style in this.GetConstants()) {
                AddMapEntry(GemColor[(int)style.GetValue(this)], this.GetLocalization($"MapEntry_{style.Name}"));
            }
            HitSound = SoundID.Shatter;
        }

        public override ushort GetMapOption(int i, int j) => (ushort)Math.Clamp(Main.tile[i, j].TileFrameX / 36, 0, 5);

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
            r = 0.05f;
            g = 0.05f;
            b = 0.05f;
        }

        public override void RandomUpdate(int i, int j) {
        }

        public override void NumDust(int i, int j, bool fail, ref int num) {
            num = fail ? 1 : 3;
        }

        private void ShatterGore<T>(int i, int j) where T : ModGore {
            Gore.NewGore(new EntitySource_TileBreak(i, j), new Vector2(i * 16f + Main.rand.Next(16) - 8f, j * 16f + Main.rand.Next(10)), Main.rand.NextVector2Circular(2f, 2f), ModContent.GoreType<T>());
        }

        public override bool CreateDust(int i, int j, ref int type) {
            if (Main.netMode == NetmodeID.Server)
                return true;

            switch (Main.tile[i, j].TileFrameX / 36) {
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

        public override void KillMultiTile(int i, int j, int frameX, int frameY) {
            var source = new EntitySource_TileBreak(i, j);
            int drop = (frameX / 36) switch {
                Diamond => ItemID.Diamond,
                Ruby => ItemID.Ruby,
                Emerald => ItemID.Emerald,
                Sapphire => ItemID.Sapphire,
                Topaz => ItemID.Topaz,
                _ => ItemID.Amethyst,
            };
            Item.NewItem(source, i * 16, j * 16, 32, 32, drop, Main.rand.Next(6, 11));
        }

        private bool ShouldDrawSnowTile(int left, int bottom) {
            return TileID.Sets.IcesSnow[Main.tile[left, bottom + 1].TileType] && TileID.Sets.IcesSnow[Main.tile[left + 1, bottom + 1].TileType];
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
            var tile = Main.tile[i, j];

            int left = i - tile.TileFrameX % 36 / 18;
            int top = j - tile.TileFrameY / 18;
            bool drawSnow = ShouldDrawSnowTile(left, top + 1);

            FastRandom rand = new(left + top * left);
            rand.NextSeed();
            var texture = TextureAssets.Tile[Type].Value;
            var baseDrawCoords = new Vector2(i * 16f, j * 16f + 12f) - Main.screenPosition + Helper.TileDrawOffset;
            var drawCoords = new Vector2(baseDrawCoords.X, baseDrawCoords.Y - (drawSnow ? 14 : 12) - rand.Next(4) * 2f);
            var frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 18, tile.TileFrameY > 0 ? 22 : 18);

            var lighting = Lighting.GetColor(i, j);
            float intensity = Math.Clamp((lighting.R + lighting.G + lighting.B) / 255f, 0.1f, 1f);
            intensity *= MathF.Pow(Helper.Wave((Main.GlobalTimeWrappedHourly / 60f + rand.NextFloat()) * MathHelper.TwoPi, 0f, 1f), 3f);
            //for (int k = 0; k < 4; k++) {
            //    spriteBatch.Draw(
            //        texture,
            //        drawCoords + (k * MathHelper.PiOver2).ToRotationVector2() * 2f,
            //        frame,
            //        Color.White.UseA(0) * intensity * Helper.Wave(Main.GlobalTimeWrappedHourly * 3f + k * MathHelper.PiOver2, 0.2f, 1f));
            //}

            if (tile.TileFrameX % 36 == 0 && tile.TileFrameY == 0) {
                Main.spriteBatch.Draw(AequusTextures.Bloom6, baseDrawCoords + new Vector2(16f, 0f), null, GemColor[tile.TileFrameX / 36] with { A = 0 } * 0.7f * intensity, 0f, AequusTextures.Bloom6.Size() / 2f, 0.5f + intensity * 0.5f, SpriteEffects.None, 0f);
            }

            if (tile.IsTileInvisible) {
                return true;
            }

            spriteBatch.Draw(
                texture,
                drawCoords,
                frame,
                lighting);

            if (drawSnow && tile.TileFrameY > 0) {
                Main.instance.LoadTiles(TileID.SmallPiles);
                texture = TextureAssets.Tile[TileID.SmallPiles].Value;
                frame = new Rectangle(900 + 36 * rand.Next(0, 6), 18, 16, 16);
                lighting = Helper.GetLightingSection(i - 1, j, 2, 1);

                spriteBatch.Draw(
                    texture,
                    new Vector2(baseDrawCoords.X, baseDrawCoords.Y - 10f),
                    frame,
                    lighting);
            }
            return false;
        }

        public static void Generate() {
            var rand = WorldGen.genRand;
            for (int i = 0; i < Main.maxTilesX * Main.maxTilesY / 80; i++) {
                int x = rand.Next(100, Main.maxTilesX - 100);
                int y = rand.Next((int)Main.worldSurface, Main.maxTilesY - 100);
                if (Main.tile[x, y].HasTile && Main.tile[x + 1, y].IsFullySolid() &&
                    TileID.Sets.IcesSnow[Main.tile[x, y].TileType]) {
                    WorldGen.PlaceTile(x, y - 1, ModContent.TileType<BigGemsTile>(), mute: true, style: rand.Next(Diamond + 1));
                }
            }
        }
    }
}