using Aequus.Content.Biomes.GoreNest.Tiles;
using Aequus.Content.CrossMod.ModCalls;
using Aequus.UI.EventProgressBars;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Events.DemonSiege {
    public class DemonSiegeSystem : ModSystem {
        public static readonly Color TextColor = new(255, 210, 25, 255);

        public static readonly Dictionary<Point, DemonSiegeSacrifice> ActiveSacrifices = new();
        public static readonly List<Point> SacrificeRemovalQueue = new();
        public static readonly Dictionary<int, SacrificeData> RegisteredSacrifices = new();
        public static readonly Dictionary<int, int> SacrificeResultItemIDToOriginalItemID = new();

        public static int DemonSiegePause;

        public override void Load() {

            if (!Main.dedServ) {
                LegacyEventProgressBarLoader.AddBar(new DemonSiegeProgressBar() {
                    EventKey = $"Mods.Aequus.Biomes.{nameof(DemonSiegeBiome)}.DisplayName",
                    Icon = AequusTextures.DemonSiege_EventIcons.Path,
                    backgroundColor = new Color(180, 100, 20, 128),
                });
            }
        }

        public override void AddRecipes() {
            foreach (var s in RegisteredSacrifices.Values) {
                if (s.Hide || s.OriginalItem == s.NewItem)
                    continue;

                Recipe.Create(s.NewItem)
                    .AddIngredient(s.OriginalItem)
                    .AddTile<GoreNestDummy>()
                    .TryRegisterAfter(s.OriginalItem);
            }
        }

        public override void Unload() {
            RegisteredSacrifices.Clear();
            SacrificeResultItemIDToOriginalItemID.Clear();
            ActiveSacrifices.Clear();
            SacrificeRemovalQueue.Clear();
        }

        public override void ClearWorld() {
            SacrificeRemovalQueue.Clear();
            ActiveSacrifices.Clear();
        }

        public override void PostUpdateInput() {
            if (Main.netMode == NetmodeID.Server) {
                return;
            }

            var screenCenter = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) / 2f;
            float screenSize = MathF.Sqrt(Main.screenWidth * Main.screenWidth + Main.screenHeight + Main.screenHeight);
            foreach (var sacrifice in ActiveSacrifices) {
                var v = sacrifice.Value;
                var center = v.WorldCenter;
                v._visible = Vector2.Distance(v.WorldCenter, screenCenter) < screenSize + v.Range * 2f;
            }
        }

        public override void PostUpdateNPCs() {
            if (DemonSiegePause > 0)
                DemonSiegePause--;
            foreach (var s in ActiveSacrifices) {
                s.Value.TileX = s.Key.X;
                s.Value.TileY = s.Key.Y;
                s.Value.Update();
            }
            foreach (var p in SacrificeRemovalQueue) {
                ActiveSacrifices.Remove(p);
                if (Main.netMode != NetmodeID.SinglePlayer) {
                    PacketSystem.Send((packet) => {
                        packet.Write((ushort)p.X);
                        packet.Write((ushort)p.Y);
                    }, PacketType.RemoveDemonSiege);
                }
            }
            SacrificeRemovalQueue.Clear();
        }

        private void DrawSacrificeRings() {
            if (ActiveSacrifices.Count <= 0) {
                return;
            }

            var auraTexture = AequusTextures.GoreNestAura.Value;
            var screenCenter = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) / 2f;
            float screenSize = MathF.Sqrt(Main.screenWidth * Main.screenWidth + Main.screenHeight + Main.screenHeight);
            Main.spriteBatch.Begin_World(shader: false);
            try {
                foreach (var sacrifice in ActiveSacrifices) {
                    var v = sacrifice.Value;
                    var center = v.WorldCenter;
                    if (!v.Renderable) {
                        continue;
                    }
                    var origin = auraTexture.Size() / 2f;
                    var drawCoords = (center - Main.screenPosition).Floor();
                    float scale = v.Range * 2f / auraTexture.Width;
                    float opacity = 1f;

                    if (v.TimeLeft < 360) {
                        opacity = v.TimeLeft / 360f;
                    }

                    float auraScale = v.AuraScale;
                    var color = Color.Lerp(Color.Red * 0.75f, Color.OrangeRed, Helper.Wave(Main.GlobalTimeWrappedHourly * 2.5f, 0.1f, 1f)) * opacity;
                    Main.spriteBatch.Draw(auraTexture, drawCoords, null, color,
                        0f, origin, scale * auraScale, SpriteEffects.None, 0f);
                }
            }
            catch {

            }
            Main.spriteBatch.End();
        }
        public override void PostDrawTiles() {
            DrawSacrificeRings();
        }

        public bool DrawDemonSiegeRanges(Texture2D auraTexture) {
            return true;
        }

        public static void RegisterSacrifice(SacrificeData sacrifice) {
            RegisteredSacrifices[sacrifice.OriginalItem] = sacrifice;
            SacrificeResultItemIDToOriginalItemID.Add(sacrifice.NewItem, sacrifice.OriginalItem);
        }

        public static bool NewInvasion(int x, int y, Item sacrifice, int player = byte.MaxValue, bool checkIsValidSacrifice = true, bool allowAdding = true, bool allowAdding_IgnoreMax = false) {
            sacrifice = sacrifice.Clone();
            sacrifice.stack = 1;
            if (ActiveSacrifices.TryGetValue(new Point(x, y), out var value)) {
                if (allowAdding) {
                    if (allowAdding_IgnoreMax || value.MaxItems < value.Items.Count) {
                        value.Items.Add(sacrifice);
                        return true;
                    }
                }
                return false;
            }
            if (!RegisteredSacrifices.TryGetValue(sacrifice.netID, out var sacrificeData)) {
                if (checkIsValidSacrifice) {
                    return false;
                }
                sacrificeData = new SacrificeData(sacrifice.netID, sacrifice.netID + 1, UpgradeProgressionType.PreHardmode);
            }
            var s = new DemonSiegeSacrifice(x, y) {
                player = (byte)player
            };
            s.Items.Add(sacrifice);
            if (Main.netMode != NetmodeID.SinglePlayer) {
                PacketSystem.Send((p) => {
                    p.Write((ushort)x);
                    p.Write((ushort)y);
                    p.Write((byte)player);
                    ItemIO.Send(sacrifice, p, writeStack: true, writeFavorite: false);
                }, PacketType.StartDemonSiege);
            }
            if (player != 255) {
                s.OnPlayerActivate(Main.player[player]);
            }
            ActiveSacrifices.Add(new Point(x, y), s);
            return true;
        }

        public static void ReceiveStartRequest(BinaryReader reader) {
            int x = reader.ReadUInt16();
            int y = reader.ReadUInt16();
            byte player = reader.ReadByte();
            var s = new DemonSiegeSacrifice(x, y) {
                player = player,
            };
            var sacrifice = new Item();
            ItemIO.Receive(sacrifice, reader, readStack: true, readFavorite: false);
            s.Items.Add(sacrifice);
            ActiveSacrifices.Add(new Point(x, y), s);

            if (Main.netMode == NetmodeID.Server) {
                PacketSystem.Send((p) => {
                    p.Write((ushort)x);
                    p.Write((ushort)y);
                    p.Write(player);
                    ItemIO.Send(sacrifice, p, writeStack: true, writeFavorite: false);
                }, PacketType.StartDemonSiege, ignore: player);
            }
        }

        /// <summary>
        /// Finds and returns the closest demon siege
        /// </summary>
        /// <returns></returns>
        public static Point FindDemonSiege(Vector2 location) {
            foreach (var s in ActiveSacrifices) {
                if (Vector2.Distance(location, new Vector2(s.Value.TileX * 16f + 24f, s.Value.TileY * 16f)) < s.Value.Range) {
                    return s.Key;
                }
            }
            return Point.Zero;
        }

        public static object CallAddDemonSiegeData(Mod callingMod, object[] args) {
            if (Helper.UnboxInt.TryUnbox(args[2], out int baseItem) && Helper.UnboxInt.TryUnbox(args[3], out int newItem) && Helper.UnboxInt.TryUnbox(args[4], out int progression)) {
                var s = new SacrificeData(baseItem, newItem, (UpgradeProgressionType)(byte)progression);
                RegisterSacrifice(s);
                return ModCallManager.Success;
            }
            return ModCallManager.Failure;
        }
        public static object CallHideDemonSiegeData(Mod callingMod, object[] args) {
            if (Helper.UnboxInt.TryUnbox(args[2], out int baseItem)) {
                if (RegisteredSacrifices.ContainsKey(baseItem)) {
                    var val = RegisteredSacrifices[baseItem];
                    val.Hide = Helper.UnboxBoolean.Unbox(args[3]);
                    RegisteredSacrifices[baseItem] = val;
                    return ModCallManager.Success;
                }
            }
            return ModCallManager.Failure;
        }
    }
}