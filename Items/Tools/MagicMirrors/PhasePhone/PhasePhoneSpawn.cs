using Aequus.Items.Tools.MagicMirrors.PhaseMirror;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.MagicMirrors.PhasePhone {
    public class PhasePhoneSpawn : PhasePhoneBase {
        public override int ShellphoneClone => ItemID.ShellphoneSpawn;
        public override int ShellphoneConvert => ModContent.ItemType<PhasePhoneOcean>();

        public override void Teleport(Player player, Item item, IPhaseMirror me) {
            player.Shellphone_Spawn();
        }

        public override void GetPhaseMirrorDust(Player player, Item item, IPhaseMirror me, out int dustType, out Color dustColor) {
            dustType = DustID.RainbowMk2;
            dustColor = Main.rand.Next(4) switch {
                2 => Color.Yellow,
                3 => Color.White,
                _ => new(100, 255, 100),
            };
        }
    }
}