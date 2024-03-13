using Terraria;
using Terraria.Localization;

namespace Aequus.Common.Players.Stats;

public struct CompareTime : IStatComparison<float> {
    public float Before { get; set; }

    public float After { get; set; }
    public StatDifference Difference { get; set; }
    public string DifferenceText { get; set; }

    public LocalizedText Suffix { get; init; }

    private readonly GetStat<float> _getStat;

    public CompareTime(GetStat<float> GetStat, LocalizedText Suffix = null) {
        _getStat = GetStat;
        this.Suffix = Suffix;
    }

    public void Record(Player player) {
        Before = _getStat(player);
    }

    public void Measure(Player player) {
        After = _getStat(player);
        Difference = DifferenceMethods.CompareFloat(After, Before);
        DifferenceText = Difference.GetSign() + ExtendLanguage.Seconds(After - (double)Before);
    }

    public object Clone() {
        return MemberwiseClone();
    }
}