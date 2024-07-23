namespace Aequus.Systems.Shimmer;

public interface IShimmerOverride {
    /// <returns>Return <see langword="false"/> to allow vanilla shimmer actions to occur.</returns>
    bool GetShimmered(Item item, int type) { return false; }
    /// <summary>This method is not instanced.</summary>
    /// <returns>Return <see langword="null"/> to allow vanilla shimmer conditions to be checked.</returns>
    bool? IsTransformLocked(int type) { return null; }
    /// <returns>Return <see langword="null"/> to allow vanilla shimmer conditions to be checked.</returns>
    bool? CanShimmer(Item item, int type) { return null; }
}
