using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Common.World {
    public class ChestOpenedTracker : ModSystem {
        public static List<Point> UnopenedChests { get; private set; }
        public static int CheckingChest;
        public static int CheckingPlayer;

        public override void Load() {
            UnopenedChests = new List<Point>();
        }

        public override void Unload() {
            UnopenedChests?.Clear();
        }

        public override void SaveWorldData(TagCompound tag) {
            UnopenedChests.Remove(new Point(-1, -1));
            if (UnopenedChests.Count > 1) {
                tag["UnopenedChestsX"] = UnopenedChests.ConvertAll((p) => p.X);
                tag["UnopenedChestsY"] = UnopenedChests.ConvertAll((p) => p.Y);
            }
        }

        public override void LoadWorldData(TagCompound tag) {
            if (tag.TryGet<List<int>>("UnopenedChestsX", out var x) && tag.TryGet<List<int>>("UnopenedChestsY", out var y)) {
                if (x.Count != y.Count)
                    return;

                for (int i = 0; i < x.Count; i++) {
                    UnopenedChests.Add(new Point(x[i], y[i]));
                }
            }
        }

        public override void OnWorldLoad() {
            UnopenedChests?.Clear();
        }

        public override void OnWorldUnload() {
            UnopenedChests?.Clear();
        }

        public static bool IsRealChest(int i) {
            return Main.chest[i] != null && !Main.chest[i].bankChest && Main.chest[i].x > 0 && Main.chest[i].y > 0 && Main.chest[i].item != null;
        }

        private void UpdateCheckChest() {
            CheckingChest %= UnopenedChests.Count;
            int chestID = Chest.FindChest(UnopenedChests[CheckingChest].X, UnopenedChests[CheckingChest].Y);
            if (chestID == -1 || !IsRealChest(chestID)) {
                UnopenedChests.RemoveAt(CheckingChest);
            }
            CheckingChest++;
        }
        private void UpdatePlayerNearChests() {
            if (Main.CurrentFrameFlags.ActivePlayersCount <= 0)
                return;
            CheckingPlayer %= Main.CurrentFrameFlags.ActivePlayersCount;
            for (int i = 0; i < UnopenedChests.Count; i++) {
                if (Main.player[CheckingPlayer].Distance(UnopenedChests[i].ToWorldCoordinates(16f, 16f)) < Chest.chestStackRange) {
                    UnopenedChests.RemoveAt(i);
                    i--;
                }
            }
            CheckingPlayer++;
        }

        public override void PostUpdateEverything() {
            if (UnopenedChests == null) {
                UnopenedChests = new List<Point>();
            }
            if (UnopenedChests.Count == 0) {
                UnopenedChests.Add(new Point(-1, -1));
                for (int i = 0; i < Main.maxChests; i++) {
                    if (IsRealChest(i)) {
                        UnopenedChests.Add(new Point(Main.chest[i].x, Main.chest[i].y));
                    }
                }
            }
            //Main.NewText(UnopenedChests.Count);
            UpdateCheckChest();
            UpdatePlayerNearChests();
        }
    }
}