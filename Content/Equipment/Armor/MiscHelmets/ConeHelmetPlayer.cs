namespace Aequus.Content.Equipment.Armor.MiscHelmets;

public sealed class ConeHelmetPlayer : ModPlayer {
    public ConeHelmet armorConeHelmet;
    public Vector2 _coneHelmetShake;
    public int _coneHelmetIntensity;

    public override void ResetEffects() {
        armorConeHelmet = null;
    }

    public override void PreUpdate() {
        if (_coneHelmetIntensity > 0) {
            _coneHelmetIntensity--;
            _coneHelmetShake = Main.rand.NextVector2Square(-_coneHelmetIntensity, _coneHelmetIntensity) * 0.4f;
        }
    }

    public override void OnHurt(Player.HurtInfo info) {
        if (armorConeHelmet == null) {
            return;
        }

        armorConeHelmet.hitsLeft--;
        _coneHelmetIntensity = 12;

        if (armorConeHelmet.hitsLeft <= 0) {
            armorConeHelmet.OnBreak(Player);
            armorConeHelmet = null;
        }
    }
}
