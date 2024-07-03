using Microsoft.Xna.Framework.Input;

namespace Aequu2.Core.Debug.CheatCodes;

internal class RevealElements() : CheatCode<MultiStateProvider>(
    Params.DebugOnly | Params.SaveAndLoad,
    new MultiStateProvider("off", "weapons only", "all"),
    Keys.W, Keys.Left
) {
    public const int Off = 0;
    public const int WeaponsOnly = 1;
    public const int All = 2;
}
