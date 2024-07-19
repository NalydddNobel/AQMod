using System;

namespace Aequus.Common.Structures.Conditions;

public sealed class ConditionCollection(params Condition[] conditions) {
    private Condition[] _conditions = conditions;
    public Condition[] Conditions { get; set; } = conditions;

    public ConditionCollection() : this([]) { }

    public void Add(Condition condition) {
        Array.Resize(ref _conditions, _conditions.Length + 1);
        _conditions[^1] = condition;
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
