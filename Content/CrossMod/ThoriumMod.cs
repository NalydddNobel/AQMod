using Aequus.Common.Players.Stats;
using Aequus.Content.DataSets;
using Aequus.Core.CrossMod;
using System;
using System.Collections.Generic;

namespace Aequus.Content.CrossMod;

internal class ThoriumMod : SupportedMod<ThoriumMod> {
    public static Dictionary<int, RepellantType> SupportsRepellant { get; private set; } = new();

    public override void SafeLoad(Mod mod) {
    }

    public override void SetStaticDefaults() {
        LoadBloodCrownAffixes();
    }

    private void LoadBloodCrownAffixes() {
        StatCompareLoader.Register(new CompareFloat((p) => TryCall(out int value, "GetLifeRecovery", p) ? value : 0f, this.GetStatCompareText("LifeRecovery")));
        StatCompareLoader.Register(new CompareFloat((p) => TryCall(out int value, "GetHealerHealBonus", p) ? value : 0f, this.GetStatCompareText("HealBonus")));
        StatCompareLoader.Register(new CompareFloat((p) => TryCall(out int value, "GetBardInspirationMax", p) ? value : 0f, this.GetStatCompareText("MaxInspiration")));
    }

    public override void AddRecipes() {
        foreach (var b in SupportsRepellant) {
            var flags = b.Value;
            var npcId = b.Key;
            foreach (var f in flags.GetFlags()) {
                Call($"Add{Enum.GetName(f)}RepellentNPCID", npcId);
            }
        }
        foreach (var b in BuffMetadata.PlayerDoTDebuff) {
            if (b >= BuffID.Count && BuffLoader.GetBuff(b).Mod.Name == "Aequus") {
                Call("AddPlayerDoTBuffID", b);
            }
        }
        foreach (var b in BuffMetadata.PlayerStatusDebuff) {
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