namespace Aequus.Content.ItemPrefixes.Necromancy {
    public class PhantasmalPrefix : NecromancyPrefixBase {
        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) {
            damageMult += 0.15f;
            useTimeMult *= 0.9f;
            manaMult *= 0.9f;
            knockbackMult += 0.15f;
        }

        public override void ModifyValue(ref float valueMult) {
            valueMult = 2f;
        }
    }
}