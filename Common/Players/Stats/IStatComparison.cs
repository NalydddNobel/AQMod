using System;
using Terraria;
using Terraria.Localization;

namespace Aequus.Common.Players.Stats;

public interface IStatComparison : ICloneable {
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

public interface IStatComparison<T> : IStatComparison {
    T Before { get; set; }
    T After { get; set; }
}

public delegate T GetStat<T>(Player player);
