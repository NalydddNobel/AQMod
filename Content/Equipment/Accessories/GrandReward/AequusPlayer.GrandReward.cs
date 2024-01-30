using Aequus.Content.Equipment.Accessories.GrandReward;
using Aequus.Core;
using Aequus.Core.Generator;

namespace Aequus;

public partial class AequusPlayer {
    /// <summary>
    /// Used to increase droprates. Rerolls the drop (amt of lootluck) times, if there is a decimal left, then it has a (lootluck decimal) chance of rerolling again.
    /// </summary>
    [ResetEffects()]
    public System.Single dropRolls;
    [ResetEffects]
    public System.Boolean accGrandRewardDownside;

    [SaveData("CosmicChest")]
    [SaveDataAttribute.IsListedBoolean]
    public System.Boolean usedCosmicChest;

    private void UpdateCosmicChest() {
        if (usedCosmicChest) {
            dropRolls += CosmicChest.LuckIncrease;
        }
    }
}