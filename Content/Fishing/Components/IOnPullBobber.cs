namespace Aequus.Content.Fishing.Components;

/// <summary>Only works on Fishing Pole or Bait items.</summary>
internal interface IOnPullBobber {
    /// <returns><see langword="true" /> to override default pulling operations.</returns>
    public bool PrePullBobber(Player player, Projectile bobber, ref int bait) { return false; }
    public void PostPullBobber(Player player, Projectile bobber, int bait) { }
}
