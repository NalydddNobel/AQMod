using Aequus.Buffs.Debuffs;
using Aequus.Common.Particles;
using Aequus.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs {
    public class IronLotusDebuff : ModBuff {
        public static int Damage = 300;
        public static int DamageNumber = 15;

        public override string Texture => Aequus.PlaceholderDebuff;

        public override void SetStaticDefaults() {
            Main.debuff[Type] = true;
            AequusBuff.IsFire.Add(Type);
            AequusBuff.PlayerDoTBuff.Add(Type);
        }

        public override void Update(NPC npc, ref int buffIndex) {
            npc.Aequus().debuffIronLotus = true;
        }
    }
}

namespace Aequus.NPCs {
    public partial class AequusNPC {
        public bool debuffIronLotus;

        private void PostAI_IronLotusSpread(NPC npc) {
            if (!debuffIronLotus) {
                return;
            }

            int debuff = npc.FindBuffIndex(ModContent.BuffType<IronLotusDebuff>());
            if (debuff == -1) {
                return; // ???????????????
            }

            int plr = Player.FindClosest(npc.position, npc.width, npc.height);
            for (int i = 0; i < Main.maxNPCs; i++) {
                if (Main.npc[i].active && (Main.npc[i].type == NPCID.TargetDummy || Main.npc[i].CanBeChasedBy(Main.player[plr])) && Main.npc[i].Distance(npc.Center) < 100f) {
                    Main.npc[i].AddBuff(ModContent.BuffType<IronLotusDebuff>(), npc.buffTime[debuff]);
                }
            }
        }

        private void DebuffEffect_IronLotus(NPC npc) {
            if (!debuffIronLotus) {
                return;
            }

            int amt = (int)(npc.Size.Length() / 32f);
            for (int i = 0; i < amt; i++) {
                ParticleSystem.New<MonoBloomParticle>(ParticleLayer.BehindPlayers).Setup(
                    Main.rand.NextFromRect(npc.Hitbox), 
                    -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-2f, 2f), -Main.rand.NextFloat(-2f, 2f)),
                    new Color(180, 90, 40, 60) * 0.5f, new Color(6, 0, 3, 0), 
                    Main.rand.NextFloat(2f, 3f), 0.3f, 
                    Main.rand.NextFloat(MathHelper.TwoPi)
                );
            }
        }

        private void UpdateLifeRegen_IronLotus(NPC npc, ref LifeRegenModifiers modifiers) {
            if (debuffIronLotus) {
                modifiers.LifeRegen -= IronLotusDebuff.Damage;
                modifiers.DamageNumber = IronLotusDebuff.DamageNumber;
                modifiers.ApplyOiled = true;
            }
        }
    }
}