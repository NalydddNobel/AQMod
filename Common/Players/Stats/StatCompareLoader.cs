using Aequus.Content.Equipment.Accessories.AccCrowns.Blood;
using System;
using System.Collections.Generic;
using Terraria.Localization;

namespace Aequus.Common.Players.Stats;

public class StatCompareLoader : ModSystem {
    public static List<IStatComparison> RegisteredStats { get; private set; } = new();
    internal static List<StatComparer> TrackedComparers { get; private set; } = new();

    public static void Register(IStatComparison comparator) {
        RegisteredStats.Add(comparator);
    }

    public override void PostSetupContent() {
        RegisteredStats.AddRange(new List<IStatComparison>() {
            new CompareTime((p) => p.wingTimeMax, Affix("WingTime")),
            new ComparePercent((p) => p.moveSpeed, Affix("MoveSpeed")),
            new ComparePercent((p) => p.manaCost, Affix("ManaCost"), Multiplier: -100f),
            new ComparePercent((p) => p.endurance, Affix("DamageReduction")),
            new CompareFloat((p) => p.maxMinions, Affix("MinionSlots")),
            new CompareFloat((p) => p.maxTurrets, Affix("SentrySlots")),
            new CompareFloat((p) => p.lifeRegen, Affix("LifeRegen"), Multiplier: 0.5f),
            new CompareFloat((p) => p.statLifeMax2, Affix("MaxLife")),
            new CompareFloat((p) => p.statManaMax2, Affix("MaxMana")),
            new CompareFloat((p) => p.statDefense, Affix("Defense")),
            new CompareFloat((p) => p.jumpSpeedBoost, Affix("JumpSpeedBoost")),
            new CompareFloat((p) => p.extraFall, Affix("ExtraFall")),
            new CompareFloat((p) => p.fishingSkill, Affix("FishingSkill")),
            new CompareFloat((p) => Player.tileRangeX, Affix("TileRangeX")),
            new CompareFloat((p) => Player.tileRangeY, Affix("TileRangeY")),
            new CompareFloat((p) => p.blockRange, Affix("BlockRange")),
            new CompareVague((p) => p.aggro, Affix("Aggro.More"), Affix("Aggro.Less")),
            new CompareVague((p) => p.spikedBoots, Affix("ClimbingGear.More"), Affix("ClimbingGear.Less")),
        });

        for (int i = 0; i < DamageClassLoader.DamageClassCount; i++) {
            AddDamageTracker(DamageClassLoader.GetDamageClass(i));
        }

        static void AddDamageTracker(DamageClass damageClass) {
            string key = damageClass.Name.Replace("DamageClass", "").Replace("Damage", "");
            string name = damageClass.DisplayName.Value.ToLower().Replace(" damage", "");
            if (damageClass.Mod != null && damageClass.Mod != Aequus.Instance) {
                key = damageClass.Mod.Name + "." + key;
            }
            RegisteredStats.Add(new ComparePercent((p) => p.GetDamage(damageClass).Additive, Affix(key, () => $"bonus {name} damage")));
            RegisteredStats.Add(new CompareFloat((p) => p.GetDamage(damageClass).Flat, Affix(key + "Flat", () => $"bonus flat {name} damage")));
            RegisteredStats.Add(new ComparePercent((p) => p.GetCritChance(damageClass), Affix(key + "Crit", () => $"bonus {name} critical strike chance"), Multiplier: 1f));
            RegisteredStats.Add(new ComparePercent((p) => p.GetKnockback(damageClass).Additive, Affix(key + "KB", () => $"bonus {name} knockback")));
            RegisteredStats.Add(new ComparePercent((p) => p.GetAttackSpeed(damageClass), Affix(key + "Speed", () => $"bonus {name} speed")));
            RegisteredStats.Add(new ComparePercent((p) => p.GetArmorPenetration(damageClass), Affix(key + "Speed", () => $"bonus {name} armor penetration")));
        }
    }

    public static LocalizedText Affix(string key, Func<string> createDefault = null) {
        return Language.GetOrRegister(BloodCrown.CategoryKey + ".StatAffix." + key, createDefault);
    }

    public override void AddRecipes() {
        foreach (var comparer in TrackedComparers) {
            comparer.Load();
        }
    }

    public override void Unload() {
        TrackedComparers.Clear();
        RegisteredStats.Clear();
    }
}

public static class StatCompareExtensions {
    public static LocalizedText GetStatCompareText(this ILocalizedModType modType, string key, Func<string> createDefault = null) => StatCompareLoader.Affix($"{modType.Name}." + key, createDefault);
}