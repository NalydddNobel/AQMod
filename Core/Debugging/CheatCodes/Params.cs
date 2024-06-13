using System.ComponentModel;

namespace Aequus.Core.Debugging.CheatCodes;

[EditorBrowsable(EditorBrowsableState.Never)]
public enum Params : byte {
    None = 0,
    DebugOnly = 1 << 0,
    SaveAndLoad = 1 << 1,
}
