using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Items.Components;

public class ComponentLoader : ILoadable {
    public void Load(Mod mod) {
        On_Player.UpdateItemDye += IUpdateItemDye.Player_UpdateItemDye;
    }

    public void Unload() {
    }
}
