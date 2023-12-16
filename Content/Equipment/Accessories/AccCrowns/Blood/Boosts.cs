using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.Equipment.Accessories.AccCrowns.Blood;
public class Boosts : ModSystem {
    public delegate T GetStat<T>(Player player);

    public enum StatDifference {
        None,
        Negative,
        Positive
    }

    public interface IStatTracker {
        LocalizedText Suffix { get; init; }

        StatDifference Difference { get; set; }
        string DifferenceText { get; set; }

        /// <summary>
        /// Record stats for comparison later.
        /// </summary>
        /// <param name="player"></param>
        void Record(Player player);

        /// <summary>
        /// Fill out <see cref="Difference"/> and <see cref="DifferenceText"/> with new player stats.
        /// </summary>
        /// <param name="player"></param>
        void Measure(Player player);
    }
    public interface IStatTracker<T> : IStatTracker {
        T Before { get; set; }
        T After { get; set; }
    }

    public struct FloatTracker : IStatTracker<float> {
        public float Before { get; set; }

        public float After { get; set; }
        public StatDifference Difference { get; set; }
        public string DifferenceText { get; set; }

        public LocalizedText Suffix { get; init; }

        private readonly GetStat<float> _getStat;
        private readonly double _multiplier;

        public FloatTracker(GetStat<float> GetStat, LocalizedText Suffix = null, double Multiplier = 1f) {
            _getStat = GetStat;
            _multiplier = Multiplier;
            this.Suffix = Suffix;
        }

        public void Record(Player player) {
            Before = _getStat(player);
        }

        public void Measure(Player player) {
            After = _getStat(player);
            Difference = After == Before ? StatDifference.None : (After < Before) ? StatDifference.Negative : StatDifference.Positive;
            DifferenceText = GetSign(Difference) + Math.Round((After - Before) * _multiplier * 100 / 100).ToString();
        }
    }
    public struct PercentTracker : IStatTracker<float> {
        public float Before { get; set; }

        public float After { get; set; }
        public StatDifference Difference { get; set; }
        public string DifferenceText { get; set; }

        public LocalizedText Suffix { get; init; }

        private readonly GetStat<float> _getStat;
        private readonly double _multiplier;

        public PercentTracker(GetStat<float> GetStat, LocalizedText Suffix = null, double Multiplier = 100.0) {
            _getStat = GetStat;
            _multiplier = Multiplier;
            this.Suffix = Suffix;
        }

        public void Record(Player player) {
            Before = _getStat(player);
        }

        public void Measure(Player player) {
            After = _getStat(player);
            Difference = After == Before ? StatDifference.None : (After < Before) ? StatDifference.Negative : StatDifference.Positive;
            DifferenceText = GetSign(Difference) + Math.Round((After - Before) * _multiplier).ToString() + "%";
        }
    }
    public struct TimeTracker : IStatTracker<float> {
        public float Before { get; set; }

        public float After { get; set; }
        public StatDifference Difference { get; set; }
        public string DifferenceText { get; set; }

        public LocalizedText Suffix { get; init; }

        private readonly GetStat<float> _getStat;

        public TimeTracker(GetStat<float> GetStat, LocalizedText Suffix = null) {
            _getStat = GetStat;
            this.Suffix = Suffix;
        }

        public void Record(Player player) {
            Before = _getStat(player);
        }

        public void Measure(Player player) {
            After = _getStat(player);
            Difference = After == Before ? StatDifference.None : (After < Before) ? StatDifference.Negative : StatDifference.Positive;
            DifferenceText = GetSign(Difference) + TextHelper.Seconds(After - (double)Before);
        }
    }
    public struct VagueTracker : IStatTracker<float> {
        public float Before { get; set; }

        public float After { get; set; }
        public StatDifference Difference { get; set; }
        public string DifferenceText { get; set; }

        public LocalizedText Suffix { get; init; }

        private readonly GetStat<float> _getStat;
        private readonly LocalizedText _negativeText;
        private readonly LocalizedText _positiveText;

        public VagueTracker(GetStat<float> GetStat, LocalizedText NegativeText, LocalizedText PositiveText, LocalizedText Suffix = null) {
            _getStat = GetStat;
            _negativeText = NegativeText;
            _positiveText = PositiveText;
            this.Suffix = Suffix;
        }

