using Aequus.Content.Items.Tools.MagicMirrors.PhaseMirror;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Tools.MagicMirrors.PhasePhone {
    public class PhasePhoneUnderworld : PhasePhoneBase {
        public override int ShellphoneClone => ItemID.ShellphoneHell;
        public override int ShellphoneConvert => ModContent.ItemType<PhasePhoneHome>();

        public override void Teleport(Player player, Item item, IPhaseMirror me) {
            player.DemonConch();
        }

        public override void GetPhaseMirrorDust(Player player, Item item, IPhaseMirror me, out int dustType, out Color dustColor) {
            dustType = DustID.Lava;
            dustColor = default;
        }
    }
}