using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.Projectiles;

public abstract class HeldSwordProjectile : HeldProjBase {
    private bool _init;

    public int swingTime;
    public int swingTimeMax;
    public int combo;

    protected float baseSwordScale;
    public Vector2 BaseAngleVector { get => Vector2.Normalize(Projectile.velocity); set => Projectile.velocity = Vector2.Normalize(value); }
    public virtual float AnimProgress => Math.Clamp(1f - swingTime / (float)swingTimeMax, 0f, 1f);

    public int hitsLeft;

    public int TimesSwinged {
        get {
            return Main.player[Projectile.owner].GetModPlayer<AequusPlayer>().itemUsage / 60;
        }
        set {
            Main.player[Projectile.owner].GetModPlayer<AequusPlayer>().itemUsage = (ushort)(value * 60);
        }
    }

    public virtual int AmountAllowedActive => 1;

    public override void SetDefaults() {
        Projectile.DefaultToHeldProj();
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.localNPCHitCooldown = 500;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.ownerHitCheck = true;
        Projectile.aiStyle = ProjAIStyleID.Spear;
        hitsLeft = 2;
        _init = false;
        combo = 0;
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

        int amountAllowedActive = AmountAllowedActive;
        if ((amountAllowedActive > 0 && player.ownedProjectileCounts[Type] > amountAllowedActive) || swingTime <= 0) {
            Projectile.Kill();
        }

        if (!player.frozen && !player.stoned) {
            float progress = AnimProgress;
            UpdateSword(player, aequus, progress);
        }
        else {
            Projectile.timeLeft++;
        }
    }

    public virtual float SwingProgress(float progress) {
        return progress;
    }

    public virtual float GetScale(float progress) {
        return baseSwordScale;
    }

    protected abstract void UpdateSword(Player player, AequusPlayer aequus, float progress);

    private void DoInitialize(Player player, AequusPlayer aequus) {
        combo = aequus.itemCombo;
        if (player.whoAmI == Projectile.owner) {
            ProjectileHelper.MeleeScale(Projectile);
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

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        hitsLeft--;
    }

    public override bool? CanHitNPC(NPC target) {
        return hitsLeft > 0 ? null : false;
    }

    public override void OnKill(int timeLeft) {
        Main.player[Projectile.owner].ownedProjectileCounts[Type]--;
        Main.player[Projectile.owner].GetModPlayer<AequusPlayer>().itemCombo = (ushort)(combo == 0 ? swingTimeMax : 0);
        TimesSwinged++;
    }

    public override void SendExtraAI(BinaryWriter writer) {
        writer.Write(combo);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        combo = reader.ReadInt32();
    }

    #region Swing Progress methods
    public static float SwingProgressStariteSword(float progress) {
        return MathF.Max(MathF.Sqrt(MathF.Sqrt(MathF.Sqrt(SwingProgressAequus(progress)))), MathHelper.Lerp(progress, 1f, progress));
    }

    public static float SwingProgressSplit(float progress) {
        return progress >= 0.5f ? 0.5f + (0.5f - MathF.Pow(2f, 20f * (0.5f - (progress - 0.5f)) - 10f) / 2f) : MathF.Pow(2f, 20f * progress - 10f) / 2f;
    }
    public static float SwingProgressAequus(float progress, float pow = 2f) {
        if (progress > 0.5f) {
            return 0.5f - SwingProgressAequus(0.5f - (progress - 0.5f), pow) + 0.5f;
        }
        return ((float)Math.Sin(Math.Pow(progress, pow) * MathHelper.TwoPi - MathHelper.PiOver2) + 1f) / 2f;
    }
    public static float SwingProgressBoring(float progress, float pow = 2f, float startSwishing = 0.15f) {
        float oldProg = progress;
        float max = 1f - startSwishing;
        if (progress < startSwishing) {
            progress *= (float)Math.Pow(progress / startSwishing, pow);
        }
        else if (progress > max) {
            progress -= max;
            progress = startSwishing - progress;
            progress *= (float)Math.Pow(progress / startSwishing, pow);
            progress = startSwishing - progress;
            progress += max;
        }
        return MathHelper.Clamp((float)Math.Sin(progress * MathHelper.Pi - MathHelper.PiOver2) / 2f + 0.5f, 0f, oldProg);
    }
    #endregion
}