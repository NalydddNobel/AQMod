using Aequus.Items.Tools.MagicMirrors.PhaseMirror;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.MagicMirrors.PhasePhone {
    public class PhasePhoneOcean : PhasePhoneBase {
        public override int ShellphoneClone => ItemID.ShellphoneOcean;
        public override int ShellphoneConvert => ModContent.ItemType<PhasePhoneUnderworld>();

        public override void Teleport(Player player, Item item, IPhaseMirror me) {
            player.MagicConch();
        }

        public override void GetPhaseMirrorDust(Player player, Item item, IPhaseMirror me, out int dustType, out Color dustColor) {
            dustType = Dust.dustWater();
            dustColor = default;
        }
    }
}