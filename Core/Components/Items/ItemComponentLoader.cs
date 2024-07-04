namespace AequusRemake.Core.Entities.Items.Components;

public class ItemComponentLoader : ILoad {
    public void Load(Mod mod) {
        On_Player.UpdateItemDye += IUpdateItemDye.Player_UpdateItemDye;
        On_Player.ApplyPotionDelay += IApplyPotionDelay.On_Player_ApplyPotionDelay;
    }

    public void Unload() {
    }
}