using Aequus.Common.Players.Stats;
using Aequus.Content.DataSets;
using Aequus.Core.CrossMod;
using System;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod; 

internal class ThoriumMod : ModSupport<ThoriumMod> {
    public static Dictionary<int, RepellantType> SupportsRepellant { get; private set; } = new();

    public override void SafeLoad(Mod mod) {
    }

    public override void SetStaticDefaults() {
        LoadBloodCrownAffixes();
    }

    private void LoadBloodCrownAffixes() {
        StatCompareLoader.Register(new CompareFloat((p) => TryCall(out int value, "GetLifeRecovery", p) ? value : 0f, Affix("LifeRecovery")));
        StatCompareLoader.Register(new CompareFloat((p) => TryCall(out int value, "GetHealerHealBonus", p) ? value : 0f, Affix("HealBonus")));
        StatCompareLoader.Register(new CompareFloat((p) => TryCall(out int value, "GetBardInspirationMax", p) ? value : 0f, Affix("MaxInspiration")));

        static LocalizedText Affix(string key, Func<string> createDefault = null) {
            return StatCompareLoader.Affix("ThoriumMod." + key, createDefault);
        }
    }

    public override void AddRecipes() {
        foreach (var b in SupportsRepellant) {
            var flags = b.Value;
            var npcId = b.Key;
            foreach (var f in flags.GetFlags()) {
                Call($"Add{Enum.GetName(f)}RepellentNPCID", npcId);
            }
        }
        foreach (var b in BuffSets.PlayerDoTDebuff) {
            if (b >= BuffID.Count && BuffLoader.GetBuff(b).Mod.Name == "Aequus") {
                Call("AddPlayerDoTBuffID", b);
            }
        }
        foreach (var b in BuffSets.PlayerStatusDebuff) {
            if (b >= BuffID.Count && BuffLoader.GetBuff(b).Mod.Name == "Aequus") {
                Call("AddPlayerStatusBuffID", b);
            }
        }
    }

    public override void SafeUnload() {
        SupportsRepellant.Clear();
    }

    [Flags]
    public enum RepellantType {
        Bat = 1 << 0,
        Insect = 1 << 1,
        Fish = 1 << 2,
        Skeleton = 1 << 3,
        Zombie = 1 << 4,
    }
}