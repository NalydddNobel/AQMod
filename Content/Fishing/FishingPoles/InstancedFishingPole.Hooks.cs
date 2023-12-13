using Aequus.Common.Items;
using Aequus.Core;
using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Aequus.Content.Fishing.FishingPoles;

internal partial class InstancedFishingPole : InstancedModItem {
    public readonly InstanceHook<InstancedFishingPole, Func<Projectile, bool>> PreAI = new();
    public readonly InstanceHook<InstancedFishingPole, Action<Projectile, int>> OnKill = new();
    public readonly InstanceHook<InstancedFishingPole, Func<Projectile, Color, bool>> PreDraw = new();
}