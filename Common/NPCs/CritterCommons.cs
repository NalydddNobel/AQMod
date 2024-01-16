using System;
using System.Linq;

namespace Aequus.Common.NPCs;

public class CritterCommons : GlobalNPC {
    public interface ICritter {
        bool IsGolden { get; }
    }

    public override void SetStaticDefaults() {
        foreach (var npc in Mod.GetContent<ModNPC>().Where(n => n is ICritter)) {
            NPCID.Sets.CountsAsCritter[npc.Type] = true;
            if ((npc as ICritter).IsGolden) {
                NPCID.Sets.GoldCrittersCollection.Add(npc.Type);
                NPCID.Sets.NormalGoldCritterBestiaryPriority.Add(npc.Type);
            }
        }
    }

    public class GoldenCritterCommons : GlobalNPC {
        public override bool AppliesToEntity(NPC npc, bool lateInstantiation) {
            return npc.ModNPC is ICritter critter && critter.IsGolden;
        }

        public override void SetDefaults(NPC npc) {
            npc.rarity = 3;
        }

        public override void PostAI(NPC npc) {
            npc.position += npc.netOffset;
            Color color = LightHelper.GetLightColor(npc.Center);
            int sparkleChance = Math.Max(Math.Max(color.R, color.G), color.B);
            if (sparkleChance > 30) {
                sparkleChance /= 30;
                if (Main.rand.Next(300) < sparkleChance) {
                    Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.TintableDustLighted, Alpha: 254, newColor: new Color(255, 255, 0), Scale: 0.5f);
                    dust.velocity *= 0f;
                }
            }
            npc.position -= npc.netOffset;
        }
    }
}
