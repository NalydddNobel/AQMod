using Aequus.Common.Buffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs {
    public class BoneRingWeakness : ModBuff {
        public override string Texture => Aequus.VanillaTexture + "Buff_" + BuffID.Weak;

        public static float BossMovementSpeedMultiplier = 0.4f;
        public static float MovementSpeedMultiplier = 0.4f;

        public override void SetStaticDefaults() {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;

            AequusBuff.SetImmune(NPCID.WallofFleshEye, false, Type);
            foreach (var n in ContentSamples.NpcsByNetId) {
                if (n.Value.boss || (NPCID.Sets.DebuffImmunitySets.TryGetValue(n.Key, out var buff) && buff != null && buff.SpecificallyImmuneTo != null && buff.SpecificallyImmuneTo.ContainsAny(BuffID.Weak))) {
                    AequusBuff.SetImmune(n.Key, false, Type);
                }
            }
            AequusBuff.PlayerStatusBuff.Add(Type);
        }

        public override void Update(NPC npc, ref int buffIndex) {
            if (npc.boss) {
                npc.StatSpeed() *= BossMovementSpeedMultiplier;
            }
            else {
                npc.StatSpeed() *= MovementSpeedMultiplier;
            }
            npc.Aequus().debuffBoneRing = true;
        }
    }
}

namespace Aequus.NPCs {
    public partial class AequusNPC {
        public bool debuffBoneRing;

        private void DrawEffects_BoneRingDebuff(NPC npc, ref Color drawColor) {
            if (debuffBoneRing) {
                drawColor = (drawColor * 0.8f) with { A = drawColor.A };
            }
        }

        private void DebuffEffects_BoneRing(NPC npc) {
            if (!debuffBoneRing) {
                return;
            }

            if (syncedTimer % 10 == 0) {
                var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Slush, Alpha: 160, Scale: Main.rand.NextFloat(0.6f, 1f));
                d.velocity = npc.velocity.SafeNormalize(Vector2.Zero);
                d.noGravity = true;
                d.fadeIn = d.scale + 0.5f;
            }
            if (npc.life >= 0 && Main.GameUpdateCount % 30 == 0) {
                npc.HitEffect(0, 10);
            }
            if (!npc.noTileCollide) {
                int x = (int)((npc.position.X + npc.width / 2f) / 16f);
                int checkTilesTop = (int)((npc.position.Y + npc.height) / 16f);
                int checkTilesBottom = (int)(npc.position.Y / 16f);
                for (int j = checkTilesBottom; j <= checkTilesTop; j++) {
                    if (Main.tile[x, j].IsFullySolid()) {
                        var d = Dust.NewDustPerfect(new Vector2(npc.position.X + Main.rand.NextFloat(npc.width), npc.position.Y + npc.height), DustID.Slush, Vector2.Zero, 160, Scale: Main.rand.NextFloat(0.6f, 1f));
                        d.noGravity = true;
                        d.fadeIn = d.scale + 0.5f;
                        break;
                    }
                }
            }
        }
    }
}