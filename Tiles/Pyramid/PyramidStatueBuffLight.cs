using Terraria;

namespace Aequus.Tiles.Pyramid {
    public class PyramidStatueBuffLight : PyramidStatueBuff {
        public override void Update(Player player, ref int buffIndex) {
            player.Aequus().darkDamage += 0.1f;
        }
    }
}