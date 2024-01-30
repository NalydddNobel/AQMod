using Aequus.Common.Buffs.Components;
using Aequus.Content.DataSets;
using Aequus.Core.DataSets;
using Aequus.Old.Content.Particles;
using System;
using Terraria.Audio;

namespace Aequus.Old.Content.StatusEffects.DamageOverTime;

public class CorruptionHellfire : ModBuff, IOnAddBuff {
    public static Int32 BaseDamage { get; set; } = 28;
    public static Int32 StackDamage { get; set; } = 2;
    public static Byte MaxStacks { get; set; } = 200;
    public static Int32 BaseDamageNumber { get; set; } = 14;
    public static Int32 StackDamageNumber { get; set; } = 1;
    public static Color FireColor { get; set; } = new Color(100, 28, 160, 60);
    public static Color BloomColor { get; set; } = new Color(20, 2, 80, 10);

    public override void SetStaticDefaults() {
        Main.debuff[Type] = true;
        Main.buffNoSave[Type] = true;
        BuffSets.DemonSiegeImmune.Add((BuffEntry)Type);
        BuffSets.PlayerDoTDebuff.Add((BuffEntry)Type);
    }

    public override Boolean ReApply(NPC npc, Int32 time, Int32 buffIndex) {
        if (npc.TryGetGlobalNPC(out CorruptionHellfireNPC corruptionHellfire)) {
            corruptionHellfire.stacks = (Byte)Math.Min(corruptionHellfire.stacks + 1, MaxStacks);
        }
        return false;
    }

    public override void Update(NPC npc, ref Int32 buffIndex) {
        if (npc.TryGetGlobalNPC(out CorruptionHellfireNPC corruptionHellfire)) {
            corruptionHellfire.stacks = Math.Max(corruptionHellfire.stacks, (Byte)1);
            corruptionHellfire.hasDebuff = true;
        }
    }

    public void PostAddBuff(NPC npc, Int32 duration, Boolean quiet) {
        if (npc.HasBuff<CorruptionHellfire>()) {
            SoundEngine.PlaySound(AequusSounds.InflictFireDebuff with { Pitch = -0.2f, PitchVariance = 0.1f }, npc.Center);
        }
    }
}

public class CorruptionHellfireNPC : GlobalNPC {
    public Boolean hasDebuff;
    public Byte stacks;

    public override Boolean InstancePerEntity => true;

    public override Boolean AppliesToEntity(NPC entity, Boolean lateInstantiation) {
        return !entity.buffImmune[ModContent.BuffType<CorruptionHellfire>()];
    }

    public override void ResetEffects(NPC npc) {
        hasDebuff = false;
    }

    public override void PostAI(NPC npc) {
        if (hasDebuff && Main.netMode != NetmodeID.Server) {
            Int32 amt = Math.Max((Int32)(npc.Size.Length() / 32f), 1);
            foreach (var p in ModContent.GetInstance<LegacyBloomParticle>().NewMultiple(amt)) {
                p.Location = Main.rand.NextVector2FromRectangle(npc.Hitbox);
                p.Velocity = -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
                p.Color = CorruptionHellfire.FireColor;
                p.BloomColor = CorruptionHellfire.BloomColor * 0.07f;
                p.Scale = Main.rand.NextFloat(1.5f, 3f);
                p.BloomScale = 0.3f;
                p.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                p.dontEmitLight = false;
                p.Frame = (Byte)Main.rand.Next(3);
            }
        }
    }

    public override void UpdateLifeRegen(NPC npc, ref Int32 damage) {
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
