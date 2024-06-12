using Aequus.Core.CodeGeneration;
using Microsoft.Xna.Framework.Input;
using ReLogic.OS;
using System.Collections.Generic;

namespace Aequus.Core.Debugging.CheatCodes;

internal abstract partial class CheatCode {
    public class RevealAllElements() : CheatCode(Params.DebugOnly | Params.Toggle | Params.SaveAndLoad, Keys.W, Keys.Left);
    public class VisibleElements() : CheatCode(Params.DebugOnly | Params.Toggle | Params.SaveAndLoad, Keys.W, Keys.Right);
    public class ItemInternalNames() : CheatCode(Params.DebugOnly, Keys.D, Keys.Down);
    [Gen.AequusItem_ModifyTooltips]
    public static void ShowInternalNames(Item item, List<TooltipLine> tooltips) {
#if DEBUG
        if (IsActive<ItemInternalNames>()) {
            string fullName = item.ModItem?.FullName ?? ItemID.Search.GetName(item.netID);
            tooltips.Add(new TooltipLine(Instance, "fullname", fullName));

            // Copy to clipboard
            if (!Main.oldKeyState.IsKeyDown(Keys.Down)) {
                var clipboard = Platform.Get<IClipboard>();

                clipboard.Value = fullName;
            }
        }
#endif
    }

}
