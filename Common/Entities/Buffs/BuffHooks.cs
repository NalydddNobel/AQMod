namespace Aequus.Common.Entities.Buffs;

public interface ICheckQuickBuff {
    bool CheckQuickBuff(Player player);
}

public class BuffHooks : LoadedType {
    protected override void Load() {
        On_Player.QuickBuff_ShouldBotherUsingThisBuff += On_Player_QuickBuff_ShouldBotherUsingThisBuff;
    }

    private static bool On_Player_QuickBuff_ShouldBotherUsingThisBuff(On_Player.orig_QuickBuff_ShouldBotherUsingThisBuff orig, Player self, int attemptedType) {
        if (BuffLoader.GetBuff(attemptedType) is ICheckQuickBuff modifyQuickBuff && !modifyQuickBuff.CheckQuickBuff(self)) {
            return false;
        }

        return orig(self, attemptedType);
    }
}