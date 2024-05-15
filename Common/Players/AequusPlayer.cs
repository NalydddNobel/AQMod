using Aequus.Content.Items.Weapons.Ranged.Bows.SkyHunterCrossbow;
using Aequus.Core.CodeGeneration;
using System.Runtime.CompilerServices;
using Terraria.GameInput;
using Terraria.ModLoader.IO;

namespace Aequus;

[Gen.AequusPlayer_ResetField<StatModifier>("wingTime")]
public partial class AequusPlayer : ModPlayer {
    public Vector2 transitionVelocity;

    public int timeSinceRespawn;

    public override void Load() {
        _resetEffects = new();
        _resetEffects.Generate();
    }

    public override void Unload() {
        _resetEffects = null;
    }

    public override void Initialize() {
        Timers = new();
    }

    public override void OnEnterWorld() {
        timeSinceRespawn = 0;
    }

    public override void PreUpdate() {
        UpdateGiftRing();
        UpdateTimers();
        UpdateItemFields();
    }

    public override void PostUpdateEquips() {
        PostUpdateEquipsInner();
        UpdateTeamEffects();
        Player.wingTimeMax = (int)wingTime.ApplyTo(Player.wingTimeMax);
    }

    public override void PostUpdateMiscEffects() {
        HandleTileEffects();
        UpdateScrapBlockState();
        if ((transitionVelocity - Player.velocity).Length() < 0.01f) {
            transitionVelocity = Player.velocity;
        }
        transitionVelocity = Vector2.Lerp(transitionVelocity, Player.velocity, 0.25f);
    }

    public override void PostUpdate() {
        UpdateDangers();
        timeSinceRespawn++;
    }

    public override void ModifyZoom(ref float zoom) {
        if (zoom < 0.5f) {
            if (Player.HeldItem.ModItem is SkyHunterCrossbow && (PlayerInput.UsingGamepad || Main.mouseRight)) {
                zoom = 0.5f;
            }
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
#if !DEBUG
        ProcBoneRing(target);
        ProcBlackPhial(target);
#endif
    }

    internal void OnKillNPC(in KillInfo info) {
        RestoreBreathOnKillNPC(in info);
    }

    public override void OnRespawn() {
        timeSinceRespawn = 0;
        OnRespawnInner();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ResetObj<T>(ref T obj) {
        obj = default(T);
    }

    #region IO
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SaveObj<T>(TagCompound tag, string name, T obj) {
        if (obj?.Equals(default(T)) == true) {
            tag[name] = obj;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void LoadObj<T>(TagCompound tag, string name, ref T obj) {
        obj = default;
        if (tag.TryGet(name, out T result)) {
            obj = result;
        }
    }
    #endregion

    #region Misc
    /// <param name="Center">The enemy's center.</param>
    /// <param name="Type">The enemy's type.</param>
    public record struct KillInfo(Vector2 Center, int Type);

    public struct MiscDamageHit {
        public DamageClass DamageClass;
        public Rectangle DamagingHitbox;
        public double Damage;
        public float Knockback;
    }
    #endregion
}