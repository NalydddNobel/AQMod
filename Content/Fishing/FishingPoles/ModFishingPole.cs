namespace Aequus.Content.Fishing.FishingPoles;

public abstract class ModFishingPole : ModItem {
    [CloneByReference]
    protected ModProjectile Bobber { get; private set; }

    protected override System.Boolean CloneNewInstances => true;

    public override System.String LocalizationCategory => "Fishing.FishingPoles";

    public sealed override void Load() {
        Bobber = new InstancedFishingBobber(this);
        Mod.AddContent(Bobber);
    }

    public virtual System.Boolean BobberPreAI(Projectile bobber) => true;
    public virtual void BobberOnKill(Projectile bobber, System.Int32 timeLeft) { }
    public virtual System.Boolean BobberPreDraw(Projectile bobber, ref Color lightColor) => true;
    public virtual void GetDrawData(Projectile bobber, ref System.Single polePosX, ref System.Single polePosY, ref Color lineColor) { }
}