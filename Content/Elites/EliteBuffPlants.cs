using Aequus.Common.Net;
using Aequus.Content.Elites.Items;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
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

        public static ushort[] StyleToMossStone = new[] {
            TileID.ArgonMoss,
            TileID.KryptonMoss,
            TileID.XenonMoss,
            TileID.PurpleMoss,
        };
        public static ushort[] StyleToMossBrick = new[] {
            TileID.ArgonMossBrick,
            TileID.KryptonMossBrick,
            TileID.XenonMossBrick,
            TileID.PurpleMossBrick,
        };
        public static short[] StyleToDust = new[] {
            DustID.ArgonMoss,
            DustID.KryptonMoss,
            DustID.XenonMoss,
            DustID.PurpleCrystalShard,
        };
        public static Vector3[] StyleToColor = new[] {
            new Vector3(1.05f, 0f, 0.62f),
            new Vector3(0.72f, 1.4f, 0f),
            new Vector3(0f, 1f, 1.05f),
            new Vector3(0.6f, 0f, 1.05f),
        };

        public static int GetStyle(int i, int j) {
            return Math.Clamp(Main.tile[i, j].TileFrameX / FullFrameSize, 0, 3);
        }

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
            AddMapEntry(new Color(208, 0, 126), Lang.GetItemName(ModContent.ItemType<ElitePlantArgon>()));
            AddMapEntry(new Color(144, 254, 2), Lang.GetItemName(ModContent.ItemType<ElitePlantKrypton>()));
            AddMapEntry(new Color(0, 197, 208), Lang.GetItemName(ModContent.ItemType<ElitePlantXenon>()));
            AddMapEntry(new Color(160, 0, 208), Lang.GetItemName(ModContent.ItemType<ElitePlantNeon>()));
            HitSound = SoundID.Item10.WithPitchOffset(0.9f);
        }

        public override ushort GetMapOption(int i, int j) => (ushort)GetStyle(i, j);

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            var clr = StyleToColor[GetStyle(i, j)];
            r = clr.X;
            g = clr.Y;
            b = clr.Z;
        }

        public override void RandomUpdate(int i, int j)
        {
            int reps = 20;
            int maxDist = 30;
            int frame = Main.tile[i, j].TileFrameX / FullFrameSize;
            int mossTileID = StyleToMossStone[GetStyle(i, j)];
            int mossBrickTileID = StyleToMossBrick[GetStyle(i, j)];
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

                if (Main.tile[x, y].TileType == mossTileID && reps < 40)
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
                    if (AequusTile.GrowGrass(x, y, mossTileID))
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
                    if (AequusTile.GrowGrass(x, y, mossBrickTileID))
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
            type = StyleToDust[GetStyle(i, j)];
            return true;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), 
                i * 16, j * 16, 32, 32, 
                (frameX / FullFrameSize) switch {
                    Neon => ModContent.ItemType<ElitePlantNeon>(),
                    Xenon => ModContent.ItemType<ElitePlantXenon>(),
                    Krypton => ModContent.ItemType<ElitePlantKrypton>(),
                    _ => ModContent.ItemType<ElitePlantArgon>(),
                });
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

    public class EliteBuffPlantsActivatePacket : PacketHandler {
        public override PacketType LegacyPacketType => PacketType.EliteBuffPlantsActivate;

        public void Send(NPC npc, int i, int j) {
            var p = GetPacket();
            p.Write((byte)npc.whoAmI);
            p.Write((ushort)i);
            p.Write((ushort)j);
            p.Send();
        }

        public override void Receive(BinaryReader reader) {
            byte npc = reader.ReadByte();
            ushort i = reader.ReadUInt16();
            ushort j = reader.ReadUInt16();

            if (npc > Main.maxNPCs || !Main.npc[npc].active || !WorldGen.InWorld(i, j)) {
                return;
            }

            EliteBuffPlantsHostile.ActivatePlantEffects(Main.npc[npc], i, j);
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

        public override void Unload() {
            StyleToPrefix = null;
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

            int prefixID = StyleToPrefix[GetStyle(i, j)].Type;

            if (!npc.Aequus().HasPrefix(prefixID) && npc.CanBeChasedBy()) {
                npc.Aequus().SetPrefix(npc, prefixID, true);
                npc.netUpdate = true;
                ActivatePlantEffects(npc, i, j);
            }
        }

        internal static void ActivatePlantEffects(NPC npc, int i, int j) {

            if (Main.netMode == NetmodeID.Server) {
                ModContent.GetInstance<EliteBuffPlantsActivatePacket>().Send(npc, i, j);
                return;
            }

            Color dustColor = GetStyle(i, j) switch {
                Krypton => new(200, 255, 50, 50),
                Xenon => new(50, 200, 255, 50),
                Neon => new(200, 50, 255, 50),
                _ => new(255, 50, 200, 50),
            };

            SoundEngine.PlaySound(AequusSounds.jump, new(i * 16f, j * 16f));
            for (int l = 0; l < 20; l++) {
                var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<MonoSparkleDust>(), newColor: dustColor, Scale: Main.rand.NextFloat(0.5f, 1.5f));
                d.fadeIn = d.scale + 0.6f;
                d.noGravity = true;
                d.position.Y -= 20f;
                d.velocity.X *= 0.66f;
                d.velocity.Y += 2f;
            }
            int left = i - Main.tile[i, j].TileFrameX % FullFrameSize / FrameSize;
            int top = j - Main.tile[i, j].TileFrameY % FullFrameSize / FrameSize;
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