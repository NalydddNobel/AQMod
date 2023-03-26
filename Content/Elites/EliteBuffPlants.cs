using Aequus.Content.Elites.Items;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
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

        public const int FrameSize = 24;
        public const int FullFrameSize = FrameSize * 2;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = new[] { 22, 26, };
            TileObjectData.newTile.CoordinateWidth = FrameSize;
            TileObjectData.newTile.CoordinatePadding = 0;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(208, 0, 126), TextHelper.GetText("MapObject.ArgonEvilPlant"));
            AddMapEntry(new Color(144, 254, 2), TextHelper.GetText("MapObject.KryptonEvilPlant"));
            AddMapEntry(new Color(0, 197, 208), TextHelper.GetText("MapObject.XenonEvilPlant"));
            AddMapEntry(new Color(208, 0, 160), TextHelper.GetText("MapObject.NeonEvilPlant"));
            HitSound = SoundID.Item10.WithPitchOffset(0.9f);
        }

        public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].TileFrameX / FullFrameSize);

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            switch (Main.tile[i, j].TileFrameX / FullFrameSize)
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
            int frame = Main.tile[i, j].TileFrameX / FullFrameSize;
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
            switch (Main.tile[i, j].TileFrameX / FullFrameSize)
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
            switch (frameX / FullFrameSize)
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
            if (Main.tile[i, j].TileFrameX % FullFrameSize < FullFrameSize / 2 - 2 || Main.tile[i, j].TileFrameY <= 0)
            {
                return false;
            }

            var frame = new Rectangle(Main.tile[i, j].TileFrameX - FullFrameSize / 2, FullFrameSize, FullFrameSize, FullFrameSize);
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

        private static ElitePrefix[] StyleToPrefix;

        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            StyleToPrefix = new ElitePrefix[4] {
                 ModContent.GetInstance<ArgonElite>(),
                 ModContent.GetInstance<KryptonElite>(),
                 ModContent.GetInstance<XenonElite>(),
                 ModContent.GetInstance<NeonElite>(),
            };
        }

        public static void CheckElitePlants(NPC npc) {
            var tileCoordinates = npc.Center.ToTileCoordinates() + new Point(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10));
            if (!WorldGen.InWorld(tileCoordinates.X, tileCoordinates.Y, 20) || !Main.tile[tileCoordinates].HasTile || Main.tile[tileCoordinates].TileType < TileID.Count) {
                return;
            }

            ActivatePlant(npc, tileCoordinates.X, tileCoordinates.Y);
        }

        public static void ActivatePlant(NPC npc, int i, int j) {
            if (Main.tile[i, j].TileType != ModContent.TileType<EliteBuffPlantsHostile>()) {
                return;
            }

            int prefixID = Main.tile[i, j].TileFrameX / FullFrameSize;
            if (!StyleToPrefix.IndexInRange(prefixID)) {
                return;
            }
            prefixID = StyleToPrefix[prefixID].Type;

            Color dustColor = (Main.tile[i, j].TileFrameX / FullFrameSize) switch {
                1 => new(200, 255, 50, 50),
                2 => new(50, 200, 255, 50),
                3 => new(200, 50, 255, 50),
                _ => new(255, 50, 200, 50),
            };

            if (!npc.Aequus().HasPrefix(prefixID) && npc.CanBeChasedBy()) {
                npc.Aequus().SetPrefix(npc, prefixID, true);
                npc.netUpdate = true;

                SoundEngine.PlaySound(AequusSounds.jump, new(i * 16f, j * 16f));
                for (int l = 0; l < 20; l++) {
                    var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<MonoSparkleDust>(), newColor: dustColor, Scale: Main.rand.NextFloat(0.5f, 1.5f));
                    d.fadeIn = d.scale + 0.6f;
                    d.noGravity = true;
                    d.position.Y -= 20f;
                    d.velocity.X *= 0.66f;
                    d.velocity.Y += 2f;
                }
                int left = i - Main.tile[i, j].TileFrameX / FrameSize;
                int top = j - Main.tile[i, j].TileFrameY / FrameSize;
                for (int l = 0; l < 10; l++) {
                    var d = Dust.NewDustDirect(new(left * 16f, top * 16f), 32, 32, ModContent.DustType<MonoSparkleDust>(), newColor: dustColor, Scale: Main.rand.NextFloat(0.5f, 1.5f));
                    d.fadeIn = d.scale + 0.6f;
                    d.noGravity = true;
                    d.velocity.X *= 0.5f;
                    d.velocity.Y -= 3f;
                }
            }
        }
    }
}