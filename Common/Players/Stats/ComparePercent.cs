using System;
using Terraria;
using Terraria.Localization;

namespace Aequus.Common.Players.Stats;

public struct ComparePercent : IStatComparison<float> {
    public float Before { get; set; }

    public float After { get; set; }
    public StatDifference Difference { get; set; }
    public string DifferenceText { get; set; }

    public LocalizedText Suffix { get; init; }

    private readonly GetStat<float> _getStat;
    private readonly double _multiplier;

    public ComparePercent(GetStat<float> GetStat, LocalizedText Suffix = null, double Multiplier = 100.0) {
        _getStat = GetStat;
        _multiplier = Multiplier;
        this.Suffix = Suffix;
    }

    public void Record(Player player) {
        Before = _getStat(player);
    }

    public void Measure(Player player) {
        After = _getStat(player);
        Difference = DifferenceMethods.CompareFloat(After, Before);
        DifferenceText = Difference.GetSign() + Math.Round((After - Before) * _multiplier).ToString() + "%";
    }

    public object Clone() {
        return MemberwiseClone();
    }
}
