using Aequu2.Core.CodeGeneration;

namespace Aequu2;

public partial class Aequu2Player {
    /// <summary>Used to increase droprates. Rerolls the drop (amt of lootluck) times, if there is a decimal left, then it has a (lootluck decimal) chance of rerolling again.</summary>
    [ResetEffects]
    public float dropRolls;
    [ResetEffects]
    public bool accGrandRewardDownside;
}