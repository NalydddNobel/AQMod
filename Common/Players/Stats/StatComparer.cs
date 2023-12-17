using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace Aequus.Common.Players.Stats;

public class StatComparer {
    public List<IStatComparison> Comparisons { get; private set; }

    internal StatComparer() {
        StatCompareLoader.TrackedComparers.Add(this);
    }

    ~StatComparer() {
        StatCompareLoader.TrackedComparers.Remove(this);
    }

    public void Record(Player player) {
        foreach (var c in Comparisons) {
            c.Record(player);
        }
    }

    public void Measure(Player player) {
        foreach (var c in Comparisons) {
            c.Measure(player);
        }
    }

    internal void Load() {
        Comparisons = StatCompareLoader.RegisteredStats.Select((c) => (IStatComparison)c.Clone()).ToList();
    }
}