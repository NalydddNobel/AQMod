using Aequus.Common.Items.SentryChip;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.SentryChip {
    public class ApplyEquipFunctionalInteraction : SentryInteraction {
        public override void Load(Mod mod) {
            AddTo(ItemID.FireGauntlet);
            AddTo(ItemID.MagmaStone);
            AddTo(ItemID.ArcticDivingGear);
            AddTo(ItemID.JellyfishDivingGear);
            AddTo(ItemID.JellyfishNecklace);
            AddTo(ItemID.Magiluminescence);
        }

        public override void OnSentryAI(SentryAccessoryInfo info) {
            info.DummyPlayer.ApplyEquipFunctional(info.Accessory, false);
        }
    }
}