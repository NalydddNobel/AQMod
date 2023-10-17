using Aequus.Content.Items.Tools.MagicMirrors.PhaseMirror;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Tools.MagicMirrors.PhasePhone {
    public class PhasePhoneHome : PhasePhoneBase {
        public override int ShellphoneClone => ItemID.Shellphone;
        public override int ShellphoneConvert => ModContent.ItemType<PhasePhoneSpawn>();

        public override void Teleport(Player player, Item item, IPhaseMirror me) {
            player.Spawn(PlayerSpawnContext.RecallFromItem);
        }

        public override void GetPhaseMirrorDust(Player player, Item item, IPhaseMirror me, out int dustType, out Color dustColor) {
            dustType = DustID.MagicMirror;
            dustColor = Color.White;
        }
    }
}