namespace Aequus.Content.ItemPrefixes.Necromancy {
    public class HysteriaPrefix : NecromancyPrefixBase {
        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) {
            damageMult += 0.15f;
            manaMult *= 0.85f;
            knockbackMult += 0.5f;
        }

        public override void ModifyValue(ref float valueMult) {
            valueMult = 1.9f;
        }
    }
}
