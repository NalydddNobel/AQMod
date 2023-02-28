using Aequus.Content.Elites.Items;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Content.Elites
{
    public class EliteBuffPlants : ModTile 
    {
        public const int Argon = 0;
        public const int Krypton = 1;
        public const int Xenon = 2;
        public const int Neon = 3;

        public const int FullFrameWidth = 48;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = new[] { 22, 26, };
            TileObjectData.newTile.CoordinateWidth = FullFrameWidth / 2;
            TileObjectData.newTile.CoordinatePadding = 0;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(208, 0, 126), CreateMapEntryName("ArgonEvilPlant"));
            AddMapEntry(new Color(144, 254, 2), CreateMapEntryName("KryptonEvilPlant"));
            AddMapEntry(new Color(0, 197, 208), CreateMapEntryName("XenonEvilPlant"));
            AddMapEntry(new Color(208, 0, 160), CreateMapEntryName("NeonEvilPlant"));
            HitSound = SoundID.Item10.WithPitchOffset(0.9f);
        }

        public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].TileFrameX / FullFrameWidth);

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            switch (Main.tile[i, j].TileFrameX / FullFrameWidth)
            {
                default:
                    {
                        r = 1.05f;
                        g = 0f;
                        b = 0.62f;
                    }
                    break;

                case Krypton:
                    {
                        r = 0.72f;
                        g = 1.4f;
                        b = 0f;
                    }
                    break;

                case Xenon:
                    {
                        r = 0f;
                        g = 1f;
                        b = 1.05f;
                    }
                    break;

                case Neon:
                    {
                        r = 0.6f;
                        g = 0f;
                        b = 1.05f;
                    }
                    break;
            }
        }

        public override void RandomUpdate(int i, int j)
        {
            int reps = 20;
            int maxDist = 30;
            int frame = Main.tile[i, j].TileFrameX / FullFrameWidth;
            for (int o = 0; o < reps; o++)
            {
            Reset:
                int x = i + WorldGen.genRand.Next(-maxDist, maxDist);
                int y = j + WorldGen.genRand.Next(-maxDist, maxDist);
                var w = new Vector2(x * 16f, y * 16f);
                var m = new Vector2(i * 16f, j * 16f);
                if (!WorldGen.InWorld(x, y, 10) || !Main.tile[x, y].HasTile)
                {
                    continue;
                }
                int moss = TileID.ArgonMoss;
                switch (frame)
                {
                    case Krypton:
                        moss = TileID.KryptonMoss;
                        break;
                    case Xenon:
                        moss = TileID.XenonMoss;
                        break;
                }

                if (Main.tile[x, y].TileType == moss && reps < 40)
                {
                    reps += 4;
                    maxDist = 10;
                    i = x;
                    j = y;
                    goto Reset;
                }

                if (!Collision.CanHitLine(w + Vector2.Normalize(m - w) * 20f, 16, 16, m + Vector2.Normalize(w - m) * 20f, 16, 16))
                {
                    continue;
                }

                if (Main.tile[x, y].TileType == TileID.Stone || Main.tile[x, y].TileType == TileID.ArgonMoss || Main.tile[x, y].TileType == TileID.KryptonMoss || Main.tile[x, y].TileType == TileID.XenonMoss)
                {
                    if (AequusTile.GrowGrass(x, y, moss))
                    {
                        WorldGen.SquareTileFrame(x, y, resetFrame: true);
                        if (Main.netMode != NetmodeID.SinglePlayer)
                            NetMessage.SendTileSquare(-1, x, y);
                        reps += 4;
                        maxDist = 10;
                        i = x;
                        j = y;
                        goto Reset;
                    }
                }
                else if (Main.tile[x, y].TileType == TileID.GrayBrick)
                {
                    int brickMoss = TileID.ArgonMossBrick;
                    switch (frame)
                    {
                        case Krypton:
                            brickMoss = TileID.KryptonMossBrick;
                            break;
                        case Xenon:
                            brickMoss = TileID.XenonMossBrick;
                            break;
                        case Neon:
                            brickMoss = TileID.XenonMossBrick;
                            break;
                    }
                    if (AequusTile.GrowGrass(x, y, brickMoss))
                    {
                        WorldGen.SquareTileFrame(x, y, resetFrame: true);
                        if (Main.netMode != NetmodeID.SinglePlayer)
                            NetMessage.SendTileSquare(-1, x, y);
                        reps += 4;
                        maxDist = 10;
                        i = x;
                        j = y;
                        goto Reset;
                    }
                }
            }
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            switch (Main.tile[i, j].TileFrameX / FullFrameWidth)
            {
                default:
                    type = DustID.ArgonMoss;
                    break;
                case Krypton:
                    type = DustID.KryptonMoss;
                    break;
                case Xenon:
                    type = DustID.XenonMoss;
                    break;
                case Neon:
                    type = DustID.XenonMoss;
                    break;
            }
            return true;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            var source = new EntitySource_TileBreak(i, j);
            switch (frameX / FullFrameWidth)
            {
                default:
                    {
                        Item.NewItem(source, i * 16, j * 16, 32, 32, ModContent.ItemType<ElitePlantArgon>());
                    }
                    break;

                case Krypton:
                    {
                        Item.NewItem(source, i * 16, j * 16, 32, 32, ModContent.ItemType<ElitePlantKrypton>());
                    }
                    break;

                case Xenon:
                    {
                        Item.NewItem(source, i * 16, j * 16, 32, 32, ModContent.ItemType<ElitePlantXenon>());
                    }
                    break;

                case Neon:
                    {
                        Item.NewItem(source, i * 16, j * 16, 32, 32, ModContent.ItemType<ElitePlantXenon>());
                    }
                    break;
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Main.tile[i, j].TileFrameX % FullFrameWidth < FullFrameWidth / 2 - 2 || Main.tile[i, j].TileFrameY <= 0)
            {
                return false;
            }

            var frame = new Rectangle(Main.tile[i, j].TileFrameX - FullFrameWidth / 2, FullFrameWidth, FullFrameWidth, FullFrameWidth);
            var texture = TextureAssets.Tile[Type].Value;
            spriteBatch.Draw(
                texture,
                new Vector2(i * 16f, j * 16f - 4f) - Main.screenPosition + Helper.TileDrawOffset,
                frame,
                Helper.GetLightingSection(i - 1, j - 1, 2, 2),
                0f,
                frame.Size() / 2f,
                1f, SpriteEffects.None, 0f
            );

            return false;
        }
    }

    public class EliteBuffPlantsHostile : EliteBuffPlants
    {
        public override string Texture => Helper.GetPath<EliteBuffPlants>();

        public void CheckEnemies(int i, int j)
        {
            var pos = new Vector2(i * 16f, j * 16f);
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                if (Main.npc[k].active && Main.npc[k].Distance(pos) < 200f)
                {
                    Main.npc[k].Aequus().SetPrefix(Main.npc[k], Main.tile[i, j].TileFrameX / FullFrameWidth, true);
                }
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            CheckEnemies(i, j);
        }
    }
}