using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common {
    public partial class GameUpdateData : ModSystem {
        public static Entity CurrentEntity;

        public override void ClearWorld() {
            Zombies.HardClear();
        }

        public override void PreUpdateEntities() {
            CurrentEntity = null;
            Zombies.HardClear();
        }

        public override void PostUpdateDusts() {
            CurrentEntity = null;
        }

        public static void SetupNPC(NPC npc) {
            CurrentEntity = npc;
        }

        public static void SetupProj(Projectile projectile) {
            CurrentEntity = projectile;
        }
    }
}