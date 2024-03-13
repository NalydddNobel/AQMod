using Aequus.Content.PermaPowerups.Shimmer;
using Aequus.Core.CodeGeneration;
using Aequus.Core.IO;

namespace Aequus;

public partial class AequusPlayer {
    /// <summary>Used to increase droprates. Rerolls the drop (amt of lootluck) times, if there is a decimal left, then it has a (lootluck decimal) chance of rerolling again.</summary>
    [ResetEffects]
    public float dropRolls;
    [ResetEffects]
    public bool accGrandRewardDownside;
}