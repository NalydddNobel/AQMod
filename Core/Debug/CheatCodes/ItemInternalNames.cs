using Aequus.Core.CodeGeneration;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Aequus.Core.Debug.CheatCodes;

internal class ItemInternalNames() : CheatCode<HoldStateProvider>(
    Params.DebugOnly,
    new HoldStateProvider(),
    Keys.D, Keys.Down
) {
    [Gen.AequusItem_ModifyTooltips]
    public static void ShowInternalNames(Item item, List<TooltipLine> tooltips) {
        if (ModContent.GetInstance<ItemInternalNames>()?.State?.Active == true) {
            string fullName = item.ModItem?.FullName ?? ItemID.Search.GetName(item.netID);
            tooltips.Add(new TooltipLine(Instance, "fullname", fullName));

            // Copy to clipboard

            /*
            if (!Main.oldKeyState.IsKeyDown(Keys.Down)) {
                Platform.Get<IClipboard>().Value = fullName;
            }
            */
        }
    }
}
