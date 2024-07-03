namespace Aequu2.Core.ContentGeneration;

public abstract class UnifiedModMinion : ModProjectile {
    internal InstancedMinionBuff MinionBuff { get; set; }

    protected override bool CloneNewInstances => true;

    internal abstract InstancedMinionBuff CreateMinionBuff();

    public override string LocalizationCategory => "Minions";

    public void SetItemStats(Item item) {
        item.shoot = Type;
        item.buffType = MinionBuff.Type;
    }

    public sealed override void Load() {
        MinionBuff = CreateMinionBuff();
        OnLoad();
        Mod.AddContent(MinionBuff);
    }
    protected virtual void OnLoad() { }

    public override void SetDefaults() {
        Projectile.netImportant = true;
        Projectile.friendly = true;
        Projectile.minion = true;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.penetrate = -1;
        Projectile.timeLeft *= 5;
        Projectile.minionSlots = 1f;
    }

    public override void SetStaticDefaults() {
        Main.projPet[Type] = true;
        ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        ProjectileID.Sets.MinionSacrificable[Type] = true;
    }

    public override void AI() {
        Player owner = Main.player[Projectile.owner];
        if (owner.dead) {
            MinionBuff._petFlag(owner) = false;
        }
        if (MinionBuff._petFlag(owner)) {
            Projectile.timeLeft = 2;
        }
    }
}
