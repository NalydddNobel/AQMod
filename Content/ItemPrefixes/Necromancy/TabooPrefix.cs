namespace Aequus.Content.ItemPrefixes.Necromancy {
    public class TabooPrefix : NecromancyPrefixBase {
        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) {
            useTimeMult *= 0.9f;
            knockbackMult += 0.1f;
            manaMult += 0.1f;
        }

        public override void ModifyValue(ref float valueMult) {
            valueMult = 1.2f;
        }
    }
}