namespace Aequus.Content.Items.Weapons.Ranged.JunkJet;

public struct JunkJetAmmoInfo {
    public int ProjectileId;
    public int ProjectileCount;
    public int Damage;
    public float Knockback;
    public float ShootSpeed;
    public float AttackSpeedMultiplier;
    public float BaseSpread;
    public float MaxSpread;

    public JunkJetAmmoInfo() {
        ProjectileId = 0;
        Damage = 0;
        Knockback = 0f;
        ShootSpeed = 4f;
        AttackSpeedMultiplier = 1f;
        ProjectileCount = 3;
        BaseSpread = 0f;
        MaxSpread = 0.15f;
    }
}