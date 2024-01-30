namespace Aequus.Content.Fishing.FishingPoles;

public abstract class ModFishingPole : ModItem {
    [CloneByReference]
    protected ModProjectile Bobber { get; private set; }

    protected override bool CloneNewInstances => true;

    public override string LocalizationCategory => "Fishing.FishingPoles";

    public sealed override void Load() {
        Bobber = new InstancedFishingBobber(this);
        Mod.AddContent(Bobber);
    }

    public virtual bool BobberPreAI(Projectile bobber) => true;
    public virtual void BobberOnKill(Projectile bobber, int timeLeft) { }
    public virtual bool BobberPreDraw(Projectile bobber, ref Color lightColor) => true;
    public virtual void GetDrawData(Projectile bobber, ref float polePosX, ref float polePosY, ref Color lineColor) { }
}