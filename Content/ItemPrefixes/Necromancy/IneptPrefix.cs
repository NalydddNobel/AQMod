namespace Aequus.Content.ItemPrefixes.Necromancy {
    public class IneptPrefix : NecromancyPrefixBase {
        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) {
            manaMult += 0.1f;
        }

        public override void ModifyValue(ref float valueMult) {
            valueMult = 0.8f;
        }
    }
}