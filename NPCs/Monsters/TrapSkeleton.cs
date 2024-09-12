using Aequus.Common.NPCs;
using Aequus.Common.NPCs.Base;
using Aequus.Common.Preferences;
using Aequus.Common.Utilities;
using Aequus.Content.Items.Armor.TrapArtist;
using Aequus.Tiles.Banners.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters {
    public class TrapSkeleton : LegacyAIFighter {
        // TODO: Add cross mod support for Trap Actions?
        public class TrapAction {
            public int Type { get; internal set; }
            public readonly string Name;

            public readonly Func<NPC, List<Point>, (Point, Point)> CheckWire;
            public readonly Func<NPC, int, int, bool> Apply;

            public TrapAction(string name, Func<NPC, List<Point>, (Point, Point)> checkWire, Func<NPC, int, int, bool> apply) {
                Name = name;
                CheckWire = checkWire;
                Apply = apply;
            }
        }

        private static List<TrapAction> trapActions;

        public static int TrapActionCount => trapActions.Count;

        public static TrapAction FromID(int id) {
            if (id > 0 && id < TrapActionCount) {
                return trapActions[id];
            }
            return null;
        }

        public static TrapAction FromName(string name) {
            for (int i = 0; i < TrapActionCount; i++) {
                if (trapActions[i].Name == name) {
                    return trapActions[i];
                }
            }
            return null;
        }

        public static void RegisterTrapAction(TrapAction action) {
            action.Type = TrapActionCount;
            trapActions.Add(action);
        }

        #region Trap Checks
        public static  bool GoodSpotForPressurePlate(int x, int y) {
            int badPoints = 0;
            for (int i = x - 1; i <= x + 1; i++) {
                for (int j = y - 2; j <= y; j++) {
                    if (!WorldGen.InWorld(i, j, 40) || Main.tile[i, j].IsFullySolid()) {
                        if (i != x) {
                            badPoints++;
                        }
                        else {
                            return false;
                        }
                    }
                }
            }
            if (badPoints >= 4) {
                return false;
            }

            return Main.tile[x, y + 1].IsFullySolid();
        }

        public static bool HorizontalSight(int targetX, int x, int y) {
            int dir = Math.Sign(targetX - x);
            if (dir == 0)
                return false;

            for (int i = 1; i < 100; i++) {
                int val = x + dir * i;
                if (val == targetX)
                    break;

                if (Main.tile[val, y].IsFullySolid())
                    return false;
            }
            return true;
        }

        private static (Point, Point) DartTrap_CheckWire(NPC trapper, List<Point> wires) {
            var pressurePlateSpot = Point.Zero;
            int pressurePlateRand = 1;
            for (int i = 0; i < wires.Count; i++) {
                if (Main.rand.NextBool(pressurePlateRand)
                    && (Main.tile[wires[i]].HasTile && Main.tile[wires[i]].TileType == TileID.PressurePlates
                    || !Main.tile[wires[i]].HasTile && GoodSpotForPressurePlate(wires[i].X, wires[i].Y))) {
                    pressurePlateRand += 5;
                    pressurePlateSpot = wires[i];
                }
            }
            var dartTrapSpot = Point.Zero;
            int trapRand = 4;
            for (int i = 0; i < wires.Count; i++) {
                if (Main.rand.NextBool(trapRand)) {
                    if ((Main.tile[wires[i].X, wires[i].Y - 1].IsFullySolid() || Main.tile[wires[i].X, wires[i].Y + 1].IsFullySolid())
                        && Main.tile[wires[i].X, wires[i].Y].TileType != TileID.PressurePlates
                        && (Main.tile[wires[i].X, wires[i].Y].TileType == TileID.Traps
                        || wires[i].Y <= pressurePlateSpot.Y && wires[i].Y > pressurePlateSpot.Y - 3 && HorizontalSight(pressurePlateSpot.X, wires[i].X, wires[i].Y))) {
                        dartTrapSpot = wires[i];
                    }
                }
            }

            if (pressurePlateSpot != Point.Zero && dartTrapSpot != Point.Zero) {
                if (Main.tile[pressurePlateSpot].HasTile && Main.tile[pressurePlateSpot].TileType == TileID.PressurePlates
                    && Main.tile[dartTrapSpot].HasTile && Main.tile[dartTrapSpot].TileType == TileID.Traps) {
                    return (Point.Zero, Point.Zero);
                }

                return (pressurePlateSpot, dartTrapSpot);
            }
            return (Point.Zero, Point.Zero);
        }

        private static bool DartTrap_Apply(NPC npc, int x, int y) {
            if (npc.ModNPC is not TrapSkeleton trapper) {
                return false;
            }
            WorldGen.PlaceTile(trapper.targetX, trapper.targetY, TileID.PressurePlates, mute: true, forced: true, style: 3);
            WorldGen.PlaceTile(trapper.trapX, trapper.trapY, TileID.Traps, mute: true, forced: true, style: 0);
            if (Main.tile[trapper.trapX, trapper.trapY].TileType == TileID.Traps) {
                Main.tile[trapper.trapX, trapper.trapY].TileFrameX = (short)(trapper.trapX < trapper.targetX ? 18 : 0);
            }
            if (Main.netMode == NetmodeID.Server) {
                NetMessage.SendTileSquare(-1, trapper.targetX, trapper.targetY);
                NetMessage.SendTileSquare(-1, trapper.trapX, trapper.trapX);
            }
            return Main.tile[trapper.targetX, trapper.targetY].HasTile && Main.tile[trapper.trapX, trapper.trapY].HasTile;
        }

        private static (Point, Point) ZenithTrap_CheckWire(NPC trapper, List<Point> wires) {
            if (!Aequus.ZenithSeed) {
                return (new(0, 0), new(0, 0));
            }

            for (int i = 0; i < wires.Count; i++) {
                if (!Main.rand.NextBool(4)) {
                    continue;
                }
                if (!Main.tile[wires[i]].CuttableOrNoTile() || !Main.tile[wires[i].X, wires[i].Y + 5].CuttableOrNoTile() || !Main.tile[wires[i].X, wires[i].Y + 1].SolidType() || !Main.tile[wires[i].X, wires[i].Y + 1].HasUnactuatedTile || !Main.tile[wires[i].X, wires[i].Y + 6].SolidType() || !Main.tile[wires[i].X, wires[i].Y + 6].HasUnactuatedTile) {
                    continue;
                }

                return (wires[i], new(wires[i].X, wires[i].Y + 5));
            }

            return (new(0, 0), new(0, 0));
        }

        private static bool ZenithTrap_Apply(NPC npc, int x, int y) {
            if (npc.ModNPC is not TrapSkeleton trapper) {
                return false;
            }
            WorldGen.PlaceTile(trapper.targetX, trapper.targetY, TileID.PressurePlates, mute: true, forced: true, style: 3);
            WorldGen.KillTile(trapper.trapX, trapper.trapY);
            WorldGen.PlaceTile(trapper.trapX, trapper.trapY, TileID.Explosives, mute: true, forced: true, style: 0);
            if (Main.netMode == NetmodeID.Server) {
                NetMessage.SendTileSquare(-1, trapper.targetX, trapper.targetY);
                NetMessage.SendTileSquare(-1, trapper.trapX, trapper.trapX);
            }
            return Main.tile[trapper.targetX, trapper.targetY].HasTile && Main.tile[trapper.trapX, trapper.trapY].HasTile;
        }
        #endregion

        public override void Load() {
            trapActions = new List<TrapAction>();
            RegisterTrapAction(new TrapAction("Dart Trap", DartTrap_CheckWire, DartTrap_Apply));
            RegisterTrapAction(new TrapAction("Zenith Trap", ZenithTrap_CheckWire, ZenithTrap_Apply));
        }

        public bool init;
        public int trapActionState;
        public int targetX;
        public int targetY;
        public int trapX;
        public int trapY;
        public int trapSetupTime;
        private bool _usingWrench;

        public override float SpeedCap => base.SpeedCap / (targetX > 0 ? 1f : 2.25f);

        public override void SetStaticDefaults() {
            Main.npcFrameCount[Type] = 25;
        }

        public void Reset() {
            trapActionState = -1;
            targetX = 0;
            targetY = 0;
            trapX = 0;
            trapY = 0;
            _usingWrench = false;
        }

        public override void SetDefaults() {
            NPC.width = 20;
            NPC.height = 38;
            NPC.damage = 10;
            NPC.lifeMax = 90;
            NPC.defense = 20;
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = Item.buyPrice(silver: 10);
            NPC.knockBackResist = 0.15f;
            NPC.rarity = 1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<TrapSkeletonBanner>();
            NPC.npcSlots = 2f;
            Reset();
            init = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
            this.CreateEntry(database, bestiaryEntry)
                .AddMainSpawn(BestiaryBuilder.CavernsBiome)
                .QuickUnlock();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) {
            var loot = this.CreateLoot(npcLoot);
            if (GameplayConfig.Instance.EarlyWiring) {
                loot
                    .AddOptions(chance: 1, ModContent.ItemType<InstancedDartTrapHat>(), ItemID.Wrench)
                    .Add(ItemID.Wire, chance: 1, stack: (30, 80));
            }
            else {
                loot.Add<InstancedDartTrapHat>(chance: 2, stack: 1);
            }
        }

        public void Init() {
            for (int i = 0; i < TrapActionCount; i++) {
                UpdateTrapAction(i);
                if (trapActionState != -1) {
                    break;
                }
            }
        }

        private bool MatchWire(byte wireType, Tile tile) {
            return wireType == 0 ? tile.RedWire : wireType == 1 ? tile.GreenWire : wireType == 2 ? tile.BlueWire : tile.YellowWire;
        }

        private List<Point> CreateWireList(byte wireType, int x, int y) {
            var p = new List<Point>() { new(x, y) };
            var addPoints = new List<Point>();
            var fourWay = new Point[] { new(-1, 0), new(1, 0), new(0, -1), new(0, 1) };
            for (int i = 0; i < 25; i++) {
                for (int j = 0; j < p.Count; j++) {
                    var pos = p[j];
                    for (int k = 0; k < 4; k++) {
                        var t = pos + fourWay[k];
                        if (!p.Contains(t) && !addPoints.Contains(t) && MatchWire(wireType, Main.tile[t])) {
                            addPoints.Add(t);
                        }
                    }
                }
                p.AddRange(addPoints);
            }
            return p;
        }

        public void UpdateTrapAction(int type) {
            var tileCoordinates = NPC.Center.ToTileCoordinates();
            tileCoordinates.fluffize(60);
            var npc = NPC;
            var state = trapActions[type];
            for (int k = 0; k < 50; k++) {
                int randX = tileCoordinates.X + Main.rand.Next(-15, 15);
                int randY = tileCoordinates.Y + Main.rand.Next(-15, 15);
                var tile = Main.tile[randX, randY];
                if (tile.RedWire) {
                    var wires = CreateWireList(0, randX, randY);
                    var p = state.CheckWire(npc, wires);
                    if (p.Item1.X != 0) {
                        trapActionState = type;
                        targetX = p.Item1.X;
                        targetY = p.Item1.Y;
                        trapX = p.Item2.X;
                        trapY = p.Item2.Y;
                        return;
                    }
                    break;
                }
                if (tile.GreenWire) {
                    var wires = CreateWireList(1, randX, randY);
                    var p = state.CheckWire(npc, wires);
                    if (p.Item1.X != 0) {
                        trapActionState = type;
                        targetX = p.Item1.X;
                        targetY = p.Item1.Y;
                        trapX = p.Item2.X;
                        trapY = p.Item2.Y;
                        return;
                    }
                    break;
                }
                if (tile.BlueWire) {
                    var wires = CreateWireList(2, randX, randY);
                    var p = state.CheckWire(npc, wires);
                    if (p.Item1.X != 0) {
                        trapActionState = type;
                        targetX = p.Item1.X;
                        targetY = p.Item1.Y;
                        trapX = p.Item2.X;
                        trapY = p.Item2.Y;
                        return;
                    }
                    break;
                }
                if (tile.YellowWire) {
                    var wires = CreateWireList(3, randX, randY);
                    var p = state.CheckWire(npc, wires);
                    if (p.Item1.X != 0) {
                        trapActionState = type;
                        targetX = p.Item1.X;
                        targetY = p.Item1.Y;
                        trapX = p.Item2.X;
                        trapY = p.Item2.Y;
                        return;
                    }
                    break;
                }
            }
        }

        public override void AI() {
            if (!init) {
                Reset();
                init = true;
                Init();
            }
            if (trapActionState == -1) {
                _usingWrench = false;
                if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(15)) {
                    UpdateTrapAction(Main.rand.Next(TrapActionCount));
                    if (trapActionState != -1) {
                        NPC.netUpdate = true;
                    }
                }
            }
            else {
                if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(500 + trapSetupTime * 2) && Collision.CanHitLine(NPC.position, NPC.width, NPC.height, new Vector2(targetX * 16f, targetY * 16f), 16, 16)) {
                    Reset();
                    NPC.netUpdate = true;
                }
                if (targetX > 0) {
                    if (NPC.Distance(new Vector2(targetX * 16f + 8f, targetY * 16f + 8f)) < 32f) {
                        if (!_usingWrench) {
                            _usingWrench = true;
                            NPC.frameCounter = 0;
                        }
                        trapSetupTime++;
                        if (NPC.velocity.Y == 0f) {
                            NPC.velocity.X *= 0.85f;
                        }
                        if (Main.GameUpdateCount % 7 == 0) {
                            var d = Dust.NewDustDirect(new Vector2(targetX * 16f, targetY * 16f), 16, 16, DustID.Electric);
                            d.noGravity = true;
                            d.fadeIn = d.scale + 0.15f;
                            d.noLightEmittence = true;
                        }
                        if (Main.GameUpdateCount % 7 == 3) {
                            var d = Dust.NewDustDirect(new Vector2(trapX * 16f, trapY * 16f), 16, 16, DustID.Electric);
                            d.noGravity = true;
                            d.fadeIn = d.scale + 0.15f;
                            d.noLightEmittence = true;
                        }
                        if (trapSetupTime % 30 == 0) {
                            SoundEngine.PlaySound(SoundID.NPCHit42.WithVolume(0.33f), new Vector2(targetX * 16f, targetY * 16f));
                        }
                        if (trapSetupTime % 300 == 0) {
                            NPC.netUpdate = true;
                        }

                        NPC.spriteDirection = Math.Sign(targetX * 16f - NPC.position.X);

                        if (trapSetupTime > 600) {
                            if (trapActions[trapActionState].Apply(NPC, trapX, trapY)) {
                                SoundEngine.PlaySound(SoundID.NPCHit34.WithVolume(0.5f), new Vector2(targetX * 16f, targetY * 16f));
                                Reset();
                                trapSetupTime = 0;
                                NPC.netUpdate = true;
                            }
                        }
                        if (trapSetupTime > 660) {
                            trapSetupTime = 0;
                            Reset();
                            NPC.netUpdate = true;
                        }
                        return;
                    }
                    else {
                        _usingWrench = false;
                    }
                }
                else {
                    _usingWrench = false;
                }
            }
            base.AI();
        }

        public override void PostUpdateDirection() {
            if (targetX > 0) {
                NPC.direction = Math.Sign(targetX * 16f - NPC.position.X);
            }
            if (NPC.velocity.Y == 0f) {
                NPC.spriteDirection = NPC.direction;
            }
        }

        public override void FindFrame(int frameHeight) {
            if (_usingWrench) {
                NPC.frameCounter += 0.2;
                if (NPC.frameCounter > 5.0) {
                    NPC.frameCounter = 0.0;
                }
                NPC.frame.Y = (NPC.velocity.Y != 0f ? frameHeight * 20 : frameHeight * 15) + (int)NPC.frameCounter * frameHeight;
            }
            else {
                if (NPC.velocity.Y != 0f) {
                    NPC.frame.Y = 0;
                }
                else {
                    NPC.frameCounter += Math.Abs(NPC.velocity.X);
                    if (NPC.frame.Y < frameHeight) {
                        NPC.frame.Y = frameHeight;
                    }
                    if (NPC.frameCounter > 3.0) {
                        NPC.frameCounter = 0.0;
                        NPC.frame.Y = Math.Max((NPC.frame.Y + frameHeight) % (frameHeight * 15), frameHeight);
                    }
                }
            }
        }

        public override void SendExtraAI(BinaryWriter writer) {
            BitsByte bb = new(trapActionState >= 0, targetX > 0, trapX > 0, trapSetupTime > 0);
            writer.Write(bb);
            if (bb[0]) {
                writer.Write((byte)trapActionState);
            }
            if (bb[1]) {
                writer.Write(targetX);
                writer.Write(targetY);
            }
            if (bb[2]) {
                writer.Write(trapX);
                writer.Write(trapY);
            }
            if (bb[3]) {
                writer.Write(trapSetupTime);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader) {
            BitsByte bb = reader.ReadByte();
            if (bb[0]) {
                trapActionState = reader.ReadByte();
            }
            if (bb[1]) {
                targetX = reader.ReadInt32();
                targetY = reader.ReadInt32();
            }
            if (bb[2]) {
                trapX = reader.ReadInt32();
                trapY = reader.ReadInt32();
            }
            if (bb[3]) {
                trapSetupTime = reader.ReadInt32();
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            //if (targetX > 0)
            //{
            //    spriteBatch.Draw(TextureAssets.NpcHead[0].Value, new Vector2(targetX * 16f + 8f, targetY * 16f + 8f) - screenPos, null, Color.White, 0f, TextureAssets.NpcHead[0].Value.Size() / 2f, 1f, SpriteEffects.None, 0f);
            //    spriteBatch.Draw(TextureAssets.NpcHead[0].Value, new Vector2(trapX * 16f + 8f, trapY * 16f + 8f) - screenPos, null, Color.White, 0f, TextureAssets.NpcHead[0].Value.Size() / 2f, 1f, SpriteEffects.None, 0f);
            //}
            return true;
        }

        public override void HitEffect(NPC.HitInfo hit) {
            if (Main.netMode == NetmodeID.Server) {
                return;
            }

            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Bone, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
            if (NPC.life <= 0) {
                for (int i = 0; i < 10; i++) {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Bone, -NPC.velocity.X, -NPC.velocity.Y);
                }
                NPC.DeathGore(42);
                NPC.DeathGore(43);
                NPC.DeathGore(44);
            }
        }

        public static void CheckSpawn(NPCSpawnInfo spawnInfo, IDictionary<int, float> pool) {
            if (spawnInfo.SpawnTileY > Main.worldSurface + 200 && !spawnInfo.Water) {
                var tile = Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY];
                if (Main.tileDungeon[tile.WallType] || tile.WallType == WallID.LihzahrdBrickUnsafe) {
                    return;
                }
                float chance = 0f;
                for (int i = spawnInfo.SpawnTileX - 10; i <= spawnInfo.SpawnTileX + 10; i++) {
                    for (int j = spawnInfo.SpawnTileY - 10; j <= spawnInfo.SpawnTileY + 10; j++) {
                        if (!Main.rand.NextBool(3)) {
                            if (Main.tile[i, j].RedWire || Main.tile[i, j].BlueWire || Main.tile[i, j].GreenWire || Main.tile[i, j].YellowWire) {
                                chance += 0.018f;
                            }
                        }
                    }
                }
                pool[ModContent.NPCType<TrapSkeleton>()] = Math.Min(chance, 0.1f);
            }
            return;
        }
    }
}