using Terraria;
using Terraria.Localization;

namespace Aequus.Common.Players.Stats;

public struct CompareVague : IStatComparison<float> {
    public float Before { get; set; }

    public float After { get; set; }
    public StatDifference Difference { get; set; }
    public string DifferenceText { get; set; }

    public LocalizedText Suffix { get; init; }

    private readonly GetStat<float> _getStat;
    private readonly LocalizedText _negativeText;
    private readonly LocalizedText _positiveText;

    public CompareVague(GetStat<float> GetStat, LocalizedText NegativeText, LocalizedText PositiveText, LocalizedText Suffix = null) {
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
        Difference = DifferenceMethods.CompareFloat(After, Before);
        DifferenceText = (Difference == StatDifference.Negative ? _negativeText : _positiveText).Value;
    }

    public object Clone() {
        return MemberwiseClone();
    }
}