        public void Record(Player player) {
            Before = _getStat(player);
        }

        public void Measure(Player player) {
            After = _getStat(player);
            Difference = After == Before ? StatDifference.None : (After < Before) ? StatDifference.Negative : StatDifference.Positive;
            DifferenceText = (Difference == StatDifference.Negative ? _negativeText : _positiveText).Value;
        }
    }

    public static string GetSign(StatDifference diff, string positive = "+", string negative = "") {
        return diff == StatDifference.Positive ? positive : negative;
    }

    public static List<IStatTracker> TrackedStats { get; private set; }

    public override void PostSetupContent() {
        TrackedStats = new() {
            new TimeTracker((p) => p.wingTimeMax, GetText("WingTime")),
            new PercentTracker((p) => p.moveSpeed, GetText("MoveSpeed")),
            new PercentTracker((p) => p.manaCost, GetText("ManaCost"), Multiplier: -100f),
            new PercentTracker((p) => p.endurance, GetText("DamageReduction")),
            new FloatTracker((p) => p.maxMinions, GetText("MinionSlots")),
            new FloatTracker((p) => p.maxTurrets, GetText("SentrySlots")),
            new FloatTracker((p) => p.lifeRegen, GetText("LifeRegen"), Multiplier: 0.5f),
            new FloatTracker((p) => p.statLifeMax2, GetText("MaxLife")),
            new FloatTracker((p) => p.statManaMax2, GetText("MaxMana")),
            new FloatTracker((p) => p.statDefense, GetText("Defense")),
            new FloatTracker((p) => p.jumpSpeedBoost, GetText("JumpSpeedBoost")),
            new FloatTracker((p) => p.extraFall, GetText("ExtraFall")),
            new FloatTracker((p) => p.fishingSkill, GetText("FishingSkill")),
            new FloatTracker((p) => Player.tileRangeX, GetText("TileRangeX")),
            new FloatTracker((p) => Player.tileRangeY, GetText("TileRangeY")),
            new FloatTracker((p) => p.blockRange, GetText("BlockRange")),
            new VagueTracker((p) => p.aggro, GetText("Aggro.More"), GetText("Aggro.Less")),
            new VagueTracker((p) => p.spikedBoots, GetText("ClimbingGear.More"), GetText("ClimbingGear.Less")),
        };

        for (int i = 0; i < DamageClassLoader.DamageClassCount; i++) {
            AddDamageTracker(DamageClassLoader.GetDamageClass(i));
        }

        static void AddDamageTracker(DamageClass damageClass) {
            string key = damageClass.Name.Replace("DamageClass", "").Replace("Damage", "");
            string name = damageClass.DisplayName.Value.ToLower().Replace(" damage", "");
            if (damageClass.Mod != null && damageClass.Mod != Aequus.Instance) {
                key = damageClass.Mod.Name + "." + key;
            }
            TrackedStats.Add(new PercentTracker((p) => p.GetDamage(damageClass).Additive, GetText(key, () => $"bonus {name} damage")));
            TrackedStats.Add(new FloatTracker((p) => p.GetDamage(damageClass).Flat, GetText(key + "Flat", () => $"bonus flat {name} damage")));
            TrackedStats.Add(new PercentTracker((p) => p.GetCritChance(damageClass), GetText(key + "Crit", () => $"bonus {name} critical strike chance"), Multiplier: 1f));
            TrackedStats.Add(new PercentTracker((p) => p.GetKnockback(damageClass).Additive, GetText(key + "KB", () => $"bonus {name} knockback")));
            TrackedStats.Add(new PercentTracker((p) => p.GetAttackSpeed(damageClass), GetText(key + "Speed", () => $"bonus {name} speed")));
            TrackedStats.Add(new PercentTracker((p) => p.GetArmorPenetration(damageClass), GetText(key + "Speed", () => $"bonus {name} armor penetration")));
        }

        static LocalizedText GetText(string key, Func<string> createDefault = null) {
            return Language.GetOrRegister(CrownOfBlood.CategoryKey + ".StatAffix." + key, createDefault);
        }
    }

    public override void Unload() {
        TrackedStats.Clear();
    }
}
