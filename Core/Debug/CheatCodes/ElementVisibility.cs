using Microsoft.Xna.Framework.Input;

namespace Aequu2.Core.Debug.CheatCodes;

internal class ElementVisibility() : CheatCode<MultiStateProvider>(
    Params.DebugOnly | Params.SaveAndLoad,
    new MultiStateProvider("text", "icons", "both"),
    Keys.W, Keys.Right
) {
    public const int TextOnly = 0;
    public const int IconsOnly = 1;
    public const int Both = 2;
}
