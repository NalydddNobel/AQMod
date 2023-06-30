using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Buffs {
    public abstract class BaseMinionBuff : ModBuff {
        protected abstract int MinionProj { get; }

        public override void SetStaticDefaults() {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) {
            if (player.ownedProjectileCounts[MinionProj] > 0) {
                player.buffTime[buffIndex] = 18000;
            }
            else {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}