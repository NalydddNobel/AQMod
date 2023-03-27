using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.ItemDrops {
    public class NeedLifeFruitCondition : IItemDropRuleCondition {
        public bool CanDrop(DropAttemptInfo info) {
            return info.player.statLifeMax < 500 && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3;
            //return info.player.ConsumedLifeFruit < Player.LifeFruitMax && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3;
        }

        public bool CanShowItemDropInUI() {
            return true;
        }

        public string GetConditionDescription() {
            return null;
        }
    }
    public class NeedLifeCrystalCondition : IItemDropRuleCondition {
        public bool CanDrop(DropAttemptInfo info) {
            return info.player.statLifeMax < 400;
            //return info.player.ConsumedLifeCrystals < Player.LifeCrystalMax;
        }

        public bool CanShowItemDropInUI() {
            return true;
        }

        public string GetConditionDescription() {
            return null;
        }
    }
    public class NeedManaCrystalCondition : IItemDropRuleCondition {
        public bool CanDrop(DropAttemptInfo info) {
            return info.player.statManaMax < 200;
            //return info.player.ConsumedManaCrystals < Player.ManaCrystalMax;
        }

        public bool CanShowItemDropInUI() {
            return true;
        }

        public string GetConditionDescription() {
            return null;
        }
    }
}