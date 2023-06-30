using Aequus.Common.NPCs;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.DataSets {
    public class NPCSets : DataSet {
        public static readonly HashSet<int> IsCorrupt = new();
        public static readonly HashSet<int> IsCrimson = new();
        public static readonly HashSet<int> IsHallow = new();
        public static readonly HashSet<int> FromPillarEvent = new();

        #region Bestiary
        public static List<IBestiaryInfoElement> CorruptionElements = new();
        public static List<IBestiaryInfoElement> CrimsonElements = new();
        public static List<IBestiaryInfoElement> HallowElements = new();
        public static List<IBestiaryInfoElement> PillarElements = new();

        private void LoadBestiaryElementTypes() {
            CorruptionElements.AddRange(new[] {
                BestiaryBuilder.Corruption,
                BestiaryBuilder.UndergroundCorruption,
                BestiaryBuilder.CorruptionDesert,
                BestiaryBuilder.CorruptionUndergroundDesert,
                BestiaryBuilder.CorruptionIce,
            });
            CrimsonElements.AddRange(new[] {
                BestiaryBuilder.Crimson,
                BestiaryBuilder.UndergroundCrimson,
                BestiaryBuilder.CrimsonDesert,
                BestiaryBuilder.CrimsonUndergroundDesert,
                BestiaryBuilder.CrimsonIce,
            });
            HallowElements.AddRange(new[] {
                BestiaryBuilder.Hallow,
                BestiaryBuilder.UndergroundHallow,
                BestiaryBuilder.HallowDesert,
                BestiaryBuilder.HallowUndergroundDesert,
                BestiaryBuilder.HallowIce,
            });
            PillarElements.AddRange(new[] {
                BestiaryBuilder.SolarPillar,
                BestiaryBuilder.VortexPillar,
                BestiaryBuilder.NebulaPillar,
                BestiaryBuilder.StardustPillar,
            });
        }
        private bool CheckBestiary(List<IBestiaryInfoElement> info, List<IBestiaryInfoElement> compareInfo) {
            foreach (var i in info) {
                if (i is SpawnConditionBestiaryInfoElement spawn) {
                    foreach (var j in compareInfo) {
                        if (j is not SpawnConditionBestiaryInfoElement compareSpawn) continue;
                        if (spawn.GetDisplayNameKey() == compareSpawn.GetDisplayNameKey()) {
                            return true;
                        }
                    }
                    continue;
                }

                var type = i.GetType();
                foreach (var compare in compareInfo) {
                    if (type == compare.GetType()) {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region Loading
        public override void OnLoad(Mod mod) {
            LoadBestiaryElementTypes();
        }

        public override void AddRecipes(Aequus aequus) {
            for (int i = NPCID.NegativeIDCount + 1; i < NPCLoader.NPCCount; i++) {
                var bestiaryEntry = Main.BestiaryDB.FindEntryByNPCID(i);
                if (bestiaryEntry == null || bestiaryEntry.Info == null) {
                    continue;
                }

                var info = bestiaryEntry.Info;
                if (CheckBestiary(info, CorruptionElements)) {
                    IsCorrupt.Add(i);
                }
                if (CheckBestiary(info, CrimsonElements)) {
                    IsCrimson.Add(i);
                }
                if (CheckBestiary(info, HallowElements)) {
                    IsHallow.Add(i);
                }
                if (CheckBestiary(info, PillarElements)) {
                    FromPillarEvent.Add(i);
                }
            }
        }
        #endregion

        #region Methods
        public bool IsUnholy(int npcId) {
            return IsCorrupt.Contains(npcId) || IsCrimson.Contains(npcId);
        }
        public bool IsHoly(int npcId) {
            return IsHallow.Contains(npcId);
        }
        #endregion
    }
}