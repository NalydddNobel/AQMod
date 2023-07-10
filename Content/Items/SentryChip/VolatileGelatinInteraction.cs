using Aequus.Common.Items.SentryChip;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.SentryChip {
    public class VolatileGelatinInteraction : SentryInteraction {
        public override void Load(Mod mod) {
            AddTo(ItemID.VolatileGelatin);
        }

        public override void OnSentryAI(SentryAccessoryInfo info) {
            info.DummyPlayer.VolatileGelatin(info.Accessory);
        }
    }
}