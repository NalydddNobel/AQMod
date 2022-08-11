using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.UI
{
    public class AequusUIState : UIState, ILoadable
    {
        public virtual int GetLayerIndex(List<GameInterfaceLayer> layers)
        {
            return -1;
        }

        public virtual bool ModifyInterfaceLayers(List<GameInterfaceLayer> layers, ref InterfaceScaleType scaleType)
        {
            return true;
        }

        public virtual void Load(Mod mod)
        {
        }

        public virtual void Unload()
        {
        }

        protected bool IsTalkingTo<T>() where T : ModNPC
        {
            return Main.LocalPlayer.talkNPC == -1 || Main.npc[Main.LocalPlayer.talkNPC].type != ModContent.NPCType<T>();
        }

        protected void DisableAnnoyingInventoryLayeringStuff(List<GameInterfaceLayer> layers)
        {
            int inventoryLayer = layers.FindIndex((g) => g.Name.Equals("Vanilla: Info Accessories Bar"));
            for (int i = inventoryLayer; i >= 0; i--)
            {
                if (!layers[i].Name.StartsWith("Vanilla: Interface Logic") && !layers[i].Name.Equals("Vanilla: Achievement Complete Popups")
                    && !layers[i].Name.Equals("Vanilla: Invasion Progress Bars") && layers[i].ScaleType == InterfaceScaleType.UI)
                    layers[i].Active = false;
            }
            if (Main.playerInventory)
            {
                foreach (var g in layers)
                {
                    if (g.Name.Equals(AequusUI.InterfaceLayers.Inventory))
                    {
                        g.Active = true;
                    }
                }
            }
        }
    }
}