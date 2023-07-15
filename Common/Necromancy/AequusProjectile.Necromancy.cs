using Aequus.Common;
using Aequus.Common.Necromancy;
using System.Runtime.CompilerServices;
using Terraria;

namespace Aequus.Projectiles {
    public partial class AequusProjectile {
        public ZombieInfo zombieInfo;

        public bool IsZombie {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => zombieInfo.IsZombie;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => zombieInfo.IsZombie = value;
        }
        public int PlayerOwner {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => zombieInfo.PlayerOwner;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => zombieInfo.PlayerOwner = value;
        }

        private void SetDefaults_Zombie() {
            zombieInfo = new();
        }

        private void PreAI_CheckZombie(Projectile projectile) {
            GameUpdateData.Zombies.Clear();

            if (!IsZombie) {
                return;
            }

            var player = Main.player[PlayerOwner];
            // Setup various things
            GameUpdateData.Zombies.RunningZombie = true;
            GameUpdateData.Zombies.player = player;
            GameUpdateData.Zombies.playerOldPosition = player.position;
            GameUpdateData.Zombies.npcTargetIndex = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.owner = zombieInfo.PlayerOwner;

            if (zombieInfo.HasSetDamage) {
                projectile.damage = zombieInfo.SetDamage;
            }
        }

        private void PostAI_CheckZombie(Projectile projectile) {
            GameUpdateData.Zombies.Clear();
        }
    }
}