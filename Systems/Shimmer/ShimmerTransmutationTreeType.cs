namespace Aequus.Systems.Shimmer;

public enum ShimmerTransmutationTreeType {
    /// <summary>Items convert into each other, eventually ending with an item with no conversion.</summary>
    Line,
    /// <summary>Items convert into each other, and will cycle infinitely.</summary>
    Loop,
    /// <summary>Items will cycle, but will never reach the starting point.</summary>
    Broken
}
