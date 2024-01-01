using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using System;

namespace Aequus.Content.Fishing.FishingPoles;

internal partial class InstancedFishingPole : InstancedModItem {
    public readonly FishingRodStats Stats;
    public readonly FishingRodDrawInfo DrawInfo;
    internal ModProjectile _bobber;

    public record struct FishingRodStats(int FishingPower, float ShootSpeed, int Rarity, int Value);
    public record struct FishingRodDrawInfo(Vector2 LineOriginOffset, Func<Projectile, Color> GetLineColor);

    public InstancedFishingPole(string name, FishingRodStats stats, FishingRodDrawInfo drawInfo) : base(name + "FishingPole", typeof(InstancedFishingPole).NamespaceFilePath() + $"/{name}FishingPole") {
        Stats = stats;
        DrawInfo = drawInfo;
    }

    public override string LocalizationCategory => "Fishing.FishingPoles";

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.WoodFishingPole);
        Item.fishingPole = Stats.FishingPower;
        Item.shootSpeed = Stats.ShootSpeed;
        Item.rare = Stats.Rarity;
        Item.value = Stats.Value;
        Item.shoot = _bobber?.Type ?? ProjectileID.FishingBobber;
    }
}