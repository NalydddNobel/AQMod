using System;

namespace Aequus.Common.Projectiles;

public abstract class LegacyHeldSwordProjectile : HeldProjBase {
    public const String SwordSwingFlipTimer = "SwordSwingFlip";

    private Boolean _init;

    public Int32 swingTime;
    public Int32 swingTimeMax;

    protected Single baseSwordScale;
    public Vector2 BaseAngleVector { get => Vector2.Normalize(Projectile.velocity); set => Projectile.velocity = Vector2.Normalize(value); }
    public virtual Single AnimProgress => Math.Clamp(1f - swingTime / (Single)swingTimeMax, 0f, 1f);

    public Int32 hitsLeft;

    public Int32 TimesSwinged {
        get {
            return Main.player[Projectile.owner].GetModPlayer<AequusPlayer>().itemUsage / 60;
        }
        set {
            Main.player[Projectile.owner].GetModPlayer<AequusPlayer>().itemUsage = (UInt16)(value * 60);
        }
    }

    public virtual Int32 AmountAllowedActive => 1;

    public override void SetDefaults() {
        Projectile.SetDefaultHeldProj();
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.localNPCHitCooldown = 500;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.ownerHitCheck = true;
        Projectile.aiStyle = ProjAIStyleID.Spear;
        hitsLeft = 2;
        _init = false;
    }

    public override void AI() {
        var player = Main.player[Projectile.owner];
        var aequus = player.GetModPlayer<AequusPlayer>();
        Projectile.aiStyle = -1;
        player.heldProj = Projectile.whoAmI;

        if (!_init) {
            Projectile.scale = 1f;
            DoInitialize(player, player.GetModPlayer<AequusPlayer>());
            baseSwordScale = Projectile.scale;
            Projectile.netUpdate = true;
            _init = true;
        }

        Int32 amountAllowedActive = AmountAllowedActive;
        if ((amountAllowedActive > 0 && player.ownedProjectileCounts[Type] > amountAllowedActive) || swingTime <= 0) {
            Projectile.Kill();
        }

        if (!player.frozen && !player.stoned) {
            Single progress = AnimProgress;
            UpdateSword(player, aequus, progress);
        }
        else {
            Projectile.timeLeft++;
        }
    }

    public virtual Single SwingProgress(Single progress) {
        return progress;
    }

    public virtual Single GetScale(Single progress) {
        return baseSwordScale;
    }

    protected abstract void UpdateSword(Player player, AequusPlayer aequus, Single progress);

    private void DoInitialize(Player player, AequusPlayer aequus) {
        if (player.whoAmI == Projectile.owner) {
            ExtendProjectile.MeleeScale(Projectile);
        }

        swingTimeMax = player.itemAnimationMax;

        Initialize(player, aequus);

        baseSwordScale = Projectile.scale;
        if (AmountAllowedActive == 1) {
            player.itemTime = swingTimeMax + 1;
            player.itemTimeMax = swingTimeMax + 1;
            player.itemAnimation = swingTimeMax + 1;
            player.itemAnimationMax = swingTimeMax + 1;
        }

        swingTimeMax *= Projectile.extraUpdates + 1;
        swingTime = swingTimeMax;
        Projectile.timeLeft = swingTimeMax + 2;
    }

    protected virtual void Initialize(Player player, AequusPlayer aequus) {
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, Int32 damageDone) {
        hitsLeft--;
    }

    public override Boolean? CanHitNPC(NPC target) {
        return hitsLeft > 0 ? null : false;
    }

    public override void OnKill(Int32 timeLeft) {
        Main.player[Projectile.owner].ownedProjectileCounts[Type]--;
        var aequusPlayer = Main.player[Projectile.owner].GetModPlayer<AequusPlayer>();
        if (aequusPlayer.TryGetTimer(SwordSwingFlipTimer, out var timer)) {
            timer.MaxTime = swingTimeMax + 8;
            if (timer.Active) {
                timer.TimePassed = timer.MaxTime;
            }
            else {
                timer.TimePassed = 0;
            }
        }
        else {
            aequusPlayer.SetTimer(SwordSwingFlipTimer, swingTimeMax + 8);
        }
        TimesSwinged++;
    }

    #region Swing Progress methods
    public static Single SwingProgressStariteSword(Single progress) {
        return MathF.Max(MathF.Sqrt(MathF.Sqrt(MathF.Sqrt(SwingProgressAequus(progress)))), MathHelper.Lerp(progress, 1f, progress));
    }

    public static Single SwingProgressSplit(Single progress) {
        return progress >= 0.5f ? 0.5f + (0.5f - MathF.Pow(2f, 20f * (0.5f - (progress - 0.5f)) - 10f) / 2f) : MathF.Pow(2f, 20f * progress - 10f) / 2f;
    }
    public static Single SwingProgressAequus(Single progress, Single pow = 2f) {
        if (progress > 0.5f) {
            return 0.5f - SwingProgressAequus(0.5f - (progress - 0.5f), pow) + 0.5f;
        }
        return ((Single)Math.Sin(Math.Pow(progress, pow) * MathHelper.TwoPi - MathHelper.PiOver2) + 1f) / 2f;
    }
    public static Single SwingProgressBoring(Single progress, Single pow = 2f, Single startSwishing = 0.15f) {
        Single oldProg = progress;
        Single max = 1f - startSwishing;
        if (progress < startSwishing) {
            progress *= (Single)Math.Pow(progress / startSwishing, pow);
        }
        else if (progress > max) {
            progress -= max;
            progress = startSwishing - progress;
            progress *= (Single)Math.Pow(progress / startSwishing, pow);
            progress = startSwishing - progress;
            progress += max;
        }
        return MathHelper.Clamp((Single)Math.Sin(progress * MathHelper.Pi - MathHelper.PiOver2) / 2f + 0.5f, 0f, oldProg);
    }
    #endregion
}