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
    public class CorruptionHellfire : ModBuff, BuffHooks.IOnAddBuff {
        public static int BaseDamage = 28;
        public static int StackDamage = 2;
        public static byte MaxStacks = 200;
        public static int BaseDamageNumber = 14;
        public static int StackDamageNumber = 1;
        public static Color FireColor = new Color(100, 28, 160, 60);
        public static Color BloomColor = new Color(20, 2, 80, 10);
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
            aequus.corruptionHellfireStacks = (byte)Math.Min(aequus.corruptionHellfireStacks + 1, MaxStacks);
            return false;
        }

        public override void Update(NPC npc, ref int buffIndex) {
            var aequus = npc.Aequus();
            aequus.corruptionHellfireStacks = Math.Max(aequus.corruptionHellfireStacks, (byte)1);
            aequus.debuffCorruptionFire = true;
        }

        public void PostAddBuff(NPC npc, int duration, bool quiet) {
            if (npc.HasBuff<CorruptionHellfire>()) {
                ModContent.GetInstance<BlueFireDebuffSound>().Play(npc.Center, pitchOverride: -0.2f);
            }
        }
    }
}

namespace Aequus.NPCs {
    public partial class AequusNPC {
        public bool debuffCorruptionFire;
        public byte corruptionHellfireStacks;

        private void DebuffEffect_CorruptionHellfire(NPC npc) {
            if (!debuffCorruptionFire) {
                return;
            }

            int amt = Math.Max((int)(npc.Size.Length() / 32f), 1);
            for (int i = 0; i < amt; i++) {
                ParticleSystem.New<MonoBloomParticle>(ParticleLayer.BehindPlayers).Setup(
                    Main.rand.NextFromRect(npc.Hitbox), 
                    -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f)),
                    CorruptionHellfire.FireColor, CorruptionHellfire.BloomColor * 0.07f, 
                    Main.rand.NextFloat(1.5f, 3f), 0.3f, 
                    Main.rand.NextFloat(MathHelper.TwoPi)
                );
            }
        }

        private void UpdateLifeRegen_CorruptionHellfire(NPC npc, AequusNPC aequus, ref LifeRegenModifiers modifiers) {
            if (debuffCorruptionFire) {
                modifiers.LifeRegen -= CorruptionHellfire.BaseDamage + CorruptionHellfire.StackDamage * aequus.corruptionHellfireStacks;
                modifiers.DamageNumber = CorruptionHellfire.BaseDamageNumber + CorruptionHellfire.StackDamageNumber * aequus.corruptionHellfireStacks;
                modifiers.ApplyOiled = true;
            }
            else {
                aequus.corruptionHellfireStacks = 0;
            }
        }
    }
}