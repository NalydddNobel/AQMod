using Aequus.Common;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.NPCs {
    public class NPCHooks : HookClass {
        public override void LoadHooks(Mod mod) {
        }

        public interface ITalkNPCUpdate {
            void TalkNPCUpdate(Player player);
        }
    }
}