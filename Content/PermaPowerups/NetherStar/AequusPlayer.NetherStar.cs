﻿using Aequus.Core;

namespace Aequus;

public partial class AequusPlayer {
    [SaveData("NetherStar")]
    [SaveDataAttribute.IsListedBoolean]
    public System.Boolean usedConvergentHeart;
}