using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Buffs {
    public class BuffHooks : ILoadable {
        public void Load(Mod mod) {
        }

        public void Unload() {
        }

        internal interface IOnAddBuff {
            void PreAddBuff(NPC npc, ref int duration, ref bool quiet) {
            }
            void PostAddBuff(NPC npc, int duration, bool quiet) {
            }
            void PreAddBuff(Player player, ref int duration, ref bool quiet, ref bool foodHack) {
            }
            void PostAddBuff(Player player, int duration, bool quiet, bool foodHack) {
            }
        }
    }
}