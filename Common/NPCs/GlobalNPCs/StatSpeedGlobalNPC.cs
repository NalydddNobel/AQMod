using Aequus.Common.DataSets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.NPCs.Global {
    public class StatSpeedGlobalNPC : GlobalNPC {
        public float statSpeed;
        public float statSpeedJumpSpeedMultiplier;

        public override bool InstancePerEntity => true;
        protected override bool CloneNewInstances => true;

        public StatSpeedGlobalNPC() {
            statSpeed = 1f;
            statSpeedJumpSpeedMultiplier = 0.5f;
        }

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) {
            return !NPCSets.StatSpeedBlacklist.Contains(entity.type);
        }

        public override void Load() {
            On_NPC.UpdateCollision += NPC_UpdateCollision;
        }

        private static void NPC_UpdateCollision(On_NPC.orig_UpdateCollision orig, NPC npc) {
            if (!npc.TryGetGlobalNPC<StatSpeedGlobalNPC>(out var statSpeedGlobalNPC)) {
                orig(npc);
                return;
            }

            var velocityBoost = new Vector2(statSpeedGlobalNPC.statSpeed);
            if (!npc.noGravity) {
                if (velocityBoost.Y < 1f) {
                    velocityBoost.Y /= statSpeedGlobalNPC.statSpeedJumpSpeedMultiplier;
                }
                else {
                    velocityBoost.Y = 1f + (velocityBoost.Y - 1f) * statSpeedGlobalNPC.statSpeedJumpSpeedMultiplier;
                }
            }

            npc.velocity.X *= velocityBoost.X;
            npc.velocity.Y *= velocityBoost.Y;
            orig(npc);
            npc.velocity.X /= velocityBoost.X;
            npc.velocity.Y /= velocityBoost.Y;
        }

        public override void SetDefaults(NPC npc) {
            statSpeed = 1f;
        }

        public override void ResetEffects(NPC npc) {
            statSpeed = 1f;
            statSpeedJumpSpeedMultiplier = 0.5f;
        }

        public override void PostAI(NPC npc) {
            if (npc.noTileCollide && statSpeed != 1f && !NPCSets.StatSpeedBlacklist.Contains(npc.netID)) {
                npc.position += npc.velocity * (statSpeed - 1f);
            }
        }
    }
}
