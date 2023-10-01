using Aequus.Core.IO;

namespace Aequus;

public partial class AequusPlayer {
    [SaveData("NetherStar")]
    [SaveDataAttribute.IsListedBoolean]
    public bool yinYangBonusSlot;
}