using Aequus.Common.Items.SentryChip;
using Aequus.Common.ModPlayers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.SentryChip {
    public class BoneHelmInteraction : SentryInteraction {
        public override void Load(Mod mod) {
            AddTo(ItemID.BoneHelm);
        }

        public override void OnSentryAI(SentryAccessoryInfo info) {
            PlayerReflection.Player_SpawnHallucination.Invoke(info.DummyPlayer, new object[] { info.Accessory });
        }
    }
}