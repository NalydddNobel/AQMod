using Aequus.Core.Initialization;
using Aequus.Core.UI;
using System.Collections.Generic;
using Terraria.UI;

namespace Aequus.Common.UI; 

public class AequusUIState : UIState, ILoadable, IPostSetupContent {
    public virtual UserInterface Interface => UISystem.TalkInterface;

    public virtual System.Int32 GetLayerIndex(List<GameInterfaceLayer> layers) {
        return -1;
    }

    public virtual System.Boolean ModifyInterfaceLayers(List<GameInterfaceLayer> layers, ref InterfaceScaleType scaleType) {
        return true;
    }

    public virtual void Load(Mod mod) {
    }

    public virtual void PostSetupContent(Aequus aequus) {
    }

    public virtual void Unload() {
    }

    public virtual void ConsumePlayerControls(Player player) {
    }

    public virtual System.Boolean HoverSlot(Item[] inventory, System.Int32 context, System.Int32 slot) {
        return false;
    }

    protected System.Boolean NotTalkingTo<T>() where T : ModNPC {
        return Main.LocalPlayer.talkNPC == -1 || Main.npc[Main.LocalPlayer.talkNPC].type != ModContent.NPCType<T>();
    }

    protected void DisableAnnoyingInventoryLayeringStuff(List<GameInterfaceLayer> layers) {
        System.Int32 inventoryLayer = layers.FindIndex((g) => g.Name.Equals("Vanilla: Info Accessories Bar"));
        for (System.Int32 i = inventoryLayer; i >= 0; i--) {
            if (!layers[i].Name.StartsWith("Vanilla: Interface Logic") && !layers[i].Name.Equals("Vanilla: Achievement Complete Popups")
                && !layers[i].Name.Equals("Vanilla: Invasion Progress Bars") && layers[i].ScaleType == InterfaceScaleType.UI)
                layers[i].Active = false;
        }
        if (Main.playerInventory) {
            foreach (var g in layers) {
                if (g.Name.Equals(InterfaceLayerNames.Inventory_28)) {
                    g.Active = true;
                }
            }
        }
    }

    protected void CloseThisInterface() {
        Interface.SetState(null);
    }
}