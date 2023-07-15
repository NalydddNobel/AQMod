using Aequus.Common.Necromancy;
using Aequus.NPCs;
using Aequus.Projectiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;

namespace Aequus.Common.Necromancy {
    public struct NecromancyUpdateData {
        public bool RunningZombie { get; internal set; }
        public Player player;
        public Vector2 playerOldPosition;
        public int npcTargetIndex;
        public bool HasNPCTarget => Main.npc.IndexInRange(npcTargetIndex);
        public NPC NPCTarget => Main.npc[npcTargetIndex];

        public List<NPC> ZombieNPCs;
        public List<NPC> NormalNPCs;
        public List<Projectile> Projectiles;

        public NecromancyUpdateData() {
            ZombieNPCs = new();
            NormalNPCs = new();
            Projectiles = new();
            RunningZombie = false;
            player = null;
            playerOldPosition = Vector2.Zero;
            npcTargetIndex = 0;
        }

        public void HardClear() {
            NormalNPCs.Clear();
            ZombieNPCs.Clear();
            Projectiles.Clear();
            Clear();
        }
        public void UpdateIndices() {
            for (int i = 0; i < Main.maxNPCs; i++) {
                var npc = Main.npc[i];
                if (!npc.active || !npc.TryGetGlobalNPC<AequusNPC>(out var aequus)) {
                    return;
                }

                var zombieInfo = aequus.zombieInfo;
                (zombieInfo.IsZombie ? ZombieNPCs : NormalNPCs).Add(npc);
            }
            for (int i = 0; i < Main.maxProjectiles; i++) {
                var proj = Main.projectile[i];
                if (!proj.active || !proj.friendly || !proj.TryGetGlobalProjectile<AequusProjectile>(out var aequus)) {
                    return;
                }

                var zombieInfo = aequus.zombieInfo;
                if (zombieInfo.IsZombie) {
                    Projectiles.Add(proj);
                }
            }
        }

        public void Clear() {
            RunningZombie = false;
            if (player != null) {
                ResetPlayerPosition();
            }

            player = null;
            npcTargetIndex = -1;
        }

        public void ResetPlayerPosition() {
            player.position = playerOldPosition;
        }
    }
}

namespace Aequus.Common {
    public partial class GameUpdateData {
        public static NecromancyUpdateData Zombies = new();
    }
}