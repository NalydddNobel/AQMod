using Aequus.Content.Fishing;

namespace Aequus.Core.ContentGeneration;

public abstract class UnifiedFishingPole : ModItem {
    protected ModProjectile _bobber;

    protected override bool CloneNewInstances => true;

    public override string LocalizationCategory => "Fishing.FishingPoles";

    public sealed override void Load() {
        _bobber = new InstancedFishingBobber(this);
        Mod.AddContent(_bobber);
    }

    public virtual bool BobberPreAI(Projectile bobber) => true;
    public virtual void BobberOnKill(Projectile bobber, int timeLeft) { }
    public virtual bool BobberPreDraw(Projectile bobber, ref Color lightColor) => true;
}