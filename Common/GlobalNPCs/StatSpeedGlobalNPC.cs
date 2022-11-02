using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.GlobalNPCs
{
    public class StatSpeedGlobalNPC : GlobalNPC
    {
        public static HashSet<int> IgnoreStatSpeed { get; private set; }

        public float statSpeed;

        public override bool InstancePerEntity => true;
        protected override bool CloneNewInstances => true;

        public override void Load()
        {
            IgnoreStatSpeed = new HashSet<int>()
            {
                NPCID.CultistBoss,
                NPCID.HallowBoss,
            };
            On.Terraria.NPC.UpdateCollision += NPC_UpdateCollision;
        }
        private static void NPC_UpdateCollision(On.Terraria.NPC.orig_UpdateCollision orig, NPC self)
        {
            if (IgnoreStatSpeed.Contains(self.netID))
            {
                orig(self);
                return;
            }

            float velocityBoost = self.StatSpeed();
            self.velocity *= velocityBoost;
            orig(self);
            self.velocity /= velocityBoost;
        }

        public override void ResetEffects(NPC npc)
        {
            statSpeed = 1f;
        }

        public override void PostAI(NPC npc)
        {
            if (npc.noTileCollide && statSpeed != 1f)
            {
                npc.position += npc.velocity * (statSpeed - 1f);
            }
        }
    }
}
