using Aequus.Common.DataSets;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.NPCs.Global {
    public class StatSpeedGlobalNPC : GlobalNPC {
        public static HashSet<int> IgnoreStatSpeed => NPCSets.StatSpeedBlacklist;

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

        public override void SetStaticDefaults() {
            if (Aequus.InfoLogs) {
                Aequus.Instance.Logger.Info("Loading stat speed interactions array...");
            }
            var val = Aequus.GetContentArrayFile("IgnoreStatSpeed");
            foreach (var modDict in val) {
                if (modDict.Key == "Vanilla") {
                    foreach (var npcName in modDict.Value) {
                        IgnoreStatSpeed.Add(NPCID.Search.GetId(npcName));
                    }
                }
                else if (ModLoader.TryGetMod(modDict.Key, out var mod)) {
                    if (Aequus.InfoLogs) {
                        Aequus.Instance.Logger.Info($"Loading custom wall to item ID table entries for {modDict.Key}...");
                    }
                    foreach (var npcName in modDict.Value) {
                        if (mod.TryFind<ModNPC>(npcName, out var modNPC)) {
                            IgnoreStatSpeed.Add(modNPC.Type);
                        }
                    }
                }
            }
        }

        public override void SetDefaults(NPC npc) {
            statSpeed = 1f;
        }

        public override void ResetEffects(NPC npc) {
            statSpeed = 1f;
            statSpeedJumpSpeedMultiplier = 0.5f;
        }

        public override void PostAI(NPC npc) {
            if (npc.noTileCollide && statSpeed != 1f && !IgnoreStatSpeed.Contains(npc.netID)) {
                npc.position += npc.velocity * (statSpeed - 1f);
            }
        }
    }
}
