using Aequus.Common.Items;
using Aequus.Core;
using Microsoft.Xna.Framework;

namespace Aequus.Content.Fishing.FishingPoles;

internal partial class InstancedFishingPole : InstancedModItem {
    public delegate bool DelegatePreAI(Projectile projectile);
    public InstanceHook<InstancedFishingPole, DelegatePreAI> PreAI { get; private set; } = new();

    public delegate void DelegateOnKill(Projectile projectile, int timeLeft);
    public InstanceHook<InstancedFishingPole, DelegateOnKill> OnKill { get; private set; } = new();

    public delegate bool DelegatePreDraw(Projectile projectile, ref Color lightColor);
    public InstanceHook<InstancedFishingPole, DelegatePreDraw> PreDraw { get; private set; } = new();
}