using Terraria.UI;

namespace Aequus.Common.Hooks;

public partial class TerrariaHooks : ILoad {
    public void Load(Mod mod) {
        On_ItemSlot.PickItemMovementAction += On_ItemSlot_PickItemMovementAction;
        On_Main.UpdateTime_StartDay += On_Main_UpdateTime_StartDay;
        On_Main.UpdateTime_StartNight += On_Main_UpdateTime_StartNight;
    }

    public void Unload() { }
}
