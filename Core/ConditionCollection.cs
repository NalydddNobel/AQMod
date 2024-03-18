namespace Aequus.Core;

public class ConditionCollection {
    public Condition[] Conditions { get; set; }

    public ConditionCollection(params Condition[] conditions) {
        Conditions = conditions;
    }

    public bool IsMet() {
        if (Conditions != null && Conditions.Length != 0) {
            for (int i = 0; i < Conditions.Length; i++) {
                if (Conditions[i]?.IsMet() == false) {
                    return false;
                }
            }
        }
        return true;
    }
}
