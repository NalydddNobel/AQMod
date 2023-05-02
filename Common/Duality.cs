using System;

namespace Aequus.Common {
    [Flags]
    public enum Duality : byte {
        Dark = 0,
        Light = 1,
        Neither = 2,
        Neutral = 3,
    }
}