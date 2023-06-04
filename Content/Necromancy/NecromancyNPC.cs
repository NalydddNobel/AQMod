using Aequus.Common;
using Aequus.Content.Necromancy;
using System.IO;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.NPCs {
    public partial class AequusNPC {
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

        public int soulHealth;
        public int soulHealthDecayTimer;

        private void Load_Zombie() {
            On_NPC.SetTargetTrackingValues += NPC_SetTargetTrackingValues;
            On_NPC.FindFirstNPC += NPC_FindFirstNPC;
            On_NPC.AnyNPCs += NPC_AnyNPCs;
            On_NPC.CountNPCS += NPC_CountNPCS;
        }

        #region Hooks
        private static int NPC_FindFirstNPC(On_NPC.orig_FindFirstNPC orig, int Type) {
            if (GameUpdateData.Zombies.ZombieNPCs.Count == 0) {
                return orig(Type);
            }

            var npcs = GameUpdateData.Zombies.RunningZombie ? GameUpdateData.Zombies.NormalNPCs : GameUpdateData.Zombies.ZombieNPCs;
            foreach (var npc in npcs) {
                npc.active = false;
            }

            int value = orig(Type);

            foreach (var npc in npcs) {
                npc.active = true;
            }

            return value;
        }

        private static bool NPC_AnyNPCs(On_NPC.orig_AnyNPCs orig, int Type) {
            if (GameUpdateData.Zombies.ZombieNPCs.Count == 0) {
                return orig(Type);
            }

            var npcs = GameUpdateData.Zombies.RunningZombie ? GameUpdateData.Zombies.NormalNPCs : GameUpdateData.Zombies.ZombieNPCs;
            foreach (var npc in npcs) {
                npc.active = false;
            }

            bool value = orig(Type);

            foreach (var npc in npcs) {
                npc.active = true;
            }
            return value;
        }

        private static int NPC_CountNPCS(On_NPC.orig_CountNPCS orig, int Type) {
            if (GameUpdateData.Zombies.ZombieNPCs.Count == 0) {
                return orig(Type);
            }

            var npcs = GameUpdateData.Zombies.RunningZombie ? GameUpdateData.Zombies.NormalNPCs : GameUpdateData.Zombies.ZombieNPCs;
            foreach (var npc in npcs) {
                npc.active = false;
            }

            int value = orig(Type);

            foreach (var npc in npcs) {
                npc.active = true;
            }
            return value;
        }

        private static void NPC_SetTargetTrackingValues(On_NPC.orig_SetTargetTrackingValues orig, NPC npc, bool faceTarget, float realDist, int tankTarget) {
            if (npc.TryGetGlobalNPC<AequusNPC>(out var aequusNPC) && aequusNPC.IsZombie) {
                npc.target = aequusNPC.PlayerOwner;
                if (GameUpdateData.Zombies.HasNPCTarget) {
                    npc.targetRect = GameUpdateData.Zombies.NPCTarget.getRect();
                }
                else {
                    npc.targetRect = Main.player[npc.target].getRect();
                }

                if (faceTarget) {
                    npc.direction = npc.targetRect.X + npc.targetRect.Width / 2 < npc.position.X + npc.width / 2 ? -1 : 1;
                    npc.directionY = npc.targetRect.Y + npc.targetRect.Height / 2 < npc.position.Y + npc.height / 2 ? -1 : 1;
                }
                return;
            }
            orig(npc, faceTarget, realDist, tankTarget);
        }
        #endregion

        private void SetDefaults_Zombie() {
            zombieInfo = new();
        }

        private void ResetEffects_CheckSoulHealth(NPC npc) {
            if (soulHealthDecayTimer > 0) {
                soulHealthDecayTimer--;
                return;
            }

            soulHealth--;
        }

        private void CheckHit_SoulHealth() {

        }

        private void PreAI_CheckZombie(NPC npc) {
            GameUpdateData.Zombies.Clear();

            if (!IsZombie) {
                return;
            }

            var player = Main.player[PlayerOwner];
            // Setup various things
            npc.friendly = true;
            npc.target = PlayerOwner;
            npc.GivenName = player.name + "'s " + Lang.GetNPCName(npc.netID);
            npc.boss = false;
            npc.dontTakeDamage = true;
            npc.SpawnedFromStatue = true;
            npc.npcSlots = 0f;
            GameUpdateData.Zombies.RunningZombie = true;
            GameUpdateData.Zombies.player = player;
            GameUpdateData.Zombies.playerOldPosition = player.position;

            int targetResult = -1;
            float minDistance = 1000f;
            foreach (var n in GameUpdateData.Zombies.NormalNPCs) {
                float distance = npc.Distance(n);
                if (!n.active || distance > minDistance || !n.CanBeChasedBy()) {
                    continue;
                }

                targetResult = n.whoAmI;
                minDistance = distance;
            }

            GameUpdateData.Zombies.npcTargetIndex = targetResult;
            if (GameUpdateData.Zombies.HasNPCTarget) {
                player.Center = Main.npc[targetResult].Center;
            }
        }

        private void PostAI_CheckZombie(NPC npc) {
            GameUpdateData.Zombies.Clear();
        }

        private void SendExtraAI_Zombie(BitWriter bitWriter, BinaryWriter binaryWriter) {
            bitWriter.WriteBit(IsZombie);

            if (!IsZombie) {
                return;
            }

            binaryWriter.Write((byte)PlayerOwner);
        }

        private void ReceiveExtraAI_Zombie(BitReader bitReader, BinaryReader binaryReader) {
            if (!bitReader.ReadBit()) {
                return;
            }

            PlayerOwner = binaryReader.ReadByte();
        }
    }

    public class NecromancyHitbox : ModProjectile {
        public override string Texture => AequusTextures.None.Path;

        public override void SetDefaults() {
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.width = 16;
            Projectile.height = 16;
        }
    }
}