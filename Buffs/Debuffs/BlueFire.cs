using Aequus.Buffs.Debuffs;
using Aequus.Common.Particles;
using Aequus.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs {
    public class BlueFire : ModBuff {
        public static int Damage = 20;
        public static int DamageNumber = 5;

        public override void SetStaticDefaults() {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            AequusBuff.PlayerDoTBuff.Add(Type);
        }

        public override void Update(NPC npc, ref int buffIndex) {
            npc.Aequus().debuffBlueFire = true;
        }
    }
}

namespace Aequus.NPCs {
    public partial class AequusNPC {
        public bool debuffBlueFire;

        private void DebuffEffect_BlueFire(NPC npc) {
            if (!debuffBlueFire) {
                return;
            }

            int amt = Math.Max((int)(npc.Size.Length() / 64f), 1);
            for (int i = 0; i < amt; i++) {
                ParticleSystem.New<BlueFireParticle>(ParticleLayer.BehindPlayers).Setup(
                    Main.rand.NextFromRect(npc.Hitbox),
                    -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f)),
                    new Color(45, 90, 205, 0), new Color(0, 0, 10, 0),
                    Main.rand.NextFloat(1.5f, 3f), 0.3f,
                    Main.rand.NextFloat(MathHelper.TwoPi)
                );
            }
        }

        private void UpdateLifeRegen_BlueFire(NPC npc, ref LifeRegenModifiers modifiers) {
            if (debuffBlueFire) {
                modifiers.LifeRegen -= BlueFire.Damage;
                modifiers.DamageNumber = BlueFire.DamageNumber;
                modifiers.ApplyOiled = true;
            }
        }
    }
}