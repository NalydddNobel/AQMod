using Aequus.Common.Buffs.Components;
using Aequus.Content.DataSets;
using Aequus.Core.DataSets;
using Aequus.Old.Content.Particles;
using System;
using Terraria.Audio;

namespace Aequus.Old.Content.StatusEffects.DamageOverTime;

public class CorruptionHellfire : ModBuff, IOnAddBuff {
    public static int BaseDamage { get; set; } = 28;
    public static int StackDamage { get; set; } = 2;
    public static byte MaxStacks { get; set; } = 200;
    public static int BaseDamageNumber { get; set; } = 14;
    public static int StackDamageNumber { get; set; } = 1;
    public static Color FireColor { get; set; } = new Color(100, 28, 160, 60);
    public static Color BloomColor { get; set; } = new Color(20, 2, 80, 10);

    public override void SetStaticDefaults() {
        Main.debuff[Type] = true;
        Main.buffNoSave[Type] = true;
        BuffSets.DemonSiegeImmune.Add((BuffEntry)Type);
        BuffSets.PlayerDoTDebuff.Add((BuffEntry)Type);
    }

    public override bool ReApply(NPC npc, int time, int buffIndex) {
        if (npc.TryGetGlobalNPC(out CorruptionHellfireNPC corruptionHellfire)) {
            corruptionHellfire.stacks = (byte)Math.Min(corruptionHellfire.stacks + 1, MaxStacks);
        }
        return false;
    }

    public override void Update(NPC npc, ref int buffIndex) {
        if (npc.TryGetGlobalNPC(out CorruptionHellfireNPC corruptionHellfire)) {
            corruptionHellfire.stacks = Math.Max(corruptionHellfire.stacks, (byte)1);
            corruptionHellfire.hasDebuff = true;
        }
    }

    public void PostAddBuff(NPC npc, int duration, bool quiet) {
        if (npc.HasBuff<CorruptionHellfire>()) {
            SoundEngine.PlaySound(AequusSounds.InflictFireDebuff with { Pitch = -0.2f, PitchVariance = 0.1f }, npc.Center);
        }
    }
}

public class CorruptionHellfireNPC : GlobalNPC {
    public bool hasDebuff;
    public byte stacks;

    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation) {
        return !entity.buffImmune[ModContent.BuffType<CorruptionHellfire>()];
    }

    public override void ResetEffects(NPC npc) {
        hasDebuff = false;
    }

    public override void PostAI(NPC npc) {
        if (hasDebuff && Main.netMode != NetmodeID.Server) {
            int amt = Math.Max((int)(npc.Size.Length() / 32f), 1);
            foreach (var p in ModContent.GetInstance<LegacyBloomParticle>().NewMultipleReduced(amt)) {
                p.Location = Main.rand.NextVector2FromRectangle(npc.Hitbox);
                p.Velocity = -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
                p.Color = CorruptionHellfire.FireColor;
                p.BloomColor = CorruptionHellfire.BloomColor * 0.07f;
                p.Scale = Main.rand.NextFloat(1.5f, 3f);
                p.BloomScale = 0.3f;
                p.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                p.dontEmitLight = false;
                p.Frame = (byte)Main.rand.Next(3);
            }
        }
    }

    public override void UpdateLifeRegen(NPC npc, ref int damage) {
        if (!hasDebuff) {
            stacks = 0;
            return;
        }

        npc.lifeRegen -= CorruptionHellfire.BaseDamage + CorruptionHellfire.StackDamage * stacks;
        damage = CorruptionHellfire.BaseDamageNumber + CorruptionHellfire.StackDamageNumber * stacks;

        if (npc.oiled) {
            npc.lifeRegen -= 50;
        }
    }
}
