using Aequus;
using Aequus.Common.Net;
using Aequus.Common.Tiles.Global;
using Aequus.Content.Elites;
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

namespace Aequus.Tiles.MossCaves.ElitePlants {
    [LegacyName("EliteBuffPlants")]
    public class ElitePlantTile : ModTile {
        public const int Argon = 0;
        public const int Krypton = 1;
        public const int Xenon = 2;
        public const int Neon = 3;

        public const int FrameSize = 24;
        public const int FullFrameSize = FrameSize * 2;

        public static int spawnChance;

        public static ushort[] StyleToMossStone = new[] {
            TileID.ArgonMoss,
            TileID.KryptonMoss,
            TileID.XenonMoss,
            TileID.VioletMoss,
        };
        public static ushort[] StyleToMossBrick = new[] {
            TileID.ArgonMossBrick,
            TileID.KryptonMossBrick,
            TileID.XenonMossBrick,
            TileID.VioletMossBrick,
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

        public override void SetStaticDefaults() {
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

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
            var clr = StyleToColor[GetStyle(i, j)];
            r = clr.X;
            g = clr.Y;
            b = clr.Z;
        }

        public override void RandomUpdate(int i, int j) {
            int reps = 20;
            int maxDist = 30;
            int frame = Main.tile[i, j].TileFrameX / FullFrameSize;
            int mossTileID = StyleToMossStone[GetStyle(i, j)];
            int mossBrickTileID = StyleToMossBrick[GetStyle(i, j)];
            for (int o = 0; o < reps; o++) {
            Reset:
                int x = i + WorldGen.genRand.Next(-maxDist, maxDist);
                int y = j + WorldGen.genRand.Next(-maxDist, maxDist);
                var w = new Vector2(x * 16f, y * 16f);
                var m = new Vector2(i * 16f, j * 16f);
                if (!WorldGen.InWorld(x, y, 10) || !Main.tile[x, y].HasTile) {
                    continue;
                }

                if (Main.tile[x, y].TileType == mossTileID && reps < 40) {
                    reps += 4;
                    maxDist = 10;
                    i = x;
                    j = y;
                    goto Reset;
                }

                if (!Collision.CanHitLine(w + Vector2.Normalize(m - w) * 20f, 16, 16, m + Vector2.Normalize(w - m) * 20f, 16, 16)) {
                    continue;
                }

                if (Main.tile[x, y].TileType == TileID.Stone || Main.tile[x, y].TileType == TileID.ArgonMoss || Main.tile[x, y].TileType == TileID.KryptonMoss || Main.tile[x, y].TileType == TileID.XenonMoss) {
                    if (TileHelper.TryGrowGrass(x, y, mossTileID)) {
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
                else if (Main.tile[x, y].TileType == TileID.GrayBrick) {
                    if (TileHelper.TryGrowGrass(x, y, mossBrickTileID)) {
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

        public override bool CreateDust(int i, int j, ref int type) {
            type = StyleToDust[GetStyle(i, j)];
            return true;
        }

        protected void DropMushroom(int i, int j, int frameX) {
            Item.NewItem(new EntitySource_TileBreak(i, j),
                i * 16, j * 16, 32, 32,
                (frameX / FullFrameSize) switch {
                    Neon => ModContent.ItemType<ElitePlantNeon>(),
                    Xenon => ModContent.ItemType<ElitePlantXenon>(),
                    Krypton => ModContent.ItemType<ElitePlantKrypton>(),
                    _ => ModContent.ItemType<ElitePlantArgon>(),
                });
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY) {
            if (WorldGen.genRand.NextBool(100)) {
                DropMushroom(i, j, frameX); // Drop a dupe
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
            if (Main.tile[i, j].TileFrameX % FullFrameSize < FullFrameSize / 2 - 2 || Main.tile[i, j].TileFrameY <= 0) {
                return false;
            }

            var frame = new Rectangle(Main.tile[i, j].TileFrameX - FullFrameSize / 2, FullFrameSize, FullFrameSize, FullFrameSize);
            var texture = TextureAssets.Tile[Type].Value;
            spriteBatch.Draw(
                texture,
                new Vector2(i * 16f, j * 16f - 4f) - Main.screenPosition + Helper.TileDrawOffset,
                frame,
                Color.White,
                0f,
                frame.Size() / 2f,
                1f, SpriteEffects.None, 0f
            );

            return false;
        }

        public static void GlobalRandomUpdate(in GlobalRandomTileUpdateParams info) {
            if (info.TileTypeCache == TileID.Stone) {
                if (!WorldGen.genRand.NextBool(2)) {
                    return;
                }

                TryGrow(info.X, info.Y, WorldGen.genRand.Next(4));
                return;
            }

            if (info.TileTypeCache == TileID.ArgonMoss || info.TileTypeCache == TileID.ArgonMossBrick) {
                TryGrow(info.X, info.Y, Argon);
            }
            else if (info.TileTypeCache == TileID.KryptonMoss || info.TileTypeCache == TileID.KryptonMossBrick) {
                TryGrow(info.X, info.Y, Krypton);
            }
            else if (info.TileTypeCache == TileID.XenonMoss || info.TileTypeCache == TileID.XenonMossBrick) {
                TryGrow(info.X, info.Y, Krypton);
            }
            else if (info.TileTypeCache == TileID.VioletMoss || info.TileTypeCache == TileID.VioletMossBrick) {
                TryGrow(info.X, info.Y, Krypton);
            }
        }

        protected static bool TryGrow(int i, int j, int style) {
            if (Main.tile[i + 1, j].TileType != Main.tile[i, j].TileType) {
                return false;
            }
            for (int k = 0; k < 2; k++) {
                if (Main.tile[i + k, j].Slope != SlopeType.Solid || Main.tile[i + k, j].IsHalfBlock) {
                    return false;
                }
            }

            j -= 2;
            for (int k = 0; k < 2; k++) {
                for (int l = 0; l < 2; l++) {
                    if (Main.tile[i + k, j + l].HasTile && !Main.tile[i + k, j + l].CuttableType()) {
                        return false;
                    }
                }
            }

            var rect = new Rectangle(i - 20, j - 5, 40, 14).Fluffize(20);

            if (TileHelper.ScanTiles(rect, TileHelper.HasTileAction(ModContent.TileType<EliteBuffPlantsHostile>()), TileHelper.IsTree, TileHelper.HasShimmer)) {
                return false;
            }
            for (int k = 0; k < 2; k++) {
                for (int l = 0; l < 2; l++) {
                    WorldGen.KillTile(i + k, j + l);
                    if (Main.tile[i + k, j + l].HasTile)
                        return false;
                }
            }

            int frame = style;
            for (int k = 0; k < 2; k++) {
                for (int l = 0; l < 2; l++) {
                    WorldGen.KillTile(i + k, j + l);
                    Main.tile[i + k, j + l].Active(value: true);
                    Main.tile[i + k, j + l].TileType = (ushort)ModContent.TileType<EliteBuffPlantsHostile>();
                    Main.tile[i + k, j + l].TileFrameX = (short)((frame * 2 + k) * FrameSize);
                    Main.tile[i + k, j + l].TileFrameY = (short)(l * FrameSize);
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendTileSquare(-1, i - 1, j + l - 1, 3, 3);
                }
            }
            AequusWorld.mushroomFrenzy /= 2;
            return true;
        }
    }

    public class EliteBuffPlantsHostile : ElitePlantTile {
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

                ActivatePlantEffects(Main.npc[npc], i, j);
            }
        }

        public override string Texture => Helper.GetPath<ElitePlantTile>();

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

            if (!npc.Aequus().HasPrefix(prefixID) && !npc.hide && npc.ShowNameOnHover && npc.CanBeChasedBy()) {
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

            ScreenCulling.Prepare(16);
            if (!ScreenCulling.OnScreenWorld(new Vector2(i * 16f + 16f, j * 16f + 16f)) && !ScreenCulling.OnScreenWorld(npc.Center)) {
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

        public override void KillMultiTile(int i, int j, int frameX, int frameY) {
            DropMushroom(i, j, frameX);
        }
    }
}