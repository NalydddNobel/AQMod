using Aequus.Common.DataSets;
using Aequus.Common.NPCs.Global;

namespace Aequus.Buffs.Debuffs {
    public class SnowgraveDebuff : ModBuff {
        public override string Texture => Aequus.PlaceholderDebuff;

        public override void SetStaticDefaults() {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffSets.PlayerStatusDebuff.Add(Type);
        }

        public override void Update(NPC npc, ref int buffIndex) {
            if (npc.TryGetGlobalNPC(out StatSpeedGlobalNPC speedNPC)) {
                speedNPC.statSpeed *= 0.75f;
            }
            npc.Aequus().debuffSnowgrave = true;
        }
    }
}

namespace Aequus.NPCs {
    public partial class AequusNPC {
        public bool debuffSnowgrave;

        private void DrawEffects_Snowgrave(NPC npc, ref Color drawColor) {
            if (debuffSnowgrave) {
                drawColor = (drawColor * 0.7f) with { B = drawColor.B, A = drawColor.A };
            }
        }

        private void DebuffEffects_Snowgrave(NPC npc) {
            if (debuffSnowgrave && syncedTimer % 9 == 0) {
                var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.IceRod, Scale: Main.rand.NextFloat(0.6f, 1f));
                d.velocity = npc.velocity * 0.5f;
                d.noGravity = true;
                d.fadeIn = d.scale + 0.5f;
            }
        }
    }
}