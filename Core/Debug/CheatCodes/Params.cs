using System.ComponentModel;

namespace AequusRemake.Core.Debug.CheatCodes;

[EditorBrowsable(EditorBrowsableState.Never)]
public enum Params : byte {
    None = 0,
    DebugOnly = 1 << 0,
    SaveAndLoad = 1 << 1,
}
