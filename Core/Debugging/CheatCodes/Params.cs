using System.ComponentModel;

namespace Aequus.Core.Debugging.CheatCodes;

[EditorBrowsable(EditorBrowsableState.Never)]
public enum Params : byte {
    None = 0,
    DebugOnly = 1 << 0,
    Toggle = 1 << 1,
    SaveAndLoad = 1 << 2,
}
