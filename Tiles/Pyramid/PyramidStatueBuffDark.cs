using Terraria;

namespace Aequus.Tiles.Pyramid {
    public class PyramidStatueBuffDark : PyramidStatueBuff {
        public override void Update(Player player, ref int buffIndex) {
            player.Aequus().lightDamage += 0.1f;
        }
    }
}