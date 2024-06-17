namespace Aequus.Common.Fishing;
public abstract class FishingPoleItem : ModItem {
    public virtual bool PreDrawFishingLine(Projectile bobber) {
        return true;
    }

    public virtual bool BobberPreAI(Projectile bobber) {
        return true;
    }

    public virtual void BobberOnKill(Projectile bobber, int timeLeft) { }
}