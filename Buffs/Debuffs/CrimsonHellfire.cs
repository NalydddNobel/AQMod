using Aequus.Buffs.Debuffs;
using Aequus.Common.Buffs;
using Aequus.Common.Net.Sounds;
using Aequus.Common.Particles;
using Aequus.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs {
    public class CrimsonHellfire : ModBuff, BuffHooks.IOnAddBuff {
        public static int BaseDamage = 28;
        public static int StackDamage = 4;
        public static byte MaxStacks = 200;
        public static int BaseDamageNumber = 14;
        public static int StackDamageNumber = 2;
        public static Color FireColor = new Color(205, 15, 30, 60);
        public static Color BloomColor = new Color(175, 10, 2, 10);
        public static int[] Debuffs;

        public override void SetStaticDefaults() {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            AequusBuff.DemonSiegeEnemyImmunity.Add(Type);
            AequusBuff.IsFire.Add(Type);
            AequusBuff.PlayerDoTBuff.Add(Type);
            Debuffs = new[] { Type, BuffID.OnFire3 };
        }

        public override bool ReApply(NPC npc, int time, int buffIndex) {
            var aequus = npc.Aequus();
            aequus.crimsonHellfireStacks = (byte)Math.Min(aequus.crimsonHellfireStacks + 1, MaxStacks);
            return false;
        }

        public override void Update(NPC npc, ref int buffIndex) {
            var aequus = npc.Aequus();
            aequus.crimsonHellfireStacks = Math.Max(aequus.crimsonHellfireStacks, (byte)1);
            aequus.debuffCrimsonFire = true;
        }

        public void PostAddBuff(NPC npc, int duration, bool quiet) {
            if (npc.HasBuff<CrimsonHellfire>()) {
                ModContent.GetInstance<BlueFireDebuffSound>().Play(npc.Center, pitchOverride: -0.2f);
            }
        }
    }
}

namespace Aequus.NPCs {
    public partial class AequusNPC {
        public bool debuffCrimsonFire;
        public byte crimsonHellfireStacks;

        private void DebuffEffect_CrimsonHellfire(NPC npc) {
            if (!debuffCrimsonFire) {
                return;
            }

            int amt = Math.Max((int)(npc.Size.Length() / 32f), 1);
            for (int i = 0; i < amt; i++) {
                ParticleSystem.New<MonoBloomParticle>(ParticleLayer.BehindPlayers).Setup(
                    Main.rand.NextFromRect(npc.Hitbox),
                    -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f)),
                    CrimsonHellfire.FireColor, CrimsonHellfire.BloomColor * 0.03f,
                    Main.rand.NextFloat(1.5f, 3f), 0.3f,
                    Main.rand.NextFloat(MathHelper.TwoPi)
                );
            }
        }

        private void UpdateLifeRegen_CrimsonHellfire(NPC npc, AequusNPC aequus, ref LifeRegenModifiers modifiers) {
            if (debuffCrimsonFire) {
                modifiers.LifeRegen -= CrimsonHellfire.BaseDamage + CrimsonHellfire.StackDamage * aequus.crimsonHellfireStacks;
                modifiers.DamageNumber = CrimsonHellfire.BaseDamageNumber + CrimsonHellfire.StackDamageNumber * aequus.crimsonHellfireStacks;
                modifiers.ApplyOiled = true;
            }
            else {
                aequus.crimsonHellfireStacks = 0;
            }
        }
    }
}